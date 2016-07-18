using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Template.WebApp.Support;

namespace Template.WebApp.Controllers.Base
{
    public class BaseController : Controller
    {
        protected override void OnException(ExceptionContext filterContext)
        {
            Logger.LogEvent(filterContext.Exception.Message, filterContext.Exception);
            base.OnException(filterContext);
        }
    }
}