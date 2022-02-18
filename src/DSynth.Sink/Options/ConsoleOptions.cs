/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace DSynth.Sink.Options
{
    public class ConsoleOptions : SinkOptionsBase, IValidatableObject
    {
        [JsonProperty("writeToConsole")]
        public bool WriteToConsole { get; set; } = true;

        [JsonProperty("writeToLog")]
        public bool WriteToLog { get; set; } = false;

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            foreach (var result in base.Validate(validationContext))
            {
                yield return result;
            }
        }
    }
}