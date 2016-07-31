using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Web.SessionState;
using Template.Business.Security;
using Template.Services.Config;
using Template.Services.Security;
using Template.Services.Support;

namespace Template.ServicesHost
{
    public class WebApiApplication : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
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

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            var cookieName = FormsAuthentication.FormsCookieName;
            HttpCookie authCookie = Request.Cookies[cookieName];

            if (authCookie == null)
                return;

            FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            var serializer = new JavaScriptSerializer();
            var userData = (IdentityModel)serializer.Deserialize(authTicket.UserData, typeof(IdentityModel));

            if (!userData.IsAuthenticated || !userData.IsActive)
            {
                Request.Cookies.Clear();
                return;
            }

            var identity = new CustomIdentity(userData.IsAuthenticated, userData.Name)
            { 
                IsActive = userData.IsActive,
                LastPasswordChange = userData.LastPasswordChange,
                CreatedOn = userData.CreatedOn,
                IsLockedOut = userData.IsLockedOut,
                Roles = userData.Roles,
                UserId = userData.UserId,
                Email = userData.Email,
                RememberMe = userData.RememberMe
            };

            var principal = new CustomPrincipal(identity);

            HttpContext.Current.User = principal;
            Thread.CurrentPrincipal = principal;

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var context = HttpContext.Current;
            var exception = context.Server.GetLastError();

            Logger.LogEvent("Global.asax", exception);
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}