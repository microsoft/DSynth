/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using DSynth.Reporter;
using DSynth.Common.Utilities;
using DSynth.Common.Models;
using Microsoft.ApplicationInsights;
using DSynth.Sink.Options;

namespace DSynth.Sink.Sinks
{
    public abstract class SinkBase<T> : IDSynthSink where T : class
    {
        internal ILogger Logger;
        internal string ProviderName;
        internal T Options;

        public TelemetryClient TelemetryClient { get; }

        internal IDictionary<string, string> OptionsOverrides;
        internal readonly CancellationToken Token;
        private readonly ReportManager _reportManager;
        private readonly string _metricName;

        public SinkBase(string providerName, T sinkOptions, TelemetryClient telemetryClient, ILogger logger, CancellationToken token)
        {
            ProviderName = providerName;
            Options = sinkOptions;
            TelemetryClient = telemetryClient;
            Logger = logger;
            Token = token;
            OptionsOverrides = new Dictionary<string, string>();
            _reportManager = new ReportManager(ProviderName, Token, Logger);
            _metricName = this.GetType().Name;
        }

        internal abstract Task RunAsync(PayloadPackage payloadPackage);

        public async Task SendPayloadAsync(PayloadPackage payloadPackage)
        {
            ReportMetric reportMetric = ReportMetric.StartNew(ProviderName, _metricName, payloadPackage.PayloadAsBytes.LongLength, payloadPackage.PayloadCount);
            if (payloadPackage.Overrides != null) OptionsOverrides = new Dictionary<string, string>(payloadPackage.Overrides);

            try
            {
                await RunAsync(payloadPackage).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                string formattedExMessage = ExceptionUtilities.GetFormattedMessage(
                    Resources.SinkBase.ExUnableToSendToSink,
                    this.GetType().Name,
                    ProviderName,
                    Options.ToString(),
                    ex.Message
                );

                throw new SinkException(formattedExMessage, ex);
            }
            finally
            {
                reportMetric.End();
                _reportManager.AddMetric(reportMetric);
                OptionsOverrides?.Clear();
            }
        }

        internal string PathTemplateReplacement(string template)
        {
            var timestampArray = DateTimeOffset.UtcNow
                .ToString(Resources.SinkBase.DateTimeTemplateFormat)
                .Split(Resources.SinkBase.DateTimeTemplateDelimeter);

            return template?
                .Replace(Resources.SinkBase.YearToken, timestampArray[0])
                .Replace(Resources.SinkBase.MonthToken, timestampArray[1])
                .Replace(Resources.SinkBase.DayToken, timestampArray[2])
                .Replace(Resources.SinkBase.HourToken, timestampArray[3])
                .Replace(Resources.SinkBase.MinuteToken, timestampArray[4])
                .Replace(Resources.SinkBase.SecondToken, timestampArray[5])
                .Replace(Resources.SinkBase.MillisecondToken, timestampArray[6]);
        }

        internal void RecordFailedSend(string metricsName, long totalSendCount, long totalPayloadCount)
        {
            TelemetryClient.TrackEvent(metricsName,
                new Dictionary<string, string> {
                    { "IsSuccess", $"false" },
                    { "ProviderName", ProviderName } },
                new Dictionary<string, double> {
                    { "TotalSendCount", totalSendCount },
                    { "TotalPayloadCount", totalPayloadCount } });
        }
    }
}