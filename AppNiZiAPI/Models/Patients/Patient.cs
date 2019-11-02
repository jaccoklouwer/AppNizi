using System;
using System.Collections.Generic;
using System.Text;

namespace AppNiZiAPI.Models
{
    public struct Patient
    {
        public Patient(int patientId, string firstName, string lastName, DateTime dateOfBirth, float weightInKilograms, string guid)
        {
            PatientId = patientId;
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            WeightInKilograms = weightInKilograms;
            Guid = guid;
        }

        public int PatientId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public float WeightInKilograms { get; set; }
        public string Guid { get; set; }
    }

    public struct PatientReturnModel
    {
        public PatientReturnModel(int id, string firstName, string lastName, DateTime dateOfBirth, float weightInKilograms)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            WeightInKilograms = weightInKilograms;
        }

        public PatientReturnModel(Patient patient)
        {
            Id = patient.PatientId;
            FirstName = patient.FirstName;
            LastName = patient.LastName;
            DateOfBirth = patient.DateOfBirth;
            WeightInKilograms = patient.WeightInKilograms;
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public float WeightInKilograms { get; set; }
    }
}


