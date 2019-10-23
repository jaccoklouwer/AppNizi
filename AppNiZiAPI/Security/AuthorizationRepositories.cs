using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace AppNiZiAPI.Security
{
    class AuthorizationRepositories
    {
        public bool ChecKUser(int patientId, string guid )
        {
            int outputPatientId = 0;
            using (SqlConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                conn.Open();
                var text =
                    $"SELECT id FROM Patient WHERE guid = '{guid}'";

                using (SqlCommand cmd = new SqlCommand(text, conn))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        outputPatientId = (int)reader["id"];
                    }
                }
                conn.Close();
            }

            if(patientId != outputPatientId)
            {
                return false;
            }
            return true;
        }
    }
}
