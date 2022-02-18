/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;

namespace DSynth.Common.Utilities
{
    public class EnumUtilities
    {
        private const string ExUnableToParseEnumFromString = "GetEnumValueFromString :: Unable to get enum value with string of '{0}'. Available enums are '{1}'";

        public static string GetAllTypesAsCSVString<T>()
        {
            return String.Join(", ", Enum.GetNames(typeof(T)));
        }

        public static T GetEnumValueFromString<T>(string stringType) 
            where T : struct
        {
            if (!Enum.TryParse(stringType, true, out T source))
            {
                string formattedExMessage = ExceptionUtilities.GetFormattedMessage(
                    ExUnableToParseEnumFromString,
                    stringType,
                    GetAllTypesAsCSVString<T>());

                throw new EnumUtilitiesException(formattedExMessage);
            }

            return source;
        }
    }
}