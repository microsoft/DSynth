/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using DSynth.Common.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DSynth.Options
{
    public class DSynthSettings : IValidatableObject
    {
        private const string _settingsFile = Resources.DSynthService.DSynthSettingsFile;
        private const string _rootPath = "./Profiles";
        public string FullProfilePath => $"{_rootPath}/{ProvidersFile}";

        [JsonProperty("providersFile")]
        public string ProvidersFile { get; set; } = "sample-providers.json";

        public async Task UpdateSettings(DSynthSettings newSettings)
        {
            this.ProvidersFile = newSettings.ProvidersFile;

            var newContents = JsonConvert.SerializeObject(this, Formatting.Indented);
            await File.WriteAllTextAsync(_settingsFile, newContents).ConfigureAwait(false);
        }

        public static DSynthSettings ParseAndValidateSettings(JObject dSynthSettings)
        {
            DSynthSettings config = new DSynthSettings();

            try
            {
                config = JsonConvert.DeserializeObject<DSynthSettings>(dSynthSettings.ToString());
                DataValidator.Validate(config);
            }
            catch (Exception ex)
            {
                // TODO: Add proper logging for exception, throw custom exception
                System.Console.WriteLine(ex.Message);
            }

            return config;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (String.IsNullOrEmpty(ProvidersFile))
            {
                yield return new ValidationResult("ProvidersFile file name CANNOT be null or empty");
            }
        }
    }
}