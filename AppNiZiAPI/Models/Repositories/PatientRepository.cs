using AppNiZiAPI.Models.AccountModels;
using AppNiZiAPI.Models.Views;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;

namespace AppNiZiAPI.Models.Repositories
{
    public interface IPatientRepository
    {
        PatientObject Select(int id);
        PatientObject Select(string guid);
        List<PatientView> List(int count);
        int Insert(PatientObject patient);
        bool Delete(string guid);
        PatientLogin GetPatientInfo(string guid);
    }

    public class PatientRepository : IPatientRepository
    {
        /// <summary>
        /// Select patient by ID.
        /// </summary>
        public PatientObject Select(int id)
        {
            if (id == 0)
                return null;

            PatientObject patient = null;

            using (SqlConnection sqlConn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                sqlConn.Open();
                string sqlQuery = $"SELECT * FROM patient WHERE id=@ID";

                SqlCommand sqlCmd = new SqlCommand(sqlQuery, sqlConn);
                sqlCmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;
                SqlDataReader reader = sqlCmd.ExecuteReader();

                while (reader.Read())
                {
                    patient = new PatientObject()
                    {
                        Guid = reader["guid"].ToString(),
                        DateOfBirth = Convert.ToDateTime(reader["date_of_birth"]),
                        WeightInKilograms = Convert.ToInt32(reader["weight"])
                    };
                }
            }

            return patient;
        }

        public PatientObject Select(string guid)
        {
            if (string.IsNullOrEmpty(guid))
                return null;

            PatientObject patient = null;

            using (SqlConnection sqlConn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                sqlConn.Open();
                string sqlQuery = $"SELECT * FROM patient WHERE guid=@GUID";

                SqlCommand sqlCmd = new SqlCommand(sqlQuery, sqlConn);
                sqlCmd.Parameters.Add("@GUID", SqlDbType.NVarChar).Value = guid;
                SqlDataReader reader = sqlCmd.ExecuteReader();

                while (reader.Read())
                {
                    patient = new PatientObject()
                    {
                        Guid = reader["guid"].ToString(),
                        DateOfBirth = Convert.ToDateTime(reader["date_of_birth"]),
                        WeightInKilograms = Convert.ToInt32(reader["weight"])
                    };
                }
            }

            return patient;
        }

        /// <summary>
        /// Select all patients, up to count amount.
        /// </summary>
        public List<PatientView> List(int count)
        {
            if (count == 0)
                count = 999999;

            List<PatientView> patients = new List<PatientView>();

            using (SqlConnection sqlConn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                sqlConn.Open();
                string sqlQuery = $"SELECT TOP(@COUNT) * FROM patient";

                SqlCommand sqlCmd = new SqlCommand(sqlQuery, sqlConn);
                sqlCmd.Parameters.Add("@COUNT", SqlDbType.Int).Value = count;
                SqlDataReader reader = sqlCmd.ExecuteReader();

                while (reader.Read())
                {
                    PatientView patient = new PatientView
                    {
                        GUID = reader["guid"].ToString(),
                        dateOfBirth = Convert.ToDateTime(reader["date_of_birth"]),
                        weight = Convert.ToInt32(reader["weight"])
                    };

                    patients.Add(patient);
                }

                sqlConn.Close();
            }

            return patients;
        }

        public int Insert(PatientObject patient)
        {
            if (patient == null) return 0;

            int createdObjectId = 0;

            StringBuilder sqlQuery = new StringBuilder();
            sqlQuery.Append("INSERT INTO patient (date_of_birth, weight, guid) ");
            sqlQuery.Append("OUTPUT INSERTED.id ");
            sqlQuery.Append("VALUES (@DATEOFBIRTH, @WEIGHT, @GUID) ");

            using (SqlConnection sqlConn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                sqlConn.Open();

                SqlCommand sqlCmd = new SqlCommand(sqlQuery.ToString(), sqlConn);
                sqlCmd.Parameters.Add("@DATEOFBIRTH", SqlDbType.DateTime).Value = patient.DateOfBirth;
                sqlCmd.Parameters.Add("@WEIGHT", SqlDbType.Int).Value = patient.WeightInKilograms;
                sqlCmd.Parameters.Add("@GUID", SqlDbType.NVarChar).Value = Guid.NewGuid().ToString();

                createdObjectId = (int)sqlCmd.ExecuteScalar();
            }

            return createdObjectId;
        }

        public bool Delete(string guid)
        {
            if (string.IsNullOrEmpty(guid)) return false;

            bool success = false;

            string sqlQuery = "DELETE FROM patient WHERE guid=@GUID";

            using (SqlConnection sqlConn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                sqlConn.Open();

                SqlCommand sqlCmd = new SqlCommand(sqlQuery, sqlConn);
                sqlCmd.Parameters.Add("@GUID", SqlDbType.NVarChar).Value = guid;

                int rows = sqlCmd.ExecuteNonQuery();
                if (rows > 0)
                    success = true;
            }

            return success;
        }

        public PatientLogin GetPatientInfo(string guid)
        {
            PatientLogin patientLogin = null;
            string sqlQuery =
                "SELECT a.first_name, a.last_name, a.role, r.role_name, ac.first_name AS doctor_first_name, ac.last_name as doctor_last_name, p.id AS patient_id, p.* " +
                "FROM Patient AS p " +
                "INNER JOIN Account AS a ON p.account_id = a.id " +
                "INNER JOIN Doctor AS d ON p.doctor_id = d.id " +
                "INNER JOIN Account AS ac ON ac.id = d.account_id " +
                "INNER JOIN Role AS r ON r.id = a.role " +
                "WHERE p.guid = @GUID";

            using (SqlConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                SqlCommand sqlCmd = new SqlCommand(sqlQuery.ToString(), conn);
                sqlCmd.Parameters.Add("@GUID", SqlDbType.NVarChar).Value = guid;

                conn.Open();

                SqlDataReader reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    AccountModel account = new AccountModel
                    {
                        AccountId = (int)reader["account_id"],
                        Role = (string)reader["role_name"]
                    };

                    PatientObject patient = new PatientObject
                    {
                        DateOfBirth = (DateTime)reader["date_of_birth"],
                        FirstName = (string)reader["first_name"],
                        LastName = (string)reader["last_name"],
                        Guid = (string)reader["guid"],
                        PatientId = (int)reader["patient_id"],
                        WeightInKilograms =  float.Parse(reader["weight"].ToString())
                    };

                    Doctor doctor = new Doctor
                    {
                        FirstName = (string)reader["doctor_first_name"],
                        LastName = (string)reader["doctor_last_name"],
                        DoctorId = (int)reader["doctor_id"]
                    };

                    patientLogin = new PatientLogin
                    {
                        Account = account,
                        Patient = patient,
                        Doctor = doctor
                    };
                }
                conn.Close();
            }
            return patientLogin;
        } 
    }
}
