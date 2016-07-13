using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Template.WebApp.App_Start;

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


            //ServicesClient.CookieName = FormsAuthentication.FormsCookieName;/
            AntiForgeryConfig.CookieName = "__RequestVerificationDocsnapToken";
            ValueProviderFactories.Factories.Add(new JsonValueProviderFactory());
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            try
            {
                var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];

                //var roleCookie = Request.Cookies[FormsAuthentication.FormsCookieName + AuthenticationCookieHelper.RoleCookieExt];
                //if (authCookie == null || roleCookie == null)
                //{
                //    return;
                //}

                if (authCookie == null)
                    return;

                var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            }
            catch (Exception e)
            { }
        }
    }
}