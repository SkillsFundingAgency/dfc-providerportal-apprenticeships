using System.Linq;
using System.Threading.Tasks;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Apprenticeships;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Helper;
using Dfc.ProviderPortal.Apprenticeships.Models;
using Dfc.ProviderPortal.Apprenticeships.Settings;
using Microsoft.Extensions.Options;

namespace Dfc.ProviderPortal.Apprenticeships.Services
{
 
    public class ApprenticeshipMigrationReportService : IApprenticeshipMigrationReportService
    {
        private readonly ICosmosDbHelper _cosmosDbHelper;
        private readonly CosmosDbCollectionSettings _settings;
        public ApprenticeshipMigrationReportService(
            ICosmosDbHelper cosmosDbHelper,
            IOptions<CosmosDbCollectionSettings> settings)
        {
            _cosmosDbHelper = cosmosDbHelper;
            _settings = settings.Value;


        }

        public async Task CreateApprenticeshipReport(ApprenticeshipMigrationReport report)
        {
            using (var client = _cosmosDbHelper.GetClient())
            {
                await _cosmosDbHelper.CreateDatabaseIfNotExistsAsync(client);
                await _cosmosDbHelper.CreateDocumentCollectionIfNotExistsAsync(client,
                    _settings.ApprenticeshipReportCollectionId);

                var result = _cosmosDbHelper.GetDocumentsByUKPRN<ApprenticeshipMigrationReport>(client, _settings.ApprenticeshipReportCollectionId,
                    report.ProviderUKPRN);

                if (result.Any())
                {
                    report.Id = result.FirstOrDefault().Id;

                    await _cosmosDbHelper.UpdateDocumentAsync(client, _settings.ApprenticeshipReportCollectionId,
                        report);
                }
                else
                {
                    var doc = await _cosmosDbHelper.CreateDocumentAsync(client, _settings.ApprenticeshipReportCollectionId, report);
                }
            }
        }
    }
}
