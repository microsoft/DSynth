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
    public class AzureCosmosDbOptions : SinkOptionsBase, IValidatableObject
    {
        [JsonProperty("endpoint")]
        public string Endpoint { get; set; }

        [JsonProperty("authorizationKey")]
        private string _authorizationKey { get; set; }

        [JsonIgnore]
        public string AuthorizationKey => EvaluateAndRetrieveValue(_authorizationKey);

        [JsonProperty("database")]
        public string Database { get; set; }

        [JsonProperty("collection")]
        public string Collection { get; set; }

        [JsonProperty("partitionKey")]
        public string PartitionKey { get; set; }

        [JsonProperty("batchSize")]
        public int BatchSize { get; set; }

        [JsonProperty("maxInMemorySortingBatchSize")]
        public int? MaxInMemorySortingBatchSize { get; set; }

        [JsonProperty("offerThroughput")]
        public int OfferThroughput { get; set; }

        [JsonProperty("enableUpsert")]
        public bool EnableUpsert { get; set; }

        [JsonProperty("disableAutoIdGeneration")]
        public bool DisableAutoIdGeneration { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            foreach (var result in base.Validate(validationContext))
            {
                yield return result;
            }

            if (String.IsNullOrEmpty(Endpoint))
            {
                yield return new ValidationResult("Endpoint cannot be null or empty");
            }
            if (String.IsNullOrEmpty(AuthorizationKey))
            {
                yield return new ValidationResult($"AuthorizationKey cannot be null, empty or env var is missing {_authorizationKey}");
            }
            if (String.IsNullOrEmpty(Database))
            {
                yield return new ValidationResult("Database cannot be null or empty");
            }
            if (String.IsNullOrEmpty(Collection))
            {
                yield return new ValidationResult("Collection cannot be null or empty");
            }
            if (String.IsNullOrEmpty(PartitionKey))
            {
                yield return new ValidationResult("PartitionKey cannot be null or empty");
            }
            if (BatchSize < 1)
            {
                yield return new ValidationResult("BatchSize must be >= 1");
            }
            if (MaxInMemorySortingBatchSize < 1 || MaxInMemorySortingBatchSize >= 1000000)
            {
                yield return new ValidationResult("MaxInMemorySortingBatchSize must be between 1 and 1000000");
            }
            if (OfferThroughput < 400)
            {
                yield return new ValidationResult("OfferThroughput must be >= 400");
            }
        }
    }
}