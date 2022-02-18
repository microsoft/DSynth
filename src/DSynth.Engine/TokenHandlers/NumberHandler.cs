/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Concurrent;

namespace DSynth.Engine.TokenHandlers
{
    public class NumberHandler : TokenHandlerBase
    {
        private static ConcurrentDictionary<string, double> _trackedDict = new ConcurrentDictionary<string, double>();
        private double _referenceValue;
        private int _precision;
        private double _startAt;
        private double _endAt;
        private double _minRange;
        private double _maxRange;
        private int _replacementWeight;
        private string _formattedTrackedKey;
        private bool _disposed;

        public NumberHandler(TokenDescriptor tokenDescriptor, string providerName)
            : base(tokenDescriptor, providerName, templateData: null)
        {
            switch (SourceType)
            {
                case EngineSourceType.Range:
                    ValidateAndSetParameters(tokenDescriptor);
                    break;

                case EngineSourceType.IncrementTracked:
                    ValidateAndSetTrackedParameters(tokenDescriptor);
                    break;

                case EngineSourceType.DecrementTracked:
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
            double ret = 0.0;
            switch (SourceType)
            {
                case EngineSourceType.Range:
                    ret = TokenHandlerHelpers.GetNextRandomDouble(_minRange, _maxRange);
                    ret = Math.Round(ret, _precision);
                    break;

                case EngineSourceType.IncrementTracked:
                    ret = GetNextIncrement();
                    break;

                case EngineSourceType.DecrementTracked:
                    ret = GetNextDecrement();
                    break;

                case EngineSourceType.Reference:
                    ret = _referenceValue;
                    break;

                default:
                    ret = -0.00000;
                    break;
            }

            return ret.ToString();
        }

        private double GetNextIncrement()
        {
            double ret = 0.0;
            if (_trackedDict.TryGetValue(_formattedTrackedKey, out double lastValue))
            {
                if (lastValue >= _endAt)
                    return _endAt;

                if (!TokenHandlerHelpers.ShouldDeviate(_replacementWeight))
                    return lastValue;

                double nextIncrement = TokenHandlerHelpers.GetNextRandomNumber(_minRange, _maxRange);
                ret = lastValue += nextIncrement;
                ret = ret >= _endAt ? _endAt : ret;
                _trackedDict[_formattedTrackedKey] = ret;
            }
            else
            {
                ret = _startAt;
                _trackedDict[_formattedTrackedKey] = ret;
            }

            return ret;
        }

        private double GetNextDecrement()
        {
            double ret = 0.0;
            if (_trackedDict.TryGetValue(_formattedTrackedKey, out double lastValue))
            {
                if (lastValue <= _endAt)
                    return _endAt;

                if (!TokenHandlerHelpers.ShouldDeviate(_replacementWeight))
                    return lastValue;

                double nextDecrement = TokenHandlerHelpers.GetNextRandomNumber(_minRange, _maxRange);
                ret = lastValue -= nextDecrement;
                ret = ret <= _endAt ? _endAt : ret;
                _trackedDict[_formattedTrackedKey] = ret;
            }
            else
            {
                ret = _startAt;
                _trackedDict[_formattedTrackedKey] = ret;
            }

            return ret;
        }

        private void ValidateAndSetTrackedParameters(TokenDescriptor tokenDescriptor)
        {
            ValidateParameterCount(Resources.DoubleHandler.ExpectedParameterCountWithDeviation);

            // Validate replacement value range
            string replacementValueRange = tokenDescriptor.TokenParameters[2];
            string[] ranges = replacementValueRange.Split(Resources.TokenHandlerBase.RangeDelimeter);
            if (ranges.Length != 2)
            {
                ThrowParameterException(replacementValueRange);
            }
            else
            {
                try
                {
                    _startAt = Convert.ToDouble(ranges[0]);
                    _endAt = Convert.ToDouble(ranges[1]);
                }
                catch (Exception)
                {
                    ThrowParameterException(replacementValueRange);
                }
            }

            // Validate replacement range
            string replacementRange = tokenDescriptor.TokenParameters[3];
            ranges = replacementRange.Split(Resources.TokenHandlerBase.RangeDelimeter);
            if (ranges.Length != 2)
            {
                ThrowParameterException(replacementRange);
            }
            else
            {
                try
                {
                    _minRange = Convert.ToDouble(ranges[0]);
                    _maxRange = Convert.ToDouble(ranges[1]);
                }
                catch (Exception)
                {
                    ThrowParameterException(replacementRange);
                }
            }

            // Validate replacement weight
            string replacementWeight = tokenDescriptor.TokenParameters[4];
            if (!Int32.TryParse(replacementWeight, out _replacementWeight))
            {
                ThrowParameterException(replacementWeight);
            }

            // Validate tracked key
            string trackedKey = tokenDescriptor.TokenParameters[5];
            if (String.IsNullOrEmpty(trackedKey))
            {
                ThrowParameterException(trackedKey);
            }
            else
            {
                _formattedTrackedKey = GetFormattedTrackedKey(trackedKey);
            }
        }

        private void ValidateAndSetParameters(TokenDescriptor tokenDescriptor)
        {
            ValidateParameterCount(Resources.DoubleHandler.ExpectedParameterCount);

            // Validate replacement value
            string replacementValue = tokenDescriptor.TokenParameters[2];
            if (String.IsNullOrWhiteSpace(replacementValue))
            {
                ThrowParameterException(replacementValue);
            }

            // Validate range
            string[] ranges = replacementValue.Split(Resources.TokenHandlerBase.RangeDelimeter);
            if (ranges.Length != 2)
            {
                ThrowParameterException(replacementValue);
            }
            else
            {
                try
                {
                    _minRange = Convert.ToDouble(ranges[0]);
                    _maxRange = Convert.ToDouble(ranges[1]);
                }
                catch (Exception)
                {
                    ThrowParameterException(replacementValue);
                }
            }

            // Validate precision
            string precision = tokenDescriptor.TokenParameters[3];
            if (!Int32.TryParse(precision, out _precision))
            {
                ThrowParameterException(precision);
            }
        }

        private void ValidateAndSetReferenceParameters(TokenDescriptor tokenDescriptor)
        {
            ValidateParameterCount(Resources.DoubleHandler.ExpectedParameterCountWithReference);

            // Validate tracked key
            string trackedKey = tokenDescriptor.TokenParameters[2];
            if (!_trackedDict.TryGetValue(GetFormattedTrackedKey(trackedKey), out _referenceValue))
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

            _formattedTrackedKey = null;
            _disposed = true;

            base.Dispose(disposing);
        }
    }
}