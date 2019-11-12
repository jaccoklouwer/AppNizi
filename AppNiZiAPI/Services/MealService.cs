using AppNiZiAPI.Models;
using AppNiZiAPI.Models.Handler;
using AppNiZiAPI.Models.Handlers;
using AppNiZiAPI.Models.Repositories;
using AppNiZiAPI.Services.Helpers;
using AppNiZiAPI.Services.Serializer;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AppNiZiAPI.Services
{
    public interface IMealService
    {
        IMessageHandler FeedbackHandler { get; }

        Task<Dictionary<ServiceDictionaryKey, object>> TryDeleteMeal(HttpRequest request);
        Task<Dictionary<ServiceDictionaryKey, object>> TryGetMeals(int patientId);
        Task<Dictionary<ServiceDictionaryKey, object>> TryAddMeal(int patientId, HttpRequest request);
        Task<Dictionary<ServiceDictionaryKey, object>> TryPutMeal(int patientId, int mealId,HttpRequest request);
    }
    class MealService:IMealService
    {
        public IMessageHandler FeedbackHandler { get; }

        private readonly IMealRepository _mealRepository;
        private readonly IMessageSerializer _messageSerializer;
        private readonly IQueryHelper _queryHelper;

        public MealService(IMealRepository mealRepository,
            IMessageHandler feedbackHandler, IMessageSerializer messageSerializer,
            IQueryHelper queryHelper)
        {
            _mealRepository = mealRepository;
            this.FeedbackHandler = feedbackHandler;
            _messageSerializer = messageSerializer;
            _queryHelper = queryHelper;
        }

        async public Task<Dictionary<ServiceDictionaryKey, object>> TryDeleteMeal(HttpRequest request)
        {
            Dictionary<ServiceDictionaryKey, object> dictionary = new Dictionary<ServiceDictionaryKey, object>();
            int patientId = 0;
            int mealId = 0;
            try
            {
                //TODO haal patient id op een coole manier op
                mealId = Convert.ToInt32(request.Query["mealId"].ToString());
                patientId = Convert.ToInt32(request.Query["patientId"].ToString());
            }
            catch (Exception ex)
            {
                dictionary.AddErrorMessage(ServiceDictionaryKey.ERROR, ex, FeedbackHandler);
            }
            try
            {
                bool succes = await _mealRepository.DeleteMeal(patientId, mealId);
                if (!succes)
                {
                    dictionary.Add(ServiceDictionaryKey.ERROR, "No rows affected. Does the meal exist?");
                    dictionary.Add(ServiceDictionaryKey.HTTPSTATUSCODE, HttpStatusCode.NotFound);
                }
                else
                {
                    dictionary.Add(ServiceDictionaryKey.VALUE, "Meal deleted");
                    dictionary.Add(ServiceDictionaryKey.HTTPSTATUSCODE, HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                dictionary.AddErrorMessage(ServiceDictionaryKey.ERROR, ex, FeedbackHandler);
            }
            return dictionary;
        }

        async public Task<Dictionary<ServiceDictionaryKey, object>> TryGetMeals(int patientId)
        {
            Dictionary<ServiceDictionaryKey, object> dictionary = new Dictionary<ServiceDictionaryKey, object>();

            
            try
            {
                List<Meal> meals = await _mealRepository.GetMyMeals(patientId);

                if (meals.Count <= 0)
                {
                    dictionary.Add(ServiceDictionaryKey.ERROR, $"No meals meals for the given user: {patientId}.");
                    dictionary.Add(ServiceDictionaryKey.HTTPSTATUSCODE, HttpStatusCode.NotFound);
                    return dictionary;
                }

                dynamic data = _messageSerializer.Serialize(meals);
                dictionary.Add(ServiceDictionaryKey.VALUE, data);
            }
            catch (Exception ex)
            {
                dictionary.AddErrorMessage(ServiceDictionaryKey.ERROR, ex, FeedbackHandler);
            }

            return dictionary;
        }

        async public Task<Dictionary<ServiceDictionaryKey, object>> TryAddMeal(int patientId, HttpRequest request)
        {

            Dictionary<ServiceDictionaryKey, object> dictionary = new Dictionary<ServiceDictionaryKey, object>();

            try
            {
                string requestBody = await new StreamReader(request.Body).ReadToEndAsync();

                Meal meal = new Meal();
                JsonConvert.PopulateObject(requestBody, meal);
                meal.PatientId = patientId;
                meal = await _mealRepository.AddMeal(meal);
                dynamic data = _messageSerializer.Serialize(meal);

                dictionary.Add(ServiceDictionaryKey.VALUE, data);
            }
            
                catch (Exception ex)
            {
                dictionary.AddErrorMessage(ServiceDictionaryKey.ERROR, ex, FeedbackHandler);
            }
            return dictionary;
        }

        public async Task<Dictionary<ServiceDictionaryKey, object>> TryPutMeal(int patientId, int mealId,HttpRequest request)
        {
            Dictionary<ServiceDictionaryKey, object> dictionary = new Dictionary<ServiceDictionaryKey, object>();

            try
            {
                string requestBody = await new StreamReader(request.Body).ReadToEndAsync();

                Meal meal = new Meal();
                JsonConvert.PopulateObject(requestBody, meal);
                meal.PatientId = patientId;
                meal.MealId = mealId;
                meal = await _mealRepository.PutMeal(meal);
                dynamic data = _messageSerializer.Serialize(meal);

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
