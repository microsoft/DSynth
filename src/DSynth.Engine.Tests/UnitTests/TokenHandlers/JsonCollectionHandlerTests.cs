/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Linq;
using DSynth.Engine.TokenHandlers;
using Newtonsoft.Json.Linq;
using Xunit;

namespace DSynth.Engine.Tests.UnitTests
{
    public class JsonCollectionHandlerTests
    {
        private const string _unitTestProviderName = "UnitTestProviderName";
        private const string _templateName = "UnitTestJson.template.json";
        private const string _collectionsName = "UnitTest";
        private const string _collectionName = "unitTestCollection";
        private readonly TemplateDataProvider _templateDataProvider = TemplateDataProvider.Instance.BuildTemplateSegments(_unitTestProviderName);

        [Fact]
        public void ShouldFailWithInvalidParameterCountForObject()
        {
            string exMessage = String.Empty;
            string invalidToken = "{{JsonCollection:Collection}}";
            string expectedExMessage = $"ValidateParameterCount :: Token provider 'JsonCollectionHandler' for provider '{_unitTestProviderName}' expected '4' token parameters, but got '2' for token '{invalidToken}'";
            TokenDescriptor descriptor = new TokenDescriptor(invalidToken);
            TemplateData templateData = _templateDataProvider.GetTemplateData(_unitTestProviderName, _templateName);

            try
            {
                ITokenHandler handler = TokenHandlerFactory.GetHandler(descriptor, _unitTestProviderName, templateData);
            }
            catch (TokenHandlerException ex)
            {
                exMessage = ex.Message;
            }

            Assert.Equal(expectedExMessage, exMessage);
        }

        [Fact]
        public void ShouldGetReplacementValueAsAnItemFromJsonCollection()
        {
            string token = $"{{{{JsonCollection:Collection:{_collectionsName}:{_collectionName}}}}}";
            TokenDescriptor descriptor = new TokenDescriptor(token);
            TemplateData templateData = _templateDataProvider.GetTemplateData(_unitTestProviderName, _templateName);
            ITokenHandler handler = TokenHandlerFactory.GetHandler(descriptor, _unitTestProviderName, templateData);
            string result = handler.GetReplacementValue();
            string collectionPath = $"{Resources.TemplateData.JsonCollectionsObjectName}.{_collectionName}";

            string[] collectionItems = ((JObject)templateData.Collection(_collectionsName + ".collections.json"))
                .SelectToken(collectionPath).ToObject<string[]>();

            Assert.Contains(result, collectionItems);
        }

        [Fact]
        public void ShouldFailWithInvalidParameterCountForObjectTracked()
        {
            string exMessage = String.Empty;
            string invalidToken = "{{JsonCollection:Tracked}}";
            string expectedExMessage = $"ValidateParameterCount :: Token provider 'JsonCollectionHandler' for provider '{_unitTestProviderName}' expected '5' token parameters, but got '2' for token '{invalidToken}'";
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
        public void ShouldFailWithInvalidParameterCountForObjectReference()
        {
            string exMessage = String.Empty;
            string invalidToken = "{{JsonCollection:Reference}}";
            string expectedExMessage = $"ValidateParameterCount :: Token provider 'JsonCollectionHandler' for provider '{_unitTestProviderName}' expected '3' token parameters, but got '2' for token '{invalidToken}'";
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
        public void ShouldGetTrackedReplacementValue()
        {
            string tokenTracked = $"{{{{JsonCollection:Tracked:{_collectionsName}:unitTestCollection:unitTestTrackingKey}}}}";
            TokenDescriptor descriptorTracked = new TokenDescriptor(tokenTracked);
            TemplateData templateData = _templateDataProvider.GetTemplateData(_unitTestProviderName, _templateName);
            ITokenHandler handlerTracked = TokenHandlerFactory.GetHandler(descriptorTracked, _unitTestProviderName, templateData);
            string resultTracked = handlerTracked.GetReplacementValue();

            string tokenReference = "{{JsonCollection:Reference:unitTestTrackingKey}}";
            TokenDescriptor descriptorReference = new TokenDescriptor(tokenReference);
            ITokenHandler handlerReference = TokenHandlerFactory.GetHandler(descriptorReference, _unitTestProviderName, templateData);
            string resultReference = handlerReference.GetReplacementValue();

            Assert.Contains(resultTracked, resultReference);
        }
    }
}