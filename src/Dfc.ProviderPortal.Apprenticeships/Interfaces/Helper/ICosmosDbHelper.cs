using Dfc.ProviderPortal.Apprenticeships.Interfaces.Apprenticeships;
using Dfc.ProviderPortal.Apprenticeships.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dfc.ProviderPortal.Apprenticeships.Interfaces.Helper
{
    public interface ICosmosDbHelper
    {
        DocumentClient GetClient();
        Task<Database> CreateDatabaseIfNotExistsAsync(DocumentClient client);
        Task<DocumentCollection> CreateDocumentCollectionIfNotExistsAsync(DocumentClient client, string collectionId);
        Task<Document> CreateDocumentAsync(DocumentClient client, string collectionId, object document);
        T DocumentTo<T>(Document document);
        IEnumerable<T> DocumentsTo<T>(IEnumerable<Document> documents);
        List<Apprenticeship> GetApprenticeshipByUKPRN(DocumentClient client, string collectionId, int UKPRN);
        Document GetDocumentById<T>(DocumentClient client, string collectionId, T id);
        Task<Document> UpdateDocumentAsync(DocumentClient client, string collectionId, object document);
        List<StandardsAndFrameworks> GetStandardsAndFrameworksBySearch(DocumentClient client, string collectionId, string search);
        List<StandardsAndFrameworks> GetProgTypesForFramework(DocumentClient client, string collectionId, List<StandardsAndFrameworks> docs);
        Task<List<string>> DeleteBulkUploadApprenticeships(DocumentClient client, string collectionId, int UKPRN);
        Task<List<string>> DeleteDocumentsByUKPRN(DocumentClient client, string collectionId, int UKPRN);
        List<Apprenticeship> GetApprenticeshipCollection(DocumentClient client, string collectionId);
        List<StandardsAndFrameworks> GetStandardByCode(DocumentClient client, string collectionId, int standardCode, int version);
        List<StandardsAndFrameworks> GetFrameworkByCode(DocumentClient client, string collectionId, int frameworkCode, int progType, int pathwayCode);
        IList<T> GetDocumentsByUKPRN<T>(DocumentClient client, string collectionId, int UKPRN);
        Task<List<ApprenticeshipDfcReportDocument>> GetAllDfcMigrationReports(DocumentClient client,
            string collectionId);
    }
}
