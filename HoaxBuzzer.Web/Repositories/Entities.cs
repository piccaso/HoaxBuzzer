// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace HoaxBuzzer.Web.Repositories
{
    public class Entities
    {
        public class VoteStatistics
        {
            public bool articleIsTrue { get; set; }
            public bool metricsTrue { get; set; }
            public bool votedCorrect { get; set; }
            public long sameVote { get; set; }
            public long allVotes { get; set; }
            
        }   
        public class Article
        {
            public int id { get; set; }
            public int fkGroup { get; set; }
            public string sourceUrl { get; set; }
            public bool articleIsTrue { get; set; }
            public int fkScreenshot { get; set; }
            public string heading { get; set; }
            public string summary { get; set; }
            public int? metricsAlteration { get; set; }
            public bool? metricsTrue { get; set; }
            public int? fbShares { get; set; }
            public int? fbLikes { get; set; }
            public int? fbComments { get; set; }
            public int? twRetweets { get; set; }
            public int? twLikes { get; set; }
        }
        public class ImageBase
        {
            public byte[] data { get; set; }
            public string contentType { get; set; }
        }
        
        public class Image : ImageBase
        {
            public int id { get; set; }
        }
    }
}