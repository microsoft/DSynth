/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.IO;

namespace DSynth.Common.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// When using Directory.GetFiles on Windows systems, paths will come back ./profiles/templates\\my.template.json.
        /// This is a common extension to fix the \\ and return it to /
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string FixDirectoryPathDelimeter(this string str)
        {
            return str.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }
    }
}