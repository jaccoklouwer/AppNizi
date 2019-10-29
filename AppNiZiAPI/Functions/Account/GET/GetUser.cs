using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AppNiZiAPI.Variables;
using AppNiZiAPI.Models.Repositories;
using AppNiZiAPI.Models;
using System.Collections.Generic;
using AppNiZiAPI.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Newtonsoft.Json.Linq;
using AppNiZiAPI.Security;
using System;
using AppNiZiAPI.Models.AccountModels;

namespace AppNiZiAPI.Functions.Account.GET
{
    public static class GetUser
    {
        [FunctionName("GetUser")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = (Routes.APIVersion + Routes.GetUser))] HttpRequest req,
            ILogger log)
        {
            // Auth check
            AuthLogin authLogin = await DIContainer.Instance.GetService<IAuthorization>().LoginAuthAsync(req);
            if (authLogin == null) { return new BadRequestObjectResult(Messages.AuthNoAcces); }

            IPatientRepository patientRepository = DIContainer.Instance.GetService<IPatientRepository>();
            PatientLogin patientLogin = patientRepository.GetPatientInfo(authLogin.Guid);
            patientLogin.AuthLogin = authLogin;

            return new OkObjectResult(patientLogin);
        }
    }
}
