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
using AppNiZiAPI.Security;

namespace AppNiZiAPI.Functions.Food
{
    public static class FavoriteFood
    {
        [FunctionName("PostFavoriteFood")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function,  "get", Route = (Routes.APIVersion + Routes.PostFavoriteFood))] HttpRequest req,
            ILogger log)
        {
            //if (!await Authorization.CheckAuthorization(req.Headers)) { return new BadRequestObjectResult(Messages.AuthNoAcces); }
            int foodId= 1;
            int patientId = 3;
            try
            {
                //TODO haal patient id op een coole manier op
                foodId = Convert.ToInt32(req.Query["foodId"].ToString());
                patientId = Convert.ToInt32(req.Query["patientId"].ToString());
            }
            catch (Exception)
            {
                return new BadRequestObjectResult(Messages.ErrorMissingValues);
            }

            bool succes = new FoodRepository().Favorite(patientId,foodId);

            return succes != null
                ? (ActionResult)new OkObjectResult($"alles is super sexy en je hebt een fav gedaan")
                : new BadRequestObjectResult("oopsiewoopsie er is een fout begaan banaan");
        }
    }
}
