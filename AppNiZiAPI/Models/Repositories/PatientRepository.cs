using AppNiZiAPI.Models.AccountModels;
using AppNiZiAPI.Models.Enum;
using AppNiZiAPI.Models.Views;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace AppNiZiAPI.Models.Repositories
{
    public interface IPatientRepository
    {
        Task<Patient> Select(int id);
        Task<Patient> Select(string guid);
        Task<List<Patient>> List(int count);
        Task<bool> Delete(int patientId);
        Task<PatientLogin> GetPatientInfo(string guid);
        Task<PatientLogin> RegisterPatient(PatientLogin newPatient);
        Task<bool> Update(PatientUpdateModel patient);
    }

    public class PatientRepository : IPatientRepository
    {
        /// <summary>
        /// Select patient by ID.
        /// </summary>
        public async Task<Patient> Select(int id)
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
                SqlDataReader reader = await sqlCmd.ExecuteReaderAsync();

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

        public async Task<Patient> Select(string guid)
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
                SqlDataReader reader = await sqlCmd.ExecuteReaderAsync();

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
        public async Task<List<Patient>> List(int count)
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
                SqlDataReader reader = await sqlCmd.ExecuteReaderAsync();

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

        public async Task<PatientLogin> RegisterPatient(PatientLogin newPatient)
        {
            // Return null when GUID already exists in DB
            if (newPatient.Patient.Guid != null)
                if (await CheckIfExists(newPatient.Patient.Guid))
                    return null;

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
                newPatient.Patient.AccountId = newPatient.Account.AccountId;

                sqlCmd.Parameters.Add("@DATE", SqlDbType.Date).Value = newPatient.Patient.DateOfBirth;

                sqlCmd.Parameters.Add("@DOCTORID", SqlDbType.Int).Value = newPatient.Patient.DoctorId;

                sqlCmd.Parameters.Add("@GUID", SqlDbType.NVarChar).Value = Guid.NewGuid().ToString();
                sqlCmd.Parameters.Add("@WEIGHT", SqlDbType.Float).Value = newPatient.Patient.WeightInKilograms;

                sqlConn.Open();
                newPatient.Patient.PatientId = (int) await sqlCmd.ExecuteScalarAsync();
            }

            return newPatient;
        }

        private async Task<bool> CheckIfExists(string guid)
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
                return (bool) await sqlCmd.ExecuteScalarAsync();
            }
        }

        public async Task<bool> Delete(int patientId)
        {
            bool success = false;


            string sqlQuery = "DELETE FROM patient WHERE id=@PATIENTID";

            using (SqlConnection sqlConn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                sqlConn.Open();

                SqlCommand sqlCmd = new SqlCommand(sqlQuery, sqlConn);
                sqlCmd.Parameters.Add("@PATIENTID", SqlDbType.Int).Value = patientId;

                int rows = await sqlCmd.ExecuteNonQueryAsync();
                if (rows > 0)
                    success = true;
            }

            return success;
        }

        public async Task<bool> Update(PatientUpdateModel patient)
        {
            bool success = false;

            string sqlQuery = BuildUpdateQuery(patient);

            using (SqlConnection sqlConn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                sqlConn.Open();

                SqlCommand sqlCmd = new SqlCommand(sqlQuery, sqlConn);

                sqlCmd.Parameters.Add("@PATIENTID", SqlDbType.Int).Value = patient.PatientId;

                if (patient.DateOfBirth != null && patient.DateOfBirth != new DateTime())
                    sqlCmd.Parameters.Add("@DOB", SqlDbType.Date).Value = patient.DateOfBirth;

                if (patient.DoctorId > 0)
                    sqlCmd.Parameters.Add("@DOCTORID", SqlDbType.Int).Value = patient.DoctorId;

                if (patient.WeightInKilograms > 0)
                    sqlCmd.Parameters.Add("@WEIGHT", SqlDbType.Float).Value = patient.WeightInKilograms;

                int rows = await sqlCmd.ExecuteNonQueryAsync();
                if (rows > 0)
                    success = true;
            }

            return success;
        }

        private string BuildUpdateQuery(PatientUpdateModel patient)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE patient ");
            sb.Append("SET ");

            bool isNotFirst = false;
            if (patient.DateOfBirth != null && patient.DateOfBirth != new DateTime())
            {
                sb.Append("date_of_birth=@DOB");
                isNotFirst = true;
            }
            if (patient.WeightInKilograms > 0)
            {
                if (isNotFirst)
                    sb.Append(", ");

                sb.Append("weight=@WEIGHT");
                isNotFirst = true;
            }
            if (patient.DoctorId > 0)
            {
                if (isNotFirst)
                    sb.Append(", ");

                sb.Append("doctor_id=@DOCTORID");
                isNotFirst = true;
            }

            sb.Append(" WHERE id=@PATIENTID");

            return sb.ToString();
        }

        public async Task<PatientLogin> GetPatientInfo(string guid)
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

                SqlDataReader reader = await sqlCmd.ExecuteReaderAsync();
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
