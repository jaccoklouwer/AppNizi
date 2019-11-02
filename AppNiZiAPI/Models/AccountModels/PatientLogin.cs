using System;
using System.Collections.Generic;
using System.Text;

namespace AppNiZiAPI.Models.AccountModels
{
    public class PatientLogin
    {
        public AccountModel Account { get; set; }
        public Patient Patient { get; set; }
        public DoctorModel Doctor { get; set; }
        public AuthLogin AuthLogin { get; set; }
    }
}
