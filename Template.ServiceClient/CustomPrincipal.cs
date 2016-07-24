using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Template.ServiceClient
{
    public class CustomPrincipal : IPrincipal
    {
        public IIdentity Identity { get; set; }

        public bool IsInRole(string role)
        {
            return ((CustomIdentity)Identity).Roles.Any(r => r == role);
        }

        public string GetUserRoleType()
        {
            return ((CustomIdentity)Identity).UserRoleType;
        }
    }
}
