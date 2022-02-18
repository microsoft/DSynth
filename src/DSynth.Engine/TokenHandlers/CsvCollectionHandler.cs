/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using DSynth.Common.Utilities;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace DSynth.Engine.TokenHandlers
{
    public class CsvCollectionHandler : TokenHandlerBase
    {
        private static ConcurrentDictionary<string, object> _trackedDict = new ConcurrentDictionary<string, object>();
        private static ConcurrentDictionary<string, int> _collectionIndexDict = new ConcurrentDictionary<string, int>();
        private string _collectionsFileName;
        private string _replacementValue;
        private decimal _replacementRangeLimit;
        private string _formattedTrackedKey;
        private object _referenceValue;
        private bool _disposed;
        private static object _lockObject = new object();

        public CsvCollectionHandler(TokenDescriptor tokenDescriptor, string providerName, TemplateData templateData)
            : base(tokenDescriptor, providerName, templateData)
        {
            switch (SourceType)
            {
                case EngineSourceType.Collection:
                    ValidateAndSetParameters(TokenDescriptor);
                    break;

                case EngineSourceType.Tracked:
                    ValidateAndSetTrackedParameters(TokenDescriptor);
                    break;

                case EngineSourceType.TrackedLimit:
                    ValidateAndSetTrackedLimitParameters(TokenDescriptor);
                    break;

                case EngineSourceType.Reference:
                    ValidateAndSetReferenceParameters(TokenDescriptor);
                    break;

                default:
                    ThrowEngineSourceTypeNotSupportedForHandler();
                    break;
            }
        }

        public override string GetReplacementValue()
        {
            string ret = String.Empty;
            switch (SourceType)
            {
                case EngineSourceType.Collection:
                    ret = ExtractValueByKey(
                        GetNextValueFromCollection(
                            TemplateData.Collection(_collectionsFileName), ProviderName, _collectionsFileName), _replacementValue);
                    break;

                case EngineSourceType.Tracked:
                    IEnumerable<KeyValuePair<string, object>> csvRowDataTracked =
                        GetNextValueFromCollection(TemplateData.Collection(_collectionsFileName), ProviderName, _collectionsFileName);
                    ret = ExtractValueByKey(csvRowDataTracked, _replacementValue);
                    _referenceValue = csvRowDataTracked;
                    _trackedDict[_formattedTrackedKey] = csvRowDataTracked;
                    break;

                case EngineSourceType.TrackedLimit:
                    IEnumerable<KeyValuePair<string, object>> csvRowDataTrackedLimit =
                        GetNextValueFromCollection(TemplateData.Collection(_collectionsFileName), ProviderName, _collectionsFileName, _replacementRangeLimit);
                    ret = ExtractValueByKey(csvRowDataTrackedLimit, _replacementValue);
                    _referenceValue = csvRowDataTrackedLimit;
                    _trackedDict[_formattedTrackedKey] = csvRowDataTrackedLimit;
                    break;

                case EngineSourceType.Reference:
                    var csvRowDataRef = (IEnumerable<KeyValuePair<string, object>>)_referenceValue;
                    ret = ExtractValueByKey(csvRowDataRef, _replacementValue);
                    break;
            }

            return ret;
        }

        private static IEnumerable<KeyValuePair<string, object>> GetNextValueFromCollection(dynamic collection, string providerName, string collectionsFileName, decimal limitRange = 0)
        {
            IEnumerable<KeyValuePair<string, object>> ret;

            try
            {
                string key = providerName + collectionsFileName;
                int collectionLength = collection.Length;
                int maxLines = collectionLength;

                if (limitRange > 0)
                {
                    maxLines = (int)Math.Floor(limitRange * collectionLength);
                }

                if (_collectionIndexDict.TryGetValue(key, out int index) && index <= maxLines - 1)
                {
                    ret = collection[index];
                    _collectionIndexDict[key] = ++index;
                }
                else
                {
                    index = 0;
                    ret = collection[index];
                    _collectionIndexDict[key] = ++index;
                }
            }
            catch (Exception ex)
            {
                string formattedExMessage = ExceptionUtilities.GetFormattedMessage(
                    Resources.CsvCollectionHandler.ExUnableToGetValueFromCollection);

                throw new TokenHandlerException(formattedExMessage, ex);
            }

            return ret;
        }

        public string ExtractValueByKey(IEnumerable<KeyValuePair<string, object>> keyValuePairs, string key)
        {
            try
            {
                return keyValuePairs
                    .Where(e => e.Key == key)
                    .Select(e => e.Value)
                    .SingleOrDefault()
                    .ToString();
            }
            catch (Exception ex)
            {
                string formattedExMessage = ExceptionUtilities.GetFormattedMessage(
                    Resources.CsvCollectionHandler.ExKeyNotFound,
                    key, _collectionsFileName, ProviderName);

                throw new TokenHandlerException(formattedExMessage, ex);
            }
        }

        private void ValidateAndSetParameters(TokenDescriptor tokenDescriptor)
        {
            ValidateParameterCount(Resources.CsvCollectionHandler.ExpectedParameterCount);

            // Validate collection file name
            var collectionsName = tokenDescriptor.TokenParameters[2];
            _collectionsFileName = collectionsName + Resources.TemplateDataProvider.CsvCollectionPattern;
            if (String.IsNullOrWhiteSpace(collectionsName))
            {
                ThrowParameterException(collectionsName);
            }

            // Validate replacement value
            _replacementValue = tokenDescriptor.TokenParameters[3];
            if (String.IsNullOrWhiteSpace(_replacementValue))
            {
                ThrowParameterException(_replacementValue);
            }
        }

        private void ValidateAndSetTrackedParameters(TokenDescriptor tokenDescriptor)
        {
            ValidateParameterCount(Resources.CsvCollectionHandler.ExpectedParameterCountWithTracked);

            // Validate collection file name
            var collectionsName = tokenDescriptor.TokenParameters[2];
            _collectionsFileName = collectionsName + Resources.TemplateDataProvider.CsvCollectionPattern;
            if (String.IsNullOrWhiteSpace(collectionsName))
            {
                ThrowParameterException(collectionsName);
            }

            // Validate replacement value
            _replacementValue = tokenDescriptor.TokenParameters[3];
            if (String.IsNullOrWhiteSpace(_replacementValue))
            {
                ThrowParameterException(_replacementValue);
            }

            // Validate tracked key
            string trackedKey = tokenDescriptor.TokenParameters[4];
            if (String.IsNullOrEmpty(trackedKey))
            {
                ThrowParameterException(trackedKey);
            }
            else
            {
                _formattedTrackedKey = GetFormattedTrackedKey(trackedKey, isThreadUnique: true, Resources.TemplateDataProvider.CsvCollectionPattern);
            }
        }

        private void ValidateAndSetTrackedLimitParameters(TokenDescriptor tokenDescriptor)
        {
            ValidateParameterCount(Resources.CsvCollectionHandler.ExpectedParameterCountWithTrackedLimit);

            // Validate replacement range limit
            var replacementRangeLimit = tokenDescriptor.TokenParameters[2];
            if (Decimal.TryParse(replacementRangeLimit, out decimal parsedRange))
            {
                if (parsedRange < 1 || parsedRange > 100)
                {
                    ThrowParameterException(replacementRangeLimit);
                }
                else
                {
                    _replacementRangeLimit = (parsedRange / 100);
                }
            }
            else
            {
                ThrowParameterException(replacementRangeLimit);
            }

            // Validate collection file name
            var collectionsName = tokenDescriptor.TokenParameters[3];
            _collectionsFileName = collectionsName + Resources.TemplateDataProvider.CsvCollectionPattern;
            if (String.IsNullOrWhiteSpace(collectionsName))
            {
                ThrowParameterException(collectionsName);
            }

            // Validate replacement value
            _replacementValue = tokenDescriptor.TokenParameters[4];
            if (String.IsNullOrWhiteSpace(_replacementValue))
            {
                ThrowParameterException(_replacementValue);
            }

            // Validate tracked key
            string trackedKey = tokenDescriptor.TokenParameters[5];
            if (String.IsNullOrEmpty(trackedKey))
            {
                ThrowParameterException(trackedKey);
            }
            else
            {
                _formattedTrackedKey = GetFormattedTrackedKey(trackedKey, isThreadUnique: true, Resources.TemplateDataProvider.CsvCollectionPattern);
            }
        }

        private void ValidateAndSetReferenceParameters(TokenDescriptor tokenDescriptor)
        {
            ValidateParameterCount(Resources.CsvCollectionHandler.ExpectedParameterCountWithReference);

            // Validate replacement value
            _replacementValue = tokenDescriptor.TokenParameters[2];
            if (String.IsNullOrWhiteSpace(_replacementValue))
            {
                ThrowParameterException(_replacementValue);
            }

            // Validate tracked key
            string trackedKey = tokenDescriptor.TokenParameters[3];
            if (!_trackedDict.TryGetValue(GetFormattedTrackedKey(trackedKey, isThreadUnique: true, Resources.TemplateDataProvider.CsvCollectionPattern), out _referenceValue))
            {
                ThrowParameterException(trackedKey);
            }
        }

        internal static void Reset()
        {
            _trackedDict.Clear();
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            _collectionsFileName = null;
            _replacementValue = null;
            _formattedTrackedKey = null;
            _referenceValue = null;
            _replacementValue = null;
            _disposed = true;

            base.Dispose(disposing);
        }
    }
}