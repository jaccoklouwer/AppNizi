using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AppNiZiAPI.Models;
using AppNiZiAPI.Models.Repositories;
using System.Collections.Generic;
using AppNiZiAPI.Variables;
using AppNiZiAPI.Security;

using AppNiZiAPI.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;
using System.Net;
using Microsoft.OpenApi.Models;
using Aliencube.AzureFunctions.Extensions.OpenApi.Enums;

namespace AppNiZiAPI.Functions.Meal.GET
{
    public static class GetMeals
    {
        [FunctionName("GetMeals")]
        [OpenApiOperation("GetMeals", "Meal", Summary = "Retrieves meals", Description = "retrieves the meals of the specified user", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseBody(HttpStatusCode.OK, "application/json", typeof(string), Summary = Messages.OKUpdate)]
        [OpenApiResponseBody(HttpStatusCode.Unauthorized, "application/json", typeof(string), Summary = Messages.AuthNoAcces)]
        [OpenApiResponseBody(HttpStatusCode.BadRequest, "application/json", typeof(string), Summary = Messages.ErrorMissingValues)]
        [OpenApiParameter("patientId", Description = "The patient which meals will be retrieved", In = ParameterLocation.Path, Required = true, Type = typeof(int))]
        
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = (Routes.APIVersion+Routes.GetMeals))] HttpRequest req,
            ILogger log,int patientId)
        {
            // Auth check
            AuthResultModel authResult = await DIContainer.Instance.GetService<IAuthorization>().CheckAuthorization(req, patientId);
            if (!authResult.Result)
                return new StatusCodeResult(authResult.StatusCode);

            IMealRepository mealRepository = DIContainer.Instance.GetService<IMealRepository>();
            List<Models.Meal> meals = mealRepository.GetMyMeals(patientId);

            var jsonMeals = JsonConvert.SerializeObject(meals);
            return jsonMeals != null
                ? (ActionResult)new OkObjectResult(jsonMeals)
                : new BadRequestObjectResult(Messages.ErrorMissingValues);
        }
    }
}
