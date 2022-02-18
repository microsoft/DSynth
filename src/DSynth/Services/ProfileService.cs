/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using DSynth.Common;
using DSynth.Common.Utilities;
using DSynth.Services.Extesions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace DSynth.Services
{
    public class ProfileService : IProfileService
    {
        private readonly object _lockObject = new object();

        public async Task ActivateProfile(string profileName)
        {
            try
            {
                Profile profileToActivate = GetProfileToActivate(profileName);

                JsonFileContents dSynthSettings = await JsonUtilities
                    .ReadFileAsync(Resources.DSynthService.DSynthSettingsFile)
                    .ConfigureAwait(false);

                dSynthSettings.JObjectContents["providersFile"] = profileToActivate.Name;

                await JsonUtilities
                    .WriteFileAsync(Resources.DSynthService.DSynthSettingsFile, dSynthSettings.JObjectContents)
                    .ConfigureAwait(false);
            }
            catch (ProfileServiceException)
            {
                throw;
            }
            catch (Exception ex)
            {
                string formattedExMessage = ExceptionUtilities.GetFormattedMessage(
                    Resources.ProfileService.ExUnableToActivateProfile,
                    profileName,
                    ex.Message
                );

                throw new ProfileServiceException(formattedExMessage, ex);
            }
        }

        public List<Profile> GetAvailableProfiles()
        {
            List<Profile> profiles = Directory.GetFiles(CommonResources.ProfilesRootFolderPath, Resources.ProfileService.ProfileFileFilter)
                .Select(p => Profile.CreateNew(p)).ToList<Profile>();

            return profiles;
        }

        public async Task<string> ImportProfilesPackageAsync(IFormFile file)
        {
            string ret = String.Empty;

            try
            {
                using (var ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms).ConfigureAwait(false);

                    using (ZipArchive zip = new ZipArchive(ms))
                    {
                        zip.ExtractToDirectory(CommonResources.ProfilesRootFolderPath, overwriteFiles: true);
                        ret = String.Join(", ", zip.Entries.ToList().Select(x => x.FullName));
                    }
                }
            }
            catch (Exception ex)
            {
                string formattedExMessage = ExceptionUtilities.GetFormattedMessage(
                    Resources.ProfileService.ExUnableToUploadProfile,
                    ex.Message
                );

                throw new ProfileServiceException(formattedExMessage, ex);
            }

            return ret;
        }

        public byte[] ExportProfilesPackage()
        {
            byte[] ret;

            try
            {
                lock (_lockObject)
                {
                    if (File.Exists(Resources.ProfileService.ProfilesPackagePath))
                    {
                        File.Delete(Resources.ProfileService.ProfilesPackagePath);
                    }

                    var archiveStream = new MemoryStream();

                    ZipFile.CreateFromDirectory(CommonResources.ProfilesRootFolderPath, Resources.ProfileService.ProfilesPackagePath);
                    ret = File.ReadAllBytes(Resources.ProfileService.ProfilesPackagePath);
                }
            }
            catch (Exception ex)
            {
                string formattedExMessage = ExceptionUtilities.GetFormattedMessage(
                    Resources.ProfileService.ExUnableToPackageProfiles,
                    ex.Message
                );

                throw new ProfileServiceException(formattedExMessage, ex);
            }

            return ret;
        }

        private Profile GetProfileToActivate(string profileName)
        {
            List<Profile> availableProfiles = GetAvailableProfiles();
            Profile profileToActivate = availableProfiles.GetProfileByName(profileName);

            if (profileToActivate == null)
            {
                string formattedExMessage = ExceptionUtilities.GetFormattedMessage(
                    Resources.ProfileService.ExUnableToActivate,
                    profileName,
                    availableProfiles.ExtractProfileNames()
                );

                throw new ProfileServiceException(formattedExMessage);
            }

            return profileToActivate;
        }
    }
}