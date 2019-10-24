using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace AppNiZiAPI.Models.Repositories
{
    class MealRepository : Repository, IMealRepository
    {
        //TODO laat add en delete iets teruggeven
        public bool AddMeal(int patient_id, Meal meal)
        {
            //TODO get rid of weightunitid das beetje viezigheid zoals het nu is
            StringBuilder sqlQuery = new StringBuilder();
            sqlQuery.Append("INSERT INTO Meal (patient_id, name, kcal,protein,fiber,calcium,sodium,portion_size,weight_unit_id) ");
            sqlQuery.Append("OUTPUT INSERTED.id ");
            sqlQuery.Append("VALUES (@PATIENT_ID, @NAME, @KCAL,@PROTEIN,@FIBER,@CALCIUM,@SODIUM,@PORTION_SIZE,@WEIGHT_UNIT_ID) ");
            using (conn)
            {

                conn.Open();
                int createdObjectId = 0;
                SqlCommand sqlCmd = new SqlCommand(sqlQuery.ToString(), conn);
                sqlCmd.Parameters.Add("@PATIENT_ID", SqlDbType.Int).Value = meal.PatientId;
                sqlCmd.Parameters.Add("@NAME", SqlDbType.NVarChar).Value = meal.Name;
                sqlCmd.Parameters.Add("@KCAL", SqlDbType.Float).Value = meal.KCal;
                sqlCmd.Parameters.Add("@PROTEIN", SqlDbType.Float).Value = meal.Protein;
                sqlCmd.Parameters.Add("@FIBER", SqlDbType.Float).Value = meal.Fiber;
                sqlCmd.Parameters.Add("@CALCIUM", SqlDbType.Float).Value = meal.Calcium;
                sqlCmd.Parameters.Add("@SODIUM", SqlDbType.Float).Value = meal.Sodium;
                sqlCmd.Parameters.Add("@PORTION_SIZE", SqlDbType.Int).Value = meal.PortionSize;
                sqlCmd.Parameters.Add("@WEIGHT_UNIT_ID", SqlDbType.Int).Value = meal.WeightUnitId;
            }
            conn.Close();
            return true;
        }

        public bool DeleteMeal(int patient_id, int meal_id)
        {
            bool success = false;
            //kan technisch gezien zonder patient_id
            string sqlQuery = "DELETE FROM meal WHERE patient_id=@PATIENT_ID AND meal_id=@MEAL_ID";

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
            List<Meal> meals = new List<Meal>();
            using (conn)
            {

                conn.Open();
                var text = $"SELECT * FROM Food WHERE patient_id = '{patient_id}'";

                using (SqlCommand cmd = new SqlCommand(text, conn))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    Meal meal = new Meal
                    {
                        // Uit lezen bijv
                        MealId = (int)reader["id"],
                        Name = reader.GetString(1),
                        KCal = (float)reader.GetDouble(2),
                        Protein = (float)reader.GetDouble(3),
                        Fiber = (float)reader.GetDouble(4),
                        Calcium = (float)reader.GetDouble(5),
                        Sodium = (float)reader.GetDouble(6),
                        PortionSize = (float)reader.GetDouble(7),
                        //dit kan ik gebruiken om enum weightunit te pakken?
                        // Mitch - Zou het via een inner join doen al direct uit db
                        /*
                         * SELECT *.f, description.w
                         * FROM Food as f, WeightUnit? as w
                         * WHERE f.weight_unit_id = w.id
                         * 
                         * en dan food.WeightUnitDescription = reader[description];
                         */
                        //TODO
                        WeightUnitId = (int)reader["weight_unit_id"]
                    };
                    meals.Add(meal);
                }
                conn.Close();
            }
            return meals;
        }
    }
}
