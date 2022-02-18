/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using DSynth.Engine.TokenHandlers;
using Xunit;

namespace DSynth.Engine.Tests.UnitTests
{
    public class MacAddressHandlerTests
    {
        private const string _unitTestProviderName = "UnitTestProviderName";

        [Fact]
        public void ShouldGetReplacementValue()
        {
            string token = "{{MacAddress:MacAddress}}";
            TokenDescriptor descriptor = new TokenDescriptor(token);
            ITokenHandler handler = TokenHandlerFactory.GetHandler(descriptor, _unitTestProviderName, null);
            string result = handler.GetReplacementValue();
            string[] macAddressSegments = result.Split(":");
            
            Assert.True(macAddressSegments.Length == 6);
        }
    }
}