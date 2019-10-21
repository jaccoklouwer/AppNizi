using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;
using AppNiZiAPI.Variables;
using AppNiZiAPI.Models;
using System.Collections.Generic;
using AppNiZiAPI.Models.Repositories;
using AppNiZiAPI.Security;

namespace AppNiZiAPI.Functions.DietaryManagement
{
    public static class DietaryManagement
    {
        [FunctionName("DietaryManagement")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = (Routes.APIVersion + Routes.GetDietaryManagement))] HttpRequest req, string patientId,
            ILogger log)
        {
            if (!await Authorization.CheckAuthorization(req.Headers)) { return new BadRequestObjectResult(Messages.AuthNoAcces); }
            int id = 0;
            if (!int.TryParse(patientId, out id)) { return new BadRequestObjectResult(Messages.ErrorMissingValues); }

            List<DietaryManagementModel> dietaryManagementModels = null;
            try
            {
                DietaryManagementRepository repository = new DietaryManagementRepository();
                dietaryManagementModels = repository.GetDietaryManagementByPatient(id);
            }
            catch (Exception e)
            {
                log.LogInformation(e.Message);
                return new BadRequestObjectResult(Messages.ErrorMissingValues);
            }

            if (dietaryManagementModels == null)
                return new BadRequestObjectResult("No Dietarymanagement was found!");

            string json = JsonConvert.SerializeObject(dietaryManagementModels);

            return json != null
                ? (ActionResult)new OkObjectResult(json)
                : new BadRequestObjectResult(Messages.ErrorMissingValues);
        }
    }
}

