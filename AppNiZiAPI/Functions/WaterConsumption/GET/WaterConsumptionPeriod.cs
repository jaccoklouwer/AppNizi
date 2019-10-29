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
using AppNiZiAPI.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AppNiZiAPI.Functions.WaterConsumption.GET
{
    public static class WaterConsumptionPeriod
    {
        [FunctionName("WaterConsumptionPeriod")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = (Routes.APIVersion + Routes.GetWaterConsumptionPeriod))] HttpRequest req,
            ILogger log, int patientId)
        {
            // Parse Dates, could'nt work within one if statement because the out var
            if (DateTime.TryParse(req.Query["beginDate"], out var parsedBeginDate))
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            if (DateTime.TryParse(req.Query["endDate"], out var parsedEndDate))
                return new StatusCodeResult(StatusCodes.Status400BadRequest);

            IWaterRepository waterRep = DIContainer.Instance.GetService<IWaterRepository>();
            List<WaterConsumptionViewModel> listModel = waterRep.GetWaterConsumptionPeriod(patientId, parsedBeginDate, parsedEndDate);

            if (listModel.Count == 0)
                return new StatusCodeResult(StatusCodes.Status204NoContent);
            if (listModel[0].Error)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);

            var json = JsonConvert.SerializeObject(listModel);

            return new OkObjectResult(json);
        }
    }
}
