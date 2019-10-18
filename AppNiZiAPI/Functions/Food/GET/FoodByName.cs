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

namespace AppNiZiAPI.Functions.FoodByName
{
    public static class FoodByName
    {
        [FunctionName("Food")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route =(Routes.APIVersion + Routes.FoodByName))] HttpRequest req,
            ILogger log)
        {
            //get foodname om te vinden uit query
            //generic queary handler gebruiken hier?TODO
            string foodname;
            try
            {
                foodname = req.Query["foodName"];
            }
            catch (Exception)
            {
                return new BadRequestObjectResult(Messages.ErrorMissingValues);
            }
            //TODO maak dit minder lelijk(iets minder lelijk nu maar wil graag van de specificatie models.food af)
            AppNiZiAPI.Models.Food food = new FoodRepository().Select(foodname);

            var jsonFood = JsonConvert.SerializeObject(food);
            return jsonFood != null
                ? (ActionResult)new OkObjectResult(jsonFood)
                : new BadRequestObjectResult(Messages.ErrorMissingValues);
        }
    }
}
