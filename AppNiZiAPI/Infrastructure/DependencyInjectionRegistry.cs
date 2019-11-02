﻿using AppNiZiAPI.Models.Handler;
using AppNiZiAPI.Models.Handlers;
using AppNiZiAPI.Models.Repositories;
using AppNiZiAPI.Security;
using AppNiZiAPI.Services;
using AppNiZiAPI.Services.Serializer;
using Microsoft.Extensions.DependencyInjection;
using RestSharp.Serializers;

namespace AppNiZiAPI.Infrastructure
{
    public static class DependencyInjectionRegistry
    {
        public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
        {
            services.AddSingleton<IAccountRepository, AccountRepository>();
            services.AddSingleton<IPatientRepository, PatientRepository>();
            services.AddSingleton<IDoctorRepository, DoctorRepository>();

            services.AddSingleton<IAuthorizationRepository, AuthorizationRepository>();
            services.AddSingleton<IAuthorization, Authorization>();

            services.AddSingleton<IDietaryManagementRepository, DietaryManagementRepository>();
            services.AddSingleton<IFoodRepository, FoodRepository>();
            services.AddSingleton<IMealRepository, MealRepository>();
            services.AddSingleton<IWaterRepository, WaterRepository>();
            services.AddSingleton<IConsumptionRepository, ConsumptionRespository>();

            return services;
        }

        public static IServiceCollection AddHelperServices(this IServiceCollection services)
        {
            services.AddSingleton<IMessageSerializer, JsonMessageSerializer>();
            services.AddSingleton<IQueryHelper, QueryHelper>();
            services.AddSingleton<IFeedbackHandler, FeedbackHandler>();

            return services;
        }

        public static IServiceCollection AddAPIServices(this IServiceCollection services)
        {
            services.AddSingleton<IPatientService, PatientService>();

            return services;
        }
    }
}
