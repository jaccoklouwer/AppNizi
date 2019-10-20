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
    public static class FoodByPartialName
    {
        //[FunctionName("FoodByPartialName")]
        //public static async Task<IActionResult> Run(
        //    [HttpTrigger(AuthorizationLevel.Function, "get", Route = (Routes.APIVersion + Routes.FoodByPartialname))] HttpRequest req,
        //    ILogger log)
        //{
        //    //get foodname om te vinden uit query
        //    //generic queary handler gebruiken hier?TODO
        //    string foodname;
        //    try
        //    {
        //        foodname = req.Query["foodName"];
        //    }
        //    catch (Exception)
        //    {
        //        return new BadRequestObjectResult(Messages.ErrorMissingValues);
        //    }
        //    //TODO maak dit minder lelijk
        //    List<Models.Food> food = new FoodRepository().Search(foodname);
        //    //TODO convert to JSON
        //    var jsonFood = JsonConvert.SerializeObject(food);
        //    return food != null
        //        ? (ActionResult)new OkObjectResult(food)
        //        : new BadRequestObjectResult(Messages.ErrorMissingValues);
        //}
    }
}
