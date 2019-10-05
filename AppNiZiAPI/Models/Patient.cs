using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AppNiZiAPI.Models { 
    /// <summary>
    /// Summary description for Patient
    /// </summary>
    [DataContract]
    class Patient
    {
        /// <summary>
        /// Gets or Sets PatientId
        /// </summary>
        [DataMember(Name = "patientId")]
        public int? PatientId { get; set; }

        /// <summary>
        /// Gets or Sets DoctorId
        /// </summary>
        [DataMember(Name = "doctorId")]
        public int? DoctorId { get; set; }

        /// <summary>
        /// Gets or Sets Birthdate
        /// </summary>
        [DataMember(Name = "birthdate")]
        public string Birthdate { get; set; }

        /// <summary>
        /// Gets or Sets Weight
        /// </summary>
        [DataMember(Name = "weight")]
        public List<Weight> Weight { get; set; }

        /// <summary>
        /// Gets or Sets Account
        /// </summary>
        [DataMember(Name = "account")]
        public Account Account { get; set; }
    }
}