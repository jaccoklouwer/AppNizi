using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;
using Aliencube.AzureFunctions.Extensions.OpenApi.Enums;
using AppNiZiAPI.Infrastructure;
using AppNiZiAPI.Models;
using AppNiZiAPI.Services;
using AppNiZiAPI.Variables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Threading.Tasks;

namespace AppNiZiAPI
{
    public static class DeleteConsumptionById
    {
        [FunctionName("DeleteConsumptionById")]
        #region Swagger
        [OpenApiOperation(nameof(DeleteConsumptionById), "Consumption", Summary = "Deletes a consumption by id", Description = "Deletes a consumption of a patient by id. Only available for patient.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter("consumptionId", Description = "the id of the consumption that is going to be deleted", In = ParameterLocation.Path, Required = true, Type = typeof(int))]
        [OpenApiResponseBody(HttpStatusCode.OK, "application/json", typeof(string), Summary = Messages.OKDelete)]
        [OpenApiResponseBody(HttpStatusCode.Unauthorized, "application/json", typeof(Error), Summary = Messages.AuthNoAcces)]
        [OpenApiResponseBody(HttpStatusCode.BadRequest, "application/json", typeof(Error), Summary = Messages.ErrorDelete)]
        #endregion
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = (Routes.APIVersion + Routes.Consumption))] HttpRequest req,
            ILogger log, string consumptionId)
        {
            log.LogDebug($"Triggered '" + nameof(DeleteConsumptionById) + "' with parameter: '"+ consumptionId +"'");
            return await DIContainer.Instance.GetService<IConsumptionService>().RemoveConsumption(req, consumptionId);
        }
    }
}
