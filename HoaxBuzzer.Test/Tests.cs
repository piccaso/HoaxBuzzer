using System;
using System.Data;
using System.Linq;
using Dapper;
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
        public void PostgresDa1()
        {
            var connstr = "Server=horton.elephantsql.com;Port=5432;Database=axuxxckc;User Id=axuxxckc;Password=D-PDMFucL5YjDvHHdGCcUIS8J2WXtoIM;Search Path=db,public;";
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