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
using AppNiZiAPI.Models.Handler;

namespace AppNiZiAPI
{
    public static class GetConsumptionById
    {
        [FunctionName("GetConsumptionById")]
        public static async Task<IActionResult> GetConsumptionByConsumptionId(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = (Routes.APIVersion + Routes.GetConsumptionById))] HttpRequest req,
            ILogger log, string consumptionId)
        {
            if (!int.TryParse(consumptionId, out int id)) return new BadRequestObjectResult(Messages.ErrorMissingValues);
            
            Consumption consumption = new ConsumptionRespository().GetConsumptionByConsumptionId(id);
            
            var consumptionJson = JsonConvert.SerializeObject(consumption);
            return consumptionJson != null && consumption.Id != 0
                ? (ActionResult)new OkObjectResult(consumptionJson)
                : new BadRequestObjectResult(Messages.ErrorIncorrectId);
        }
    }
}
