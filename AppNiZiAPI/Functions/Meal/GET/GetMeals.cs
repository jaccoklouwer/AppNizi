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

namespace AppNiZiAPI.Functions.Meal.GET
{
    public static class GetMeals
    {
        [FunctionName("GetMeals")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = (Routes.APIVersion+Routes.GetMeals))] HttpRequest req,
            ILogger log,int patientId)
        {
            List<Models.Meal> meals = new MealRepository().GetMyMeals(patientId);

            var jsonMeals = JsonConvert.SerializeObject(meals);
            return jsonMeals != null
                ? (ActionResult)new OkObjectResult(jsonMeals)
                : new BadRequestObjectResult(Messages.ErrorMissingValues);
        }
    }
}
