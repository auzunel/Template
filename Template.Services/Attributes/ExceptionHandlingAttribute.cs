using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace Template.Services.Attributes
{
    public class ExceptionHandlingAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            //if (context.Exception is ApplicationException)
            //{
            //    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
            //    {
            //        Content = new StringContent(context.Exception.Message),
            //        ReasonPhrase = "Exception"
            //    });

            //}

            //Log Critical errors
            //var logger = LogManager.GetCurrentClassLogger();
            //logger.Error(context.Exception.Message, context.Exception);
            //throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
            //{
            //    Content = new StringContent("An error occurred, please try again or contact the administrator."),
            //    ReasonPhrase = "Critical Exception"
            //});
        }
    }
}
