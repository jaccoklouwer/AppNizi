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

namespace AppNiZiAPI.Functions.Doctors.GET
{
    public static class GetDoctors
    {
        [FunctionName("GetDoctors")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = (Routes.APIVersion + Routes.Doctor))] HttpRequest req,
            ILogger log)
        {
            #region AuthCheck
            AuthResultModel authResult = await DIContainer.Instance.GetService<IAuthorization>().CheckAuthorization(req);
            if (!authResult.Result)
                return new StatusCodeResult(authResult.StatusCode);
            #endregion

            IDoctorRepository doctorRepository = DIContainer.Instance.GetService<IDoctorRepository>();
            List<AppNiZiAPI.Models.Doctor> doctors = doctorRepository.GetDoctors();

            return doctors.Count != 0
                ? (ActionResult)new OkObjectResult(doctors)
                : new NoContentResult();

        }
    }
}
