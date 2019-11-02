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
using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;
using System.Net;
using Aliencube.AzureFunctions.Extensions.OpenApi.Enums;
using AppNiZiAPI.Models.SwaggerModels;

namespace AppNiZiAPI.Functions.Doctor.POST
{
    public static class RegisterDoctor
    {
        [FunctionName("RegisterDoctor")]
        #region Swagger
        [OpenApiOperation("RegisterDoctor", "Doctor", Summary = "Register a new doctor", Description = "Register a new doctor", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseBody(HttpStatusCode.OK, "application/json", typeof(DoctorLogin), Summary = Messages.OKResult)]
        [OpenApiResponseBody(HttpStatusCode.Unauthorized, "application/json", typeof(Error), Summary = Messages.AuthNoAcces)]
        [OpenApiResponseBody(HttpStatusCode.BadRequest, "application/json", typeof(Error), Summary = Messages.ErrorPostBody)]
        [OpenApiResponseBody(HttpStatusCode.NotFound, "application/json", typeof(Error), Summary = Messages.ErrorPostBody)]
        [OpenApiRequestBody("application/json", typeof(SwaggerRegisterDoctor), Description = "New doctor")] 
        #endregion
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = (Routes.APIVersion + Routes.Doctor))] HttpRequest req,
            ILogger log)
        {
            AuthLogin authLogin = await DIContainer.Instance.GetService<IAuthorization>().LoginAuthAsync(req);
            if (authLogin == null) { return new BadRequestObjectResult(Messages.AuthNoAcces); }
            DoctorLogin newDoctor;

            try
            {
                StreamReader streamReader = new StreamReader(req.Body);
                var content = await streamReader.ReadToEndAsync();
                streamReader.Dispose();

                // Parse Patient Info
                JObject jsonParsed = JObject.Parse(content);
                newDoctor = new DoctorLogin
                {
                    Doctor = new DoctorModel
                    {
                        FirstName = jsonParsed["firstName"].ToString(),
                        LastName = jsonParsed["lastName"].ToString(),
                        Location = jsonParsed["location"].ToString()
                    },
                    Auth = new AuthLogin
                    {
                        Guid = authLogin.Guid
                    }
                };
            }
            catch { return new BadRequestResult(); }

            IDoctorRepository doctorRepository = DIContainer.Instance.GetService<IDoctorRepository>();
            newDoctor = doctorRepository.RegisterDoctor(newDoctor);
            newDoctor.Auth = authLogin;

            return newDoctor != null
                ? (ActionResult)new OkObjectResult(newDoctor)
                : new BadRequestResult();
        }
    }
}
