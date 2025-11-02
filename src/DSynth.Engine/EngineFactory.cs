/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using Microsoft.Extensions.Logging;
using DSynth.Engine.Engines;
using DSynth.Common.Utilities;
using System.Threading;

namespace DSynth.Engine
{
    public class EngineFactory
    {
        public static IDSynthEngine GetDSynthEngine(EngineType type, string templateName, string callingProviderName, CancellationToken token, ILogger logger = null)
        {
            EngineBase ret = null;
            switch (type)
            {
                case EngineType.JSON:
                    ret = new JsonEngine(templateName, callingProviderName, token, logger);
                    break;

                case EngineType.XML:
                    ret = new XmlEngine(templateName, callingProviderName, token, logger);
                    break;

                case EngineType.CSV:
                    ret = new CsvEngine(templateName, callingProviderName, token, logger);
                    break;

                case EngineType.Raw:
                    ret = new RawEngine(templateName, callingProviderName, token, logger);
                    break;

                case EngineType.JSONL:
                    ret = new JsonlEngine(templateName, callingProviderName, token, logger);
                    break;

                default:
                    string formattedExMessage = ExceptionUtilities.GetFormattedMessage(
                        Resources.EngineFactory.ExUnableToBuildForType,
                        type.ToString()
                    );

                    throw new EngineException(formattedExMessage);
            }

            return ret;
        }
    }
}