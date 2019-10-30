using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AppNiZiAPI.Variables;
using AppNiZiAPI.Models.Repositories;
using AppNiZiAPI.Models.Water;
using AppNiZiAPI.Security;
using AppNiZiAPI.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Net;
using AppNiZiAPI.Models;
using System.IO;
using Newtonsoft.Json.Linq;

namespace AppNiZiAPI.Functions.WaterConsumption.GET
{
    public static class DailyWaterConsumption
    {
        [FunctionName("DailyWaterConsumption")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = (Routes.APIVersion + Routes.GetDailyWaterConsumption))] HttpRequest req,
            ILogger log, string date)
        {
            int patientId;
            bool isDoctor = false;
            try
            {
                var content = await new StreamReader(req.Body).ReadToEndAsync();
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

            IWaterRepository waterRep = DIContainer.Instance.GetService<IWaterRepository>();
            WaterConsumptionDaily model = waterRep.GetDailyWaterConsumption(patientId, date);

            if (model == null || model.WaterConsumptions.Count == 0)
                return new StatusCodeResult(StatusCodes.Status204NoContent);

            var json = JsonConvert.SerializeObject(model);

            return new OkObjectResult(json);
        }
    }
}
