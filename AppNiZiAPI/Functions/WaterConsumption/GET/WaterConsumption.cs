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

namespace AppNiZiAPI.Functions.WaterConsumption.GET
{
    public static class WaterConsumption
    {
        [FunctionName("WaterConsumption")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = (Routes.APIVersion + Routes.GetWaterConsumption))] HttpRequest req,
            ILogger log, int patientId, string date)
        {
            #region AuthCheck
            AuthResultModel authResult = await DIContainer.Instance.GetService<IAuthorization>().CheckAuthorization(req, patientId);
            if (!authResult.Result)
                return new StatusCodeResult(authResult.StatusCode);
            #endregion

            IWaterRepository waterRep = DIContainer.Instance.GetService<IWaterRepository>();
            WaterConsumptionDaily model = waterRep.GetWaterConsumption(patientId, date);

            if (model == null || model.WaterConsumptions.Count == 0)
                return new StatusCodeResult(StatusCodes.Status204NoContent);

            var json = JsonConvert.SerializeObject(model);

            return new OkObjectResult(json);
        }
    }
}
