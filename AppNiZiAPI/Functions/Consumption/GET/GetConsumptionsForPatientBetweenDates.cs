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
using System;
using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using AppNiZiAPI.Infrastructure;
using AppNiZiAPI.Security;
using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;
using System.Net;
using Microsoft.OpenApi.Models;
using Aliencube.AzureFunctions.Extensions.OpenApi.Enums;

namespace AppNiZiAPI
{
    public static class GetConsumptionsForPatientBetweenDates
    {
        [FunctionName("GetConsumptionsForPatientBetweenDates")]
        #region Swagger
        [OpenApiOperation(nameof(GetConsumptionsForPatientBetweenDates), "Consumption", Summary = "Gets all consumptions for a patient between 2 dates", Description = "Gets all consumptions for a patientid between a startdate and enddate", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter("patientId", Description = "the id of the patient which adds a consumption", In = ParameterLocation.Query, Required = true, Type = typeof(int))]
        [OpenApiParameter("startDate", Description = "the start date", In = ParameterLocation.Query, Required = true, Type = typeof(string))]
        [OpenApiParameter("endDate", Description = "the end date", In = ParameterLocation.Query, Required = true, Type = typeof(string))]
        [OpenApiResponseBody(HttpStatusCode.OK, "application/json", typeof(PatientConsumptionsView))]
        [OpenApiResponseBody(HttpStatusCode.Unauthorized, "application/json", typeof(Error), Summary = Messages.AuthNoAcces)]
        [OpenApiResponseBody(HttpStatusCode.BadRequest, "application/json", typeof(Error), Summary = Messages.ErrorIncorrectId)]
        #endregion
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = (Routes.APIVersion + Routes.Consumptions))] HttpRequest req,
            ILogger log)
        {
            log.LogDebug($"Triggered '" + nameof(GetConsumptionsForPatientBetweenDates) + "'");

            DateTime startDate;
            DateTime endDate;
            string patientIdString;
            try
            {
                patientIdString = req.Query["patientId"];
                startDate = DateTime.ParseExact(req.Query["startDate"], "dd-MM-yyyy", CultureInfo.InvariantCulture);
                endDate = DateTime.ParseExact(req.Query["endDate"], "dd-MM-yyyy", CultureInfo.InvariantCulture);
            }
            catch(System.FormatException fe)
            {
                log.LogWarning(fe.Message);
                return new BadRequestObjectResult(Messages.ErrorInvalidDateValues);
            }
            catch (Exception e)
            {
                log.LogError(e.Message);
                return new BadRequestObjectResult(Messages.ErrorMissingValues);
            }
        
            if (!int.TryParse(patientIdString, out int patientId)) return new BadRequestObjectResult(Messages.ErrorIncorrectId);

            // Auth check
            AuthResultModel authResult = await DIContainer.Instance.GetService<IAuthorization>().CheckAuthorization(req, patientId);
            if (!authResult.Result)
                return new StatusCodeResult((int)authResult.StatusCode);

            IConsumptionRepository consumptionRepository = DIContainer.Instance.GetService<IConsumptionRepository>();
            PatientConsumptionsView consumptions = new PatientConsumptionsView(consumptionRepository.GetConsumptionsForPatientBetweenDates(patientId, startDate, endDate));

            var consumptionJson = JsonConvert.SerializeObject(consumptions);
            return consumptionJson != null
                ? (ActionResult)new OkObjectResult(consumptionJson)
                : new BadRequestObjectResult(Messages.ErrorIncorrectId);
        }
    }
}
