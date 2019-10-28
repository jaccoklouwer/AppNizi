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
using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;
using Microsoft.OpenApi.Models;

namespace AppNiZiAPI.Functions.DietaryManagement.GET
{
    public static class DietaryManagement
    {
        [OpenApiOperation("get")]
        [OpenApiParameter("patientId", In = ParameterLocation.Path, Required = false, Type = typeof(int))]
        [OpenApiResponseBody(HttpStatusCode.OK, "application/json", typeof(string))]
        [OpenApiResponseBody(HttpStatusCode.NotFound, "application/json", typeof(string))]
        [OpenApiResponseBody(HttpStatusCode.BadRequest, "application/json", typeof(string))]
        [OpenApiResponseBody(HttpStatusCode.Unauthorized, "application/json", typeof(string))]
        [FunctionName(nameof(GetDietaryManagementByPatient))]
        public static async Task<IActionResult> GetDietaryManagementByPatient(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = (Routes.APIVersion + Routes.GetDietaryManagement))] HttpRequest req, int patientId,
            ILogger log)
        {
            //link voor swagger https://devkimchi.com/2019/02/02/introducing-swagger-ui-on-azure-functions/
            //if (!await Authorization.CheckAuthorization(req, patientId)) { return new UnauthorizedResult(); }

            List<DietaryManagementModel> dietaryManagementModels;
            try
            {
                IDietaryManagementRepository repository = new DietaryManagementRepository();
                dietaryManagementModels = repository.GetDietaryManagementByPatient(patientId);
            }
            catch (Exception e)
            {
                log.LogInformation(e.Message);
                return new NotFoundObjectResult(Messages.ErrorMissingValues + e.Message);
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

