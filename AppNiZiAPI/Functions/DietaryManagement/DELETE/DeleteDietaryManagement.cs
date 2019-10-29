using System;
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
using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;
using System.Net;
using Aliencube.AzureFunctions.Extensions.OpenApi.Enums;
using Microsoft.OpenApi.Models;
using AppNiZiAPI.Models;
using AppNiZiAPI.Security;
using System.IO;
using Newtonsoft.Json;

namespace AppNiZiAPI.Functions.DietaryManagement.DELETE
{
    public static class DietaryManagement
    {
        [FunctionName(nameof(DeleteDietaryManagement))]
        [OpenApiOperation("CreateDieataryManagement", "DietaryManagement", Summary = "Delete a dietary managment", Description = "Delete a dietary managment of a patient", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseBody(HttpStatusCode.OK, "application/json", typeof(string), Summary = Messages.OKUpdate)]
        [OpenApiResponseBody(HttpStatusCode.Unauthorized, "application/json", typeof(string), Summary = Messages.AuthNoAcces)]
        [OpenApiResponseBody(HttpStatusCode.BadRequest, "application/json", typeof(string), Summary = Messages.ErrorPostBody)]
        [OpenApiResponseBody(HttpStatusCode.UnprocessableEntity, "application/json", typeof(string), Summary = Messages.ErrorPostBody)]
        [OpenApiParameter("dietId", Description = "the id of the diet that is going to be updated", In = ParameterLocation.Path, Required = true, Type = typeof(int))]
        public static async Task<IActionResult> DeleteDietaryManagement(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = (Routes.APIVersion + Routes.DietaryManagementById))] HttpRequest req, int dietId,
            ILogger log)
        {
            //link voor swagger https://devkimchi.com/2019/02/02/introducing-swagger-ui-on-azure-functions/
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            int patientId;
            if (string.IsNullOrEmpty(requestBody))
                return new UnprocessableEntityObjectResult(Messages.ErrorMissingValues);
            try
            {
                patientId = JsonConvert.DeserializeObject<int>(requestBody);
            }
            catch (Exception)
            {
                return new UnprocessableEntityObjectResult(Messages.ErrorIncorrectId);
            }

            AuthResultModel authResult = await DIContainer.Instance.GetService<IAuthorization>().CheckAuthorization(req, patientId);
            if (!authResult.Result)
                return new StatusCodeResult(authResult.StatusCode);

            IDietaryManagementRepository repository = DIContainer.Instance.GetService<IDietaryManagementRepository>();
            bool success;
            try
            {
                success = repository.DeleteDietaryManagement(dietId);
            }
            catch (Exception)
            {
                return new NotFoundObjectResult(Messages.ErrorMissingValues);
            }

            return success
                ? (ActionResult)new BadRequestObjectResult(Messages.ErrorMissingValues)
                : new OkObjectResult(Messages.OKDelete);
        }
    }
}
