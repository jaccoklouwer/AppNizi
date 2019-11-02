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
using System.Collections.Generic;
using AppNiZiAPI.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using AppNiZiAPI.Security;
using System.IO;
using Newtonsoft.Json.Linq;
using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;
using Microsoft.OpenApi.Models;
using System.Net;
using Aliencube.AzureFunctions.Extensions.OpenApi.Enums;

namespace AppNiZiAPI.Functions.WaterConsumption.GET
{
    public static class WaterConsumptionPeriod
    {
        [FunctionName("WaterConsumptionPeriod")]
        #region Swagger
        [OpenApiOperation("WaterConsumptionPeriod", "WaterConsumption", Summary = "Get waterconsumption from a date date", Description = "Get waterconsumption from a date date", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseBody(HttpStatusCode.OK, "application/json", typeof(WaterConsumptionViewModel[]), Summary = Messages.OKUpdate)]
        [OpenApiResponseBody(HttpStatusCode.Unauthorized, "application/json", typeof(string), Summary = Messages.AuthNoAcces)]
        [OpenApiResponseBody(HttpStatusCode.Forbidden, "application/json", typeof(string), Summary = Messages.AuthNoAcces)]
        [OpenApiResponseBody(HttpStatusCode.BadRequest, "application/json", typeof(string), Summary = Messages.ErrorPostBody)]
        [OpenApiParameter("patientId", Description = "Inserting the patientId", In = ParameterLocation.Path, Required = true, Type = typeof(int))]
        [OpenApiParameter("beginDate", Description = "Inserting the bein date", In = ParameterLocation.Query, Required = true, Type = typeof(string))]
        [OpenApiParameter("endDate", Description = "Inserting the end date", In = ParameterLocation.Query, Required = true, Type = typeof(string))] 
        #endregion
        public static async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = (Routes.APIVersion + Routes.GetWaterConsumptionPeriod))] HttpRequest req,
            ILogger log, int patientId)
        {
                #region AuthCheck
                AuthResultModel authResult = await DIContainer.Instance.GetService<IAuthorization>().AuthForDoctorOrPatient(req, patientId);
                if (!authResult.Result)
                    return new StatusCodeResult((int)authResult.StatusCode);
                #endregion
 
            // Parse Dates, could'nt work within one if statement because the out var
            if (!DateTime.TryParse(req.Query["beginDate"], out var parsedBeginDate))
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            if (!DateTime.TryParse(req.Query["endDate"], out var parsedEndDate))
                return new StatusCodeResult(StatusCodes.Status400BadRequest);

            IWaterRepository waterRep = DIContainer.Instance.GetService<IWaterRepository>();
            List<WaterConsumptionViewModel> listModel = waterRep.GetWaterConsumptionPeriod(patientId, parsedBeginDate, parsedEndDate);

            if (listModel.Count == 0)
                return new StatusCodeResult(StatusCodes.Status204NoContent);
            if (listModel[0].Error)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);

            var json = JsonConvert.SerializeObject(listModel);

            return new OkObjectResult(json);
        }
    }
}
