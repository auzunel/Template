using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Template.Services.Fomatting
{
    public class SuppressedRequiredMemberSelector : IRequiredMemberSelector
    {
        public bool IsRequiredMember(MemberInfo member)
        {
            return false;
        }
    }
}
