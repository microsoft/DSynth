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

namespace DSynth.Sink.Sinks
{
    public class AzureServiceBus : SinkBase<AzureServiceBusOptions>
    {
        private const string ExUnableToSendMessages = "Unable to send batch messages to Azure Service Bus for provider '{ProviderName}' with the following exception '{ExMessage}'";
        private AzureServiceBusOptions _options;
        private ServiceBusSender _serviceBusSender;
        private ServiceBusMessageBatch _messageBatch;
        private EventHandler<ServiceBusMessageBatchFullEventArgs> _serviceBusMessageBatchFull;

        public AzureServiceBus(string providerName, AzureServiceBusOptions options, ILogger logger, CancellationToken token)
            : base(providerName, options, logger, token)
        {
            _options = options;
            _serviceBusMessageBatchFull += HandleServiceBusMessageBatchFull;

            var client = new ServiceBusClient(_options.ConnectionString);
            _serviceBusSender = client.CreateSender(_options.TopicOrQueueName);

            var mbOptions = new CreateMessageBatchOptions
            {
                MaxSizeInBytes = _options.MaxSizeInBytes
            };

            _messageBatch = _serviceBusSender.CreateMessageBatchAsync(mbOptions, token).GetAwaiter().GetResult();
        }

        internal override async Task RunAsync(byte[] payload)
        {
            await Task.Run(() =>
            {
                ServiceBusMessage message = new ServiceBusMessage(payload);
                if (!_messageBatch.TryAddMessage(message))
                {
                    var eventArgs = new ServiceBusMessageBatchFullEventArgs(_messageBatch);
                    RaiseServiceBusMessageBatchFull(eventArgs);



                    _messageBatch.TryAddMessage(message);
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
                throw;
            }
            finally
            {
                e.ServiceBusMessageBatch.Dispose();

                _messageBatch = _serviceBusSender.CreateMessageBatchAsync(
                    new CreateMessageBatchOptions
                    {
                        MaxSizeInBytes = _options.MaxSizeInBytes
                    },
                    Token).GetAwaiter().GetResult();
            }
        }

        private void RaiseServiceBusMessageBatchFull(ServiceBusMessageBatchFullEventArgs e)
        {
            EventHandler<ServiceBusMessageBatchFullEventArgs> handler = _serviceBusMessageBatchFull;
            if (handler != null)
            {
                handler(this, e);
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