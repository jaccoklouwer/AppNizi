using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AppNiZiAPI.Variables;
using AppNiZiAPI.Models;
using AppNiZiAPI.Models.Repositories;
using System.Collections.Generic;
using System;
using System.Globalization;
using AppNiZiAPI.Security;

namespace AppNiZiAPI
{
    public static class GetConsumptionsForPatientBetweenDates
    {
        [FunctionName("GetConsumptionsForPatientBetweenDates")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = (Routes.APIVersion + Routes.Consumptions))] HttpRequest req,
            ILogger log)
        {
            DateTime startDate;
            DateTime endDate;
            string patientIdString;
            try
            {
                patientIdString = req.Query["patientId"];
                startDate = DateTime.ParseExact(req.Query["startDate"], "dd-MM-yyyy", CultureInfo.InvariantCulture);
                endDate = DateTime.ParseExact(req.Query["endDate"], "dd-MM-yyyy", CultureInfo.InvariantCulture);
            }
            catch(System.FormatException fe)
            {
                log.LogWarning(fe.Message);
                return new BadRequestObjectResult(Messages.ErrorInvalidDateValues);
            }
            catch (Exception e)
            {
                log.LogError(e.Message);
                return new BadRequestObjectResult(Messages.ErrorMissingValues);
            }
        
            if (!int.TryParse(patientIdString, out int patientId)) return new BadRequestObjectResult(Messages.ErrorIncorrectId);      
            List<Consumption> consumption = new ConsumptionRespository().GetConsumptionsForPatientBetweenDates(patientId, startDate, endDate);

            var consumptionJson = JsonConvert.SerializeObject(consumption);
            return consumptionJson != null
                ? (ActionResult)new OkObjectResult(consumptionJson)
                : new BadRequestObjectResult(Messages.ErrorIncorrectId);
        }
    }
}