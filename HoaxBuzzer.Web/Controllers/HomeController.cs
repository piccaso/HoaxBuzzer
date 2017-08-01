using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using Dapper;
using HoaxBuzzer.Web.Helper;
using HoaxBuzzer.Web.Models;
using HoaxBuzzer.Web.Repositories;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;

namespace HoaxBuzzer.Web.Controllers
{


    public class HomeController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var config = new
            {
                ajaxSetVote = Url.Action("SetVote"),
                ajaxScreenNcontent = Url.Action("ScreenNcontent"),
                ajaxScreenScontent = Url.Action("ScreenScontent"),
                ajaxScreenOcontent = Url.Action("ScreenOcontent"),
                ajaxHeartBeat = Url.Action("HeartBeat"),
                voteDelay = AppSettings.Get<int>("VoteDelay"),
            };

            var jsonConfig = JToken.FromObject(config).ToString(Formatting.Indented);
            var configScript = $"<script id='config'>window.config = {jsonConfig};</script>";
            ViewBag.config = new MvcHtmlString(configScript);

            base.OnActionExecuting(filterContext);
        }

        private readonly string _connectionString = AppSettings.Get("DbConnectionString");

        public ActionResult HeartBeat()
        {
            object result;
            using (var db = new NpgsqlConnection(_connectionString))
            {
                var referrer = Request.UrlReferrer?.ToString();
                var ts = db.QueryFirst<DateTime>("SELECT now()");
                db.Close();
                result = new {dbNow = ts, aspNow = DateTime.Now, referrer};
            }

            Global.UseGlobalVotingLogic(l =>
            {
                l.PublishDebugMessage(result);
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SetVote(int? articleId, bool voteValue)
        {
            object result = null;
            Global.UseGlobalVotingLogic(l =>
            {
                result = new
                {
                    nextArticleId = l.SetVote(articleId, voteValue),
                };
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index() => View();
        public ActionResult Iframes() => View();

        public ActionResult ScreenN() => View();

        public ActionResult ScreenNcontent(int? articleId = null)
        {
            var model = new BevoreVoteScreenModel();
            using (var db = new NpgsqlConnection(_connectionString))
            {
                if (articleId.HasValue)
                {
                    model.Article = db.GetArticle(articleId.Value);
                }
                else
                {
                    model.Article = db.GetNextArticle();
                }
                db.Close();
            }
            Global.UseGlobalVotingLogic(l=>l.SetCurrentArticleId(model.Article.id));
            return View(model);
        }

        public FileResult Image(int imageId)
        {
            using (var db = new NpgsqlConnection(_connectionString))
            {
                var image = db.GetImage(imageId);
                db.Close();
                if (string.IsNullOrWhiteSpace(image.contentType))
                {
                    image.contentType = "application/octet-stream";
                }
                return new FileContentResult(image.data, image.contentType);
            }
        }

        private AfterVoteScreenModel GetAfterVoteScreenModel(int articleId, int voteId)
        {
            var model = new AfterVoteScreenModel();
            using (var db = new NpgsqlConnection(_connectionString))
            {
                model.VoteStatistics = db.GetStatisticsForVote(voteId);
                model.Article = db.GetArticle(articleId);
                db.Close();
            }
            return model;
        }

        public ActionResult ScreenS() => View();
        public ActionResult ScreenScontent(int articleId, int voteId) => View(GetAfterVoteScreenModel(articleId, voteId));

        public ActionResult ScreenO() => View();
        public ActionResult ScreenOcontent(int articleId, int voteId) => View(GetAfterVoteScreenModel(articleId, voteId));

        public ActionResult ScreenW() => View();

    }
}