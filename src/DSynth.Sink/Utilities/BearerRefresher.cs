/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Globalization;
using System.Net.Http;
using System.Timers;
using System.Web;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DSynth.Sink.Utilities
{
    public class BearerRefresher
    {
        private const string _uriTemplate = "https://{0}/{1}/oauth2/v2.0/token";
        private const string _bodyTemplate = "client_id={0}&scope={1}&client_secret={2}&grant_type=client_credentials";
        private const string _exUnableToGetBearer = "OnBearerExpired :: Unable to get bearer token from '{0}' for provider '{1}' response '{2}'";
        private const string _debugBearerRefresh = "Getting bearer token from {Uri}, for provider {ProviderName}";
        private const string _debugSendingRequest = "Sending request to http endpoint '{AbsoluteUri}', for provider '{ProviderName}'";
        private const string _debugSuccessfulBearerRequest = "Successfully sent bearer request to '{Uri}', status code '{StatusCode}' for provider '{ProviderName}'";
        private static HttpClient _client = new HttpClient();
        private Timer _bearerExpTimer;
        private string _requestPayload;
        private Uri _uri;
        private int _expBufferSeconds;
        private string _providerName;
        private ILogger _logger;
        private BearerResponse _bearerResponse { get; set; }
        public BearerResponse BearerResponse { get { return _bearerResponse; } }

        private BearerRefresher(BearerOptions bearerOptions, string providerName, ILogger logger)
        {
            _expBufferSeconds = bearerOptions.ExpBufferSeconds;
            _providerName = providerName;
            _logger = logger;

            var encodedBearerScope = HttpUtility.UrlEncode(bearerOptions.Scope);
            _requestPayload = String.Format(
                    CultureInfo.InvariantCulture,
                    _bodyTemplate,
                    bearerOptions.AppId,
                    encodedBearerScope,
                    bearerOptions.AppSecret);

            _uri = new Uri(String.Format(
                CultureInfo.InstalledUICulture,
                _uriTemplate,
                bearerOptions.AzureManagemenrDns,
                bearerOptions.TenantId)
            );
        }

        public static BearerRefresher Initialize(BearerOptions bearerOptions, string providerName, ILogger logger)
        {
            var bearerRefresher = new BearerRefresher(bearerOptions, providerName, logger);

            return bearerRefresher;
        }

        public void Start()
        {
            try
            {
                OnBearerExpired(null, null);
                var bearerRefreshIntervalMs = (BearerResponse.ExpiresIn - _expBufferSeconds) * 1000;
                _bearerExpTimer = new System.Timers.Timer(bearerRefreshIntervalMs);
                _bearerExpTimer.Elapsed += OnBearerExpired;
                _bearerExpTimer.Start();
            }
            catch (Exception ex)
            {
                throw new SinkException(ex.Message, ex);
            }
        }

        private void OnBearerExpired(object sender, ElapsedEventArgs e)
        {
            _logger?.LogDebug(_debugBearerRefresh, _uri, _providerName);
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _uri))
            {
                request.Content = new StringContent(_requestPayload);
                request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");

                _logger?.LogDebug(_debugSendingRequest, _uri.AbsoluteUri, _providerName);
                using (HttpResponseMessage response = _client.SendAsync(request).GetAwaiter().GetResult())
                {
                    string responseString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    string responseHeadersString = response.Headers.ToString();

                    if (!response.IsSuccessStatusCode)
                    {
                        string exMessage = DSynth.Common.Utilities.ExceptionUtilities.GetFormattedMessage(
                            _exUnableToGetBearer,
                            _uri.AbsoluteUri,
                            responseString,
                            _providerName);

                        throw new BearerRefresherException(exMessage);
                    }

                    _logger?.LogDebug(_debugSuccessfulBearerRequest, _uri.AbsoluteUri, (int)response.StatusCode, _providerName);
                    _bearerResponse = JsonConvert.DeserializeObject<BearerResponse>(responseString);
                }
            }
        }
    }
}