/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using DSynth.Engine.TokenHandlers;
using Xunit;

namespace DSynth.Engine.Tests.UnitTests
{
    public class CsvCollectionHandlerTests
    {
        private const string _unitTestProviderName = "UnitTestProviderName";
        private const string _templateName = "UnitTestJson.template.json";
        private const string _collectionsName = "UnitTest";
        private const string _replacementField = "Model";
        private readonly TemplateDataProvider _templateDataProvider = TemplateDataProvider.Instance.BuildTemplateSegments(_unitTestProviderName);

        [Fact]
        public void ShouldFailWithInvalidParameterCount()
        {
            string exMessage = String.Empty;
            string invalidToken = "{{CsvCollection:Collection}}";
            string expectedExMessage = $"ValidateParameterCount :: Token provider 'CsvCollectionHandler' for provider '{_unitTestProviderName}' expected '4' token parameters, but got '2' for token '{invalidToken}'";
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
        public void ShouldGetReplacementValueAsAnItemFromCollection()
        {
            string token = $"{{{{CsvCollection:Collection:{_collectionsName}:{_replacementField}}}}}";
            TokenDescriptor descriptor = new TokenDescriptor(token);
            TemplateData templateData = _templateDataProvider.GetTemplateData(_unitTestProviderName, _templateName);
            ITokenHandler handler = TokenHandlerFactory.GetHandler(descriptor, _unitTestProviderName, templateData);
            string result = handler.GetReplacementValue();
            var csvRowData = ((dynamic)templateData).Collection(_collectionsName + ".collections.csv");
            List<string> collectionItems = new List<string>();

            // Working with dynamic, we need to iterate over the ExpandoObject to extract the required values.
            foreach (IEnumerable<KeyValuePair<string, object>> row in csvRowData)
            {
                collectionItems.Add(row.Where(r => r.Key == _replacementField).Select(r => r.Value).SingleOrDefault() as string);
            }
            
            Assert.Contains(result, collectionItems);
        }

        [Fact]
        public void ShouldFailWithInvalidParameterCountForTracked()
        {
            string exMessage = String.Empty;
            string invalidToken = "{{CsvCollection:Tracked}}";
            string expectedExMessage = $"ValidateParameterCount :: Token provider 'CsvCollectionHandler' for provider '{_unitTestProviderName}' expected '5' token parameters, but got '2' for token '{invalidToken}'";
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
        public void ShouldFailWithInvalidParameterCountForTrackedLimit()
        {
            string exMessage = String.Empty;
            string invalidToken = "{{CsvCollection:TrackedLimit}}";
            string expectedExMessage = $"ValidateParameterCount :: Token provider 'CsvCollectionHandler' for provider '{_unitTestProviderName}' expected '6' token parameters, but got '2' for token '{invalidToken}'";
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
        public void ShouldFailWithInvalidParameterForTrackedLimit()
        {
            string exMessage = String.Empty;
            string invalidToken = "{{CsvCollection:TrackedLimit:101:Sample:Model:carKey}}";
            string expectedExMessage = $"ValidateParameters :: Token provider 'CsvCollectionHandler' for provider '{_unitTestProviderName}' was unable to parse parameters from token '{invalidToken}' with a given value of '101'";
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
            string invalidToken = "{{CsvCollection:Reference}}";
            string expectedExMessage = $"ValidateParameterCount :: Token provider 'CsvCollectionHandler' for provider '{_unitTestProviderName}' expected '4' token parameters, but got '2' for token '{invalidToken}'";
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
            string tokenTracked = $"{{{{CsvCollection:Tracked:{_collectionsName}:{_replacementField}:unitTestTrackingKey}}}}";
            TokenDescriptor descriptorTracked = new TokenDescriptor(tokenTracked);
            TemplateData templateData = _templateDataProvider.GetTemplateData(_unitTestProviderName, _templateName);
            ITokenHandler handlerTracked = TokenHandlerFactory.GetHandler(descriptorTracked, _unitTestProviderName, templateData);
            string resultTracked = handlerTracked.GetReplacementValue();

            string tokenReference = $"{{{{CsvCollection:Reference:{_replacementField}:unitTestTrackingKey}}}}";
            TokenDescriptor descriptorReference = new TokenDescriptor(tokenReference);
            ITokenHandler handlerReference = TokenHandlerFactory.GetHandler(descriptorReference, _unitTestProviderName, templateData);
            string resultReference = handlerReference.GetReplacementValue();

            Assert.Contains(resultTracked, resultReference);
        }

        [Fact]
        public void ShouldGetTrackedLimitReplacementValue()
        {
            string tokenTracked = $"{{{{CsvCollection:TrackedLimit:25:{_collectionsName}:{_replacementField}:unitTestTrackingKey}}}}";
            TokenDescriptor descriptorTracked = new TokenDescriptor(tokenTracked);
            TemplateData templateData = _templateDataProvider.GetTemplateData(_unitTestProviderName, _templateName);
            ITokenHandler handlerTracked = TokenHandlerFactory.GetHandler(descriptorTracked, _unitTestProviderName, templateData);
            string resultTracked = handlerTracked.GetReplacementValue();

            string tokenReference = $"{{{{CsvCollection:Reference:{_replacementField}:unitTestTrackingKey}}}}";
            TokenDescriptor descriptorReference = new TokenDescriptor(tokenReference);
            ITokenHandler handlerReference = TokenHandlerFactory.GetHandler(descriptorReference, _unitTestProviderName, templateData);
            string resultReference = handlerReference.GetReplacementValue();

            Assert.Contains(resultTracked, resultReference);
        }
    }
}