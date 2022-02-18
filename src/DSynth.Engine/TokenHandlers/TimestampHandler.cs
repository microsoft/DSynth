/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Concurrent;

namespace DSynth.Engine.TokenHandlers
{
    public class TimestampHandler : TokenHandlerBase
    {
        private static ConcurrentDictionary<string, string> _trackedDict = new ConcurrentDictionary<string, string>();
        private string _referenceValue;
        private DateTimeFormat _format;
        private string _formattedTrackedKey;
        private bool _disposed;

        public TimestampHandler(TokenDescriptor tokenDescriptor, string providerName)
            : base(tokenDescriptor, providerName, templateData: null)
        {
            switch (SourceType)
            {
                case EngineSourceType.Tracked:
                    ValidateAndSetTrackedParameters(tokenDescriptor);
                    break;

                case EngineSourceType.Reference:
                    ValidateAndSetReferenceParameters(tokenDescriptor);
                    break;

                case EngineSourceType.DateTime:
                    ValidateAndSetParameters(tokenDescriptor);
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
                case EngineSourceType.Tracked:
                    ret = GetNextDateTimeAsString();
                    _trackedDict[_formattedTrackedKey] = ret;
                    break;

                case EngineSourceType.Reference:
                    ret = _referenceValue;
                    break;

                case EngineSourceType.DateTime:
                    ret = GetNextDateTimeAsString();
                    break;

                default:
                    ret = default;
                    break;
            }

            return ret;
        }

        private string GetNextDateTimeAsString()
        {
            switch (_format)
            {
                case DateTimeFormat.UnixTimeInMs:
                    return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

                case DateTimeFormat.UTCISO8601:
                    return DateTimeOffset.UtcNow.ToString(Resources.TokenHandlerBase.Iso8601Format);

                default:
                    return String.Empty;
            }
        }

        private void ValidateAndSetTrackedParameters(TokenDescriptor tokenDescriptor)
        {
            ValidateParameterCount(Resources.TimestampHandler.ExpectedParameterCountWithTracked);

            // Validate date time format
            string dateTimeFormat = tokenDescriptor.TokenParameters[2];
            if (!Enum.TryParse<DateTimeFormat>(dateTimeFormat, true, out _format))
            {
                ThrowParameterException(dateTimeFormat);
            }

            // Validate tracked key
            string trackedKey = tokenDescriptor.TokenParameters[3];
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
            ValidateParameterCount(Resources.TimestampHandler.ExpectedParameterCountWithReference);

            // Validate tracked key
            string trackedKey = tokenDescriptor.TokenParameters[2];
            if (!_trackedDict.TryGetValue(GetFormattedTrackedKey(trackedKey, isThreadUnique: true), out _referenceValue))
            {
                ThrowParameterException(trackedKey);
            }
        }

        private void ValidateAndSetParameters(TokenDescriptor tokenDescriptor)
        {
            ValidateParameterCount(Resources.TimestampHandler.ExpectedParameterCount);

            // Validate date time format
            string dateTimeFormat = tokenDescriptor.TokenParameters[2];
            if (!Enum.TryParse<DateTimeFormat>(dateTimeFormat, true, out _format))
            {
                ThrowParameterException(dateTimeFormat);
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

            _referenceValue = null;
            _formattedTrackedKey = null;
            _disposed = true;

            base.Dispose(disposing);
        }
    }
}