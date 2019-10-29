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

using AppNiZiAPI.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace AppNiZiAPI.Functions.Food
{
    public static class FavoriteFood
    {
        [FunctionName("PostFavoriteFood")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function,  "post", Route = (Routes.APIVersion + Routes.PostFavoriteFood))] HttpRequest req,
            ILogger log)
        {
            int foodId;
            int patientId;
          
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

            if (!await Authorization.CheckAuthorization(req, patientId)) { return new BadRequestObjectResult(Messages.AuthNoAcces); }

            IFoodRepository foodRepository = DIContainer.Instance.GetService<IFoodRepository>();

            bool succes = foodRepository.Favorite(patientId,foodId);

            return succes != false
                ? (ActionResult)new OkObjectResult($"alles is super sexy en je hebt een fav gedaan")
                : new BadRequestObjectResult("oopsiewoopsie er is een fout begaan banaan");
        }
    }
}
