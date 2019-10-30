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

namespace AppNiZiAPI.Functions.Food
{
    public static class FoodByPartialName
    {
        [FunctionName("FoodByPartialName")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = (Routes.APIVersion + Routes.FoodByPartialname))] HttpRequest req,
            ILogger log,int patientId, string foodName)
        {

            // Auth check
            AuthResultModel authResult = await DIContainer.Instance.GetService<IAuthorization>().CheckAuthorization(req, patientId);
            if (!authResult.Result)
                return new StatusCodeResult(authResult.StatusCode);

            //TODO maak dit minder lelijk

            IFoodRepository foodRepository = DIContainer.Instance.GetService<IFoodRepository>();
            List<Models.Food> food = foodRepository.Search(foodName);

            //TODO convert to JSON
            var jsonFood = JsonConvert.SerializeObject(food);
            return food != null
                ? (ActionResult)new OkObjectResult(food)
                : new BadRequestObjectResult(Messages.ErrorMissingValues);
        }
    }
}
