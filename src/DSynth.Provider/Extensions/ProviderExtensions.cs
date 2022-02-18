/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using DSynth.Common.Options;
using DSynth.Provider.Providers;

namespace DSynth.Provider.Extensions
{
    public static class ProviderExtensions
    {
        public static void UpdateProviderOptions(this IDictionary<string, ProviderBase> providers, IEnumerable<DSynthProviderOptions> updatedOptions)
        {
            foreach (var updatedOption in updatedOptions)
            {
                if (providers.TryGetValue(updatedOption.ProviderName, out ProviderBase provider))
                {
                    provider.Options = updatedOption;
                }
            }
        }

        public static string GetProviderNames(this IDictionary<string, ProviderBase> providers)
        {
            return String.Join(",", providers.Values.Select(p => p.Options.ProviderName).ToList());
        }
    }
}