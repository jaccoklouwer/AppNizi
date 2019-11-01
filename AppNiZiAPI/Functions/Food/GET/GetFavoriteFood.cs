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
using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;
using System.Net;
using Microsoft.OpenApi.Models;
using Aliencube.AzureFunctions.Extensions.OpenApi.Enums;

namespace AppNiZiAPI.Functions.Food
{
    public static class GetFavoriteFood
    {
        [FunctionName("GetFavoriteFood")]
        [OpenApiOperation("GetFavoriteFood", "Food", Summary = "Gets the users favorite foods", Description = "Retrieves the favorited fooditems of the specified patientID", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseBody(HttpStatusCode.OK, "application/json", typeof(string), Summary = Messages.OKUpdate)]
        [OpenApiResponseBody(HttpStatusCode.Unauthorized, "application/json", typeof(string), Summary = Messages.AuthNoAcces)]
        [OpenApiResponseBody(HttpStatusCode.BadRequest, "application/json", typeof(string), Summary = Messages.ErrorMissingValues)]
        [OpenApiParameter("patientId", Description = "the id of the patient thats going to get the item", In = ParameterLocation.Path, Required = true, Type = typeof(int))]
        
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous,  "get", Route = (Routes.APIVersion + Routes.GetFavoriteFood))] HttpRequest req,
            ILogger log, int patientId)
        {

            // Auth check
            AuthResultModel authResult = await DIContainer.Instance.GetService<IAuthorization>().CheckAuthorization(req, patientId);
            if (!authResult.Result)
                return new StatusCodeResult((int)authResult.StatusCode);
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
