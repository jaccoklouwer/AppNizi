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
using AppNiZiAPI.Infrastructure;
using AppNiZiAPI.Services.Handlers;
using System.Collections.Generic;
using AppNiZiAPI.Services;
using Microsoft.Extensions.DependencyInjection;
using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;
using System.Net;
using AppNiZiAPI.Models;
using AppNiZiAPI.Models.SwaggerModels;
using Aliencube.AzureFunctions.Extensions.OpenApi.Enums;

namespace AppNiZiAPI.Functions.Patients.PUT
{
    public static class UpdatePatient
    {
        [FunctionName("UpdatePatient")]
        #region Swagger
        [OpenApiOperation("RegisterPatient", "Patient", Summary = "Register a new patient", Description = "Update a patient", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseBody(HttpStatusCode.OK, "application/json", typeof(PatientUpdateModel), Summary = Messages.OKResult)]
        [OpenApiResponseBody(HttpStatusCode.Unauthorized, "application/json", typeof(Error), Summary = Messages.AuthNoAcces)]
        [OpenApiResponseBody(HttpStatusCode.BadRequest, "application/json", typeof(Error), Summary = Messages.ErrorPostBody)]
        [OpenApiResponseBody(HttpStatusCode.NotFound, "application/json", typeof(Error), Summary = Messages.ErrorPostBody)]
        [OpenApiRequestBody("application/json", typeof(PatientUpdateModel), Description = "Update patient")]
        #endregion
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "put", Route = (Routes.APIVersion + Routes.Patient))] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // Logic
            Dictionary<ServiceDictionaryKey, object> dictionary = await DIContainer.Instance.GetService<IPatientService>()
                .TryUpdatePatient(req);

            // Response
            return DIContainer.Instance.GetService<IResponseHandler>().ForgeResponse(dictionary);
        }
    }
}
