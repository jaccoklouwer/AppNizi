using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using AppNiZiAPI.Variables;
using AppNiZiAPI.Models.Repositories;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using AppNiZiAPI.Infrastructure;

namespace AppNiZiAPI.Functions.DietaryManagement.DELETE
{
    public static class DietaryManagement
    {
        /// <summary>
        /// Create Products
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [FunctionName(nameof(DeleteDietaryManagement))]
        public static async Task<IActionResult> DeleteDietaryManagement(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = (Routes.APIVersion + Routes.DietaryManagementById))] HttpRequest req, int dietId,
            ILogger log)
        {
            //link voor swagger https://devkimchi.com/2019/02/02/introducing-swagger-ui-on-azure-functions/
            log.LogInformation("C# HTTP trigger function processed a request.");
            //if (!await Authorization.CheckAuthorization(req, patientId)) { return new UnauthorizedResult(); }


            IDietaryManagementRepository repository = DIContainer.Instance.GetService<IDietaryManagementRepository>();
            bool success;
            try
            {
                success = repository.DeleteDietaryManagement(dietId);
            }
            catch (Exception)
            {
                return new NotFoundObjectResult(Messages.ErrorMissingValues);
            }

            return success
                ? (ActionResult)new BadRequestObjectResult(Messages.ErrorMissingValues)
                : new OkObjectResult(Messages.OKDelete);
        }
    }
}
