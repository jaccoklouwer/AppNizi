using System.Reflection;
using AppNiZiAPI;
using AzureFunctions.Extensions.Swashbuckle;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;

[assembly: WebJobsStartup(typeof(Startup))]

namespace AppNiZiAPI
{
    internal class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            //Register the extension
            builder.AddSwashBuckle(Assembly.GetExecutingAssembly());

        }
    }
}
