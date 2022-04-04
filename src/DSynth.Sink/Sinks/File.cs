/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Text;
using DSynth.Common.Models;
using Microsoft.ApplicationInsights;

namespace DSynth.Sink.Sinks
{
    public class File : SinkBase<Options.FileOptions>
    {
        private const string _folderPathDelimeter = "/";
        private readonly string _metricsName = String.Empty;

        public File(string providerName, Options.FileOptions options, TelemetryClient telemetryClient, ILogger logger, CancellationToken token)
            : base(providerName, options, telemetryClient, logger, token)
        {
            Directory.CreateDirectory(Options.BaseFolderPath);
        }

        internal override async Task RunAsync(PayloadPackage payloadPackage)
        {
            byte[] payload = payloadPackage.PayloadAsBytes;

            // Read filename suffix from OptionsOverrides and default to the value in Options.
            if (!OptionsOverrides.TryGetValue(Resources.SinkBase.FilenameSuffixKey, out string filenameSuffix))
            {
                filenameSuffix = Options.FilenameSuffix;
            }

            OptionsOverrides.TryGetValue(Resources.SinkBase.HeaderKey, out string header);

            var filenamePrefix = PathTemplateReplacement(Options.FilenamePattern);
            var filename = $"{filenamePrefix}{filenameSuffix}";
            var subfolderPath = PathTemplateReplacement(Options.SubfolderPattern);
            var fullFolderPath = Path.Combine(Options.BaseFolderPath, subfolderPath ?? String.Empty);
            var fullPath = $"{fullFolderPath}{_folderPathDelimeter}{filename}";

            try
            {
                if (!Directory.Exists(fullFolderPath)) { Directory.CreateDirectory(fullFolderPath); }

                if (!Directory.GetFiles(fullFolderPath, filename).Any() && !String.IsNullOrWhiteSpace(header))
                {
                    using (FileStream fileStream = new FileStream(fullPath, Options.FileMode, FileAccess.Write, FileShare.None))
                    {
                        await fileStream.WriteAsync(Encoding.UTF8.GetBytes(header).Concat(payload).ToArray(), 0, header.Length + payload.Length, Token);
                    }
                }
                else
                {
                    using (FileStream fileStream = new FileStream(fullPath, Options.FileMode, FileAccess.Write, FileShare.None))
                    {
                        await fileStream.WriteAsync(payload, 0, payload.Length, Token);
                    }
                }
                
            }
            catch (Exception)
            {
                RecordFailedSend(_metricsName, payloadPackage.PayloadCount, payloadPackage.PayloadCount);
                throw;
            }
        }
    }
}