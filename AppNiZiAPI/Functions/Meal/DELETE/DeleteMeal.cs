using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AppNiZiAPI.Models.Repositories;
using AppNiZiAPI.Models.Handlers;
using AppNiZiAPI.Variables;
using AppNiZiAPI.Security;


using AppNiZiAPI.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using AppNiZiAPI.Models;
using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;
using System.Net;
using Aliencube.AzureFunctions.Extensions.OpenApi.Enums;
using Microsoft.OpenApi.Models;

namespace AppNiZiAPI.Functions.Meal.DELETE
{
    public static class DeleteMeal
    {
        [FunctionName("DeleteMeal")]
        [OpenApiOperation("DeleteMeal", "Meal", Summary = "Deletes a meal", Description = "Removes the meal of the specified user", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseBody(HttpStatusCode.OK, "application/json", typeof(string), Summary = Messages.OKUpdate)]
        [OpenApiResponseBody(HttpStatusCode.Unauthorized, "application/json", typeof(string), Summary = Messages.AuthNoAcces)]
        [OpenApiResponseBody(HttpStatusCode.BadRequest, "application/json", typeof(string), Summary = Messages.ErrorMissingValues)]
        [OpenApiParameter("patientId", Description = "the id of the patient thats going to delete his meal", In = ParameterLocation.Path, Required = true, Type = typeof(int))]
        [OpenApiParameter("mealId", Description = "The meal to delete", In = ParameterLocation.Path, Required = true, Type = typeof(int))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "delete", "post", Route = ( Routes.APIVersion + Routes.DeleteMeal))] HttpRequest req,
            ILogger log,int patientId,int mealId)
        {
            // Auth check
            AuthResultModel authResult = await DIContainer.Instance.GetService<IAuthorization>().CheckAuthorization(req, patientId);
            if (!authResult.Result)
                return new StatusCodeResult(authResult.StatusCode);

            IMealRepository mealRepository = DIContainer.Instance.GetService<IMealRepository>();
            try
            {
                bool success = mealRepository.DeleteMeal(patientId,mealId);

                if (success)
                    return new OkObjectResult("Deleted.");
                else
                    return new NotFoundObjectResult("Deletion failed, invalid GUID?");
            }
            catch (Exception ex)
            {
                // Build error message and return it.
                string callbackMessage = new MessageHandler().BuildErrorMessage(ex);
                return new BadRequestObjectResult(callbackMessage);
            }
        }
    }
}
