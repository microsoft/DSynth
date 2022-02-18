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
    public class AzureServiceBusOptions : SinkOptionsBase, IValidatableObject
    {
        [JsonProperty("connectionString")]
        private string _connectionString { get; set; }

        [JsonIgnore]
        public string ConnectionString => EvaluateAndRetrieveValue(_connectionString);

        [JsonProperty("maxSizeInBytes")]
        public long? MaxSizeInBytes { get; set; }

        [JsonProperty("topicOrQueueName")]
        public string TopicOrQueueName { get; set; }

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
            if (MaxSizeInBytes <= 0 && MaxSizeInBytes != null)
            {
                yield return new ValidationResult("MaxSizeInBytes must be > 0");
            }
            if (String.IsNullOrEmpty(TopicOrQueueName))
            {
                yield return new ValidationResult("TopicOrQueueName cannot be null or empty");
            }
        }
    }
}