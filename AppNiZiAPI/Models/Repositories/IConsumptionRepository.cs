using System;
using System.Collections.Generic;

namespace AppNiZiAPI.Models.Repositories
{
    interface IConsumptionRepository
    {
        ConsumptionView GetConsumptionByConsumptionId(int consumptionId);
        List<PatientConsumptionView> GetConsumptionsForPatientBetweenDates(int patientId, DateTime startDate, DateTime endDate);

        bool AddConsumption(ConsumptionInput consumption);

        bool DeleteConsumption(int consumptionId, int patientId);

        bool UpdateConsumption(int consumptionId, ConsumptionInput consumption);

    }
}
