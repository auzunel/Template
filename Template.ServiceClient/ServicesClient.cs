using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace Template.ServiceClient
{
    public class ServicesClient : IDisposable
    {
        public static string CookieName { get; set; }

        private readonly CookieContainer _cookies = new CookieContainer();
        private readonly HttpClient _httpClient;
        private static readonly JavaScriptSerializer Serializer = new JavaScriptSerializer();

        public ServicesClient() : this(null)
        {
        }

        public ServicesClient(HttpCookie authCookie)
        {
            _cookies.MaxCookieSize = int.MaxValue;
            var handler = new HttpClientHandler
            {
                CookieContainer = _cookies
            };

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(ConfigurationManager.AppSettings["ServicesBaseUrl"])
            };

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("X-CookieName", CookieName);

            if (authCookie != null)
            {
                _cookies.Add(_httpClient.BaseAddress, new Cookie(authCookie.Name, authCookie.Value));
            }
        }

        public HttpClient Client
        {
            get { return _httpClient; }
        }

        public FormsAuthenticationTicket GetAuthTicket(string requestUrl, HttpResponseMessage response)
        {
            var uri = new Uri(_httpClient.BaseAddress + requestUrl);
            var responseCookies = _cookies.GetCookies(uri).Cast<Cookie>();

            var authCookie = responseCookies.FirstOrDefault(c => c.Name == CookieName);
            if (authCookie == null)
            {
                return null;
            }

            var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            if (authTicket == null)
            {
                return null;
            }

            var authRoleTicket = GetRoleTicket(requestUrl, response);
            if (authRoleTicket == null)
            {
                return null;
            }

            var principal = CreatePrincipalFromCookie(authTicket, authRoleTicket);

            var ticket = new FormsAuthenticationTicket(1, principal.Identity.Name, DateTime.UtcNow, DateTime.UtcNow.AddYears(1000), true, authTicket.UserData,
                FormsAuthentication.FormsCookiePath);

            return ticket;
        }

        public FormsAuthenticationTicket GetRoleTicket(string requestUrl, HttpResponseMessage response)
        {
            var uri = new Uri(_httpClient.BaseAddress + requestUrl);
            var responseCookies = _cookies.GetCookies(uri).Cast<Cookie>();

            var roleCookie = responseCookies.FirstOrDefault(c => c.Name == CookieName + AuthenticationCookieHelper.RoleCookieExt);
            if (roleCookie == null)
            {
                return null;
            }

            var roleCookieValue = AuthenticationCookieHelper.RebuildAuthTicket(responseCookies, roleCookie);

            return FormsAuthentication.Decrypt(roleCookieValue);
        }

        public HttpCookie GetEncryptedAuthCookie(FormsAuthenticationTicket ticket)
        {
            var encTicket = FormsAuthentication.Encrypt(ticket);

            return new HttpCookie(CookieName, encTicket);
        }

        public List<HttpCookie> GetEncryptedRoleCookie(FormsAuthenticationTicket ticket)
        {
            var encTicket = FormsAuthentication.Encrypt(ticket);
            return AuthenticationCookieHelper.GetHttpCookies(CookieName + AuthenticationCookieHelper.RoleCookieExt, encTicket);
        }

        public static CustomPrincipal CreatePrincipalFromCookie(FormsAuthenticationTicket ticket, FormsAuthenticationTicket roleTicket)
        {
            var additionalData = Serializer.Deserialize<dynamic>(ticket.UserData);

            Dictionary<string, object> userData = additionalData;

            var principal = new CustomPrincipal();

            var identityValues = (Dictionary<string, object>)userData["Identity"];

            var authenticationType = identityValues["AuthenticationType"].ToString();
            var isAuthenticated = (bool)identityValues["IsAuthenticated"];
            var name = identityValues["Name"].ToString();

            var lastPasswordChange = (DateTime?)identityValues["LastPasswordChange"];
            var createdOn = (DateTime)identityValues["CreatedOn"];
            var userId = (int?)identityValues["UserId"];
            var userRoleType = identityValues["UserRoleType"].ToString();
            var userRoleTypeId = (int?)identityValues["UserRoleTypeId"];
            var email = identityValues["Email"].ToString();
            var rememberMe = Boolean.Parse(identityValues["RememberMe"].ToString());

            var roles = new List<string>();

            if (roleTicket != null)
            {
                var roleData = Serializer.Deserialize<List<string>>(roleTicket.UserData);
                roles = roleData;
            }
            else
            {
                roles = ((object[])identityValues["Roles"]).ToList().Cast<string>().ToList();
            }

            var identity = new CustomIdentity(authenticationType, isAuthenticated, name)
            {
                CreatedOn = createdOn,
                Roles = roles,
                UserId = userId,
                LastPasswordChange = lastPasswordChange,
                UserRoleType = userRoleType,
                UserRoleTypeId = userRoleTypeId,
                Email = email,
                RememberMe = rememberMe
            };

            principal.Identity = identity;

            return principal;
        }

        public void Dispose()
        {
            if (_httpClient != null)
            {
                _httpClient.Dispose();
            }
        }
    }
}
