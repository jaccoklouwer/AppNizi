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

namespace AppNiZiAPI.Functions.Doctor.GET
{
    public static class GetDoctorById
    {
        [FunctionName("GetDoctorById")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = (Routes.APIVersion + Routes.SpecificDoctor))] HttpRequest req,
            ILogger log, int doctorId)
        {
            if (doctorId == 0)
                return new BadRequestResult();

            #region AuthCheck
            AuthResultModel authResult = await DIContainer.Instance.GetService<IAuthorization>().CheckAuthorization(req);
            if (!authResult.Result)
                return new StatusCodeResult(authResult.StatusCode);
            #endregion

            DoctorModel doctor = DIContainer.Instance.GetService<IDoctorRepository>().GetDoctorById(doctorId);

            return doctor != null
                ? (ActionResult)new OkObjectResult(doctor)
                : new NoContentResult();
        }
    }
}
