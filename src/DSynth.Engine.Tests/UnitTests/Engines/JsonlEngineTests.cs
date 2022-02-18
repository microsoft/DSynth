/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using Xunit;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace DSynth.Engine.Tests.UnitTests
{
    public class JsonlEngineTests
    {
        private const string _unitTestProviderName = "UnitTestProviderName";
        private const string _templateName = "JsonlEngineTest.template.json";

        [Fact]
        public void ShouldReturnNonEmptyJsonPayload()
        {
            IDSynthEngine engine = EngineFactory.GetDSynthEngine(EngineType.JSONL, _templateName, _unitTestProviderName, CancellationToken.None);
            JObject result = JObject.Parse((string)engine.BuildPayload());
            Assert.True(result.Count > 0);
        }

        [Fact]
        public void ShouldReturnExpectedResultFromTemplate()
        {
            string expectedValue = "{\"property1\": \"value1\",\"property2\": \"value2\"}";
            IDSynthEngine engine = EngineFactory.GetDSynthEngine(EngineType.JSONL, _templateName, _unitTestProviderName, CancellationToken.None);
            JObject result = JObject.Parse((string)engine.BuildPayload());
            JObject expectedResult = JObject.Parse(expectedValue);
            Assert.Equal(result["property1"], expectedResult["property1"]);
            Assert.Equal(result["property2"], expectedResult["property2"]);
        }
    }
}