/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using Xunit;
using System.Threading;
using System.Text.Json;

namespace DSynth.Engine.Tests.UnitTests
{
    public class JsonEngineTests
    {
        private const string _unitTestProviderName = "UnitTestProviderName";
        private const string _templateName = "JsonEngineTest.template.json";

        [Fact]
        public void ShouldReturnNonEmptyJsonPayload()
        {
            IDSynthEngine engine = EngineFactory.GetDSynthEngine(EngineType.JSON, _templateName, _unitTestProviderName, CancellationToken.None);
            JsonElement result = (JsonElement)engine.BuildPayload();

            bool resultEnumeratorHasValues = false;
            try
            {
                result.EnumerateObject();
                resultEnumeratorHasValues = true;
            }
            catch (Exception)
            {
                // Do nothing. If we are unable to enumerate,
                // resultEnumeratorHasValues is false and test
                // will fail.
            }

            Assert.True(resultEnumeratorHasValues);
        }

        [Fact]
        public void ShouldReturnExpectedResultWithNestedJsonFromTemplate()
        {
            string expectedValue = "{\"property1\": \"value1\",\"property2\": \"value2\",\"nestedJson\": [{\"property1\": \"value1\",\"property2\": \"value2\"},{\"property1\": \"value1\",\"property2\": \"value2\"}]}";
            IDSynthEngine engine = EngineFactory.GetDSynthEngine(EngineType.JSON, _templateName, _unitTestProviderName, CancellationToken.None);
            JsonElement result = (JsonElement)engine.BuildPayload();
            JsonElement expectedResult = (JsonElement)(JsonSerializer.Deserialize<object>(expectedValue));

            Assert.Equal(result.GetProperty("property1").GetString(), expectedResult.GetProperty("property1").GetString());
            Assert.Equal(result.GetProperty("property2").GetString(), expectedResult.GetProperty("property2").GetString());
            Assert.Equal(result.GetProperty("nestedJson")[0].GetProperty("property1").GetString(), expectedResult.GetProperty("nestedJson")[0].GetProperty("property1").GetString());
        }
    }
}