using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AppNiZiAPI.Models.AccountModels;
using AppNiZiAPI.Infrastructure;
using AppNiZiAPI.Security;
using AppNiZiAPI.Variables;
using Microsoft.Extensions.DependencyInjection;
using AppNiZiAPI.Models.Repositories;
using Newtonsoft.Json.Linq;
using AppNiZiAPI.Models;

namespace AppNiZiAPI.Functions.Doctor.POST
{
    public static class RegisterDoctor
    {
        [FunctionName("RegisterDoctor")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = (Routes.APIVersion + Routes.Doctor))] HttpRequest req,
            ILogger log)
        {
            AuthLogin authLogin = await DIContainer.Instance.GetService<IAuthorization>().LoginAuthAsync(req);
            if (authLogin == null) { return new BadRequestObjectResult(Messages.AuthNoAcces); }
            DoctorLogin newDoctor = new DoctorLogin { Account = new AccountModel(), Auth = new AuthLogin(), Doctor = new Models.Doctor() };

            try
            {
                StreamReader streamReader = new StreamReader(req.Body);
                var content = await streamReader.ReadToEndAsync();
                streamReader.Dispose();

                // Parse Patient Info
                JObject jsonParsed = JObject.Parse(content);
                newDoctor.Doctor.FirstName = jsonParsed["firstName"].ToString();
                newDoctor.Doctor.LastName = jsonParsed["lastName"].ToString();
                newDoctor.Doctor.Location = jsonParsed["location"].ToString();
                newDoctor.Auth.Guid = authLogin.Guid;
            }
            catch (Exception e)
            {
                log.LogInformation(e.Message);
                return new BadRequestResult();
            }

            IDoctorRepository doctorRepository = DIContainer.Instance.GetService<IDoctorRepository>();
            newDoctor = doctorRepository.RegisterDoctor(newDoctor);
            newDoctor.Auth = authLogin;

            return newDoctor != null
                ? (ActionResult)new OkObjectResult(newDoctor)
                : new BadRequestResult();
        }
    }
}
