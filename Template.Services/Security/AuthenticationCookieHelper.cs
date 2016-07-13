using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Template.Services.Security
{
    public class AuthenticationCookieHelper
    {
        public const string SeparatorString = "-";
        public const string SeparatorSplitString = "-";
        public const int ChunkSize = 1000;
        public const string RoleCookieExt = "_role";

        public static List<CookieHeaderValue> GetHeaderCookies(string cookieName, string encTicket)
        {
            var encoder = new System.Text.ASCIIEncoding();
            byte[] byteEncTicket = encoder.GetBytes(encTicket);
            string data, chunkCookieName;
            var cookieNames = new List<string>();
            var cookieChunks = new List<CookieHeaderValue>();

            int count = byteEncTicket.Length / ChunkSize;
            for (int i = 0; i < count; i++)
            {
                data = encoder.GetString(byteEncTicket.Skip(i * ChunkSize).Take(ChunkSize).ToArray());
                chunkCookieName = string.Format("{0}_chunk{1}", cookieName, i);
                cookieNames.Add(chunkCookieName);
                cookieChunks.Add(GetHeaderCookie(chunkCookieName, data));
            }

            data = encoder.GetString(byteEncTicket.Skip(count * ChunkSize).ToArray());
            chunkCookieName = string.Format("{0}_chunk{1}", cookieName, count);
            cookieNames.Add(chunkCookieName);
            cookieChunks.Add(GetHeaderCookie(chunkCookieName, data));

            var cookie = GetHeaderCookie(cookieName, string.Join(SeparatorString, cookieNames));

            cookieChunks.Add(cookie);

            return cookieChunks;
        }

        private static CookieHeaderValue GetHeaderCookie(string name, string value)
        {
            return new CookieHeaderValue(name, value)
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddYears(1000)
            };
        }
    }
}
