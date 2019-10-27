using System;
using System.Collections.Generic;
using System.Text;

namespace AppNiZiAPI.Models.Water
{
    class WaterConsumptionDaily
    {
        public double Total { get; set; }
        public int MinimumRestriction { get; set; }
        public List<WaterConsumptionViewModel> WaterConsumptions { get; set; }
    }
}
