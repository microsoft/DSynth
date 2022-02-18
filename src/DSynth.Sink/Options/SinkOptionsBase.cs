/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DSynth.Common.Utilities;
using System.Globalization;

namespace DSynth.Sink.Options
{
    public abstract class SinkOptionsBase : IValidatableObject
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static async Task<T> GetOptionsForSink<T>(string providerName, string fileName)
        {
            try
            {
                JsonFileContents jsonFileContents = await JsonUtilities.ReadFileAsync(fileName).ConfigureAwait(false);
                string providerSinkConfigSection = jsonFileContents.JObjectContents[providerName].ToString();

                return JsonConvert.DeserializeObject<T>(providerSinkConfigSection);
            }
            catch (Exception ex)
            {
                string formattedExMessage = ExceptionUtilities.GetFormattedMessage(
                    Resources.SinkBase.UnableToGetSinkConfig,
                    typeof(T).Name,
                    providerName
                );

                throw new SinkException(formattedExMessage, ex);
            }
        }

        public static T ParseAndValidateOptions<T>(dynamic sinkConfig)
        {
            T config = JsonConvert.DeserializeObject<T>(sinkConfig.ToString());
            DataValidator.Validate(config);
            return config;
        }

        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (String.IsNullOrEmpty(Type))
            {
                yield return new ValidationResult("Type must be specified");
            }
        }

        internal string EvaluateAndRetrieveValue(string value)
        {
            if (String.IsNullOrEmpty(value))
                value = String.Empty;

            if (value.StartsWith(Resources.SinkBase.EnvVarIdentifier, true, CultureInfo.InvariantCulture))
            {
                var envVar = value.Split(Resources.SinkBase.EnvVarDelimeter)[1];
                value = Environment.GetEnvironmentVariable(envVar);
            }

            return value;
        }
    }
}