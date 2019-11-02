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
using AppNiZiAPI.Models.Handlers;
using AppNiZiAPI.Models;
using AppNiZiAPI.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using AppNiZiAPI.Security;
using Newtonsoft.Json.Linq;
using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;
using System.Net;
using Aliencube.AzureFunctions.Extensions.OpenApi.Enums;
using Microsoft.OpenApi.Models;
using AppNiZiAPI.Services;
using System.Collections.Generic;
using static AppNiZiAPI.Services.PatientService;

namespace AppNiZiAPI.Functions.Patients.GET
{
    public static class GetPatientById
    {
        [FunctionName("GetPatientByID")]
        #region Swagger
        [OpenApiOperation("GetPatientByID", "Patient", Summary = "Get specific patient", Description = "Get specific patient", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseBody(HttpStatusCode.OK, "application/json", typeof(Patient), Summary = Messages.OKUpdate)]
        [OpenApiResponseBody(HttpStatusCode.Unauthorized, "application/json", typeof(string), Summary = Messages.AuthNoAcces)]
        [OpenApiResponseBody(HttpStatusCode.Forbidden, "application/json", typeof(string), Summary = Messages.AuthNoAcces)]
        [OpenApiResponseBody(HttpStatusCode.BadRequest, "application/json", typeof(string), Summary = Messages.ErrorPostBody)]
        [OpenApiParameter("patientId", Description = "Inserting the patient id", In = ParameterLocation.Path, Required = true, Type = typeof(int))]
        #endregion
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = (Routes.APIVersion + Routes.SpecificPatient))] HttpRequest req, int patientId,
            ILogger log)
        {
            #region AuthCheck
            AuthResultModel authResult = await DIContainer.Instance.GetService<IAuthorization>().AuthForDoctorOrPatient(req, patientId);
            if (!authResult.Result)
                return new StatusCodeResult((int)authResult.StatusCode);
            #endregion

            IPatientService patientRepository = DIContainer.Instance.GetService<IPatientService>();
            Dictionary<ServiceDictionaryKey, object> dictionary = patientRepository.TryGetPatientById(patientId);

            // Returns a build error message
            if (dictionary.ContainsKey(ServiceDictionaryKey.ERROR))
                return new BadRequestObjectResult(dictionary[ServiceDictionaryKey.ERROR]);

            // Return object if possible
            return dictionary.ContainsKey(ServiceDictionaryKey.OBJECT)
            ? (ActionResult)new OkObjectResult(dictionary[ServiceDictionaryKey.OBJECT])
            : new BadRequestResult();
        }
    }
}
