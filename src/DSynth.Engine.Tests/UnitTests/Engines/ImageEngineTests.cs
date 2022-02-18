/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Threading;
using DSynth.Engine.Engines;
using Xunit;

namespace DSynth.Engine.Tests.UnitTests
{
    public class ImageEngineTests
    {
        private const string _unitTestProviderName = "UnitTestProviderName";
        private const string _templateName = "ImageEngineTest.template.image";

        [Fact]
        public void ShouldReturnValidImagePayload()
        {
            IDSynthEngine engine = EngineFactory.GetDSynthEngine(EngineType.Image, _templateName, _unitTestProviderName, CancellationToken.None);
            ImageStructure result = (ImageStructure)engine.BuildPayload();

            Assert.True(result.BackgroundColor == "Black");
            Assert.True(result.FontFamily == "GenericSerif");
            Assert.True(result.FontSize == 100);
            Assert.True(result.ForegroundColor == "Blue");
            Assert.True(result.ImageBytes.Length > 4);
            Assert.True(result.ImageFormat == "jpeg");
            Assert.True(result.ImageText == "test text");
        }
    }
}