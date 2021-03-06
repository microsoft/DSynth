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
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
using Microsoft.ApplicationInsights;
using DSynth.Common.Models;

namespace DSynth.Sink.Sinks
{
    public class AzureCosmosDb : SinkBase<AzureCosmosDbOptions>
    {
        private string _metricsName = String.Empty;
        private DocumentClient _client;
        private IBulkExecutor _bulkExecutor;
        private List<string> _bulkDocumentsList;
        private AzureCosmosDbOptions _options;
        private TelemetryClient _telemetryClient;
        private event EventHandler _bulkDocumentListFull;
        private long _totalPayloadCount;

        public AzureCosmosDb(string providerName, AzureCosmosDbOptions options, TelemetryClient telemetryClient, ILogger logger, CancellationToken token)
            : base(providerName, options, telemetryClient, logger, token)
        {
            _options = options;
            _telemetryClient = telemetryClient;
            _bulkDocumentsList = new List<string>();

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

            _client = new DocumentClient(new Uri(_options.Endpoint), _options.AuthorizationKey, connectionPolicy);

            PartitionKeyDefinition partitionKey = new PartitionKeyDefinition
            {
                Paths = new Collection<string> { _options.PartitionKey }
            };

            DocumentCollection documentCollection = new DocumentCollection { Id = _options.Collection, PartitionKey = partitionKey };

            documentCollection = await _client.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri(_options.Database),
                documentCollection,
                new RequestOptions { OfferThroughput = _options.OfferThroughput }).ConfigureAwait(false);

            _bulkExecutor = new BulkExecutor(_client, documentCollection);
            await _bulkExecutor.InitializeAsync().ConfigureAwait(false);

            _bulkDocumentListFull += HandBulkDocumentListFull;
        }

        internal override async Task RunAsync(PayloadPackage payloadPackage)
        {
            await Task.Run(() =>
           {
               _bulkDocumentsList.Add(payloadPackage.PayloadAsString);
               _totalPayloadCount += payloadPackage.PayloadCount;

               if (_bulkDocumentsList.Count >= _options.BatchSize)
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
                    _options.EnableUpsert,
                    _options.DisableAutoIdGeneration,
                    null,
                    _options.MaxInMemorySortingBatchSize,
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
            Metric cosmosMetric = _telemetryClient.GetMetric(_metricsName, $"MetricName");
            cosmosMetric.TrackValue(bulkImportResponse.BadInputDocuments.Count, "BadInputDocuments");
            cosmosMetric.TrackValue(bulkImportResponse.FailedImports.Count, "FailedImports");
            cosmosMetric.TrackValue(bulkImportResponse.NumberOfDocumentsImported, "NumDocumentsImported");
            cosmosMetric.TrackValue(bulkImportResponse.TotalRequestUnitsConsumed, "TotalRequestUnitsConsumed");
            cosmosMetric.TrackValue(bulkImportResponse.TotalTimeTaken.TotalMilliseconds, "TotalTimeTaken");
        }
    }
}