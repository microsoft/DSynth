/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using DSynth.Engine.TokenHandlers;
using Newtonsoft.Json.Linq;
using Xunit;

namespace DSynth.Engine.Tests.UnitTests
{
    public class NestedHandlerTests
    {
        private const string _unitTestProviderName = "UnitTestProviderName";
        private readonly TemplateDataProvider _templateDataProvider = TemplateDataProvider.Instance.BuildTemplateSegments(_unitTestProviderName);

        [Fact]
        public void ShouldFailWithInvalidParameterCountForNestedJson()
        {
            string exMessage = String.Empty;
            string invalidToken = "{{Nested:Json}}";
            string expectedExMessage = $"ValidateParameterCount :: Token provider 'NestedHandler' for provider '{_unitTestProviderName}' expected '4' token parameters, but got '2' for token '{invalidToken}'";
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
        public void ShouldGetReplacementValueForNestedToken()
        {
            string token = "{{Nested:Json:NestedJsonChild.template.json:1..2}}";
            TokenDescriptor descriptor = new TokenDescriptor(token);
            ITokenHandler handler = TokenHandlerFactory.GetHandler(descriptor, _unitTestProviderName, null);
            
            bool isValidJson = false;
            
            try
            {
                JObject result = JObject.Parse(handler.GetReplacementValue());
                isValidJson = true;
            }
            catch (Exception)
            {
                throw;
            }

            Assert.True(isValidJson);
        }
    }
}