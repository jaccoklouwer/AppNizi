using AppNiZiAPI.Models.Handlers;
using AppNiZiAPI.Models.Water;
using AppNiZiAPI.Variables;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace AppNiZiAPI.Models.Repositories
{
    class WaterRepository : IWaterRepository
    {
        private List<WaterConsumptionViewModel> waterConsumptions = new List<WaterConsumptionViewModel>();
        private int MinumumRestriction = 0;

        public WaterConsumptionDaily GetDailyWaterConsumption(int patientId, DateTime date)
        {
            using (SqlConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                conn.Open();
                string sqlQuery =
                    "SELECT w.*, u.short, u.unit, dm.amount AS maxAmount " +
                    "FROM WaterConsumption AS w " +
                    "INNER JOIN WeightUnit AS u ON w.weight_unit_id = u.id " +
                    "INNER JOIN DietaryManagement AS dm ON dm.patient_id = w.patient_id " +
                    "WHERE w.patient_id = @PATIENTID AND w.date = @DATE";

                SqlCommand cmd = new SqlCommand(sqlQuery, conn);
                cmd.Parameters.Add("@PATIENTID", SqlDbType.Int).Value = patientId;
                cmd.Parameters.Add("@DATE", SqlDbType.Date).Value = date;

                SqlDataReader reader = cmd.ExecuteReader();
                ReadFromDataReader(reader);

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

        public WaterConsumptionModel GetSingleWaterConsumption(int patientId, int waterId)
        {
            WaterConsumptionViewModel waterConsumption = null;
            using (SqlConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                conn.Open();
                var sqlQuery = $"SELECT w.*, u.short, u.unit " +
                    $"FROM WaterConsumption AS w " +
                    $"INNER JOIN WeightUnit AS u ON u.id = w.weight_unit_id " +
                    $"WHERE w.id = @ID AND w.patient_id = @PATIENTID";

                SqlCommand cmd = new SqlCommand(sqlQuery, conn);
                cmd.Parameters.Add("@PATIENTID", SqlDbType.Int).Value = patientId;
                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = waterId;

                SqlDataReader reader = cmd.ExecuteReader();

                try
                {
                    while (reader.Read())
                    {
                        waterConsumption = new WaterConsumptionViewModel
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
                    }
                }
                catch (Exception e)
                {
                    waterConsumption = new WaterConsumptionViewModel
                    {
                        Error = true
                    };
                }

                conn.Close();
            }
            return waterConsumption;
        }

        public List<WaterConsumptionViewModel> GetWaterConsumptionPeriod(int patientId, DateTime beginDate, DateTime endDate)
        {
            using (SqlConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                conn.Open();
                var sqlQuery = $"SELECT w.*, u.short, u.unit " +
                    "FROM WaterConsumption AS w " +
                    "INNER JOIN WeightUnit AS u ON u.id = w.weight_unit_id " +
                    "WHERE w.patient_id = @PATIENTID AND w.date BETWEEN @BEGINDATE AND @ENDDATE " +
                    "ORDER BY w.date ASC";

                SqlCommand cmd = new SqlCommand(sqlQuery, conn);
                cmd.Parameters.Add("@PATIENTID", SqlDbType.Int).Value = patientId;
                cmd.Parameters.Add("@BEGINDATE", SqlDbType.Date).Value = beginDate;
                cmd.Parameters.Add("@ENDDATE", SqlDbType.Date).Value = endDate;

                SqlDataReader reader = cmd.ExecuteReader();
                ReadFromDataReader(reader);

                conn.Close();
            }
            return waterConsumptions;
        }

        public WaterConsumptionModel InsertWaterConsumption(WaterConsumptionModel model, bool update)
        {
            using (SqlConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                var sqlQuery = "";
                if (update)
                {
                    sqlQuery =
                    $"UPDATE WaterConsumption " +
                    $"SET date = @DATE, amount = @AMOUNT, patient_id = @PATIENTID " +
                    $"WHERE id = @ID";
                }
                else
                {
                    sqlQuery =
                    $"INSERT INTO WaterConsumption(date, amount, patient_id, weight_unit_id) " +
                    $"OUTPUT Inserted.id " +
                    $"VALUES(@DATE,@AMOUNT ,@PATIENTID, 7)";
                }


                SqlCommand cmd = new SqlCommand(sqlQuery, conn);
                cmd.Parameters.Add("@PATIENTID", SqlDbType.Int).Value = model.PatientId;
                cmd.Parameters.Add("@DATE", SqlDbType.NVarChar).Value = model.Date.ToString("yyy-MM-dd");
                cmd.Parameters.Add("@AMOUNT", SqlDbType.Int).Value = model.Amount;
                if (update)
                    cmd.Parameters.Add("@ID", SqlDbType.Int).Value = model.Id;

                conn.Open();

                int result = (int)cmd.ExecuteScalar();
                model.Id = result;
                conn.Close();

                if (result != 0)
                    return model;
            }
            return null;
        }

        public bool RemoveWaterConsumptions(int patientId, int waterId)
        {
            using (SqlConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                var sqlQuery =
                    $"DELETE FROM WaterConsumption " +
                    $"WHERE id = @ID AND patient_id = @PATIENTID";
     
                SqlCommand cmd = new SqlCommand(sqlQuery, conn);
                cmd.Parameters.Add("@PATIENTID", SqlDbType.Int).Value = patientId;
                cmd.Parameters.Add("@ID", SqlDbType.NVarChar).Value = waterId;

                conn.Open();

                int result = cmd.ExecuteNonQuery();
                conn.Close();

                return result > 0
                    ? true
                    : false;
            }
        }

        private void ReadFromDataReader(SqlDataReader reader)
        {
            waterConsumptions = new List<WaterConsumptionViewModel>();
            try
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
            catch (Exception e)
            {
                WaterConsumptionViewModel waterConsumption = new WaterConsumptionViewModel
                {
                    Error = true
                };
                waterConsumptions.Add(waterConsumption);
            }
        }
    }

    interface IWaterRepository
    {
        WaterConsumptionDaily GetDailyWaterConsumption(int patientId, DateTime date);
        List<WaterConsumptionViewModel> GetWaterConsumptionPeriod(int patientId, DateTime beginDate, DateTime endDate);
        WaterConsumptionModel InsertWaterConsumption(WaterConsumptionModel model, bool update);
        WaterConsumptionModel GetSingleWaterConsumption(int patientId, int waterId);
        bool RemoveWaterConsumptions(int patientId, int waterId);
    }
}
