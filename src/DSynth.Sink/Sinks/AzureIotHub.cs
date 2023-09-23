/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using DSynth.Sink.Options;
using DSynth.Common.Models;
using Microsoft.ApplicationInsights;
using Microsoft.Azure.Devices.Client;
using System.Collections.Generic;
using System.Text;

namespace DSynth.Sink.Sinks
{
    public class AzureIotHub : SinkBase<AzureIotHubOptions>
    {
        private readonly string _metricsName;
        private DeviceClient _client;
        private System.Timers.Timer _queueFlushTimer;
        private static List<Message> _iotHubMessages;
        private event EventHandler _messageBatchFull;
        private long _trackedBatchSizeBytes;
        private long _totalPayloadCount;
        private static object _lockObject = new object();
        private SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

        public AzureIotHub(string providerName, AzureIotHubOptions sinkOptions, TelemetryClient telemetryClient, ILogger logger, CancellationToken token)
            : base(providerName, sinkOptions, telemetryClient, logger, token)
        {
            _client = DeviceClient.CreateFromConnectionString(Options.DeviceConnectionString, TransportType.Amqp_Tcp_Only);
            _iotHubMessages = new List<Message>();
            _queueFlushTimer = new System.Timers.Timer(Options.BatchFlushIntervalMiliSec);
            DetermineSendStrategy();
            _metricsName = $"{ProviderName}-{Options.Type}-{Options.DeviceId}";
        }

        private void DetermineSendStrategy()
        {
            if (Options.BatchFlushIntervalMiliSec <= 0 && Options.BatchSizeInBytes <= 0)
            {
                _queueFlushTimer.Enabled = false;
                return;
            }

            if (Options.BatchFlushIntervalMiliSec >= 0)
            {
                _queueFlushTimer.Elapsed += HandleMessageBatchFull;
                _queueFlushTimer.Enabled = true;
                _queueFlushTimer.Start();
            }

            if (Options.BatchSizeInBytes >= 0)
            {
                _messageBatchFull += HandleMessageBatchFull;
            }
        }

        internal override async Task RunAsync(PayloadPackage payloadPackage)
        {
            // If both options are 0, we will send the messages as non batch, 1 at a time.
            if (Options.BatchSizeInBytes <= 0 && Options.BatchFlushIntervalMiliSec <= 0)
            {
                await HandleSingleMessage(new Message(payloadPackage.PayloadAsBytes)).ConfigureAwait(false);
                return;
            }

            // When we specify either batch size or batch interval, we will process as batch.

            await _semaphoreSlim.WaitAsync().ConfigureAwait(false);
            await Task.Run(() =>
            {
                if (!TryAdd(payloadPackage.PayloadAsBytes))
                {
                    _messageBatchFull?.Invoke(this, EventArgs.Empty);
                    TryAdd(payloadPackage.PayloadAsBytes);
                }
                else
                {
                    _totalPayloadCount += payloadPackage.PayloadCount;
                }
            }).ConfigureAwait(false);

            _semaphoreSlim.Release();
        }

        private bool TryAdd(byte[] messageBytes)
        {
            // _semaphoreSlim.Wait();
            int batchSizeBytesLimit = Options.BatchSizeInBytes;
            int incommingMessageSize = messageBytes.Length;
            if (_trackedBatchSizeBytes + incommingMessageSize >= batchSizeBytesLimit && batchSizeBytesLimit > 0)
            {
                return false;
            }

            _trackedBatchSizeBytes += incommingMessageSize;
            using var eventMessage = new Message(messageBytes)
            {
                ContentEncoding = Encoding.UTF8.ToString(),
                ContentType = "application/json",
            };
            _iotHubMessages.Add(new Message(messageBytes));

            // _semaphoreSlim.Release();

            return true;
        }

        private async Task HandleSingleMessage(Message message)
        {
            await _client.SendEventAsync(message).ConfigureAwait(false);
        }

        private void HandleMessageBatchFull(object sender, EventArgs e)
        {
            _semaphoreSlim.Wait();
            // lock (_lockObject)
            // {
            // StopIntervalTimer();

            if (!_iotHubMessages.Any())
            {
                // StartIntervalTimer();
                return;
            }

            try
            {
                _client.SendEventBatchAsync(_iotHubMessages).GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                RecordFailedSend(_metricsName, _iotHubMessages.Count, _totalPayloadCount);
                throw;
            }
            finally
            {
                _totalPayloadCount = 0;
                _iotHubMessages.Clear();
                // StartIntervalTimer();
            }
            _semaphoreSlim.Release();
            // }
        }

        private void StopIntervalTimer()
        {
            if (!_queueFlushTimer.Enabled)
            {
                return;
            }

            _queueFlushTimer.Stop();
        }

        private void StartIntervalTimer()
        {
            if (!_queueFlushTimer.Enabled)
            {
                return;
            }

            _queueFlushTimer.Start();
        }
    }
}