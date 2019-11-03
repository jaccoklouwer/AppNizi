using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AppNiZiAPI.Variables;
using AppNiZiAPI.Models.Repositories;
using System.Collections.Generic;
using AppNiZiAPI.Security;

using AppNiZiAPI.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using AppNiZiAPI.Models;
using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;
using System.Net;
using Microsoft.OpenApi.Models;
using Aliencube.AzureFunctions.Extensions.OpenApi.Enums;
using AppNiZiAPI.Services.Handlers;
using AppNiZiAPI.Services;

namespace AppNiZiAPI.Functions.Food
{
    public static class FoodByPartialName
    {
        [FunctionName("FoodByPartialName")]
        [OpenApiOperation("SearchFood", "Food", Summary = "Searches the FoodDB", Description = "Allows you to search the food Database and gets items that start with the supplied letters", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseBody(HttpStatusCode.OK, "application/json", typeof(Models.Food[]), Summary = Messages.OKUpdate)]
        [OpenApiResponseBody(HttpStatusCode.Unauthorized, "application/json", typeof(Error), Summary = Messages.AuthNoAcces)]
        [OpenApiResponseBody(HttpStatusCode.BadRequest, "application/json", typeof(Error), Summary = Messages.ErrorMissingValues)]
        [OpenApiParameter("foodName", Description = "The letters to use in the search", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter("count", Description = "amount of items to retrieve", In = ParameterLocation.Path, Required = true, Type = typeof(int))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = (Routes.APIVersion + Routes.FoodByPartialname))] HttpRequest req,
            ILogger log,string foodName, int count)
        {
            int patientId = await DIContainer.Instance.GetService<IAuthorization>().GetUserId(req);
            // Auth check
            AuthResultModel authResult = await DIContainer.Instance.GetService<IAuthorization>().CheckAuthorization(req, patientId);
            if (!authResult.Result)
                return new StatusCodeResult((int)authResult.StatusCode);

            Dictionary<ServiceDictionaryKey, object> dictionary = await DIContainer.Instance.GetService<IFoodService>().TryGetFoodBySearch(foodName,count);


            return DIContainer.Instance.GetService<IResponseHandler>().ForgeResponse(dictionary);
        }
    }
}
