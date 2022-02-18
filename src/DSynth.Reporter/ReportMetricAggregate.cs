/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using DSynth.Common.JsonSerializers;

namespace DSynth.Reporter
{
    public class ReportMetricAggregate
    {
        private readonly int[] _availablePercentiles = new int[] {25, 50, 75, 99};
        private readonly List<long> _sizeInBytes = new List<long>();
        private readonly List<long> _latencyInMs = new List<long>();
        private bool isReportBuilt = false;

        [JsonProperty("providerName")]
        public readonly string ProviderName;

        [JsonProperty("metricName")]
        public readonly string MetricName;

        [JsonProperty("totalRequests")]
        public long TotalRequests;

        [JsonProperty("totalBytes")]
        public long TotalBytes;

        [JsonProperty("timestamp")]
        [JsonConverter(typeof(ISO8601DateTimeConverter))]
        public readonly DateTimeOffset Timestamp;

        [JsonProperty("sizeInBytesPercentiles")]
        public Dictionary<string, double> SizeInBytesPercentiles { get; set; }

        [JsonProperty("runtimeInMsPercentiles")]
        public Dictionary<string, double> RuntimeInMsPercentiles { get; set; }

        [JsonProperty("sampleWindowInSeconds")]
        public int SampleWindowInSeconds { get; set; }

        public ReportMetricAggregate(ReportMetric metric, int sampleWindowInSeconds)
        {
            ProviderName = metric.ProviderName;
            MetricName = metric.MetricName;
            TotalRequests = 1;
            Timestamp = DateTimeOffset.FromUnixTimeSeconds(metric.IdAsTotalSeconds);
            _sizeInBytes.Add(metric.SizeInBytes);
            _latencyInMs.Add(metric.LatencyInMs);
            SampleWindowInSeconds = sampleWindowInSeconds;
        }        

        public void AddMetric(ReportMetric metric)
        {
            _sizeInBytes.Add(metric.SizeInBytes);
            _latencyInMs.Add(metric.LatencyInMs);
            TotalRequests++;
        }

        public string GetReportAsJson()
        {
            BuildReport();
            return JsonConvert.SerializeObject(this);
        }

        public ReportMetricAggregate GetReportAsObject()
        {
            BuildReport();
            return this;
        }

        private void BuildReport()
        {
            if (isReportBuilt)
            {
                return;
            }

            TotalBytes = _sizeInBytes.Sum();
            SizeInBytesPercentiles = BuildPercentileForItem(_sizeInBytes.ToArray());
            RuntimeInMsPercentiles = BuildPercentileForItem(_latencyInMs.ToArray());

            isReportBuilt = true;
        }

        private Dictionary<string, double> BuildPercentileForItem(long[] itemValues)
        {
            Dictionary<string, double> itemPercentiles = new Dictionary<string, double>();

            foreach (int pct in _availablePercentiles)
            {
                string name = $"P{pct}";
                double pctile = (double)pct / 100;
                double percentile = GetPercentile(itemValues, pctile);
                itemPercentiles[name] = percentile;
            }

            return itemPercentiles;
        }

        private double GetPercentile(long[] sequence, double percentile)
        {
            Array.Sort(sequence);
            int N = sequence.Length;
            double n = (N - 1) * percentile + 1;

            if (n == 1d) return sequence[0];
            else if (n == N) return sequence[N - 1];
            else
            {
                int k = (int)n;
                double d = n - k;
                return sequence[k - 1] + d * (sequence[k] - sequence[k - 1]);
            }
        }        
    }
}