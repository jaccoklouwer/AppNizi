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
using AppNiZiAPI.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using AppNiZiAPI.Models;
using AppNiZiAPI.Variables;
using AppNiZiAPI.Models.Repositories;
using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;
using System.Net;
using Microsoft.OpenApi.Models;
using Aliencube.AzureFunctions.Extensions.OpenApi.Enums;

namespace AppNiZiAPI.Functions.Doctor.GET
{
    public static class GetDoctorById
    {
        [FunctionName("GetDoctorById")]
        #region Swagger
        [OpenApiOperation("GetDoctorById", "Doctor", Summary = "Get doctor by id", Description = "Get doctor by id", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseBody(HttpStatusCode.OK, "application/json", typeof(DoctorModel), Summary = Messages.OKUpdate)]
        [OpenApiResponseBody(HttpStatusCode.Unauthorized, "application/json", typeof(string), Summary = Messages.AuthNoAcces)]
        [OpenApiResponseBody(HttpStatusCode.Forbidden, "application/json", typeof(string), Summary = Messages.AuthNoAcces)]
        [OpenApiResponseBody(HttpStatusCode.BadRequest, "application/json", typeof(string), Summary = Messages.ErrorPostBody)]
        [OpenApiParameter("doctorId", Description = "Inserting the doctor id", In = ParameterLocation.Path, Required = true, Type = typeof(int))]
        #endregion
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = (Routes.APIVersion + Routes.SpecificDoctor))] HttpRequest req,
            ILogger log, int doctorId)
        {
            if (doctorId == 0)
                return new BadRequestResult();

            #region AuthCheck
            AuthResultModel authResult = await DIContainer.Instance.GetService<IAuthorization>().CheckAuthorization(req);
            if (!authResult.Result)
                return new StatusCodeResult((int)authResult.StatusCode);
            #endregion

            DoctorModel doctor = DIContainer.Instance.GetService<IDoctorRepository>().GetDoctorById(doctorId);

            return doctor != null
                ? (ActionResult)new OkObjectResult(doctor)
                : new NoContentResult();
        }
    }
}
