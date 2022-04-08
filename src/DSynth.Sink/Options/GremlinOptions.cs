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
    public class ApacheGremlinOptions : SinkOptionsBase, IValidatableObject
    {
        [JsonProperty("endpoint")]
        public string Endpoint { get; set; }

        [JsonProperty("authorizationKey")]
        private string _authorizationKey { get; set; }

        [JsonIgnore]
        public string AuthorizationKey => EvaluateAndRetrieveValue(_authorizationKey);

        [JsonProperty("database")]
        public string Database { get; set; }

        [JsonProperty("container")]
        public string Container { get; set; }

        [JsonIgnore]
        public string ContainerLink { get; }

        [JsonProperty("enableSSL")]
        public bool EnableSSL { get; set; } = true;

        [JsonProperty("port")]
        public int Port { get; set; } = 443;

        [JsonProperty("maxInProcessPerConnection")]
        public int MaxInProcessPerConnection { get; set; } = 10;

        [JsonProperty("poolSize")]
        public int PoolSize { get; set; } = 30;

        [JsonProperty("eeconnectionAttempts")]
        public int ReconnectionAttempts { get; set; } = 3;

        [JsonProperty("reconnectionBaseDelayMs")]
        public int ReconnectionBaseDelayMs { get; set; } = 500;

        [JsonProperty("keepAliveIntervalSec")]
        public int KeepAliveIntervalSec { get; set; } = 10;

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
            if (String.IsNullOrEmpty(Container))
            {
                yield return new ValidationResult("Container cannot be null or empty");
            }
            if (Port <= 0)
            {
                yield return new ValidationResult("Port must be > 0");
            }
            if (MaxInProcessPerConnection <= 0)
            {
                yield return new ValidationResult("MaxInProcessPerConnection must be > 0");
            }
            if (PoolSize <= 0)
            {
                yield return new ValidationResult("PoolSize must be > 0");
            }
            if (ReconnectionAttempts <= 0)
            {
                yield return new ValidationResult("ReconnectionAttempts must be > 0");
            }
            if (ReconnectionBaseDelayMs <= 0)
            {
                yield return new ValidationResult("ReconnectionBaseDelayMs must be > 0");
            }
            if (KeepAliveIntervalSec <= 0)
            {
                yield return new ValidationResult("KeepAliveIntervalSec must be > 0");
            }
        }
    }
}