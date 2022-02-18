/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSynth.Common;
using DSynth.Common.Options;
using DSynth.Common.Utilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace DSynth.Services.Extensions
{
    public static class ProviderPackageExtensions
    {
        public static async Task UpdateProvidersOptionsAsync(this IDictionary<string, ProviderPackage> packages, List<DSynthProviderOptions> updatedOptions, string profileFileName, ILogger logger)
        {
            foreach (var updatedOption in updatedOptions)
            {
                if (packages.TryGetValue(updatedOption.ProviderName, out ProviderPackage package))
                {
                    package.Options.UpdateOptions(updatedOption, profileFileName);

                    logger.LogInformation(
                        Resources.ProviderPackage.InfoInitializeProvider,
                        updatedOption.ProviderName,
                        updatedOption.Type,
                        updatedOption.ToJsonString());
                }
            }

            // Read the current config and set the
            // provider section to the updated values
            // and write them to the profile file.
            var profilePath = $"{CommonResources.ProfilesRootFolderPath}/{profileFileName}";
            JsonFileContents fileContents = await JsonUtilities.ReadFileAsync(profilePath).ConfigureAwait(false);
            fileContents.JObjectContents[Resources.DSynthService.ProviderSectionName] = JToken.FromObject(updatedOptions);
            await JsonUtilities.WriteFileAsync(profilePath, fileContents.JObjectContents).ConfigureAwait(false);
        }

        public static string GetProviderNames(this IDictionary<string, ProviderPackage> package)
        {
            return String.Join(", ", package.Values.Select(p => p.Options.ProviderName).ToList());
        }

        public static IEnumerable<DSynthProviderOptions> GetProvidersOptionsAsJson(this ConcurrentDictionary<string, ProviderPackage> providerPackagesDict)
        {
            IList<DSynthProviderOptions> providerPackages = new List<DSynthProviderOptions>();

            foreach (ProviderPackage providerPackage in providerPackagesDict.Values)
            {
                providerPackages.Add(providerPackage.Options);
            }

            return providerPackages;
        }
    }
}