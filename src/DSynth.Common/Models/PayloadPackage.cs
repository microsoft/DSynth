/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;

namespace DSynth.Common.Models
{
    public class PayloadPackage
    {
        private const string _payloadAsStringNotSupported = "Retrieving payload as a string is not supported.";
        public byte[] PayloadAsBytes { get; }
        public string PayloadAsString { get; }
        public IDictionary<string, string> Overrides { get; }

        public PayloadPackage()
        {
        }

        public PayloadPackage(byte[] payloadAsBytes, string payloadAsString = null, IDictionary<string, string> overrides = null)
        {
            PayloadAsBytes = payloadAsBytes;
            PayloadAsString = payloadAsString ?? _payloadAsStringNotSupported;
            Overrides = overrides;
        }

        public static PayloadPackage CreateNew(byte[] payloadAsBytes, string payloadAsString = null, Action<IDictionary<string, string>> overrides = null)
        {
            IDictionary<string, string> overridesDict = new Dictionary<string, string>();
            if (overrides != null) overrides(overridesDict);
            
            return new PayloadPackage(
                payloadAsBytes, 
                payloadAsString ?? _payloadAsStringNotSupported, 
                overridesDict);
        }
    }
}