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
using AppNiZiAPI.Models;
using AppNiZiAPI.Models.Repositories;
using Microsoft.Extensions.DependencyInjection;
using AppNiZiAPI.Infrastructure;
using AppNiZiAPI.Models.Dietarymanagement;

namespace AppNiZiAPI.Functions.DietaryManagement.PUT
{

    public static class DietaryManagement
    {
        /// <summary>
        /// Create DieataryManagement
        /// </summary>
        /// <param name="req"></param>
        /// <param name="dietId"></param>
        /// <returns></returns>
        [FunctionName(nameof(RuUpdateDietaryManagementn))]
        public static async Task<IActionResult> RuUpdateDietaryManagementn(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = (Routes.APIVersion + Routes.DietaryManagementById))]
            HttpRequest req, int dietId,
            ILogger log)
        {

            //link voor swagger https://medium.com/@yuka1984/open-api-swagger-and-swagger-ui-on-azure-functions-v2-c-a4a460b34b55
            log.LogInformation("C# HTTP trigger function processed a request.");
            //if (!await Authorization.CheckAuthorization(req, patientId)) { return new UnauthorizedResult(); }


            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            if (string.IsNullOrEmpty(requestBody))
                return new UnprocessableEntityObjectResult(Messages.ErrorMissingValues);

            IDietaryManagementRepository repository = DIContainer.Instance.GetService<IDietaryManagementRepository>();
            try
            {
                DietaryManagementModel dietary = JsonConvert.DeserializeObject<DietaryManagementModel>(requestBody);
                bool success = repository.UpdateDietaryManagement(dietId, dietary);

                if (success)
                {
                    return new OkObjectResult(Messages.OKUpdate);
                }
                else
                {
                    return new BadRequestObjectResult(Messages.ErrorPostBody);
                }
            }
            catch (Exception e)
            {

                return new NotFoundObjectResult(Messages.ErrorMissingValues + e.Message);
            }
        }
    }
}
