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
using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;
using System.Net;
using Microsoft.OpenApi.Models;
using AppNiZiAPI.Models;
using AppNiZiAPI.Models.Repositories;
using AppNiZiAPI.Infrastructure;
using AppNiZiAPI.Security;
using Microsoft.Extensions.DependencyInjection;
using Aliencube.AzureFunctions.Extensions.OpenApi.Enums;

namespace AppNiZiAPI.Functions.Food.DELETE
{
    public static class UnFavoriteFood
    {
        [FunctionName("UnFavoriteFood")]
        [OpenApiOperation("Delete Favorite Food", "Food", Summary = "Unfavorites the fooditem", Description = "Deletes the connection between the patient and this fooditem in essence unfavoriting it", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseBody(HttpStatusCode.OK, "application/json", typeof(string), Summary = Messages.OKUpdate)]
        [OpenApiResponseBody(HttpStatusCode.Unauthorized, "application/json", typeof(string), Summary = Messages.AuthNoAcces)]
        [OpenApiResponseBody(HttpStatusCode.BadRequest, "application/json", typeof(string), Summary = Messages.ErrorMissingValues)]
        [OpenApiParameter("patientId", Description = "the id of the patient thats unfavoriting the food", In = ParameterLocation.Query, Required = true, Type = typeof(int))]
        [OpenApiParameter("foodId", Description = "the fooditem to be unfavorited", In = ParameterLocation.Query, Required = true, Type = typeof(int))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = (Routes.APIVersion + Routes.UnFavoriteFood))] HttpRequest req,
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

            // Auth check
            AuthResultModel authResult = await DIContainer.Instance.GetService<IAuthorization>().CheckAuthorization(req, patientId);
            if (!authResult.Result)
                return new StatusCodeResult((int)authResult.StatusCode);

            IFoodRepository foodRepository = DIContainer.Instance.GetService<IFoodRepository>();

            bool succes = foodRepository.UnFavorite(patientId, foodId);

            return succes != false
                ? (ActionResult)new OkObjectResult($"favorite deleted")
                : new BadRequestObjectResult("oopsiewoopsie er is een fout begaan banaan");
        }
    }
}
