/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.IO;
using System.Threading;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;

namespace DSynth.Engine.Engines
{
    public class XmlEngine : EngineBase
    {
        private readonly XmlSerializer _xmlSerializer;

        public XmlEngine(string templateName, string callingProviderName, CancellationToken token, ILogger logger)
            : base(templateName, callingProviderName, token, logger)
        {
            _xmlSerializer = new XmlSerializer(typeof(object), new XmlRootAttribute("Template"));
        }

        public override object BuildPayload()
        {
            string template = GetTemplateWithReplacedTokens();
            return XmlDeserializeFromString(template);
        }

        public object XmlDeserializeFromString(string template)
        {
            object serializedTemplate;

            using (TextReader reader = new StringReader(template))
            {
                serializedTemplate = _xmlSerializer.Deserialize(reader);
            }

            return serializedTemplate;
        }
    }
}