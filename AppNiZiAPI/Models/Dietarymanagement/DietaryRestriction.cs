using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AppNiZiAPI.Models.Dietarymanagement
{
    public class DietaryRestriction
    {
        /// <summary>
        /// Restriction Id
        /// </summary>
        [Required]
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Restriction Description
        /// </summary>
        [Required]
        [JsonProperty("Description")]
        public string Description { get; set; }
    }
}
