using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AppNiZiAPI.Security;
using AppNiZiAPI.Models;
using AppNiZiAPI.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using AppNiZiAPI.Variables;
using AppNiZiAPI.Models.Repositories;
using System.Collections.Generic;
using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;
using Aliencube.AzureFunctions.Extensions.OpenApi.Enums;
using System.Net;
using Microsoft.OpenApi.Models;

namespace AppNiZiAPI.Functions.Doctor.GET
{
    public static class GetDoctorPatients
    {
        [FunctionName("GetDoctorPatients")]
        #region Swagger
        [OpenApiOperation("GetDoctorPatients", "Doctor", Summary = "Get the patients from a doctor", Description = "Get the patients from a doctor", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseBody(HttpStatusCode.OK, "application/json", typeof(Patient[]), Summary = Messages.OKUpdate)]
        [OpenApiResponseBody(HttpStatusCode.Unauthorized, "application/json", typeof(string), Summary = Messages.AuthNoAcces)]
        [OpenApiResponseBody(HttpStatusCode.Forbidden, "application/json", typeof(string), Summary = Messages.AuthNoAcces)]
        [OpenApiResponseBody(HttpStatusCode.BadRequest, "application/json", typeof(string), Summary = Messages.ErrorPostBody)]
        [OpenApiParameter("doctorId", Description = "Inserting the doctor id", In = ParameterLocation.Path, Required = true, Type = typeof(int))]
        #endregion
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = (Routes.APIVersion + Routes.GetDoctorPatients))] HttpRequest req,
            ILogger log, int doctorId)
        {
            #region AuthCheck
            AuthResultModel authResult = await DIContainer.Instance.GetService<IAuthorization>().AuthForDoctor(req, doctorId);
            if (!authResult.Result)
                return new StatusCodeResult((int)authResult.StatusCode);
            #endregion

            IDoctorRepository doctorRepository = DIContainer.Instance.GetService<IDoctorRepository>();
            List<Patient> patients = doctorRepository.GetDoctorPatients(doctorId);

            return patients != null
                ? (ActionResult)new OkObjectResult(patients)
                : new NoContentResult();
        }
    }
}
