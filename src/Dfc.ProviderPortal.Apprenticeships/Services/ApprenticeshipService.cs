using Dfc.ProviderPortal.Apprenticeships.Interfaces.Apprenticeships;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Helper;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Models;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Services;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Settings;
using Dfc.ProviderPortal.Apprenticeships.Models;
using Dfc.ProviderPortal.Apprenticeships.Settings;
using Dfc.ProviderPortal.Packages;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dfc.ProviderPortal.Apprenticeships.Services
{
    public class ApprenticeshipService : IApprenticeshipService
    {
        private readonly ICosmosDbHelper _cosmosDbHelper;
        private readonly ICosmosDbCollectionSettings _settings;

        public ApprenticeshipService(
            ICosmosDbHelper cosmosDbHelper,
            IOptions<CosmosDbCollectionSettings> settings)
        {
            Throw.IfNull(cosmosDbHelper, nameof(cosmosDbHelper));
            Throw.IfNull(settings, nameof(settings));
            _cosmosDbHelper = cosmosDbHelper;
            _settings = settings.Value;

        }
        public async Task<IApprenticeship> AddApprenticeship(IApprenticeship apprenticeship)
        {
            Throw.IfNull(apprenticeship, nameof(apprenticeship));

            Apprenticeship persisted;

            using (var client = _cosmosDbHelper.GetClient())
            {
                await _cosmosDbHelper.CreateDatabaseIfNotExistsAsync(client);
                await _cosmosDbHelper.CreateDocumentCollectionIfNotExistsAsync(client, _settings.ApprenticeshipCollectionId);
                var doc = await _cosmosDbHelper.CreateDocumentAsync(client, _settings.ApprenticeshipCollectionId, apprenticeship);
                persisted = _cosmosDbHelper.DocumentTo<Apprenticeship>(doc);
            }

            return persisted;
        }
        public async Task<IEnumerable<IStandardsAndFrameworks>> StandardsAndFrameworksSearch(string search)
        {
            Throw.IfNullOrWhiteSpace(search, nameof(search));
            IEnumerable<IStandardsAndFrameworks> persisted = null;
            using (var client = _cosmosDbHelper.GetClient())
            {
                await _cosmosDbHelper.CreateDatabaseIfNotExistsAsync(client);
                await _cosmosDbHelper.CreateDocumentCollectionIfNotExistsAsync(client, _settings.StandardsCollectionId);
                await _cosmosDbHelper.CreateDocumentCollectionIfNotExistsAsync(client, _settings.FrameworkCollectionId);

                var standardDocs = _cosmosDbHelper.GetStandardsAndFrameworksBySearch(client, _settings.StandardsCollectionId, search);
                var frameworkDocs = _cosmosDbHelper.GetStandardsAndFrameworksBySearch(client, _settings.FrameworkCollectionId, search);
                persisted = standardDocs.Concat(frameworkDocs);
            }

            return persisted;
        }
    }
}
