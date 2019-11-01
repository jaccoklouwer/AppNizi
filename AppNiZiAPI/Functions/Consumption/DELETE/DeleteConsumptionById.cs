using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using AppNiZiAPI.Variables;
using AppNiZiAPI.Models.Repositories;
using Microsoft.Extensions.DependencyInjection;
using AppNiZiAPI.Infrastructure;
using AppNiZiAPI.Security;
using AppNiZiAPI.Models;
using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;
using Aliencube.AzureFunctions.Extensions.OpenApi.Enums;
using System.Net;
using Microsoft.OpenApi.Models;

namespace AppNiZiAPI
{
    public static class DeleteConsumptionById
    {
        [FunctionName("DeleteConsumptionById")]
        #region Swagger
        [OpenApiOperation(nameof(DeleteConsumptionById), "Consumption", Summary = "Deletes a consumption by id", Description = "Deletes a consumption of a patient by id", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter("consumptionId", Description = "the id of the consumption that is going to be deleted", In = ParameterLocation.Path, Required = true, Type = typeof(int))]
        [OpenApiResponseBody(HttpStatusCode.OK, "application/json", typeof(string), Summary = Messages.OKDelete)]
        [OpenApiResponseBody(HttpStatusCode.Unauthorized, "application/json", typeof(Error), Summary = Messages.AuthNoAcces)]
        [OpenApiResponseBody(HttpStatusCode.BadRequest, "application/json", typeof(Error), Summary = Messages.ErrorIncorrectId)]
        /*[OpenApiResponseBody(HttpStatusCode.BadRequest, "application/json", typeof(Error), Summary = Messages.ErrorDelete)]*/
        #endregion
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = (Routes.APIVersion + Routes.Consumption))] HttpRequest req,
            ILogger log, string consumptionId)
        {
            log.LogDebug($"Triggered '" + nameof(DeleteConsumptionById) + "' with parameter: '"+ consumptionId +"'");
            if (!int.TryParse(consumptionId, out int Id) || Id <= 0) return new BadRequestObjectResult(Messages.ErrorIncorrectId);

            IConsumptionRepository consumptionRepository = DIContainer.Instance.GetService<IConsumptionRepository>();
            int patientId = consumptionRepository.GetConsumptionByConsumptionId(Id).PatientId;

            // Auth check
            AuthResultModel authResult = await DIContainer.Instance.GetService<IAuthorization>().CheckAuthorization(req, patientId);
            if (!authResult.Result)
                return new StatusCodeResult((int)authResult.StatusCode);

            bool deleted = consumptionRepository.DeleteConsumption(Id, patientId);

            return deleted
                ? (ActionResult)new OkObjectResult(Messages.OKDelete)
                : new BadRequestObjectResult(Messages.ErrorDelete);
        }
    }
}
