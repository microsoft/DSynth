/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Collections.Generic;
using System.Threading.Tasks;
using DSynth.Common.Models;
using DSynth.Common.Options;
using DSynth.Models;

namespace DSynth.Services
{
    public interface IDSynthService
    {
        /// <summary>
        /// Starts DSynth
        /// </summary>
        Task StartAsync();

        /// <summary>
        /// Stops DSynth
        /// </summary>
        Task StopAsync();

        /// <summary>
        /// Restarts DSynth
        /// </summary>
        Task RestartAsync();

        /// <summary>
        /// Gets all DSynthProviderOptions as JSON
        /// </summary>
        /// <returns>string</returns>
        IEnumerable<DSynthProviderOptions> GetProvidersOptions();

        /// <summary>
        /// Update the provider options of all providers
        /// </summary>
        /// <param name="updatedProvidersOptions"></param>
        /// <returns>List of DSynthProviderOptions</returns>
        Task<List<DSynthProviderOptions>> UpdateProvidersOptionsAsync(List<DSynthProviderOptions> updatedProvidersOptions);

        /// <summary>
        /// Retrieves the names of the running providers
        /// </summary>
        /// <returns>Object containing an array of running providers</returns>
        object GetProviderNames();

        /// <summary>
        /// Retrieves the next payload from a given provider name
        /// </summary>
        /// <param name="providerName"></param>
        /// <returns>PayloadPackage</returns>
        PayloadPackage GetNextPayload(string providerName);

        /// <summary>
        /// Returns the operational status of DSynth
        /// </summary>
        /// <returns>DSynthStatus</returns>
        DSynthStatus GetStatus();
    }
}