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
using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;
using System.Net;
using Microsoft.OpenApi.Models;
using Aliencube.AzureFunctions.Extensions.OpenApi.Enums;
using AppNiZiAPI.Services;
using System.Collections.Generic;
using AppNiZiAPI.Services.Handlers;

namespace AppNiZiAPI.Functions.WaterConsumption.PUT_DELETE
{
    public static class DeleteWaterConsumption
    {
        [FunctionName("DeleteWaterConsumption")]
        #region Swagger
        [OpenApiOperation("DeleteWaterConsumption", "WaterConsumption", Summary = "Delete water consumption", Description = "Delete water consumption", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseBody(HttpStatusCode.OK, "application/json", typeof(string), Summary = Messages.OKDelete)]
        [OpenApiResponseBody(HttpStatusCode.Unauthorized, "application/json", typeof(Error), Summary = Messages.AuthNoAcces)]
        [OpenApiResponseBody(HttpStatusCode.BadRequest, "application/json", typeof(Error), Summary = Messages.ErrorPostBody)]
        [OpenApiResponseBody(HttpStatusCode.UnprocessableEntity, "application/json", typeof(Error), Summary = Messages.ErrorPostBody)]
        [OpenApiParameter("waterId", Description = "Single input id", In = ParameterLocation.Path, Required = true, Type = typeof(int))]
        #endregion
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = (Routes.APIVersion + Routes.SingleWaterConsumption))] HttpRequest req,
            ILogger log, int waterId)
        {
            int patientId = await DIContainer.Instance.GetService<IAuthorization>().GetUserId(req);
            if (patientId == 0)
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);

            Dictionary<ServiceDictionaryKey, object> dictionary = DIContainer.Instance.GetService<IWaterService>().TryDeleteWaterConsumption(waterId, patientId);

            return DIContainer.Instance.GetService<IResponseHandler>().ForgeResponse(dictionary);
        }
    }
}
