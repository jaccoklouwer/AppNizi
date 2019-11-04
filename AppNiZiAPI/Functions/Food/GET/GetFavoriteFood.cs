using System;
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
using System.Collections.Generic;
using AppNiZiAPI.Security;

using AppNiZiAPI.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using AppNiZiAPI.Models;
using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;
using System.Net;
using Microsoft.OpenApi.Models;
using Aliencube.AzureFunctions.Extensions.OpenApi.Enums;
using AppNiZiAPI.Services;
using AppNiZiAPI.Services.Handlers;

namespace AppNiZiAPI.Functions.Food
{
    public static class GetFavoriteFood
    {
        [FunctionName("GetFavoriteFood")]
        [OpenApiOperation("GetFavoriteFood", "Food", Summary = "Gets the users favorite foods", Description = "Retrieves the favorited fooditems of the specified patientID", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseBody(HttpStatusCode.OK, "application/json", typeof(Models.Food[]), Summary = Messages.OKUpdate)]
        [OpenApiResponseBody(HttpStatusCode.Unauthorized, "application/json", typeof(Error), Summary = Messages.AuthNoAcces)]
        [OpenApiResponseBody(HttpStatusCode.BadRequest, "application/json", typeof(Error), Summary = Messages.ErrorMissingValues)]
        [OpenApiParameter("patientId", Description = "the id of the patient which items should be got", In = ParameterLocation.Path, Required = true, Type = typeof(int))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous,  "get", Route = (Routes.APIVersion + Routes.GetFavoriteFood))] HttpRequest req,
            ILogger log, int patientId)
        {
            // Auth check
            AuthResultModel authResult = await DIContainer.Instance.GetService<IAuthorization>().AuthForDoctorOrPatient(req, patientId);
            if (!authResult.Result)
                return new StatusCodeResult((int)authResult.StatusCode);

            Dictionary<ServiceDictionaryKey, object> dictionary = await DIContainer.Instance.GetService<IFoodService>().TryGetFavoriteFood(patientId);

            return DIContainer.Instance.GetService<IResponseHandler>().ForgeResponse(dictionary);
        }
    }
}
