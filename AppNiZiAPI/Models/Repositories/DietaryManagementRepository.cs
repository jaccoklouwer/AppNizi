﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using AppNiZiAPI.Models.Dietarymanagement;

namespace AppNiZiAPI.Models.Repositories
{
    public interface IDietaryManagementRepository
    {
        Task<bool> AddDietaryManagement(DietaryManagementModel dietary);
        Task<bool> DeleteDietaryManagement(int id);

        Task<bool> DeleteByPatientId(int patientId);

        Task<List<DietaryManagementModel>> GetDietaryManagementByPatientAsync(int patientId);
        Task<List<DietaryRestriction>> GetDietaryRestrictions();
        Task<bool> UpdateDietaryManagement(int id, DietaryManagementModel dietaryManagement);
    }

    public class DietaryManagementRepository : Repository, IDietaryManagementRepository
    {

        public async Task<List<DietaryManagementModel>> GetDietaryManagementByPatientAsync(int patientId)
        {

            List<DietaryManagementModel> dietaryManagementModels = new List<DietaryManagementModel>();

            conn.Open();
            var text = $"SELECT b.description, a.amount, a.is_active, a.patient_id, a.id " +
                $"FROM DietaryManagement AS a, DietaryRestriction AS b " +
                $"WHERE a.dietary_restriction_id = b.id AND a.patient_id={patientId}";

            using (SqlCommand cmd = new SqlCommand(text, conn))
            {
                try
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    DietaryManagementModel dietaryManagementModel = new DietaryManagementModel();
                    while (reader.Read())
                    {

                        // Uit lezen bijv
                        dietaryManagementModel.Id = Int32.Parse(reader["id"].ToString());
                        dietaryManagementModel.Description = reader["description"].ToString();
                        dietaryManagementModel.Amount = Int32.Parse(reader["amount"].ToString());
                        dietaryManagementModel.PatientId = Int32.Parse(reader["patient_id"].ToString());
                        dietaryManagementModel.IsActive = Convert.ToBoolean(reader["is_active"]);

                        dietaryManagementModels.Add(dietaryManagementModel);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            conn.Close();
            return await Task.FromResult(dietaryManagementModels);
        }

        public async Task<List<DietaryRestriction>> GetDietaryRestrictions()
        {
            List<DietaryRestriction> dietaryRestrictions = new List<DietaryRestriction>();
            conn.Open();
            var query = "SELECT * FROM DietaryRestriction";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                try
                {
                    DietaryRestriction restriction = new DietaryRestriction();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        restriction.Id = Int32.Parse(reader["id"].ToString());
                        restriction.Description = reader["description"].ToString();
                        dietaryRestrictions.Add(restriction);
                    }
                }
                catch (Exception e)
                {

                    throw e;
                }
            }

            conn.Close();
            return await Task.FromResult(dietaryRestrictions);
        }


        public async Task<bool> UpdateDietaryManagement(int id, DietaryManagementModel dietaryManagement)
        {

            conn.Open();
            var query = @"UPDATE DietaryManagement
                                SET dietary_restriction_id = (SELECT r.id
                                                                FROM DietaryRestriction as r
                                                                WHERE r.description = @DESCRIPTION)
                                  ,amount = @AMOUNT
                                  ,patient_id = @PATIENT
                                  ,is_active = @ISACTIVE
                             WHERE id = @ID";

            SqlCommand command = new SqlCommand(query, conn);
            command.Parameters.Add("@DESCRIPTION", SqlDbType.VarChar).Value = dietaryManagement.Description;
            command.Parameters.Add("@AMOUNT", SqlDbType.Int).Value = dietaryManagement.Amount;
            command.Parameters.Add("@PATIENT", SqlDbType.Int).Value = dietaryManagement.PatientId;
            command.Parameters.Add("@ISACTIVE", SqlDbType.Bit).Value = dietaryManagement.IsActive;
            command.Parameters.Add("@ID", SqlDbType.Int).Value = id;
            bool succes = (command.ExecuteNonQuery() > 0);
            conn.Close();
            return await Task.FromResult(succes);
        }

        public async Task<bool> DeleteDietaryManagement(int id)
        {

            conn.Open();
            var query = @"DELETE FROM DietaryManagement
                                WHERE id = @ID";

            try
            {
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.Add("@ID", SqlDbType.Int).Value = id;
                bool succes = (command.ExecuteNonQuery() > 0);
                conn.Close();
                return await Task.FromResult(succes);
            }
            catch (Exception)
            {
                conn.Close();
                return false;
            }

        }

        public async Task<bool> AddDietaryManagement(DietaryManagementModel dietary)
        {
            conn.Open();
            var query = @"INSERT INTO DietaryManagement
                                   (dietary_restriction_id,
                                    amount,
                                    patient_id,
                                    is_active)
                             VALUES
                                   (
                                    (
                                    SELECT r.id
                                        FROM DietaryRestriction as r
                                        WHERE r.description = @DESCRIPTION
                                    ),
                                    @AMOUNT,
                                    @PATIENT,
                                    @ISACTIVE
                                )";

            try
            {
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.Add("@DESCRIPTION", SqlDbType.VarChar).Value = dietary.Description;
                command.Parameters.Add("@AMOUNT", SqlDbType.Int).Value = dietary.Amount;
                command.Parameters.Add("@PATIENT", SqlDbType.Int).Value = dietary.PatientId;
                command.Parameters.Add("@ISACTIVE", SqlDbType.Bit).Value = dietary.IsActive;
                bool succes = (command.ExecuteNonQuery() > 0);
                conn.Close();
                return await Task.FromResult(succes);
            }
            catch (Exception e)
            {
                conn.Close();
                throw e;
            }

        }

        public async Task<bool> DeleteByPatientId(int patientId)
        {
            bool success = false;

            string sqlQuery = "DELETE FROM DietaryManagement WHERE patient_id = @ID";

            using (SqlConnection sqlConn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {
                sqlConn.Open();

                SqlCommand sqlCmd = new SqlCommand(sqlQuery, sqlConn);
                sqlCmd.Parameters.Add("@ID", SqlDbType.Int).Value = patientId;

                int rows = sqlCmd.ExecuteNonQuery();
                if (rows > 0)
                    success = true;
                sqlConn.Close();
            }

            return await Task.FromResult(success);
        }
    }
}
