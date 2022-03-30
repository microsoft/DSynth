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
    public class AzureEventHubOptions : SinkOptionsBase, IValidatableObject
    {
        [JsonProperty("connectionString")]
        private string _connectionString { get; set; }

        [JsonIgnore]
        public string ConnectionString => EvaluateAndRetrieveValue(_connectionString);

        [JsonProperty("eventBatchSizeInBytes")]
        public long EventBatchSizeInBytes { get; set; }

        [JsonProperty("operationTimeoutMs")]
        public int OperationTimeoutMs { get; set; } = 60000;

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            foreach (var result in base.Validate(validationContext))
            {
                yield return result;
            }

            if (String.IsNullOrEmpty(ConnectionString))
            {
                yield return new ValidationResult($"ConnectionString cannot be null, empty or env var is missing {_connectionString}");
            }
            if (EventBatchSizeInBytes <= 0)
            {
                yield return new ValidationResult("EventBatchSizeInBytes must be > 0");
            }
            if (OperationTimeoutMs <= 0)
            {
                yield return new ValidationResult("OperationTimeoutMs must be > 0");
            }
        }
    }
}