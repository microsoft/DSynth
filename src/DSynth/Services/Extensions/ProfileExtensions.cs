/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace DSynth.Services.Extesions
{
    public static class ProfileExtensions
    {
        public static string ExtractProfileNames(this IEnumerable<Profile> profiles)
        {
            return String.Join(", ", profiles.Select(p => p.Name));
        }

        public static Profile GetProfileByName(this IEnumerable<Profile> profiles, string profileName)
        {
            return profiles.Where(p => p.Name == profileName).SingleOrDefault();
        }
    }
}