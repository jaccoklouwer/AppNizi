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
using AppNiZiAPI.Models.Handlers;
using AppNiZiAPI.Variables;
using AppNiZiAPI.Security;


using AppNiZiAPI.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using AppNiZiAPI.Models;
using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;
using System.Net;
using Aliencube.AzureFunctions.Extensions.OpenApi.Enums;
using Microsoft.OpenApi.Models;
using AppNiZiAPI.Services;
using System.Collections.Generic;
using AppNiZiAPI.Services.Handlers;

namespace AppNiZiAPI.Functions.Meal.DELETE
{
    public static class DeleteMeal
    {
        [FunctionName("DeleteMeal")]
        [OpenApiOperation("DeleteMeal", "Meal", Summary = "Deletes a meal", Description = "Removes the meal of the specified user", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseBody(HttpStatusCode.OK, "application/json", typeof(string), Summary = Messages.OKUpdate)]
        [OpenApiResponseBody(HttpStatusCode.Unauthorized, "application/json", typeof(Error), Summary = Messages.AuthNoAcces)]
        [OpenApiResponseBody(HttpStatusCode.BadRequest, "application/json", typeof(Error), Summary = Messages.ErrorMissingValues)]
        [OpenApiParameter("patientId", Description = "the id of the patient thats going to delete his meal", In = ParameterLocation.Query, Required = true, Type = typeof(int))]
        [OpenApiParameter("mealId", Description = "The meal to delete", In = ParameterLocation.Query, Required = true, Type = typeof(int))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", "post", Route = ( Routes.APIVersion + Routes.DeleteMeal))] HttpRequest req,
            ILogger log)
        {
            int patientId = await DIContainer.Instance.GetService<IAuthorization>().GetUserId(req);
            // Auth check
            AuthResultModel authResult = await DIContainer.Instance.GetService<IAuthorization>().CheckAuthorization(req, patientId);
            if (!authResult.Result)
                return new StatusCodeResult((int)authResult.StatusCode);

            Dictionary<ServiceDictionaryKey, object> dictionary = await DIContainer.Instance.GetService<IMealService>().TryDeleteMeal(req);


            return DIContainer.Instance.GetService<IResponseHandler>().ForgeResponse(dictionary);
        }
    }
}
