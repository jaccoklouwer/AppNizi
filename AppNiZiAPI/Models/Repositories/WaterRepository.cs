using AppNiZiAPI.Models.Water;
using AppNiZiAPI.Variables;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace AppNiZiAPI.Models.Repositories
{
    class WaterRepository
    {
        private List<WaterConsumptionViewModel> waterConsumptions = new List<WaterConsumptionViewModel>();
        private int MinumumRestriction = 0;

        public WaterConsumptionDaily GetWaterConsumption(int patientId, string date)
        {
            using (SqlConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                conn.Open();
                var text = 
                    $"SELECT w.*, u.short, u.unit, dm.amount AS maxAmount " +
                    $"FROM WaterConsumption AS w " +
                    $"INNER JOIN WeightUnit AS u ON w.weight_unit_id = u.id " +
                    $"INNER JOIN DietaryManagement AS dm ON dm.patient_id = w.patient_id " +
                    $"WHERE w.patient_id = {patientId} AND w.date = '{date}'";

                using (SqlCommand cmd = new SqlCommand(text, conn))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    ReadFromDataReader(reader);
                }
                conn.Close();
            }

            double totalAmount = 0;

            foreach (var waterConsumption in waterConsumptions)
            {
                totalAmount += waterConsumption.Amount;
            }

            return new WaterConsumptionDaily
            {
                Total = totalAmount,
                WaterConsumptions = waterConsumptions,
                MinimumRestriction = MinumumRestriction
            };
        }

        public List<WaterConsumptionViewModel> GetWaterConsumptionPeriod(int patientId, string beginDate, string endDate)
        {
            using (SqlConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                conn.Open();
                var text = $"SELECT w.*, u.short, u.unit " +
                    $"FROM WaterConsumption AS w " +
                    $"INNER JOIN WeightUnit AS u ON u.id = w.weight_unit_id "+
                    $"WHERE w.patient_id = {patientId} AND w.date BETWEEN '{beginDate}' AND '{endDate}'";

                using (SqlCommand cmd = new SqlCommand(text, conn))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    ReadFromDataReader(reader);
                }
                conn.Close();
            }
            return waterConsumptions;
        }


        private void ReadFromDataReader(SqlDataReader reader)
        {
            while (reader.Read())
            {
                WaterConsumptionViewModel waterConsumption = new WaterConsumptionViewModel
                {
                    Id = (int)reader["id"],
                    Amount = (double)reader["amount"],
                    Date = (DateTime)reader["date"],
                    PatientId = (int)reader["patient_id"],
                    WeightUnit = new WeightUnitModel
                    {
                        Id = (int)reader["weight_unit_id"],
                        Short = (string)reader["short"],
                        Unit = (string)reader["unit"]
                    }
                };
                waterConsumptions.Add(waterConsumption);

                try
                {
                    MinumumRestriction = (int)reader["maxAmount"];
                }
                catch { }
            }
        }
    }
}
