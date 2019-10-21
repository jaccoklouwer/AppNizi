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

namespace AppNiZiAPI.Functions.DietaryManagement.DELETE
{
    public static class DeleteDietaryManagement
    {
        [FunctionName("DeleteDietaryManagement")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = (Routes.APIVersion + Routes.DietaryManagementById))] HttpRequest req, string dietId,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            if (!await Authorization.CheckAuthorization(req.Headers)) { return new BadRequestObjectResult(Messages.AuthNoAcces); }

            int id = 0;
            if (!int.TryParse(dietId, out id)) { return new BadRequestObjectResult(Messages.ErrorMissingValues); }

            DietaryManagementRepository repository = new DietaryManagementRepository();
            bool success = false;
            try
            {
                success = repository.DeleteDietaryManagement(id);
            }
            catch (Exception)
            {

                return new BadRequestObjectResult(Messages.ErrorMissingValues);
            }

            return success
                ? (ActionResult)new BadRequestObjectResult(Messages.ErrorMissingValues)
                : new OkObjectResult(Messages.OKDelete);
        }
    }
}
