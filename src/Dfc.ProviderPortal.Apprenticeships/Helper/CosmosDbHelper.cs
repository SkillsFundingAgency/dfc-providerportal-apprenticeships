﻿using Dfc.ProviderPortal.Apprenticeships.Interfaces.Helper;
using Dfc.ProviderPortal.Apprenticeships.Interfaces.Settings;
using Dfc.ProviderPortal.Apprenticeships.Models;
using Dfc.ProviderPortal.Apprenticeships.Settings;
using Dfc.ProviderPortal.Packages;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Dfc.ProviderPortal.Apprenticeships.Models.Enums;
using Microsoft.Azure.Documents.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Web.Http;
using System.Net.Http;
using System.IO;
using System.Reflection;

namespace Dfc.ProviderPortal.Apprenticeships.Helper
{
    public class CosmosDbHelper : ICosmosDbHelper
    {
        private static readonly Regex _searchableChars = new Regex("[^a-z0-9]", RegexOptions.IgnoreCase);

        private readonly ICosmosDbSettings _settings;
        private readonly DocumentClient _client;

        public CosmosDbHelper(IOptions<CosmosDbSettings> settings, DocumentClient documentClient)
        {
            Throw.IfNull(settings, nameof(settings));

            _settings = settings.Value;
            _client = documentClient;
        }

        public async Task<Database> CreateDatabaseIfNotExistsAsync(DocumentClient client)
        {
            Throw.IfNull(client, nameof(client));

            var db = new Database { Id = _settings.DatabaseId };

            return await client.CreateDatabaseIfNotExistsAsync(db);
        }

        public async Task<Document> CreateDocumentAsync(
            DocumentClient client,
            string collectionId,
            object document)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNullOrWhiteSpace(collectionId, nameof(collectionId));
            Throw.IfNull(document, nameof(document));

            var uri = UriFactory.CreateDocumentCollectionUri(
                _settings.DatabaseId,
                collectionId);

            return await client.CreateDocumentAsync(uri, document);
        }

        public async Task<DocumentCollection> CreateDocumentCollectionIfNotExistsAsync(
            DocumentClient client,
            string collectionId)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNullOrWhiteSpace(collectionId, nameof(collectionId));

            var uri = UriFactory.CreateDatabaseUri(_settings.DatabaseId);
            var coll = new DocumentCollection
            {
                Id = collectionId,
                PartitionKey = new PartitionKeyDefinition()
                {
                    Paths = new Collection<string>()
            {
                "/ProviderUKPRN"
            }
                }
            };

