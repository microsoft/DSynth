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
        public byte[] PayloadAsBytes { get; }
        public long PayloadCount { get; }
        public string PayloadAsString { get; }
        public List<object> PayloadAsObjectList { get; set; }
        public IDictionary<string, string> Overrides { get; }

        public PayloadPackage()
        {
        }

        private PayloadPackage(byte[] payloadAsBytes, long payloadCount, string payloadAsString = null, List<object> payloadAsObjectList = null, IDictionary<string, string> overrides = null)
        {
            PayloadAsBytes = payloadAsBytes;
            PayloadCount = payloadCount;
            PayloadAsString = payloadAsString;
            PayloadAsObjectList = payloadAsObjectList;
            Overrides = overrides;
        }

        public static PayloadPackage CreateNew(byte[] payloadAsBytes, long payloadCount, string payloadAsString = null, List<object> payloadAsObjectList = null,  Action<IDictionary<string, string>> overrides = null)
        {
            IDictionary<string, string> overridesDict = new Dictionary<string, string>();
            if (overrides != null) overrides(overridesDict);

            return new PayloadPackage(
                payloadAsBytes,
                payloadCount,
                payloadAsString ?? String.Empty,
                payloadAsObjectList,
                overridesDict);
        }
    }
}