﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace AppNiZiAPI.Security
{
    class AuthorizationRepository : IAuthorizationRepository
    {
        public bool PatientAuth(int userId, string guid, bool isDoctor)
        {
            int outputUserId = 0;
            var text = "";
            using (SqlConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                conn.Open();

                if (isDoctor)
                    text = 
                        $"SELECT id FROM Doctor WHERE guid = '{guid}'";
                else
                    text =
                        $"SELECT id FROM Patient WHERE guid = '{guid}'";

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
                return false;
            return true;
        }

        public bool CheckDoctorAcces(int patientId, string guid)
        {
            // VOOR TESTEN GEBRUIK DEZE GUID, doctor id is 1
#if DEBUG

            guid = "jfjfjfj";
#endif

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
        public int GetAccountId(string guid, bool isDoctor)
        {
            int userId = 0;
            var query = "";
            using (SqlConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                conn.Open();

                if (isDoctor)
                    query = 
                        $"SELECT id FROM Doctor WHERE guid = '{guid}'";
                else
                    query =
                    $"SELECT id FROM Patient WHERE guid = '{guid}'";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        userId = (int)reader["id"];
                    }
                }
                conn.Close();
            }
            return userId;
        }
    }

    public interface IAuthorizationRepository
    {
        bool PatientAuth(int userId, string guid, bool isDoctor);
        bool CheckDoctorAcces(int patientId, string guid);
        int GetAccountId(string guid, bool isDoctor);
    }
}
