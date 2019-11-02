using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace AppNiZiAPI.Models.Repositories
{
    class MealRepository :IMealRepository
    {
        //TODO laat add en delete iets teruggeven
        public bool AddMeal(Meal meal)
        {
            SqlConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection"));

            string text = $"Select Id From Weightunit where unit = '{meal.WeightUnit}'";
            int weightunitid=1;
            using (SqlCommand cmd = new SqlCommand(text, conn))
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    weightunitid = reader.GetInt32(0);
                }
                conn.Close();
            }
            StringBuilder sqlQuery = new StringBuilder();
            sqlQuery.Append("INSERT INTO Meal (patient_id, name, kcal,protein,fiber,calcium,sodium,portion_size,weight_unit_id,picture) ");
            sqlQuery.Append("OUTPUT INSERTED.id ");
            sqlQuery.Append("VALUES (@PATIENT_ID, @NAME, @KCAL,@PROTEIN,@FIBER,@CALCIUM,@SODIUM,@PORTION_SIZE,@WEIGHT_UNIT_ID,@PICTURE) ");
            using (conn)
            {

                conn.Open();
                SqlCommand sqlCmd = new SqlCommand(sqlQuery.ToString(), conn);
                sqlCmd.Parameters.Add("@PATIENT_ID", SqlDbType.Int).Value = meal.PatientId;
                sqlCmd.Parameters.Add("@NAME", SqlDbType.NVarChar).Value = meal.Name;
                sqlCmd.Parameters.Add("@KCAL", SqlDbType.Float).Value = meal.KCal;
                sqlCmd.Parameters.Add("@PROTEIN", SqlDbType.Float).Value = meal.Protein;
                sqlCmd.Parameters.Add("@FIBER", SqlDbType.Float).Value = meal.Fiber;
                sqlCmd.Parameters.Add("@CALCIUM", SqlDbType.Float).Value = meal.Calcium;
                sqlCmd.Parameters.Add("@SODIUM", SqlDbType.Float).Value = meal.Sodium;
                sqlCmd.Parameters.Add("@PORTION_SIZE", SqlDbType.Int).Value = meal.PortionSize;
                sqlCmd.Parameters.Add("@WEIGHT_UNIT_ID", SqlDbType.Int).Value = weightunitid ;
                sqlCmd.Parameters.Add("@PICTURE", SqlDbType.NVarChar).Value = meal.Picture;
                int rows = sqlCmd.ExecuteNonQuery();
            }
            conn.Close();
            return true;
        }

        public bool DeleteMeal(int patient_id, int meal_id)
        {
            SqlConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection"));
            bool success = false;
            //kan technisch gezien zonder patient_id
            string sqlQuery = "DELETE FROM meal WHERE patient_id=@PATIENT_ID AND id=@MEAL_ID";

            using (conn)
            {
                conn.Open();

                SqlCommand sqlCmd = new SqlCommand(sqlQuery, conn);
                sqlCmd.Parameters.Add("@PATIENT_ID", SqlDbType.Int).Value = patient_id;
                sqlCmd.Parameters.Add("@MEAL_ID", SqlDbType.Int).Value = meal_id;
                int rows = sqlCmd.ExecuteNonQuery();

                if (rows != 0)
                    success = true;
            }
            return success;
        }

        public List<Meal> GetMyMeals(int patient_id)
        {
            SqlConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection"));
            List<Meal> meals = new List<Meal>();
            using (conn)
            {

                conn.Open();
                var text = $"SELECT * FROM Meal Inner Join WeightUnit On  meal.weight_unit_id = WeightUnit.id WHERE patient_id = '{patient_id}'";

                using (SqlCommand cmd = new SqlCommand(text, conn))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Meal meal = new Meal
                        {
                            // Uit lezen bijv
                            MealId = (int)reader["id"],
                            PatientId = (int)reader["patient_id"],
                            Name = (string)reader["name"],
                            KCal =    (double)reader["kcal"],
                            Protein = (double)reader["protein"],
                            Fiber =   (double)reader["fiber"],
                            Calcium = (double)reader["calcium"],
                            Sodium =  (double)reader["sodium"],
                            PortionSize = (double)reader["portion_size"],
                            Picture = (string)reader["picture"],
                            WeightUnit = (string)reader["unit"]
                        };
                        meals.Add(meal);
                    }
                }
                conn.Close();
            }
            return meals;
        }
    }
}
