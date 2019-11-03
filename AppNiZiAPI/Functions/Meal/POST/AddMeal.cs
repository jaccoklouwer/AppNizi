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

namespace AppNiZiAPI.Functions.Meal.POST
{
    public static class AddMeal
    {
        [FunctionName("AddMeal")]
        [OpenApiOperation("AddMeal", "Meal", Summary = "Adds a meal", Description = "Adds a meal to the specified patient", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseBody(HttpStatusCode.OK, "application/json", typeof(Models.Meal), Summary = Messages.OKUpdate)]
        [OpenApiResponseBody(HttpStatusCode.Unauthorized, "application/json", typeof(Error), Summary = Messages.AuthNoAcces)]
        [OpenApiResponseBody(HttpStatusCode.BadRequest, "application/json", typeof(Error), Summary = Messages.ErrorMissingValues)]
        [OpenApiRequestBody("application/json", typeof(Models.Meal), Description = "The meal object to be added")]
        [OpenApiParameter("patientId", Description = "The patient who is adding the meal", In = ParameterLocation.Path, Required = true, Type = typeof(int))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous,  "post", Route = (Routes.APIVersion+Routes.AddMeal))] HttpRequest req,
            ILogger log,int patientId)
        {
            #region AuthCheck
            AuthResultModel authResult = await DIContainer.Instance.GetService<IAuthorization>().CheckAuthorization(req, patientId);
            if (!authResult.Result)
                return new StatusCodeResult((int)authResult.StatusCode);
            #endregion


            Dictionary<ServiceDictionaryKey, object> dictionary = await DIContainer.Instance.GetService<IMealService>().TryAddMeal(patientId, req);


            return DIContainer.Instance.GetService<IResponseHandler>().ForgeResponse(dictionary);
        }
    }
}
