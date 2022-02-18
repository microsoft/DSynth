/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Globalization;
using DSynth.Engine.TokenHandlers;
using Xunit;

namespace DSynth.Engine.Tests
{
    public class DateTimeHandlerTests
    {
        private const string _unitTestProviderName = "UnitTestProviderName";
        private const string _iso8601Format = "yyyy-MM-ddTHH:mm:ss.fffffffZ";

        [Fact]
        public void ShouldFailWithInvalidParameterCount()
        {
            string exMessage = String.Empty;
            string invalidToken = "{{DateTime:Range}}";
            string expectedExMessage = $"ValidateParameterCount :: Token provider 'DateTimeHandler' for provider '{_unitTestProviderName}' expected '5' token parameters, but got '2' for token '{invalidToken}'";
            TokenDescriptor descriptor = new TokenDescriptor(invalidToken);

            try
            {
                ITokenHandler handler = TokenHandlerFactory.GetHandler(descriptor, _unitTestProviderName, null);
            }
            catch (TokenHandlerException ex)
            {
                exMessage = ex.Message;
            }

            Assert.Equal(expectedExMessage, exMessage);
        }

        [Fact]
        public void ShouldGetReplacementValueFromRange()
        {
            string token = "{{DateTime:Range:UTCISO8601:Years:-85..-5}}";
            TokenDescriptor descriptor = new TokenDescriptor(token);
            ITokenHandler handler = TokenHandlerFactory.GetHandler(descriptor, _unitTestProviderName, null);
            string result = handler.GetReplacementValue();
            var dt = DateTimeOffset.Parse(result);
            var ranges = descriptor.TokenParameters[4].Split("..");

            var start = Convert.ToInt32(ranges[0]);
            var end = Convert.ToInt32(ranges[1]);

            var currentYearMin = DateTimeOffset.UtcNow.AddYears(start);
            var currentYearMax = DateTimeOffset.UtcNow.AddYears(end);

            Assert.True(currentYearMin.Year <= dt.Year && currentYearMax.Year >= dt.Year );
        }

        [Fact]
        public void ShouldGetReplacementValueFromRangeForMonths()
        {
            string token = "{{DateTime:Range:UTCISO8601:Months:-85..-5}}";
            TokenDescriptor descriptor = new TokenDescriptor(token);
            ITokenHandler handler = TokenHandlerFactory.GetHandler(descriptor, _unitTestProviderName, null);
            string result = handler.GetReplacementValue();
            var dt = DateTimeOffset.Parse(result);
            Assert.NotEqual(DateTimeOffset.MinValue.ToString(_iso8601Format), dt.ToString(_iso8601Format));
        }

        [Fact]
        public void ShouldGetReplacementValueFromRangeForDays()
        {
            string token = "{{DateTime:Range:UTCISO8601:Days:-85..-5}}";
            TokenDescriptor descriptor = new TokenDescriptor(token);
            ITokenHandler handler = TokenHandlerFactory.GetHandler(descriptor, _unitTestProviderName, null);
            string result = handler.GetReplacementValue();
            var dt = DateTimeOffset.Parse(result);
            Assert.NotEqual(DateTimeOffset.MinValue.ToString(_iso8601Format), dt.ToString(_iso8601Format));
        }

        [Fact]
        public void ShouldGetReplacementValueFromRangeForHours()
        {
            string token = "{{DateTime:Range:UTCISO8601:Hours:-85..-5}}";
            TokenDescriptor descriptor = new TokenDescriptor(token);
            ITokenHandler handler = TokenHandlerFactory.GetHandler(descriptor, _unitTestProviderName, null);
            string result = handler.GetReplacementValue();
            var dt = DateTimeOffset.Parse(result);
            Assert.NotEqual(DateTimeOffset.MinValue.ToString(_iso8601Format), dt.ToString(_iso8601Format));
        }

        [Fact]
        public void ShouldGetReplacementValueFromRangeForMinutes()
        {
            string token = "{{DateTime:Range:UTCISO8601:Minutes:-85..-5}}";
            TokenDescriptor descriptor = new TokenDescriptor(token);
            ITokenHandler handler = TokenHandlerFactory.GetHandler(descriptor, _unitTestProviderName, null);
            string result = handler.GetReplacementValue();
            var dt = DateTimeOffset.Parse(result);
            Assert.NotEqual(DateTimeOffset.MinValue.ToString(_iso8601Format), dt.ToString(_iso8601Format));
        }

        [Fact]
        public void ShouldGetReplacementValueFromRangeForSeconds()
        {
            string token = "{{DateTime:Range:UTCISO8601:Seconds:-85..-5}}";
            TokenDescriptor descriptor = new TokenDescriptor(token);
            ITokenHandler handler = TokenHandlerFactory.GetHandler(descriptor, _unitTestProviderName, null);
            string result = handler.GetReplacementValue();
            var dt = DateTimeOffset.Parse(result);
            Assert.NotEqual(DateTimeOffset.MinValue.ToString(_iso8601Format), dt.ToString(_iso8601Format));
        }

        [Fact]
        public void ShouldGetReplacementValueFromRangeForMilliseconds()
        {
            string token = "{{DateTime:Range:UTCISO8601:Milliseconds:-85..-5}}";
            TokenDescriptor descriptor = new TokenDescriptor(token);
            ITokenHandler handler = TokenHandlerFactory.GetHandler(descriptor, _unitTestProviderName, null);
            string result = handler.GetReplacementValue();
            var dt = DateTimeOffset.Parse(result);
            Assert.NotEqual(DateTimeOffset.MinValue.ToString(_iso8601Format), dt.ToString(_iso8601Format));
        }

        [Fact]
        public void ShouldGetReplacementValueAsUTCISO8601()
        {
            string token = "{{DateTime:Range:UTCISO8601:Years:-85..-5}}";
            TokenDescriptor descriptor = new TokenDescriptor(token);
            ITokenHandler handler = TokenHandlerFactory.GetHandler(descriptor, _unitTestProviderName, null);
            string result = handler.GetReplacementValue();
            Assert.True(DateTimeOffset.TryParseExact(result, _iso8601Format, CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.None, out DateTimeOffset iso8601Format));
        }

        [Fact]
        public void ShouldGetReplacementValueAsUnixTimeMs()
        {
            string token = "{{DateTime:Range:UnixTimeInMs:Years:-85..-5}}";
            TokenDescriptor descriptor = new TokenDescriptor(token);
            ITokenHandler handler = TokenHandlerFactory.GetHandler(descriptor, _unitTestProviderName, null);
            string result = handler.GetReplacementValue();
            Assert.True(Int64.TryParse(result, NumberStyles.Any, null, out long resultAsLong));
        }
    }
}
