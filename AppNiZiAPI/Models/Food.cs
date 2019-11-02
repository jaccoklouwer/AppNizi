using System;
using System.Collections.Generic;
using System.Text;

namespace AppNiZiAPI.Models
{
    public class Food
    {
        public int FoodId { get; set; }
        public string Name { get; set; }
        public double KCal { get; set; }
        public double Protein { get; set; }
        public double Fiber { get; set; }
        public double Calcium { get; set; }
        public double Sodium { get; set; }
        public double PortionSize { get; set; }
        public string WeightUnit { get; set; }
        public string Picture { get; set; }

    }
}
