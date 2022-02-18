/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Diagnostics;

namespace DSynth.Reporter
{
    public struct ReportMetric
    {
        public string ProviderName { get; set; }
        public string MetricName { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public long IdAsTotalSeconds { get; set; }
        public long SizeInBytes { get; set; }
        public long LatencyInMs { get; set; }
        private Stopwatch _stopwatch;

        public static ReportMetric StartNew(string providerName, string metricName, long sizeInBytes)
        {
            ReportMetric r = new ReportMetric();
            r.ProviderName = providerName;
            r.MetricName = metricName;
            r.SizeInBytes = sizeInBytes;
            r._stopwatch = Stopwatch.StartNew();
            return r;
        }

        public void End()
        {
            Timestamp = DateTimeOffset.UtcNow;
            LatencyInMs = _stopwatch.ElapsedMilliseconds;
        }

        public void SetId(long secondsBucket)
        {
            IdAsTotalSeconds = secondsBucket;
        }
    }
}