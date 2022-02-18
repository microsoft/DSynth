/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace DSynth.Sink.Options
{
    public class AzureLogAnalyticsOptions : SinkOptionsBase, IValidatableObject
    {
        [JsonProperty("workspaceId")]
        public string WorkspaceId { get; set; }

        [JsonProperty("sharedKey")]
        private string _sharedKey { get; set; }

        [JsonIgnore]
        public string SharedKey => EvaluateAndRetrieveValue(_sharedKey);

        [JsonProperty("logType")]
        public string LogType { get; set; }

        [JsonProperty("timestampField")]
        public string TimestampField { get; set; }

        [JsonProperty("requestTimeoutMs")]
        public int RequestTimeoutMs { get; set; } = 60000;

        [JsonProperty("dnsSuffix")]
        public string DnsSuffix { get; set; } = "ods.opinsights.azure.com";

        [JsonProperty("apiVersion")]
        public string ApiVersion { get; set; } = "2016-04-01";

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            foreach (var result in base.Validate(validationContext))
            {
                yield return result;
            }

            if (String.IsNullOrWhiteSpace(WorkspaceId))
            {
                yield return new ValidationResult($"WorkspaceId must be provided");
            }
            if (String.IsNullOrWhiteSpace(SharedKey))
            {
                yield return new ValidationResult($"SharedKey must be provided");
            }
            if (String.IsNullOrWhiteSpace(LogType))
            {
                yield return new ValidationResult($"LogType must be provided");
            }
            if (RequestTimeoutMs < 1)
            {
                yield return new ValidationResult($"RequestTimeoutMs must be >= 0");
            }
            if (String.IsNullOrWhiteSpace(DnsSuffix))
            {
                yield return new ValidationResult($"DnsSuffix must be provided");
            }
            if (String.IsNullOrWhiteSpace(ApiVersion))
            {
                yield return new ValidationResult($"ApiVersion must be provided");
            }
        }
    }
}