/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Threading;
using DSynth.Common.Utilities;

namespace DSynth.Engine.TokenHandlers
{
    public abstract class TokenHandlerBase : ITokenHandler, IDisposable
    {
        internal TokenDescriptor TokenDescriptor;
        internal EngineSourceType SourceType;
        internal TemplateData TemplateData;
        private int _threadId = Thread.CurrentThread.ManagedThreadId;
        private string _handlerTypeName;
        internal string ProviderName;
        private bool _disposed;

        protected TokenHandlerBase(TokenDescriptor tokenDescriptor, string providerName, TemplateData templateData)
        {
            TokenDescriptor = tokenDescriptor;
            SourceType = TokenDescriptor.TokenSourceType;
            ProviderName = providerName;
            TemplateData = templateData;
            _handlerTypeName = this.GetType().Name;
        }

        public abstract string GetReplacementValue();

        public static void ResetHandlers()
        {
            TimestampHandler.Reset();
            NumberHandler.Reset();
            CsvCollectionHandler.Reset();
            GuidHandler.Reset();
            JsonCollectionHandler.Reset();
        }

        internal string GetFormattedTrackedKey(string trackedKey, bool isThreadUnique = false, string suffix = "")
        {
            string ret;
            if (isThreadUnique)
            {
                ret = $"{ProviderName}-{trackedKey}-{_threadId}{suffix}";
            }
            else
            {
                ret = $"{ProviderName}-{trackedKey}{suffix}";
            }

            return ret;
        }

        internal void ValidateParameterCount(int expectedCount)
        {
            if (TokenDescriptor.TokenParameters.Length != expectedCount)
            {
                string formattedExMessage = ExceptionUtilities.GetFormattedMessage(
                    Resources.TokenHandlerBase.ExUnableToValidateParameterCount,
                    _handlerTypeName,
                    ProviderName,
                    expectedCount,
                    TokenDescriptor.TokenParameters.Length,
                    TokenDescriptor.Token);

                throw new TokenHandlerException(formattedExMessage);
            }
        }

        internal void ThrowParameterException(string invalidParameter = "")
        {
            string formattedExMessage = ExceptionUtilities.GetFormattedMessage(
                    Resources.TokenHandlerBase.ExUnableToParseParameters,
                    _handlerTypeName,
                    ProviderName,
                    TokenDescriptor.Token,
                    invalidParameter
                );

            throw new TokenHandlerException(formattedExMessage);
        }

        internal void ThrowEngineSourceTypeNotSupportedForHandler()
        {
            string formattedExMessage = ExceptionUtilities.GetFormattedMessage(
                    Resources.TokenHandlerBase.ExUnsupportedSourceTypeWithHandler,
                    SourceType.ToString(),
                    _handlerTypeName
                );

            throw new TokenHandlerException(formattedExMessage);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            ProviderName = null;
            TemplateData = null;
            _handlerTypeName = null;
            _disposed = true;
        }
    }
}