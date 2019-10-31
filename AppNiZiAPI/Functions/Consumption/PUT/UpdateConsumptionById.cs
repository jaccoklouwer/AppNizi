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
using Microsoft.Extensions.DependencyInjection;
using AppNiZiAPI.Infrastructure;
using AppNiZiAPI.Security;

namespace AppNiZiAPI
{

    public static class UpdateConsumptionById
    {
        [FunctionName("UpdateConsumptionById")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = (Routes.APIVersion + Routes.Consumption))] HttpRequest req,
            ILogger log, string consumptionId)
        {
            log.LogDebug($"Triggered '" + typeof(UpdateConsumptionById).Name + "' with parameter: '" + consumptionId + "'");

            if (!int.TryParse(consumptionId, out int id)) return new BadRequestObjectResult(Messages.ErrorIncorrectId);

            IConsumptionRepository consumptionRepository = DIContainer.Instance.GetService<IConsumptionRepository>();
            int targetPatientId = consumptionRepository.GetConsumptionByConsumptionId(id).PatientId;

            Consumption updateConsumption = new Consumption();
            string consumptionJson = await new StreamReader(req.Body).ReadToEndAsync();
            JsonConvert.PopulateObject(consumptionJson, updateConsumption);

            // Check if updated consumption patientId equals target patientId
            if (updateConsumption.PatientId != targetPatientId) return new BadRequestObjectResult(Messages.ErrorPost);

            // Auth check
            AuthResultModel authResult = await DIContainer.Instance.GetService<IAuthorization>().CheckAuthorization(req, targetPatientId);
            if (!authResult.Result)
                return new StatusCodeResult((int)authResult.StatusCode);

            
            if (consumptionRepository.UpdateConsumption(id, updateConsumption))
            {
                return new OkObjectResult(Messages.OKPost);
            }
            return new BadRequestObjectResult(Messages.ErrorPost);
        }
    }
}
