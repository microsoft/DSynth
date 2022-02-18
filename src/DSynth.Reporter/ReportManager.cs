/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DSynth.Reporter
{
    public class ReportManager
    {
        private const string _infoInitializingMessage = "Initializing ReportManager for provider '{ProviderName}'";
        private const string _infoFinishedInitializingMessage = "Finished initializing ReportManager for provider '{ProviderName}'";
        private readonly string _providerName;
        private readonly CancellationToken _token;
        private readonly ILogger _logger;
        private ConcurrentDictionary<long, ReportMetricAggregate> _aggReportMetricDict;
        private ConcurrentQueue<ReportMetric> _indvReportMetricsQueue;
        private List<Task> _reportTasks;
        
        /// <summary>
        /// The sample window in seconds to aggregate metrics to, i.e.
        /// if set to 10, we will take metrics for each given 10 second
        /// window and provide percentiles across that window and ship them.
        /// </summary>
        private int _sampleWindowInSeconds = 10;
        private long _currentAggBucketSeconds;
        
        public ReportManager(string providerName, CancellationToken token, ILogger logger)
        {
            _providerName = providerName;
            _token = token;
            _logger = logger;
            Initialize();
        }

        /// <summary>
        /// When we add the metric to the queue, we need to determine the seconds bucket
        /// the metric should be in. This allows us to aggregate metrics on a less granular scale,
        /// reducing the amount of data that gets written. Initially, the _currentAggBucketSeconds
        /// gets set to the current date time offset total unix seconds. From there, we compare
        /// each new metric timestamp with the current bucket. If it falls within the bucket, we
        /// set the metric id to the current bucket, else we increment the bucket by the seconds we
        /// want to aggregate to and set the metric id. Then we add the metric to the queue to be aggregated.
        /// </summary>
        public void AddMetric(ReportMetric metric)
        {
            long currentMetricSeconds = metric.Timestamp.ToUnixTimeSeconds();

            if (currentMetricSeconds < _currentAggBucketSeconds + _sampleWindowInSeconds)
            {
                metric.SetId(_currentAggBucketSeconds);
            }
            else
            {
                _currentAggBucketSeconds += _sampleWindowInSeconds;
                metric.SetId(_currentAggBucketSeconds);
            }

            _indvReportMetricsQueue.Enqueue(metric);
        }

        private void Initialize()
        {
            _logger.LogInformation(_infoInitializingMessage, _providerName);
            _aggReportMetricDict = new ConcurrentDictionary<long, ReportMetricAggregate>();
            _indvReportMetricsQueue = new ConcurrentQueue<ReportMetric>();
            _reportTasks = new List<Task>();
            _currentAggBucketSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            _reportTasks.Add(
                new ReportAggregator(
                    _aggReportMetricDict,
                    _indvReportMetricsQueue,
                    _providerName,
                    _sampleWindowInSeconds,
                    _token,
                    _logger)
                .RunAsync());

            _reportTasks.Add(
                new ReportShipper(
                    _aggReportMetricDict,
                    _providerName,
                    _token,
                    _logger)
                .RunAsync());

            _logger.LogInformation(_infoFinishedInitializingMessage, _providerName);
        }
    }
}