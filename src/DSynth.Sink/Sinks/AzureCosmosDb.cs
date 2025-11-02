/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DSynth.Common.Models;
using DSynth.Sink.Options;
using Microsoft.ApplicationInsights;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace DSynth.Sink.Sinks
{
    public class AzureCosmosDb : SinkBase<AzureCosmosDbOptions>
    {
        private string _metricsName = String.Empty;
        private CosmosClient _cosmosClient;
        private Container _container;
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
            CosmosClientOptions cosmosClientOptions = new CosmosClientOptions
            {
                ConnectionMode = ConnectionMode.Direct,
                AllowBulkExecution = true
            };

            _cosmosClient = new CosmosClient(_options.Endpoint, _options.AuthorizationKey, cosmosClientOptions);

            Database database = await _cosmosClient.CreateDatabaseIfNotExistsAsync(_options.Database).ConfigureAwait(false);

            ContainerProperties containerProperties = new ContainerProperties
            {
                Id = _options.Collection,
                PartitionKeyPath = _options.PartitionKey
            };

            _container = await database.CreateContainerIfNotExistsAsync(
                containerProperties,
                throughput: _options.OfferThroughput).ConfigureAwait(false);

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
                var bulkImportTask = BulkImportAsync(_bulkDocumentsList);
                bulkImportTask.Wait(Token);
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

        private async Task BulkImportAsync(List<string> documents)
        {
            var startTime = DateTime.UtcNow;
            var tasks = new List<Task<ItemResponse<JObject>>>();

            foreach (var documentJson in documents)
            {
                var document = JObject.Parse(documentJson);

                if (_options.EnableUpsert)
                {
                    tasks.Add(_container.UpsertItemAsync(document, cancellationToken: Token));
                }
                else
                {
                    tasks.Add(_container.CreateItemAsync(document, cancellationToken: Token));
                }
            }

            var results = await Task.WhenAll(tasks).ConfigureAwait(false);

            int successCount = results.Length;
            int failureCount = 0; // Tasks that fault won't complete, so we track them separately
            double totalRU = 0;

            foreach (var result in results)
            {
                totalRU += result.RequestCharge;
            }

            var duration = DateTime.UtcNow - startTime;
            SendMetrics(successCount, failureCount, totalRU, duration);
        }

        private void SendMetrics(int successCount, int failureCount, double totalRU, TimeSpan duration)
        {
            Metric cosmosMetric = _telemetryClient.GetMetric(_metricsName, $"MetricName");
            cosmosMetric.TrackValue(failureCount, "FailedImports");
            cosmosMetric.TrackValue(successCount, "NumDocumentsImported");
            cosmosMetric.TrackValue(totalRU, "TotalRequestUnitsConsumed");
            cosmosMetric.TrackValue(duration.TotalMilliseconds, "TotalTimeTaken");
        }
    }
}