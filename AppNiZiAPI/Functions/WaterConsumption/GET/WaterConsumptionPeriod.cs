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
using AppNiZiAPI.Models;
using System.Collections.Generic;

namespace AppNiZiAPI.Functions.WaterConsumption.GET
{
    public static class WaterConsumptionPeriod
    {
        [FunctionName("WaterConsumptionPeriod")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = (Routes.APIVersion + Routes.GetWaterConsumptionPeriod))] HttpRequest req,
            ILogger log, int patientId)
        {
            string beginDate = req.Query["beginDate"];
            string endDate = req.Query["endDate"];

            if(beginDate == null || endDate == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }

            WaterRepository rep = new WaterRepository();
            List<WaterConsumptionViewModel> listModel = rep.GetWaterConsumptionPeriod(patientId, beginDate, endDate);

            if (listModel.Count == 0)
            {
                return new StatusCodeResult(StatusCodes.Status204NoContent);
            }

            var json = JsonConvert.SerializeObject(listModel);

            return new OkObjectResult(json);
        }
    }
}