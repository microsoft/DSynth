/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Threading;
using Microsoft.Extensions.Logging;
using DSynth.Common.Options;
using DSynth.Engine;
using DSynth.Common.Models;

namespace DSynth.Provider.Providers
{
    public abstract class ProviderBase : IDSynthProvider
    {
        internal ProviderQueue ProviderQueue;
        internal readonly ILogger Logger;
        internal IDSynthEngine DSynthEngine;
        private readonly EngineType _engineType;
        public DSynthProviderOptions Options { get; internal set; }
        public abstract PayloadPackage Package { get; }

        protected ProviderBase(
            DSynthProviderOptions options,
            EngineType engineType,
            ILogger logger,
            CancellationToken token)
        {
            Options = options;
            _engineType = engineType;
            Logger = logger;
        }

        public void Initialize(CancellationToken token)
        {
            DSynthEngine = EngineFactory.GetDSynthEngine(
                _engineType,
                Options.TemplateName,
                Options.ProviderName,
                token,
                Logger);

            ProviderQueue = ProviderQueue.CreateNew(DSynthEngine, Options, Logger, token);
        }
    }
}