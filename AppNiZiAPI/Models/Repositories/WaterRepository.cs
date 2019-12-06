using AppNiZiAPI.Models.Handlers;
using AppNiZiAPI.Models.Water;
using AppNiZiAPI.Variables;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace AppNiZiAPI.Models.Repositories
{
    class WaterRepository : IWaterRepository
    {
        private List<WaterConsumptionViewModel> waterConsumptions = new List<WaterConsumptionViewModel>();
        private int MinumumRestriction = 0;

        public async Task<WaterConsumptionDaily> GetDailyWaterConsumption(int patientId, DateTime date)
        {
            // Make empty list
            List<WaterConsumptionViewModel> waterConsumptionViews;

            // Using Async methodes and functions
            using (SqlConnection sqlCOnnection = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                await sqlCOnnection.OpenAsync();

                string sqlStringCommand =
                    "SELECT w.*, u.short, u.unit, dm.amount AS maxAmount " +
                    "FROM WaterConsumption AS w " +
                    "INNER JOIN WeightUnit AS u ON w.weight_unit_id = u.id " +
                    "INNER JOIN DietaryManagement AS dm ON dm.patient_id = w.patient_id " +
                    "WHERE w.patient_id = @PATIENTID AND w.date = @DATE";

                using(SqlCommand sqlCommand = new SqlCommand(sqlStringCommand, sqlCOnnection))
                {
                    sqlCommand.Parameters.Add("@PATIENTID", SqlDbType.Int).Value = patientId;
                    sqlCommand.Parameters.Add("@DATE", SqlDbType.Date).Value = date;

                    using(SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync())
                    {
                        waterConsumptionViews = await ReadFromDataReaderAsync(sqlDataReader);
                    }
                }
                sqlCOnnection.Close();
            }

            if (waterConsumptionViews.Count == 0)
                return null;

            double totalAmount = 0;

            foreach (var waterConsumption in waterConsumptionViews)
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

        public async Task<WaterConsumptionModel> GetSingleWaterConsumption(int patientId, int waterId)
        {
            WaterConsumptionViewModel waterConsumption = null;

            using (SqlConnection sqlCOnnection = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                await sqlCOnnection.OpenAsync();
                var sqlStringCommand = $"SELECT w.*, u.short, u.unit " +
                    $"FROM WaterConsumption AS w " +
                    $"INNER JOIN WeightUnit AS u ON u.id = w.weight_unit_id " +
                    $"WHERE w.id = @ID AND w.patient_id = @PATIENTID";

                using (SqlCommand sqlCommand = new SqlCommand(sqlStringCommand, sqlCOnnection))
                {
                    sqlCommand.Parameters.Add("@PATIENTID", SqlDbType.Int).Value = patientId;
                    sqlCommand.Parameters.Add("@ID", SqlDbType.Int).Value = waterId;

                    using (SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync())
                    {
                        try
                        {
                            while (await sqlDataReader.ReadAsync())
                            {
                                waterConsumption = new WaterConsumptionViewModel
                                {
                                    Id = (int)sqlDataReader["id"],
                                    Amount = (double)sqlDataReader["amount"],
                                    Date = (DateTime)sqlDataReader["date"],
                                    PatientId = (int)sqlDataReader["patient_id"],
                                    WeightUnit = new WeightUnitModel
                                    {
                                        Id = (int)sqlDataReader["weight_unit_id"],
                                        Short = (string)sqlDataReader["short"],
                                        Unit = (string)sqlDataReader["unit"]
                                    }
                                };
                            }
                        }
                        catch
                        {
                            waterConsumption = new WaterConsumptionViewModel
                            {
                                Error = true
                            };
                        }
                    }
                }
                sqlCOnnection.Close();
            }
            return waterConsumption;
        }

        public List<WaterConsumptionViewModel> GetWaterConsumptionPeriod(int patientId, DateTime beginDate, DateTime endDate)
        {
            using (SqlConnection sqlCOnnection = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                sqlCOnnection.Open();
                var sqlStringCommand = $"SELECT w.*, u.short, u.unit " +
                    "FROM WaterConsumption AS w " +
                    "INNER JOIN WeightUnit AS u ON u.id = w.weight_unit_id " +
                    "WHERE w.patient_id = @PATIENTID AND w.date BETWEEN @BEGINDATE AND @ENDDATE " +
                    "ORDER BY w.date ASC";

                SqlCommand cmd = new SqlCommand(sqlStringCommand, sqlCOnnection);
                cmd.Parameters.Add("@PATIENTID", SqlDbType.Int).Value = patientId;
                cmd.Parameters.Add("@BEGINDATE", SqlDbType.Date).Value = beginDate;
                cmd.Parameters.Add("@ENDDATE", SqlDbType.Date).Value = endDate;

                SqlDataReader reader = cmd.ExecuteReader();
                ReadFromDataReader(reader);

                sqlCOnnection.Close();
            }
            return waterConsumptions;
        }

        public async Task<WaterConsumptionModel> InsertWaterConsumption(WaterConsumptionModel model, bool update)
        {
            using (SqlConnection sqlConnection = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                string sqlStringCommand = "";
                if (update)
                {
                    sqlStringCommand =
                    $"UPDATE WaterConsumption " +
                    $"SET date = @DATE, amount = @AMOUNT, patient_id = @PATIENTID " +
                    $"OUTPUT Inserted.id " +
                    $"WHERE id = @ID";
                }
                else
                {
                    sqlStringCommand =
                    $"INSERT INTO WaterConsumption(date, amount, patient_id, weight_unit_id) " +
                    $"OUTPUT Inserted.id " +
                    $"VALUES(@DATE,@AMOUNT ,@PATIENTID, 7)";
                }

                await sqlConnection.OpenAsync();
                using (SqlCommand sqlCommand = new SqlCommand(sqlStringCommand, sqlConnection))
                {
                    sqlCommand.Parameters.Add("@PATIENTID", SqlDbType.Int).Value = model.PatientId;
                    sqlCommand.Parameters.Add("@DATE", SqlDbType.NVarChar).Value = model.Date.ToString("yyy-MM-dd");
                    sqlCommand.Parameters.Add("@AMOUNT", SqlDbType.Int).Value = model.Amount;
                    if (update)
                        sqlCommand.Parameters.Add("@ID", SqlDbType.Int).Value = model.Id;

                    try
                    {
                        model.Id = (int)await sqlCommand.ExecuteScalarAsync();
                        sqlConnection.Close();
                    }
                    catch { return null; }
                }
                return model;
            }
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

        private async Task<List<WaterConsumptionViewModel>> ReadFromDataReaderAsync(SqlDataReader reader)
        {
            List<WaterConsumptionViewModel> waterConsumptionViews = new List<WaterConsumptionViewModel>();
            try
            {
                while (await reader.ReadAsync())
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
                    waterConsumptionViews.Add(waterConsumption);

                    try
                    {
                        MinumumRestriction = (int)reader["maxAmount"];
                    }
                    catch { }
                }
            }
            catch 
            {
                WaterConsumptionViewModel waterConsumption = new WaterConsumptionViewModel
                {
                    Error = true
                };
                waterConsumptionViews.Add(waterConsumption);
            }
            return waterConsumptionViews;
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
        Task<WaterConsumptionDaily> GetDailyWaterConsumption(int patientId, DateTime date);
        List<WaterConsumptionViewModel> GetWaterConsumptionPeriod(int patientId, DateTime beginDate, DateTime endDate);
        Task<WaterConsumptionModel> InsertWaterConsumption(WaterConsumptionModel model, bool update);
        Task<WaterConsumptionModel> GetSingleWaterConsumption(int patientId, int waterId);
        bool RemoveWaterConsumptions(int patientId, int waterId);
    }
}
