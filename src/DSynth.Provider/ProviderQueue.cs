/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DSynth.Common.Utilities;
using DSynth.Engine;
using DSynth.Common.Options;
using Microsoft.Extensions.Logging;

namespace DSynth.Provider
{
    public class ProviderQueue
    {
        private IList<object> _payloadCollection;
        private readonly DSynthProviderOptions _options;
        private readonly ILogger _logger;
        private readonly CancellationToken _token;
        private readonly Random _random;
        private BlockingCollection<object> _blockingCollection;
        private IDSynthEngine _dsynthEngine;
        private readonly SemaphoreSlim _productionSemaphore;
        private long _totalConsumed = 0;

        public ProviderQueue(IDSynthEngine dSynthEngine, DSynthProviderOptions options, ILogger logger, CancellationToken token)
        {
            _options = options;
            _logger = logger;
            _token = token;
            _random = new Random();
            _payloadCollection = new List<object>();
            _blockingCollection = new BlockingCollection<object>(_options.AdvancedOptions.TargetQueueSize);
            _dsynthEngine = dSynthEngine;

            // Semaphore initialized with target queue size - signals available production slots
            _productionSemaphore = new SemaphoreSlim(_options.AdvancedOptions.TargetQueueSize, _options.AdvancedOptions.TargetQueueSize);

            // Start queue worker tasks - they wait on semaphore signals
            for (int i = 0; i < _options.AdvancedOptions.QueueWorkers; i++)
            {
                Task.Run(() => PopulateCollectionAsync(_dsynthEngine), token);
            }
        }

        public static ProviderQueue CreateNew(IDSynthEngine dSynthEngine, DSynthProviderOptions options, ILogger logger, CancellationToken token)
        {
            return new ProviderQueue(dSynthEngine, options, logger, token);
        }

        private async Task PopulateCollectionAsync(IDSynthEngine dSynthEngine)
        {
            while (!_token.IsCancellationRequested)
            {
                try
                {
                    // Wait for a signal that space is available (blocks until item consumed)
                    await _productionSemaphore.WaitAsync(_token);

                    // Build payload - this happens ONLY when there's space/demand
                    var payload = dSynthEngine.BuildPayload();

                    // Add to queue (should never block since semaphore guarantees space)
                    if (!_blockingCollection.TryAdd(payload, 0, _token))
                    {
                        // Shouldn't happen, but release semaphore if add fails
                        _productionSemaphore.Release();
                    }
                }
                catch (OperationCanceledException)
                {
                    // Normal cancellation, exit gracefully
                    break;
                }
                catch (ObjectDisposedException)
                {
                    // Collection disposed, exit gracefully
                    break;
                }
                catch (Exception ex)
                {
                    var formattedExMessage = ExceptionUtilities.GetFormattedMessage(
                        Resources.ProviderQueue.ExUnableToPopulateCollection,
                        _options.Type,
                        _options.ProviderName);

                    var providerException = new ProviderException(formattedExMessage, ex);
                    _logger.LogError(providerException, providerException.Message);

                    // Release semaphore so production can continue
                    _productionSemaphore.Release();

                    throw providerException;
                }
            }
        }

        private object TryDequeue()
        {
            object ret = new object();

            try
            {
                // Bypass queue for queue size of 1 (direct generation mode)
                if (_options.AdvancedOptions.TargetQueueSize == 1)
                {
                    return _dsynthEngine.BuildPayload();
                }

                // Block until item is available
                if (_blockingCollection.TryTake(out ret, Timeout.Infinite, _token))
                {
                    // Item consumed - signal producers that space is available
                    _productionSemaphore.Release();

                    Interlocked.Increment(ref _totalConsumed);
                }
            }
            catch (OperationCanceledException)
            {
                // Normal cancellation
                return ret;
            }
            catch (ObjectDisposedException ex)
            {
                throw new ProviderException(ex.Message, ex);
            }

            return ret;
        }

        public object Dequeue(out long payloadCount)
        {
            object ret;
            try
            {
                if (_options.MaxBatchSize == 0 && _options.MinBatchSize == 1)
                {
                    payloadCount = 1;
                    ret = TryDequeue();
                }
                else
                {
                    // If the MaxBatchSize = 0, this means that the MaxBatchSize
                    // is disabled and generate a collection the static size of MinBatchSize.
                    payloadCount = _options.MaxBatchSize == 0 ?
                        _options.MinBatchSize : _random.Next(_options.MinBatchSize, _options.MaxBatchSize);

                    _payloadCollection.Clear();

                    // Pre-allocate capacity if known
                    if (_payloadCollection is List<object> list)
                    {
                        list.Capacity = (int)payloadCount;
                    }

                    while (_payloadCollection.Count < payloadCount && !_token.IsCancellationRequested)
                    {
                        _payloadCollection.Add(TryDequeue());
                    }

                    ret = _payloadCollection;
                }
            }
            catch (Exception ex)
            {
                string formattedExMessage = ExceptionUtilities.GetFormattedMessage(
                    Resources.ProviderQueue.ExUnableToGetNextPayload,
                    _options.ProviderName);

                throw new ProviderException(formattedExMessage, ex);
            }

            return ret;
        }
    }
}