using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AppNiZiAPI.Variables;
using AppNiZiAPI.Models;
using System.Collections.Generic;
using AppNiZiAPI.Models.Repositories;
using System.Net;
using AppNiZiAPI.Security;
using Authorization = AppNiZiAPI.Security.Authorization;
using AzureFunctions.Extensions.Swashbuckle.Attribute;

namespace AppNiZiAPI.Functions.DietaryManagement
{
    public static class DietaryManagement
    {
        /// <summary>
        /// Get DietaryManagement of a Patient
        /// </summary>
        /// <param name="patientId"></param>
        /// <returns>list of dietarymanagement</returns>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(string))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(Error))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(Error))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized, Type = typeof(Error))]
        [RequestHttpHeader("Authorization", isRequired: false)]
        [FunctionName("Get DietaryManagement By Patient")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = (Routes.APIVersion + Routes.GetDietaryManagement))] HttpRequest req, string patientId,
            ILogger log)
        {
            //link voor swagger https://medium.com/@yuka1984/open-api-swagger-and-swagger-ui-on-azure-functions-v2-c-a4a460b34b55
            //if (!await Authorization.CheckAuthorization(req, patientId)) { return new UnauthorizedResult(); }
            int id = 0;
            if (!int.TryParse(patientId, out id)) { return new UnprocessableEntityObjectResult(Messages.ErrorMissingValues); }

            List<DietaryManagementModel> dietaryManagementModels = null;
            try
            {
                DietaryManagementRepository repository = new DietaryManagementRepository();
                dietaryManagementModels = repository.GetDietaryManagementByPatient(id);
            }
            catch (Exception e)
            {
                log.LogInformation(e.Message);
                return new NotFoundObjectResult(Messages.ErrorMissingValues);
            }

            if (dietaryManagementModels == null)
                return new BadRequestObjectResult("No Dietarymanagement was found!");

            string json = JsonConvert.SerializeObject(dietaryManagementModels);

            return json != null
                ? (ActionResult)new OkObjectResult(json)
                : new BadRequestObjectResult(Messages.ErrorMissingValues);
        }
    }
}

