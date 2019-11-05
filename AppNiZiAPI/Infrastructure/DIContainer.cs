using AppNiZiAPI.Models.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AppNiZiAPI.Infrastructure
{
    public sealed class DIContainer
    {
        private static readonly IServiceProvider _instance = Build();
        public static IServiceProvider Instance => _instance;

        static DIContainer()
        {

        }

        private DIContainer()
        {

        }

        private static IServiceProvider Build()
        {
            var services = new ServiceCollection();

            // Cloud uses environment variables, local uses local.settings.json - hence optional
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            // Here we're adding the different services, such as API's, queue, blob library and repositories. Register them in the registry.
            services.AddRepositoryServices();
            services.AddAPIServices();
            services.AddHelperServices();

            return services.BuildServiceProvider();
        }
    }
}
