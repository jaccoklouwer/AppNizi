using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using AppNiZiAPI.Variables;
using AppNiZiAPI.Security;
using System.IO;
using Newtonsoft.Json.Linq;
using System;
using AppNiZiAPI.Models;
using Microsoft.Extensions.DependencyInjection;
using AppNiZiAPI.Models.Repositories;
using AppNiZiAPI.Infrastructure;
using AppNiZiAPI.Models.Handlers;
using Aliencube.AzureFunctions.Extensions.OpenApi.Enums;
using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;
using System.Net;
using Microsoft.OpenApi.Models;

namespace AppNiZiAPI.Functions.WaterConsumption.PUT
{
    public static class UpdateWaterConsumption
    {
        [FunctionName("UpdateWaterConsumption")]
        #region Swagger
        [OpenApiOperation("UpdateWaterConsumption", "WaterConsumption", Summary = "Update water consumption", Description = "Update water consumption", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseBody(HttpStatusCode.OK, "application/json", typeof(WaterConsumptionModel), Summary = Messages.OKResult)]
        [OpenApiResponseBody(HttpStatusCode.Unauthorized, "application/json", typeof(Error), Summary = Messages.AuthNoAcces)]
        [OpenApiResponseBody(HttpStatusCode.BadRequest, "application/json", typeof(Error), Summary = Messages.ErrorPostBody)]
        [OpenApiResponseBody(HttpStatusCode.NotFound, "application/json", typeof(Error), Summary = Messages.ErrorPostBody)]
        [OpenApiRequestBody("application/json", typeof(WaterConsumptionModel), Description = "Id can be null or zero")]
        #endregion
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = (Routes.APIVersion + Routes.SingleWaterConsumption))] HttpRequest req,
            ILogger log, int waterId)
        {
            int patientId = await DIContainer.Instance.GetService<IAuthorization>().GetUserId(req);
            if (patientId == 0)
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);

            WaterConsumptionModel model = new WaterConsumptionModel();
            IWaterRepository waterRep = DIContainer.Instance.GetService<IWaterRepository>();

            try
            {
                var content = await new StreamReader(req.Body).ReadToEndAsync();
                var jsonParsed = JObject.Parse(content);

                model = new WaterConsumptionModel()
                {
                    PatientId = patientId,
                    Amount = (int)jsonParsed["amount"],
                    Id = waterId
                };
                DateTime.TryParse(jsonParsed["date"].ToString(), out var date);
                if (date != null)
                    model.Date = date;
            }
            catch
            {
                return new BadRequestObjectResult(Messages.ErrorPost);
            }

            model = await waterRep.InsertWaterConsumption(model, true);

            return model != null
                ? (ActionResult)new OkObjectResult(model)
                : new BadRequestResult();
        }
    }
}
