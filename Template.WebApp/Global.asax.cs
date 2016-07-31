using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using Template.WebApp.App_Start;
using Template.WebApp.Support;

namespace Template.WebApp
{
    public class MvcApplication : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            IoCConfig.RegisterDependencies();

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AntiForgeryConfig.CookieName = "__RequestVerificationTemplateToken";
            ValueProviderFactories.Factories.Add(new JsonValueProviderFactory());
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var context = HttpContext.Current;
            var exception = context.Server.GetLastError();

            var cryptoEx = exception as CryptographicException;
            if (cryptoEx != null)
            {
                string[] myCookies = Request.Cookies.AllKeys;
                foreach (string cookie in myCookies)
                {
                    Response.Cookies[cookie].Expires = DateTime.Now.AddDays(-1);
                }
                Server.ClearError();
            }

            var u = new UrlHelper(HttpContext.Current.Request.RequestContext);

            var ex = exception as HttpException;

            if (ex == null)
            {
                var invalidCast = exception as System.InvalidCastException;
                if (null != invalidCast)
                {
                    Response.Redirect(u.Action("Error401", "Error"));
                }
                else
                {
                    Response.Redirect(u.Action("Error500", "Error"));
                }
                return; ;
            }

            var errorCode = ex.GetHttpCode();

            switch (errorCode)
            {
                case 404:
                    Response.Redirect(u.Action("Error404", "Error"));
                    break;
                case 401:
                    Response.Redirect(u.Action("Error401", "Error"));
                    break;
                default:
                    Logger.LogEvent("Global.asax", ex);
                    Response.Redirect(u.Action("Error500", "Error"));
                    break;
            }
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}