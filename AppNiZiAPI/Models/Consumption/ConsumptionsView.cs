using System;
using System.Collections.Generic;
using System.Text;

namespace AppNiZiAPI
{
    public class ConsumptionsView
    {
        public List<ConsumptionView> Consumptions { get; }
        public float KCalTotal { get; }
        public float ProteinTotal { get; }
        public float FiberTotal { get; }
        public float CaliumTotal { get; }
        public float SodiumTotal { get; }

        public ConsumptionsView(List<ConsumptionView> consumptions)
        {
            this.Consumptions = consumptions;
            foreach (ConsumptionView consumption in consumptions)
            {
                this.KCalTotal += consumption.KCal;
                this.ProteinTotal += consumption.Protein;
                this.FiberTotal += consumption.Fiber;
                this.CaliumTotal += consumption.Calium;
                this.SodiumTotal += consumption.Sodium;
            }
        }
    }
}
