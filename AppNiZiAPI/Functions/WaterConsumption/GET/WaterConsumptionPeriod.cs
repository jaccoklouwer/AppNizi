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
using AppNiZiAPI.Security;
using System.IO;
using Newtonsoft.Json.Linq;

namespace AppNiZiAPI.Functions.WaterConsumption.GET
{
    public static class WaterConsumptionPeriod
    {
        [FunctionName("WaterConsumptionPeriod")]
        public static async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = (Routes.APIVersion + Routes.GetWaterConsumptionPeriod))] HttpRequest req,
            ILogger log)
        {

            int patientId;
            bool isDoctor = false;
            try
            {
                StreamReader streamReader = new StreamReader(req.Body);
                var content = await streamReader.ReadToEndAsync();
                streamReader.DiscardBufferedData();
                JObject jsonParsed = JObject.Parse(content);
                patientId = (int)jsonParsed["patientId"];
                if (jsonParsed.ContainsKey("Role") && jsonParsed["Role"].ToString() == "Doctor")
                    isDoctor = true;

                #region AuthCheck
                AuthResultModel authResult = await DIContainer.Instance.GetService<IAuthorization>().CheckAuthorization(req, patientId, isDoctor);
                if (!authResult.Result)
                    return new StatusCodeResult(authResult.StatusCode);
                #endregion
            }
            catch
            {
                return new BadRequestResult();
            }

            // Parse Dates, could'nt work within one if statement because the out var
            if (!DateTime.TryParse(req.Query["beginDate"], out var parsedBeginDate))
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            if (!DateTime.TryParse(req.Query["endDate"], out var parsedEndDate))
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
