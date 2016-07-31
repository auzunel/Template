using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Template.Business;
using Template.Business.Security;

namespace Template.Services.Security
{
    public class CustomPrincipal : IPrincipal
    {
        public IIdentity Identity { get; private set; }

        public CustomPrincipal(IIdentity identity)
        {
            this.Identity = identity;
        }

        public static bool Login(string userName, string password)
        {
            var user = UserManagement.Login(userName, password);
            return SetPrincipal(user);
        }

        public static bool Load(string email)
        {
            var user = UserManagement.Load(email);
            return SetPrincipal(user);
        }

        public bool IsInRole(string role)
        {
            return ((CustomIdentity)Identity).Roles.Any(r => r == role);
        }

        private static bool SetPrincipal(IIdentity identity)
        {
            if (identity.IsAuthenticated)
            {
                var principal = new CustomPrincipal(identity);

                //For HttpContext
                HttpContext.Current.User = principal;
                //For Current Thread
                Thread.CurrentPrincipal = principal;
            }
            return identity.IsAuthenticated;
        }
    }
}
