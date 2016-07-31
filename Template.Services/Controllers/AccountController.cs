using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using System.Web.Security;
using Template.Business.Security;
using Template.Services.RequestModels;
using Template.Services.Security;

namespace Template.Services.Controllers
{
    public class AccountController : ApiController
    {

        //[HttpGet]
        //[ActionName("Login")]
        //public HttpResponseMessage Login()
        [HttpPost]
        [ActionName("Login")]
        public HttpResponseMessage Login(LoginRequestModel model)
        {
            try
            {
                //var model = new LoginRequestModel
                //{
                //    Email = "auzunel@HOtmail.com",
                //    Password = "1234",
                //    CookieName = FormsAuthentication.FormsCookieName
                //};

                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }

                if (CustomPrincipal.Login(model.Email, model.Password))
                {
                    var serializer = new JavaScriptSerializer();

                    //var userRoleData = serializer.Serialize(((CustomIdentity)HttpContext.Current.User.Identity).Roles);
                    var userData = serializer.Serialize(new
                    {
                        ((CustomIdentity)HttpContext.Current.User.Identity).AuthenticationType,
                        ((CustomIdentity)HttpContext.Current.User.Identity).IsAuthenticated,
                        ((CustomIdentity)HttpContext.Current.User.Identity).LastPasswordChange,
                        ((CustomIdentity)HttpContext.Current.User.Identity).CreatedOn,
                        ((CustomIdentity)HttpContext.Current.User.Identity).UserId,
                        ((CustomIdentity)HttpContext.Current.User.Identity).Name,     
                        ((CustomIdentity)HttpContext.Current.User.Identity).Email,
                        ((CustomIdentity)HttpContext.Current.User.Identity).RememberMe,
                        ((CustomIdentity)HttpContext.Current.User.Identity).Roles,
                        ((CustomIdentity)HttpContext.Current.User.Identity).IsActive
                    });

                    // isPersistent = true, remember me function
                    var ticket = new FormsAuthenticationTicket(1, model.Email, DateTime.UtcNow, DateTime.UtcNow.AddYears(1000), true, userData, FormsAuthentication.FormsCookiePath);
                    //var roleTicket = new FormsAuthenticationTicket(2, model.Email, DateTime.UtcNow, DateTime.UtcNow.AddYears(1000), true, userRoleData, FormsAuthentication.FormsCookiePath);

                    var encTicket = FormsAuthentication.Encrypt(ticket);
                    //var roleEncTicket = FormsAuthentication.Encrypt(roleTicket);

                    var response = Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        IsLoggedIn = true,
                        Reason = string.Empty
                    });

                    //var expirationDate = model.RememberMe
                    //    ? DateTime.UtcNow.AddYears(1000)
                    //    : DateTime.UtcNow.AddHours(Convert.ToDouble(ConfigurationManager.AppSettings["CookieExpiration"]));

                    var cookie = new CookieHeaderValue(model.CookieName, encTicket)
                    {
                        HttpOnly = true,
                        Expires = DateTime.UtcNow.AddYears(1000)
                        //Expires = expirationDate

                    };

                    response.Headers.AddCookies(new List<CookieHeaderValue> { cookie });

                    //Role cookies - is it necessary?
                    //var roleCookies = AuthenticationCookieHelper.GetHeaderCookies(model.CookieName + AuthenticationCookieHelper.RoleCookieExt, roleEncTicket);
                    //foreach (var roleCookie in roleCookies)
                    //{
                    //    response.Headers.AddCookies(new List<CookieHeaderValue> { roleCookie });
                    //}

                    return response;
                }

                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Invalid username/password.");
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message);
            }
        }

        [HttpGet]
        [ActionName("Register")]
        public HttpResponseMessage Register()
        {
            UserManagement.Register("auzunel@hotmail.com", "1234");
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [Authorize(Roles="Admin")]
        [HttpGet]
        public HttpResponseMessage Demo()
        {
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
