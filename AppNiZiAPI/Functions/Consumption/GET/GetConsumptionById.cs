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
using AppNiZiAPI.Security;
using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;
using System.Net;
using Microsoft.OpenApi.Models;
using Aliencube.AzureFunctions.Extensions.OpenApi.Enums;
using AppNiZiAPI.Services;

namespace AppNiZiAPI
{
    public static class GetConsumptionById
    {
        [FunctionName("GetConsumptionById")]
        #region Swagger
        [OpenApiOperation(nameof(GetConsumptionById), "Consumption", Summary = "Gets a consumption by id", Description = "Gets a consumption of a patient by id. Available for patient and doctor of patient.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter("consumptionId", Description = "the id of the consumption that is targeted", In = ParameterLocation.Path, Required = true, Type = typeof(int))]
        [OpenApiResponseBody(HttpStatusCode.OK, "application/json", typeof(ConsumptionView))]
        [OpenApiResponseBody(HttpStatusCode.Unauthorized, "application/json", typeof(Error), Summary = Messages.AuthNoAcces)]
        [OpenApiResponseBody(HttpStatusCode.BadRequest, "application/json", typeof(Error), Summary = Messages.ErrorIncorrectId)]
        #endregion
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = (Routes.APIVersion + Routes.Consumption))] HttpRequest req,
            ILogger log, string consumptionId)
        {
            log.LogDebug($"Triggered '" + nameof(GetConsumptionById) + "' with parameter: '" + consumptionId + "'");
            return await DIContainer.Instance.GetService<IConsumptionService>().GetConsumptionByConsumptionId(req, consumptionId);
        }
    }
}
