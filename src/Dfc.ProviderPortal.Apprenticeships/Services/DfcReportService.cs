using Dfc.ProviderPortal.Apprenticeships.Interfaces.Apprenticeships;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Helper;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Settings;
using Dfc.ProviderPortal.Apprenticeships.Models;
using Dfc.ProviderPortal.Apprenticeships.Settings;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dfc.ProviderPortal.Apprenticeships.Services
{
    public class DfcReportService : IDfcReportService
    {
        private readonly ICosmosDbHelper _cosmosDbHelper;
        private readonly ICosmosDbCollectionSettings _settings;

        public DfcReportService(ICosmosDbHelper cosmosDbHelper, IOptions<CosmosDbCollectionSettings> settings)
        {
            _cosmosDbHelper = cosmosDbHelper;
            _settings = settings.Value;
        }

        public async Task<IEnumerable<ApprenticeshipDfcReportDocument>> GetDfcReports()
        {
            try
            {
                var client = _cosmosDbHelper.GetClient();
                var result = await _cosmosDbHelper.GetAllDfcMigrationReports(client,
                    _settings.ApprenticeshipDfcReportCollectionId);
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
