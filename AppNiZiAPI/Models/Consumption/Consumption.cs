using AppNiZiAPI.Models;
using System;

namespace AppNiZiAPI
{
    public class Consumption : BaseEntity
    {
        public string FoodName { get; set; }

        public float KCal { get; set; }

        public float Protein { get; set; }

        public float Fiber { get; set; }

        public float Calium { get; set; }

        public float Sodium { get; set; }

        public int Amount { get; set; }

        public float WeightUnitId { get; set; }

        public DateTime Date { get; set; }

        public int PatientId { get; set; }

    }
}
