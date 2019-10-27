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
using AppNiZiAPI.Models.Repositories;

namespace AppNiZiAPI
{

    public static class UpdateConsumptionById
    {
        [FunctionName("UpdateConsumptionById")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = (Routes.APIVersion + Routes.Consumption))] HttpRequest req,
            ILogger log, string consumptionId)
        {
            if (!int.TryParse(consumptionId, out int id)) return new BadRequestObjectResult(Messages.ErrorIncorrectId);
            Consumption updateConsumption = new Consumption();
            string consumptionJson = await new StreamReader(req.Body).ReadToEndAsync();
            JsonConvert.PopulateObject(consumptionJson, updateConsumption);

            // TODO: What if Consumption.Id != consumptionId??

            if (new ConsumptionRespository().UpdateConsumption(id, updateConsumption))
            {
                return new OkObjectResult(Messages.OKPost);
            }
            return new BadRequestObjectResult(Messages.ErrorPost);
        }
    }
}