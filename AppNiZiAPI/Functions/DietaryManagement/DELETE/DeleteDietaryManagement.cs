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
using System.Net;
using AppNiZiAPI.Models;
using AzureFunctions.Extensions.Swashbuckle.Attribute;
using Authorization = AppNiZiAPI.Security.Authorization;

namespace AppNiZiAPI.Functions.DietaryManagement.DELETE
{
    public static class DeleteDietaryManagement
    {
        /// <summary>
        /// Delete a dietaryManagment
        /// </summary>
        /// <param name="dietId"></param>
        /// <returns>succes</returns>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(string))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(Error))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(Error))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized, Type = typeof(Error))]
        [RequestHttpHeader("Authorization", isRequired: false)]
        [FunctionName("DeleteDietaryManagement")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = (Routes.APIVersion + Routes.DietaryManagementById))] HttpRequest req, string dietId,
            ILogger log)
        {
            //link voor swagger https://medium.com/@yuka1984/open-api-swagger-and-swagger-ui-on-azure-functions-v2-c-a4a460b34b55
            log.LogInformation("C# HTTP trigger function processed a request.");
            //if (!await Authorization.CheckAuthorization(req, patientId)) { return new UnauthorizedResult(); }

            int id = 0;
            if (!int.TryParse(dietId, out id)) { return new UnprocessableEntityObjectResult(Messages.ErrorMissingValues); }

            IDietaryManagementRepository repository = new DietaryManagementRepository();
            bool success = false;
            try
            {
                success = repository.DeleteDietaryManagement(id);
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
