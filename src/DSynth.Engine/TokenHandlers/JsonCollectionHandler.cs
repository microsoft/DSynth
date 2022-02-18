/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using Newtonsoft.Json.Linq;
using DSynth.Common.Utilities;
using System.Collections.Concurrent;

namespace DSynth.Engine.TokenHandlers
{
    public class JsonCollectionHandler : TokenHandlerBase
    {
        private static ConcurrentDictionary<string, object> _trackedDict = new ConcurrentDictionary<string, object>();
        private string _collectionsFileName;
        private string _replacementValue;
        private string _formattedTrackedKey;
        private object _referenceValue;
        private bool _disposed;

        public JsonCollectionHandler(TokenDescriptor tokenDescriptor, string providerName, TemplateData templateData)
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
                    ret = GetNextValueFromCollection(TemplateData.Collection(_collectionsFileName));
                    break;

                case EngineSourceType.Tracked:
                    ret = GetNextValueFromCollection(TemplateData.Collection(_collectionsFileName));
                    _trackedDict[_formattedTrackedKey] = ret;
                    break;

                case EngineSourceType.Reference:
                    ret = (string)_referenceValue;
                    break;

                default:
                    ThrowEngineSourceTypeNotSupportedForHandler();
                    break;
            }

            return ret;
        }

        private string GetNextValueFromCollection(dynamic collection)
        {
            string ret = String.Empty;
            try
            {
                int minIndex, maxIndex, randomIndex;

                string collectionPath = $"{Resources.TemplateData.JsonCollectionsObjectName}.{_replacementValue}";
                JArray jsonCollection = (JArray)collection.SelectToken(collectionPath);
                minIndex = 0;
                maxIndex = jsonCollection.Count;
                randomIndex = TokenHandlerHelpers.GetNextRandomInt(minIndex, maxIndex);
                ret = jsonCollection[randomIndex].ToString();
            }
            catch (Exception ex)
            {
                string formattedExMessage = ExceptionUtilities.GetFormattedMessage(
                    Resources.JsonCollectionHandler.ExUnableToGetValueFromCollection);

                throw new TokenHandlerException(formattedExMessage, ex);
            }

            return ret;
        }

        private void ValidateAndSetParameters(TokenDescriptor tokenDescriptor)
        {
            ValidateParameterCount(Resources.JsonCollectionHandler.ExpectedParameterCount);

            // Validate collection file name
            var collectionsName = tokenDescriptor.TokenParameters[2];
            _collectionsFileName = collectionsName + Resources.TemplateDataProvider.JsonCollectionPattern;
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
            ValidateParameterCount(Resources.JsonCollectionHandler.ExpectedParameterCountWithTracked);

            // Validate collection file name
            var collectionsName = tokenDescriptor.TokenParameters[2];
            _collectionsFileName = collectionsName + Resources.TemplateDataProvider.JsonCollectionPattern;
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
                _formattedTrackedKey = GetFormattedTrackedKey(trackedKey, isThreadUnique: true, Resources.TemplateDataProvider.JsonCollectionPattern);
            }
        }

        private void ValidateAndSetReferenceParameters(TokenDescriptor tokenDescriptor)
        {
            ValidateParameterCount(Resources.JsonCollectionHandler.ExpectedParameterCountWithReference);

            // Validate tracked key
            string trackedKey = tokenDescriptor.TokenParameters[2];
            if (!_trackedDict.TryGetValue(GetFormattedTrackedKey(trackedKey, isThreadUnique: true, Resources.TemplateDataProvider.JsonCollectionPattern), out _referenceValue))
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