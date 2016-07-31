using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Security;

namespace Template.WebApp.Support
{
    public class ServiceClient : IDisposable
    {
        private readonly CookieContainer _cookieContainer = new CookieContainer();
        private readonly HttpClient _client;
        private readonly HttpClientHandler _handler;

        public HttpClient Client 
        {
            get { return _client; }
        }

        public ServiceClient() : this(null) { }

        public ServiceClient(HttpCookie cookie) : base()
        {            
            _cookieContainer.MaxCookieSize = int.MaxValue;
          
            _handler = new HttpClientHandler();
            _handler.CookieContainer = _cookieContainer;

            _client = new HttpClient(_handler);
            _client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ServicesBaseUrl"]); 
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Add("X-CookieName", FormsAuthentication.FormsCookieName);

            if (cookie != null)
            {
                _cookieContainer.Add(_client.BaseAddress, new Cookie(cookie.Name, cookie.Value));
            }
        }

        public void GetCookie(string requestUrl)
        {
            var uri = new Uri(_client.BaseAddress + requestUrl);
            var responseCookies = _cookieContainer.GetCookies(uri).Cast<Cookie>();

            var authCookie = responseCookies.Where(x => x.Name == FormsAuthentication.FormsCookieName).FirstOrDefault();
            if (authCookie != null)
            {
                var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                var identity = authTicket.UserData;
            }
        }

        public void Dispose()
        {
            if (_client != null)
            {
                _client.Dispose();
            }

            if (_handler != null)
            {
                _handler.Dispose();
            }
        }
    }
}