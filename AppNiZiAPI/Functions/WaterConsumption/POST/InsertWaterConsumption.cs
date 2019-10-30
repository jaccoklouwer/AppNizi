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
    public static class InsertWaterConsumption
    {
        [FunctionName("InsertrWaterConsumption")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", "put", Route = (Routes.APIVersion + Routes.PostWaterConsumption))] HttpRequest req,
            ILogger log)
        {
            WaterConsumptionModel model = new WaterConsumptionModel();
            bool update = false;
            IWaterRepository waterRep = DIContainer.Instance.GetService<IWaterRepository>();

            try
            {
                var content = await new StreamReader(req.Body).ReadToEndAsync();
                var jsonParsed = JObject.Parse(content);
                int patientId = (int)jsonParsed["patientId"];

                #region AuthCheck
                AuthResultModel authResult = await DIContainer.Instance.GetService<IAuthorization>().CheckAuthorization(req, patientId);
                if (!authResult.Result)
                    return new StatusCodeResult(authResult.StatusCode);
                #endregion

                model = new WaterConsumptionModel()
                {
                    PatientId = patientId,
                    Amount = (int)jsonParsed["amount"],
                    Date = Convert.ToDateTime(jsonParsed["date"].ToString())
                };

                if(req.Method.ToLower() == "put")
                {
                    model.Id = (int)jsonParsed["id"];
                    update = true;
                }
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(Messages.ErrorPost);
            }

            Result result = waterRep.InsertWaterConsumption(model, update);

            return result.Succesfull 
                ? (ActionResult)new OkObjectResult(result) 
                : new BadRequestObjectResult(result);
        }
    }
}
