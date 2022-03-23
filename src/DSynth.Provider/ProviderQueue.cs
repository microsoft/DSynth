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

        public ProviderQueue(IDSynthEngine dSynthEngine, DSynthProviderOptions options, ILogger logger, CancellationToken token)
        {
            _options = options;
            _logger = logger;
            _token = token;
            _random = new Random();
            _payloadCollection = new List<object>();
            _blockingCollection = new BlockingCollection<object>(_options.AdvancedOptions.TargetQueueSize);

            Parallel.For(0, _options.AdvancedOptions.QueueWorkers, index =>
            {
                Task.Run(() => PopulateCollectionAsync(dSynthEngine));
            });
        }

        public static ProviderQueue CreateNew(IDSynthEngine dSynthEngine, DSynthProviderOptions options, ILogger logger, CancellationToken token)
        {
            return new ProviderQueue(dSynthEngine, options, logger, token);
        }

        private Task PopulateCollectionAsync(IDSynthEngine dSynthEngine)
        {
            while (!_token.IsCancellationRequested)
            {
                try
                {
                    _blockingCollection.Add(dSynthEngine.BuildPayload());
                }
                catch (Exception ex)
                {
                    _blockingCollection.Dispose();
                    var formattedExMessage = ExceptionUtilities.GetFormattedMessage(
                        Resources.ProviderQueue.ExUnableToPopulateCollection,
                        _options.Type,
                        _options.ProviderName);

                    var providerException = new ProviderException(formattedExMessage, ex);
                    _logger.LogError(providerException, providerException.Message);
                    throw providerException;
                }
            }

            return Task.CompletedTask;
        }

        private object TryDequeue()
        {
            object ret = new object();

            try
            {
                while (!_blockingCollection.TryTake(out ret) && !_token.IsCancellationRequested)
                {
                    // Slight delay when the blocking collection is empty. We will
                    // backoff slightly to allow the collection to get items, so we
                    // are not constantly spinning on an empty collection.
                    Task.Delay(5).GetAwaiter().GetResult();
                }
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