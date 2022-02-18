/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using DSynth.Common.Options;
using Newtonsoft.Json;

namespace DSynth.Common.Extensions
{
    public static class DSynthProviderOptionsExtensions
    {
        public static string ToJsonString(this IEnumerable<DSynthProviderOptions> options)
        {
            return JsonConvert.SerializeObject(options);
        }

        public static string GetProviderNames(this IEnumerable<DSynthProviderOptions> providers)
        {
            return String.Join(",", providers.Select(p => p.ProviderName).ToList());
        }
    }
}