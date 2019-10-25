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

namespace AppNiZiAPI.Functions.Meal.DELETE
{
    public static class DeleteMeal
    {
        [FunctionName("DeleteMeal")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "delete", "post", Route = ( Routes.APIVersion + Routes.DeleteMeal))] HttpRequest req,
            ILogger log,int mealId,int patientId)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                bool success = new MealRepository().DeleteMeal(patientId,mealId);

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
