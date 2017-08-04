using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages;
using HoaxBuzzer.Web.Repositories;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HoaxBuzzer.Web.Helper
{
    public static class ViewHelper
    {
        public static IHtmlString DumpJson(this object o) => new HtmlString(JToken.FromObject(o).ToString(Formatting.Indented));

        public static IHtmlString ToRawHtml(this string s) => new HtmlString(s);

        public static RightAndWrong GetRightAndWrong(this Entities.VoteStatistics s)
        {
            var r = new RightAndWrong();
            var notSameVote = s.allVotes - s.sameVote;

            if (s.votedCorrect)
            {
                r.Right = s.sameVote;
                r.Wrong = notSameVote;
            }
            else
            {
                r.Wrong = s.sameVote;
                r.Right = notSameVote;
            }
            return r;
        }
    }

    public class RightAndWrong
    {
        public long Right { get; set; }
        public long Wrong { get; set; }
    }
}