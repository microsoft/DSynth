/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Globalization;
using DSynth.Engine.TokenHandlers;
using Xunit;

namespace DSynth.Engine.Tests.UnitTests
{
    public class TimestampHandlerTests
    {
        private const string _unitTestProviderName = "UnitTestProviderName";

        [Fact]
        public void ShouldFailWithInvalidParameterCount()
        {
            string exMessage = String.Empty;
            string invalidToken = "{{Timestamp:DateTime}}";
            string expectedExMessage = $"ValidateParameterCount :: Token provider 'TimestampHandler' for provider '{_unitTestProviderName}' expected '3' token parameters, but got '2' for token '{invalidToken}'";
            TokenDescriptor descriptor = new TokenDescriptor(invalidToken);

            try
            {
                ITokenHandler handler = TokenHandlerFactory.GetHandler(descriptor, _unitTestProviderName, null);
            }
            catch (TokenHandlerException ex)
            {
                exMessage = ex.Message;
            }

            Assert.Equal(exMessage, expectedExMessage);
        }

        [Fact]
        public void ShouldGetReplacementValueAsUnixTimestamp()
        {
            string token = "{{Timestamp:DateTime:UnixTimeInMs}}";
            TokenDescriptor descriptor = new TokenDescriptor(token);
            ITokenHandler handler = TokenHandlerFactory.GetHandler(descriptor, _unitTestProviderName, null);
            string result = handler.GetReplacementValue();
            Assert.True(Int64.TryParse(result, NumberStyles.None, CultureInfo.InvariantCulture.NumberFormat, out long resultAsLong));
        }

        [Fact]
        public void ShouldGetReplacementValueAsISO8601Timestamp()
        {
            string token = "{{Timestamp:DateTime:UTCISO8601}}";
            TokenDescriptor descriptor = new TokenDescriptor(token);
            ITokenHandler handler = TokenHandlerFactory.GetHandler(descriptor, _unitTestProviderName, null);
            string result = handler.GetReplacementValue();
            Assert.True(DateTimeOffset.TryParseExact(result, "yyyy-MM-ddTHH:mm:ss.fffffffZ", CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.None, out DateTimeOffset iso8601Format));
        }

        [Fact]
        public void ShouldFailWithInvalidParameterCountForDateTimeTracked()
        {
            string exMessage = String.Empty;
            string invalidToken = "{{Timestamp:Tracked}}";
            string expectedExMessage = $"ValidateParameterCount :: Token provider 'TimestampHandler' for provider '{_unitTestProviderName}' expected '4' token parameters, but got '2' for token '{invalidToken}'";
            TokenDescriptor descriptor = new TokenDescriptor(invalidToken);

            try
            {
                ITokenHandler handler = TokenHandlerFactory.GetHandler(descriptor, _unitTestProviderName, null);
            }
            catch (TokenHandlerException ex)
            {
                exMessage = ex.Message;
            }

            Assert.Equal(exMessage, expectedExMessage);
        }

        [Fact]
        public void ShouldGetTrackedReplacementValueAsUnixTimestamp()
        {
            string tokenTracked = "{{Timestamp:Tracked:UnixTimeInMs:unitTestTrackingKey}}";
            TokenDescriptor descriptorTracked = new TokenDescriptor(tokenTracked);
            ITokenHandler handlerTracked = TokenHandlerFactory.GetHandler(descriptorTracked, _unitTestProviderName, null);
            string resultTracked = handlerTracked.GetReplacementValue();

            string tokenReference = "{{Timestamp:Reference:unitTestTrackingKey}}";
            TokenDescriptor descriptorReference = new TokenDescriptor(tokenReference);
            ITokenHandler handlerReference = TokenHandlerFactory.GetHandler(descriptorReference, _unitTestProviderName, null);
            string resultReference = handlerReference.GetReplacementValue();

            Assert.Equal(resultTracked, resultReference);
        }

        [Fact]
        public void ShouldGetTrackedReplacementValueAsISO8601Timestamp()
        {
            string tokenTracked = "{{Timestamp:Tracked:UTCISO8601:unitTestTrackingKey}}";
            TokenDescriptor descriptorTracked = new TokenDescriptor(tokenTracked);
            ITokenHandler handlerTracked = TokenHandlerFactory.GetHandler(descriptorTracked, _unitTestProviderName, null);
            string resultTracked = handlerTracked.GetReplacementValue();

            string tokenReference = "{{Timestamp:Reference:unitTestTrackingKey}}";
            TokenDescriptor descriptorReference = new TokenDescriptor(tokenReference);
            ITokenHandler handlerReference = TokenHandlerFactory.GetHandler(descriptorReference, _unitTestProviderName, null);
            string resultReference = handlerReference.GetReplacementValue();

            Assert.Equal(resultTracked, resultReference);
        }

        [Fact]
        public void ShouldFailWithInvalidParameterCountForDateTimeReference()
        {
            string exMessage = String.Empty;
            string invalidToken = "{{Timestamp:Reference}}";
            string expectedExMessage = $"ValidateParameterCount :: Token provider 'TimestampHandler' for provider '{_unitTestProviderName}' expected '3' token parameters, but got '2' for token '{invalidToken}'";
            TokenDescriptor descriptor = new TokenDescriptor(invalidToken);

            try
            {
                ITokenHandler handler = TokenHandlerFactory.GetHandler(descriptor, _unitTestProviderName, null);
            }
            catch (TokenHandlerException ex)
            {
                exMessage = ex.Message;
            }

            Assert.Equal(exMessage, expectedExMessage);
        }
    }
}