using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using AppNiZiAPI.Variables;
using AppNiZiAPI.Models.Dietarymanagement;
using AppNiZiAPI.Models;
using System.Collections.Generic;
using AppNiZiAPI.Security;
using AppNiZiAPI.Infrastructure;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;
using Microsoft.OpenApi.Models;
using Aliencube.AzureFunctions.Extensions.OpenApi.Enums;
using AppNiZiAPI.Services;
using AppNiZiAPI.Services.Handlers;

namespace AppNiZiAPI.Functions.DietaryManagement.GET
{
    public static class DietaryManagement
    {
        [FunctionName(nameof(GetDietaryManagementByPatient))]
        #region swagger
        [OpenApiOperation(nameof(GetDietaryManagementByPatient), "DietaryManagement", Summary = "Updates a dietary managment", Description = "updates the dietary management of a patient", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseBody(HttpStatusCode.OK, "application/json", typeof(DietaryView), Summary = Messages.OKResult)]
        [OpenApiResponseBody(HttpStatusCode.Unauthorized, "application/json", typeof(Error), Summary = Messages.AuthNoAcces)]
        [OpenApiResponseBody(HttpStatusCode.BadRequest, "application/json", typeof(Error), Summary = Messages.ErrorPostBody)]
        [OpenApiResponseBody(HttpStatusCode.NotFound, "application/json", typeof(Error), Summary = Messages.ErrorPostBody)]
        [OpenApiParameter("patientId", Description = "the id of the diet that is going to be updated", In = ParameterLocation.Path, Required = true, Type = typeof(string))] 
        #endregion
        public static async Task<IActionResult> GetDietaryManagementByPatient(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = (Routes.APIVersion + Routes.GetDietaryManagement))] HttpRequest req, string patientId,
            ILogger log)
        {
            //link voor swagger https://devkimchi.com/2019/02/02/introducing-swagger-ui-on-azure-functions/

            int id;
            if (!int.TryParse(patientId, out id))
                return new UnprocessableEntityObjectResult(Messages.ErrorIncorrectId);

            #region AuthCheck
            AuthResultModel authResult = await DIContainer.Instance.GetService<IAuthorization>().AuthForDoctorOrPatient(req, id);
            if (!authResult.Result)
                return new StatusCodeResult((int)authResult.StatusCode);
            #endregion

            Dictionary<ServiceDictionaryKey, object> dictionary = await DIContainer.Instance.GetService<IDietaryManagementService>().TryGetDietaryManagementByPatient(id);



            return DIContainer.Instance.GetService<IResponseHandler>().ForgeResponse(dictionary);
        }
    }
}

