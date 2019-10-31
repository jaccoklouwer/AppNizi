using System.IO;
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
using Aliencube.AzureFunctions.Extensions.OpenApi.Enums;
using System.Net;
using Microsoft.OpenApi.Models;

namespace AppNiZiAPI
{
    public static class AddConsumption
    {
        [FunctionName("AddConsumption")]
        #region Swagger
        [OpenApiOperation(nameof(AddConsumption), "Consumption", Summary = "Adds a consumption", Description = "Adds a consumption for a patient by using the consumption data from the requestbody", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiRequestBody("application/json", typeof(Consumption))]
        [OpenApiResponseBody(HttpStatusCode.OK, "application/json", typeof(string), Summary = Messages.OKPost)]
        [OpenApiResponseBody(HttpStatusCode.Unauthorized, "application/json", typeof(Error), Summary = Messages.AuthNoAcces)]
        [OpenApiResponseBody(HttpStatusCode.BadRequest, "application/json", typeof(Error), Summary = Messages.ErrorIncorrectId)]
        [OpenApiResponseBody(HttpStatusCode.BadRequest, "application/json", typeof(Error), Summary = Messages.ErrorPost)]
        #endregion
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = (Routes.APIVersion + Routes.Consumptions))] HttpRequest req,
            ILogger log)
        {
            log.LogDebug($"Triggered '" + nameof(AddConsumption) + "'");

            Consumption newConsumption = new Consumption();
            string consumptionJson = await new StreamReader(req.Body).ReadToEndAsync();
            JsonConvert.PopulateObject(consumptionJson, newConsumption);

            int patientId = newConsumption.PatientId;
            // Auth check
            AuthResultModel authResult = await DIContainer.Instance.GetService<IAuthorization>().CheckAuthorization(req, patientId);
            if (!authResult.Result)
                return new StatusCodeResult((int)authResult.StatusCode);


            IConsumptionRepository consumptionRepository = DIContainer.Instance.GetService<IConsumptionRepository>();
            if (consumptionRepository.AddConsumption(newConsumption))
            {
                return new OkObjectResult(Messages.OKPost);
            }
            return new BadRequestObjectResult(Messages.ErrorPost);
        }
    }
}
