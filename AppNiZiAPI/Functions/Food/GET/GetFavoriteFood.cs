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

namespace AppNiZiAPI.Functions.Food
{
    public static class GetFavoriteFood
    {
        //[FunctionName("GetFavoriteFood")]
        //public static async Task<IActionResult> Run(
        //    [HttpTrigger(AuthorizationLevel.Function,  "post", Route = (Routes.APIVersion + Routes.FavoriteFood))] HttpRequest req,
        //    ILogger log, int patientId)
        //{
        //    //int patientId;
        //    try
        //    {
        //        //TODO je hebt vies en dan heb je wat ik hier doe moet op een christelijke manier 
                
        //    }
        //    catch (Exception)
        //    {
        //        return new BadRequestObjectResult(Messages.ErrorMissingValues);
        //    }
        //    //TODO maak dit minder lelijk
        //    List<Models.Food> food = new FoodRepository().Favorites(patientId);
        //    //TODO convert to JSON
        //    var jsonFood = JsonConvert.SerializeObject(food);

        //    return food != null
        //        ? (ActionResult)new OkObjectResult(food)
        //        : new BadRequestObjectResult(Messages.ErrorMissingValues);
        //}
    }
}
