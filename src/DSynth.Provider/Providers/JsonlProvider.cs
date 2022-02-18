/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using Microsoft.Extensions.Logging;
using System.Linq;
using DSynth.Common.Options;
using System.Text;
using DSynth.Engine;
using System.Threading;
using DSynth.Common.Models;
using System;
using System.Collections.Generic;

namespace DSynth.Provider.Providers
{
    public class JsonlProvider : ProviderBase
    {
        public override PayloadPackage Package => PreparePayloadPackage();
        private readonly List<string> _payload;

        public JsonlProvider(DSynthProviderOptions options, ILogger logger, CancellationToken token)
            : base(options, EngineType.JSONL, logger, token)
        {
            _payload = new List<string>();
        }

        public PayloadPackage PreparePayloadPackage()
        {
            try
            {
                _payload.Clear();
                object nextPayload = ProviderQueue.Dequeue();

                if (nextPayload is List<object>)
                {
                    List<object> payload = (List<object>)nextPayload;
                    _payload.AddRange(payload.Select(i => i.ToString()).ToList());
                }
                else if (nextPayload is string)
                {
                    string nPayload = (string)nextPayload;
                    _payload.Add(nPayload);
                }

                var stringPayload = String.Join(Environment.NewLine, _payload) + Environment.NewLine;
                return PayloadPackage.CreateNew(Encoding.UTF8.GetBytes(stringPayload), stringPayload);
            }
            catch (Exception ex)
            {
                throw new ProviderException(ex.Message, ex);
            }
        }
    }
}