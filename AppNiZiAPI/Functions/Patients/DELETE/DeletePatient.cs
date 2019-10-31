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

namespace AppNiZiAPI.Functions.Patients.DELETE
{
    public static class DeletePatient
    {
        [FunctionName("DeletePatient")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = (Routes.APIVersion + Routes.SpecificPatient))] HttpRequest req, int patientId,
            ILogger log)
        {

            #region AuthCheck
            AuthResultModel authResult = await DIContainer.Instance.GetService<IAuthorization>().CheckAuthorization(req, patientId);
            if (!authResult.Result)
                return new StatusCodeResult((int)authResult.StatusCode);
            #endregion

            if (patientId == 0)
                return new BadRequestObjectResult("No patientId parameter passed.");

            try
            {
                IPatientRepository patientRepository = DIContainer.Instance.GetService<IPatientRepository>();
                bool success = patientRepository.Delete(patientId);

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
