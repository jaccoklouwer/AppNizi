using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AppNiZiAPI.Models
{
    class Doctor : Account
    {
        /// <summary>
        /// Gets or Sets DoctorId
        /// </summary>
        [Required]
        public int? DoctorId { get; set; }

        /// <summary>
        /// Gets or Sets Birthdate
        /// </summary>
        public DateTime Birthday { get; set; }
    }
}
