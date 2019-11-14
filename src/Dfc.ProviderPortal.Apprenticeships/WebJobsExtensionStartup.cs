using Dfc.ProviderPortal.Apprenticeships;
using Dfc.ProviderPortal.Apprenticeships.Helper;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Helper;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Services;
using Dfc.ProviderPortal.Apprenticeships.Services;
using Dfc.ProviderPortal.Apprenticeships.Settings;
using Dfc.ProviderPortal.Packages.AzureFunctions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Apprenticeships;

[assembly: WebJobsStartup(typeof(WebJobsExtensionStartup), "Web Jobs Extension Startup")]

namespace Dfc.ProviderPortal.Apprenticeships
{
    public class WebJobsExtensionStartup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddDependencyInjection();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            builder.Services.AddSingleton<IConfiguration>(configuration);
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


        }
    }
}
