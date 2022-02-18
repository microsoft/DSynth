/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using DSynth.Engine.TokenHandlers;
using Xunit;

namespace DSynth.Engine.Tests.UnitTests
{
    public class GuidHandlerTests
    {
        private const string _unitTestProviderName = "UnitTestProviderName";

        [Fact]
        public void ShouldGetReplacementValue()
        {
            string token = "{{Guid:NewGuid}}";
            TokenDescriptor descriptor = new TokenDescriptor(token);
            ITokenHandler handler = TokenHandlerFactory.GetHandler(descriptor, _unitTestProviderName, null);
            string result = handler.GetReplacementValue();
            Assert.True(Guid.TryParse(result, out Guid resultAsGuid));
        }

        [Fact]
        public void ShouldFailWithInvalidParameterCountForTracked()
        {
            string exMessage = String.Empty;
            string invalidToken = "{{Guid:Tracked}}";
            string expectedExMessage = "ValidateParameterCount :: Token provider 'GuidHandler' for provider 'UnitTestProviderName' expected '3' token parameters, but got '2' for token '{{Guid:Tracked}}'";
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
        public void ShouldFailWithInvalidParameterCountForReference()
        {
            string exMessage = String.Empty;
            string invalidToken = "{{Guid:Reference}}";
            string expectedExMessage = "ValidateParameterCount :: Token provider 'GuidHandler' for provider 'UnitTestProviderName' expected '3' token parameters, but got '2' for token '{{Guid:Reference}}'";
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
        public void ShouldGetReplacementValueFromTrackedReference()
        {
            string tokenTracked = "{{Guid:Tracked:guidKey}}";
            TokenDescriptor descriptorTracked = new TokenDescriptor(tokenTracked);
            ITokenHandler handlerTracked = TokenHandlerFactory.GetHandler(descriptorTracked, _unitTestProviderName, null);
            string resultTracked = handlerTracked.GetReplacementValue();

            string tokenReference = "{{Guid:Reference:guidKey}}";
            TokenDescriptor descriptorReference = new TokenDescriptor(tokenReference);
            ITokenHandler handlerReference = TokenHandlerFactory.GetHandler(descriptorReference, _unitTestProviderName, null);
            string resultReference = handlerReference.GetReplacementValue();

            Assert.Equal(resultTracked, resultReference);
        }
    }
}