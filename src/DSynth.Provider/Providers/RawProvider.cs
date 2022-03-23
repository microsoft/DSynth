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

namespace DSynth.Provider.Providers
{
    public class RawProvider : ProviderBase
    {
        public override PayloadPackage Package => PreparePayloadPackage();

        public RawProvider(DSynthProviderOptions options, ILogger logger, CancellationToken token)
            : base(options, EngineType.Raw, logger, token)
        {
        }

        public PayloadPackage PreparePayloadPackage()
        {
            try
            {
                string payloadString = (string)ProviderQueue.Dequeue(out long payloadCount);
                return PayloadPackage.CreateNew(Encoding.UTF8.GetBytes(payloadString), payloadCount, payloadString);
            }
            catch (Exception ex)
            {
                throw new ProviderException(ex.Message, ex);
            }
        }
    }
}