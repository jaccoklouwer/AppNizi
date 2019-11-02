using System;
using System.Collections.Generic;
using System.Text;

namespace AppNiZiAPI
{
    public class PatientConsumptionsView
    {
        public List<PatientConsumptionView> Consumptions { get; }
        public float KCalTotal { get; }
        public float ProteinTotal { get; }
        public float FiberTotal { get; }
        public float CaliumTotal { get; }
        public float SodiumTotal { get; }

        public PatientConsumptionsView(List<PatientConsumptionView> consumptions)
        {
            this.Consumptions = consumptions;
            foreach (PatientConsumptionView consumption in consumptions)
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
