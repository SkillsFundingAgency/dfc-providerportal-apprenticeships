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
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Dfc.ProviderPortal.Apprenticeships.Models.Enums;

namespace Dfc.ProviderPortal.Apprenticeships.Helper
{
    public class CosmosDbHelper : ICosmosDbHelper
    {
        private readonly ICosmosDbSettings _settings;

        public CosmosDbHelper(IOptions<CosmosDbSettings> settings)
        {
            Throw.IfNull(settings, nameof(settings));

            _settings = settings.Value;
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
            var coll = new DocumentCollection { Id = collectionId };

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

        public DocumentClient GetClient()
        {
            return new DocumentClient(new Uri(_settings.EndpointUri), _settings.PrimaryKey);
        }

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
                                           in allDocs where saf.StandardName.ToLower().Contains(s)
                                           group saf by saf.StandardName into grouped
                                           where grouped.Count() == formattedSearch.Count
                                           select grouped.FirstOrDefault()).ToList();
                        docs.Select(x => { x.ApprenticeshipType = Models.Enums.ApprenticeshipType.StandardCode; return x; }).ToList();
                        break;
                    }
                case "frameworks":
                    {
                        docs = (from string s in formattedSearch
                                from StandardsAndFrameworks saf
                                in allDocs
                                where saf.NasTitle.ToLower().Contains(s)
                                group saf by saf.NasTitle into grouped
                                where grouped.Count() == formattedSearch.Count
                                select grouped.FirstOrDefault()).ToList();
                        docs.Select(x => { x.ApprenticeshipType = Models.Enums.ApprenticeshipType.FrameworkCode; return x; }).ToList();


                        break;
                    }
            }
            return docs;
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
                var result = await client.DeleteDocumentAsync(docUri);

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

        internal static List<string> FormatSearchTerm(string searchTerm)
        {
            Throw.IfNullOrWhiteSpace(searchTerm, nameof(searchTerm));

            var split = searchTerm
                .ToLower()
                .Split(' ')
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            return split;
        }
    }
}
