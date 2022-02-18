/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Threading;
using DSynth.Sink;
using DSynth.Provider;
using DSynth.Common.Options;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.ApplicationInsights;
using System.Collections.Generic;

namespace DSynth.Services
{
    public class ProviderPackage
    {
        public IDSynthProvider Provider;
        public List<IDSynthSink> Sinks;
        public DSynthProviderOptions Options;
        private TelemetryClient _telemetryClient;
        private ILogger _logger;

        public ProviderPackage(DSynthProviderOptions options, TelemetryClient telemetryClient, ILogger logger)
        {
            Options = options ?? throw new System.ArgumentNullException(nameof(options));
            _telemetryClient = telemetryClient;
            _logger = logger;
        }

        public static ProviderPackage CreateNew(DSynthProviderOptions options, TelemetryClient telemetryClient, ILogger logger)
        {
            return new ProviderPackage(options, telemetryClient, logger);
        }

        public void Initialize(CancellationToken token)
        {
            _logger.LogInformation(
                Resources.ProviderPackage.InfoInitializeProvider,
                Options.ProviderName,
                Options.Type,
                Options.ToJsonString());

            Sinks = new List<IDSynthSink>();
            Provider = ProviderFactory.GetDSynthProvider(Options, _logger, token);
            Provider.Initialize(token);

            if (Options.IsPushEnabled)
            {
                string sinkOptions = JsonConvert.SerializeObject(Options.Sinks);

                _logger.LogInformation(
                    Resources.ProviderPackage.InfoInitializeSink,
                    Options.ProviderName,
                    sinkOptions);

                foreach (var sink in Options.Sinks)
                {
                    Sinks.Add(SinkFactory.GetDSynthSink(sink, Options.ProviderName, _telemetryClient, _logger, token));
                }
            }
        }
    }
}