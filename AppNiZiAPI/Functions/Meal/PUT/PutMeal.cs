using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AppNiZiAPI.Models.Repositories;
using AppNiZiAPI.Variables;
using AppNiZiAPI.Security;
using AppNiZiAPI.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using AppNiZiAPI.Models;
using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;
using System.Net;
using Microsoft.OpenApi.Models;
using Aliencube.AzureFunctions.Extensions.OpenApi.Enums;
using System.Collections.Generic;
using AppNiZiAPI.Services;
using AppNiZiAPI.Services.Handlers;

namespace AppNiZiAPI.Functions.Meal.PUT
{
    public static class PutMeal
    {
        [FunctionName("PutMeal")]
        [OpenApiOperation("PutMeal", "Meal", Summary = "Adds a meal", Description = "Updates the specified meal", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseBody(HttpStatusCode.OK, "application/json", typeof(Models.Meal), Summary = Messages.OKUpdate)]
        [OpenApiResponseBody(HttpStatusCode.Unauthorized, "application/json", typeof(Error), Summary = Messages.AuthNoAcces)]
        [OpenApiResponseBody(HttpStatusCode.BadRequest, "application/json", typeof(Error), Summary = Messages.ErrorMissingValues)]
        [OpenApiRequestBody("application/json", typeof(Models.Meal), Description = "The meal object to be added")]
        [OpenApiParameter("patientId", Description = "The patient who is adding the meal", In = ParameterLocation.Path, Required = true, Type = typeof(int))]
        [OpenApiParameter("patientId", Description = "The patient who is adding the meal", In = ParameterLocation.Path, Required = true, Type = typeof(int))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = (Routes.APIVersion + Routes.PutMeal))] HttpRequest req,
            ILogger log,int patientId, int mealId)
        {
            // Auth check
            AuthResultModel authResult = await DIContainer.Instance.GetService<IAuthorization>().CheckAuthorization(req, patientId);
            if (!authResult.Result)
                return new StatusCodeResult((int)authResult.StatusCode);

            Dictionary<ServiceDictionaryKey, object> dictionary = await DIContainer.Instance.GetService<IMealService>().TryPutMeal(patientId,mealId,req);


            return DIContainer.Instance.GetService<IResponseHandler>().ForgeResponse(dictionary);
        }
    }
}