            return await client.CreateDocumentCollectionIfNotExistsAsync(uri, coll);
        }

        public T DocumentTo<T>(Document document)
        {
            Throw.IfNull(document, nameof(document));
            return (T)(dynamic)document;
        }

        public IEnumerable<T> DocumentsTo<T>(IEnumerable<Document> documents)
        {
            Throw.IfNull(documents, nameof(documents));
            return (IEnumerable<T>)(IEnumerable<dynamic>)documents;
        }

        public DocumentClient GetClient() => _client;

        public Document GetDocumentById<T>(DocumentClient client, string collectionId, T id)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNullOrWhiteSpace(collectionId, nameof(collectionId));
            Throw.IfNull(id, nameof(id));

            var uri = UriFactory.CreateDocumentCollectionUri(
                _settings.DatabaseId,
                collectionId);

            var options = new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = -1 };

            var doc = client.CreateDocumentQuery(uri, options)
                .Where(x => x.Id == id.ToString())
                .AsEnumerable()
                .FirstOrDefault();

            return doc;
        }
        public List<Apprenticeship> GetApprenticeshipByUKPRN(DocumentClient client, string collectionId, int UKPRN)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNullOrWhiteSpace(collectionId, nameof(collectionId));
            Throw.IfNull(UKPRN, nameof(UKPRN));

            Uri uri = UriFactory.CreateDocumentCollectionUri(_settings.DatabaseId, collectionId);
            FeedOptions options = new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = -1 };

            List<Apprenticeship> docs = client.CreateDocumentQuery<Apprenticeship>(uri, options)
                                             .Where(x => x.ProviderUKPRN == UKPRN)
                                             .ToList(); // .AsEnumerable();

            return docs;
        }

        public IList<T> GetDocumentsByUKPRN<T>(DocumentClient client, string collectionId, int UKPRN)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNullOrWhiteSpace(collectionId, nameof(collectionId));
            Throw.IfNull(UKPRN, nameof(UKPRN));

            Uri uri = UriFactory.CreateDocumentCollectionUri(_settings.DatabaseId, collectionId);
            FeedOptions options = new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = -1 };


            var docs = client.CreateDocumentQuery<T>(uri, $"SELECT * FROM c WHERE c.ProviderUKPRN = {UKPRN}");

            return docs == null ? new List<T>() : docs.ToList();
        }

        public async Task<Document> UpdateDocumentAsync(
            DocumentClient client,
            string collectionId,
            object document)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNullOrWhiteSpace(collectionId, nameof(collectionId));
            Throw.IfNull(document, nameof(document));

            var uri = UriFactory.CreateDocumentCollectionUri(
                _settings.DatabaseId,
                collectionId);

            return await client.UpsertDocumentAsync(uri, document);
        }

        public List<StandardsAndFrameworks> GetStandardsAndFrameworksBySearch(DocumentClient client, string collectionId, string search)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNullOrWhiteSpace(collectionId, nameof(collectionId));
            Throw.IfNullOrWhiteSpace(search, nameof(search));

            Uri uri = UriFactory.CreateDocumentCollectionUri(_settings.DatabaseId, collectionId);
            FeedOptions options = new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = -1 };

            List<StandardsAndFrameworks> docs = new List<StandardsAndFrameworks>();
            var formattedSearch = FormatSearchTerm(search);
            var allDocs = client.CreateDocumentQuery<StandardsAndFrameworks>(uri, options).ToList();

            switch (collectionId)
            {
                case "standards":
                    {
                        docs = (from string s in formattedSearch
                                from StandardsAndFrameworks saf
                                in allDocs
                                let searchWords = FormatSearchTerm(saf.StandardName)
                                where IsMatch(formattedSearch, searchWords)
                                select saf).Where(x => x.RecordStatusId == 2).ToList();
                        docs.Select(x =>
                        {
                            x.ApprenticeshipType = Models.Enums.ApprenticeshipType.StandardCode; return x;
                        }).ToList();
                        break;
                    }
                case "frameworks":
                    {
                        docs = (from string s in formattedSearch
                                from StandardsAndFrameworks saf
                                in allDocs
                                let searchWords = FormatSearchTerm(saf.NasTitle)
                                where IsMatch(formattedSearch, searchWords)
                                select saf).Where(x => x.RecordStatusId == 2).ToList();
                        docs.Select(x => { x.ApprenticeshipType = Models.Enums.ApprenticeshipType.FrameworkCode; return x; }).ToList();
                        break;
                    }
            }
            return docs;

            bool IsMatch(IEnumerable<string> searchWords, IEnumerable<string> referenceWords)
            {
                // Match whenever any search term is found in reference words using a prefix match
                // i.e. search for 'retail' should match 'retail' & 'retailer'
                // but search for 'etail' should not match either
                // *unless* the search term is a number in which case the entire word must match

                return searchWords.Any(s => referenceWords.Any(r => s.All(Char.IsDigit) ? r.Equals(s) : r.StartsWith(s)));
            }
        }

        public List<StandardsAndFrameworks> GetProgTypesForFramework(DocumentClient client, string collectionId, List<StandardsAndFrameworks> docs)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNullOrWhiteSpace(collectionId, nameof(collectionId));
            Throw.IfNullOrEmpty(docs, nameof(docs));
            Uri uri = UriFactory.CreateDocumentCollectionUri(_settings.DatabaseId, collectionId);
            FeedOptions options = new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = -1 };



            foreach (var doc in docs)
            {
                List<ProgType> progType = new List<ProgType>();

                progType = client.CreateDocumentQuery<ProgType>(uri, options)
                    .Where(x => x.ProgTypeId == doc.ProgType).ToList();

                if (!string.IsNullOrEmpty(progType[0].ProgTypeId.ToString()))
                {
                    doc.ProgTypeDesc = progType[0].ProgTypeDesc;
                    doc.ProgTypeDesc2 = progType[0].ProgTypeDesc2;
                }


            }
            return docs;
        }
        public List<StandardsAndFrameworks> GetStandardByCode(DocumentClient client, string collectionId, int standardCode, int version)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNullOrWhiteSpace(collectionId, nameof(collectionId));

            Uri uri = UriFactory.CreateDocumentCollectionUri(_settings.DatabaseId, collectionId);
            FeedOptions options = new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = -1 };

            List<StandardsAndFrameworks> docs = client.CreateDocumentQuery<StandardsAndFrameworks>(uri, options)
                                             .Where(x => x.StandardCode == standardCode && x.Version == version)
                                             .ToList();

            return docs;
        }
        public List<StandardsAndFrameworks> GetFrameworkByCode(DocumentClient client, string collectionId, int frameworkCode, int progType, int pathwayCode)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNullOrWhiteSpace(collectionId, nameof(collectionId));

            Uri uri = UriFactory.CreateDocumentCollectionUri(_settings.DatabaseId, collectionId);
            FeedOptions options = new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = -1 };

            List<StandardsAndFrameworks> docs = client.CreateDocumentQuery<StandardsAndFrameworks>(uri, options)
                                             .Where(x => x.FrameworkCode == frameworkCode
                                                        && x.ProgType == progType
                                                        && x.PathwayCode == pathwayCode)
                                             .ToList();

            return docs;
        }
        public async Task<List<string>> DeleteBulkUploadApprenticeships(DocumentClient client, string collectionId, int UKPRN)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNullOrWhiteSpace(collectionId, nameof(collectionId));
            Throw.IfNull(UKPRN, nameof(UKPRN));

            Uri uri = UriFactory.CreateDocumentCollectionUri(_settings.DatabaseId, collectionId);
            FeedOptions options = new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = -1 };

            List<Apprenticeship> docs = client.CreateDocumentQuery<Apprenticeship>(uri, options)
                .Where(x => x.ProviderUKPRN == UKPRN && (x.RecordStatus == RecordStatus.BulkUploadPending ||
                                                         x.RecordStatus == RecordStatus.BulkUploadReadyToGoLive)).ToList();

            var responseList = new List<string>();

            foreach (var doc in docs)
            {
                Uri docUri = UriFactory.CreateDocumentUri(_settings.DatabaseId, collectionId, doc.id.ToString());
                var result = await client.DeleteDocumentAsync(docUri, new RequestOptions() { PartitionKey = new PartitionKey(doc.ProviderUKPRN) });

                if (result.StatusCode == HttpStatusCode.NoContent)
                {
                    responseList.Add($"Apprenticeship with Title ( { doc.ApprenticeshipTitle } ) was deleted.");
                }
                else
                {
                    responseList.Add($"Course with Title ( { doc.ApprenticeshipTitle } ) wasn't deleted. StatusCode: ( { result.StatusCode } )");
                }

            }

            return responseList;
        }

        public async Task<List<string>> DeleteDocumentsByUKPRN(DocumentClient client, string collectionId, int UKPRN)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNullOrWhiteSpace(collectionId, nameof(collectionId));
            Throw.IfNull(UKPRN, nameof(UKPRN));

            Uri uri = UriFactory.CreateDocumentCollectionUri(_settings.DatabaseId, collectionId);
            FeedOptions options = new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = -1 };

            List<Apprenticeship> docs = client.CreateDocumentQuery<Apprenticeship>(uri, options)
                .Where(x => x.ProviderUKPRN == UKPRN)
                .ToList();

            var responseList = new List<string>();

            foreach (var doc in docs)
            {
                Uri docUri = UriFactory.CreateDocumentUri(_settings.DatabaseId, collectionId, doc.id.ToString());
                var result = await client.DeleteDocumentAsync(docUri, new RequestOptions() { PartitionKey = new PartitionKey(doc.ProviderUKPRN) });

                if (result.StatusCode == HttpStatusCode.NoContent)
                {
                    responseList.Add($"Apprenticeship with Title ( { doc.ApprenticeshipTitle } ) was deleted.");
                }
                else
                {
                    responseList.Add($"Apprenticeship with Title ( { doc.ApprenticeshipTitle } ) wasn't deleted. StatusCode: ( { result.StatusCode } )");
                }

            }

            return responseList;
        }

        public List<Apprenticeship> GetApprenticeshipCollection(DocumentClient client, string collectionId)
        {

            Uri uri = UriFactory.CreateDocumentCollectionUri(_settings.DatabaseId, collectionId);
            FeedOptions options = new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = -1 };

            return client.CreateDocumentQuery<Apprenticeship>(uri, options).ToList();

        }
        internal static List<string> FormatSearchTerm(string searchTerm)
        {
            Throw.IfNullOrWhiteSpace(searchTerm, nameof(searchTerm));

            var split = _searchableChars.Replace(searchTerm, " ")
                .ToLower()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            return split;
        }

        public async Task<List<ApprenticeshipDfcReportDocument>> GetAllDfcMigrationReports(DocumentClient client, string collectionId)
        {
            var reports = new List<ApprenticeshipDfcReportDocument>();

            Uri uri = UriFactory.CreateDocumentCollectionUri(_settings.DatabaseId, collectionId);
            FeedOptions options = new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = -1 };

            using (var queryable = client.CreateDocumentQuery<ApprenticeshipDfcReportDocument>(uri, options).AsDocumentQuery())
            {
                while (queryable.HasMoreResults)
                {
                    foreach (ApprenticeshipDfcReportDocument report in await queryable.ExecuteNextAsync<ApprenticeshipDfcReportDocument>())
                    {
                        //Some Providers have ',' in there name which is breaking the CSV
                        report.ProviderName = report.ProviderName.Replace(",", "");
                        reports.Add(report);
                    }
                }
            }

            return reports;
        }

        public async Task<int> GetTotalLiveApprenticeships(DocumentClient client, string collectionId)
        {
            Uri uri = UriFactory.CreateDocumentCollectionUri(_settings.DatabaseId, collectionId);
            FeedOptions options = new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = -1 };

            return await client.CreateDocumentQuery<Apprenticeship>(uri, options)
                .Where(cr => cr.RecordStatus == RecordStatus.Live)
                .CountAsync();
        }


        public async Task<int> UpdateRecordStatuses(DocumentClient client, string collectionId, string procedureName, int UKPRN, int currentStatusMask, int newStatus, int partitionKey)
        {
            RequestOptions requestOptions = new RequestOptions { PartitionKey = new PartitionKey(partitionKey), EnableScriptLogging = true };

            var response = await client.ExecuteStoredProcedureAsync<dynamic>(
                UriFactory.CreateStoredProcedureUri(_settings.DatabaseId, collectionId, "UpdateRecordStatuses"),
                requestOptions,
                UKPRN,
                currentStatusMask,
                newStatus);

            return response.Response.updated;

        }


        public async Task DeployStoredProcedures()
        {
            var client = GetClient();
            
            await DeployStoredProcedureToCollection("apprenticeship", "UpdateRecordStatuses");

            async Task DeployStoredProcedureToCollection(string collection, string storedProcedureName)
            {
                var scriptFilePath = $"Dfc.ProviderPortal.Apprenticeships.Data.StoredProcedures.{storedProcedureName}.js";

                using (var stream = typeof(CosmosDbHelper).Assembly.GetManifestResourceStream(scriptFilePath))
                {
                    if (stream == null)
                    {
                        throw new ArgumentException(
                            $"Cannot find stored procedure '{scriptFilePath}'.",
                            nameof(storedProcedureName));
                    }

                    string script;
                    using (var reader = new StreamReader(stream))
                    {
                        script = reader.ReadToEnd();
                    }

                    var storedProcId = storedProcedureName;
                    var collectionUri = UriFactory.CreateDocumentCollectionUri(_settings.DatabaseId, collection);

                    var storedProcedure = new StoredProcedure()
                    {
                        Body = script,
                        Id = storedProcId
                    };

                    try
                    {
                        await client.CreateStoredProcedureAsync(collectionUri, storedProcedure);
                    }
                    catch (DocumentClientException dex) when (dex.StatusCode == System.Net.HttpStatusCode.Conflict)
                    {
                        // Already exists - replace it

                        var sprocUri = UriFactory.CreateStoredProcedureUri(_settings.DatabaseId, collection, storedProcId);
                        await client.ReplaceStoredProcedureAsync(sprocUri, storedProcedure);
                    }
                }
            }
        }

    }
}