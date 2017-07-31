using System.Collections.Generic;
using System.Data;
using Dapper;

namespace HoaxBuzzer.Web.Repositories
{
    public static class Db
    {
        public static Entities.VoteStatistics GetStatisticsForVote(this IDbConnection db, int voteId)
        {
            return db.QueryFirst<Entities.VoteStatistics>("getStatisticsForVote", new {voteId}, commandType: CommandType.StoredProcedure);
        }

        public static int setVote(this IDbConnection db, Entities.Article article, bool voteValue) => setVote(db, article.id, voteValue);
        public static int setVote(this IDbConnection db, int articleId, bool voteValue)
        {
            return db.QueryFirst<int>("setVote", new {article_id = articleId, vote_value = voteValue}, commandType:CommandType.StoredProcedure);
        }

        public static int GetNextArticleId(this IDbConnection db)
        {
            return db.QueryFirst<int>("getNextArticleId", commandType: CommandType.StoredProcedure);
        }

        public static Entities.Article GetNextArticle(this IDbConnection db)
        {
            return db.QueryFirst<Entities.Article>("getNextArticle", commandType: CommandType.StoredProcedure);
        }

        public static Entities.Article GetArticle(this IDbConnection db, int articleId)
        {
            return db.QueryFirst<Entities.Article>("SELECT * FROM article WHERE id=@id", new {id = articleId});
        }


        public static Entities.ImageBase GetImage(this IDbConnection db, int imageId)
        {
            const string sql = "SELECT * FROM image i WHERE i.id=@imageId LIMIT 1";
            return db.QueryFirst<Entities.Image>(sql, new {imageId});
        }
    }
}