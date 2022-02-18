/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using Newtonsoft.Json;

namespace DSynth.Options
{
    public class ApplicationInsightsOptions
    {
        [JsonProperty("connectionString")]
        public string ConnectionString { get; set; }
    }
}