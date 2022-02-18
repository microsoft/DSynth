/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Threading;
using Xunit;

namespace DSynth.Engine.Tests.UnitTests
{
    public class CsvEngineTests
    {
        private const string _unitTestProviderName = "UnitTestProviderName";
        private const string _templateName = "CsvEngineTest.template.csv";

        [Fact]
        public void ShouldReturnNonEmptyCsvPayload()
        {
            IDSynthEngine engine = EngineFactory.GetDSynthEngine(EngineType.CSV, _templateName, _unitTestProviderName, CancellationToken.None);
            string result = (String)engine.BuildPayload();
            Assert.True(!String.IsNullOrEmpty(result));
        }

        [Fact]
        public void ShouldReturnExpectedCsvTemplateValues()
        {
            string expectedValues = $"value1,value2";
            IDSynthEngine engine = EngineFactory.GetDSynthEngine(EngineType.CSV, _templateName, _unitTestProviderName, CancellationToken.None);
            string result = (string)engine.BuildPayload();
            Assert.Equal(result, expectedValues);
        }

        [Fact]
        public void ShouldHaveExpectedCsvTemplateHeaders()
        {
            string expectedValues = "column1,column2";
            IDSynthEngine engine = EngineFactory.GetDSynthEngine(EngineType.CSV, _templateName, _unitTestProviderName, CancellationToken.None);
            engine.TemplateDataDictionary[_unitTestProviderName].Metadata.TryGetValue("Header", out string header);
            Assert.Equal(header, expectedValues);
        }
    }
}