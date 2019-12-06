using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AppNiZiAPI.Variables;
using AppNiZiAPI.Models.Repositories;
using AppNiZiAPI.Models;
using System.Collections.Generic;
using AppNiZiAPI.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Newtonsoft.Json.Linq;
using AppNiZiAPI.Security;
using System;
using AppNiZiAPI.Models.AccountModels;
using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;
using System.Net;
using Microsoft.OpenApi.Models;
using Aliencube.AzureFunctions.Extensions.OpenApi.Enums;

namespace AppNiZiAPI.Functions.Account.GET
{
    public static class GetUserAsPatient
    {
        [FunctionName("GetUserAsPatient")]
        #region Swagger
        [OpenApiOperation("LoginPatient", "Patient", Summary = "Get User As Patient from Acces Token", Description = "Get User As Patient from Acces Token", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseBody(HttpStatusCode.OK, "application/json", typeof(PatientLogin), Summary = Messages.OKResult)]
        [OpenApiResponseBody(HttpStatusCode.Unauthorized, "application/json", typeof(Error), Summary = Messages.AuthNoAcces)]
        [OpenApiResponseBody(HttpStatusCode.BadRequest, "application/json", typeof(Error), Summary = Messages.ErrorPostBody)]
        [OpenApiResponseBody(HttpStatusCode.NotFound, "application/json", typeof(Error), Summary = Messages.ErrorPostBody)] 
        #endregion
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = (Routes.APIVersion + Routes.LoginPatient))] HttpRequest req,
            ILogger log)
        {
            // Auth check
            AuthLogin authLogin = await DIContainer.Instance.GetService<IAuthorization>().LoginAuthAsync(req);
            if (authLogin == null) { return new BadRequestObjectResult(Messages.AuthNoAcces); }

            IPatientRepository patientRepository = DIContainer.Instance.GetService<IPatientRepository>();
            PatientLogin patientLogin = await patientRepository.GetPatientInfo(authLogin.Guid);
            if (patientLogin == null)
                return new BadRequestResult();

            patientLogin.AuthLogin = authLogin;

            return new OkObjectResult(patientLogin);
        }
    }
}
