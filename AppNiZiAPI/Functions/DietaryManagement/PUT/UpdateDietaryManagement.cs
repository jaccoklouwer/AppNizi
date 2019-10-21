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
using AppNiZiAPI.Security;

namespace AppNiZiAPI.Functions.DietaryManagement.PUT
{
    public static class UpdateDietaryManagement
    {
        [FunctionName("UpdateDietaryManagement")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = (Routes.APIVersion + Routes.DietaryManagementById))] HttpRequest req, int dietId,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            if (!await Authorization.CheckAuthorization(req.Headers)) { return new BadRequestObjectResult(Messages.AuthNoAcces); }      

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            if (string.IsNullOrEmpty(requestBody))
                return new BadRequestObjectResult(Messages.ErrorMissingValues);

            DietaryManagementModel dietary = JsonConvert.DeserializeObject<DietaryManagementModel>(requestBody);

            DietaryManagementRepository repository = new DietaryManagementRepository();
            try
            {
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
            catch (Exception)
            {

                return new BadRequestObjectResult(Messages.ErrorServer);
            }
        }
    }
}
