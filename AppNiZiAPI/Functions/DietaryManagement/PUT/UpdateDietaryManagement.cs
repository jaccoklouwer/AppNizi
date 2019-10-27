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
using AppNiZiAPI.Models;
using AppNiZiAPI.Models.Repositories;
using AppNiZiAPI.Security;
using AzureFunctions.Extensions.Swashbuckle.Attribute;
using System.Net;
using Authorization = AppNiZiAPI.Security.Authorization;

namespace AppNiZiAPI.Functions.DietaryManagement.PUT
{
    public static class UpdateDietaryManagement
    {
        /// <summary>
        /// Update a dietaryManagment
        /// </summary>
        /// <param name="dietId"></param>
        /// <param name="req"></param>
        /// <returns>list of dietarymanagement</returns>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(string))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(Error))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(Error))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized, Type = typeof(Error))]
        [RequestHttpHeader("Authorization", isRequired: false)]
        [FunctionName("UpdateDietaryManagement")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = (Routes.APIVersion + Routes.DietaryManagementById))]
            [RequestBodyType(typeof(DietaryManagementModel), "Dietary management")]HttpRequest req, string dietId,
            ILogger log)
        {

            //link voor swagger https://medium.com/@yuka1984/open-api-swagger-and-swagger-ui-on-azure-functions-v2-c-a4a460b34b55
            log.LogInformation("C# HTTP trigger function processed a request.");
            //if (!await Authorization.CheckAuthorization(req, patientId)) { return new UnauthorizedResult(); }

            int id = 0;
            if (!int.TryParse(dietId, out id)) { return new BadRequestObjectResult(Messages.ErrorMissingValues); }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            if (string.IsNullOrEmpty(requestBody))
                return new UnprocessableEntityObjectResult(Messages.ErrorMissingValues);

            IDietaryManagementRepository repository = new DietaryManagementRepository();
            try
            {
                DietaryManagementModel dietary = JsonConvert.DeserializeObject<DietaryManagementModel>(requestBody);
                bool success = repository.UpdateDietaryManagement(id, dietary);

                if (success)
                {
                    return new OkObjectResult(Messages.OKUpdate);
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
