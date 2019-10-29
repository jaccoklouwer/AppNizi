using System;
using System.Collections.Generic;

namespace AppNiZiAPI.Models.Repositories
{
    interface IConsumptionRepository
    {
        Consumption GetConsumptionByConsumptionId(int consumptionId);
        List<ConsumptionView> GetConsumptionsForPatientBetweenDates(int patientId, DateTime startDate, DateTime endDate);

        int GetConsumptionPatientId(int consumptionId);

        bool AddConsumption(ConsumptionView consumption, int patientId);

        bool DeleteConsumption(int consumptionId, int patientId);

        bool UpdateConsumption(int consumptionId, ConsumptionView consumption, int patientId);

    }
}
