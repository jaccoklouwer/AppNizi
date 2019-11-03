using AppNiZiAPI.Models.AccountModels;
using AppNiZiAPI.Models.Enum;
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
        Patient Select(int id);
        Patient Select(string guid);
        List<Patient> List(int count);
        bool Delete(int patientId);
        PatientLogin GetPatientInfo(string guid);
        PatientLogin RegisterPatient(PatientLogin newPatient);
    }

    public class PatientRepository : IPatientRepository
    {
        /// <summary>
        /// Select patient by ID.
        /// </summary>
        public Patient Select(int id)
        {
            if (id == 0)
                return new Patient();

            Patient patient = new Patient();

            using (SqlConnection sqlConn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                sqlConn.Open();
                string sqlQuery = $"SELECT p.*, a.* " +
                    $"FROM Patient AS p " +
                    $"INNER JOIN Account AS A ON p.account_id = a.id " +
                    $"WHERE p.id=@ID";

                SqlCommand sqlCmd = new SqlCommand(sqlQuery, sqlConn);
                sqlCmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;
                SqlDataReader reader = sqlCmd.ExecuteReader();

                while (reader.Read())
                {
                    patient.PatientId = (int)reader["id"];
                    patient.DoctorId = (int)reader["doctor_id"];
                    patient.AccountId = (int)reader["account_id"];
                    patient.Guid = reader["guid"].ToString();
                    patient.DateOfBirth = Convert.ToDateTime(reader["date_of_birth"]);
                    patient.WeightInKilograms = Convert.ToInt32(reader["weight"]);
                    patient.FirstName = reader["first_name"].ToString();
                    patient.LastName = reader["last_name"].ToString();
                }
            }

            return patient;
        }

        public Patient Select(string guid)
        {
            if (string.IsNullOrEmpty(guid))
                return new Patient();

            Patient patient = new Patient();

            using (SqlConnection sqlConn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                sqlConn.Open();
                string sqlQuery = $"SELECT * FROM patient WHERE guid=@GUID";

                SqlCommand sqlCmd = new SqlCommand(sqlQuery, sqlConn);
                sqlCmd.Parameters.Add("@GUID", SqlDbType.NVarChar).Value = guid;
                SqlDataReader reader = sqlCmd.ExecuteReader();

                while (reader.Read())
                {
                    patient = new Patient()
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
        public List<Patient> List(int count)
        {
            if (count == 0)
                count = 999999;

            List<Patient> patients = new List<Patient>();

            using (SqlConnection sqlConn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                sqlConn.Open();
                string sqlQuery = $"SELECT TOP(@COUNT) p.*, a.* " +
                    $"FROM Patient AS p " +
                    $"INNER JOIN Account AS A ON p.account_id = a.id";

                SqlCommand sqlCmd = new SqlCommand(sqlQuery, sqlConn);
                sqlCmd.Parameters.Add("@COUNT", SqlDbType.Int).Value = count;
                SqlDataReader reader = sqlCmd.ExecuteReader();

                while (reader.Read())
                {
                    Patient patient = new Patient
                    {
                        PatientId = (int)reader["id"],
                        Guid = reader["guid"].ToString(),
                        DateOfBirth = Convert.ToDateTime(reader["date_of_birth"]),
                        WeightInKilograms = Convert.ToInt32(reader["weight"]),
                        FirstName = reader["first_name"].ToString(),
                        LastName = reader["last_name"].ToString()
                    };

                    patients.Add(patient);
                }

                sqlConn.Close();
            }

            return patients;
        }

        public PatientLogin RegisterPatient(PatientLogin newPatient)
        {
            // Return null when GUID already exists in DB
            if (CheckIfExists(newPatient.Patient.Guid))
                return null;

            // Register First a new account, if there is an error the AccountId will be 0
            newPatient.Account.AccountId = RegisterAccount(newPatient.Patient);
            if (newPatient.Account.AccountId == 0)
                return null;

            string sqlQuery =
                "INSERT INTO Patient(account_id, date_of_birth, doctor_id, guid, weight) " +
                "OUTPUT Inserted.id " +
                "VALUES(@ACCOUNTID, @DATE, @DOCTORID, @GUID, @WEIGHT) ";

            using (SqlConnection sqlConn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                SqlCommand sqlCmd = new SqlCommand(sqlQuery, sqlConn);
                sqlCmd.Parameters.Add("@ACCOUNTID", SqlDbType.Int).Value = newPatient.Account.AccountId;
                sqlCmd.Parameters.Add("@DATE", SqlDbType.Date).Value = newPatient.Patient.DateOfBirth;
                sqlCmd.Parameters.Add("@DOCTORID", SqlDbType.Int).Value = newPatient.Doctor.DoctorId;
                sqlCmd.Parameters.Add("@GUID", SqlDbType.NVarChar).Value = newPatient.Patient.Guid;
                sqlCmd.Parameters.Add("@WEIGHT", SqlDbType.Float).Value = newPatient.Patient.WeightInKilograms;

                sqlConn.Open();
                int result = sqlCmd.ExecuteNonQuery();
                if (result < 0)
                    return null;
            }

            // Get all patient info, included the doctor
            return GetPatientInfo(newPatient.Patient.Guid);
        }

        private int RegisterAccount(Patient patient)
        {
            string sqlQuery =
                "INSERT INTO Account(first_name, last_name, role) " +
                "OUTPUT Inserted.id " +
                "VALUES(@FIRSTNAME, @LASTNAME, @ROLE)";

            using (SqlConnection sqlConn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                SqlCommand sqlCmd = new SqlCommand(sqlQuery, sqlConn);
                sqlCmd.Parameters.Add("@FIRSTNAME", SqlDbType.NVarChar).Value = patient.FirstName;
                sqlCmd.Parameters.Add("@LASTNAME", SqlDbType.NVarChar).Value = patient.LastName;
                sqlCmd.Parameters.Add("@ROLE", SqlDbType.Int).Value = Role.Patient;

                sqlConn.Open();
                return (int)sqlCmd.ExecuteScalar();
            }
        }

        private bool CheckIfExists(string guid)
        {
            string sqlQuery =
                "SELECT CASE WHEN EXISTS ( " +
                "SELECT * FROM Patient WHERE guid = @GUID ) " +
                "OR EXISTS ( SELECT * FROM Doctor WHERE guid = @GUID ) " +
                "THEN CAST (1 AS BIT) ELSE CAST (0 AS BIT) END";

            using (SqlConnection sqlConn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                SqlCommand sqlCmd = new SqlCommand(sqlQuery, sqlConn);
                sqlCmd.Parameters.Add("@GUID", SqlDbType.NVarChar).Value = guid;
                //sqlCmd.Parameters.Add("@DOCTERGUID", SqlDbType.NVarChar).Value = guid;

                sqlConn.Open();
                return (bool)sqlCmd.ExecuteScalar();
            }
        }

        public bool Delete(int patientId)
        {
            bool success = false;


            string sqlQuery = "DELETE FROM patient WHERE id=@PATIENTID";

            using (SqlConnection sqlConn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                sqlConn.Open();

                SqlCommand sqlCmd = new SqlCommand(sqlQuery, sqlConn);
                sqlCmd.Parameters.Add("@PATIENTID", SqlDbType.Int).Value = patientId;

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

                    Patient patient = new Patient
                    {
                        DateOfBirth = (DateTime)reader["date_of_birth"],
                        FirstName = (string)reader["first_name"],
                        LastName = (string)reader["last_name"],
                        Guid = (string)reader["guid"],
                        PatientId = (int)reader["patient_id"],
                        WeightInKilograms = float.Parse(reader["weight"].ToString())
                    };

                    DoctorModel doctor = new DoctorModel
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
