using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AppNiZiAPI.Variables;
using AppNiZiAPI.Models.Repositories;
using AppNiZiAPI.Models.Water;
using AppNiZiAPI.Security;
using AppNiZiAPI.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Net;
using AppNiZiAPI.Models;
using System.IO;
using Newtonsoft.Json.Linq;
using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;
using Microsoft.OpenApi.Models;
using Aliencube.AzureFunctions.Extensions.OpenApi.Enums;
using System;

namespace AppNiZiAPI.Functions.WaterConsumption.GET
{
    public static class DailyWaterConsumption
    {
        [FunctionName("DailyWaterConsumption")]
        [OpenApiOperation("DailyWaterConsumption", "WaterConsumption", Summary = "Get waterconsumption from a date", Description = "Get waterconsumption from a date", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseBody(HttpStatusCode.OK, "application/json", typeof(WaterConsumptionDaily), Summary = Messages.OKUpdate)]
        [OpenApiResponseBody(HttpStatusCode.Unauthorized, "application/json", typeof(string), Summary = Messages.AuthNoAcces)]
        [OpenApiResponseBody(HttpStatusCode.Forbidden, "application/json", typeof(string), Summary = Messages.AuthNoAcces)]
        [OpenApiResponseBody(HttpStatusCode.BadRequest, "application/json", typeof(string), Summary = Messages.ErrorPostBody)]
        [OpenApiParameter("patientId:int", Description = "Inserting the patientId", In = ParameterLocation.Path, Required = true, Type = typeof(int))]
        [OpenApiParameter("date", Description = "Inserting the date", In = ParameterLocation.Query, Required = true, Type = typeof(string))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = (Routes.APIVersion + Routes.GetDailyWaterConsumption))] HttpRequest req,
            ILogger log, int patientId)
        {
            #region AuthCheck
            AuthResultModel authResult = await DIContainer.Instance.GetService<IAuthorization>().AuthForDoctorOrPatient(req, patientId);
            if (!authResult.Result)
                return new StatusCodeResult(authResult.StatusCode);
            #endregion

            if (!DateTime.TryParse(req.Query["date"], out var parsedDate))
                return new StatusCodeResult(StatusCodes.Status400BadRequest);

            IWaterRepository waterRep = DIContainer.Instance.GetService<IWaterRepository>();
            WaterConsumptionDaily model = waterRep.GetDailyWaterConsumption(patientId, parsedDate);

            return model != null || model.WaterConsumptions.Count != 0
                ? (ActionResult)new OkObjectResult(model)
                : new StatusCodeResult(StatusCodes.Status204NoContent);
        }
    }
}
