using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace AppNiZiAPI.Models
{
    class Doctor
    {
        /// <summary>
        /// Gets or Sets Account
        /// </summary>
        [DataMember(Name = "account")]
        public Account Account { get; set; }

        /// <summary>
        /// Gets or Sets DoctorId
        /// </summary>
        [Required]
        [DataMember(Name = "doctor_id")]
        public int? DoctorId { get; set; }

        /// <summary>
        /// Gets or Sets Birthdate
        /// </summary>
        [DataMember(Name = "birthdate")]
        public int? Birthdate { get; set; }
    }
}
