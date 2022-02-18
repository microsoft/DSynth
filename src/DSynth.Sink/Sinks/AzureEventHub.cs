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

namespace DSynth.Sink.Sinks
{
    public class AzureEventHub : SinkBase<AzureEventHubOptions>
    {
        private const string WarnUnableToSendToEventHub = "Unable to send to Event Hub for provider '{ProviderName}' with the following exception '{ExMessage}'";
        private EventHubClient _client;
        private AzureEventHubOptions _options;
        private event EventHandler _eventDataBatchFull;
        private EventDataBatch _eventDataBatch;

        public AzureEventHub(string providerName, AzureEventHubOptions options, ILogger logger, CancellationToken token)
            : base(providerName, options, logger, token)
        {
            _options = options;
            _eventDataBatch = new EventDataBatch(_options.EventBatchSizeInBytes);
            _eventDataBatchFull += HandleEventDataBatchFull;

            _client = EventHubClient.CreateFromConnectionString(_options.ConnectionString);
            _client.RetryPolicy = RetryPolicy.Default;
        }

        internal override async Task RunAsync(byte[] payload)
        {
            await Task.Run(() =>
            {
                EventData eventData = new EventData(payload);
                if (!_eventDataBatch.TryAdd(eventData))
                {
                    _eventDataBatchFull?.Invoke(this, EventArgs.Empty);
                    _eventDataBatch.TryAdd(eventData);
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
                throw;
            }
            finally
            {
                _eventDataBatch.Dispose();
                _eventDataBatch = new EventDataBatch(_options.EventBatchSizeInBytes);
            }
        }
    }
}