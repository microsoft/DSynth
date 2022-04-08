/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using DSynth.Sink.Options;
using DSynth.Sink.Sinks;
using Newtonsoft.Json.Linq;
using DSynth.Common.Utilities;
using Microsoft.ApplicationInsights;

namespace DSynth.Sink
{
    public class SinkFactory
    {
        public static IDSynthSink GetDSynthSink(JObject sinkOptions, string providerName, TelemetryClient telemetryClient, ILogger logger, CancellationToken token)
        {
            try
            {
                SinkType type = EnumUtilities.GetEnumValueFromString<SinkType>((string)sinkOptions.SelectToken("type"));

                switch (type)
                {
                    case SinkType.Console:
                        ConsoleOptions consoleSinkOptions = ConsoleOptions.ParseAndValidateOptions<ConsoleOptions>(sinkOptions);
                        return new Sinks.Console(providerName, consoleSinkOptions, telemetryClient, logger, token);

                    case SinkType.HTTP:
                        HttpOptions httpSinkOptions = HttpOptions.ParseAndValidateOptions<HttpOptions>(sinkOptions);
                        return new Http(providerName, httpSinkOptions, telemetryClient, logger, token);

                    case SinkType.File:
                        FileOptions fileSinkOptions = FileOptions.ParseAndValidateOptions<FileOptions>(sinkOptions);
                        return new File(providerName, fileSinkOptions, telemetryClient, logger, token);

                    case SinkType.AzureBlob:
                        AzureBlobOptions azureBlobSinkOptions = AzureBlobOptions.ParseAndValidateOptions<AzureBlobOptions>(sinkOptions);
                        return new AzureBlob(providerName, azureBlobSinkOptions, telemetryClient, logger, token);

                    case SinkType.SocketServer:
                        SocketServerOptions socketServerSinkOptions = SocketServerOptions.ParseAndValidateOptions<SocketServerOptions>(sinkOptions);
                        return new SocketServer(providerName, socketServerSinkOptions, telemetryClient, logger, token);

                    case SinkType.AzureEventHub:
                        AzureEventHubOptions azureEventHubOptions = AzureEventHubOptions.ParseAndValidateOptions<AzureEventHubOptions>(sinkOptions);
                        return new AzureEventHub(providerName, azureEventHubOptions, telemetryClient, logger, token);

                    case SinkType.AzureServiceBus:
                        AzureServiceBusOptions azurServiceBusOptions = AzureServiceBusOptions.ParseAndValidateOptions<AzureServiceBusOptions>(sinkOptions);
                        return new AzureServiceBus(providerName, azurServiceBusOptions, telemetryClient, logger, token);

                    case SinkType.AzureCosmosDb:
                        AzureCosmosDbOptions azureCosmosDbOptions = AzureCosmosDbOptions.ParseAndValidateOptions<AzureCosmosDbOptions>(sinkOptions);
                        return new AzureCosmosDb(providerName, azureCosmosDbOptions, telemetryClient, logger, token);

                    case SinkType.AzureLogAnalytics:
                        AzureLogAnalyticsOptions azureLogAnalyticsOptions = AzureLogAnalyticsOptions.ParseAndValidateOptions<AzureLogAnalyticsOptions>(sinkOptions);
                        return new AzureLogAnalytics(providerName, azureLogAnalyticsOptions, telemetryClient, logger, token);

                    case SinkType.AzureCustomLogs:
                        AzureCustomLogsOptions azureCustomLogsOptions = AzureCustomLogsOptions.ParseAndValidateOptions<AzureCustomLogsOptions>(sinkOptions);
                        return new AzureCustomLogs(providerName, azureCustomLogsOptions, telemetryClient, logger, token);

                    case SinkType.ApacheGremlin:
                        ApacheGremlinOptions apacheGremlinOptions = ApacheGremlinOptions.ParseAndValidateOptions<ApacheGremlinOptions>(sinkOptions);
                        return new ApacheGremlin(providerName, apacheGremlinOptions, telemetryClient, logger, token);
                    
                    default:
                        ConsoleOptions consoleSinkOptionsDef = ConsoleOptions.ParseAndValidateOptions<ConsoleOptions>(sinkOptions);
                        return new Sinks.Console(providerName, consoleSinkOptionsDef, telemetryClient, logger, token);
                }
            }
            catch (Exception ex)
            {
                throw new SinkException(ex.Message, ex);
            }
        }
    }
}