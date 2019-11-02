using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AppNiZiAPI.Models.AccountModels;
using AppNiZiAPI.Infrastructure;
using AppNiZiAPI.Security;
using AppNiZiAPI.Variables;
using Microsoft.Extensions.DependencyInjection;
using AppNiZiAPI.Models.Repositories;
using Newtonsoft.Json.Linq;
using AppNiZiAPI.Models;
using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;
using Aliencube.AzureFunctions.Extensions.OpenApi.Enums;
using System.Net;
using AppNiZiAPI.Models.SwaggerModels;
using AppNiZiAPI.Services;
using System.Collections.Generic;

namespace AppNiZiAPI.Functions.Account.POST
{
    public static class RegisterPatient
    {
        [FunctionName("RegisterPatient")]
        #region Swagger
        [OpenApiOperation("RegisterPatient", "Patient", Summary = "Register a new patient", Description = "Register a new patient", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseBody(HttpStatusCode.OK, "application/json", typeof(PatientLogin), Summary = Messages.OKResult)]
        [OpenApiResponseBody(HttpStatusCode.Unauthorized, "application/json", typeof(Error), Summary = Messages.AuthNoAcces)]
        [OpenApiResponseBody(HttpStatusCode.BadRequest, "application/json", typeof(Error), Summary = Messages.ErrorPostBody)]
        [OpenApiResponseBody(HttpStatusCode.NotFound, "application/json", typeof(Error), Summary = Messages.ErrorPostBody)]
        [OpenApiRequestBody("application/json", typeof(SwaggerRegisterPatient), Description = "New patient")]
        #endregion
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = (Routes.APIVersion + Routes.Patients))] HttpRequest req,
            ILogger log)
        {
            // Auth check
            AuthLogin authLogin = await DIContainer.Instance.GetService<IAuthorization>().LoginAuthAsync(req);
            if (authLogin == null) { return new BadRequestObjectResult(Messages.AuthNoAcces); }

            log.LogInformation("C# HTTP trigger function processed a request.");

            IPatientService patientService = DIContainer.Instance.GetService<IPatientService>();
            Dictionary<ServiceDictionaryKey, object> dictionary = await patientService.TryRegisterPatient(req, authLogin);

            // Returns a build error message
            if (dictionary.ContainsKey(ServiceDictionaryKey.ERROR))
                return new BadRequestObjectResult(dictionary[ServiceDictionaryKey.ERROR]);

            // Return object if possible
            return dictionary.ContainsKey(ServiceDictionaryKey.VALUE)
            ? (ActionResult)new OkObjectResult(dictionary[ServiceDictionaryKey.VALUE])
            : new BadRequestResult();
        }
    }
}
