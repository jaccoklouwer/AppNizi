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

        public const string SpecificPatient = "/patients/{patientId: int}";

        // Doctors
        public const string GetDoctorPatients = "/doctor/patients";

        // Consumption
        public const string GetConsumptionById = "/consumption/{consumptionId}";

        // Water Consumption
        public const string PostWaterConsumption = "/waterConsumption";
        public const string GetWaterConsumption = "/waterconsumption/{patientId: int}";

        // DietaryManagement
        public const string DietaryManagement = "/dietaryManagement";
        public const string GetDietaryManagement = "/dietaryManagement/{patientId}";
        public const string DietaryManagementById = "/dietaryManagement/{dietId}";

        //Food
        public const string FoodByName = "/food/{foodName}";
        public const string FoodByPartialname = "/food/partial/{foodName}";
        //TODO verbeter deze fantastische naamgeving 

        public const string GetFavoriteFood = "/food/favorite/{patientId}";
        public const string PostFavoriteFood = "/test/postfavorite";
    }
}
