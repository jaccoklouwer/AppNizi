using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AppNiZiAPI.Variables;
using AppNiZiAPI.Models.Dietarymanagement;
using AppNiZiAPI.Models;
using System.Collections.Generic;
using AppNiZiAPI.Models.Repositories;
using AppNiZiAPI.Security;
using AppNiZiAPI.Infrastructure;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;
using Microsoft.OpenApi.Models;
using Aliencube.AzureFunctions.Extensions.OpenApi.Enums;

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

            List<DietaryManagementModel> dietaryManagementModels;
            List<DietaryRestriction> dietaryRestrictions;
            DietaryView view;
            try
            {
                IDietaryManagementRepository repository = DIContainer.Instance.GetService<IDietaryManagementRepository>();
                dietaryManagementModels = await repository.GetDietaryManagementByPatientAsync(id);
                dietaryRestrictions = await repository.GetDietaryRestrictions();
                view = new DietaryView();
                view.DietaryManagements = dietaryManagementModels;
                view.restrictions = dietaryRestrictions;
            }
            catch (Exception e)
            {
                log.LogInformation(e.Message);
                return new NotFoundObjectResult(Messages.ErrorMissingValues + e.Message);
            }

            if (dietaryManagementModels == null)
                return new BadRequestObjectResult("No Dietarymanagement was found!");

            string json = JsonConvert.SerializeObject(view);

            return json != null
                ? (ActionResult)new OkObjectResult(json)
                : new BadRequestObjectResult(Messages.ErrorMissingValues);
        }
    }
}

