/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using Microsoft.Extensions.Logging;
using DSynth.Common.Options;
using DSynth.Engine;
using System.Threading;
using DSynth.Common.Models;
using System;
using Google.Protobuf;

namespace DSynth.Provider.Providers
{
    public class ProtoProvider : ProviderBase
    {
        public override PayloadPackage Package => PreparePayloadPackage();

        public ProtoProvider(DSynthProviderOptions options, ILogger logger, CancellationToken token)
            : base(options, EngineType.Proto, logger, token)
        {
        }

        public PayloadPackage PreparePayloadPackage()
        {
            try
            {
                ProtoPayload nextPayload = (ProtoPayload)ProviderQueue.Dequeue(out long payloadCount);
                
                // Dynamically create the appropriate class/message that
                // gets converted into a byte array in protobuf encoding.
                var type = Type.GetType(nextPayload.ProtoOptions.FullClassName);
                var message = (IMessage)Activator.CreateInstance(type);
                string data = nextPayload.DataAsString;
                var result = JsonParser.Default.Parse(data, message?.Descriptor);
                var bytes = result.ToByteArray();

                return PayloadPackage.CreateNew(bytes, payloadCount, data);
            }
            catch (Exception ex)
            {
                throw new ProviderException(ex.Message, ex);
            }
        }
    }
}