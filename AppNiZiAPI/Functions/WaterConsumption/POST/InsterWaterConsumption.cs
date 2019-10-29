using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using AppNiZiAPI.Variables;
using AppNiZiAPI.Security;
using System.IO;
using Newtonsoft.Json.Linq;
using System;
using AppNiZiAPI.Models;
using Microsoft.Extensions.DependencyInjection;
using AppNiZiAPI.Models.Repositories;
using AppNiZiAPI.Infrastructure;
using AppNiZiAPI.Models.Handlers;

namespace AppNiZiAPI.Functions.WaterConsumption.POST
{
    public static class InsterWaterConsumption
    {
        [FunctionName("InsterWaterConsumption")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = (Routes.APIVersion + Routes.PostWaterConsumption))] HttpRequest req,
            ILogger log)
        {
            WaterConsumptionModel model = new WaterConsumptionModel();

            try
            {
                var content = await new StreamReader(req.Body).ReadToEndAsync();
                var jsonParsed = JObject.Parse(content);
                int patientId = (int)jsonParsed["patientId"];
                if (!await Authorization.CheckAuthorization(req, patientId)) { return new BadRequestObjectResult(Messages.AuthNoAcces); }

                model = new WaterConsumptionModel()
                {
                    PatientId = patientId,
                    Amount = (int)jsonParsed["amount"],
                    Date = Convert.ToDateTime(jsonParsed["date"].ToString())
                };
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(Messages.ErrorPost);
            }

            IWaterRepository waterRep = DIContainer.Instance.GetService<IWaterRepository>();
            Result result = waterRep.InsertWaterConsumption(model);

            if (result.Succesfull)
                return new OkObjectResult(result);
            return new BadRequestObjectResult(result);
        }
    }
}
