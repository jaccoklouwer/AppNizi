using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AppNiZiAPI
{
    public class ConsumptionInput
    {
        [Required]
        [JsonProperty("FoodName")]
        public string FoodName { get; set; }

        [JsonProperty("KCal")]
        public float KCal { get; set; }

        [JsonProperty("Protein")]
        public float Protein { get; set; }

        [JsonProperty("Fiber")]
        public float Fiber { get; set; }

        [JsonProperty("Calium")]
        public float Calium { get; set; }

        [JsonProperty("Sodium")]
        public float Sodium { get; set; }

        [Required]
        [JsonProperty("Amount")]
        public int Amount { get; set; }

        [Required]
        [JsonProperty("WeightUnitId")]
        public float WeightUnitId { get; set; }

        [Required]
        [JsonProperty("Date")]
        public DateTime Date { get; set; }

        [Required]
        [JsonProperty("PatientId")]
        public int PatientId { get; set; }
    }
}
