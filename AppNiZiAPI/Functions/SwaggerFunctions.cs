using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

using Aliencube.AzureFunctions.Extensions.OpenApi;
using Aliencube.AzureFunctions.Extensions.OpenApi.Abstractions;
using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;
using Aliencube.AzureFunctions.Extensions.OpenApi.Extensions;
using AppNiZiAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi;

using Newtonsoft.Json;

namespace SampleFunction
{
    public static class SwaggerFunctions
    {
        private static AppSettings settings = new AppSettings();
        private static IDocument doc = new Document(new DocumentHelper());
        private static ISwaggerUI swaggerUI = new SwaggerUI();

        [FunctionName(nameof(RenderOpenApiDocument))]
        [OpenApiIgnore]
        public static async Task<IActionResult> RenderOpenApiDocument(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "swagger/{version}.{extension}")] HttpRequest req,
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

        private static OpenApiFormat GetExtension(string extension)
        {
            if (extension.ToLower() == "json")
            {
                return OpenApiFormat.Json;
            }
            else {
                return OpenApiFormat.Yaml;
            }
        }

        private static OpenApiSpecVersion GetSpecVersion(string version)
        {
            if (version.Contains("2"))
            {
                return OpenApiSpecVersion.OpenApi2_0;
            }
            else {
                return OpenApiSpecVersion.OpenApi3_0;
            }
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
                                 .BuildAsync()
                                 .RenderAsync("swagger/v3.json", settings.SwaggerAuthKey)
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