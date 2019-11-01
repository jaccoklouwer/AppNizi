using System;
using System.Collections.Generic;
using System.Text;

namespace AppNiZiAPI.Models.SwaggerModels
{
    class SwaggerRegisterPatient
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string dateOfBirth { get; set; }
        public float weight { get; set; }
        public int doctorId { get; set; }
    }
}
