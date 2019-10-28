using AppNiZiAPI.Models.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppNiZiAPI.Infrastructure
{
    public static class DependencyInjectionRegistry
    {
        public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
        {
            services.AddSingleton<IPatientRepository, PatientRepository>();

            services.AddSingleton<IFoodRepository, FoodRepository>();

            services.AddSingleton<IMealRepository, MealRepository>();

            return services;
        }
    }
}
