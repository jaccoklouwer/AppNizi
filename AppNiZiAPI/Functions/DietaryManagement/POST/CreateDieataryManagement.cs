using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using AppNiZiAPI.Models;
using AzureFunctions.Extensions.Swashbuckle.Attribute;
using AppNiZiAPI.Variables;
using AppNiZiAPI.Models.Repositories;

namespace AppNiZiAPI.Functions.DietaryManagement.POST
{
    public static class CreateDieataryManagement
    {
        /// <summary>
        /// Get DietaryManagement of a Patient
        /// </summary>
        /// <param name="patientId"></param>
        /// <returns>list of dietarymanagement</returns>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(string))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(Error))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized, Type = typeof(Error))]
        [RequestHttpHeader("Authorization", isRequired: false)]
        [FunctionName("CreateDieataryManagement")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = (Routes.APIVersion + Routes.DietaryManagement))] HttpRequest req,
            ILogger log)
        {
            //link voor swagger https://medium.com/@yuka1984/open-api-swagger-and-swagger-ui-on-azure-functions-v2-c-a4a460b34b55
            log.LogInformation("C# HTTP trigger function processed a request.");
            //if (!await Authorization.CheckAuthorization(req.Headers)) { return new UnauthorizedResult(); }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            if (string.IsNullOrEmpty(requestBody))
                return new UnprocessableEntityObjectResult(Messages.ErrorMissingValues);

            DietaryManagementModel dietary = null;

            IDietaryManagementRepository repository = new DietaryManagementRepository();
            
            try
            {
                dietary = JsonConvert.DeserializeObject<DietaryManagementModel>(requestBody);
                bool success = repository.AddDietaryManagement(dietary);
                if (success)
                {
                    return new OkObjectResult(Messages.OKPost);
                }
                else
                {
                    return new BadRequestObjectResult(Messages.ErrorPostBody);
                }
            }
            catch (Exception e)
            {
                return new NotFoundObjectResult(Messages.ErrorMissingValues + e.Message);
            }
        }
    }
}
