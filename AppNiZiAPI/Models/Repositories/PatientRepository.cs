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
        /// Fetch 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<PatientView> Select(int count)
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
    }
}
