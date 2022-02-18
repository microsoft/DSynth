/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Newtonsoft.Json;

namespace DSynth.Sink.Options
{
    public class HttpOptions : SinkOptionsBase, IValidatableObject
    {
        [JsonProperty("endpointScheme")]
        public string EndpointScheme { get; set; } = "http";

        [JsonProperty("endpointDns")]
        public string EndpointDns { get; set; }

        [JsonProperty("endpointPath")]
        public string EndpointPath { get; set; }

        [JsonProperty("endpointPort")]
        public int EndpointPort { get; set; } = 80;

        [JsonProperty("mimeType")]
        public string MimeType { get; set; } = "application/json";

        [JsonProperty("requestTimeoutSeconds")]
        public int RequestTimeoutMs { get; set; } = 60000;

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            foreach (var result in base.Validate(validationContext))
            {
                yield return result;
            }

            if (!IsAcceptedScheme())
            {
                yield return new ValidationResult("EndpointScheme must be either 'http' or 'https'");
            }
            if (String.IsNullOrEmpty(EndpointDns))
            {
                yield return new ValidationResult("EndpointDns must be specified");
            }
            if (EndpointPort <= 0)
            {
                yield return new ValidationResult("EndpointPort must be > 0");
            }
            if (String.IsNullOrEmpty(EndpointDns))
            {
                yield return new ValidationResult("EndpointDns must be specified");
            }
            if (!IsValidUri(out string uriString))
            {
                yield return new ValidationResult($"Invalid URI format '{uriString}'");
            }
            if (RequestTimeoutMs < 1)
            {
                yield return new ValidationResult($"RequestTimeoutMs must be >= 0");
            }
        }

        private bool IsValidUri(out string uriString)
        {
            StringBuilder uriStringBuilder = new StringBuilder();
            uriStringBuilder
                .Append(EndpointScheme)
                .Append("://")
                .Append(EndpointDns)
                .Append(":")
                .Append(EndpointPort)
                .Append(EndpointPath);
            
            uriString = uriStringBuilder.ToString();
            
            return Uri.IsWellFormedUriString(uriString, UriKind.Absolute);
        }

        private bool IsAcceptedScheme()
        {
            return EndpointScheme == "http" || EndpointScheme == "https";
        }
    }
}