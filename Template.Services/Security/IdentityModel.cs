using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template.Services.Security
{
    public class IdentityModel
    {
        public DateTime? LastPasswordChange { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsLockedOut { get; set; }
        public List<string> Roles { get; set; }
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public bool RememberMe { get; set; }
        public bool IsActive { get; set; }
        public bool IsAuthenticated { get; set; }
        public string Name { get; set; }
    }
}
