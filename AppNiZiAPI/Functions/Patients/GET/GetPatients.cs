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
using AppNiZiAPI.Models;
using AppNiZiAPI.Models.Views;
using System.Collections.Generic;
using AppNiZiAPI.Models.Repositories;
using AppNiZiAPI.Models.Handler;

namespace AppNiZiAPI.Functions.Patients
{
    /// <summary>
    /// Retrieve an array of all patients.
    /// </summary>
    public static class GetPatients
    {
        [FunctionName("GetPatients")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = (Routes.APIVersion + Routes.Patients))] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            int count = new QueryHandler().ExtractIntegerFromRequest("count", req);

            List<PatientView> patients = new PatientRepository().List(count);

            dynamic data = JsonConvert.SerializeObject(patients);
            
            return patients != null
                ? (ActionResult)new OkObjectResult(data)
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}
