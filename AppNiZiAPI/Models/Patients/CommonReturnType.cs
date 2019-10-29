using System;
using System.Collections.Generic;
using System.Text;

namespace AppNiZiAPI.Models
{
    public class PatientObject
    {
        public int PatientId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public float WeightInKilograms { get; set; }
        public string Guid { get; set; }
    }

}


