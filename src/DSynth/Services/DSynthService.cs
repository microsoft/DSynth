/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DSynth.Provider.Extensions;
using DSynth.Common.Options;
using Microsoft.Extensions.Logging;
using DSynth.Common.Models;
using DSynth.Common.Extensions;
using DSynth.Services.Extensions;
using DSynth.Options;
using System.Linq;
using DSynth.Common.Utilities;
using DSynth.Models;
using Microsoft.ApplicationInsights;
using System.Runtime;
using Microsoft.Extensions.Hosting;
using Microsoft.ApplicationInsights.DataContracts;
using System.Diagnostics;
using Newtonsoft.Json;

namespace DSynth.Services
{
    public class DSynthService : IDSynthService, IHostedService
    {
        private List<Task> _providerTasks;
        private DSynthSettings _settings;
        private TelemetryClient _telemetryClient;
        private ILogger<DSynthService> _logger;
        private CancellationTokenSource _tokenSource;
        private CancellationToken _token;
        private DSynthStatus _dSynthStatus;
        private IHostApplicationLifetime _applicationLifetime;
        private bool _isHeadless = false;

        public static ConcurrentDictionary<string, ProviderPackage> DSynthPackageDict { get; }
            = new ConcurrentDictionary<string, ProviderPackage>();

        public DSynthService(TelemetryClient telemetryClient, IHostApplicationLifetime applicationLifetime, ILogger<DSynthService> logger)
        {
            _telemetryClient = telemetryClient;
            _logger = logger;
            _providerTasks = new List<Task>();
            _dSynthStatus = new DSynthStatus();
            _applicationLifetime = applicationLifetime;

            telemetryClient.TrackEvent(new EventTelemetry("DSynthAppStart"));
            logger.LogInformation("DSynth Application Starting...");

            _applicationLifetime.ApplicationStopping.Register(() =>
            {
                telemetryClient.TrackEvent(new EventTelemetry("DSynthAppStop"));
                logger.LogInformation("DSynth Application Stopping...");
                telemetryClient.Flush();
                Task.Delay(5000).Wait();
            });
        }

