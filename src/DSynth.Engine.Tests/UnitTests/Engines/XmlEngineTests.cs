/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Threading;
using System.Xml;
using Xunit;

namespace DSynth.Engine.Tests.UnitTests
{
    public class XmlEngineTests
    {
        private const string _unitTestProviderName = "UnitTestProviderName";
        private const string _templateName = "XmlEngineTest.template.xml";

        [Fact]
        public void ShouldReturnNonEmptyXmlPayload()
        {
            IDSynthEngine engine = EngineFactory.GetDSynthEngine(EngineType.XML, _templateName, _unitTestProviderName, CancellationToken.None);
            XmlNode[] result = (XmlNode[])engine.BuildPayload();
            Assert.True(result.Length > 0);
        }

        [Fact]
        public void ShouldReturnExpectedXmlResultsFromTemplate()
        {
            IDSynthEngine engine = EngineFactory.GetDSynthEngine(EngineType.XML, _templateName, _unitTestProviderName, CancellationToken.None);
            XmlNode[] result = (XmlNode[])engine.BuildPayload();
            Assert.True(result[0].LocalName == "Property1");
            Assert.True(result[0].InnerText == "value1");
            Assert.True(result[1].LocalName == "Property2");
            Assert.True(result[1].InnerText == "value2");
        }
    }
}