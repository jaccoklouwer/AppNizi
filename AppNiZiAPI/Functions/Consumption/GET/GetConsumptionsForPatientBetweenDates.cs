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
    public static class GetConsumptionsForPatientBetweenDates
    {
        [FunctionName("GetConsumptionsForPatientBetweenDates")]
        #region Swagger
        [OpenApiOperation(nameof(GetConsumptionsForPatientBetweenDates), "Consumption", Summary = "Gets all consumptions for a patient between 2 dates", Description = "Gets all consumptions for a patientid between a startdate and enddate. Available for patient and doctor of patient.", Visibility = OpenApiVisibilityType.Important)]
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
            return await DIContainer.Instance.GetService<IConsumptionService>().GetConsumptionsForPatientBetweenDates(req);
        }
    }
}
