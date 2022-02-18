/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

namespace DSynth.Sink
{
    public static class Resources
    {
        public static class SinkBase
        {
            // Properties
            public const string DateTimeTemplateFormat = "yyyy/MM/dd/HH/mm/ss/fff";
            public const string DateTimeTemplateDelimeter = "/";
            public const string YearToken = "{year}";
            public const string MonthToken = "{month}";
            public const string DayToken = "{day}";
            public const string HourToken = "{hour}";
            public const string MinuteToken = "{minute}";
            public const string SecondToken = "{second}";
            public const string MillisecondToken = "{millisecond}";
            public const string EnvVarDelimeter = ":";
            public static string EnvVarIdentifier = $"env{EnvVarDelimeter}";
            public const string HeaderKey = "Header";
            public const string FilenameSuffixKey = "FileNameSuffix";

            // Exceptions
            public const string UnableToGetSinkConfig = "Initialize :: Unable to get the sink configuration for sink type '{0}' and provider '{1}'";
            public const string ExUnableToSendToSink = "RunAsync :: Sink type '{0}' for provider '{1}' is unable to write/send to sink with the following configuration '{2}', exception '{3}'";
        }
    }
}