using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AppNiZiAPI.Models.Dietarymanagement
{
    public class DietaryView
    {
        /// <summary>
        /// Restrictions
        /// </summary>
        [Required]
        [JsonProperty("Restrictions")]
        public List<DietaryRestriction> restrictions { get; set; }

        /// <summary>
        /// Restrictions
        /// </summary>
        [Required]
        [JsonProperty("Dietarymanagement")]
        public List<DietaryManagementModel> DietaryManagements { get; set; }
    }
}
