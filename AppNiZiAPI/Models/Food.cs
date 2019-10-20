using System;
using System.Collections.Generic;
using System.Text;

namespace AppNiZiAPI.Models
{
    public class Food
    {
        public int FoodId { get; set; }
        public string Name { get; set; }
        public float KCal { get; set; }
        public float Protein { get; set; }
        public float Fiber { get; set; }
        public float Calcium { get; set; }
        public float Sodium { get; set; }
        // determines the size of the above data (e.g 100grams of banana contains 5g protein)
        public float PortionSize { get; set; }
        public float WeightUnitId { get; set; }

    }
}
