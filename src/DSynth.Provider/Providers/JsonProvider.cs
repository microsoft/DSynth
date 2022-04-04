/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using Microsoft.Extensions.Logging;
using DSynth.Common.Options;
using System.Text;
using System.Text.Json;
using DSynth.Engine;
using System.Threading;
using DSynth.Common.Models;
using System;

namespace DSynth.Provider.Providers
{
    public class JsonProvider : ProviderBase
    {
        public override PayloadPackage Package => PreparePayloadPackage();

        public JsonProvider(DSynthProviderOptions options, ILogger logger, CancellationToken token)
            : base(options, EngineType.JSON, logger, token)
        {
        }

        public PayloadPackage PreparePayloadPackage()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string stringPayload = JsonSerializer.Serialize(ProviderQueue.Dequeue(out long payloadCount), options);
                return PayloadPackage.CreateNew(Encoding.UTF8.GetBytes(stringPayload), payloadCount, stringPayload);
            }
            catch (Exception ex)
            {
                throw new ProviderException(ex.Message, ex);
            }
        }
    }
}