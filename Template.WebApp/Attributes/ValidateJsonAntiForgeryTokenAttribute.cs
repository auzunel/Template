using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Template.WebApp.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ValidateJsonAntiForgeryTokenAttribute :FilterAttribute, IAuthorizationFilter
    {
        public string Salt { get; set; }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            var httpContext = new JsonAntiForgeryHttpContextWrapper(HttpContext.Current);
            var httpCookie = httpContext.Request.Cookies.Get("__RequestVerificationToken");
            if (httpCookie != null)
            {
                AntiForgery.Validate(httpCookie.Value, httpContext.Request.Form["__RequestVerificationToken"]);
            }
        }

        private class JsonAntiForgeryHttpContextWrapper : HttpContextWrapper
        {
            private readonly HttpRequestBase _request;

            public JsonAntiForgeryHttpContextWrapper(HttpContext httpContext)
                : base(httpContext)
            {
                _request = new JsonAntiForgeryHttpRequestWrapper(httpContext.Request);
            }

            public override HttpRequestBase Request
            {
                get { return _request; }
            }
        }

        private class JsonAntiForgeryHttpRequestWrapper : HttpRequestWrapper
        {
            private readonly NameValueCollection _form;

            public JsonAntiForgeryHttpRequestWrapper(HttpRequest request)
                : base(request)
            {
                _form = new NameValueCollection(request.Form);
                if (request.Headers["__RequestVerificationToken"] != null)
                {
                    _form["__RequestVerificationToken"] = request.Headers["__RequestVerificationToken"];
                }
            }

            public override NameValueCollection Form
            {
                get { return _form; }
            }
        }
    }
}