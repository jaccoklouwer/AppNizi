using System;
using System.Collections.Generic;
using System.Text;

namespace AppNiZiAPI.Variables
{
    public static class Routes
    {
        // API info
        public const string APIVersion = "v1";

        // Patients
        public const string Patients = "/patients";
        public const string Me = "/me";
        public const string PatientId = "/{patientGuid}";

        public const string SpecificPatient = "/patients/{patientId: int}";

        // Doctors
        public const string GetDoctorPatients = "/doctor/patients";

        // Water Consumption
        public const string PostWaterConsumption = "/waterConsumption";
        public const string GetWaterConsumption = "/waterconsumption/{patientId: int}";

        // DietaryManagement
        public const string DietaryManagement = "/dietaryManagement";

        //Food
        public const string FoodByName = "/food/{foodName: string}";
        public const string FoodByPartialname = "/food/{foodName: string}";
        //TODO verbeter deze fantastische naamgeving 
        public const string FavoriteFoods = "/food/{id:int}";
        public const string FavoriteFood = "/food/{foodId: int}{patient_Id:int}";
    }
}
