using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AppNiZiAPI.Variables;
using System.Net.Http;
using AppNiZiAPI.Models;
using AppNiZiAPI.Models.Repositories;
using System.Net;
using AppNiZiAPI.Models.Handlers;

namespace AppNiZiAPI.Functions.Patients.POST
{


    public static class CreatePatient
    {




        [FunctionName("CreatePatient")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = (Routes.APIVersion + Routes.Patients))] HttpRequestMessage req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // Check authorization, if not authorized we're gonna return a 401 here.

            string jsonContent = await req.Content.ReadAsStringAsync();

            // No object received
            if (string.IsNullOrEmpty(jsonContent))
                return req.CreateResponse(HttpStatusCode.BadRequest, "Invalid object received.");

            try
            {
                // Received object from client
                PatientObject patient = JsonConvert.DeserializeObject<PatientObject>(jsonContent);

                // Insert and fetch object
                int id = new PatientRepository().Insert(patient);
                PatientObject returnedObj = new PatientRepository().Select(id);

                // Return response
                return req.CreateResponse(HttpStatusCode.Created, returnedObj);
            }
            catch (Exception ex)
            {
                // Build error message and return it.
                string callbackMessage = new MessageHandler().BuildErrorMessage(ex);
                return req.CreateResponse(HttpStatusCode.BadRequest, callbackMessage);
            }
        }
    }

}