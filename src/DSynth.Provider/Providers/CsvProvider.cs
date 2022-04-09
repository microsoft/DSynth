/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using DSynth.Common.Options;
using DSynth.Engine;
using System.Threading;
using DSynth.Common.Models;
using System;

namespace DSynth.Provider.Providers
{
    public class CsvProvider : ProviderBase
    {
        private const string _exUnableToExtractHeader = "ExtractHeader :: Unable to extract header values. Make sure ::Header={YOUR_HEADER_VALUES} in your csv template file.";
        private const string headerMetadataKey = "Header";
        private readonly List<string> _payload;
        private readonly StringBuilder _payloadStringBuilder = new StringBuilder();
        public override PayloadPackage Package => PreparePayloadPackage();

        public CsvProvider(DSynthProviderOptions options, ILogger logger, CancellationToken token)
            : base(options, EngineType.CSV, logger, token)
        {
            _payload = new List<string>();
        }

        // Working with CSV is a little different than other file formats. Since we use dynamic,
        // we need to determine if the incoming next payload is String or List<string> before
        // trying to convert it and provide the caller the next string payload.
        public PayloadPackage PreparePayloadPackage()
        {
            try
            {
                _payload.Clear();
                _payloadStringBuilder.Clear();
                object nextPayload = ProviderQueue.Dequeue(out long payloadCount);

                if (nextPayload is System.String)
                {
                    _payload.Add((string)nextPayload);
                }
                else if (nextPayload is List<object>)
                {
                    List<object> payload = (List<object>)nextPayload;
                    List<string> payloadStringCollection = payload.Select(i => i.ToString()).ToList();

                    _payload.AddRange(payloadStringCollection);
                }

                // Build the resulting string. Due to how different OSs interpret strings, OSX will add line 
                // endings and Linux will not add line endings. Need to check and add \n if needed.
                foreach (string s in _payload)
                {
                    _payloadStringBuilder.Append(s);
                    if (!s.EndsWith('\n'))
                    {
                        _payloadStringBuilder.Append('\n');
                    }
                }

                return PayloadPackage.CreateNew(Encoding.UTF8.GetBytes(_payloadStringBuilder.ToString()), payloadCount, _payloadStringBuilder.ToString(), null, overrides =>
                {
                    overrides.Add("Header", $"{ExtractHeader()}");
                });
            }
            catch (Exception ex)
            {
                throw new ProviderException(ex.Message, ex);
            }
        }

        private string ExtractHeader()
        {
            if (!DSynthEngine.TemplateDataDictionary[Options.ProviderName].Metadata.TryGetValue(headerMetadataKey, out string header))
            {
                throw new ProviderException(_exUnableToExtractHeader);
            }

            return $"{header}\n";
        }
    }
}