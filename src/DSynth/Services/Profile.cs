/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Linq;
using DSynth.Common.Extensions;

namespace DSynth
{
    public class Profile
    {
        public string Name { get; set; }
        public string Path { get; set; }

        public static Profile CreateNew(string path)
        {
            return new Profile
            {
                Path = path.FixDirectoryPathDelimeter(),
                Name = ExtractProfileName(path)
            };
        }

        /// <summary>
        /// Gets just the profile name, dropping path information
        /// </summary>
        private static string ExtractProfileName(string path)
        {
            return path.Split('/').Last();
        }
    }
}