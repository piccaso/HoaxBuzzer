using System;
using System.Data;
using System.Linq;
using Dapper;
using HoaxBuzzer.Web.Helper;
using HoaxBuzzer.Web.Repositories;
using Npgsql;
using NUnit.Framework;

namespace HoaxBuzzer.Test
{

    public static class RandomExtension
    {
        public static bool NextBool(this Random r) => r.NextDouble() >= 0.5;
    }
    
    [TestFixture]
    public class Tests
    {
        [Test]
        public void AppSettings1()
        {
            var exCnt = 0;
            try
            {
                AppSettings.Get("does-not-exist-234");
            }
            catch (Exception)
            {
                exCnt++;
            }
            var guid = Guid.NewGuid().ToString();
            var fallback = AppSettings.Get("does-not-exist-456", guid);
            Assert.AreEqual(guid,fallback);
            Assert.AreEqual(1,exCnt);
        }
        
        [Test]
        public void PostgresDa1()
        {
            var connstr = AppSettings.Get("DbConnectionString");
            using (var db = new NpgsqlConnection(connstr))
            {
                var r = new Random();
                
                var articles = db.Query<Entities.Article>("select * from article").ToList();
                var images = articles.Select(a => db.GetImage(a.fkScreenshot)).ToList();

                var statistics = articles.Select(a =>
                {
                    var voteId = db.setVote(a, r.NextBool());
                    return db.getStatisticsForVote(voteId);
                }).ToList();

                for (var i = 0; i < articles.Count; i++)
                {
                    var a = db.GetNextArticle();
                    var voteId = db.setVote(a.id, r.NextBool());
                    var stat = db.getStatisticsForVote(voteId);
                    statistics.Add(stat);
                }
                
                db.Close();
            }
        }
    }
}