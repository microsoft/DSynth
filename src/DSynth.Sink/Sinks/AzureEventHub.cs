/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Extensions.Logging;
using DSynth.Sink.Options;
using Microsoft.ApplicationInsights;
using System.Collections.Generic;
using DSynth.Common.Models;
using Microsoft.ApplicationInsights.Metrics;
using Microsoft.ApplicationInsights.DataContracts;
using Newtonsoft.Json;

namespace DSynth.Sink.Sinks
{
    public class AzureEventHub : SinkBase<AzureEventHubOptions>
    {
        private const string WarnUnableToSendToEventHub = "Unable to send to Event Hub for provider '{ProviderName}' with the following exception '{ExMessage}'";
        private readonly string _metricsName = String.Empty;
        private EventHubClient _client;
        private AzureEventHubOptions _options;
        private TelemetryClient _telemetryClient;
        private event EventHandler _eventDataBatchFull;
        private EventDataBatch _eventDataBatch;
        private long _totalPayloadCount;

        public AzureEventHub(string providerName, AzureEventHubOptions options, TelemetryClient telemetryClient, ILogger logger, CancellationToken token)
            : base(providerName, options, telemetryClient, logger, token)
        {
            _options = options;
            _telemetryClient = telemetryClient;
            _eventDataBatch = new EventDataBatch(_options.EventBatchSizeInBytes);
            _eventDataBatchFull += HandleEventDataBatchFull;

            EventHubsConnectionStringBuilder csb = new EventHubsConnectionStringBuilder(_options.ConnectionString)
            {
                OperationTimeout = TimeSpan.FromMilliseconds(_options.OperationTimeoutMs)
            };

            _client = EventHubClient.Create(csb);
            _client.RetryPolicy = RetryPolicy.Default;

            _metricsName = $"{ProviderName}-{Options.Type}-{_client.EventHubName}";
        }

        internal override async Task RunAsync(PayloadPackage payloadPackage)
        {
            await Task.Run(() =>
            {
                EventData eventData = new EventData(payloadPackage.PayloadAsBytes);
                if (!_eventDataBatch.TryAdd(eventData))
                {
                    _eventDataBatchFull?.Invoke(this, EventArgs.Empty);
                    _eventDataBatch.TryAdd(eventData);
                    _totalPayloadCount += payloadPackage.PayloadCount;
                }
                else
                {
                    _totalPayloadCount += payloadPackage.PayloadCount;
                }

            }).ConfigureAwait(false);
        }

        private void HandleEventDataBatchFull(object obj, EventArgs e)
        {
            try
            {
                _client.SendAsync(_eventDataBatch).ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                RecordFailedSend(_metricsName, _eventDataBatch.Count, _totalPayloadCount);
                throw;
            }
            finally
            {
                _totalPayloadCount = 0;
                _eventDataBatch.Dispose();
                _eventDataBatch = new EventDataBatch(_options.EventBatchSizeInBytes);
            }
        }
    }
}