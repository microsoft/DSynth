/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Threading;
using Microsoft.Extensions.Logging;

namespace DSynth.Engine.Engines
{
    public class CsvEngine : EngineBase
    {
        public CsvEngine(string templateName, string callingProviderName, CancellationToken token, ILogger logger = null)
            : base(templateName, callingProviderName, token, logger)
        {
        }

        public override object BuildPayload()
        {
            return GetTemplateWithReplacedTokens();
        }
    }
}