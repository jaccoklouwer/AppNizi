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

namespace AppNiZiAPI.Functions.Doctor.DELETE
{
    public static class DeleteDoctor
    {
        [FunctionName("DeleteDoctor")]
        #region Swagger
        [OpenApiOperation("DeleteDoctor", "Doctor", Summary = "Delete specific doctor", Description = "Delete specific doctor", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseBody(HttpStatusCode.OK, "application/json", typeof(Patient[]), Summary = Messages.OKUpdate)]
        [OpenApiResponseBody(HttpStatusCode.Unauthorized, "application/json", typeof(string), Summary = Messages.AuthNoAcces)]
        [OpenApiResponseBody(HttpStatusCode.Forbidden, "application/json", typeof(string), Summary = Messages.AuthNoAcces)]
        [OpenApiResponseBody(HttpStatusCode.BadRequest, "application/json", typeof(string), Summary = Messages.ErrorPostBody)]
        [OpenApiResponseBody(HttpStatusCode.Conflict, "application/json", typeof(string), Summary = Messages.ErrorPostBody)]
        [OpenApiParameter("doctorId", Description = "Inserting the doctor id", In = ParameterLocation.Path, Required = true, Type = typeof(int))]
        #endregion
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = (Routes.APIVersion + Routes.SpecificDoctor))] HttpRequest req,
            ILogger log, int doctorId)
        {
            #region AuthCheck
            AuthResultModel authResult = await DIContainer.Instance.GetService<IAuthorization>().AuthForDoctor(req, doctorId);
            if (!authResult.Result)
                return new StatusCodeResult((int)authResult.StatusCode);
            #endregion

            try
            {
                IDoctorRepository doctorRepository = DIContainer.Instance.GetService<IDoctorRepository>();
                bool success = doctorRepository.Delete(doctorId);

                return doctorRepository.Delete(doctorId)
                    ? (ActionResult)new OkResult()
                    : new StatusCodeResult(StatusCodes.Status409Conflict);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(new MessageHandler().BuildErrorMessage(e));
            }
        }
    }
}
