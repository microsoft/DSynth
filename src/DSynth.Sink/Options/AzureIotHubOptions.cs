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
    public class AzureIotHubOptions : SinkOptionsBase, IValidatableObject
    {
        [JsonProperty("deviceConnectionString")]
        private string _deviceConnectionString { get; set; }

        [JsonIgnore]
        public string DeviceConnectionString => EvaluateAndRetrieveValue(_deviceConnectionString);

        [JsonProperty("deviceId")]
        public string DeviceId { get; set; } = "Undefined";

        [JsonProperty("batchSizeInBytes")]
        public int BatchSizeInBytes { get; set; } = 262144; //default 256Kb

        [JsonProperty("batchFlushIntervalMiliSec")]
        public int BatchFlushIntervalMiliSec { get; set; } = 30000;
    }
}