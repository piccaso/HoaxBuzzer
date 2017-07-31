using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HoaxBuzzer.Web.Repositories;

namespace HoaxBuzzer.Web.Models
{
    public class BevoreVoteScreenModel
    {
        public Entities.Article Article { get; set; }
    }

    public class AfterVoteScreenModel : BevoreVoteScreenModel
    {
        public Entities.VoteStatistics VoteStatistics { get; set; }
    }
}