using System;
using System.Collections.Generic;
using System.Text;

namespace AppNiZiAPI.Models
{
    // Might use this for food weight measurements too?
    class Weight
    {
        public Weight(float amount, WeightUnit unit)
        {
            this.amount = amount;
            this.unit = unit;
        }

        public float amount { get; set; }
        public WeightUnit unit { get; set; }
        
    }
}
