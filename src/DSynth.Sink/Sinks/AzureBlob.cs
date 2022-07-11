/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using DSynth.Sink.Options;
using System.Text;
using DSynth.Common.Models;
using Microsoft.ApplicationInsights;

namespace DSynth.Sink.Sinks
{
    public class AzureBlob : SinkBase<AzureBlobOptions>
    {
        private const string _folderPathDelimeter = "/";
        private const string _warnContainerDoesNotExist = "Container '{ContainerName}' does not exist, attempting to create container.";
        private const string _exMessageContainerDoesNotExistString = "The specified container does not exist.";
        private string _infoBlobContainerCreated = "Container '{ContainerName}' created";
        private readonly string _metricsName = String.Empty;
        private Lazy<BlobContainerClient> _lazyClient;

        public AzureBlob(string providerName, AzureBlobOptions options, TelemetryClient telemetryClient, ILogger logger, CancellationToken token)
            : base(providerName, options, telemetryClient, logger, token)
        {
            _lazyClient = new Lazy<BlobContainerClient>(() =>
            {
                if (String.IsNullOrEmpty(Options.ConnectionString))
                {
                    var blobContainerUri = new Uri(new Uri(Options.StorageEndpoint), new Uri(Options.BlobContainerName, UriKind.Relative));
                    var credentialOptions = new DefaultAzureCredentialOptions { ManagedIdentityClientId = Options.ManagedIdentityClientId };
                    var credential = new DefaultAzureCredential(credentialOptions);

                    return new BlobContainerClient(blobContainerUri, credential);
                }
                else
                {
                    return new BlobContainerClient(Options.ConnectionString, Options.BlobContainerName);
                }
            });

            _lazyClient.Value.CreateIfNotExistsAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            _metricsName = $"{ProviderName}-{Options.Type}-{_lazyClient.Value.AccountName}-{_lazyClient.Value.Name}";
        }

        internal override async Task RunAsync(PayloadPackage payloadPackage)
        {
            byte[] payload = payloadPackage.PayloadAsBytes;

            // Read filename suffix from OptionsOverrides and default to the value in Options.
            if (!OptionsOverrides.TryGetValue(Resources.SinkBase.FilenameSuffixKey, out string filenameSuffix))
            {
                filenameSuffix = Options.FilenameSuffix;
            }

            if (OptionsOverrides.TryGetValue(Resources.SinkBase.HeaderKey, out string header))
            {
                payload = Encoding.UTF8.GetBytes(header).Concat(payload).ToArray();
            }

            var filenamePrefix = PathTemplateReplacement(Options.FilenamePattern);
            var filename = $"{filenamePrefix}{filenameSuffix}";
            var subfolderPath = PathTemplateReplacement(Options.SubfolderPattern ?? String.Empty);
            var fullPath = $"{subfolderPath}{_folderPathDelimeter}{filename}";

            try
            {
                using (MemoryStream ms = new MemoryStream(payload))
                {
                    await _lazyClient.Value.UploadBlobAsync(fullPath, ms, Token)
                        .ConfigureAwait(false);
                }
            }
            catch (Azure.RequestFailedException ex)
            {
                RecordFailedSend(_metricsName, payloadPackage.PayloadCount, payloadPackage.PayloadCount);
                
                if (ex.Message.Contains(_exMessageContainerDoesNotExistString))
                {
                    Logger.LogWarning(_warnContainerDoesNotExist, Options.BlobContainerName);
                    await _lazyClient.Value.CreateIfNotExistsAsync().ConfigureAwait(false);
                    Logger.LogInformation(_infoBlobContainerCreated, Options.BlobContainerName);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}