/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Concurrent;

namespace DSynth.Engine.TokenHandlers
{
    public class GuidHandler : TokenHandlerBase
    {
        private static ConcurrentDictionary<string, string> _trackedDict = new ConcurrentDictionary<string, string>();
        private string _formattedTrackedKey;
        private string _referenceValue;
        private bool _disposed;
        public GuidHandler(TokenDescriptor tokenDescriptor, string providerName)
            : base(tokenDescriptor, providerName, templateData: null)
        {
            switch (SourceType)
            {
                case EngineSourceType.NewGuid:
                    // No validation needed
                    break;

                case EngineSourceType.Tracked:
                    ValidateAndSetTrackedParameters(tokenDescriptor);
                    break;

                case EngineSourceType.Reference:
                    ValidateAndSetReferenceParameters(tokenDescriptor);
                    break;

                default:
                    ThrowEngineSourceTypeNotSupportedForHandler();
                    break;
            }
        }

        public override string GetReplacementValue()
        {
            string ret = default;

            switch (SourceType)
            {
                case EngineSourceType.NewGuid:
                    ret = System.Guid.NewGuid().ToString();
                    break;

                case EngineSourceType.Tracked:
                    ret = System.Guid.NewGuid().ToString();
                    _trackedDict[_formattedTrackedKey] = ret;
                    break;

                case EngineSourceType.Reference:
                    ret = _referenceValue;
                    break;

                default:
                    ThrowEngineSourceTypeNotSupportedForHandler();
                    break;
            }

            return ret.ToString();
        }

        private void ValidateAndSetTrackedParameters(TokenDescriptor tokenDescriptor)
        {
            ValidateParameterCount(Resources.GuidHandler.ExpectedParameterCountWithTracked);

            string trackedKey = tokenDescriptor.TokenParameters[2];
            if (String.IsNullOrEmpty(trackedKey))
            {
                ThrowParameterException(trackedKey);
            }
            else
            {
                _formattedTrackedKey = GetFormattedTrackedKey(trackedKey, isThreadUnique: true);
            }
        }

        private void ValidateAndSetReferenceParameters(TokenDescriptor tokenDescriptor)
        {
            ValidateParameterCount(Resources.GuidHandler.ExpectedParameterCountWithReference);

            string trackedKey = tokenDescriptor.TokenParameters[2];
            if (!_trackedDict.TryGetValue(GetFormattedTrackedKey(trackedKey, isThreadUnique: true), out _referenceValue))
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

            _disposed = true;

            base.Dispose(disposing);
        }
    }
}