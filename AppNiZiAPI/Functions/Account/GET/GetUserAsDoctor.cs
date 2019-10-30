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
using System.Threading.Tasks;

namespace AppNiZiAPI.Functions.Account.GET
{
    public static class GetUserAsDoctor
    {
        [FunctionName("GetUserAsDoctor")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = (Routes.APIVersion + Routes.DoctorMe))] HttpRequest req,
            ILogger log)
        {
            AuthLogin authLogin = await DIContainer.Instance.GetService<IAuthorization>().LoginAuthAsync(req);
            if (authLogin == null) { return new BadRequestObjectResult(Messages.AuthNoAcces); }

            IDoctorRepository doctorRepository = DIContainer.Instance.GetService<IDoctorRepository>();
            DoctorLogin doctorLogin = doctorRepository.GetLoggedInDoctor(authLogin.Guid);

            return doctorLogin != null
                ? (ActionResult)new OkObjectResult(doctorLogin)
                : new BadRequestResult();
        }
    }
}
