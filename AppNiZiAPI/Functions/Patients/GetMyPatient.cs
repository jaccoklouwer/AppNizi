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
using System.Data.SqlClient;

namespace AppNiZiAPI
{
    public static class Function1
    {
        // Functie naam zelfde als Methode naam (Normaal staat er "Run", deze kan je gewoon veranderen naar iets anders)
        // Zet in de HttpTrigger de Route op de goede route van Routes.cs. Zet altijd eerst Routes.APIVersion ervoor. Zodat het dus "api/v1/patients/me" wordt
        [FunctionName("GetMyPatient")]
        public static async Task<IActionResult> GetMyPatient(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = (Routes.APIVersion + Routes.Patients + Routes.Me))] HttpRequest req,
            ILogger log)
        {
            var connString = Environment.GetEnvironmentVariable("sqldb_connection");
            var accountId = 0;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                // Haal info uit de Body, moet nog worden aangepast vanwege Oauth enzo.
                try
                {
                    var content = await new StreamReader(req.Body).ReadToEndAsync();
                    log.LogInformation(content); // Log het in console
                    accountId = JsonConvert.DeserializeObject<int>(content);
                }
                catch (Exception)
                {
                    return new BadRequestObjectResult(Messages.ErrorMissingValues);
                }

                conn.Open();
                var text = $"SELECT * FROM Patients WHERE id=";

                using (SqlCommand cmd = new SqlCommand(text, conn))
                {
                    try
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            // Uit lezen bijv
                            string naam = reader["name"].ToString();
                            int age = Int32.Parse(reader["age"].ToString());
                        }
                    }
                    catch (Exception)
                    {
                        return new BadRequestObjectResult(Messages.ErrorMissingValues);
                        throw;
                    }
                }
                conn.Close();
            }

            string json = "Uiteindelijk hier via JsonConverter weer omzetten";

            return json != null
                ? (ActionResult)new OkObjectResult(json)
                : new BadRequestObjectResult(Messages.ErrorMissingValues);
        }
    }
}
