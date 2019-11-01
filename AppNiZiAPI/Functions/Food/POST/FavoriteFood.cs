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
using AppNiZiAPI.Models;
using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;
using System.Net;
using Microsoft.OpenApi.Models;
using Aliencube.AzureFunctions.Extensions.OpenApi.Enums;

namespace AppNiZiAPI.Functions.Food
{
    public static class FavoriteFood
    {
        [FunctionName("PostFavoriteFood")]
        [OpenApiOperation("Add Favorite Food", "Food", Summary = "Makes a fooditem a favorite", Description = "Makes a supplied fooditem a favorite of the user and retrievable with getFavoriteFood", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseBody(HttpStatusCode.OK, "application/json", typeof(string), Summary = Messages.OKUpdate)]
        [OpenApiResponseBody(HttpStatusCode.Unauthorized, "application/json", typeof(string), Summary = Messages.AuthNoAcces)]
        [OpenApiResponseBody(HttpStatusCode.BadRequest, "application/json", typeof(string), Summary = Messages.ErrorMissingValues)]
        [OpenApiParameter("patientId", Description = "the id of the patient thats favoriting the food", In = ParameterLocation.Query, Required = true, Type = typeof(int))]
        [OpenApiParameter("foodId", Description = "the fooditem to be favorited", In = ParameterLocation.Query, Required = true, Type = typeof(int))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous,  "post", Route = (Routes.APIVersion + Routes.PostFavoriteFood))] HttpRequest req,
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

            bool succes = foodRepository.Favorite(patientId,foodId);

            return succes != false
                ? (ActionResult)new OkObjectResult($"alles is super sexy en je hebt een fav gedaan")
                : new BadRequestObjectResult("oopsiewoopsie er is een fout begaan banaan");
        }
    }
}
