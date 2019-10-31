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


namespace AppNiZiAPI.Functions.Account.POST
{
    public static class RegisterPatient
    {
        [FunctionName("RegisterPatient")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = (Routes.APIVersion + Routes.Patients))] HttpRequest req,
            ILogger log)
        {
            // Auth check
            AuthLogin authLogin = await DIContainer.Instance.GetService<IAuthorization>().LoginAuthAsync(req);
            if (authLogin == null) { return new BadRequestObjectResult(Messages.AuthNoAcces); }
            PatientLogin newPatient = new PatientLogin { Patient = new PatientObject(), Account = new AccountModel(), AuthLogin = new AuthLogin(), Doctor = new Models.DoctorModel()  };

            try
            {
                StreamReader streamReader = new StreamReader(req.Body);
                var content = await streamReader.ReadToEndAsync();
                streamReader.Dispose();

                // Parse Patient Info
                JObject jsonParsed = JObject.Parse(content);
                newPatient.Patient.FirstName = jsonParsed["firstName"].ToString();
                newPatient.Patient.LastName = jsonParsed["lastName"].ToString();
                newPatient.Patient.DateOfBirth = (DateTime)jsonParsed["dateOfBirth"];
                newPatient.Patient.WeightInKilograms = (float)jsonParsed["weight"];
                newPatient.Doctor.DoctorId = (int)jsonParsed["doctorId"];
                newPatient.Patient.Guid = authLogin.Guid;
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
