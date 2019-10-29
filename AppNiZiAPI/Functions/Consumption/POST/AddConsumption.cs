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
using Microsoft.Extensions.DependencyInjection;
using AppNiZiAPI.Infrastructure;

namespace AppNiZiAPI
{
    public static class AddConsumption
    {
        [FunctionName("AddConsumption")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = (Routes.APIVersion + Routes.Consumptions))] HttpRequest req,
            ILogger log)
        {
            log.LogDebug($"Triggered '" + typeof(AddConsumption).Name + "'");

            Consumption newConsumption = new Consumption();
            string consumptionJson = await new StreamReader(req.Body).ReadToEndAsync();
            JsonConvert.PopulateObject(consumptionJson, newConsumption);

            IConsumptionRepository consumptionRepository = DIContainer.Instance.GetService<IConsumptionRepository>();
            if (consumptionRepository.AddConsumption(newConsumption))
            {
                return new OkObjectResult(Messages.OKPost);
            }
            return new BadRequestObjectResult(Messages.ErrorPost);
        }
    }
}
