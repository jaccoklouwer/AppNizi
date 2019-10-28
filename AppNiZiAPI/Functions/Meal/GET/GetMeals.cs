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

namespace AppNiZiAPI.Functions.Meal.GET
{
    public static class GetMeals
    {
        //[FunctionName("GetMeals")]
        //public static async Task<IActionResult> Run(
        //    [HttpTrigger(AuthorizationLevel.Function, "get", Route = (Routes.APIVersion+Routes.GetMeals))] HttpRequest req,
        //    ILogger log,int patientId)
        //{
        //    if (!await Authorization.CheckAuthorization(req, patientId)) { return new BadRequestObjectResult(Messages.AuthNoAcces); }
        //    IMealRepository mealRepository = DIContainer.Instance.GetService<IMealRepository>();
        //    List<Models.Meal> meals = mealRepository.GetMyMeals(patientId);

        //    var jsonMeals = JsonConvert.SerializeObject(meals);
        //    return jsonMeals != null
        //        ? (ActionResult)new OkObjectResult(jsonMeals)
        //        : new BadRequestObjectResult(Messages.ErrorMissingValues);
        //}
    }
}
