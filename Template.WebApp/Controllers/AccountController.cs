using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Template.WebApp.Attributes;
using Template.WebApp.Controllers.Base;
using Template.WebApp.Models;
using Template.WebApp.Models.Account;
using Template.WebApp.Support;

namespace Template.WebApp.Controllers
{
    public class AccountController : BaseController
    {
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                using (var servicesClient = new ServiceClient())
                {
                    model.CookieName = FormsAuthentication.FormsCookieName;

                    var postData = new ObjectContent(model.GetType(), model, new JsonMediaTypeFormatter());
                    var response = await servicesClient.Client.PostAsync(ServiceLinks.Login, postData);

                    if(response.StatusCode != HttpStatusCode.OK)
                    {
                        ViewBag.FailedResult = "Wrong Credentials";
                        return View();
                    }

                    var result = await response.Content.ReadAsAsync<LoginResultModel>();

                    if (!result.IsLoggedIn)
                    {
                        ViewBag.FailedResult = string.IsNullOrEmpty(result.Reason) ? "Wrong credentials" : result.Reason;
                        return View();
                    }

                    servicesClient.GetCookie(ServiceLinks.Login);

                    //var authTicket = servicesClient.GetAuthTicket(ServiceLinks.Login, response);
                    //var roleTicket = servicesClient.GetRoleTicket(ServiceLinks.Login, response);
                    //var customPrincipal = ServicesClient.CreatePrincipalFromCookie(authTicket, roleTicket);

                    //System.Web.HttpContext.Current.User = customPrincipal;

                    //Response.Cookies.Add(servicesClient.GetEncryptedAuthCookie(authTicket));
                    //var roleCookies = servicesClient.GetEncryptedRoleCookie(roleTicket);
                    //foreach (var cookie in roleCookies)
                    //{
                    //    Response.Cookies.Add(cookie);
                    //}
                }
            }

            return View();
        }
    }
}