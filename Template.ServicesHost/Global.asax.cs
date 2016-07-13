using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using System.Web.Script.Serialization;
using System.Web.Security;
using Template.Business.Security;
using Template.Services.Config;
using Template.Services.Security;

namespace Template.ServicesHost
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var config = GlobalConfiguration.Configuration;

            RouteConfig.RegisterRoutes(config);
            WebApiConfig.Configure(config);
            AutofacWebAPI.Initialize(config);
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var context = HttpContext.Current;
            var exception = context.Server.GetLastError();

            //var logger = LogManager.GetCurrentClassLogger();
            //logger.Error(exception.Message, exception);
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            //var cookieNameHeader = Request.Headers.Get("X-CookieName");
            var cookieNameHeader = FormsAuthentication.FormsCookieName;

            if (string.IsNullOrWhiteSpace(cookieNameHeader))
            {
                return;
            }

            var authCookie = Request.Cookies[cookieNameHeader];
            if (authCookie == null)
            {
                return;
            }

            var authTicket = FormsAuthentication.Decrypt(authCookie.Value);

            var principal = CreatePrincipalFromCookie(authTicket);

            if (principal == null)
            {
                return;
            }

            if (((CustomIdentity)principal.Identity).IsActive == false)
            {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }

            Context.User = principal;
        }

        private static CustomPrincipal CreatePrincipalFromCookie(FormsAuthenticationTicket ticket)
        {
            var serializer = new JavaScriptSerializer();

            Dictionary<string, object> userData = serializer.Deserialize<dynamic>(ticket.UserData);

            var identityValues = (Dictionary<string, object>)userData["Identity"];

            var name = identityValues["Name"].ToString();

            if (CustomPrincipal.Load(name))
            {
                return (CustomPrincipal)HttpContext.Current.User;
            }

            return null;
        }
    }
}
