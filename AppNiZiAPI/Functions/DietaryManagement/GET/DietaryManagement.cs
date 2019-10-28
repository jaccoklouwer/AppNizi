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
using Authorization = AppNiZiAPI.Security.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace AppNiZiAPI.Functions.DietaryManagement.GET
{
    public static class DietaryManagement
    {
        /// <summary>
        /// Get DietaryManagement
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(DietaryManagementModel[]))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(Error))]
        [FunctionName(nameof(GetDietaryManagementByPatient))]
        public static async Task<IActionResult> GetDietaryManagementByPatient(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = (Routes.APIVersion + Routes.GetDietaryManagement))] HttpRequest req, int patientId,
            ILogger log)
        {
            //link voor swagger https://devkimchi.com/2019/02/02/introducing-swagger-ui-on-azure-functions/
            if (!await Authorization.CheckAuthorization(req, patientId)) { return new BadRequestObjectResult(Messages.AuthNoAcces); }

            List<DietaryManagementModel> dietaryManagementModels;
            List<DietaryRestriction> dietaryRestrictions;
            DietaryView view;
            try
            {
                IDietaryManagementRepository repository = DIContainer.Instance.GetService<IDietaryManagementRepository>();
                dietaryManagementModels = repository.GetDietaryManagementByPatient(patientId);
                dietaryRestrictions = repository.GetDietaryRestrictions();
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

