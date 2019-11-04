using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace AppNiZiAPI.Security
{
    class AuthorizationRepository : IAuthorizationRepository
    {
        // Checked wanneer patient info over hemzelf opvraagt. Wanneer een doctor het vraagt checked is eerst of ie wel toegang heeft bij de patient
        public bool HasAcces(int userId, string guid)
        {
            string sqlQuery =
                "SELECT CASE WHEN EXISTS " +
                "( SELECT * FROM Patient WHERE guid = @GUID AND id = @USERID) OR EXISTS " +
                "( SELECT d.* FROM Doctor AS d INNER JOIN Patient AS p ON d.id = p.doctor_id " +
                "WHERE d.guid = @GUID AND p.id = @USERID ) " +
                "THEN CAST (1 AS BIT) ELSE CAST (0 AS BIT) END";
            using (SqlConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand(sqlQuery, conn);
                    cmd.Parameters.Add("@USERID", SqlDbType.Int).Value = userId;
                    cmd.Parameters.Add("@GUID", SqlDbType.NVarChar).Value = guid;
                    conn.Open();
                    return (bool)cmd.ExecuteScalar();
                }
                catch
                { return false; }
            }
        }

        public bool UserAuth(int userId, string guid, bool isDoctor)
        {
            int outputUserId = 0;
            using (SqlConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                conn.Open();

                string sqlQuery;
                if (isDoctor)
                    sqlQuery = "SELECT id FROM Doctor WHERE guid = @GUID";
                else
                    sqlQuery = "SELECT id FROM Patient WHERE guid = @GUID";

                SqlCommand cmd = new SqlCommand(sqlQuery, conn);
                cmd.Parameters.Add("@GUID", SqlDbType.NVarChar).Value = guid;

                try
                {
                    outputUserId = (int)cmd.ExecuteScalar();
                }
                catch { return false; }
            }
            return userId == outputUserId;
        }

        public bool CheckDoctorAcces(int patientId, string guid)
        {
            bool result = false;
            using (SqlConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                conn.Open();

                string sqlQuery =
                    "SELECT CASE WHEN EXISTS ( " +
                    "SELECT * FROM Patient " +
                    "LEFT OUTER JOIN Doctor ON Patient.doctor_id = Doctor.id " +
                    "WHERE Doctor.guid = @GUID AND Patient.id = @PATIENTID )" +
                    "THEN CAST (1 AS BIT) " +
                    "ELSE CAST (0 AS BIT) END";

                SqlCommand cmd = new SqlCommand(sqlQuery, conn);
                cmd.Parameters.Add("@PATIENTID", SqlDbType.Int).Value = patientId;
                cmd.Parameters.Add("@GUID", SqlDbType.NVarChar).Value = guid;

                result = (bool)cmd.ExecuteScalar();
                conn.Close();
            }
            return result;
        }
        public int GetUserId(string guid, bool isDoctor)
        {
            using (SqlConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                string sqlQuery;
                if (isDoctor)
                    sqlQuery = "SELECT id FROM Doctor WHERE guid = @GUID";
                else
                    sqlQuery = "SELECT id FROM Patient WHERE guid = @GUID";

                SqlCommand cmd = new SqlCommand(sqlQuery, conn);
                cmd.Parameters.Add("@GUID", SqlDbType.NVarChar).Value = guid;
                conn.Open();
                try
                {
                    return (int)cmd.ExecuteScalar();
                }
                catch { return 0; }
            }
        }
    }

    public interface IAuthorizationRepository
    {
        bool UserAuth(int userId, string guid, bool isDoctor);
        bool CheckDoctorAcces(int patientId, string guid);
        int GetUserId(string guid, bool isDoctor);
        bool HasAcces(int userId, string guid);
    }
}
