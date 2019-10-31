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
using AppNiZiAPI.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using AppNiZiAPI.Security;
using Newtonsoft.Json.Linq;

namespace AppNiZiAPI.Functions.Patients.GET
{
    public static class GetPatientById
    {
        [FunctionName("GetPatientByID")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = (Routes.APIVersion + Routes.SpecificPatient))] HttpRequest req, int patientId,
            ILogger log)
        {
            bool isDoctor = false;
            if (patientId == 0)
                return new BadRequestObjectResult("No patientId parameter passed.");

            try
            {
                var content = await new StreamReader(req.Body).ReadToEndAsync();
                JObject jsonParsed = JObject.Parse(content);
                if (jsonParsed.ContainsKey("Role") && jsonParsed["Role"].ToString() == "Doctor")
                    isDoctor = true;
            }
            catch{}

            #region AuthCheck
            AuthResultModel authResult = await DIContainer.Instance.GetService<IAuthorization>().CheckAuthorization(req, patientId, isDoctor);
            if (!authResult.Result)
                return new StatusCodeResult((int)authResult.StatusCode);
            #endregion

            try
            {
                IPatientRepository patientRepository = DIContainer.Instance.GetService<IPatientRepository>();
                PatientObject patient = patientRepository.Select(patientId);

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
