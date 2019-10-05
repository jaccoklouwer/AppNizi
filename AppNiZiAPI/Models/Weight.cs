using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace AppNiZiAPI.Models
{
    class Weight
    {
        /// <summary>
        /// Gets or Sets Amount
        /// </summary>
        [Required]
        [DataMember(Name = "amount")]
        public string Amount { get; set; }

        /// <summary>
        /// Gets or Sets Unit
        /// </summary>
        [Required]
        [DataMember(Name = "unit")]
        public string Unit { get; set; }

    }
}
