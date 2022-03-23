/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using DSynth.Sink.Options;
using System.Text;
using DSynth.Common.Models;
using Microsoft.ApplicationInsights;
using System;

namespace DSynth.Sink.Sinks
{
    public class Console : SinkBase<ConsoleOptions>
    {
        private readonly string _metricsName = String.Empty;
        public Console(string providerName, ConsoleOptions options, TelemetryClient telemetryClient, ILogger logger, CancellationToken token)
            : base(providerName, options, telemetryClient, logger, token)
        {
            _metricsName = $"{ProviderName}-{Options.Type}";
        }

        internal override async Task RunAsync(PayloadPackage payloadPackage)
        {
            byte[] payload = payloadPackage.PayloadAsBytes;

            if (OptionsOverrides.TryGetValue(Resources.SinkBase.HeaderKey, out string header))
            {
                payload = Encoding.UTF8.GetBytes(header).Concat(payload).ToArray();
            }

            string stringPayload = Encoding.UTF8.GetString(payload);
            if (Options.WriteToLog)
            {
                await Task.Run(() => Logger.LogInformation(stringPayload)).ConfigureAwait(false);
                RecordSentMetrics(_metricsName, payloadPackage.PayloadCount, payloadPackage.PayloadCount, true);
            }
            if (Options.WriteToConsole)
            {
                await Task.Run(() => System.Console.WriteLine($"{stringPayload}\n")).ConfigureAwait(false);
            }
        }
    }
}