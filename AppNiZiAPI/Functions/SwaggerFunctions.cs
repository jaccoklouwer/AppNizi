using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;
using Aliencube.AzureFunctions.Extensions.OpenApi;
using System.Reflection;
using System.Net;

namespace AppNiZiAPI.Functions
{
    public static class SwaggerFunctions
    {
        [OpenApiIgnore]
        [FunctionName(nameof(RenderOpenApiDocument))]
        public static async Task<IActionResult> RenderOpenApiDocument(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "openapi/{version}.{extension}")] HttpRequest req,
        string version,
        string extension,
        ILogger log)
        {
            var ver = GetSpecVersion(version);
            var ext = GetExtension(extension);

            var settings = new AppSettings();
            var helper = new DocumentHelper();
            var document = new Document(helper);
            var result = await document.InitialiseDocument()
                                       .AddMetadata(settings.OpenApiInfo)
                                       .AddServer(req, settings.HttpSettings.RoutePrefix)
                                       .Build(Assembly.GetExecutingAssembly())
                                       .RenderAsync(ver, ext)
                                       .ConfigureAwait(false);
            var response = new ContentResult()
            {
                Content = result,
                ContentType = "application/json",
                StatusCode = (int)HttpStatusCode.OK
            };

            return response;
        }

        [FunctionName(nameof(RenderSwaggerUI))]
        [OpenApiIgnore]
        public static async Task<IActionResult> RenderSwaggerUI(
    [HttpTrigger(AuthorizationLevel.Function, "get", Route = "swagger/ui")] HttpRequest req,
    ILogger log)
        {
            var settings = new AppSettings();
            var ui = new SwaggerUI();
            var result = await ui.AddMetadata(settings.OpenApiInfo)
                                 .AddServer(req, settings.HttpSettings.RoutePrefix)
                                 .BuildAsync(typeof(SwaggerUI).Assembly)
                                 .RenderAsync("swagger.json", settings.SwaggerAuthKey)
                                 .ConfigureAwait(false);
            var response = new ContentResult()
            {
                Content = result,
                ContentType = "text/html",
                StatusCode = (int)HttpStatusCode.OK
            };

            return response;
        }
    }
}
