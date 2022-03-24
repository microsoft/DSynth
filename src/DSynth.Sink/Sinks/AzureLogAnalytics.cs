/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Globalization;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using DSynth.Common.Models;
using DSynth.Sink.Options;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;

namespace DSynth.Sink.Sinks
{
    public class AzureLogAnalytics : SinkBase<AzureLogAnalyticsOptions>
    {
        private const string _debugSendingPayloadMessage = "Sending to http endpoint: {AbsoluteUri}";
        private const string _debugSuccessfulSendRequest = "Successfully sent Log Analytics request to '{Uri}', status code '{StatusCode}', response message '{ResponseMessage}'";
        private const string _errorUnableToSendRequest = "RunAsync :: Unable to send Log Analytics request to '{Uri}', status code '{StatusCode}', response message '{ResponseMessage}'";
        private const string _endpointTemplate = "https://{0}.{1}/api/logs?api-version={2}";
        private const string _signatureStringTemplate = "POST\n{0}\napplication/json\nx-ms-date:{1}\n/api/logs";
        private readonly string _metricsName = String.Empty;
        private static Lazy<HttpClient> _lazyClient;
        private readonly Uri _workspaceEndpoint;
        private static readonly object _lockObject = new object();

        public AzureLogAnalytics(string providerName, AzureLogAnalyticsOptions sinkOptions, TelemetryClient telemetryClient, ILogger logger, CancellationToken token)
            : base(providerName, sinkOptions, telemetryClient, logger, token)
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

                _workspaceEndpoint = new Uri(String.Format(
                    CultureInfo.InvariantCulture,
                    _endpointTemplate,
                    Options.WorkspaceId,
                    Options.DnsSuffix,
                    Options.ApiVersion));

                _metricsName = $"{ProviderName}-{Options.Type}-{Options.LogType}";
            }
        }

        internal override async Task RunAsync(PayloadPackage payloadPackage)
        {
            byte[] payload = payloadPackage.PayloadAsBytes;
            var rfcDate = DateTime.UtcNow.ToString("r");
            string signatureString = String.Format(CultureInfo.InvariantCulture, _signatureStringTemplate, payload.Length, rfcDate);
            string hashedString = BuildSignature(signatureString, Options.SharedKey);
            string signature = $"SharedKey {Options.WorkspaceId}:{hashedString}";

            try
            {
                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _workspaceEndpoint))
                {
                    request.Headers.Add("Accept", "application/json");
                    request.Headers.Add("Log-Type", Options.LogType);
                    request.Headers.Add("Authorization", signature);
                    request.Headers.Add("x-ms-date", rfcDate);
                    request.Headers.Add("time-generated-field", Options.TimestampField);
                    request.Content = new ByteArrayContent(payload);
                    request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                    Logger.LogDebug(_debugSendingPayloadMessage, _workspaceEndpoint.AbsoluteUri);
                    using (HttpResponseMessage response = await _lazyClient.Value.SendAsync(request))
                    {
                        var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        if (!response.IsSuccessStatusCode)
                        {
                            RecordSentMetrics(_metricsName, payloadPackage.PayloadCount, payloadPackage.PayloadCount, false);
                            Logger.LogError(_errorUnableToSendRequest, _workspaceEndpoint.AbsoluteUri, (int)response.StatusCode, responseString);
                        }
                        else
                        {
                            RecordSentMetrics(_metricsName, payloadPackage.PayloadCount, payloadPackage.PayloadCount, true);
                            Logger.LogDebug(_debugSuccessfulSendRequest, _workspaceEndpoint.AbsoluteUri, (int)response.StatusCode, responseString);
                        }
                    }
                }
            }
            catch (Exception)
            {
                RecordSentMetrics(_metricsName, payloadPackage.PayloadCount, payloadPackage.PayloadCount, false);
                throw;
            }
        }

        private static string BuildSignature(string message, string secret)
        {
            var encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = Convert.FromBase64String(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hash = hmacsha256.ComputeHash(messageBytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}