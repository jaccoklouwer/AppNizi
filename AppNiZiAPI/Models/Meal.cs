﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AppNiZiAPI.Models
{
    class Meal
    {
        public int MealId { get; set; }
        public string Name { get; set; }
        public int PatientId { get; set; }
        public float KCal { get; set; }
        public float Protein { get; set; }
        public float Fiber { get; set; }
        public float Calcium { get; set; }
        public float Sodium { get; set; }
        public float PortionSize { get; set; }
        public float WeightUnitId { get; set; }

    }
}