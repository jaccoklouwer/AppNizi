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

namespace AppNiZiAPI.Functions.Patients.GET
{
    public static class GetPatientById
    {
        [FunctionName("GetPatientByID")]
        #region Swagger
        [OpenApiOperation("GetPatientByID", "Patient", Summary = "Get specific patient", Description = "Get specific patient", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseBody(HttpStatusCode.OK, "application/json", typeof(PatientObject), Summary = Messages.OKUpdate)]
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

            try
            {
                IPatientRepository patientRepository = DIContainer.Instance.GetService<IPatientRepository>();
                PatientObject patient = patientRepository.Select(patientId);

                // Return object if possible
                return patient != null
                ? (ActionResult)new OkObjectResult(patient)
                : new BadRequestResult();
            }
            catch (Exception ex)
            {
                // Build error message and return it.
                string callbackMessage = new MessageHandler().BuildErrorMessage(ex);
                return new BadRequestObjectResult(callbackMessage);
            }
        }
    }
}
