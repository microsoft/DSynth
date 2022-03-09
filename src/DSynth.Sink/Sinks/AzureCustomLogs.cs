/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DSynth.Sink.Options;
using DSynth.Sink.Utilities;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;

namespace DSynth.Sink.Sinks
{
    public class AzureCustomLogs : SinkBase<AzureCustomLogsOptions>
    {
        private const string _debugSendingRequest = "Sending request to http endpoint: {AbsoluteUri}";
        private const string _debugSuccessfulSendRequest = "Successfully sent custom logs request to '{Uri}', status code '{StatusCode}', response message '{ResponseMessage}', response headers '{ResponseHeaders}'";
        private const string _errorUnableToSendRequest = "RunAsync :: Unable to send custom logs request to '{Uri}', status code '{StatusCode}', response message '{ResponseMessage}', response headers '{ResponseHeaders}'";
        private const string _dceUriTemplate = "{0}/dataCollectionRules/{1}/streams/Custom-{2}?api-version={3}";
        private static Lazy<HttpClient> _lazyClient;
        private readonly Uri _customLogEndpoint;
        private BearerRefresher _bearerRefresher;
        private static readonly object _lockObject = new object();

        public AzureCustomLogs(string providerName, AzureCustomLogsOptions sinkOptions, TelemetryClient telemetryClient, ILogger logger, CancellationToken token)
            : base(providerName, sinkOptions, logger, token)
        {
            lock (_lockObject)
            {
                if (_lazyClient == null)
                {
                    _lazyClient = new Lazy<HttpClient>(() =>
                    {
                        var client = new HttpClient();
                        client.Timeout = TimeSpan.FromMilliseconds(Options.RequestTimeoutMs);
                        client.DefaultRequestVersion = new Version(2, 0);

                        return client;
                    });
                }

                _customLogEndpoint = new Uri(String.Format(
                    CultureInfo.InvariantCulture,
                    _dceUriTemplate,
                    Options.DataCollectionEndpoint,
                    Options.DcrImmutableId,
                    Options.CustomTableName,
                    Options.ApiVersion));

                InitializeBearerRefresher();
            }
        }

        private void InitializeBearerRefresher()
        {
            try
            {
                _bearerRefresher = Utilities.BearerRefresher.Initialize(
                    Options.GetBearerOptions(),
                    ProviderName,
                    Logger);

                _bearerRefresher.Start();
            }
            catch (BearerRefresherException)
            {
                throw;
            }
        }

        internal override async Task RunAsync(byte[] payload)
        {
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _customLogEndpoint))
            {
                if (Options.EnableCompression)
                {
                    request.Content = new ByteArrayContent(Compress(payload));
                    request.Content.Headers.ContentEncoding.Add("gzip");
                }
                else
                {
                    request.Content = new ByteArrayContent(payload);
                }

                request.Headers.Add("Authorization", $"Bearer {_bearerRefresher.BearerResponse.AccessToken}");
                request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                Logger.LogDebug(_debugSendingRequest, _customLogEndpoint.AbsoluteUri);
                using (HttpResponseMessage response = await _lazyClient.Value.SendAsync(request).ConfigureAwait(false))
                {
                    string responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    string responseHeadersString = response.Headers.ToString();
                    if (!response.IsSuccessStatusCode)
                    {
                        Logger.LogError(_errorUnableToSendRequest, _customLogEndpoint.AbsoluteUri, (int)response.StatusCode, responseString, responseHeadersString);
                    }
                    else
                    {
                        Logger.LogDebug(_debugSuccessfulSendRequest, _customLogEndpoint.AbsoluteUri, (int)response.StatusCode, responseString, responseHeadersString);
                    }
                }
            }
        }

        private byte[] Compress(byte[] payload)
        {
            using (var memoryStream = new MemoryStream())
            using (var zipStream = new GZipStream(memoryStream, CompressionMode.Compress))
            {
                zipStream.Write(payload, 0, payload.Length);
                zipStream.Flush();
                payload = memoryStream.ToArray();
            }

            return payload;
        }
    }
}