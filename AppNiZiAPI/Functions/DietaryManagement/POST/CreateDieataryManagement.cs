using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using AppNiZiAPI.Variables;
using Microsoft.Extensions.DependencyInjection;
using AppNiZiAPI.Models.Dietarymanagement;
using AppNiZiAPI.Infrastructure;
using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;
using Aliencube.AzureFunctions.Extensions.OpenApi.Enums;
using Microsoft.OpenApi.Models;
using AppNiZiAPI.Security;
using AppNiZiAPI.Models;
using AppNiZiAPI.Services;
using System.Collections.Generic;
using AppNiZiAPI.Services.Handlers;

namespace AppNiZiAPI.Functions.DietaryManagement.POST
{
    public static class DieataryManagement
    {
        [FunctionName(nameof(CreateDieataryManagement))]
        #region swagger
        [OpenApiOperation("CreateDieataryManagement", "DietaryManagement", Summary = "Create anew dietary managment", Description = "Create anew dietary managment of a patient", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseBody(HttpStatusCode.OK, "application/json", typeof(string), Summary = Messages.OKPost)]
        [OpenApiResponseBody(HttpStatusCode.Unauthorized, "application/json", typeof(string), Summary = Messages.AuthNoAcces)]
        [OpenApiResponseBody(HttpStatusCode.BadRequest, "application/json", typeof(string), Summary = Messages.ErrorPostBody)]
        [OpenApiResponseBody(HttpStatusCode.UnprocessableEntity, "application/json", typeof(string), Summary = Messages.ErrorPostBody)]
        [OpenApiRequestBody("application/json", typeof(DietaryManagementModel), Description = "the new values of the dietaryManagement")] 
        #endregion
        public static async Task<IActionResult> CreateDieataryManagement(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = (Routes.APIVersion + Routes.DietaryManagement))] HttpRequest req,
            ILogger log)
        {
            //link voor swagger https://devkimchi.com/2019/02/02/introducing-swagger-ui-on-azure-functions/
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            if (string.IsNullOrEmpty(requestBody))
                return new UnprocessableEntityObjectResult(Messages.ErrorMissingValues);

            DietaryManagementModel dietary = new DietaryManagementModel();
            JsonConvert.PopulateObject(requestBody, dietary);
            #region AuthCheck
            AuthResultModel authResult = await DIContainer.Instance.GetService<IAuthorization>().CheckAuthorization(req, dietary.PatientId);
            if (!authResult.Result)
                return new StatusCodeResult((int)authResult.StatusCode);
            #endregion

            Dictionary<ServiceDictionaryKey, object> dictionary = await DIContainer.Instance.GetService<IDietaryManagementService>().TryAddDietaryManagement(dietary);

            return DIContainer.Instance.GetService<IResponseHandler>().ForgeResponse(dictionary);
        }
    }
}
