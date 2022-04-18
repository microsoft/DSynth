/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using DSynth.Common.Utilities;
using DSynth.Engine.TokenHandlers;

namespace DSynth.Engine
{
    public static class Resources
    {
        public static class TemplateDataProvider
        {
            /// <summary>
            /// The search pattern used to find all relevant JSON files under the Collections folder
            /// </summary>
            public const string JsonCollectionPattern = ".collections.json";

            /// <summary>
            /// The search pattern used to find all relevant CSB files under the Collections folder
            /// </summary>
            public const string CsvCollectionPattern = ".collections.csv";

            /// <summary>
            /// The search pattern used to find all relevant files under the Templates folder
            /// </summary>
            public const string TemplatePattern = ".template.";

            /// <summary>
            /// The regex pattern to match metadata for a given template
            /// </summary>
            public const string TemplateMetadataPattern = "::(.*?)\n";

            // Exceptions
            public const string ExUnableToLoadCollections = "LoadCollections :: Unable to load contents from file '{0}', exception {1}";
        }

        public static class TemplateData
        {
            // Properties

            /// <summary>
            /// The token used to signify template metadata. Currently, this is used only to call out
            /// the header for CSV files and can be extended to allow other metadata as needed.
            /// </summary>
            public const string TemplateMetadataLineToken = "::";

            /// <summary>
            /// Use Unix line endings for templates.
            /// </summary>
            public const string TemplateLineEndingToken = "\n";

            /// <summary>
            /// The delimeter used to separate key and value for constructing the metadata dictionary
            /// </summary>
            public const string TemplateMetadataDelimeter = "=";

            /// <summary>
            /// The path to the collections made available to the template
            /// </summary>
            public const string TemplateCollectionPathKeyName = "CollectionsFileName";

            /// <summary>
            /// The name of the collections property when the source of jsonCollection is specified, 
            /// </summary>
            public const string JsonCollectionsObjectName = "collections";

            // Exception messages

            public const string ExUnableToParseTemplateStructure = "ParseTemplateStructure :: Unable to parse template structure of the provided template '{0}'";
            public const string ExUnableToGetCollectionByName = "GetCollectionByName :: Unable to get collection with file name of '{0}'";
        }

        public static class TokenDescriptor
        {
            // Properties

            /// <summary>
            /// The delimeter used to separate parameters from the template, i.e. Object:jsonCollection:uic
            /// </summary>
            public const string ReplacementTokenDelimeter = ":";

            /// <summary>
            /// The number of expected parameters to be able to construct TemplateDescriptor
            /// </summary>
            public const int ExpectedParameterCount = 2;

            //Exception messages

            public static string ExUnexpectedNumParameters = "TokenDescriptor :: Unexpected parameter count of '{0}', expecting '{1}' from parameter string '{2}'";
            public const string ArgumentExceptionRawParameters = "ExtractParameters :: '{0}' cannot be null or whitespace,";
        }

        public static class EngineBase
        {
            // Properties

            /// <summary>
            /// RegEx pattern to match for token replacement, i.e. pattern will match string
            /// between {{ and }}
            /// </summary>
            public const string ReplacementTokenRegexPattern = "(?:\\{{)(.*?)(?:\\}})";

            // Exceptions

            public const string ExUnableToBuildPayload = "GetTemplateWithReplacedTokens :: Unable to build payload for provider '{0}', please fix the errors and try running again.";
            public static string ExUnsupportedType = $"GetHandler :: Unsupported type for the given token value of '{{0}}'. Example of expected types {EnumUtilities.GetAllTypesAsCSVString<TokenHandlerType>()}";
            public const string ExUnableToInitialize = "Initialize :: Unable to initialize engine of type '{0}', see inner exception for details...";

            // Log messages

            public const string InfoInitializeComplete = "Initialize complete for engine type '{EngineType}',";
            public const string InfoInitializeEngines = "Initializing engine type '{EngineType}' for provider: '{ProviderType}',";
        }

        public static class EngineFactory
        {
            // Exceptions

            public const string ExUnableToBuildForType = "GetDSynthEngine :: Unable to build IDSynthEngine for type '{0}'";
        }

        public static class TokenHandlerBase
        {
            public const string RangeDelimeter = "..";
            public const string Iso8601Format = "yyyy-MM-ddTHH:mm:ss.fffffffZ";
            public const string ExUnableToValidateParameterCount = "ValidateParameterCount :: Token provider '{0}' for provider '{1}' expected '{2}' token parameters, but got '{3}' for token '{4}'";
            public const string ExUnableToParseParameters = "ValidateParameters :: Token provider '{0}' for provider '{1}' was unable to parse parameters from token '{2}' with a given value of '{3}'";
            public const string ExUnsupportedSourceTypeWithHandler = "HandlerCtor :: The selected source type of '{0}' is not supported with handler '{1}'";
        }

        public static class GuidHandler
        {
            public const int ExpectedParameterCountWithTracked = 3;
            public const int ExpectedParameterCountWithReference = 3;
        }

        public static class DoubleHandler
        {
            // Properties

            public const int ExpectedParameterCount = 4;
            public const int ExpectedParameterCountWithReference = 3;
            public const int ExpectedParameterCountWithDeviation = 6;
        }

        public static class Int64Handler
        {
            // Properties

            public const int ExpectedParameterCountWithDeviation = 6;
            public const int ExpectedParameterCountWithReference = 3;
        }

        public static class JsonCollectionHandler
        {
            // Properties

            public const int ExpectedParameterCount = 4;
            public const int ExpectedParameterCountWithTracked = 5;
            public const int ExpectedParameterCountWithReference = 3;

            // Exceptions

            public const string ExUnableToGetValueFromCollection = "GetValueFromCollection :: Unable to get value from collection";
        }

        public static class CsvCollectionHandler
        {
            // Properties

            public const int ExpectedParameterCount = 4;
            public const int ExpectedParameterCountWithTracked = 5;
            public const int ExpectedParameterCountWithTrackedLimit = 6;
            public const int ExpectedParameterCountWithReference = 4;

            // Exceptions

            public const string ExKeyNotFound = "ExtractValueByKey :: The following key '{0}' was not found from collection '{1}' for provider '{2}'";
            public const string ExUnableToGetValueFromCollection = "GetValueFromCollection :: Unable to get value from collection";
        }

        public static class TimestampHandler
        {
            // Properties

            public const int ExpectedParameterCount = 3;
            public const int ExpectedParameterCountWithTracked = 4;
            public const int ExpectedParameterCountWithReference = 3;
        }

        public static class DateTimeHandler
        {
            // Properties

            public const int ExpectedParameterCount = 5;
        }

        public static class NestedHandler
        {
            // Properties

            public const int ExpectedParameterCount = 4;
        }

        public static class CoordinateHandler
        {
            // Properties

            public const int ExpectedParameterCount = 6;

            // Exceptions

            public const string ExInvalidFormat = "Invalid format of type '{0}', available formats are '{1}'";
        }
    }
}