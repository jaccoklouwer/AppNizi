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

namespace AppNiZiAPI.Functions.Food
{
    public static class GetFavoriteFood
    {
        [FunctionName("GetFavoriteFood")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function,  "get", Route = (Routes.APIVersion + Routes.GetFavoriteFood))] HttpRequest req,
            ILogger log, int patientId)
        {

            if (!await Authorization.CheckAuthorization(req, patientId)) { return new BadRequestObjectResult(Messages.AuthNoAcces); }
            //TODO maak dit minder lelijk

            IFoodRepository foodRepository = DIContainer.Instance.GetService<IFoodRepository>();
            List<Models.Food> food = foodRepository.Favorites(patientId);
            //TODO convert to JSON
            var jsonFood = JsonConvert.SerializeObject(food);

            return food != null
                ? (ActionResult)new OkObjectResult(food)
                : new BadRequestObjectResult(Messages.ErrorMissingValues);
        }
    }
}
