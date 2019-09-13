using Dfc.ProviderPortal.Apprenticeships.Helper;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Apprenticeships;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Helper;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Models;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Models.Regions;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Services;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Settings;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Tribal;
using Dfc.ProviderPortal.Apprenticeships.Models;
using Dfc.ProviderPortal.Apprenticeships.Models.Enums;
using Dfc.ProviderPortal.Apprenticeships.Models.Providers;
using Dfc.ProviderPortal.Apprenticeships.Models.Tribal;
using Dfc.ProviderPortal.Apprenticeships.Settings;
using Dfc.ProviderPortal.Packages;
using Microsoft.Azure.Documents;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dfc.ProviderPortal.Apprenticeships.Services
{
    public class ApprenticeshipService : IApprenticeshipService
    {
        private readonly ICosmosDbHelper _cosmosDbHelper;
        private readonly ITribalHelper _tribalHelper;
        private readonly ICosmosDbCollectionSettings _settings;
        private readonly IProviderServiceSettings _providerServiceSettings;


        public ApprenticeshipService(
            ICosmosDbHelper cosmosDbHelper,
            ITribalHelper tribalHelper,
            IOptions<CosmosDbCollectionSettings> settings,
            IOptions<ProviderServiceSettings> providerServiceSettings)
        {
            Throw.IfNull(cosmosDbHelper, nameof(cosmosDbHelper));
            Throw.IfNull(tribalHelper, nameof(tribalHelper));
            Throw.IfNull(settings, nameof(settings));
            Throw.IfNull(providerServiceSettings, nameof(providerServiceSettings));

            _cosmosDbHelper = cosmosDbHelper;
            _tribalHelper = tribalHelper;
            _settings = settings.Value;
            _providerServiceSettings = providerServiceSettings.Value;
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
                await _cosmosDbHelper.CreateDocumentCollectionIfNotExistsAsync(client, _settings.FrameworksCollectionId);

                var standardDocs = _cosmosDbHelper.GetStandardsAndFrameworksBySearch(client, _settings.StandardsCollectionId, search);
                var frameworkDocs = _cosmosDbHelper.GetStandardsAndFrameworksBySearch(client, _settings.FrameworksCollectionId, search);
                if(frameworkDocs.Count > 0)
                {
                    frameworkDocs = _cosmosDbHelper.GetProgTypesForFramework(client, _settings.ProgTypesCollectionId, frameworkDocs);
                }
                var apprenticeshipsForUKPRN = 
                
                persisted = standardDocs.Concat(frameworkDocs);
            }

            return persisted;
        }
        public IEnumerable<IStandardsAndFrameworks> CheckForDuplicateApprenticeships(IEnumerable<IStandardsAndFrameworks> standardsAndFrameworks, int UKPRN)
        {
            Throw.IfNull<int>(UKPRN, nameof(UKPRN));
            Throw.IfLessThan(0, UKPRN, nameof(UKPRN));

            if(standardsAndFrameworks != null)
            {
                var apprenticeships = GetApprenticeshipByUKPRN(UKPRN).Result.Where(x => x.RecordStatus == RecordStatus.Live);
                if (apprenticeships.Any())
                {
                    foreach(var item in standardsAndFrameworks)
                    {
                        var anyApprenticeshipForStandards = apprenticeships.Where(x => x.StandardCode.HasValue && 
                                                                      x.StandardCode == item.StandardCode && 
                                                                      x.Version == item.Version);
                        if (anyApprenticeshipForStandards.Any())
                        {
                            item.AlreadyCreated = true;
                        }
                        var anyApprenticeshipForFrameworks = apprenticeships.Where(x => x.FrameworkCode.HasValue &&
                                                                                   x.FrameworkCode == item.FrameworkCode &&
                                                                                   x.ProgType == item.ProgType &&
                                                                                   x.PathwayCode == item.PathwayCode);
                        if(anyApprenticeshipForFrameworks.Any())
                        {
                            item.AlreadyCreated = true;
                        }
                    }
                }
            }

            return standardsAndFrameworks;
        }
        public async Task<IApprenticeship> GetApprenticeshipById(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException($"Cannot be an empty {nameof(Guid)}", nameof(id));

            Apprenticeship persisted = null;

            using (var client = _cosmosDbHelper.GetClient())
            {
                await _cosmosDbHelper.CreateDatabaseIfNotExistsAsync(client);
                await _cosmosDbHelper.CreateDocumentCollectionIfNotExistsAsync(client, _settings.ApprenticeshipCollectionId);

                var doc = _cosmosDbHelper.GetDocumentById(client, _settings.ApprenticeshipCollectionId, id);
                persisted = _cosmosDbHelper.DocumentTo<Apprenticeship>(doc);
            }

            return persisted;
        }
        public async Task<IStandardsAndFrameworks> GetStandardsAndFrameworksById(Guid id, int type)
        {
            if (id == Guid.Empty)
                throw new ArgumentException($"Cannot be an empty {nameof(Guid)}", nameof(id));

            StandardsAndFrameworks persisted = null;

            using (var client = _cosmosDbHelper.GetClient())
            {
                await _cosmosDbHelper.CreateDatabaseIfNotExistsAsync(client);
                await _cosmosDbHelper.CreateDocumentCollectionIfNotExistsAsync(client, _settings.StandardsCollectionId);
                await _cosmosDbHelper.CreateDocumentCollectionIfNotExistsAsync(client, _settings.FrameworksCollectionId);

                Document doc = null;
                switch((ApprenticeshipType)type)
                {
                    case ApprenticeshipType.StandardCode:
                        {
                            doc = _cosmosDbHelper.GetDocumentById(client, _settings.StandardsCollectionId, id);
                            persisted = _cosmosDbHelper.DocumentTo<StandardsAndFrameworks>(doc);
                            break;
                        }
                    case ApprenticeshipType.FrameworkCode:
                        {
                            doc = _cosmosDbHelper.GetDocumentById(client, _settings.FrameworksCollectionId, id);
                            List<StandardsAndFrameworks> docs = new List<StandardsAndFrameworks>();
                            docs.Add(_cosmosDbHelper.DocumentTo<StandardsAndFrameworks>(doc));
                            docs = _cosmosDbHelper.GetProgTypesForFramework(client, _settings.ProgTypesCollectionId, docs);
                            persisted = docs[0];
                            break;
                        }
                }
                
            }

            return persisted;
        }
        public async Task<List<StandardsAndFrameworks>> GetStandardByCode(int standardCode, int standardVersion)
        {

            List<StandardsAndFrameworks> persisted = null;

            using (var client = _cosmosDbHelper.GetClient())
            {
                await _cosmosDbHelper.CreateDatabaseIfNotExistsAsync(client);
                await _cosmosDbHelper.CreateDocumentCollectionIfNotExistsAsync(client, _settings.StandardsCollectionId);
                var doc = _cosmosDbHelper.GetStandardByCode(client, _settings.StandardsCollectionId, standardCode, standardVersion);
                persisted = doc;
            }

            return persisted;
        }

        public async Task<List<StandardsAndFrameworks>> GetFrameworkByCode(int frameworkCode, int progType, int pathwayCode)
        {

            List<StandardsAndFrameworks> persisted = null;

            using (var client = _cosmosDbHelper.GetClient())
            {
                await _cosmosDbHelper.CreateDatabaseIfNotExistsAsync(client);
                await _cosmosDbHelper.CreateDocumentCollectionIfNotExistsAsync(client, _settings.StandardsCollectionId);
                var doc = _cosmosDbHelper.GetFrameworkByCode(client, _settings.FrameworksCollectionId, frameworkCode, progType, pathwayCode);
                persisted = doc;
            }

            return persisted;
        }

        public async Task<IEnumerable<IApprenticeship>> GetApprenticeshipByUKPRN(int UKPRN)
        {
            Throw.IfNull<int>(UKPRN, nameof(UKPRN));
            Throw.IfLessThan(0, UKPRN, nameof(UKPRN));

            IEnumerable<Apprenticeship> persisted = null;
            using (var client = _cosmosDbHelper.GetClient())
            {
                await _cosmosDbHelper.CreateDatabaseIfNotExistsAsync(client);
                await _cosmosDbHelper.CreateDocumentCollectionIfNotExistsAsync(client, _settings.ApprenticeshipCollectionId);

                var docs = _cosmosDbHelper.GetApprenticeshipByUKPRN(client, _settings.ApprenticeshipCollectionId, UKPRN);
                persisted = docs;
            }

            return persisted;
        }

        public async Task<IApprenticeship> Update(IApprenticeship apprenticeship)
        {


            Throw.IfNull(apprenticeship, nameof(apprenticeship));

            Apprenticeship updated = null;

            using (var client = _cosmosDbHelper.GetClient())
            {
                await _cosmosDbHelper.CreateDatabaseIfNotExistsAsync(client);
                await _cosmosDbHelper.CreateDocumentCollectionIfNotExistsAsync(client, _settings.ApprenticeshipCollectionId);

                var updatedDocument = await _cosmosDbHelper.UpdateDocumentAsync(client, _settings.ApprenticeshipCollectionId, apprenticeship);
                updated = _cosmosDbHelper.DocumentTo<Apprenticeship>(updatedDocument);
            }

            return updated;

        }

        public async Task<HttpResponseMessage> ChangeApprenticeshipStatusForUKPRNSelection(int UKPRN, RecordStatus CurrentStatus, RecordStatus StatusToBeChangedTo)
        {
            Throw.IfNull<int>(UKPRN, nameof(UKPRN));
            Throw.IfLessThan(0, UKPRN, nameof(UKPRN));

            var allApprenticeships = GetApprenticeshipByUKPRN(UKPRN).Result;
            var apprenticeshipsToBeChanged = allApprenticeships.Where(x => x.RecordStatus == CurrentStatus).ToList();

            try
            {
                foreach (var apprenticeship in apprenticeshipsToBeChanged)
                {
                    apprenticeship.RecordStatus = StatusToBeChangedTo;
                    var result = Update(apprenticeship);
                }

                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.ExpectationFailed);
            }
        }

        public async Task<List<string>> DeleteBulkUploadApprenticeships(int UKPRN)
        {
            Throw.IfNull<int>(UKPRN, nameof(UKPRN));
            Throw.IfLessThan(0, UKPRN, nameof(UKPRN));

            List<string> results = null;
            using (var client = _cosmosDbHelper.GetClient())
            {
                results = await _cosmosDbHelper.DeleteBulkUploadApprenticeships(client, _settings.ApprenticeshipCollectionId, UKPRN);
            }

            return results;
        }

        public async Task<List<string>> DeleteApprenticeshipsByUKPRN(int UKPRN)
        {
            Throw.IfNull<int>(UKPRN, nameof(UKPRN));
            Throw.IfLessThan(0, UKPRN, nameof(UKPRN));

            List<string> results = null;
            using (var client = _cosmosDbHelper.GetClient())
            {
                results = await _cosmosDbHelper.DeleteDocumentsByUKPRN(client, _settings.ApprenticeshipCollectionId, UKPRN);
            }

            return results;
        }
        public async Task<IEnumerable<IApprenticeship>> GetApprenticeshipCollection()
        {
            using (var client = _cosmosDbHelper.GetClient())
            {
                await _cosmosDbHelper.CreateDatabaseIfNotExistsAsync(client);
                await _cosmosDbHelper.CreateDocumentCollectionIfNotExistsAsync(client, _settings.ApprenticeshipCollectionId);

                return _cosmosDbHelper.GetApprenticeshipCollection(client, _settings.ApprenticeshipCollectionId);
            }

            
        }
        public IEnumerable<ITribalProvider> ApprenticeshipsToTribalProviders(List<Apprenticeship> apprenticeships)
        {
            List<TribalProvider> providers = new List<TribalProvider>();
            List<string> listOfProviderUKPRN = new List<string>();

            listOfProviderUKPRN = apprenticeships.Select(x => x.ProviderUKPRN.ToString())
                                                 .Distinct()
                                                 .ToList();
            foreach(var ukprn in listOfProviderUKPRN)
            {
                var providerApprenticeships = apprenticeships.Where(x => x.ProviderUKPRN.ToString() == ukprn && x.RecordStatus == RecordStatus.Live).ToList();
                
                var providerDetailsList = GetProviderDetails(ukprn);
                if(providerDetailsList != null && providerDetailsList.Count() > 0)
                {
                    
                    var tribalProvider = _tribalHelper.CreateTribalProviderFromProvider(providerDetailsList.FirstOrDefault());
                    var apprenticeshipLocations = providerApprenticeships.Where(x => x.ApprenticeshipLocations != null)
                                                 .SelectMany(x => x.ApprenticeshipLocations);
                    
                    tribalProvider.Locations = _tribalHelper.ApprenticeshipLocationsToLocations(apprenticeshipLocations.Where(x => x.RecordStatus == RecordStatus.Live));
                    tribalProvider.Standards = _tribalHelper.ApprenticeshipsToStandards(providerApprenticeships.Where(x => x.StandardCode.HasValue));
                    tribalProvider.Frameworks = _tribalHelper.ApprenticeshipsToFrameworks(providerApprenticeships.Where(x => x.FrameworkCode.HasValue));
                    providers.Add(tribalProvider);
                }
            }
            return providers;
        }
        public async Task<IEnumerable<IApprenticeship>> GetUpdatedApprenticeships()
        {
            IEnumerable<Apprenticeship> persisted = null;
            
            using (var client = _cosmosDbHelper.GetClient())
            {
                await _cosmosDbHelper.CreateDatabaseIfNotExistsAsync(client);
                await _cosmosDbHelper.CreateDocumentCollectionIfNotExistsAsync(client, _settings.ApprenticeshipCollectionId);

                var docs = _cosmosDbHelper.GetApprenticeshipCollection(client, _settings.ApprenticeshipCollectionId);
                persisted = OnlyUpdatedCourses(docs);
                persisted = RemoveNonLiveApprenticeships(persisted);
            }

            return persisted;
        }
        internal IEnumerable<Provider> GetProviderDetails(string UKPRN)
        {
            return new ProviderServiceWrapper(_providerServiceSettings).GetProviderByUKPRN(UKPRN);
        }
        internal IEnumerable<Apprenticeship> OnlyUpdatedCourses(IEnumerable<Apprenticeship> apprenticeships)
        {
            DateTime dateToCheckAgainst = DateTime.Now.Subtract(TimeSpan.FromDays(1));

            return apprenticeships.Where(x => x.UpdatedDate.HasValue && x.UpdatedDate > dateToCheckAgainst && x.RecordStatus == RecordStatus.Live).ToList();
        }
        internal IEnumerable<Apprenticeship> RemoveNonLiveApprenticeships(IEnumerable<Apprenticeship> apprenticeships)
        {
            List<Apprenticeship> editedList = new List<Apprenticeship>();
            foreach(var apprenticeship in apprenticeships)
            {
                if(apprenticeship.ApprenticeshipLocations.All(x => x.RecordStatus == RecordStatus.Live))
                {
                    editedList.Add(apprenticeship);
                }
            }
            return editedList;
        }

    }
}
