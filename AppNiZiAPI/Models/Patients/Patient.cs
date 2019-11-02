using System;
using System.Collections.Generic;
using System.Text;

namespace AppNiZiAPI.Models
{
    public struct Patient
    {
        public int PatientId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public float WeightInKilograms { get; set; }
        public string Guid { get; set; }
    }

    public struct PatientReturnModel
    {
        public string Guid { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public float WeightInKilograms { get; set; }
    }
}


