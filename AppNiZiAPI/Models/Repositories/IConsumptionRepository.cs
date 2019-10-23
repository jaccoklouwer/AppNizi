using System;
using System.Collections.Generic;
using System.Text;

namespace AppNiZiAPI.Models.Repositories
{
    interface IConsumptionRepository
    {
        Consumption GetConsumptionByConsumptionId(int consumptionId);
        List<Consumption> GetConsumptionsForPatientBetweenDates(int patientId, DateTime startDate, DateTime endDate);

        void AddConsumption(Consumption consumption);

        void DeleteConsumption(int consumptionId);

        void UpdateConsumption(Consumption consumption);

    }
}
