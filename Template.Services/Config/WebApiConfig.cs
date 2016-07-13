using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.Validation;
using System.Web.Http.Validation.Providers;
using Template.Services.Attributes;
using Template.Services.Fomatting;
using Template.Services.RequestCommands;

namespace Template.Services.Config
{
    public static class WebApiConfig
    {
        public static void Configure(HttpConfiguration config)
        {
            // Enable Errors - Shows unhandled error details(HttpResponseException)
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            //config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Never;

            // Enable CORS
            //config.MessageHandlers.Add(new CorsHandler());

            // Formatters - OnlyJsonConfigAttribute
            //Removed the unnecessary formatters that won’t be needed: JQueryMvcFormUrlEncodedFormatter and
            //FormUrlEncodedFormatter 
            var jqueryFormatter = config.Formatters.FirstOrDefault(x => x.GetType() == typeof(JQueryMvcFormUrlEncodedFormatter));
            config.Formatters.Remove(config.Formatters.FormUrlEncodedFormatter);
            config.Formatters.Remove(jqueryFormatter);

            //config.Formatters.XmlFormatter.SupportedMediaTypes.Clear(); -One way to remove XML Formatter

            //Camel case formatting for json
            var serializerSettings = config.Formatters.JsonFormatter.SerializerSettings;
            serializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            // Suppressing the IRequiredMemberSelector for all formatters
            foreach (var formatter in config.Formatters)
            {
                formatter.RequiredMemberSelector = new SuppressedRequiredMemberSelector();
            }

            //Default Services
            // If ExcludeMatchOnTypeOnly is true then we don't match on type only which means that we return null if we can't match on anything in the request. This is useful
            // for generating 406 (Not Acceptable) status codes.
            config.Services.Replace(typeof(IContentNegotiator), new DefaultContentNegotiator(excludeMatchOnTypeOnly: true));

            // Remove all the validation providers except for DataAnnotationsModelValidatorProvider - only Data Annotations validation functionality is needed.
            config.Services.RemoveAll(typeof(ModelValidatorProvider), validator => !(validator is DataAnnotationsModelValidatorProvider));

            // ParameterBindingRules
            // Any complex type parameter which is Assignable From IRequestCommand will be bound from the URI
            config.ParameterBindingRules.Insert(0, descriptor => typeof(IRequestCommand).IsAssignableFrom(descriptor.ParameterType)
                                                                     ? new FromUriAttribute().GetBinding(descriptor) : null);

        }

        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{action}/{id}", new { id = RouteParameter.Optional });
            config.Filters.Add(new ExceptionHandlingAttribute());
        }
    }
}
