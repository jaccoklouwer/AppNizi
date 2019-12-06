using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace AppNiZiAPI.Models.Repositories
{
    class MealRepository :IMealRepository
    {
        public async Task<Meal> AddMeal(Meal meal)
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
                int rows = await sqlCmd.ExecuteNonQueryAsync();
                
            }
            conn.Close();
            return await GetMealbyName(meal.Name);
        }

        public async Task<bool> DeleteMeal(int patient_id, int meal_id)
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
                int rows = await sqlCmd.ExecuteNonQueryAsync();

                if (rows != 0)
                    success = true;
            }
            return success;
        }

        public async Task<Meal> GetMealbyName(string name)
        {
            SqlConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection"));
            Meal meal = new Meal();
            using (conn)
            {

                conn.Open();
                var text = $"SELECT * FROM Meal inner join WeightUnit on meal.weight_unit_id = Weightunit.id where name = '{name}' ";

                using (SqlCommand cmd = new SqlCommand(text, conn))
                {
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        // Uit lezen bijv
                        meal.MealId = (int)reader["id"];
                        meal.PatientId = (int)reader["patient_id"];
                        meal.Name = (string)reader["name"];
                        meal.KCal = (double)reader["kcal"];
                        meal.Protein = (double)reader["protein"];
                        meal.Fiber = (double)reader["fiber"];
                        meal.Calcium = (double)reader["calcium"];
                        meal.Sodium = (double)reader["sodium"];
                        meal.PortionSize = (double)reader["portion_size"];
                        meal.Picture = (string)reader["picture"];
                        meal.WeightUnit = (string)reader["unit"];
                    }
                }
                conn.Close();
            }
            return meal;
        }

        public async Task<List<Meal>> GetMyMeals(int patient_id)
        {
            SqlConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection"));
            List<Meal> meals = new List<Meal>();
            using (conn)
            {

                conn.Open();
                var text = $"SELECT * FROM Meal Inner Join WeightUnit On  meal.weight_unit_id = WeightUnit.id WHERE patient_id = '{patient_id}'";

                using (SqlCommand cmd = new SqlCommand(text, conn))
                {
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
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

        public async Task<Meal> PutMeal(Meal meal)
        {
            SqlConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection"));

            string text = $"Select Id From Weightunit where unit = '{meal.WeightUnit}'";
            int weightunitid = 1;
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
            sqlQuery.Append("Update Meal ");
            sqlQuery.Append("set name = @NAME , kcal = @KCAL,protein= @PROTEIN,fiber = @FIBER,calcium =@CALCIUM,sodium =@SODIUM,portion_size = @PORTION_SIZE,weight_unit_id = @WEIGHT_UNIT_ID,picture = @PICTURE ");
            sqlQuery.Append("WHERE id = @MEAL_ID");
            using (conn)
            {

                conn.Open();
                SqlCommand sqlCmd = new SqlCommand(sqlQuery.ToString(), conn);
                sqlCmd.Parameters.Add("@PATIENT_ID", SqlDbType.Int).Value = meal.PatientId;
                sqlCmd.Parameters.Add("@MEAL_ID", SqlDbType.Int).Value = meal.MealId;
                sqlCmd.Parameters.Add("@NAME", SqlDbType.NVarChar).Value = meal.Name;
                sqlCmd.Parameters.Add("@KCAL", SqlDbType.Float).Value = meal.KCal;
                sqlCmd.Parameters.Add("@PROTEIN", SqlDbType.Float).Value = meal.Protein;
                sqlCmd.Parameters.Add("@FIBER", SqlDbType.Float).Value = meal.Fiber;
                sqlCmd.Parameters.Add("@CALCIUM", SqlDbType.Float).Value = meal.Calcium;
                sqlCmd.Parameters.Add("@SODIUM", SqlDbType.Float).Value = meal.Sodium;
                sqlCmd.Parameters.Add("@PORTION_SIZE", SqlDbType.Int).Value = meal.PortionSize;
                sqlCmd.Parameters.Add("@WEIGHT_UNIT_ID", SqlDbType.Int).Value = weightunitid;
                sqlCmd.Parameters.Add("@PICTURE", SqlDbType.NVarChar).Value = meal.Picture;
                int rows = await sqlCmd.ExecuteNonQueryAsync();

            }
            conn.Close();
            return await GetMealbyName(meal.Name);
        }
    }
}
