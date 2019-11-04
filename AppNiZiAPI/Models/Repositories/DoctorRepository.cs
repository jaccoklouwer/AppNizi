using AppNiZiAPI.Models.AccountModels;
using AppNiZiAPI.Models.Enum;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using AppNiZiAPI.Infrastructure;

namespace AppNiZiAPI.Models.Repositories
{
    class DoctorRepository : IDoctorRepository
    {
        public List<Patient> GetDoctorPatients(int doctorId)
        {
            List<Patient> list = new List<Patient>();
            using (SqlConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                conn.Open();
                string sqlQuery =
                "SELECT p.date_of_birth, p.weight, p.id, a.first_name, a.last_name " +
                "FROM Patient AS p " +
                "INNER JOIN Account AS a ON a.id = p.account_id " +
                "WHERE p.doctor_id = @DOCTORID";

                SqlCommand cmd = new SqlCommand(sqlQuery, conn);
                cmd.Parameters.Add("@DOCTORID", SqlDbType.Int).Value = doctorId;

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Patient patient = new Patient();
                    patient.FirstName = reader["first_name"].ToString();
                    patient.LastName = reader["last_name"].ToString();
                    patient.DateOfBirth = (DateTime)reader["date_of_birth"];
                    patient.WeightInKilograms = float.Parse(reader["weight"].ToString());
                    patient.PatientId = (int)reader["id"];

                    list.Add(patient);
                }
                conn.Close();
            }
            return list.Count != 0
                ? list
                : null;
        }

