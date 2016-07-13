using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Template.Services.Config
{
    public class RouteConfig
    {
        public static void RegisterRoutes(HttpConfiguration config)
        {
            var routes = config.Routes;

            routes.MapHttpRoute("DefaultApiWithAction", "api/{controller}/{action}/{id}", new
            {
                id = RouteParameter.Optional
            });

            routes.MapHttpRoute("WebApiHttpRoute", "api/{controller}/{id}", new
            {
                controller = "Echo",
                id = RouteParameter.Optional
            });
        }
    }
}
