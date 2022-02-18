/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using DSynth.Engine.TokenHandlers;
using DSynth.Common.Utilities;
using System.Threading;
using System.Collections.Concurrent;

namespace DSynth.Engine.Engines
{
    public abstract class EngineBase : IDSynthEngine
    {
        public ConcurrentDictionary<string, TemplateData> TemplateDataDictionary { get; private set; }

        private readonly string _callingProviderName;
        internal readonly ILogger Logger;
        private readonly Regex _regex;
        private TemplateData _templateData;
        private static TemplateDataProvider _templateDataProvider;

        protected EngineBase(string templateName, string callingProviderName, CancellationToken token, ILogger logger = null)
        {
            _callingProviderName = callingProviderName;

            token.Register(() =>
            {
                TemplateDataProvider.Clear();
                TokenHandlerBase.ResetHandlers();
            });

            Logger = logger;
            _regex = new Regex(Resources.EngineBase.ReplacementTokenRegexPattern);
            Initialize(templateName);
        }

        public abstract object BuildPayload();

        private void Initialize(string templateName)
        {
            try
            {
                Logger?.LogInformation(
                    Resources.EngineBase.InfoInitializeEngines,
                    this.GetType().Name,
                    _callingProviderName);

                _templateDataProvider = TemplateDataProvider.Instance;
                
                _templateData = _templateDataProvider
                    .BuildTemplateSegments(_callingProviderName)
                    .GetTemplateData(_callingProviderName ,templateName);

                TemplateDataDictionary = new ConcurrentDictionary<string, TemplateData>();
                TemplateDataDictionary[_callingProviderName] = _templateData;
            }
            catch (TemplateDataException ex)
            {
                string formattedExMessage = ExceptionUtilities.GetFormattedMessage(
                    Resources.EngineBase.ExUnableToInitialize,
                    this.GetType().Name);

                Logger?.LogError(ex, ex.Message);
                throw new EngineException(formattedExMessage, ex);
            }
            catch (AggregateException ex)
            {
                Logger?.LogError(ex, ex.Message);
                throw new EngineException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                string formattedExMessage = ExceptionUtilities.GetFormattedMessage(
                    Resources.EngineBase.ExUnableToBuildPayload,
                    _callingProviderName,
                    ex.Message);

                throw new EngineException(formattedExMessage, ex);
            }
            finally
            {
                Logger?.LogInformation(Resources.EngineBase.InfoInitializeComplete, this.GetType().Name);
            }
        }

        internal string GetTemplateWithReplacedTokens()
        {
            try
            {
                return _templateData.RenderTemplate();
            }
            catch (Exception ex)
            {
                string formattedExMessage = ExceptionUtilities.GetFormattedMessage(
                    Resources.EngineBase.ExUnableToBuildPayload,
                    _callingProviderName,
                    ex.Message);

                throw new EngineException(formattedExMessage, ex);
            }
        }
    }
}