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
using AppNiZiAPI.Models;

namespace AppNiZiAPI.Functions.FoodByName
{
    public static class FoodByName
    {
        [FunctionName("Food")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", (Routes.APIVersion + Routes.FoodByName))] HttpRequest req,
            ILogger log)
        {
            var connString = Environment.GetEnvironmentVariable("sqldb_connection");
            //get foodname om te vinden uit query
            string foodname;
            try
            {
                foodname = req.Query["foodname"];
            }
            catch (Exception)
            {
                return new BadRequestObjectResult(Messages.ErrorMissingValues);
            }

            Food food = new Food();

            using (SqlConnection conn = new SqlConnection(connString))
            {

                conn.Open();
                var text = $"SELECT * FROM Food WHERE name="+foodname;

                using (SqlCommand cmd = new SqlCommand(text, conn))
                {
                    try
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            // Uit lezen bijv
                            food.KCal = (float)reader["kcal"];
                            food.Protein = (float)reader["protein"];
                            food.Fiber = (float)reader["fiber"];
                            food.Calcium = (float)reader["calcium"];
                            food.Sodium = (float)reader["sodium"];
                            food.PortionSize = (float)reader["portion_size"];
                            //dit kan ik gebruiken om enum weightunit te pakken?
                            // Mitch - Zou het via een inner join doen al direct uit db
                            /*
                             * SELECT *.f, description.w
                             * FROM Food as f, WeightUnit? as w
                             * WHERE f.weight_unit_id = w.id
                             * 
                             * en dan food.WeightUnitDescription = reader[description];
                             */
                            food.WeightUnitId = (int)reader["weight_unit_id"];
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

            //zie dat jullie hier json teruggeven kan ik niet gewoon meteen food teruggeven?
            return food != null
                ? (ActionResult)new OkObjectResult(food)
                : new BadRequestObjectResult(Messages.ErrorMissingValues);
        }
    }
}
