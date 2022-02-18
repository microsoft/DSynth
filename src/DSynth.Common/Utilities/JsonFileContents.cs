/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using Newtonsoft.Json.Linq;

namespace DSynth.Common.Utilities
{
    public class JsonFileContents
    {
        public JsonFileContents(string fileContents)
        {
            StringContents = fileContents;
            JObjectContents = JObject.Parse(fileContents);
        }

        public string StringContents { get; }
        public JObject JObjectContents { get; }
    }
}