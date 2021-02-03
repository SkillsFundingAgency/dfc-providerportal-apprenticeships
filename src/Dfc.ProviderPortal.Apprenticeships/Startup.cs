using System;
using Dfc.ProviderPortal.Apprenticeships;
using Dfc.ProviderPortal.Apprenticeships.Helper;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Apprenticeships;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Helper;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Services;
using Dfc.ProviderPortal.Apprenticeships.Services;
using Dfc.ProviderPortal.Apprenticeships.Settings;
using DFC.Swagger.Standard;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
[assembly: FunctionsStartup(typeof(Startup))]

namespace Dfc.ProviderPortal.Apprenticeships
{
    public class Startup : FunctionsStartup
    {
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            builder.ConfigurationBuilder
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("local.settings.json", optional: true);
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configuration = builder.GetContext().Configuration;

            builder.Services.Configure<CosmosDbSettings>(configuration.GetSection(nameof(CosmosDbSettings)));
            builder.Services.Configure<CosmosDbCollectionSettings>(configuration.GetSection(nameof(CosmosDbCollectionSettings)));
            builder.Services.Configure<ProviderServiceSettings>(configuration.GetSection(nameof(ProviderServiceSettings)));
            builder.Services.Configure<ReferenceDataServiceSettings>(configuration.GetSection(nameof(ReferenceDataServiceSettings)));
            builder.Services.AddScoped<IReferenceDataServiceWrapper, ReferenceDataServiceWrapper>();
            builder.Services.AddScoped<ICosmosDbHelper, CosmosDbHelper>();
            builder.Services.AddScoped<ITribalHelper, TribalHelper>();
            builder.Services.AddScoped<IApprenticeshipService, ApprenticeshipService>();
            builder.Services.AddScoped<IApprenticeshipMigrationReportService, ApprenticeshipMigrationReportService>();
            builder.Services.AddScoped<IDfcReportService, DfcReportService>();
            builder.Services.AddScoped<ISwaggerDocumentGenerator, SwaggerDocumentGenerator>();

            builder.Services.AddSingleton<DocumentClient>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<CosmosDbSettings>>().Value;

                return new DocumentClient(
                    new Uri(settings.EndpointUri),
                    settings.PrimaryKey,
                    new ConnectionPolicy() { ConnectionMode = ConnectionMode.Direct });
            });

            var serviceProvider = builder.Services.BuildServiceProvider();
            serviceProvider.GetService<ICosmosDbHelper>().DeployStoredProcedures().Wait();
        }
    }
}