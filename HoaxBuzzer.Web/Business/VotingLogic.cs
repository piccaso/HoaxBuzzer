using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using CoAP;
using HoaxBuzzer.Web.Helper;
using HoaxBuzzer.Web.Repositories;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace HoaxBuzzer.Web.Business
{
    public class VotingLogic
    {
        public readonly object Sync = new object();
        private MqttClient _mqttClient = null;
        private readonly Encoding _encoding = Encoding.UTF8;
        private Channels _channels = null;
        private readonly string _connectionString = AppSettings.Get("DbConnectionString");
        private int? _currentArticleId;
        private DateTime? _ignoreVotesUntil = null;

        public class Channels
        {
            public string Prefix { get; set; }
            public string Websocket { get; set; }

            public string ScreenN { get; set; }
            public string ScreenS { get; set; }
            public string ScreenW { get; set; }
            public string ScreenO { get; set; }

            public string OpenTriggerRelease { get; set; }

            public string Vote { get; set; }

            public string Debug { get; set; }
            public string Command { get; set; }
        }

        public Channels GetChannels()
        {
            GetMqttClient();
            return _channels;
        }


        public static IHtmlString GetJsonChannels()
        {
            var sb = new StringBuilder();
            Global.UseGlobalVotingLogic(l =>
            {
                var channels = l.GetChannels();
                var json = JToken.FromObject(channels).ToString(Formatting.Indented);
                sb.AppendLine("<script id='channels'>");
                sb.AppendLine($"window.channels = {json};");
                sb.AppendLine("</script>");
            });
            return new HtmlString(sb.ToString());
        }

        public VotingLogic()
        {
            GetMqttClient();
        }

        private MqttClient GetMqttClient()
        {
            if (_channels == null)
            {
                var prefix = AppSettings.Get("MqttPrefix").TrimEnd('/');
                var websocket = AppSettings.Get("MqttWebsocket");
                _channels = new Channels
                {
                    Prefix = prefix,
                    Websocket = websocket,
                    ScreenN = $"{prefix}/N",
                    ScreenS = $"{prefix}/S",
                    ScreenW = $"{prefix}/W",
                    ScreenO = $"{prefix}/O",
                    Vote = $"{prefix}/Vote",
                    Debug = $"{prefix}/Debug",
                    Command = $"{prefix}/Command",
                    OpenTriggerRelease = AppSettings.Get<string>("OpenTriggerTopic", null),
                };

            }
            if (_mqttClient == null || _mqttClient.IsConnected == false)
            {
                _mqttClient = MqttClientFactory.CreateConnectedClient();

                _mqttClient.MqttMsgSubscribed += (sender, args) =>
                {
                    var client = (MqttClient) sender;
                    var msg = new
                    {
                        @event = "MqttMsgSubscribed",
                        client.ClientId,
                        args.MessageId,
                        args.GrantedQoSLevels,
                    };
                    PublishMessage(_channels.Debug, msg);
                };

                _mqttClient.Subscribe(_channels.Vote);
                if (!string.IsNullOrWhiteSpace(_channels.OpenTriggerRelease)) _mqttClient.Subscribe(_channels.OpenTriggerRelease);
                
                _mqttClient.MqttMsgPublishReceived += (sender, args) =>
                {
                    var msg = JToken.Parse(_encoding.GetString(args.Message));
                    MessageReceived(args.Topic, msg, args.QosLevel, args.Retain, args.DupFlag);
                };
            }
            return _mqttClient;
        }

        public void PublishDebugMessage(object message)
        {
            try
            {
                PublishMessage(GetChannels().Debug, message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            
        }

        private void PublishMessage(string topic, object message, byte qosLevel = MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE,
            bool retain = false)
        {
            var client = GetMqttClient();
            var json = JToken.FromObject(message).ToString(Formatting.Indented);
            var binary = _encoding.GetBytes(json);
            client.Publish(topic, binary, qosLevel, retain);
        }

        private void MessageReceived(string topic, JToken message, byte qosLevel, bool retain, bool dupFlag)
        {
            try
            {
                if (topic == _channels.Vote)
                {
                    var articleId = message["articleId"]?.ToObject<int?>();
                    var voteValue = message["voteValue"].ToObject<bool>();
                    SetVote(articleId, voteValue);
                    return;
                }

                if (topic == _channels.OpenTriggerRelease && !string.IsNullOrWhiteSpace(_channels.OpenTriggerRelease))
                {
                    var uniqueIdentifier = message["UniqueIdentifier"].ToObject<string>();
                    SetVoteOpenTrigger(uniqueIdentifier);
                    return;
                }

                throw new Exception("oob topic: " + topic);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                PublishDebugMessage(e);
            }
        }

        private void SetVoteOpenTrigger(string uniqueIdentifier)
        {
            var matchFalse = AppSettings.Get("OpenTriggerMatchFalse");
            var matchTrue = AppSettings.Get("OpenTriggerMatchTrue");
            bool? vote = null;

            if (uniqueIdentifier.Contains(matchTrue)) vote = true;
            if (uniqueIdentifier.Contains(matchFalse)) vote = false;
            if (!vote.HasValue)
            {
                throw new Exception($"oob uid: {uniqueIdentifier}");
            }
            SetVote(null, vote.Value, uniqueIdentifier);

        }

        public int? SetVote(int? articleId, bool voteValue, string uniqueIdentifier = null)
        {
            lock (Sync)
            {
                if(_ignoreVotesUntil.HasValue && _ignoreVotesUntil.Value > DateTime.Now) return null;
                var delay = AppSettings.Get<int>("VoteDelay");
                _ignoreVotesUntil = DateTime.Now.AddSeconds(delay);
            }

            if (!articleId.HasValue) articleId = _currentArticleId;
            if (!articleId.HasValue) throw new Exception("articleId needed to set vote");

            int voteId, nextArticleId;
            Entities.VoteStatistics voteStatistics = null;
            using (var db = new NpgsqlConnection(_connectionString))
            {
                voteId = db.setVote(articleId.Value, voteValue);
                nextArticleId = db.GetNextArticleId();
                if (uniqueIdentifier != null)
                {
                    voteStatistics = db.GetStatisticsForVote(voteId);
                }
                db.Close();
            }

            // N: next question
            // S: prev question + answer statistics
            // O: prev question + answer statistics

            PublishMessage(_channels.ScreenN, new {articleId=nextArticleId});
            PublishMessage(_channels.ScreenS, new {articleId, voteId});
            PublishMessage(_channels.ScreenO, new {articleId, voteId});
            _currentArticleId = nextArticleId;

            if (uniqueIdentifier != null && voteStatistics != null)
            {
                var payloadConfigKey = voteStatistics.votedCorrect ? "OpenTriggerCorrectAnswerPayload" : "OpenTriggerWrongAnswerPayload";
                var payload = AppSettings.Get(payloadConfigKey);
                var uri = new Uri($"coap://{uniqueIdentifier}/led/RGB");
                var request = new Request(Method.PUT) {URI = uri, PayloadString = payload};
                request.Send();
            }

            return nextArticleId;
        }

        public void SetCurrentArticleId(int articleId)
        {
            _currentArticleId = articleId;
        }
    }
}