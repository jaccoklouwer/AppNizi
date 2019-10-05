using System;
using System.Collections.Generic;
using System.Text;

namespace AppNiZiAPI.Models
{
    class Patient : Account
    {
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime Birthday { get; set; }
        // TODO: public Weight weight { get; set; }

    }
}
