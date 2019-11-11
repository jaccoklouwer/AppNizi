using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using AppNiZiAPI.Models.Dietarymanagement;

namespace AppNiZiAPI.Models.Repositories
{
    public interface IDietaryManagementRepository
    {
        Task<bool> AddDietaryManagement(DietaryManagementModel dietary);
        Task<bool> DeleteDietaryManagement(int id);

        Task<bool> DeleteByPatientId(int patientId);

        Task<List<DietaryManagementModel>> GetDietaryManagementByPatientAsync(int patientId);
        Task<List<DietaryRestriction>> GetDietaryRestrictions();
        Task<bool> UpdateDietaryManagement(int id, DietaryManagementModel dietaryManagement);
    }

    public class DietaryManagementRepository : Repository, IDietaryManagementRepository
    {

        public async Task<List<DietaryManagementModel>> GetDietaryManagementByPatientAsync(int patientId)
        {

            List<DietaryManagementModel> dietaryManagementModels = new List<DietaryManagementModel>();
            using (SqlConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                conn.Open();
                var text = $"SELECT b.description, a.amount, a.is_active, a.patient_id, a.id " +
                    $"FROM DietaryManagement AS a, DietaryRestriction AS b " +
                    $"WHERE a.dietary_restriction_id = b.id AND a.patient_id={patientId}";

                using (SqlCommand cmd = new SqlCommand(text, conn))
                {
                    try
                    {
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

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
                        throw e;
                    }
                }
            }
            return dietaryManagementModels;
        }

        public async Task<List<DietaryRestriction>> GetDietaryRestrictions()
        {
            List<DietaryRestriction> dietaryRestrictions = new List<DietaryRestriction>();
            using (SqlConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                conn.Open();
                var query = "SELECT * FROM DietaryRestriction";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    try
                    {
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();
                        while (reader.Read())
                        {
                            DietaryRestriction restriction = new DietaryRestriction();
                            restriction.Id = Int32.Parse(reader["id"].ToString());
                            restriction.Description = reader["description"].ToString();
                            dietaryRestrictions.Add(restriction);
                        }
                    }
                    catch (Exception e)
                    {

                        throw e;
                    }
                }

            }
            return dietaryRestrictions;
        }


        public async Task<bool> UpdateDietaryManagement(int id, DietaryManagementModel dietaryManagement)
        {
            bool succes;
            using (SqlConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                conn.Open();
                var query = @"UPDATE DietaryManagement
                                SET dietary_restriction_id = (SELECT r.id
                                                                FROM DietaryRestriction as r
                                                                WHERE r.description = @DESCRIPTION)
                                  ,amount = @AMOUNT
                                  ,patient_id = @PATIENT
                                  ,is_active = @ISACTIVE
                             WHERE id = @ID";

                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.Add("@DESCRIPTION", SqlDbType.VarChar).Value = dietaryManagement.Description;
                command.Parameters.Add("@AMOUNT", SqlDbType.Int).Value = dietaryManagement.Amount;
                command.Parameters.Add("@PATIENT", SqlDbType.Int).Value = dietaryManagement.PatientId;
                command.Parameters.Add("@ISACTIVE", SqlDbType.Bit).Value = dietaryManagement.IsActive;
                command.Parameters.Add("@ID", SqlDbType.Int).Value = id;
                succes = await command.ExecuteNonQueryAsync() > 0;
            }
            return succes;
        }

        public async Task<bool> DeleteDietaryManagement(int id)
        {

            using (SqlConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                conn.Open();
                var query = @"DELETE FROM DietaryManagement
                                WHERE id = @ID";
                bool succes;
                try
                {
                    SqlCommand command = new SqlCommand(query, conn);
                    command.Parameters.Add("@ID", SqlDbType.Int).Value = id;
                    succes = await command.ExecuteNonQueryAsync() > 0;
                }
                catch (Exception)
                {
                    succes = false;
                }
                return succes;
            }
        }

        public async Task<bool> AddDietaryManagement(DietaryManagementModel dietary)
        {
            using (SqlConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                conn.Open();
                var query = @"INSERT INTO DietaryManagement
                                   (dietary_restriction_id,
                                    amount,
                                    patient_id,
                                    is_active)
                             VALUES
                                   (
                                    (
                                    SELECT r.id
                                        FROM DietaryRestriction as r
                                        WHERE r.description = @DESCRIPTION
                                    ),
                                    @AMOUNT,
                                    @PATIENT,
                                    @ISACTIVE
                                )";

                bool succes;
                try
                {
                    SqlCommand command = new SqlCommand(query, conn);
                    command.Parameters.Add("@DESCRIPTION", SqlDbType.VarChar).Value = dietary.Description;
                    command.Parameters.Add("@AMOUNT", SqlDbType.Int).Value = dietary.Amount;
                    command.Parameters.Add("@PATIENT", SqlDbType.Int).Value = dietary.PatientId;
                    command.Parameters.Add("@ISACTIVE", SqlDbType.Bit).Value = dietary.IsActive;
                    succes = await command.ExecuteNonQueryAsync() > 0;
                }
                catch (Exception e)
                {
                    throw e;
                }
                return succes;
            }
        }

        public async Task<bool> DeleteByPatientId(int patientId)
        {
            bool success = false;

            string sqlQuery = "DELETE FROM DietaryManagement WHERE patient_id = @ID";

            using (SqlConnection sqlConn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                sqlConn.Open();

                SqlCommand sqlCmd = new SqlCommand(sqlQuery, sqlConn);
                sqlCmd.Parameters.Add("@ID", SqlDbType.Int).Value = patientId;

                int rows = await sqlCmd.ExecuteNonQueryAsync();
                if (rows > 0)
                    success = true;
                sqlConn.Close();
            }

            return success;
        }
    }
}
