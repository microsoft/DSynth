/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Text.Json;
using System.Threading;
using DSynth.Common.Models;
using Microsoft.Extensions.Logging;

namespace DSynth.Engine.Engines
{
    public class ProtoEngine : EngineBase
    {
        private static readonly JsonSerializerOptions jsonOptions;

        static ProtoEngine()
        {
            jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public ProtoEngine(string templateName, string callingProviderName, CancellationToken token, ILogger logger)
            : base(templateName, callingProviderName, token, logger)
        {
        }

        public override object BuildPayload()
        {
            string template = GetTemplateWithReplacedTokens();

            return JsonSerializer.Deserialize<ProtoPayload>(template, jsonOptions);
        }
    }
}