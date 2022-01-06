using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace VisaNet.VONRegister.Helpers.Html
{
    public static class HtmlExtensionMethods
    {
        public static MvcHtmlString EmailFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, Dictionary<string, object>  htmlAttributes)
        {
            var emailfor = htmlHelper.TextBoxFor(expression, htmlAttributes);
            return new MvcHtmlString(emailfor.ToHtmlString().Replace("type=\"text\"", "type=\"email\""));
        }
    }
}