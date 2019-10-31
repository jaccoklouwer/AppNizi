using System;
using System.Collections.Generic;
using System.Text;

namespace AppNiZiAPI.Models.AccountModels
{
    public class DoctorLogin
    {
        public AccountModel Account { get; set; }
        public DoctorModel Doctor { get; set; }
        public AuthLogin Auth { get; set; }
    }
}
