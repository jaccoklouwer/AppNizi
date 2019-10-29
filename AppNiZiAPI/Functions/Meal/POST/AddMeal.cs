using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AppNiZiAPI.Models.Repositories;
using AppNiZiAPI.Variables;
using AppNiZiAPI.Security;
using AppNiZiAPI.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace AppNiZiAPI.Functions.Meal.POST
{
    public static class AddMeal
    {
        [FunctionName("AddMeal")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function,  "post", Route = (Routes.APIVersion+Routes.AddMeal))] HttpRequest req,
            ILogger log,int patientId)
        {
            if (!await Authorization.CheckAuthorization(req, patientId)) { return new BadRequestObjectResult(Messages.AuthNoAcces); }
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //no way dat dit werkt WAT? wrm zo easy al me ingewikkelde shit :(
            Models.Meal meal = new Models.Meal();
            JsonConvert.PopulateObject(requestBody, meal);

            IMealRepository mealRepository = DIContainer.Instance.GetService<IMealRepository>();
            bool succes = mealRepository.AddMeal(meal);

            return succes != null
                ? (ActionResult)new OkObjectResult(succes)
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}
