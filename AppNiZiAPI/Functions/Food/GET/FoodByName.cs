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
using System.Data.SqlClient;
using AppNiZiAPI.Models;
using AppNiZiAPI.Models.Repositories;
using System.Security.Claims;
using AppNiZiAPI.Security;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Headers;


namespace AppNiZiAPI
{
    public static class FoodByName
    {
        [FunctionName("Food")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = (Routes.APIVersion + Routes.FoodByName))] HttpRequest req,
            ILogger log, string foodName)
        {

            //if (!await Authorization.CheckAuthorization(req.Headers)) { return new BadRequestObjectResult(Messages.AuthNoAcces); }
            //TODO maak dit minder lelijk(iets minder lelijk nu maar wil graag van de specificatie models.food af)
            Food food = new FoodRepository().Select(foodName);

            var jsonFood = JsonConvert.SerializeObject(food);
            return jsonFood != null
                ? (ActionResult)new OkObjectResult(jsonFood)
                : new BadRequestObjectResult(Messages.ErrorMissingValues);
        }
    }
}
