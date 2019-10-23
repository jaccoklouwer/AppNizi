using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace AppNiZiAPI.Models.Repositories
{
    public class ConsumptionRespository : Repository , IConsumptionRepository
    {
        public void AddConsumption(Consumption consumption)
        {
            throw new NotImplementedException();
        }

        public void DeleteConsumption(int consumptionId)
        {
            throw new NotImplementedException();
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
                        // Maybe use SqlDataReaderMapper https://www.nuget.org/packages/SqlDataReaderMapper/
                        consumption.Id = (int)reader["id"];
                        consumption.FoodName = reader["food_name"].ToString();
                        /*consumption.KCal = (float)reader["kcal"];
                        consumption.Protein = (float)reader["protein"];
                        consumption.Fiber = (float)reader["fiber"];
                        consumption.Calcium = (float)reader["calcium"];
                        consumption.Sodium = (float)reader["sodium"];
                        consumption.Amount = (int)reader["amount"];*/
                        consumption.WeightUnitId = (int)reader["weight_unit_id"];
                        consumption.Date = Convert.ToDateTime(reader["date"]);
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

        public List<Consumption> GetConsumptionsForPatientBetweenDates(int patientId, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public void UpdateConsumption(Consumption consumption)
        {
            throw new NotImplementedException();
        }
    }
}
