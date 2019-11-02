using AppNiZiAPI.Models.Repositories;
using AppNiZiAPI.Security;
using Microsoft.Extensions.DependencyInjection;

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
    }
}
