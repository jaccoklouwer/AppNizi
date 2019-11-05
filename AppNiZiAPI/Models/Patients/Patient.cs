using System;
using System.Collections.Generic;
using System.Text;

namespace AppNiZiAPI.Models
{
    public class Patient
    {
        public Patient() { }

        public Patient(int patientId, string firstName, string lastName, DateTime dateOfBirth, float weightInKilograms, string guid,
            int accountId, int doctorId)
        {
            PatientId = patientId;
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            WeightInKilograms = weightInKilograms;
            Guid = guid;
            AccountId = accountId;
            DoctorId = doctorId;
        }

        public int PatientId { get; set; }
        public int AccountId { get; set; }
        public int DoctorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public float WeightInKilograms { get; set; }
        public string Guid { get; set; }
    }

    public struct PatientReturnModel
    {
        public PatientReturnModel(int id, string firstName, string lastName, DateTime dateOfBirth, float weightInKilograms, int doctorId)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            WeightInKilograms = weightInKilograms;
            HandlingDoctorId = doctorId;
        }

        public PatientReturnModel(Patient patient)
        {
            Id = patient.PatientId;
            FirstName = patient.FirstName;
            LastName = patient.LastName;
            DateOfBirth = patient.DateOfBirth;
            WeightInKilograms = patient.WeightInKilograms;
            HandlingDoctorId = patient.DoctorId;
        }

        public int Id { get; set; }
        public int HandlingDoctorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public float WeightInKilograms { get; set; }
    }

    public class PatientUpdateModel
    {
        public PatientUpdateModel(int patientId, DateTime dateOfBirth, float weightInKilograms, int doctorId)
        {
            PatientId = patientId;
            DateOfBirth = dateOfBirth;
            WeightInKilograms = weightInKilograms;
            DoctorId = doctorId;
        }

        public int PatientId { get; set; }
        public DateTime DateOfBirth { get; set; }
        public float WeightInKilograms { get; set; }
        public int DoctorId { get; set; }
    }
}


