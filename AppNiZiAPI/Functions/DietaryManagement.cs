using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;
using AppNiZiAPI.Variables;
using AppNiZiAPI.Models;
using System.Collections.Generic;

namespace AppNiZiAPI.Functions
{
    public static class DietaryManagement
    {
        [FunctionName("DietaryManagement")]
        public static async Task<IActionResult> GetDietaryManagement(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = (Routes.APIVersion + Routes.DietaryManagement))] HttpRequest req,
            ILogger log)
        {
            var connString = Environment.GetEnvironmentVariable("sqldb_connection");
            List<DietaryManagementModel> dietaryManagementModels = new List<DietaryManagementModel>();

            TestPatient patient = new TestPatient();

            using (SqlConnection conn = new SqlConnection(connString))
            {
                //Haal info uit de Body, moet nog worden aangepast vanwege Oauth enzo.
                try
                {
                    var content = await new StreamReader(req.Body).ReadToEndAsync();
                    log.LogInformation(content); // Log het in console
                    patient = JsonConvert.DeserializeObject<TestPatient>(content);
                }
                catch (Exception e)
                {
                    log.LogInformation(e.Message);
                    return new BadRequestObjectResult(Messages.ErrorMissingValues);
                }

                conn.Open();
                var text = $"SELECT b.description, a.amount, a.is_active, a.patient_id, a.id " +
                    $"FROM DietaryManagement AS a, DietaryRestriction AS b " +
                    $"WHERE a.dietary_restriction_id = b.id AND a.patient_id={patient.PatientId}";

                using (SqlCommand cmd = new SqlCommand(text, conn))
                {
                    try
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            DietaryManagementModel dietaryManagementModel = new DietaryManagementModel();
                            // Uit lezen bijv
                            dietaryManagementModel.Id = Int32.Parse(reader["id"].ToString());
                            dietaryManagementModel.Description = reader["description"].ToString();
                            dietaryManagementModel.Amount = Int32.Parse(reader["amount"].ToString());
                            dietaryManagementModel.PatientId = Int32.Parse(reader["patient_id"].ToString());
                            dietaryManagementModel.IsActive = Convert.ToBoolean(reader["is_active"]);

                            dietaryManagementModels.Add(dietaryManagementModel);
                        }
                    }
                    catch (Exception e)
                    {
                        log.LogInformation(e.Message);
                        return new BadRequestObjectResult(e.Message);
                        throw;
                    }
                }
                conn.Close();
            }

            string json = JsonConvert.SerializeObject(dietaryManagementModels);

            return json != null
                ? (ActionResult)new OkObjectResult(json)
                : new BadRequestObjectResult(Messages.ErrorMissingValues);
        }
    }
    }

