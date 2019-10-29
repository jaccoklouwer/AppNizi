using System;
using System.Collections.Generic;
using System.Text;

namespace AppNiZiAPI.Models.AccountModels
{
    public class PatientLogin
    {
        public AccountModel Account { get; set; }
        public PatientObject Patient { get; set; }
        public Doctor Doctor { get; set; }
        public AuthLogin AuthLogin { get; set; }
    }
}
