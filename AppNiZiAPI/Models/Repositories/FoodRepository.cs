using AppNiZiAPI.Variables;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace AppNiZiAPI.Models.Repositories
{
    class FoodRepository
    {
        //change to Id
        public Food Select(string foodname)
        {
            Food food = new Food();


            using (SqlConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {

                conn.Open();
                var text = $"SELECT * FROM Food WHERE name=" + foodname;

                using (SqlCommand cmd = new SqlCommand(text, conn))
                {
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            // Uit lezen bijv
                            food.KCal = (float)reader["kcal"];
                            food.Protein = (float)reader["protein"];
                            food.Fiber = (float)reader["fiber"];
                            food.Calcium = (float)reader["calcium"];
                            food.Sodium = (float)reader["sodium"];
                            food.PortionSize = (float)reader["portion_size"];
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
                            food.WeightUnitId = (int)reader["weight_unit_id"];
                        }
                }
                conn.Close();
            }
            return food;
        }
        public List<Food> Search(string foodname)
        {
            
            List<Food> foods = new List<Food>();

            //Todo controleer op lengte (moet minstens 2 zijn)
            using (SqlConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {

                conn.Open();
                var text = $"SELECT * FROM Food Where name LIKE " + foodname +"%";

                using (SqlCommand cmd = new SqlCommand(text, conn))
                {
                    //Todo limit dit met een count parameter anders gaan we straks 200 ap*** ophalen)
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Food food = new Food
                        {
                            // Uit lezen bijv
                            KCal = (float)reader["kcal"],
                            Protein = (float)reader["protein"],
                            Fiber = (float)reader["fiber"],
                            Calcium = (float)reader["calcium"],
                            Sodium = (float)reader["sodium"],
                            PortionSize = (float)reader["portion_size"],
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
                        foods.Add(food);
                    }
                }
                conn.Close();
            }
            return foods;
        }
        public List<Food> Favorites(int id)
        {
            List<Food> foods = new List<Food>();
            using (SqlConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
            {

                conn.Open();
                var text = $"Select* from Food Inner Join MyFood On Food.id = MyFood.food_id where patient_id ="+ id;

                using (SqlCommand cmd = new SqlCommand(text, conn))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Food food = new Food
                        {
                            // Uit lezen bijv
                            KCal = (float)reader["kcal"],
                            Protein = (float)reader["protein"],
                            Fiber = (float)reader["fiber"],
                            Calcium = (float)reader["calcium"],
                            Sodium = (float)reader["sodium"],
                            PortionSize = (float)reader["portion_size"],
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
            //TODO beetje overdreven constructie "geleend" van create patient 
            //TODO patient_id kan uit ingelogde user komen
            StringBuilder sqlQuery = new StringBuilder();
            sqlQuery.Append("INSERT INTO MyFood (food_id, patient_id) ");
            sqlQuery.Append("VALUES (@FOODID, @PATIENTID) ");

            try
            {
                using (SqlConnection sqlConn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")))
                {
                    sqlConn.Open();

                    SqlCommand sqlCmd = new SqlCommand(sqlQuery.ToString(), sqlConn);
                    sqlCmd.Parameters.Add("@FOODID", SqlDbType.Int).Value = patient_id;
                    sqlCmd.Parameters.Add("@PATIENTID", SqlDbType.Int).Value = food_id;
                    //TODO TEST DIT
                    sqlCmd.ExecuteNonQuery();
                }
            }
            catch
            {
                succes = false;
            }

            return succes;
        }
    }
}
