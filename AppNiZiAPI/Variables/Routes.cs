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
        public const string Patients = "/patients";
        public const string Me = "/me";
        public const string PatientId = "/{patientGuid}";

        public const string SpecificPatient = "/patients/{patientId:int}";

        // Doctors
        public const string GetDoctorPatients = "/doctor/patients";

        // Consumption
        public const string Consumption = "/consumption/{consumptionId}";
        public const string Consumptions = "/consumptions";

        // Water Consumption
        public const string PostWaterConsumption = "/waterConsumption";
        public const string GetDailyWaterConsumption = "/waterconsumption/daily/{date}";
        public const string GetWaterConsumptionPeriod = "/waterconsumption/period";
        public const string SingleWaterConsumption = "/waterconsumptions/{waterId:int}"; // GET, DELETE

        // DietaryManagement
        public const string DietaryManagement = "/dietaryManagement";
        public const string GetDietaryManagement = "/dietaryManagement/{patientId}";
        public const string DietaryManagementById = "/dietaryManagement/{dietId}";

        //Food
        public const string FoodById = "/food/{patientId:int}/{foodId:int}";
        public const string FoodByPartialname = "/food/partial/{patientId:int}/{foodName}";
        //TODO verbeter deze fantastische naamgeving 

        public const string GetFavoriteFood = "/food/favorite/{patientId:int}";
        public const string PostFavoriteFood = "/food/favorite";

        //Meal
        public const string AddMeal = "/meal/{patientId:int}";
        public const string DeleteMeal = "/meal/{patientId:int}/{mealId}";
        public const string GetMeals = "/meal/{patientId:int}";

        //Account
        public const string RegisterPatient = "/patients/register";
        public const string Account = "/account";


    }
}
