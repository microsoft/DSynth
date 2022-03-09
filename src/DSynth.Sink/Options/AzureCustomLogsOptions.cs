/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DSynth.Sink.Utilities;
using Newtonsoft.Json;

namespace DSynth.Sink.Options
{
    public class AzureCustomLogsOptions : SinkOptionsBase, IValidatableObject
    {
        [JsonProperty("tenantId")]
        public string TenantId { get; set; }

        [JsonProperty("appId")]
        public string AppId { get; set; }

        [JsonProperty("appSecret")]
        private string _appSecret { get; set; }

        [JsonIgnore]
        public string AppSecret => EvaluateAndRetrieveValue(_appSecret);

        [JsonProperty("dcrImmutableId")]
        public string DcrImmutableId { get; set; }

        [JsonProperty("dataCollectionEndpoint")]
        public string DataCollectionEndpoint { get; set; }

        [JsonProperty("customTableName")]
        public string CustomTableName { get; set; }

        [JsonProperty("enableCompression")]
        public bool EnableCompression { get; set; } = false;

        [JsonProperty("bearerScope")]
        public string BearerScope { get; set; } = "https://monitor.azure.com//.default";

        [JsonProperty("bearerExpBufferSeconds")]
        public int BearerExpBufferSeconds { get; set; } = 60;

        [JsonProperty("requestTimeoutMs")]
        public int RequestTimeoutMs { get; set; } = 60000;

        [JsonProperty("azureManagementDns")]
        public string AzureManagementDns { get; set; } = "login.microsoftonline.com";

        [JsonProperty("apiVersion")]
        public string ApiVersion { get; set; } = "2021-11-01-preview";

        public BearerOptions GetBearerOptions()
        {
            return new BearerOptions
            {
                TenantId = TenantId,
                AppId = AppId,
                AppSecret = AppSecret,
                Scope = BearerScope,
                ExpBufferSeconds = BearerExpBufferSeconds,
                AzureManagemenrDns = AzureManagementDns
            };
        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            foreach (var result in base.Validate(validationContext))
            {
                yield return result;
            }

            if (String.IsNullOrWhiteSpace(TenantId))
            {
                yield return new ValidationResult($"TenantId must be provided");
            }
            if (String.IsNullOrWhiteSpace(AppId))
            {
                yield return new ValidationResult($"AppId must be provided");
            }
            if (String.IsNullOrWhiteSpace(AppSecret))
            {
                yield return new ValidationResult($"AppSecret must be provided");
            }
            if (String.IsNullOrWhiteSpace(DcrImmutableId))
            {
                yield return new ValidationResult($"DcrImmutableId must be provided");
            }
            if (String.IsNullOrWhiteSpace(DataCollectionEndpoint))
            {
                yield return new ValidationResult($"DataCollectionEndpoint must be provided");
            }
            if (String.IsNullOrWhiteSpace(CustomTableName))
            {
                yield return new ValidationResult($"CustomTableName must be provided");
            }
            if (String.IsNullOrWhiteSpace(BearerScope))
            {
                yield return new ValidationResult($"BearerScope must be provided");
            }
            if (BearerExpBufferSeconds < 1)
            {
                yield return new ValidationResult($"BearerExpBufferMs must be > 0");
            }
            if (RequestTimeoutMs < 1)
            {
                yield return new ValidationResult($"RequestTimeoutMs must be > 0");
            }
            if (String.IsNullOrWhiteSpace(AzureManagementDns))
            {
                yield return new ValidationResult($"AzureManagementDns must be provided");
            }
            if (String.IsNullOrWhiteSpace(ApiVersion))
            {
                yield return new ValidationResult($"ApiVersion must be provided");
            }
        }
    }
}