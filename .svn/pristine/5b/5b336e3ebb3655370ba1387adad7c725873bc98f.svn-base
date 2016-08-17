using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;

namespace RSI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                "DefaultApiWithId", 
                "Api/{controller}/{id}", 
                new { id = RouteParameter.Optional }, 
                new { id = @"\d+" });

            config.Routes.MapHttpRoute(
                "DefaultApiWithAction", 
                "Api/{controller}/{action}");

            config.Routes.MapHttpRoute(
                "DefaultApiGet", 
                "Api/{controller}/", 
                new { action = "Get" }, 
                new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            config.Routes.MapHttpRoute(
                "DefaultApiPost", 
                "Api/{controller}/", 
                new { action = "Post" }, 
                new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
        }
    }
}