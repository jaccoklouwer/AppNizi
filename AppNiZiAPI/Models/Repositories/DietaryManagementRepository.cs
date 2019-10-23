using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace AppNiZiAPI.Models.Repositories
{
    public class DietaryManagementRepository
    {
        private string connString { get; set; }
        public DietaryManagementRepository()
        {
            connString = Environment.GetEnvironmentVariable("sqldb_connection");
        }

        public List<DietaryManagementModel> GetDietaryManagementByPatient(int patientId)
        {

            List<DietaryManagementModel> dietaryManagementModels = new List<DietaryManagementModel>();
            using (SqlConnection conn = new SqlConnection(connString))
            {

                conn.Open();
                var text = $"SELECT b.description, a.amount, a.is_active, a.patient_id, a.id " +
                    $"FROM DietaryManagement AS a, DietaryRestriction AS b " +
                    $"WHERE a.dietary_restriction_id = b.id AND a.patient_id={patientId}";

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
                        throw e;
                    }
                }
                conn.Close();
                return dietaryManagementModels;
            }
        }

        public bool UpdateDietaryManagement(int id, DietaryManagementModel dietaryManagement) {

            using (SqlConnection conn = new SqlConnection(connString)) {
                conn.Open();
                var query = $@"UPDATE DietaryManagement
                                SETdietary_restriction_id = (SELECT r.id
                                                                FROM DietaryRestriction as r
                                                                WHERE r.description = {dietaryManagement.Description} )
                                  ,amount = {dietaryManagement.Amount}
                                  ,patient_id = {dietaryManagement.PatientId}
                                  ,is_active = {dietaryManagement.IsActive}
                             WHERE id = {id}";

                SqlCommand command = new SqlCommand(query, conn);
                return command.ExecuteNonQuery() > 0;
            }
        }

        public bool DeleteDietaryManagement(int id) {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                var query = $@"DELETE FROM DietaryManagement
                                WHERE id = {id}";

                try
                {
                    SqlCommand command = new SqlCommand(query, conn);
                    return command.ExecuteNonQuery() > 0;
                }
                catch (Exception)
                {

                    return false;
                }
            }
        }
    }
}
