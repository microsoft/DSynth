/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using DSynth.Sink.Options;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.CosmosDB.BulkExecutor;
using Microsoft.Azure.CosmosDB.BulkExecutor.BulkImport;
using Microsoft.Azure.CosmosDB.BulkExecutor.Graph;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Microsoft.ApplicationInsights;
using DSynth.Common.Models;

namespace DSynth.Sink.Sinks
{
    public class AzureCosmosDbGraph : SinkBase<AzureCosmosDbGraphOptions>
    {
        private string _metricsName = String.Empty;
        private DocumentClient _client;
        private IBulkExecutor _bulkExecutor;
        private List<object> _bulkDocumentsList;
        private event EventHandler _bulkDocumentListFull;
        private long _totalPayloadCount;

        public AzureCosmosDbGraph(string providerName, AzureCosmosDbGraphOptions options, TelemetryClient telemetryClient, ILogger logger, CancellationToken token)
            : base(providerName, options, telemetryClient, logger, token)
        {
            _bulkDocumentsList = new List<object>();

            _metricsName = $"{ProviderName}-{Options.Type}-{Options.Collection}";
            InitializeClient().Wait();
        }

        private async Task InitializeClient()
        {
            ConnectionPolicy connectionPolicy = new ConnectionPolicy
            {
                ConnectionMode = Microsoft.Azure.Documents.Client.ConnectionMode.Direct,
                ConnectionProtocol = Protocol.Tcp
            };

            _client = new DocumentClient(new Uri(Options.Endpoint), Options.AuthorizationKey, connectionPolicy);

            PartitionKeyDefinition partitionKey = new PartitionKeyDefinition
            {
                Paths = new Collection<string> { Options.PartitionKey }
            };

            DocumentCollection documentCollection = new DocumentCollection { Id = Options.Collection, PartitionKey = partitionKey };

            documentCollection = await _client.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri(Options.Database),
                documentCollection,
                new RequestOptions { OfferThroughput = Options.OfferThroughput }).ConfigureAwait(false);

            _bulkExecutor = new GraphBulkExecutor(_client, documentCollection);
            await _bulkExecutor.InitializeAsync().ConfigureAwait(false);

            _bulkDocumentListFull += HandBulkDocumentListFull;
        }

        internal override async Task RunAsync(PayloadPackage payloadPackage)
        {
            await Task.Run(() =>
           {
               _bulkDocumentsList.AddRange(payloadPackage.PayloadAsObjectList);
               _totalPayloadCount += payloadPackage.PayloadCount;

               if (_bulkDocumentsList.Count >= Options.BatchSize)
               {
                   _bulkDocumentListFull?.Invoke(this, EventArgs.Empty);
               }
           });
        }

        private void HandBulkDocumentListFull(object sender, EventArgs e)
        {
            try
            {
                var bulkImportResponse = _bulkExecutor.BulkImportAsync(
                    _bulkDocumentsList,
                    Options.EnableUpsert,
                    Options.DisableAutoIdGeneration,
                    null,
                    Options.MaxInMemorySortingBatchSize,
                    Token).Result;

                SendMetrics(bulkImportResponse);
            }
            catch (Exception)
            {
                RecordFailedSend(_metricsName, _bulkDocumentsList.Count, _totalPayloadCount);
                throw;
            }
            finally
            {
                _totalPayloadCount = 0;
                _bulkDocumentsList.Clear();
            }
        }

        private void SendMetrics(BulkImportResponse bulkImportResponse)
        {
            Metric cosmosMetric = TelemetryClient.GetMetric(_metricsName, $"MetricName");
            cosmosMetric.TrackValue(bulkImportResponse.BadInputDocuments.Count, "BadInputDocuments");
            cosmosMetric.TrackValue(bulkImportResponse.FailedImports.Count, "FailedImports");
            cosmosMetric.TrackValue(bulkImportResponse.NumberOfDocumentsImported, "NumDocumentsImported");
            cosmosMetric.TrackValue(bulkImportResponse.TotalRequestUnitsConsumed, "TotalRequestUnitsConsumed");
            cosmosMetric.TrackValue(bulkImportResponse.TotalTimeTaken.TotalMilliseconds, "TotalTimeTaken");
        }
    }
}