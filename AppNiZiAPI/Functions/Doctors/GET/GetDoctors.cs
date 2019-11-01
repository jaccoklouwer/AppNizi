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
using AppNiZiAPI.Security;
using Microsoft.Extensions.DependencyInjection;
using AppNiZiAPI.Models;
using AppNiZiAPI.Infrastructure;
using AppNiZiAPI.Models.Repositories;
using System.Collections.Generic;
using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;
using System.Net;
using Microsoft.OpenApi.Models;
using Aliencube.AzureFunctions.Extensions.OpenApi.Enums;

namespace AppNiZiAPI.Functions.Doctors.GET
{
    public static class GetDoctors
    {
        [FunctionName("GetDoctors")]
        #region Swagger
        [OpenApiOperation("GetDoctors", "Doctor", Summary = "Get list of all doctors", Description = "Get list of all doctors", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseBody(HttpStatusCode.OK, "application/json", typeof(DoctorModel[]), Summary = Messages.OKUpdate)]
        [OpenApiResponseBody(HttpStatusCode.Unauthorized, "application/json", typeof(string), Summary = Messages.AuthNoAcces)]
        [OpenApiResponseBody(HttpStatusCode.Forbidden, "application/json", typeof(string), Summary = Messages.AuthNoAcces)]
        [OpenApiResponseBody(HttpStatusCode.BadRequest, "application/json", typeof(string), Summary = Messages.ErrorPostBody)]
        #endregion
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = (Routes.APIVersion + Routes.Doctor))] HttpRequest req,
            ILogger log)
        {
            #region AuthCheck
            AuthResultModel authResult = await DIContainer.Instance.GetService<IAuthorization>().CheckAuthorization(req);
            if (!authResult.Result)
                return new StatusCodeResult((int)authResult.StatusCode);
            #endregion

            IDoctorRepository doctorRepository = DIContainer.Instance.GetService<IDoctorRepository>();
            List<AppNiZiAPI.Models.DoctorModel> doctors = doctorRepository.GetDoctors();

            return doctors.Count != 0
                ? (ActionResult)new OkObjectResult(doctors)
                : new NoContentResult();
        }
    }
}
