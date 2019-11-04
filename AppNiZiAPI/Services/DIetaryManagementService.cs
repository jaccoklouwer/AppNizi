using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AppNiZiAPI.Models.Repositories;
using AppNiZiAPI.Models.Handler;
using AppNiZiAPI.Models.Handlers;
using AppNiZiAPI.Services.Serializer;
using AppNiZiAPI.Models.Dietarymanagement;
using Newtonsoft.Json;
using System.IO;
using AppNiZiAPI.Services.Helpers;
using System.Net;

namespace AppNiZiAPI.Services
{
    public interface IDietaryManagementService
    {
        IMessageHandler FeedbackHandler { get; }

        Task<Dictionary<ServiceDictionaryKey, object>> TryAddDietaryManagement(DietaryManagementModel dietary);
        Task<Dictionary<ServiceDictionaryKey, object>> TryDeleteDietaryManagement(int id);
        Task<Dictionary<ServiceDictionaryKey, object>> TryGetDietaryManagementByPatient(int patientId);
        Task<Dictionary<ServiceDictionaryKey, object>> TryUpdateDietaryManagement(int id, DietaryManagementModel dietary);
    }

    public class DIetaryManagementService : IDietaryManagementService
    {
        public IMessageHandler FeedbackHandler { get; }

        private readonly IDietaryManagementRepository _dietaryManagmentRepository;
        private readonly IMessageSerializer _messageSerializer;
        private readonly IQueryHelper _queryHelper;

        public DIetaryManagementService(IMessageHandler feedbackHandler, IDietaryManagementRepository dietaryManagementRepository, IMessageSerializer messageSerializer, IQueryHelper queryHelper)
        {
            this.FeedbackHandler = feedbackHandler;
            _dietaryManagmentRepository = dietaryManagementRepository;
            _messageSerializer = messageSerializer;
            _queryHelper = queryHelper;
        }

        public async Task<Dictionary<ServiceDictionaryKey, object>> TryAddDietaryManagement(DietaryManagementModel dietary)
        {
            Dictionary<ServiceDictionaryKey, object> dictionary = new Dictionary<ServiceDictionaryKey, object>();

            try
            {
                bool susces = await _dietaryManagmentRepository.AddDietaryManagement(dietary);
                dynamic data = _messageSerializer.Serialize(susces);
                dictionary.Add(ServiceDictionaryKey.VALUE, data);
            }
            catch (Exception ex)
            {
                dictionary.AddErrorMessage(ServiceDictionaryKey.ERROR, ex, FeedbackHandler);
            }
            return dictionary;
        }

        public async Task<Dictionary<ServiceDictionaryKey, object>> TryDeleteDietaryManagement(int id)
        {
            Dictionary<ServiceDictionaryKey, object> dictionary = new Dictionary<ServiceDictionaryKey, object>();

            try
            {
                bool succes = await _dietaryManagmentRepository.DeleteDietaryManagement(id);
                if (!succes)
                {
                    dictionary.Add(ServiceDictionaryKey.ERROR, "No rows affected. Does the meal exist?");
                    dictionary.Add(ServiceDictionaryKey.HTTPSTATUSCODE, HttpStatusCode.NotFound);
                }
                else
                {
                    dictionary.Add(ServiceDictionaryKey.VALUE, "Dietary Management deleted");
                    dictionary.Add(ServiceDictionaryKey.HTTPSTATUSCODE, HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                dictionary.AddErrorMessage(ServiceDictionaryKey.ERROR, ex, FeedbackHandler);
            }
            return dictionary;
        }

        public async Task<Dictionary<ServiceDictionaryKey, object>> TryGetDietaryManagementByPatient(int patientId)
        {
            Dictionary<ServiceDictionaryKey, object> dictionary = new Dictionary<ServiceDictionaryKey, object>();

            try
            {
                List<DietaryManagementModel> dietaryManagement = await _dietaryManagmentRepository.GetDietaryManagementByPatientAsync(patientId);
                List<DietaryRestriction> restrictions = await _dietaryManagmentRepository.GetDietaryRestrictions();
                DietaryView view = new DietaryView();
                view.DietaryManagements = dietaryManagement;
                view.restrictions = restrictions;

                if (dietaryManagement.Count <= 0)
                {
                    dictionary.Add(ServiceDictionaryKey.ERROR, $"No dietary management for the given user: {patientId}.");
                    dictionary.Add(ServiceDictionaryKey.HTTPSTATUSCODE, HttpStatusCode.NotFound);
                    return dictionary;
                }

                if (restrictions.Count <= 0)
                {
                    dictionary.Add(ServiceDictionaryKey.ERROR, $"No dietary restrictions found.");
                    dictionary.Add(ServiceDictionaryKey.HTTPSTATUSCODE, HttpStatusCode.NotFound);
                    return dictionary;
                }

                dynamic data = _messageSerializer.Serialize(view);
                dictionary.Add(ServiceDictionaryKey.VALUE, data);
            }
            catch (Exception ex)
            {
                dictionary.AddErrorMessage(ServiceDictionaryKey.ERROR, ex, FeedbackHandler);
            }

            return dictionary;
        }

      

        public async Task<Dictionary<ServiceDictionaryKey, object>> TryUpdateDietaryManagement(int id, DietaryManagementModel dietary)
        {
            Dictionary<ServiceDictionaryKey, object> dictionary = new Dictionary<ServiceDictionaryKey, object>();

            try
            {

                bool succes = await _dietaryManagmentRepository.UpdateDietaryManagement(id, dietary);
                dynamic data = _messageSerializer.Serialize(succes);
                dictionary.Add(ServiceDictionaryKey.VALUE, data);
            }
            catch (Exception ex)
            {
                dictionary.AddErrorMessage(ServiceDictionaryKey.ERROR, ex, FeedbackHandler);
            }

            return dictionary;
        }
    }
}
