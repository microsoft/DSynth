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
    public class NumberHandlerTests
    {
        private const string _unitTestProviderName = "UnitTestProviderName";

        [Fact]
        public void ShouldFailWithInvalidParameterCountForNumberRange()
        {
            string exMessage = String.Empty;
            string invalidToken = "{{Number:Range}}";
            string expectedExMessage = $"ValidateParameterCount :: Token provider 'NumberHandler' for provider '{_unitTestProviderName}' expected '4' token parameters, but got '2' for token '{invalidToken}'";
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
        public void ShouldGetReplacementValueAsNumberWithinAGivenRangeAndPrecision()
        {
            int precision = 3;
            int minValue = 0;
            int maxValue = 15;

            string token = $"{{{{Number:Range:{minValue}..{maxValue}:{precision}}}}}";
            TokenDescriptor descriptor = new TokenDescriptor(token);
            ITokenHandler handler = TokenHandlerFactory.GetHandler(descriptor, _unitTestProviderName, null);
            string result = handler.GetReplacementValue();
            int wholeNumber = Int32.Parse(result.Split(".")[0]);
            int resultPrecision = result.Split(".")[1].Length;

            Assert.True(wholeNumber >= minValue && wholeNumber <= maxValue);
            Assert.True(resultPrecision <= precision);
            Assert.True(Double.TryParse(result, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture.NumberFormat, out double resultAsDouble));
        }

        [Fact]
        public void ShouldFailWithInvalidParameterCountForNumberIncrementTracked()
        {
            string exMessage = String.Empty;
            string invalidToken = "{{Number:IncrementTracked}}";
            string expectedExMessage = $"ValidateParameterCount :: Token provider 'NumberHandler' for provider '{_unitTestProviderName}' expected '6' token parameters, but got '2' for token '{invalidToken}'";
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
        public void ShouldFailWithInvalidParameterCountForNumberDecrementTracked()
        {
            string exMessage = String.Empty;
            string invalidToken = "{{Number:DecrementTracked}}";
            string expectedExMessage = $"ValidateParameterCount :: Token provider 'NumberHandler' for provider '{_unitTestProviderName}' expected '6' token parameters, but got '2' for token '{invalidToken}'";
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
        public void ShouldGetReplacementValueAsIncrementedNumberForAGivenRange()
        {
            double minValue = 1.1;
            double maxValue = 2.2;

            string token = $"{{{{Number:IncrementTracked:{minValue}..{maxValue}:0.1..0.1:100:unitTestTrackingKey}}}}";
            TokenDescriptor descriptor = new TokenDescriptor(token);
            ITokenHandler handler = TokenHandlerFactory.GetHandler(descriptor, _unitTestProviderName, null);

            // Run twice as the first time uses min value as the starting number
            // and we want to validate that incrementing is working properly.
            string result = handler.GetReplacementValue();
            result = handler.GetReplacementValue();

            Assert.True(Double.TryParse(result, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture.NumberFormat, out double resultAsDouble));
            Assert.True(resultAsDouble > minValue);
            Assert.True(resultAsDouble >= minValue && resultAsDouble <= maxValue);
        }

        [Fact]
        public void ShouldGetReplacementValueAsDecrementedNumberForAGivenRange()
        {
            double minValue = 1.1;
            double maxValue = 2.2;

            string token = $"{{{{Number:DecrementTracked:{maxValue}..{minValue}:0.1..0.1:100:unitTestTrackingKey}}}}";
            TokenDescriptor descriptor = new TokenDescriptor(token);
            ITokenHandler handler = TokenHandlerFactory.GetHandler(descriptor, _unitTestProviderName, null);

            // Run twice as the first time uses max value as the starting number
            // and we want to validate that decrementing is working properly.
            string result = handler.GetReplacementValue();
            result = handler.GetReplacementValue();

            Assert.True(Double.TryParse(result, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture.NumberFormat, out double resultAsDouble));
            Assert.True(resultAsDouble < maxValue);
            Assert.True(resultAsDouble >= minValue && resultAsDouble <= maxValue);
        }

        [Fact]
        public void ShouldFailWithInvalidParameterCountForNumberReference()
        {
            string exMessage = String.Empty;
            string invalidToken = "{{Number:Reference}}";
            string expectedExMessage = $"ValidateParameterCount :: Token provider 'NumberHandler' for provider '{_unitTestProviderName}' expected '3' token parameters, but got '2' for token '{invalidToken}'";
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
        public void ShouldGetTrackedReplacementValueAsAsNumber()
        {
            double minValue = 1.1;
            double maxValue = 2.2;

            string tokenTracked = $"{{{{Number:IncrementTracked:{minValue}..{maxValue}:0.1..0.1:100:unitTestTrackingKey}}}}";
            TokenDescriptor descriptorTracked = new TokenDescriptor(tokenTracked);
            ITokenHandler handlerTracked = TokenHandlerFactory.GetHandler(descriptorTracked, _unitTestProviderName, null);
            string resultTracked = handlerTracked.GetReplacementValue();

            string tokenReference = $"{{{{Number:Reference:unitTestTrackingKey}}}}";
            TokenDescriptor descriptorReference = new TokenDescriptor(tokenReference);
            ITokenHandler handlerReference = TokenHandlerFactory.GetHandler(descriptorReference, _unitTestProviderName, null);
            string resultReference = handlerReference.GetReplacementValue();

            Assert.Equal(resultTracked, resultReference);
        }
    }
}