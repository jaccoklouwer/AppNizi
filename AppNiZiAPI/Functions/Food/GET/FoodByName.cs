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
using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;
using System.Net;
using Microsoft.OpenApi.Models;
using Aliencube.AzureFunctions.Extensions.OpenApi.Enums;

namespace AppNiZiAPI
{
    public static class FoodByName
    {
        [FunctionName("Food")]
        [OpenApiOperation("GetFoodById", "Food", Summary = "Gets the requested FoodItem", Description = "updates the dietary management of a patient", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseBody(HttpStatusCode.OK, "application/json", typeof(string), Summary = Messages.OKUpdate)]
        [OpenApiResponseBody(HttpStatusCode.Unauthorized, "application/json", typeof(string), Summary = Messages.AuthNoAcces)]
        [OpenApiResponseBody(HttpStatusCode.BadRequest, "application/json", typeof(string), Summary = Messages.ErrorMissingValues)]
        [OpenApiParameter("patientId", Description = "the id of the patient thats going to get the item", In = ParameterLocation.Path, Required = true, Type = typeof(int))]
        [OpenApiParameter("foodId", Description = "the id of food item to be retrieved", In = ParameterLocation.Path, Required = true, Type = typeof(int))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = (Routes.APIVersion + Routes.FoodById))] HttpRequest req,
            ILogger log,int patientId, int foodId)
        {

            #region AuthCheck
            AuthResultModel authResult = await DIContainer.Instance.GetService<IAuthorization>().CheckAuthorization(req, patientId);
            if (!authResult.Result)
                return new StatusCodeResult((int)authResult.StatusCode);
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
