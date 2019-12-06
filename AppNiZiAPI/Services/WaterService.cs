using AppNiZiAPI.Infrastructure;
using AppNiZiAPI.Models;
using AppNiZiAPI.Models.Handlers;
using AppNiZiAPI.Models.Repositories;
using AppNiZiAPI.Security;
using AppNiZiAPI.Services.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AppNiZiAPI.Services
{
    public interface IWaterService
    {
        Task<IActionResult> GetWaterById(HttpRequest req, int waterId);
        Dictionary<ServiceDictionaryKey, object> TryDeleteWaterConsumption(int waterId, int patientId);

    }

    class WaterService : IWaterService
    {
        public IMessageHandler FeedbackHandler { get; }

        private readonly IAuthorization _authorizationService;
        private readonly IWaterRepository _waterRepository;

        public WaterService(IAuthorization authorization, IWaterRepository waterRepository, IMessageHandler messageHandler)
        {
            this._authorizationService = authorization;
            this._waterRepository = waterRepository;
            this.FeedbackHandler = messageHandler;
        }

        public async Task<IActionResult> GetWaterById(HttpRequest req, int waterId)
        {
            int patientId = await _authorizationService.GetUserId(req);
            if (patientId == 0)
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);

            WaterConsumptionModel waterConsumptionModel = await _waterRepository.GetSingleWaterConsumption(patientId, waterId);
            return waterConsumptionModel != null
                ? (ActionResult)new OkObjectResult(waterConsumptionModel)
                : new StatusCodeResult(StatusCodes.Status204NoContent);
        }

        public Dictionary<ServiceDictionaryKey, object> TryDeleteWaterConsumption(int waterId, int patientId)
        {
            Dictionary<ServiceDictionaryKey, object> dictionary = new Dictionary<ServiceDictionaryKey, object>();

            try
            {
                if (_waterRepository.RemoveWaterConsumptions(patientId, waterId))
                {
                    dictionary.Add(ServiceDictionaryKey.VALUE, $"Water Consumption with id = {waterId} deleted");
                    dictionary.Add(ServiceDictionaryKey.HTTPSTATUSCODE, HttpStatusCode.OK);
                }
                else
                {
                    dictionary.Add(ServiceDictionaryKey.ERROR, "No rows affected. Does the water input exist?");
                    dictionary.Add(ServiceDictionaryKey.HTTPSTATUSCODE, HttpStatusCode.NotFound);
                }
            }
            catch (Exception ex)
            {
                dictionary.AddErrorMessage(ServiceDictionaryKey.ERROR, ex, FeedbackHandler);
            }
            return dictionary;
        }
    }
}
