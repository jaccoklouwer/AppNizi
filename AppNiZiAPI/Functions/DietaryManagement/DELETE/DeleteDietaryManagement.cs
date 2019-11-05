using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using AppNiZiAPI.Variables;
using Microsoft.Extensions.DependencyInjection;
using AppNiZiAPI.Infrastructure;
using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;
using System.Net;
using Aliencube.AzureFunctions.Extensions.OpenApi.Enums;
using Microsoft.OpenApi.Models;
using AppNiZiAPI.Models;
using AppNiZiAPI.Security;
using AppNiZiAPI.Services;
using System.Collections.Generic;
using AppNiZiAPI.Services.Handlers;

namespace AppNiZiAPI.Functions.DietaryManagement.DELETE
{
    public static class DietaryManagement
    {
        [FunctionName(nameof(DeleteDietaryManagement))]

        #region swagger
        [OpenApiOperation(nameof(DeleteDietaryManagement), "DietaryManagement", Summary = "Delete a dietary managment", Description = "Delete a dietary managment of a patient", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseBody(HttpStatusCode.OK, "application/json", typeof(string), Summary = Messages.OKDelete)]
        [OpenApiResponseBody(HttpStatusCode.Unauthorized, "application/json", typeof(Error), Summary = Messages.AuthNoAcces)]
        [OpenApiResponseBody(HttpStatusCode.BadRequest, "application/json", typeof(Error), Summary = Messages.ErrorPostBody)]
        [OpenApiResponseBody(HttpStatusCode.UnprocessableEntity, "application/json", typeof(Error), Summary = Messages.ErrorPostBody)]
        [OpenApiParameter("dietId", Description = "the id of the diet that is going to be updated", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiRequestBody("patientId", typeof(int), Description = "the id of a patient for authentication")] 
        #endregion
        public static async Task<IActionResult> DeleteDietaryManagement(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = (Routes.APIVersion + Routes.DietaryManagementById))] HttpRequest req, string dietId,
            ILogger log)
        {
            //link voor swagger https://devkimchi.com/2019/02/02/introducing-swagger-ui-on-azure-functions/
            log.LogInformation("C# HTTP trigger function processed a request.");
            int id;

            if (!int.TryParse(dietId, out id))
                return new UnprocessableEntityObjectResult(Messages.ErrorIncorrectId);

            #region AuthCheck
            AuthResultModel authResult = await DIContainer.Instance.GetService<IAuthorization>().CheckAuthorization(req);
            if (!authResult.Result)
                return new StatusCodeResult((int)authResult.StatusCode);
            #endregion

            Dictionary<ServiceDictionaryKey, object> dictionary = await DIContainer.Instance.GetService<IDietaryManagementService>().TryDeleteDietaryManagement(id);


            return DIContainer.Instance.GetService<IResponseHandler>().ForgeResponse(dictionary);
        }
    }
}
