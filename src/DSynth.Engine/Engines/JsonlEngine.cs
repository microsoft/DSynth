/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Threading;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace DSynth.Engine.Engines
{
    public class JsonlEngine : EngineBase
    {
        private bool _hasValidated = false;

        public JsonlEngine(string templateName, string callingProviderName, CancellationToken token, ILogger logger)
            : base(templateName, callingProviderName, token, logger)
        {
        }

        public override object BuildPayload()
        {
            string ret = string.Empty;
            if (!_hasValidated)
            {
                _hasValidated = true;
                string template = GetTemplateWithReplacedTokens();
                ret = JsonSerializer.Serialize(JsonSerializer.Deserialize<object>(template));                
            }
            else
            {
                ret = GetTemplateWithReplacedTokens();
            }


            return ret;
        }
    }
}