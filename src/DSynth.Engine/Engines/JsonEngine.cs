/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Threading;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace DSynth.Engine.Engines
{
    public class JsonEngine : EngineBase
    {
        public JsonEngine(string templateName, string callingProviderName, CancellationToken token, ILogger logger)
            : base(templateName, callingProviderName, token, logger)
        {
            
        }

        public override object BuildPayload()
        {
            string template = GetTemplateWithReplacedTokens();
            return JsonSerializer.Deserialize<object>(template);
        }
    }
}