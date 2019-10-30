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

namespace AppNiZiAPI.Functions.Doctor.GET
{
    public static class GetDoctorPatients
    {
        [FunctionName("GetDoctorPatients")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = (Routes.APIVersion + Routes.GetDoctorPatients))] HttpRequest req,
            ILogger log, int doctorId)
        {
            #region AuthCheck
            AuthResultModel authResult = await DIContainer.Instance.GetService<IAuthorization>().CheckAuthorization(req,doctorId,true);
            if (!authResult.Result)
                return new StatusCodeResult(authResult.StatusCode);
            #endregion

            IDoctorRepository doctorRepository = DIContainer.Instance.GetService<IDoctorRepository>();
            List<PatientObject> patients = doctorRepository.GetDoctorPatients(doctorId);

            return patients != null
                ? (ActionResult)new OkObjectResult(patients)
                : new NoContentResult();
        }
    }
}
