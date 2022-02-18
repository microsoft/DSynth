/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DSynth.Services
{
    public interface IProfileService
    {
        /// <summary>
        /// Activates a specified profile that is available in the Profiles dir
        /// </summary>
        /// <exception>cref="DSynth.Engine.ProfileServiceException"</exception>
        Task ActivateProfile(string profileName);

        /// <summary>
        /// Gets a list of available profiles from the Profiles dir
        /// </summary>
        /// <exception>cref="DSynth.Engine.ProfileServiceException"</exception>
        List<Profile> GetAvailableProfiles();

        /// <summary>
        /// Allows importing profiles via zip file
        /// </summary>
        /// <exception> cref="DSynth.Engine.ProfileServiceException"</exception>
        Task<string> ImportProfilesPackageAsync(IFormFile file);

        /// <summary>
        /// Allows exporting zip files via byte array
        /// </summary>
        /// <returns>byte[]</returns>
        /// <exception>cref="DSynth.Engine.ProfileServiceException"</exception>
        byte[] ExportProfilesPackage();
    }
}