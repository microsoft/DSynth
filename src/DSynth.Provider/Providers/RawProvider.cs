/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using Microsoft.Extensions.Logging;
using DSynth.Common.Options;
using DSynth.Engine;
using System.Text;
using System.Threading;
using DSynth.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DSynth.Provider.Providers
{
    public class RawProvider : ProviderBase
    {
        private readonly List<string> _payload;
        public override PayloadPackage Package => PreparePayloadPackage();

        public RawProvider(DSynthProviderOptions options, ILogger logger, CancellationToken token)
            : base(options, EngineType.Raw, logger, token)
        {
            _payload = new List<string>();
        }

        public PayloadPackage PreparePayloadPackage()
        {
            try
            {
                _payload.Clear();
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

                var stringPayload = String.Join('\n', _payload);

                return PayloadPackage.CreateNew(Encoding.UTF8.GetBytes(stringPayload), payloadCount, stringPayload);
            }
            catch (Exception ex)
            {
                throw new ProviderException(ex.Message, ex);
            }
        }
    }
}