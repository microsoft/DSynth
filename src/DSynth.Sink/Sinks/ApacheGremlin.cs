/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
 
using System;
using System.Globalization;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using DSynth.Common.Models;
using DSynth.Sink.Options;
using Gremlin.Net.Driver;
using Gremlin.Net.Structure.IO.GraphSON;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;

namespace DSynth.Sink.Sinks
{
    public class ApacheGremlin : SinkBase<ApacheGremlinOptions>
    {
        private readonly string _containerLinkTemplate = "/dbs/{0}/colls/{1}";
        private string _metricsName = String.Empty;
        private GremlinClient _gremlinClient;

        public ApacheGremlin(string providerName, ApacheGremlinOptions options, TelemetryClient telemetryClient, ILogger logger, CancellationToken token)
            : base(providerName, options, telemetryClient, logger, token)
        {
            InitializeClient();
            _metricsName = $"{ProviderName}-{Options.Type}-{Options.Container}";
        }

        private void InitializeClient()
        {
            try{
            var username = String.Format(CultureInfo.InvariantCulture, _containerLinkTemplate, Options.Database, Options.Container);

            var gremlinServer =
                new GremlinServer(
                    Options.Endpoint,
                    Options.Port,
                    enableSsl: Options.EnableSSL,
                    username: username,
                    password: Options.AuthorizationKey);

            ConnectionPoolSettings connectionPoolSettings = new ConnectionPoolSettings()
            {
                MaxInProcessPerConnection = Options.MaxInProcessPerConnection,
                PoolSize = Options.PoolSize,
                ReconnectionAttempts = Options.ReconnectionAttempts,
                ReconnectionBaseDelay = TimeSpan.FromMilliseconds(Options.ReconnectionBaseDelayMs)
            };

            var webSocketConfiguration =
                new Action<ClientWebSocketOptions>(options =>
                {
                    options.KeepAliveInterval = TimeSpan.FromSeconds(Options.KeepAliveIntervalSec);
                });

            _gremlinClient =
                new GremlinClient(
                    gremlinServer: gremlinServer,
                    
                    messageSerializer: new GraphSON2MessageSerializer(),
                    connectionPoolSettings: connectionPoolSettings,
                    webSocketConfiguration: webSocketConfiguration);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
            }
        }

        internal override async Task RunAsync(PayloadPackage payloadPackage)
        {
            var payloads = payloadPackage.PayloadAsString.Split("\n");

            foreach (var payload in payloads)
            {
                try
                {
                    ResultSet<dynamic> response = await _gremlinClient.SubmitAsync<dynamic>(payload).ConfigureAwait(false);
                    // SendMetrics(response);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        // private void SendMetrics(ResultSet<dynamic> response)
        // {
        //     Metric cosmosMetric = TelemetryClient.GetMetric(_metricsName, $"MetricName");
        //     cosmosMetric.TrackValue(response.BadInputDocuments.Count, "BadInputDocuments");
        //     cosmosMetric.TrackValue(response.FailedImports.Count, "FailedImports");
        //     cosmosMetric.TrackValue(response.NumberOfDocumentsImported, "NumDocumentsImported");
        //     cosmosMetric.TrackValue(response.TotalRequestUnitsConsumed, "TotalRequestUnitsConsumed");
        //     cosmosMetric.TrackValue(response.TotalTimeTaken.TotalMilliseconds, "TotalTimeTaken");
        // }
    }
}