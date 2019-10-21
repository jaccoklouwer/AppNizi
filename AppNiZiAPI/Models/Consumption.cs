using System;
using System.Collections.Generic;
using System.Text;

namespace AppNiZiAPI.Models
{
    public class Consumption : BaseEntity
    {
        public string FoodName { get; set; }
        public float KCal { get; set; }
        public float Protein { get; set; }
        public float Fiber { get; set; }
        public float Calcium { get; set; }
        public float Sodium { get; set; }
        public int Amount { get; set; }
        public float WeightUnitId { get; set; }
        public DateTime Date { get; set; }
        public int PatientId { get; set; }
    }
}
