using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AppNiZiAPI.Models
{
    class SwaggerIsDoctor
    {
        [Required]
        public int PatientId { get; set; }
        public string Role { get; set; }
    }
}
