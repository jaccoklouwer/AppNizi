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

namespace AppNiZiAPI.Functions.Doctor.DELETE
{
    public static class DeleteDoctor
    {
        [FunctionName("DeleteDoctor")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = (Routes.APIVersion + Routes.SpecificDoctor))] HttpRequest req,
            ILogger log, int doctorId)
        {
            #region AuthCheck
            AuthResultModel authResult = await DIContainer.Instance.GetService<IAuthorization>().CheckAuthorization(req, doctorId);
            if (!authResult.Result)
                return new StatusCodeResult(authResult.StatusCode);
            #endregion

            if (doctorId == 0)
                return new BadRequestObjectResult("No doctor id parameter passed.");

            try
            {
                IPatientRepository patientRepository = DIContainer.Instance.GetService<IPatientRepository>();
                bool success = patientRepository.Delete(doctorId);

                if (success)
                    return new OkObjectResult("Deleted.");
                else
                    return new NotFoundObjectResult("Deletion failed, invalid GUID?");
            }
            catch (Exception ex)
            {
                // Build error message and return it.
                string callbackMessage = new MessageHandler().BuildErrorMessage(ex);
                return new BadRequestObjectResult(callbackMessage);
            }
        }
    }
}
