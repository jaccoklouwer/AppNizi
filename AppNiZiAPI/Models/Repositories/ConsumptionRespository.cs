using AppNiZiAPI.Models.Repositories;
using AppNiZiAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace AppNiZiAPI
{
    public class ConsumptionRespository : IConsumptionRepository
    {
        public bool AddConsumption(Consumption consumption)
        {
            bool added;
            var insert = $"INSERT INTO Consumption " +
                $"(food_name, kcal, protein, fiber, calium, sodium, amount, weight_unit_id, date, patient_id)";
            var values = $" VALUES (@food_name, @kcal, @protein, @fiber, @calium, @sodium, @amount, @weight_unit_id, @date, @patient_id)";
            var insertQuery = insert + values;

            using (SqlConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                conn.Open();
                try
                {
                    added = ConsumptionCommand(insertQuery, consumption,conn).ExecuteNonQuery() > 0;
                }
                catch (Exception)
                {
                    added = false;
                }
                conn.Close();
            }
            return added;
        }

        public bool DeleteConsumption(int consumptionId, int patientId)
        {
            if (patientId == 0) return false;

            bool affected;
            var query = $"DELETE FROM Consumption WHERE id = '{consumptionId}'";
            using (SqlConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                conn.Open();
                try
                {
                    SqlCommand command = new SqlCommand(query, conn);
                    affected = command.ExecuteNonQuery() > 0;
                }
                catch (Exception)
                {
                    affected = false;
                }
                conn.Close();
            }
            return affected;
        }

        public ConsumptionView GetConsumptionByConsumptionId(int consumptionId)
        {
            var query = $"SELECT Consumption.*, WeightUnit.short, WeightUnit.unit " +
                $"FROM Consumption " +
                $"INNER JOIN WeightUnit ON Consumption.weight_unit_id = WeightUnit.id " +
                $"WHERE Consumption.id = '{consumptionId}'";

            ConsumptionView consumption = new ConsumptionView();
            using (SqlConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                conn.Open();
                using (SqlCommand sqlCommand = new SqlCommand(query, conn))
                {
                    try
                    {
                        SqlDataReader reader = sqlCommand.ExecuteReader();
                        while (reader.Read())
                        {
                            consumption.ConsumptionId = (int)reader["id"];
                            consumption.FoodName = reader["food_name"].ToString();
                            consumption.KCal = (float)Convert.ToDouble(reader["kcal"]);
                            consumption.Protein = (float)Convert.ToDouble(reader["protein"]);
                            consumption.Fiber = (float)Convert.ToDouble(reader["fiber"]);
                            consumption.Calium = (float)Convert.ToDouble(reader["calium"]);
                            consumption.Sodium = (float)Convert.ToDouble(reader["sodium"]);
                            consumption.Amount = (int)reader["amount"];
                            consumption.Weight = new WeightUnitModel
                            {
                                Id = (int)reader["weight_unit_id"],
                                Short = (string)reader["short"],
                                Unit = (string)reader["unit"]
                            };
                            consumption.Date = Convert.ToDateTime(reader["date"]).Date;
                            consumption.PatientId = (int)reader["patient_id"];
                            consumption.Valid = true;
                        }
                    }
                    catch (Exception)
                    {
                        consumption.Valid = false;
                        return consumption;
                    }
                }
                conn.Close();
            }
            return consumption;
        }

        public List<PatientConsumptionView> GetConsumptionsForPatientBetweenDates(int patientId, DateTime startDate, DateTime endDate)
        {
            string sqlStartDate = startDate.Date.ToString("yyyy-MM-dd").Replace("/", "-");
            string sqlEndDate = endDate.Date.ToString("yyyy-MM-dd").Replace("/", "-");

            var query = $"SELECT Consumption.*, WeightUnit.short, WeightUnit.unit " +
                $"FROM Consumption " +
                $"INNER JOIN WeightUnit ON Consumption.weight_unit_id = WeightUnit.id " +
                $"WHERE Consumption.patient_id = '{patientId}' AND Consumption.date BETWEEN '{sqlStartDate}' AND '{sqlEndDate}'";

            List<PatientConsumptionView> consumptions = new List<PatientConsumptionView>();
            PatientConsumptionView consumption;

            using (SqlConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                conn.Open();
                using (SqlCommand sqlCommand = new SqlCommand(query, conn))
                {
                    try
                    {

                        SqlDataReader reader = sqlCommand.ExecuteReader();
                        while (reader.Read())
                        {
                            consumption = new PatientConsumptionView();

                            try
                            {
                                consumption.ConsumptionId = (int)reader["id"];
                                consumption.FoodName = reader["food_name"].ToString();
                                consumption.KCal = (float)Convert.ToDouble(reader["kcal"]);
                                consumption.Protein = (float)Convert.ToDouble(reader["protein"]);
                                consumption.Fiber = (float)Convert.ToDouble(reader["fiber"]);
                                consumption.Calium = (float)Convert.ToDouble(reader["calium"]);
                                consumption.Sodium = (float)Convert.ToDouble(reader["sodium"]);
                                consumption.Amount = (int)reader["amount"];
                                consumption.Weight = new WeightUnitModel
                                {
                                    Id = (int)reader["weight_unit_id"],
                                    Short = (string)reader["short"],
                                    Unit = (string)reader["unit"]
                                };
                                consumption.Date = Convert.ToDateTime(reader["date"]).Date;
                                consumption.Valid = true;
                            }
                            catch (Exception ex)
                            {
                                if (consumption.ConsumptionId == 0)
                                {
                                    throw ex;
                                }
                            }

                            // Adds incomplete consumption to consumptions. Consumption is Invalid by default
                            consumptions.Add(consumption);
                        }
                    }
                    catch (Exception e)
                    {
                        conn.Close();
                        throw e;
                    }
                }
                conn.Close();
            }
            return consumptions;
        }

        public bool UpdateConsumption(int consumptionId, Consumption consumption)
        {
            bool updated;
            var updateQuery = $"UPDATE Consumption SET " +
                $"food_name = @food_name, kcal = @kcal, protein = @protein, fiber = @fiber, " +
                $"calium = @calium, sodium = @sodium, amount = @amount, weight_unit_id = @weight_unit_id, " +
                $"date = @date, patient_id = @patient_id " +
                $"Where Id = {consumptionId}";
            using (SqlConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                conn.Open();
                try
                {
                    updated = ConsumptionCommand(updateQuery, consumption,conn).ExecuteNonQuery() > 0;
                }
                catch (Exception)
                {
                    updated = false;
                }
                conn.Close();
            }
            return updated;
        }

        private SqlCommand ConsumptionCommand(string query, Consumption consumption,SqlConnection conn)
        {
            SqlCommand command = new SqlCommand(query, conn);
            command.Parameters.AddWithValue("@food_name", consumption.FoodName);
            command.Parameters.AddWithValue("@kcal", consumption.KCal);
            command.Parameters.AddWithValue("@protein", consumption.Protein);
            command.Parameters.AddWithValue("@fiber", consumption.Fiber);
            command.Parameters.AddWithValue("@calium", consumption.Calium);
            command.Parameters.AddWithValue("@sodium", consumption.Sodium);
            command.Parameters.AddWithValue("@amount", consumption.Amount);
            command.Parameters.AddWithValue("@weight_unit_id", consumption.WeightUnitId);
            command.Parameters.AddWithValue("@date", consumption.Date);
            command.Parameters.AddWithValue("@patient_id", consumption.PatientId);
            return command;
        }

    }
}
