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
using AppNiZiAPI.Models.Repositories;
using AppNiZiAPI.Models.Handlers;
using AppNiZiAPI.Models;

namespace AppNiZiAPI.Functions.Patients.GET
{
    public static class GetPatientByGUID
    {
        [FunctionName("GetPatientByGUID")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = (Routes.APIVersion + Routes.Patients + "/{guid}"))] HttpRequest req, string guid,
            ILogger log)
        {
            log.LogInformation($"C# HTTP trigger function processed a request. {guid}");

            if (string.IsNullOrEmpty(guid))
                return new BadRequestObjectResult("No GUID parameter passed.");

            try
            {
                PatientObject patient = new PatientRepository().Select(guid);

                // Return object if possible
                return patient != null
                ? (ActionResult)new OkObjectResult(patient)
                : new BadRequestObjectResult("Bad request. Check GUID.");
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
