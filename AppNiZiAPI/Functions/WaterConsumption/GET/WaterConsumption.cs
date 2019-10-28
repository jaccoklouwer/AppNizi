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


namespace AppNiZiAPI.Functions.WaterConsumption.GET
{
    public static class WaterConsumption
    {
        [FunctionName("WaterConsumption")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = (Routes.APIVersion + Routes.GetWaterConsumption))] HttpRequest req,
            ILogger log, int patientId, string date)
        {
            
            // Auth check
            if (!await Authorization.CheckAuthorization(req, patientId)) { return new BadRequestObjectResult(Messages.AuthNoAcces); }

            WaterRepository rep = new WaterRepository();
            WaterConsumptionDaily model = rep.GetWaterConsumption(patientId, date);

            if (model.WaterConsumptions.Count == 0)
            {
                return new StatusCodeResult(StatusCodes.Status204NoContent);
            }

            var json = JsonConvert.SerializeObject(model);

            return new OkObjectResult(json);
        }
    }


}
