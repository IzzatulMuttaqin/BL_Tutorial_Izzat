using AzureFunctions.Extensions.Swashbuckle;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

[assembly: WebJobsStartup(typeof(BL_Tutorial_Izzat.API.ServiceStartup))]
namespace BL_Tutorial_Izzat.API
{
    public class ServiceStartup : FunctionsStartup
    {
        private static readonly IConfigurationRoot Configuration = new ConfigurationBuilder()
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile("appsettings.json", true)
            .AddEnvironmentVariables()
            .Build();
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.AddSwashBuckle(Assembly.GetExecutingAssembly(), c =>
            {
                c.ConfigureSwaggerGen = a => a.SchemaGeneratorOptions.SchemaIdSelector = (type) => type.FullName;
            });

            builder.Services.AddSingleton(s =>
            {
                var connectionString = Configuration["cosmos-bl-tutorial-serverless"];
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException(
                        "Please specify a valid CosmosDB connection string in the appSettings.json file or your Azure Functions Settings.");
                }
                return new CosmosClientBuilder(connectionString).Build();
            });
        }
    }
}
