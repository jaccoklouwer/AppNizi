using AppNiZiAPI.Models.Repositories;
using AppNiZiAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace AppNiZiAPI
{
    public class ConsumptionRespository : Repository, IConsumptionRepository
    {
        
        public bool AddConsumption(ConsumptionView consumption, int patientId)
        {
            bool added;
            var insert = $"INSERT INTO Consumption " +
                $"(food_name, kcal, protein, fiber, calium, sodium, amount, weight_unit_id, date, patient_id)";
            var values = $" VALUES (@food_name, @kcal, @protein, @fiber, @calium, @sodium, @amount, @weight_unit_id, @date, @patient_id)";
            var insertQuery = insert + values;
            conn.Open();
            try
            {
                added = ConsumptionCommand(insertQuery, consumption, patientId).ExecuteNonQuery() > 0;
            }
            catch (Exception)
            {
                added = false;
            }
            conn.Close();
            return added;
        }

        public int GetConsumptionPatientId(int consumptionId)
        {
            //TODO: Test method

            int patientId = 0;
            var query = $"SELECT Consumption.patient_id FROM Consumption WHERE id = '{consumptionId}'";
            using (SqlCommand sqlCommand = new SqlCommand(query, conn))
            {
                conn.Open();
                try
                {
                    SqlDataReader reader = sqlCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        patientId = (int)reader["patient_id"];
                    }
                }
                catch (Exception e)
                {
                    // logging?
                    throw e;
                }
                conn.Close();
            }
            return patientId;
        }

        private bool DeleteConsumption(int consumptionId)
        {
            bool affected;
            var query = $"DELETE FROM Consumption WHERE id = '{consumptionId}'";
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
            return affected;
        }

        public bool DeleteConsumption(int consumptionId, int patientId)
        {
            if (patientId == 0) return false;
            if (GetConsumptionPatientId(consumptionId) ==  patientId) return DeleteConsumption(consumptionId);
            return false;
        }

        public Consumption GetConsumptionByConsumptionId(int consumptionId)
        {
            var query = $"SELECT * FROM Consumption WHERE id = '{consumptionId}'";

            conn.Open();
            Consumption consumption = new Consumption();
            using (SqlCommand sqlCommand = new SqlCommand(query, conn))
            {
                try
                {
                    SqlDataReader reader = sqlCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        consumption.Id = (int)reader["id"];
                        consumption.FoodName = reader["food_name"].ToString();
                        consumption.KCal = (float)Convert.ToDouble(reader["kcal"]);
                        consumption.Protein = (float)Convert.ToDouble(reader["protein"]);
                        consumption.Fiber = (float)Convert.ToDouble(reader["fiber"]);
                        consumption.Calium = (float)Convert.ToDouble(reader["calium"]);
                        consumption.Sodium = (float)Convert.ToDouble(reader["sodium"]);
                        consumption.Amount = (int)reader["amount"];
                        consumption.WeightUnitId = (int)reader["weight_unit_id"];
                        consumption.Date = Convert.ToDateTime(reader["date"]).Date;
                        consumption.PatientId = (int)reader["patient_id"];
                    }
                }
                catch (Exception e)
                {
                    // logging?
                    throw e;
                }
            }
            conn.Close();
            return consumption;
        }

        public List<ConsumptionView> GetConsumptionsForPatientBetweenDates(int patientId, DateTime startDate, DateTime endDate)
        {
            String sqlStartDate = startDate.Date.ToString("yyyy-MM-dd").Replace("/", "-");
            String sqlEndDate = endDate.Date.ToString("yyyy-MM-dd").Replace("/", "-");
            var query = $"SELECT * FROM Consumption WHERE patient_id = '{patientId}' AND date BETWEEN '{sqlStartDate}' AND '{sqlEndDate}'";

            List<ConsumptionView> consumptions = new List<ConsumptionView>();
            conn.Open();

            using (SqlCommand sqlCommand = new SqlCommand(query, conn))
            {
                try
                {

                    SqlDataReader reader = sqlCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        Consumption consumption = new Consumption();
                        consumption.Id = (int)reader["id"];
                        consumption.FoodName = reader["food_name"].ToString();
                        consumption.KCal = (float)Convert.ToDouble(reader["kcal"]);
                        consumption.Protein = (float)Convert.ToDouble(reader["protein"]);
                        consumption.Fiber = (float)Convert.ToDouble(reader["fiber"]);
                        consumption.Calium = (float)Convert.ToDouble(reader["calium"]);
                        consumption.Sodium = (float)Convert.ToDouble(reader["sodium"]);
                        consumption.Amount = (int)reader["amount"];
                        consumption.WeightUnitId = (int)reader["weight_unit_id"];
                        consumption.Date = Convert.ToDateTime(reader["date"]).Date;
                        consumption.PatientId = (int)reader["patient_id"];
                        consumptions.Add(new ConsumptionView(consumption));
                    }
                }
                catch (Exception e)
                {
                    consumptions.Add(new ConsumptionView());
                    throw e;
                }
            }
            conn.Close();
            return consumptions;
        }

        public bool UpdateConsumption(int consumptionId, ConsumptionView consumption, int patientId)
        {
            bool updated;
            var updateQuery = $"UPDATE Consumption SET " +
                $"food_name = @food_name, kcal = @kcal, protein = @protein, fiber = @fiber, " +
                $"calium = @calium, sodium = @sodium, amount = @amount, weight_unit_id = @weight_unit_id, " +
                $"date = @date, patient_id = @patient_id " +
                $"Where Id = {consumptionId}";
            conn.Open();
            try
            {
                updated = ConsumptionCommand(updateQuery, consumption, patientId).ExecuteNonQuery() > 0;
            }
            catch (Exception)
            {
                updated = false;
            }
            conn.Close();
            return updated;
        }

        private SqlCommand ConsumptionCommand(String query, Consumption consumption)
        {
            return ConsumptionCommand(query, new ConsumptionView(consumption), consumption.PatientId);
        }

        private SqlCommand ConsumptionCommand(String query, ConsumptionView consumptionView, int PatientId)
        {
            SqlCommand command = new SqlCommand(query, conn);
            command.Parameters.AddWithValue("@food_name", consumptionView.FoodName);
            command.Parameters.AddWithValue("@kcal", consumptionView.KCal);
            command.Parameters.AddWithValue("@protein", consumptionView.Protein);
            command.Parameters.AddWithValue("@fiber", consumptionView.Fiber);
            command.Parameters.AddWithValue("@calium", consumptionView.Calium);
            command.Parameters.AddWithValue("@sodium", consumptionView.Sodium);
            command.Parameters.AddWithValue("@amount", consumptionView.Amount);
            command.Parameters.AddWithValue("@weight_unit_id", consumptionView.WeightUnitId);
            command.Parameters.AddWithValue("@date", consumptionView.Date);
            command.Parameters.AddWithValue("@patient_id", PatientId);
            return command;
        }

    }
}
