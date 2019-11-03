using System;
using System.Collections.Generic;
using System.Text;

namespace AppNiZiAPI.Variables
{
    public static class Routes
    {
        // API info
        public const string APIVersion = "v1";

        //Swagger
        public const string SwaggerUI = "swagger/ui";
        public const string SwaggerJson = "swagger/json";

        // Patients
        public const string Patient = "/patient";
        public const string Patients = "/patients";
        public const string Me = "/me";
        public const string SpecificPatient = "/patient/{patientId}";

        // Doctors
        public const string GetDoctorPatients = "/doctor/{doctorId}/patients";
        public const string SpecificDoctor = "/doctor/{doctorId}"; // GET, DELETE, PUT
        public const string Doctor = "/doctor"; // GET, POST, DELETE
        public const string DoctorMe = "/doctor/me";

        // Consumption
        public const string Consumption = "/consumption/{consumptionId}";
        public const string Consumptions = "/consumptions";

        // Water Consumption
        public const string PostWaterConsumption = "/waterconsumption";

        public const string GetDailyWaterConsumption = "/waterconsumption/daily/{patientId}";
        public const string GetWaterConsumptionPeriod = "/waterconsumption/period/{patientId}";
        public const string SingleWaterConsumption = "/waterconsumption/{waterId}"; // GET, DELETE

        // DietaryManagement
        public const string DietaryManagement = "/dietaryManagement";
        public const string GetDietaryManagement = "/dietaryManagement/{patientId}";
        public const string DietaryManagementById = "/dietaryManagement/{dietId}";

        //Food
        public const string FoodById = "/food/{patientId}/{foodId}";
        public const string FoodByPartialname = "/food/partial/{patientId}/{foodName}/{count}";
        //TODO verbeter deze fantastische naamgeving 

        public const string GetFavoriteFood = "/food/favorite/{patientId}";
        public const string PostFavoriteFood = "/food/favorite";
        public const string UnFavoriteFood = "/food/favorite";

        //Meal
        public const string AddMeal = "/meal/{patientId}";
        public const string DeleteMeal = "/meal/{patientId}/{mealId}";
        public const string GetMeals = "/meal/{patientId}";

        //Account
        public const string RegisterPatient = "/patients/register";
        public const string Account = "/account";


    }
}
