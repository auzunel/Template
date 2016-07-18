using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Template.ServiceClient
{
    public class AuthenticationCookieHelper
    {
        public const string SeparatorString = "-";
        public const string SeparatorSplitString = "-";
        public const int ChunkSize = 1000;
        public const string RoleCookieExt = "_role";

        public static List<HttpCookie> GetHttpCookies(string cookieName, string encTicket)
        {
            var encoder = new System.Text.ASCIIEncoding();
            var byteEncTicket = encoder.GetBytes(encTicket);

            string data, chunkCookieName;
            var cookieNames = new List<string>();
            var cookieChunks = new List<HttpCookie>();

            int count = byteEncTicket.Length / ChunkSize;
            for (int i = 0; i < count; i++)
            {
                data = encoder.GetString(byteEncTicket.Skip(count * ChunkSize).Take(ChunkSize).ToArray());
                chunkCookieName = string.Format("{0}_chunk{1}", cookieName, i);
                cookieNames.Add(chunkCookieName);
                cookieChunks.Add(GetHttpCookie(chunkCookieName, data));
            }

            data = encoder.GetString(byteEncTicket.Skip(count * ChunkSize).ToArray());
            chunkCookieName = string.Format("{0}_chunk{1}", cookieName, count);
            cookieNames.Add(chunkCookieName);
            cookieChunks.Add(GetHttpCookie(chunkCookieName, data));

            var cookie = GetHttpCookie(cookieName, string.Join(SeparatorString, cookieNames));

            cookieChunks.Add(cookie);

            return cookieChunks;
        }

        private static HttpCookie GetHttpCookie(string name, string value)
        {
            return new HttpCookie(name, value)
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddYears(1000)
            };
        }

        public static List<string> GetCookieNames(string cookieValue)
        {
            return cookieValue.Split(new string[] { SeparatorSplitString },
                StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public static string RebuildAuthTicket(IEnumerable<Cookie> responseCookies, Cookie chunkNameCookie)
        {
            var cookieNames = chunkNameCookie.Value.Split(new string[] { SeparatorSplitString }, StringSplitOptions.RemoveEmptyEntries);
            var authCookieValue = string.Empty;
            foreach (var cookieName in cookieNames)
            {
                var cookieChunk = responseCookies.FirstOrDefault(c => c.Name == cookieName);
                if (cookieChunk == null)
                {
                    return null;
                }
                authCookieValue += cookieChunk.Value;
            }

            return authCookieValue;
        }

        public static string RebuildAuthTicket(List<HttpCookie> responseCookies, HttpCookie chunkNameCookie)
        {
            var cookieNames = chunkNameCookie.Value.Split(new string[] { SeparatorSplitString }, StringSplitOptions.RemoveEmptyEntries);
            var authCookieValue = string.Empty;
            foreach (var cookieName in cookieNames)
            {
                var cookieChunk = responseCookies.FirstOrDefault(c => c.Name == cookieName);
                if (cookieChunk == null)
                {
                    return null;
                }
                authCookieValue += cookieChunk.Value;
            }

            return authCookieValue;
        }

    }
}
