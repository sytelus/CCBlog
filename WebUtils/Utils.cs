using System.Web.Script.Serialization;
using Microsoft.Security.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace WebUtils
{
    public static class Utils
    {
        public static IHtmlString Safe(this HtmlHelper html, string unsafeHtml)
        {
            var safeHtml = Sanitizer.GetSafeHtmlFragment(unsafeHtml);
            return MvcHtmlString.Create(safeHtml);
        }

        public static IHtmlString Raw(this HtmlHelper htmlHelper, string html)
        {
            return MvcHtmlString.Create(html);
        }

        public static IHtmlString Concat(this HtmlHelper htmlHelper, params IHtmlString[] strings)
        {
            var concat = string.Join<IHtmlString>("", strings);
            return MvcHtmlString.Create(concat);
        }

        public static IHtmlString Blank(this HtmlHelper htmlHelper)
        {
            return MvcHtmlString.Empty;
        }

        public static T FromJson<T>(string input)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Deserialize<T>(input);
        }

        public static string ToJson(object input)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(input);
        }
    }
}
