using AppNiZiAPI.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppNiZiAPI
{
    public class PatientConsumptionView
    {
        public int ConsumptionId { get; set; }
        public string FoodName { get; set; }
        public float KCal { get; set; }
        public float Protein { get; set; }
        public float Fiber { get; set; }
        public float Calium { get; set; }
        public float Sodium { get; set; }
        public int Amount { get; set; }
        public WeightUnitModel Weight { get; set; }
        public DateTime Date { get; set; }

    }
}
