using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace AppNiZiAPI.Security
{
    class AuthorizationRepositories
    {
        public bool PatientAuth(int userId, string guid, bool isDoctor)
        {
            int outputUserId = 0;
            var text = "";
            using (SqlConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                conn.Open();

                if (isDoctor)
                {
                    text = $"SELECT id FROM Doctor WHERE guid = '{guid}'";
                }
                else
                {
                    text =
                    $"SELECT id FROM Patient WHERE guid = '{guid}'";
                }

                using (SqlCommand cmd = new SqlCommand(text, conn))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        outputUserId = (int)reader["id"];
                    }
                }
                conn.Close();
            }

            if (userId != outputUserId)
            {
                return false;
            }
            return true;
        }

        public bool CheckDoctorAcces(int patientId, string guid)
        {
            bool result = false;
            using (SqlConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                conn.Open();
                var text =
                    $"SELECT CASE WHEN EXISTS ( " +
                    $"SELECT * FROM Patient " +
                    $"LEFT OUTER JOIN Doctor ON Patient.doctor_id = Doctor.id " +
                    $"WHERE Doctor.guid = '{guid}' AND Patient.id = {patientId} )" +
                    $"THEN CAST (1 AS BIT) " +
                    $"ELSE CAST (0 AS BIT) END";

                using (SqlCommand cmd = new SqlCommand(text, conn))
                {
                    //SqlDataReader reader = cmd.ExecuteReader();
                    result = (bool)cmd.ExecuteScalar();
                }
                conn.Close();
            }
            return result;
        }
    }
}
