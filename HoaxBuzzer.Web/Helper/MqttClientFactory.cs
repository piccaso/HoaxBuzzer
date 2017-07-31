using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace HoaxBuzzer.Web.Helper
{
    public static class MqttClientFactory
    {
        public static MqttClient CreateConnectedClient(Uri connection, string clientId = null)
        {
            var uri = new UriBuilder(connection);
            var validSchemas = new[] { "tcp", "mqtt" };
            if (!validSchemas.Contains(uri.Scheme)) throw new InvalidProgramException($"Scheme: {uri.Scheme} is not supported");
            var port = uri.Port > 0 ? uri.Port : 1883;
            var client = new MqttClient(uri.Host, port, false, null, null, MqttSslProtocols.None);
            if (string.IsNullOrWhiteSpace(clientId)) clientId = Guid.NewGuid().ToString();
            if (!string.IsNullOrWhiteSpace(uri.Uri.UserInfo))
            {
                var username = uri.UserName;
                var password = uri.Password;
                client.Connect(clientId, username, password);
            }
            else
            {
                client.Connect(clientId);
            }
            return client;
        }

        public static MqttClient CreateConnectedClient()
        {
            var connection = AppSettings.Get<Uri>("MqttConnection");
            return CreateConnectedClient(connection);
        }

        public static ushort Subscribe(this MqttClient client, params string[] topics)
        {
            return client.Subscribe(topics, new[] {MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE});
        }

    }
}