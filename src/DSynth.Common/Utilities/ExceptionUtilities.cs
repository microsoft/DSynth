/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Globalization;

namespace DSynth.Common.Utilities
{
    public class ExceptionUtilities
    {
        public static string GetFormattedMessage(string messageTemplate, params object[] tokens)
        {
            return String.Format(CultureInfo.InvariantCulture, messageTemplate, tokens);
        }
    }
}