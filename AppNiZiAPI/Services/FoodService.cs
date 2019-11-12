using AppNiZiAPI.Models;
using AppNiZiAPI.Models.AccountModels;
using AppNiZiAPI.Models.Handler;
using AppNiZiAPI.Models.Handlers;
using AppNiZiAPI.Models.Repositories;
using AppNiZiAPI.Services.Helpers;
using AppNiZiAPI.Services.Serializer;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AppNiZiAPI.Services
{
    public interface IFoodService
    {
        IMessageHandler FeedbackHandler { get; }

        Task<Dictionary<ServiceDictionaryKey, object>> TryGetFoodById(int id);
        Task<Dictionary<ServiceDictionaryKey, object>> TryGetFoodBySearch(string searchQuery, int count);
        Task<Dictionary<ServiceDictionaryKey, object>> TryGetFavoriteFood(int patientId);
        Task<Dictionary<ServiceDictionaryKey, object>> TryPostFavoriteFood(HttpRequest request);
        Task<Dictionary<ServiceDictionaryKey, object>> TryDeleteFavoriteFood(HttpRequest request);
    }
    class FoodService : IFoodService
    {
        public IMessageHandler FeedbackHandler { get; }

        private readonly IFoodRepository _foodRepository;
        private readonly IMessageSerializer _messageSerializer;
        private readonly IQueryHelper _queryHelper;

        public FoodService(IFoodRepository foodRepository,
            IMessageHandler feedbackHandler, IMessageSerializer messageSerializer,
            IQueryHelper queryHelper)
        {
            _foodRepository = foodRepository;
            this.FeedbackHandler = feedbackHandler;
            _messageSerializer = messageSerializer;
            _queryHelper = queryHelper;
        }

        public async Task<Dictionary<ServiceDictionaryKey, object>> TryDeleteFavoriteFood(HttpRequest request)
        {
            Dictionary<ServiceDictionaryKey, object> dictionary = new Dictionary<ServiceDictionaryKey, object>();
            int foodId = 0;
            int patientId = 0;
            try
            {
                //TODO haal patient id op een coole manier op
                foodId = Convert.ToInt32(request.Query["foodId"].ToString());
                patientId = Convert.ToInt32(request.Query["patientId"].ToString());
            }
            catch (Exception ex)
            {
                dictionary.AddErrorMessage(ServiceDictionaryKey.ERROR, ex, FeedbackHandler);
            }
            try
            {
                bool succes = await _foodRepository.UnFavorite(patientId, foodId);
                if (!succes)
                {
                    dictionary.Add(ServiceDictionaryKey.ERROR, "No rows affected. Does the favorite exist?");
                    dictionary.Add(ServiceDictionaryKey.HTTPSTATUSCODE, HttpStatusCode.NotFound);
                }
                else
                {
                    dictionary.Add(ServiceDictionaryKey.VALUE, "Favorited item is deleted");
                    dictionary.Add(ServiceDictionaryKey.HTTPSTATUSCODE, HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                dictionary.AddErrorMessage(ServiceDictionaryKey.ERROR, ex, FeedbackHandler);
            }
            return dictionary;
        }

        public async Task<Dictionary<ServiceDictionaryKey, object>> TryGetFavoriteFood(int patientId)
        {
            Dictionary<ServiceDictionaryKey, object> dictionary = new Dictionary<ServiceDictionaryKey, object>();


            try
            {
                List<Food> foods = await _foodRepository.Favorites(patientId);

                if (foods.Count <= 0)
                {
                    dictionary.Add(ServiceDictionaryKey.ERROR, $"No favorites found for given ID: {patientId}.");
                    dictionary.Add(ServiceDictionaryKey.HTTPSTATUSCODE, HttpStatusCode.NotFound);
                    return dictionary;
                }

                dynamic data = _messageSerializer.Serialize(foods);
                dictionary.Add(ServiceDictionaryKey.VALUE, data);
            }
            catch (Exception ex)
            {
                dictionary.AddErrorMessage(ServiceDictionaryKey.ERROR, ex, FeedbackHandler);
            }

            return dictionary;
        }

        public async Task<Dictionary<ServiceDictionaryKey, object>> TryGetFoodById(int id)
        {
            Dictionary<ServiceDictionaryKey, object> dictionary = new Dictionary<ServiceDictionaryKey, object>();

            try
            {
                Food food = await _foodRepository.SelectAsync(id);

                if (food.FoodId <= 0)
                {
                    dictionary.Add(ServiceDictionaryKey.ERROR, $"No food found for given ID: {id}.");
                    dictionary.Add(ServiceDictionaryKey.HTTPSTATUSCODE, HttpStatusCode.NotFound);
                    return dictionary;
                }

                dynamic data = _messageSerializer.Serialize(food);
                dictionary.Add(ServiceDictionaryKey.VALUE, data);
            }
            catch (Exception ex)
            {
                dictionary.AddErrorMessage(ServiceDictionaryKey.ERROR, ex, FeedbackHandler);
            }

            return dictionary;
        }

        async public Task<Dictionary<ServiceDictionaryKey, object>> TryGetFoodBySearch(string searchQuery, int count)
        {
            Dictionary<ServiceDictionaryKey, object> dictionary = new Dictionary<ServiceDictionaryKey, object>();

            if (count <= 0)
            {
                dictionary.Add(ServiceDictionaryKey.ERROR, $"You forgot to supply a count higher than 0");
                dictionary.Add(ServiceDictionaryKey.HTTPSTATUSCODE, HttpStatusCode.BadRequest);
            }
            try
            {
                List<Food> foods = await _foodRepository.Search(searchQuery, count);

                if (foods.Count <= 0)
                {
                    dictionary.Add(ServiceDictionaryKey.ERROR, $"No foods found for the given query: {searchQuery}.");
                    dictionary.Add(ServiceDictionaryKey.HTTPSTATUSCODE, HttpStatusCode.NotFound);
                    return dictionary;
                }

                dynamic data = _messageSerializer.Serialize(foods);
                dictionary.Add(ServiceDictionaryKey.VALUE, data);
            }
            catch (Exception ex)
            {
                dictionary.AddErrorMessage(ServiceDictionaryKey.ERROR, ex, FeedbackHandler);
            }

            return dictionary;
        }

        async public Task<Dictionary<ServiceDictionaryKey, object>> TryPostFavoriteFood(HttpRequest request)
        {
            Dictionary<ServiceDictionaryKey, object> dictionary = new Dictionary<ServiceDictionaryKey, object>();

            int foodId = 0;
            int patientId = 0;

            try
            {
                foodId = Convert.ToInt32(request.Query["foodId"].ToString());
                patientId = Convert.ToInt32(request.Query["patientId"].ToString());
            }
            catch
            {
                dictionary.Add(ServiceDictionaryKey.ERROR, $"Bad parameters");
                dictionary.Add(ServiceDictionaryKey.HTTPSTATUSCODE, HttpStatusCode.BadRequest);
            }
            try
            {
                bool succes = await _foodRepository.Favorite(patientId, foodId);

                if (!succes)
                {
                    dictionary.Add(ServiceDictionaryKey.ERROR, $"No favorites could be performed");
                    dictionary.Add(ServiceDictionaryKey.HTTPSTATUSCODE, HttpStatusCode.BadRequest);
                    return dictionary;
                }
                else
                {
                    dictionary.Add(ServiceDictionaryKey.VALUE, $"The Favorite has been added for foodid:{foodId} and patientid :{patientId}");
                    dictionary.Add(ServiceDictionaryKey.HTTPSTATUSCODE, HttpStatusCode.OK);
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
