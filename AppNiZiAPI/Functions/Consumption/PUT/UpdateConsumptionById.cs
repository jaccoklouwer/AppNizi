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
using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;
using Aliencube.AzureFunctions.Extensions.OpenApi.Enums;
using System.Net;
using Microsoft.OpenApi.Models;
using AppNiZiAPI.Models.Handlers;
using AppNiZiAPI.Services;

namespace AppNiZiAPI
{

    public static class UpdateConsumptionById
    {
        [FunctionName("UpdateConsumptionById")]
        #region Swagger
        [OpenApiOperation(nameof(UpdateConsumptionById), "Consumption", Summary = "Updates a consumption", Description = "Updates a consumption of a patient by using the consumption id located in the url path and the consumption data from the requestbody. Only available for patient.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter("consumptionId", Description = "the id of the consumption that is targeted", In = ParameterLocation.Path, Required = true, Type = typeof(int))]
        [OpenApiRequestBody("application/json", typeof(ConsumptionInput))]
        [OpenApiResponseBody(HttpStatusCode.OK, "application/json", typeof(string), Summary = Messages.OKUpdate)]
        [OpenApiResponseBody(HttpStatusCode.Unauthorized, "application/json", typeof(Error), Summary = Messages.AuthNoAcces)]
        [OpenApiResponseBody(HttpStatusCode.BadRequest, "application/json", typeof(Error), Summary = Messages.ErrorPut)]
        #endregion
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = (Routes.APIVersion + Routes.Consumption))] HttpRequest req,
            ILogger log, string consumptionId)
        {
            log.LogDebug($"Triggered '" + typeof(UpdateConsumptionById).Name + "' with parameter: '" + consumptionId + "'");
            return await DIContainer.Instance.GetService<IConsumptionService>().UpdateConsumption(req, consumptionId);
        }
    }
}
