using AppNiZiAPI.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppNiZiAPI
{
    public class ConsumptionView
    {
        public int ConsumptionId { get; }
        public string FoodName { get; }
        public float KCal { get; }
        public float Protein { get; }
        public float Fiber { get; }
        public float Calium { get; }
        public float Sodium { get; }
        public int Amount { get; }
        public float WeightUnitId { get; }
        public DateTime Date { get; }
        public bool Valid { get; }

        public ConsumptionView(Consumption consumption)
        {
            this.ConsumptionId = consumption.Id;
            this.FoodName = consumption.FoodName;
            this.KCal = consumption.KCal;
            this.Protein = consumption.Protein;
            this.Fiber = consumption.Fiber;
            this.Calium = consumption.Calium;
            this.Sodium = consumption.Sodium;
            this.Amount = consumption.Amount;
            this.WeightUnitId = consumption.WeightUnitId;
            this.Date = consumption.Date;
            this.Valid = true;
        }

        public ConsumptionView()
        {
            this.Valid = false;
        }
    }
}
