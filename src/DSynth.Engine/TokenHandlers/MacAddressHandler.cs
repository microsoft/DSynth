/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Linq;

namespace DSynth.Engine.TokenHandlers
{
    public class MacAddressHandler : TokenHandlerBase
    {
        public MacAddressHandler(TokenDescriptor tokenDescriptor, string providerName) 
            : base(tokenDescriptor, providerName, null)
        {
        }

        public override string GetReplacementValue()
        {
            switch (SourceType)
            {
                case EngineSourceType.MacAddress:
                    return GetNextMacAddress();
                
                default:
                    return String.Empty;
            }
        }

        private static string GetNextMacAddress()
        {
            var buffer = new byte[6];
            TokenHandlerHelpers.GetNextBytes(buffer);
            var result = buffer.Select(x => string.Format("{0}", x.ToString("X2"))).ToArray();
            return String.Join(":", result);
        }
    }
}