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
using Aliencube.AzureFunctions.Extensions.OpenApi.Enums;
using System.Net;
using AppNiZiAPI.Models.SwaggerModels;

namespace AppNiZiAPI.Functions.Account.POST
{
    public static class RegisterPatient
    {
        [FunctionName("RegisterPatient")]
        #region Swagger
        [OpenApiOperation("RegisterPatient", "Patient", Summary = "Register a new patient", Description = "Register a new patient", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseBody(HttpStatusCode.OK, "application/json", typeof(PatientLogin), Summary = Messages.OKResult)]
        [OpenApiResponseBody(HttpStatusCode.Unauthorized, "application/json", typeof(Error), Summary = Messages.AuthNoAcces)]
        [OpenApiResponseBody(HttpStatusCode.BadRequest, "application/json", typeof(Error), Summary = Messages.ErrorPostBody)]
        [OpenApiResponseBody(HttpStatusCode.NotFound, "application/json", typeof(Error), Summary = Messages.ErrorPostBody)]
        [OpenApiRequestBody("application/json", typeof(SwaggerRegisterPatient), Description = "New patient")]
        #endregion
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = (Routes.APIVersion + Routes.Patients))] HttpRequest req,
            ILogger log)
        {
            // Auth check
            AuthLogin authLogin = await DIContainer.Instance.GetService<IAuthorization>().LoginAuthAsync(req);
            if (authLogin == null) { return new BadRequestObjectResult(Messages.AuthNoAcces); }
            PatientLogin newPatient;

            try
            {
                StreamReader streamReader = new StreamReader(req.Body);
                var content = await streamReader.ReadToEndAsync();
                streamReader.Dispose();

                // Parse Patient Info
                JObject jsonParsed = JObject.Parse(content);

                newPatient = new PatientLogin
                {
                    Patient = new Patient
                    {
                        FirstName = jsonParsed["firstName"].ToString(),
                        LastName = jsonParsed["lastName"].ToString(),
                        DateOfBirth = (DateTime)jsonParsed["dateOfBirth"],
                        WeightInKilograms = (float)jsonParsed["weight"],
                        Guid = authLogin.Guid
                    },
                    Doctor = new DoctorModel
                    {
                        DoctorId = (int)jsonParsed["doctorId"]
                    }
                };
            }
            catch (Exception e)
            {
                log.LogInformation(e.Message);
                return new BadRequestResult();
            }
            
            IPatientRepository patientRepository = DIContainer.Instance.GetService<IPatientRepository>();
            newPatient = patientRepository.RegisterPatient(newPatient);
            newPatient.AuthLogin = authLogin;

            return newPatient != null
                ? (ActionResult)new OkObjectResult(newPatient)
                : new BadRequestResult();
        }
    }
}
