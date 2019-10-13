using AppNiZiAPI.Models.Views;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;

namespace AppNiZiAPI.Models.Repositories
{
    class PatientRepository
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
                        guid = reader["guid"].ToString(),
                        dateOfBirth = Convert.ToDateTime(reader["date_of_birth"]),
                        weightInKilograms = Convert.ToInt32(reader["weight"])
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
                sqlCmd.Parameters.Add("@DATEOFBIRTH", SqlDbType.DateTime).Value = patient.dateOfBirth;
                sqlCmd.Parameters.Add("@WEIGHT", SqlDbType.Int).Value = patient.weightInKilograms;
                sqlCmd.Parameters.Add("@GUID", SqlDbType.NVarChar).Value = Guid.NewGuid().ToString();

                createdObjectId = (int) sqlCmd.ExecuteScalar();
            }

            return createdObjectId;
        }
    }
}
