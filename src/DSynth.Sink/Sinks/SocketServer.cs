/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/


using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using DSynth.Sink.Options;
using DSynth.Common.Models;
using Microsoft.ApplicationInsights;

namespace DSynth.Sink.Sinks
{
    public class SocketServer : SinkBase<SocketServerOptions>
    {
        private const string _infoSocketServerStarted = "Starting socket server for provider '{ProviderName}' listening on port '{Port}'";
        private const string _infoClientConnected = "Client connected for provider '{ProviderName}' on port '{Port}'";
        private const string _warnNoClientsConnected = "No clients connected to socket server for provider {ProviderName}";
        private const string _warnLostConnection = "The connected client is no longer connected for provider {PayloadProvider}. This could have been a transient issue or the client closed the connection, exception message '{ExMessage}'";
        private const string _infoResettingSocket = "Resetting socket for provider {PayloadProvider} to allow incomming connections";
        private readonly string _metricsName = String.Empty;
        private Socket _socket;
        private Socket _connectedSocket;
        private SocketServerOptions _options;

        public SocketServer(string providerName, SocketServerOptions sinkOptions, TelemetryClient telemetryClient, ILogger logger, CancellationToken token)
            : base(providerName, sinkOptions, telemetryClient, logger, token)
        {
            _options = sinkOptions;

            SetupServer();
            RegisterCancellationEvent(token);
            _metricsName = $"{ProviderName}-{Options.Type}";
        }

        private void RegisterCancellationEvent(CancellationToken token)
        {
            token.UnsafeRegister((obj) =>
            {
                _socket.Dispose();
                _socket = null;
            }, null);
        }

        private void SetupServer()
        {
            Logger.LogInformation(_infoSocketServerStarted, ProviderName, _options.EndpointPort);

            var endPoint = new IPEndPoint(IPAddress.Any, _options.EndpointPort);
            _socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            _socket.Bind(endPoint);
            _socket.Listen(1);
            _socket.BeginAccept(new AsyncCallback(ConnectedCallback), null);
        }

        private void ConnectedCallback(IAsyncResult ar)
        {
            try
            {
                Socket socket = _socket.EndAccept(ar);
                socket.SendTimeout = _options.SendTimeoutInMs;
                _connectedSocket = socket;
                Logger.LogInformation(_infoClientConnected, ProviderName, _options.EndpointPort);
            }
            catch (ObjectDisposedException)
            {
                // Swallow error since we want to dispose the socket
                // based on the cancellation token getting cancelled.
                // The only time the token gets cancelled is from a
                // Stop or restart and this object will get reconstructed.
            }
        }

        private void ResetSocket()
        {
            Logger.LogInformation(_infoResettingSocket, ProviderName);

            _connectedSocket.Dispose();
            _connectedSocket = null;
            _socket.BeginAccept(new AsyncCallback(ConnectedCallback), null);
        }

        internal override async Task RunAsync(PayloadPackage payloadPackage)
        {
            byte[] payload = payloadPackage.PayloadAsBytes;

            if (OptionsOverrides.TryGetValue(Resources.SinkBase.HeaderKey, out string header))
            {
                payload = Encoding.UTF8.GetBytes(header).Concat(payload).ToArray();
            }

            if (_connectedSocket != null)
            {
                if (_connectedSocket.Connected)
                {
                    try
                    {
                        await _connectedSocket.SendAsync(payload, SocketFlags.None).ConfigureAwait(false);
                        RecordSentMetrics(_metricsName, payloadPackage.PayloadCount, payloadPackage.PayloadCount, true);
                    }
                    catch (Exception ex)
                    {
                        RecordSentMetrics(_metricsName, payloadPackage.PayloadCount, payloadPackage.PayloadCount, false);
                        Logger.LogWarning(_warnLostConnection, ProviderName, ex.Message);
                        ResetSocket();
                    }
                }
                else
                {
                    RecordSentMetrics(_metricsName, payloadPackage.PayloadCount, payloadPackage.PayloadCount, false);
                    ResetSocket();
                }
            }
            else
            {
                Logger.LogWarning(_warnNoClientsConnected, ProviderName);
                await Task.Delay(5000).ConfigureAwait(false);
                await RunAsync(payloadPackage).ConfigureAwait(false);
            }
        }
    }
}