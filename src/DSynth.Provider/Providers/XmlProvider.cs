/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.IO;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;
using DSynth.Common.Options;
using DSynth.Engine;
using DSynth.Common.Utilities;
using System.Threading;
using DSynth.Common.Models;
using System;

namespace DSynth.Provider.Providers
{
    public class XmlProvider : ProviderBase
    {
        public override PayloadPackage Package => PreparePayloadPackage();

        public XmlProvider(DSynthProviderOptions options, ILogger logger, CancellationToken token)
            : base(options, EngineType.XML, logger, token)
        {
        }

        public PayloadPackage PreparePayloadPackage()
        {
            try
            {
                object payload = ProviderQueue.Dequeue();
                return PayloadPackage.CreateNew(XmlSerializeToBytes(payload), XmlSerializeToString(payload));
            }
            catch (Exception ex)
            {
                throw new ProviderException(ex.Message, ex);
            }
        }

        private static byte[] XmlSerializeToBytes(object objectInstance)
        {
            var serializer = new XmlSerializer(objectInstance.GetType());
            var sb = new StringBuilder();

            using (TextWriter writer = new Utf8StringWriter(sb))
            {
                serializer.Serialize(writer, objectInstance);
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        public static string XmlSerializeToString(object objectInstance)
        {
            var serializer = new XmlSerializer(objectInstance.GetType());
            var sb = new StringBuilder();

            using (TextWriter writer = new Utf8StringWriter(sb))
            {
                serializer.Serialize(writer, objectInstance);
            }

            return sb.ToString();
        }
    }
}