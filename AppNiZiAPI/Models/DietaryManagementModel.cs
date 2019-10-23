using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AppNiZiAPI.Models
{
    public class DietaryManagementModel
    {
        /// <summary>
        /// Diet Id
        /// </summary>
        [Required]
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Diet Description
        /// </summary>
        [Required]
        [JsonProperty("Description")]
        public string Description { get; set; }

        /// <summary>
        /// Diet Amount
        /// </summary>
        [Required]
        [JsonProperty("Amount")]
        public int Amount { get; set; }

        /// <summary>
        /// Diet status
        /// </summary>
        [Required]
        [JsonProperty("IsActive")]
        public bool IsActive { get; set; }

        /// <summary>
        /// Diet Patient
        /// </summary>
        [Required]
        [JsonProperty("Patient")]
        public int PatientId { get; set; }
    }
}
