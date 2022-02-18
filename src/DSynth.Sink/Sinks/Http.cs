/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using DSynth.Sink.Options;
using System.Net.Http.Headers;

namespace DSynth.Sink.Sinks
{
    public class Http : SinkBase<HttpOptions>
    {
        private const string _debugSendingPayloadMessage = "Sending to http endpoint: {AbsoluteUri}";
        private const string _debugSuccessfulSendRequest = "Successfully sent HTTP request to '{Uri}', status code '{StatusCode}', response message '{ResponseMessage}'";
        private const string _errorUnableToSendRequest = "RunAsync :: Unable to send HTTP request to '{Uri}', status code '{StatusCode}', response message '{ResponseMessage}'";
        private readonly Lazy<HttpClient> _lazyClient;

        public Http(string providerName, HttpOptions options, ILogger logger, CancellationToken token)
            : base(providerName, options, logger, token)
        {
            _lazyClient = new Lazy<HttpClient>(() =>
            {
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromMilliseconds(Options.RequestTimeoutMs);
                client.DefaultRequestVersion = new Version(2, 0);

                return client;
            });
        }

        internal override async Task RunAsync(byte[] payload)
        {
            if (OptionsOverrides.TryGetValue(Resources.SinkBase.HeaderKey, out string header))
            {
                payload = Encoding.UTF8.GetBytes(header).Concat(payload).ToArray();
            }

            Uri fullUri = new UriBuilder(
                Options.EndpointScheme, Options.EndpointDns, Options.EndpointPort, Options.EndpointPath).Uri;

            Logger.LogDebug(_debugSendingPayloadMessage, fullUri.AbsoluteUri);

            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, fullUri))
            {
                request.Content = new ByteArrayContent(payload);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue(Options.MimeType);

                using (HttpResponseMessage response = await _lazyClient.Value.SendAsync(request).ConfigureAwait(false))
                {
                    string responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    if (!response.IsSuccessStatusCode)
                    {
                        Logger.LogError(_errorUnableToSendRequest, fullUri.AbsoluteUri, (int)response.StatusCode, responseString);
                    }
                    else
                    {
                        Logger.LogDebug(_debugSuccessfulSendRequest, fullUri.AbsoluteUri, (int)response.StatusCode, responseString);
                    }
                }
            }
        }
    }
}