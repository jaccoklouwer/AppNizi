using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AppNiZiAPI.Services
{
    interface IConsumptionService
    {
        Task<ActionResult> AddConsumption(HttpRequest req);

        Task<ActionResult> UpdateConsumption(HttpRequest req, string consumptionId);

        Task<ActionResult> RemoveConsumption(HttpRequest req, string consumptionId);

        Task<ActionResult> GetConsumptionByConsumptionId(HttpRequest req, string consumptionId);

        Task<ActionResult> GetConsumptionsForPatientBetweenDates(HttpRequest req);
    }
}
