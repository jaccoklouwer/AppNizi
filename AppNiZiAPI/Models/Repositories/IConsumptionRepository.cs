using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppNiZiAPI.Models.Repositories
{
    interface IConsumptionRepository
    {
        Task<ConsumptionView> GetConsumptionByConsumptionId(int consumptionId);
        Task<List<PatientConsumptionView>> GetConsumptionsForPatientBetweenDates(int patientId, DateTime startDate, DateTime endDate);

        Task<bool> AddConsumption(ConsumptionInput consumption);

        Task<bool> DeleteConsumption(int consumptionId, int patientId);

        Task<bool> UpdateConsumption(int consumptionId, ConsumptionInput consumption);

    }
}
