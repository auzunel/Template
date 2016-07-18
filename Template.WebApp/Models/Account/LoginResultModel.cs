using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Template.WebApp.Models.Account
{
    public class LoginResultModel
    {
        public bool IsLoggedIn { get; set; }
        public string Reason { get; set; }
    }
}