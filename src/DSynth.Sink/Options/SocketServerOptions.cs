/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace DSynth.Sink.Options
{
    public class SocketServerOptions : SinkOptionsBase, IValidatableObject
    {
        [JsonProperty("endpointPort")]
        public int EndpointPort { get; set; }

        [JsonProperty("sendTimeoutInMs")]
        public int SendTimeoutInMs { get; set; } = 1000;

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            foreach (var result in base.Validate(validationContext))
            {
                yield return result;
            }

            if (EndpointPort <= 0)
            {
                yield return new ValidationResult("EndpointPort must be > 0");
            }
            if (SendTimeoutInMs <=0)
            {
                yield return new ValidationResult("SendTimeoutInMs must be > 0");
            }
        }
    }
}