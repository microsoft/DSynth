/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Extensions.Logging;

namespace DSynth.Reporter
{
    public class ReportShipper
    {
        /// <summary>
        /// The time to wait to ship aggregated metrics
        /// </summary>
        private const int _shipWaitTimeInMs = 5000;

        /// <summary>
        /// The working buffer delay, used to allow metrics to finish.
        /// We ignore anything between now and minus 30 seconds as there
        /// are still metrics being added and aggregated
        /// </summary>
        private const int _workingBufferInSeconds = 30;

        private const string _infoStartingMessage = "Starting ReportShipper for provider '{ProviderName}'";
        private const string _infoEndingMessage = "Ending ReportShipper for provider '{ProviderName}'";
        private ConcurrentDictionary<long, ReportMetricAggregate> _aggReportMetricDict;
        private string _providerName;
        private System.Timers.Timer _shipTimer;
        private CancellationToken _token;
        private ILogger _logger;

        public ReportShipper(ConcurrentDictionary<long, ReportMetricAggregate> aggReportMetricDict, string ProviderName, CancellationToken token, ILogger logger)
        {
            _aggReportMetricDict = aggReportMetricDict;
            _providerName = ProviderName;
            _token = token;
            _logger = logger;

            _shipTimer = new System.Timers.Timer(_shipWaitTimeInMs);
            _shipTimer.Elapsed += HandleReports;
        }

        private void HandleReports(object sender, ElapsedEventArgs e)
        {
            IList<ReportMetricAggregate> reportsToShip = GetReportsToShip();

            if (reportsToShip != null)
            {
                foreach (ReportMetricAggregate rm in reportsToShip)
                {
                    if (rm != null)
                    {
                        _logger.LogInformation("Report for provider: {ProviderName}, metric: {MetricName}, values: {ReportValues}", rm.ProviderName, rm.MetricName, rm.GetReportAsJson());
                    }
                }
            }

            if (_token.IsCancellationRequested)
            {
                _shipTimer.Stop();
                _logger.LogInformation(_infoEndingMessage, _providerName);
                return;
            }
        }

        public async Task RunAsync()
        {
            _logger.LogInformation(_infoStartingMessage, _providerName);

            await Task.Run(() =>
            {
                _shipTimer.Start();
            }).ConfigureAwait(false);

        }

        private IList<ReportMetricAggregate> GetReportsToShip()
        {
            long maxReportTimeInSeconds = DateTimeOffset.UtcNow
                .AddSeconds(-_workingBufferInSeconds)
                .ToUnixTimeSeconds();

            long[] reportKeys = _aggReportMetricDict
                .Where(k => k.Key <= maxReportTimeInSeconds)
                .Select(v => v.Key)
                .ToArray();

            IList<ReportMetricAggregate> reportsToShip = null;

            if (reportKeys.Any())
            {
                reportsToShip = new List<ReportMetricAggregate>();
                foreach (long reportKey in reportKeys)
                {
                    ReportMetricAggregate ReportMetric = GetNextReport(reportKey);
                    reportsToShip.Add(ReportMetric);
                }
            }

            return reportsToShip;
        }

        private ReportMetricAggregate GetNextReport(long key)
        {
            _aggReportMetricDict.TryRemove(key, out ReportMetricAggregate ReportMetric);
            return ReportMetric;
        }
    }
}