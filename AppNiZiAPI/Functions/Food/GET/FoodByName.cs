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

using AppNiZiAPI.Infrastructure;
using Microsoft.Extensions.DependencyInjection;


namespace AppNiZiAPI
{
    public static class FoodByName
    {
        [FunctionName("Food")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = (Routes.APIVersion + Routes.FoodById))] HttpRequest req,
            ILogger log,int patientId, int foodId)
        {

            #region AuthCheck
            AuthResultModel authResult = await DIContainer.Instance.GetService<IAuthorization>().CheckAuthorization(req, patientId);
            if (!authResult.Result)
                return new StatusCodeResult(authResult.StatusCode);
            #endregion

            IFoodRepository foodRepository = DIContainer.Instance.GetService<IFoodRepository>();
            //FoodRepository foodRepository = new FoodRepository();
            Food food = foodRepository.Select(foodId);

            var jsonFood = JsonConvert.SerializeObject(food);
            return jsonFood != null
                ? (ActionResult)new OkObjectResult(jsonFood)
                : new BadRequestObjectResult(Messages.ErrorMissingValues);
        }
    }
}
