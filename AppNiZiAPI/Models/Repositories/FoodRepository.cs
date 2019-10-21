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
        const string cheatConnectionString = "geheim voor outsiders"
        //change to Id
        public Food Select(string foodname)
        {
            Food food = new Food();

            SqlConnection conn = new SqlConnection();

            if (Environment.GetEnvironmentVariable("sqldb_connection") == null)
            {
                
                conn.ConnectionString = cheatConnectionString;
            }
            else
            {
                conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection"));
            }


            //new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection")
            using (conn)
            {

                conn.Open();
                var text = $"SELECT * FROM Food WHERE name = '{foodname}'";

                using (SqlCommand cmd = new SqlCommand(text, conn))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        // TODO OOOH doe dit anders indexes zo gevaarlijk
                        food.FoodId = (int)reader["id"];
                        food.Name = reader.GetString(1);
                        food.KCal = (float)reader.GetDouble(2);
                        food.Protein = (float)reader.GetDouble(3);
                        food.Fiber = (float)reader.GetDouble(4);
                        food.Calcium = (float)reader.GetDouble(5);
                        food.Sodium = (float)reader.GetDouble(6);
                        food.PortionSize = (float)reader.GetDouble(7);
                        //dit kan ik gebruiken om enum weightunit te pakken?
                        // Mitch - Zou het via een inner join doen al direct uit db
                        /*
                         * SELECT *.f, description.w
                         * FROM Food as f, WeightUnit? as w
                         * WHERE f.weight_unit_id = w.id
                         * 
                         * en dan food.WeightUnitDescription = reader[description];
                         */
                        //TODO dit zo doen weightunitid(int) moet een weighunit(string) worden
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

            SqlConnection conn = new SqlConnection();

            if (Environment.GetEnvironmentVariable("sqldb_connection") == null)
            {

                conn.ConnectionString = cheatConnectionString;
            }
            else
            {
                conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection"));
            }

            //Todo controleer op lengte (moet minstens 2 zijn)
            using (conn)
            {

                conn.Open();
                var text = $"SELECT * FROM Food Where name LIKE  '{foodname}%'";

                using (SqlCommand cmd = new SqlCommand(text, conn))
                {
                    //Todo limit dit met een count parameter anders gaan we straks 200 ap*** ophalen)
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Food food = new Food
                        {
                            // Uit lezen bijv
                            FoodId = (int)reader["id"],
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

            SqlConnection conn = new SqlConnection();

            if (Environment.GetEnvironmentVariable("sqldb_connection") == null)
            {

                conn.ConnectionString = cheatConnectionString;
            }
            else
            {
                conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection"));
            }

            using (conn)
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
                            FoodId = (int)reader["id"],
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

            SqlConnection conn = new SqlConnection();

            if (Environment.GetEnvironmentVariable("sqldb_connection") == null)
            {
                conn.ConnectionString = cheatConnectionString;
            }
            else
            {
                conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection"));
            }

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
    }
}