        public DoctorLogin GetLoggedInDoctor(string guid)
        {
            DoctorLogin doctorLogin = null;
            using (SqlConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                string sqlQuery =
                "SELECT d.*, a.first_name, a.last_name, r.role_name " +
                "FROM Doctor AS d " +
                "INNER JOIN Account AS a ON a.id = d.account_id " +
                "INNER JOIN Role AS r ON r.id = a.role " +
                "WHERE d.guid = @GUID";

                SqlCommand cmd = new SqlCommand(sqlQuery, conn);
                cmd.Parameters.Add("@GUID", SqlDbType.NVarChar).Value = guid;

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    doctorLogin = new DoctorLogin
                    {
                        Doctor = new DoctorModel
                        {
                            FirstName = reader["first_name"].ToString(),
                            LastName = reader["last_name"].ToString(),
                            DoctorId = (int)reader["id"],
                            Location = reader["location"].ToString()
                        },
                        Account = new AccountModel
                        {
                            AccountId = (int)reader["account_id"],
                            Role = reader["role_name"].ToString()
                        },
                        Auth = new AuthLogin
                        {
                            Guid = reader["guid"].ToString(),
                        }
                    };
                }
                conn.Close();
            }
            return doctorLogin.Doctor.DoctorId != 0
                ? doctorLogin
                : null;
        }

        public List<DoctorModel> GetDoctors()
        {
            List<DoctorModel> doctors = new List<DoctorModel>();

            using (SqlConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                string sqlQuery =
                "SELECT d.*, a.first_name, a.last_name " +
                "FROM Doctor AS d " +
                "INNER JOIN Account AS a ON a.id = d.account_id";

                SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    DoctorModel doctor = new DoctorModel();
                    doctor.FirstName = reader["first_name"].ToString();
                    doctor.LastName = reader["last_name"].ToString();
                    doctor.DoctorId = (int)reader["id"];
                    doctor.Location = reader["location"].ToString();

                    doctors.Add(doctor);
                }
                conn.Close();
            }
            return doctors.Count != 0
                ? doctors
                : null;
        }

        public DoctorLogin RegisterDoctor(DoctorLogin doctorLogin)
        {
            IAccountRepository accountRepository = DIContainer.Instance.GetService<IAccountRepository>();

            // Return null when GUID already exists in DB
            if (CheckIfExists(doctorLogin.Auth.Guid))
                return null;

            // Register First a new account, if there is an error the AccountId will be 0
            doctorLogin.Account.AccountId = accountRepository.RegisterAccount(doctorLogin.Doctor.FirstName, doctorLogin.Doctor.LastName, (int)Role.Doctor);
            if (doctorLogin.Account.AccountId == 0)
                return null;

            string sqlQuery =
                "INSERT INTO Doctor(account_id, guid, location) " +
                "OUTPUT Inserted.id " +
                "VALUES(@ACCOUNTID, @GUID, @LOCATION) ";

            using (SqlConnection sqlConn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                SqlCommand sqlCmd = new SqlCommand(sqlQuery, sqlConn);
                sqlCmd.Parameters.Add("@ACCOUNTID", SqlDbType.Int).Value = doctorLogin.Account.AccountId;
                sqlCmd.Parameters.Add("@LOCATION", SqlDbType.NVarChar).Value = doctorLogin.Doctor.Location;
                sqlCmd.Parameters.Add("@GUID", SqlDbType.NVarChar).Value = doctorLogin.Auth.Guid;

                try
                {
                    sqlConn.Open();
                    int result = (int)sqlCmd.ExecuteScalar();
                    if (result == 0)
                        return null;
                    doctorLogin.Doctor.DoctorId = result;
                }
                catch
                {
                    return null;
                }
                return doctorLogin;
            }
        }

        public DoctorModel GetDoctorById(int doctorId)
        {
            DoctorModel doctor = null;
            string sqlQuery =
                "SELECT d.*, a.first_name, a.last_name " +
                "FROM Doctor AS d " +
                "INNER JOIN Account AS a ON a.id = d.account_id " +
                "WHERE d.id = @DOCTORID";

            using (SqlConnection sqlConn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                SqlCommand cmd = new SqlCommand(sqlQuery, sqlConn);
                cmd.Parameters.Add("@DOCTORID", SqlDbType.Int).Value = doctorId;

                sqlConn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    try
                    {
                        doctor = new DoctorModel();
                        doctor.DoctorId = doctorId;
                        doctor.FirstName = reader["first_name"].ToString();
                        doctor.LastName = reader["last_name"].ToString();
                        doctor.Location = reader["location"].ToString();
                    }
                    catch { return null; }
                }
            }
            return doctor;
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
                SqlCommand cmd = new SqlCommand(sqlQuery, sqlConn);
                cmd.Parameters.Add("@GUID", SqlDbType.NVarChar).Value = guid;

                sqlConn.Open();
                return (bool)cmd.ExecuteScalar();
            }
        }

        public bool Delete(int doctorId)
        {
            bool success = false;
            int accountId = 0;

            string sqlQuery = "SELECT account_id FROM Doctor WHERE id=@DOCTORID";
            using (SqlConnection sqlConn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                SqlCommand sqlCmd = new SqlCommand(sqlQuery, sqlConn);
                sqlCmd.Parameters.Add("@DOCTORID", SqlDbType.Int).Value = doctorId;

                try
                {
                    sqlConn.Open();
                    accountId = (int)sqlCmd.ExecuteScalar();
                    sqlConn.Close();
                }
                catch{ return false; }
                if (accountId == 0)
                    return false;
            }

            sqlQuery =
                "DELETE FROM patient WHERE id=@DOCTORID;" +
                "DELETE FROM Account WHERE id=@ACCOUNTID";

            using (SqlConnection sqlConn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                SqlCommand sqlCmd = new SqlCommand(sqlQuery, sqlConn);
                sqlCmd.Parameters.Add("@DOCTORID", SqlDbType.Int).Value = doctorId;
                sqlCmd.Parameters.Add("@ACCOUNTID", SqlDbType.Int).Value = accountId;

                sqlConn.Open();
                int rows = sqlCmd.ExecuteNonQuery();
                if (rows > 0)
                    success = true;
            }
            return success;
        }
    }

    public interface IDoctorRepository
    {
        List<Patient> GetDoctorPatients(int doctorId);
        DoctorLogin GetLoggedInDoctor(string guid);
        List<DoctorModel> GetDoctors();
        DoctorLogin RegisterDoctor(DoctorLogin newDoctor);
        DoctorModel GetDoctorById(int doctorId);
        bool Delete(int doctorId);
    }
}
