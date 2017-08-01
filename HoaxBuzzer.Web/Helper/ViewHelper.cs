using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HoaxBuzzer.Web.Helper
{
    public static class ViewHelper
    {
        public static IHtmlString DumpJson(this object o) => new HtmlString(JToken.FromObject(o).ToString(Formatting.Indented));

        public static IHtmlString ToRawHtml(this string s) => new HtmlString(s);
    }
}