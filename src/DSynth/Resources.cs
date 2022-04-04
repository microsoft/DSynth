/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

namespace DSynth
{
    public static class Resources
    {
        public static class DSynthService
        {
            // Properties
            public const string DSynthSettingsFile = "./dsynth.json";
            public const string ProviderSectionName = "providers";
            public const int RestartDelayInMs = 3000;

            // Exceptions
            public const string ExUnableToFindProviderWithNameOf = "GetNextPayload :: Unable to find a provider with the name of: {0}. If you just updated the options to include additional providers, you will need to restart DSynth.";
            public const string UnsupportedProviderType = "Build :: Unsupported provider type, '{0}', available types '{1}'";

            // Log Messages
            public const string InfoRestartingDSynth = "Restarting DSynth";
            public const string DebugRetrievingPayloadPackage = "ProviderSinkTask :: Retrieving payload package took {0}ms";
        }

        public static class ProfileService
        {
            // Properties
            public const string ProfileFileExtension = ".json";
            public static string ProfileFileFilter = $"*{ProfileFileExtension}";
            public const string ProfileExtractionRoot = "./";
            public static string ProfilesPackageName = "dsynth_profiles.zip";
            public static string ProfilesPackagePath = $"{ProfileExtractionRoot}/{ProfilesPackageName}";

            // Exceptions
            public const string ExUnableToActivate = "ActivateProfile :: Unable to activate profile with the name of '{0}'. Available profiles are '{1}'";
            public const string ExUnableToUploadProfile = "UploadProfilePackage :: Unable to upload profile, exception message '{0}', see inner exception for details...";
            public const string ExUnableToActivateProfile = "ActivateProfile :: Unable to activate profile '{0}', exception message '{1}'";
            public const string ExUnableToPackageProfiles = "PackageProfiles :: Unable to package profiles for export, exception message '{0}'";

            // Log Messages
            public const string InfoActivatingProfile = "Activating profile '{ProfileName}'";
            public const string InfoAvailableProfiles = "The following profiles are available '{AvailableProfiles}'";
        }

        public static class ProviderPackage
        {
            public const string InfoInitializeProvider = "Initializing provider with the name of '{ProviderName}' and type '{ProviderType}' with the following options '{DSynthProviderOptions}'";
            public const string InfoInitializeSink = "Initializing sink for provider '{ProviderName}' with the following sink options '{SinkOptions}'";
            public const string InfoStartingProviders = "Starting the following providers: '{ProviderNames}'";
            public const string InfoStoppingProviders = "Stopping the following providers: '{ProviderNames}'";
        }
    }
}