/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DSynth.Common.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;

namespace DSynth.Common.Options
{
    public class DSynthProviderOptions : IValidatableObject
    {
        [JsonProperty("isPushEnabled")]
        public bool IsPushEnabled { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("providerName")]
        public string ProviderName { get; set; }

        [JsonProperty("sinks")]
        public dynamic Sinks { get; set; }

        [JsonProperty("intervalInMs")]
        public int IntervalInMs { get; set; }

        [JsonProperty("templateName")]
        public string TemplateName { get; set; }

        [JsonProperty("minBatchSize")]
        public int MinBatchSize { get; set; } = 1;

        [JsonProperty("maxBatchSize")]
        public int MaxBatchSize { get; set; } = 0;

        [JsonProperty("maxIterations")]
        public int MaxIterations { get; set; } = 0;

        [JsonProperty("terminateWhenComplete")]
        public bool TerminateWhenComplete { get; set; } = false;

        // Advanced options to control the payload queue
        [JsonProperty("advancedOptions")]
        public AdvancedOptions AdvancedOptions { get; set; } = new AdvancedOptions();

        private static readonly object _syncLock = new object();

        public void UpdateOptions(DSynthProviderOptions options, string profileFileName)
        {
            lock (_syncLock)
            {
                // Updating regular options
                this.IsPushEnabled = options.IsPushEnabled;
                this.Type = options.Type;
                this.ProviderName = options.ProviderName;
                this.IntervalInMs = options.IntervalInMs;
                this.MinBatchSize = options.MinBatchSize;
                this.MaxBatchSize = options.MaxBatchSize;

                // Updating advanced options
                this.AdvancedOptions.PushDisabledIntervalInMs = options.AdvancedOptions.PushDisabledIntervalInMs;
                this.AdvancedOptions.QueueWorkers = options.AdvancedOptions.QueueWorkers;
                this.AdvancedOptions.TargetQueueSize = options.AdvancedOptions.TargetQueueSize;
            }
        }

        public static IList<DSynthProviderOptions> ParseAndValidateOptions(JObject providerOptions, ILogger logger)
        {
            IList<DSynthProviderOptions> config = new List<DSynthProviderOptions>();

            try
            {
                config = JsonConvert.DeserializeObject<IList<DSynthProviderOptions>>(providerOptions["providers"].ToString());

                Parallel.ForEach(config, (currentConfig) =>
                {
                    DataValidator.Validate(currentConfig);
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }

            return config;
        }

        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (String.IsNullOrEmpty(Type))
            {
                yield return new ValidationResult("Type CANNOT be null or empty");
            }
            if (String.IsNullOrEmpty(ProviderName))
            {
                yield return new ValidationResult("ProviderName CANNOT be null or empty");
            }
            if (IntervalInMs < 0)
            {
                yield return new ValidationResult("IntervalInMs must be >= 0");
            }
            if (String.IsNullOrEmpty(TemplateName))
            {
                yield return new ValidationResult("TemplateName CANNOT be null or empty");
            }
            if (!TemplateExists(out string path))
            {
                yield return new ValidationResult($"TemplateName '{TemplateName}' does not exist in the following path '{path}'");
            }
            if (MaxBatchSize < MinBatchSize && MaxBatchSize > 0)
            {
                yield return new ValidationResult("MaxBatchSize must be > MinBatchSize, unless you want to disable MaxBatchSize then you would set it to 0");
            }
            if (MaxBatchSize < 0)
            {
                yield return new ValidationResult("MaxBatchSize must be >= 1, unless you want to disable MaxBatchSize then you would set it to 0");
            }
            if (MinBatchSize < 0 && IsPushEnabled)
            {
                yield return new ValidationResult("MinBatchSize must be >= 0");
            }
            if (Sinks == null)
            {
                yield return new ValidationResult("Sink must be specified. If you want to output to console only, use \"sink\":{\"type\": \"Console\"}");
            }
            if (!IsAllowedSink(out string allowedWinkMessage))
            {
                yield return new ValidationResult(allowedWinkMessage);
            }
            if (!IsMinBatchSizeAllowed(out string minBatchSizemessage))
            {
                yield return new ValidationResult(minBatchSizemessage);
            }
            if (MaxIterations < 0)
            {
                yield return new ValidationResult("MaxIterations must be >= 0");
            }
        }

        private bool TemplateExists(out string path)
        {
            path = $"{CommonResources.TemplatesRootFolderPath}/{TemplateName}";
            return File.Exists(path);
        }

        private bool IsAllowedSink(out string message)
        {
            List<string> messages = new List<string>();
            bool isAllowed = true;

            foreach (var sink in Sinks)
            {
                message = String.Empty;
                string sinkType = (string)sink.type;

                // Allowed for Image
                if (String.Equals(Type, "Image", StringComparison.OrdinalIgnoreCase))
                {
                    string[] allowedImageSinkTypes = new string[2] { "File", "AzureBlob" };

                    if (!allowedImageSinkTypes.Contains(sinkType))
                    {
                        messages.Add(
                            $"The provided Sink Type of '{sinkType}' is not allowed. The only allowed sink types are '{String.Join(",", allowedImageSinkTypes)}' for provider type 'Image'");
                    }
                }
            }

            message = String.Join("\n", messages);
            isAllowed = messages.Any() ? false : true;
            return isAllowed;
        }

        private bool IsMinBatchSizeAllowed(out string message)
        {
            List<string> messages = new List<string>();
            bool isAllowed = true;

            foreach (var sink in Sinks)
            {
                message = String.Empty;
                string sinkType = (string)sink.type;

                // Allowed for Image
                if (String.Equals(Type, "Image", StringComparison.OrdinalIgnoreCase))
                {
                    if (MinBatchSize != 1)
                    {
                        messages.Add(
                            $"The provided MinBatchSize of '{MinBatchSize}' is not allowed. The only allowed value is '1' for provider type 'Image'");
                    }
                }
            }

            message = String.Join("\n", messages);
            isAllowed = messages.Any() ? false : true;
            return isAllowed;
        }
    }

    public class AdvancedOptions
    {
        [JsonProperty("pushDisabledIntervalInMs")]
        public int PushDisabledIntervalInMs { get; set; } = 10000;

        [JsonProperty("targetQueueSize")]
        public int TargetQueueSize { get; set; } = 50000;

        [JsonProperty("queueWorkers")]
        public int QueueWorkers { get; set; } = 1;
    }
}