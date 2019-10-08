﻿using System;
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
        public const string Me = "/patients/me";
        public const string SpecificPatient = "/patients/{patientId: int}";

        // Doctors
        public const string GetDoctorPatients = "/doctor/patients";

        // Water Consumption
        public const string PostWaterConsumption = "/waterConsumption";
        public const string GetWaterConsumption = "/waterconsumption/{patientId: int}";
    }
}