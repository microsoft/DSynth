/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace DSynth.Common.Utilities
{
    public class JsonUtilities
    {
        public static async Task<JsonFileContents> ReadFileAsync(string jsonFilePath)
        {
            JsonFileContents ret = null;
            try
            {
                var contents = await File.ReadAllTextAsync(jsonFilePath).ConfigureAwait(false);
                ret = new JsonFileContents(contents);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
            }

            return ret;
        }

        public static async Task WriteFileAsync(string jsonFilePath, JObject json)
        {
            await File.WriteAllTextAsync(jsonFilePath, json.ToString()).ConfigureAwait(false);
        }
    }
}