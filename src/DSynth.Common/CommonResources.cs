/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.IO;

namespace DSynth.Common
{
    public static class CommonResources
    {
        public static string DirectorySeparatorChar = $"{Path.AltDirectorySeparatorChar}";
        public static string ProfilesRootFolderPath = $".{DirectorySeparatorChar}Profiles";
        public static readonly string TemplatesRootFolderPath = $"{ProfilesRootFolderPath}{DirectorySeparatorChar}Templates";
        public static readonly string CollectionsRootFolderPath = $"{TemplatesRootFolderPath}{DirectorySeparatorChar}Collections";
    }
}