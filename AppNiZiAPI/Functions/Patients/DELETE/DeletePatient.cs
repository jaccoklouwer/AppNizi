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
using System.Net.Http;
using System.Net;
using AppNiZiAPI.Models.Repositories;
using AppNiZiAPI.Models.Handlers;
using AppNiZiAPI.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using AppNiZiAPI.Security;
using AppNiZiAPI.Models;
using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;
using Aliencube.AzureFunctions.Extensions.OpenApi.Enums;
using Microsoft.OpenApi.Models;
using AppNiZiAPI.Services;
using System.Collections.Generic;
using AppNiZiAPI.Services.Handlers;

namespace AppNiZiAPI.Functions.Patients.DELETE
{
    public static class DeletePatient
    {
        [FunctionName("DeletePatient")]
        #region Swagger
        [OpenApiOperation("DeletePatient", "Patient", Summary = "Delete a specific patient", Description = "Delete a specific patient", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseBody(HttpStatusCode.OK, "application/json", typeof(string), Summary = Messages.OKUpdate)]
        [OpenApiResponseBody(HttpStatusCode.Unauthorized, "application/json", typeof(string), Summary = Messages.AuthNoAcces)]
        [OpenApiResponseBody(HttpStatusCode.Forbidden, "application/json", typeof(string), Summary = Messages.AuthNoAcces)]
        [OpenApiResponseBody(HttpStatusCode.BadRequest, "application/json", typeof(string), Summary = Messages.ErrorPostBody)]
        [OpenApiParameter("patientId", Description = "Inserting the patient id", In = ParameterLocation.Path, Required = true, Type = typeof(int))]
        #endregion
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = (Routes.APIVersion + Routes.SpecificPatient))] HttpRequest req,
            string patientId,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // Logic
            Dictionary<ServiceDictionaryKey, object> dictionary = await DIContainer.Instance.GetService<IPatientService>()
                .TryDeletePatient(req, patientId);

            // Response
            return DIContainer.Instance.GetService<IResponseHandler>().ForgeResponse(dictionary);
        }
    }
}
