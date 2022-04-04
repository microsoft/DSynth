/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using DSynth.Sink.Options;
using System;
using Azure.Messaging.ServiceBus;
using DSynth.Common.Models;
using Microsoft.ApplicationInsights;

namespace DSynth.Sink.Sinks
{
    public class AzureServiceBus : SinkBase<AzureServiceBusOptions>
    {
        private const string ExUnableToSendMessages = "Unable to send batch messages to Azure Service Bus for provider '{ProviderName}' with the following exception '{ExMessage}'";
        private readonly string _metricsName = String.Empty;
        private ServiceBusSender _serviceBusSender;
        private ServiceBusMessageBatch _messageBatch;
        private EventHandler<ServiceBusMessageBatchFullEventArgs> _serviceBusMessageBatchFull;
        private long _totalPayloadCount;

        public AzureServiceBus(string providerName, AzureServiceBusOptions options, TelemetryClient telemetryClient, ILogger logger, CancellationToken token)
            : base(providerName, options, telemetryClient, logger, token)
        {
            _serviceBusMessageBatchFull += HandleServiceBusMessageBatchFull;

            var client = new ServiceBusClient(Options.ConnectionString);
            _serviceBusSender = client.CreateSender(Options.TopicOrQueueName);

            var mbOptions = new CreateMessageBatchOptions
            {
                MaxSizeInBytes = Options.MaxSizeInBytes
            };

            _messageBatch = _serviceBusSender.CreateMessageBatchAsync(mbOptions, token).GetAwaiter().GetResult();

            _metricsName = $"{ProviderName}-{Options.Type}-{Options.TopicOrQueueName}";
        }

        internal override async Task RunAsync(PayloadPackage payloadPackage)
        {
            await Task.Run(() =>
            {
                ServiceBusMessage message = new ServiceBusMessage(payloadPackage.PayloadAsBytes);
                if (!_messageBatch.TryAddMessage(message))
                {
                    var eventArgs = new ServiceBusMessageBatchFullEventArgs(_messageBatch);

                    _serviceBusMessageBatchFull?.Invoke(this, eventArgs);
                    _messageBatch.TryAddMessage(message);
                    _totalPayloadCount += payloadPackage.PayloadCount;
                }
            });
        }

        private void HandleServiceBusMessageBatchFull(object obj, ServiceBusMessageBatchFullEventArgs e)
        {
            try
            {
                _serviceBusSender.SendMessagesAsync(e.ServiceBusMessageBatch).Wait();
            }
            catch (Exception)
            {
                RecordFailedSend(_metricsName, _messageBatch.Count, _totalPayloadCount);
                throw;
            }
            finally
            {
                _totalPayloadCount = 0;
                e.ServiceBusMessageBatch.Dispose();

                _messageBatch = _serviceBusSender.CreateMessageBatchAsync(
                    new CreateMessageBatchOptions
                    {
                        MaxSizeInBytes = Options.MaxSizeInBytes
                    },
                    Token).GetAwaiter().GetResult();
            }
        }

        private class ServiceBusMessageBatchFullEventArgs
        {
            public ServiceBusMessageBatchFullEventArgs(ServiceBusMessageBatch messageBatch)
            {
                ServiceBusMessageBatch = messageBatch;
            }

            public ServiceBusMessageBatch ServiceBusMessageBatch { get; set; }
        }
    }
}