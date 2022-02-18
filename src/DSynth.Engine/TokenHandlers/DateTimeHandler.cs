/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;

namespace DSynth.Engine.TokenHandlers
{
    public class DateTimeHandler : TokenHandlerBase
    {
        private DateTimeFormat _format;
        private DateTimeComponent _component;
        private int _start;
        private int _end;

        public DateTimeHandler(TokenDescriptor tokenDescriptor, string providerName)
            : base(tokenDescriptor, providerName, templateData: null)
        {
            switch (SourceType)
            {
                case EngineSourceType.Range:
                    ValidateAndSetParameters(tokenDescriptor);
                    break;

                default:
                    ThrowEngineSourceTypeNotSupportedForHandler();
                    break;
            }
        }

        public override string GetReplacementValue()
        {
            int adjustedValue = TokenHandlerHelpers.GetNextRandomInt(_start, _end);
            DateTimeOffset timestampNow = DateTimeOffset.UtcNow;
            DateTimeOffset dt = DateTimeOffset.MinValue;

            switch (_component)
            {
                case DateTimeComponent.Years:
                    dt = timestampNow.AddYears(adjustedValue);
                    break;

                case DateTimeComponent.Months:
                    dt = timestampNow.AddMonths(adjustedValue);
                    break;

                case DateTimeComponent.Days:
                    dt = timestampNow.AddDays(adjustedValue);
                    break;

                case DateTimeComponent.Hours:
                    dt = timestampNow.AddHours(adjustedValue);
                    break;

                case DateTimeComponent.Minutes:
                    dt = timestampNow.AddMinutes(adjustedValue);
                    break;

                case DateTimeComponent.Seconds:
                    dt = timestampNow.AddSeconds(adjustedValue);
                    break;

                case DateTimeComponent.Milliseconds:
                    dt = timestampNow.AddMilliseconds(adjustedValue);
                    break;
            }
            var ret = FormatDateTime(dt);
            return ret;
        }

        private string FormatDateTime(DateTimeOffset dateTimeOffset)
        {
            switch (_format)
            {
                case (DateTimeFormat.UTCISO8601):
                    return dateTimeOffset.ToString(Resources.TokenHandlerBase.Iso8601Format);

                case (DateTimeFormat.UnixTimeInMs):
                    return dateTimeOffset.ToUnixTimeMilliseconds().ToString();

                case (DateTimeFormat.DateISO8601):
                    return dateTimeOffset.ToString("yyyy-MM-dd");

                default:
                    return DateTimeOffset.MinValue.ToString(Resources.TokenHandlerBase.Iso8601Format);
            }
        }

        private void ValidateAndSetParameters(TokenDescriptor tokenDescriptor)
        {
            ValidateParameterCount(Resources.DateTimeHandler.ExpectedParameterCount);

            // Validate date time format
            string dateTimeFormat = tokenDescriptor.TokenParameters[2];
            if (!Enum.TryParse<DateTimeFormat>(dateTimeFormat, true, out _format))
            {
                ThrowParameterException(dateTimeFormat);
            }

            // Validate component
            string dateTimeComponent = tokenDescriptor.TokenParameters[3];
            if (!Enum.TryParse<DateTimeComponent>(dateTimeComponent, out _component))
            {
                ThrowParameterException(dateTimeComponent);
            }

            // Validate range
            string replacementRange = tokenDescriptor.TokenParameters[4];
            string[] ranges = replacementRange.Split(Resources.TokenHandlerBase.RangeDelimeter);
            if (ranges.Length != 2)
            {
                ThrowParameterException(replacementRange);
            }
            else
            {
                try
                {
                    _start = Convert.ToInt32(ranges[0]);
                    _end = Convert.ToInt32(ranges[1]);
                }
                catch (Exception)
                {
                    ThrowParameterException(replacementRange);
                }
            }
        }
    }
}