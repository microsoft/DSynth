/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Threading;
using Xunit;

namespace DSynth.Engine.Tests.UnitTests
{
    public class RawEngineTests
    {
        private const string _unitTestProviderName = "UnitTestProviderName";
        private const string _templateName = "RawEngineTest.template.raw";

        [Fact]
        public void ShouldReturnNonEmptyRawPayload()
        {
            IDSynthEngine engine = EngineFactory.GetDSynthEngine(EngineType.Raw, _templateName, _unitTestProviderName, CancellationToken.None);
            string result = (string)engine.BuildPayload();
            Assert.True(!String.IsNullOrEmpty(result));
        }

        [Fact]
        public void ShouldReturnExpectedRawResultFromTemplate()
        {
            string expectedValue = $"Raw template with raw text";
            IDSynthEngine engine = EngineFactory.GetDSynthEngine(EngineType.Raw, _templateName, _unitTestProviderName, CancellationToken.None);
            string result = (string)engine.BuildPayload();
            Assert.Equal(result, expectedValue);
        }
    }
}