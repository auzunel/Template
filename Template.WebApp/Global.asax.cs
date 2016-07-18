using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using Template.ServiceClient;
using Template.WebApp.App_Start;
using Template.WebApp.Support;

namespace Template.WebApp
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            IoCConfig.RegisterDependencies();

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            AntiForgeryConfig.CookieName = "__RequestVerificationTemplateToken";
            ServicesClient.CookieName = FormsAuthentication.FormsCookieName;
            ValueProviderFactories.Factories.Add(new JsonValueProviderFactory());

            //Logger.LogEvent("Application_Start");
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            // If there is a problem; logout, clear session and cookies
            try
            {
                var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                var roleCookie = Request.Cookies[FormsAuthentication.FormsCookieName + AuthenticationCookieHelper.RoleCookieExt];
                if (authCookie == null || roleCookie == null)
                {
                    return;
                }

                var authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                var cookieNameList = Request.Cookies.Keys;
                var cookieList = new List<HttpCookie>();
                foreach (string cookieName in cookieNameList)
                {
                    cookieList.Add(Request.Cookies[cookieName]);
                }

                var roleTicket = AuthenticationCookieHelper.RebuildAuthTicket(cookieList, roleCookie);
                var encyRoleTicket = FormsAuthentication.Decrypt(roleTicket);


                Context.User = ServicesClient.CreatePrincipalFromCookie(authTicket, encyRoleTicket);
            }
            catch (Exception exception)
            {
                Logger.LogEvent(exception.Message, exception);
                FormsAuthentication.SignOut();

                var session = HttpContext.Current.Session;

                if (session != null)
                {
                    Session.Abandon();
                }
                string[] cookies = Request.Cookies.AllKeys;
                foreach (var cookieName in cookies)
                {
                    Response.Cookies[cookieName].Expires = DateTime.Now.AddDays(-1);
                }
            }
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
                    Logger.LogEvent(exception.Message, exception);
                    Response.Redirect(u.Action("Error500", "Error"));
                    break;
            }
        }
    }
}