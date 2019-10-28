using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using AppNiZiAPI.Models;
using AppNiZiAPI.Variables;
using AppNiZiAPI.Models.Repositories;


namespace AppNiZiAPI.Functions.DietaryManagement.POST
{
    public static class DieataryManagement
    {
        /// <summary>
        /// Create DieataryManagement
        /// </summary>
        /// <param name="req"
        /// <returns></returns>
        [FunctionName(nameof(CreateDieataryManagement))]
        public static async Task<IActionResult> CreateDieataryManagement(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = (Routes.APIVersion + Routes.DietaryManagement))] HttpRequest req,
            ILogger log)
        {
            //link voor swagger https://devkimchi.com/2019/02/02/introducing-swagger-ui-on-azure-functions/
            log.LogInformation("C# HTTP trigger function processed a request.");
            //if (!await Authorization.CheckAuthorization(req.Headers)) { return new UnauthorizedResult(); }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            if (string.IsNullOrEmpty(requestBody))
                return new UnprocessableEntityObjectResult(Messages.ErrorMissingValues);
            IDietaryManagementRepository repository = new DietaryManagementRepository();
            
            try
            {
                DietaryManagementModel dietary = JsonConvert.DeserializeObject<DietaryManagementModel>(requestBody);
                bool success = repository.AddDietaryManagement(dietary);
                if (success)
                {
                    return new OkObjectResult(Messages.OKPost);
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
