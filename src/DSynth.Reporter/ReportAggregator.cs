/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DSynth.Reporter
{
    public class ReportAggregator
    {
        /// <summary>
        /// The time to wait to aggregate new individual ReportMetrics
        /// </summary>
        private const int _aggWaitTimeInMs = 5000;
        private const string _infoStartingMessage = "Starting ReportAggregator for provider '{ProviderName}'";
        private const string _infoEndingMessage = "Ending ReportAggregator for provider '{ProviderName}'";
        private ConcurrentDictionary<long, ReportMetricAggregate> _aggReportMetricDict;
        private ConcurrentQueue<ReportMetric> _indvReportMetricsQueue;
        private string _providerName;
        private int _sampleWindowInSeconds;
        private CancellationToken _token;
        private ILogger _logger;

        public ReportAggregator(
            ConcurrentDictionary<long, ReportMetricAggregate> aggReportMetricDict,
            ConcurrentQueue<ReportMetric> indvReportMetricsQueue,
            string providerName,
            int sampleWindowInSeconds,
            CancellationToken token,
            ILogger logger)
        {
            _aggReportMetricDict = aggReportMetricDict;
            _indvReportMetricsQueue = indvReportMetricsQueue;
            _providerName = providerName;
            _sampleWindowInSeconds = sampleWindowInSeconds;
            _token = token;
            _logger = logger;
        }

        public async Task RunAsync()
        {
            _logger.LogInformation(_infoStartingMessage, _providerName);

            while (!_token.IsCancellationRequested)
            {
                while (_indvReportMetricsQueue.TryDequeue(out ReportMetric currentIndvReport))
                {
                    long aggReportId = currentIndvReport.IdAsTotalSeconds;
                    if (_aggReportMetricDict.TryGetValue(aggReportId, out ReportMetricAggregate retrievedAggReport))
                    {
                        retrievedAggReport.AddMetric(currentIndvReport);
                    }
                    else
                    {
                        _aggReportMetricDict[aggReportId] = new ReportMetricAggregate(
                            currentIndvReport, _sampleWindowInSeconds
                        );
                    }
                }

                await Task.Delay(_aggWaitTimeInMs);
            }

            _logger.LogInformation(_infoEndingMessage, _providerName);
        }
    }
}