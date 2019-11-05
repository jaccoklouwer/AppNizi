using AppNiZiAPI.Models.AccountModels;
using AppNiZiAPI.Models.Enum;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace AppNiZiAPI.Models.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        public bool CheckIfExists(string guid)
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

                sqlConn.Open();
                return (bool)sqlCmd.ExecuteScalar();
            }
        }

        // Returned new account id
        public int RegisterAccount(string firstName, string lastName, int role)
        {
            string sqlQuery =
                "INSERT INTO Account(first_name, last_name, role) " +
                "OUTPUT Inserted.id " +
                "VALUES(@FIRSTNAME, @LASTNAME, @ROLE)";

            using (SqlConnection sqlConn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                SqlCommand sqlCmd = new SqlCommand(sqlQuery, sqlConn);
                sqlCmd.Parameters.Add("@FIRSTNAME", SqlDbType.NVarChar).Value = firstName;
                sqlCmd.Parameters.Add("@LASTNAME", SqlDbType.NVarChar).Value = lastName;
                sqlCmd.Parameters.Add("@ROLE", SqlDbType.Int).Value = role;

                sqlConn.Open();
                return (int)sqlCmd.ExecuteScalar();
            }
        }

        public bool Delete(int accountId)
        {
            bool success = false;

            string sqlQuery = "DELETE FROM account WHERE id=@ID";

            using (SqlConnection sqlConn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                sqlConn.Open();

                SqlCommand sqlCmd = new SqlCommand(sqlQuery, sqlConn);
                sqlCmd.Parameters.Add("@ID", SqlDbType.Int).Value = accountId;

                int rows = sqlCmd.ExecuteNonQuery();
                if (rows > 0)
                    success = true;
            }

            return success;
        }

        public AccountModel RegisterAccount(Patient patient, Role role)
        {
            AccountModel accountModel = new AccountModel();

            string sqlQuery =
                "INSERT INTO Account(first_name, last_name, role) " +
                "OUTPUT Inserted.id " +
                "VALUES(@FIRSTNAME, @LASTNAME, @ROLE)";

            using (SqlConnection sqlConn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                SqlCommand sqlCmd = new SqlCommand(sqlQuery, sqlConn);
                sqlCmd.Parameters.Add("@FIRSTNAME", SqlDbType.NVarChar).Value = patient.FirstName;
                sqlCmd.Parameters.Add("@LASTNAME", SqlDbType.NVarChar).Value = patient.LastName;
                sqlCmd.Parameters.Add("@ROLE", SqlDbType.Int).Value = role;

                sqlConn.Open();
                accountModel.AccountId = (int)sqlCmd.ExecuteScalar();
                accountModel.Role = role.ToString();
            }

            return accountModel;
        }
    }

    public interface IAccountRepository
    {
        AccountModel RegisterAccount(Patient patient, Role role);
        bool CheckIfExists(string guid);
        int RegisterAccount(string firstName, string lastName, int role);
        bool Delete(int accountId);
    }
}
