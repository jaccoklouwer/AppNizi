using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AppNiZiAPI.Variables;
using Microsoft.Extensions.DependencyInjection;
using AppNiZiAPI.Infrastructure;
using AppNiZiAPI.Models.Dietarymanagement;
using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;
using Aliencube.AzureFunctions.Extensions.OpenApi.Enums;
using System.Net;
using Microsoft.OpenApi.Models;
using AppNiZiAPI.Security;
using AppNiZiAPI.Models;
using AppNiZiAPI.Services;
using System.Collections.Generic;
using AppNiZiAPI.Services.Handlers;

namespace AppNiZiAPI.Functions.DietaryManagement.PUT
{

    public static class DietaryManagement
    {

        [FunctionName(nameof(UpdateDietaryManagement))]
        #region swagger
        [OpenApiOperation("UpdateDietaryManagement", "DietaryManagement", Summary = "Updates a dietary managment", Description = "updates the dietary management of a patient", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseBody(HttpStatusCode.OK, "application/json", typeof(string), Summary = Messages.OKUpdate)]
        [OpenApiResponseBody(HttpStatusCode.Unauthorized, "application/json", typeof(Error), Summary = Messages.AuthNoAcces)]
        [OpenApiResponseBody(HttpStatusCode.BadRequest, "application/json", typeof(Error), Summary = Messages.ErrorPostBody)]
        [OpenApiResponseBody(HttpStatusCode.UnprocessableEntity, "application/json", typeof(Error), Summary = Messages.ErrorMissingValues)]
        [OpenApiRequestBody("application/json", typeof(DietaryManagementModel), Description = "the new values of the dietaryManagement")]
        [OpenApiParameter("dietId", Description = "the id of the diet that is going to be updated", In = ParameterLocation.Path, Required = true, Type = typeof(int))]
        #endregion
        public static async Task<IActionResult> UpdateDietaryManagement(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = (Routes.APIVersion + Routes.DietaryManagementById))]
            HttpRequest req, int dietId,
            ILogger log)
        {

            //link voor swagger https://medium.com/@yuka1984/open-api-swagger-and-swagger-ui-on-azure-functions-v2-c-a4a460b34b55
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            DietaryManagementModel dietary = new DietaryManagementModel();
            JsonConvert.PopulateObject(requestBody, dietary);
            AuthResultModel authResult = await DIContainer.Instance.GetService<IAuthorization>().CheckAuthorization(req, dietary.PatientId);
            if (!authResult.Result)
                return new StatusCodeResult((int)authResult.StatusCode);

            Dictionary<ServiceDictionaryKey, object> dictionary = await DIContainer.Instance.GetService<IDietaryManagementService>().TryUpdateDietaryManagement(dietId, dietary);

            return DIContainer.Instance.GetService<IResponseHandler>().ForgeResponse(dictionary);
        }
    }
}