        public async Task StartAsync()
        {
            // If tasks exist, this means DSynth is running and we just return
            if (_providerTasks.Any()) return;

            // Initialize cancellation token
            _tokenSource = new CancellationTokenSource();
            _token = _tokenSource.Token;

            IList<DSynthProviderOptions> providersOptions = new List<DSynthProviderOptions>();
            try
            {
                // Load DSynth settings
                JsonFileContents jsonFileContents = await JsonUtilities.ReadFileAsync(Resources.DSynthService.DSynthSettingsFile)
                    .ConfigureAwait(false);

                _settings = DSynthSettings.ParseAndValidateSettings(jsonFileContents.JObjectContents);

                // Load provider options
                jsonFileContents = await JsonUtilities.ReadFileAsync(_settings.FullProfilePath)
                    .ConfigureAwait(false);

                providersOptions = DSynthProviderOptions.ParseAndValidateOptions(jsonFileContents.JObjectContents, _logger);

                _dSynthStatus.Start();
                _dSynthStatus.SetRunningProfile(_settings.ProvidersFile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw new DSynthServiceException(ex.Message, ex);
            }

            _logger.LogInformation(
                Resources.ProviderPackage.InfoStartingProviders,
                providersOptions.GetProviderNames());

            // Create provider packages based on all providers retrieved from configuration
            var result = Parallel.ForEach(providersOptions, (providerOptions) =>
            {
                try
                {
                    var providerPackage = ProviderPackage.CreateNew(providerOptions, _telemetryClient, _logger);
                    providerPackage.Initialize(_token);
                    DSynthPackageDict.TryAdd(providerOptions.ProviderName, providerPackage);

                    if (providerPackage.Options.IsPushEnabled)
                    {
                        _providerTasks.Add(Task.Run(() =>
                        {
                            ProviderSinkTask(providerPackage, providerOptions.MaxIterations, _token);
                        }));
                    }
                }
                catch (DSynth.Provider.ProviderException ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
                catch (DSynth.Sink.SinkException ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
                catch (AggregateException ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            });
        }

        private Task ProviderSinkTask(ProviderPackage package, int maxIterations, CancellationToken token)
        {
            // Check to see if maxIterations > 0 and if so, set it to 0 to start tracking iterations.
            // Else we set it to -1 to keep sending infinitely, until DSynth is stopped.
            int iterationCount = maxIterations > 0 ? 0 : -1;
            while (!token.IsCancellationRequested && package.Options.IsPushEnabled && iterationCount < maxIterations)
            {
                try
                {
                    var sw = Stopwatch.StartNew();
                    PayloadPackage PayloadPackage = package.Provider.Package;
                    _logger.LogTrace(Resources.DSynthService.TraceRetrievingPayloadPackage, sw.ElapsedMilliseconds);

                    if (PayloadPackage.PayloadAsBytes.Length > 4)
                    {
                        Task.WhenAll(package.Sinks.Select(i => i.SendPayloadAsync(PayloadPackage))).GetAwaiter().GetResult();
                        iterationCount = maxIterations > 0 ? ++iterationCount : -1;
                    }
                }
                catch (DSynth.Sink.SinkException ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
                catch (DSynth.Provider.ProviderException ex)
                {
                    _logger.LogError(ex, ex.Message);
                    throw;
                }

                Task.Delay(package.Options.IntervalInMs).Wait();
            }

            // Log event when we complete sending payloads to the provider's sink
            _telemetryClient.TrackEvent("ProviderSinkTaskComplete", new Dictionary<string, string> {
                {"ProviderName",package.Options.ProviderName},
                {"CancellationRequested",token.IsCancellationRequested.ToString()},
                {"IsPushEnabled",package.Options.IsPushEnabled.ToString()},
                {"MaxIterations", maxIterations.ToString()},
                {"IterationCount", iterationCount.ToString()},
                {"DSynthStatus", JsonConvert.SerializeObject(_dSynthStatus)}});

            if (_isHeadless && package.Options.TerminateWhenComplete) { _applicationLifetime.StopApplication(); }

            // If the provider gets disabled, we want to make sure we are not spamming
            // the while loop and set a default longer check duration. The next config
            // update will reset this value back to the desired interval.
            package.Options.IntervalInMs = package.Options.AdvancedOptions.PushDisabledIntervalInMs;

            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            _logger.LogInformation(
                    Resources.ProviderPackage.InfoStoppingProviders,
                    DSynthPackageDict.GetProviderNames());

            _tokenSource.Cancel();
            _providerTasks = new List<Task>();
            DSynthPackageDict.Clear();
            _dSynthStatus.Stop();

            // Force garbage collection when DSynth stops. Since the application keeps running,
            // memory stays allocated. This will help reclaim memory once DSynth stops.
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect();

            return Task.CompletedTask;
        }

        public async Task RestartAsync()
        {
            _logger.LogInformation(Resources.DSynthService.InfoRestartingDSynth);

            await StopAsync().ConfigureAwait(false);
            await Task.Delay(Resources.DSynthService.RestartDelayInMs).ConfigureAwait(false);
            await StartAsync().ConfigureAwait(false);
        }

        public IEnumerable<DSynthProviderOptions> GetProvidersOptions()
        {
            return DSynthPackageDict.GetProvidersOptionsAsJson();
        }

        public async Task<List<DSynthProviderOptions>> UpdateProvidersOptionsAsync(List<DSynthProviderOptions> updatedProvidersOptions)
        {
            await DSynthPackageDict.UpdateProvidersOptionsAsync(updatedProvidersOptions, _settings.ProvidersFile, _logger)
                .ConfigureAwait(false);

            return updatedProvidersOptions;
        }

        public object GetProviderNames()
        {
            object ret = new
            {
                providerNames = DSynthPackageDict.Select(x => x.Key).ToArray()
            };

            return ret;
        }

        public PayloadPackage GetNextPayload(string providerName)
        {
            PayloadPackage ret = new PayloadPackage();
            if (DSynthPackageDict.TryGetValue(providerName, out ProviderPackage providerPackage))
            {
                ret = providerPackage.Provider.Package;
            }

            return ret;
        }

        public DSynthStatus GetStatus()
        {
            return _dSynthStatus;
        }

        /// <summary>
        /// Headless entry point
        /// </summary>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _isHeadless = true;
            _logger.LogInformation("Starting DSynth headless...");
            await StartAsync();
        }

        /// <summary>
        /// Headless exit point
        /// </summary>
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await StopAsync();
        }
    }
}