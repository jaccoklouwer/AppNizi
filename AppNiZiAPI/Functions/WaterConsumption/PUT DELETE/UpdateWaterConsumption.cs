using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using AppNiZiAPI.Variables;
using AppNiZiAPI.Models.Repositories;
using AppNiZiAPI.Security;
using AppNiZiAPI.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using AppNiZiAPI.Models;
using System.IO;
using Newtonsoft.Json.Linq;

namespace AppNiZiAPI.Functions.WaterConsumption.PUT
{
    public static class UpdateWaterConsumption
    {
        [FunctionName("UpdateWaterConsumption")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get","delete", Route = (Routes.APIVersion + Routes.SingleWaterConsumption))] HttpRequest req,
            ILogger log, int waterId)
        {
            int patientId;
            try
            {
                var content = await new StreamReader(req.Body).ReadToEndAsync();
                JObject jsonParsed = JObject.Parse(content);
                patientId = (int)jsonParsed["patientId"];

                #region AuthCheck
                AuthResultModel authResult = await DIContainer.Instance.GetService<IAuthorization>().CheckAuthorization(req, patientId);
                if (!authResult.Result)
                    return new StatusCodeResult(authResult.StatusCode);
                #endregion
            }
            catch 
            {
                return new BadRequestResult();
            }

            IWaterRepository waterRep = DIContainer.Instance.GetService<IWaterRepository>();

            // GET
            if (req.Method.ToLower() == "get")
            {
                WaterConsumptionModel waterConsumptionModel = waterRep.GetSingleWaterConsumption(patientId, waterId);
                return waterConsumptionModel != null
                    ? (ActionResult)new OkObjectResult(waterConsumptionModel)
                    : new StatusCodeResult(StatusCodes.Status204NoContent);
            }

            // Delete
            return waterRep.RemoveWaterConsumptions(patientId, waterId)
                ? (ActionResult)new OkResult()
                : new BadRequestResult();
        }
    }
}
