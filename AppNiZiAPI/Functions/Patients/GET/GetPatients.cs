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
using AppNiZiAPI.Models.Views;
using System.Collections.Generic;
using AppNiZiAPI.Models.Repositories;
using AppNiZiAPI.Models.Handler;
using System.Security.Claims;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Headers;
using AppNiZiAPI.Security;
using Microsoft.IdentityModel.Logging;
using AppNiZiAPI.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;
using Aliencube.AzureFunctions.Extensions.OpenApi.Enums;
using System.Net;
using Microsoft.OpenApi.Models;

namespace AppNiZiAPI.Functions.Patients
{
    /// <summary>
    /// Retrieve an array of all patients.
    /// </summary>
    public static class GetPatients
    {
        [FunctionName("GetPatients")]
        #region Swagger
        [OpenApiOperation("GetPatients", "Patient", Summary = "Get all patients", Description = "Get all patients", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseBody(HttpStatusCode.OK, "application/json", typeof(PatientObject[]), Summary = Messages.OKUpdate)]
        [OpenApiResponseBody(HttpStatusCode.Unauthorized, "application/json", typeof(string), Summary = Messages.AuthNoAcces)]
        [OpenApiResponseBody(HttpStatusCode.Forbidden, "application/json", typeof(string), Summary = Messages.AuthNoAcces)]
        [OpenApiResponseBody(HttpStatusCode.BadRequest, "application/json", typeof(string), Summary = Messages.ErrorPostBody)]
        [OpenApiParameter("patientId", Description = "Inserting the patient id", In = ParameterLocation.Path, Required = true, Type = typeof(int))]
        #endregion
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Admin, "get", Route = (Routes.APIVersion + Routes.Patients))] HttpRequest req,
            ILogger log)
        {
            // Authorization
            //if (!await Authorization.CheckAuthorization(req.Headers)){ return new BadRequestObjectResult(Messages.AuthNoAcces);}

            log.LogInformation("C# HTTP trigger function processed a request.");

            int count = new QueryHandler().ExtractIntegerFromRequestQuery("count", req);

            IPatientRepository patientRepository = DIContainer.Instance.GetService<IPatientRepository>();
            List<PatientView> patients = patientRepository.List(count);

            dynamic data = JsonConvert.SerializeObject(patients);
            
            return patients != null
                ? (ActionResult)new OkObjectResult(data)
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}
