
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using AppNiZiAPI.Variables;
using AppNiZiAPI.Security;
using AppNiZiAPI.Models.Repositories;

namespace AppNiZiAPI
{
    public static class DeleteConsumptionById
    {
        [FunctionName("DeleteConsumptionById")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = (Routes.APIVersion + Routes.Consumption))] HttpRequest req,
            ILogger log, string consumptionId)
        {
            if (!int.TryParse(consumptionId, out int Id) || Id <= 0) return new BadRequestObjectResult(Messages.ErrorIncorrectId);

            bool deleted = new ConsumptionRespository().DeleteConsumption(Id);

            return deleted
                ? (ActionResult)new OkObjectResult(Messages.OKDelete)
                : new BadRequestObjectResult(Messages.ErrorDelete);
        }
    }
}
