using AppNiZiAPI.Variables;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace AppNiZiAPI.Models.Repositories
{
    class FoodRepository: IFoodRepository
    {
        //change to Id
        public Food Select(int foodId)
        {
            Food food = new Food();
            SqlConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection"));
            using (conn)
            {

                conn.Open();
                var text = $"SELECT Food.*, WeightUnit.unit from Food Inner Join WeightUnit On  food.weight_unit_id = WeightUnit.id where food.id = '{foodId}'";

                using (SqlCommand cmd = new SqlCommand(text, conn))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        // TODO OOOH doe dit anders indexes zo gevaarlijk
                        food.FoodId = (int)reader["id"];
                        //food.Name = reader.GetString(1);
                        food.Name = (string)reader["name"];
                        food.KCal = (double)reader["kcal"];
                        food.Protein = (double)reader["protein"];
                        food.Fiber = (double)reader["fiber"];
                        food.Calcium = (double)reader["calcium"];
                        food.Sodium = (double)reader["sodium"];
                        food.PortionSize = (double)reader["portion_size"];
                        food.Picture = (string)reader["picture"];
                        food.WeightUnit = (string)reader["unit"];
                    }
                }
                conn.Close();
            }
            return food;
        }
        public List<Food> Search(string foodname,int count)
        {
            
            List<Food> foods = new List<Food>();
            SqlConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection"));
            using (conn)
            {

                conn.Open();
                var text = $"SELECT TOP {count} * FROM Food Inner Join WeightUnit On  food.weight_unit_id = WeightUnit.id Where name LIKE '{foodname}%'";
                SqlCommand sqlCmd = new SqlCommand(text, conn);
                //sqlCmd.Parameters.Add("@COUNT", SqlDbType.Int).Value = count;
                using (sqlCmd)
                {
                    //Todo limit dit met een count parameter anders gaan we straks 200 ap*** ophalen)
                    //Done
                    SqlDataReader reader = sqlCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Food food = new Food
                        {
                            // Uit lezen bijv
                            FoodId = (int)reader["id"],
                            Name = (string)reader["name"],
                            KCal = (double)reader["kcal"],
                            Protein = (double)reader["protein"],
                            Fiber = (double)reader["fiber"],
                            Calcium = (double)reader["calcium"],
                            Sodium = (double)reader["sodium"],
                            PortionSize = (double)reader["portion_size"],
                            Picture = (string)reader["picture"],
                            WeightUnit = (string)reader["unit"]
                        };
                        foods.Add(food);
                    }
                }
                conn.Close();
            }
            return foods;
        }
        public List<Food> Favorites(int patientId)
        {
            List<Food> foods = new List<Food>();

            SqlConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection"));
            using (conn)
            {

                conn.Open();
                var text = $"Select* from Food Inner Join MyFood On Food.id = MyFood.food_id where patient_id ="+ patientId;

                using (SqlCommand cmd = new SqlCommand(text, conn))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Food food = new Food
                        {
                            FoodId = (int)reader["id"],
                            Name = (string)reader["name"],
                            KCal = (double)reader["kcal"],
                            Protein = (double)reader["protein"],
                            Fiber = (double)reader["fiber"],
                            Calcium = (double)reader["calcium"],
                            Sodium = (double)reader["sodium"],
                            PortionSize = (double)reader["portion_size"],
                            Picture = (string)reader["picture"],
                            WeightUnit = (string)reader["unit"]
                        };
                        foods.Add(food);
                    }
                }
                conn.Close();
            }

            return foods;
        }
        public bool Favorite(int patient_id,int food_id)
        {
            bool succes = true;
            SqlConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection"));
            //TODO patient_id kan uit ingelogde user komen
            StringBuilder sqlQuery = new StringBuilder();
            sqlQuery.Append("INSERT INTO MyFood (food_id, patient_id) ");
            sqlQuery.Append("VALUES (@FOODID, @PATIENTID) ");
            // ff als een loser doen
            // als een loser werkt het wel :)
            // TODO doe dit als een winner
            SqlParameter param1 = new SqlParameter();
            param1.ParameterName = "@FOODID";
            param1.Value = food_id;

            SqlParameter param2 = new SqlParameter();
            param2.ParameterName = "@PATIENTID";
            param2.Value = patient_id;
            try
            {
                using (conn)
                {
                    conn.Open();

                    SqlCommand sqlCmd = new SqlCommand(sqlQuery.ToString(), conn);
                    sqlCmd.Parameters.Add(param1);
                    sqlCmd.Parameters.Add(param2);
                    //TODO TEST DIT
                    sqlCmd.ExecuteNonQuery();
                }
                conn.Close();
            }
            catch
            {
                succes = false;
            }

            return succes;
        }

        public bool UnFavorite(int patient_id, int food_id)
        {
            SqlConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection"));
            bool success = false;
            string sqlQuery = "DELETE FROM MyFood WHERE patient_id=@PATIENT_ID AND food_id=@FOOD_ID";

            using (conn)
            {
                conn.Open();

                SqlCommand sqlCmd = new SqlCommand(sqlQuery, conn);
                sqlCmd.Parameters.Add("@PATIENT_ID", SqlDbType.Int).Value = patient_id;
                sqlCmd.Parameters.Add("@FOOD_ID", SqlDbType.Int).Value = food_id;
                int rows = sqlCmd.ExecuteNonQuery();

                if (rows != 0)
                    success = true;
            }
            return success;
        }
    }
}
