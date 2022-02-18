/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using DSynth.Common.Utilities;

namespace DSynth.Engine.TokenHandlers
{
    public struct TokenDescriptor : IDisposable
    {
        public TokenHandlerType TokenHandlerType { get; set; }
        public EngineSourceType TokenSourceType { get; set; }
        public string Token { get; set; }
        public string[] TokenParameters { get; set; }
        private bool _disposed;

        public TokenDescriptor(string tokenParameterString)
            : this()
        {
            if (String.IsNullOrEmpty(tokenParameterString))
            {
                string formattedExMessage = ExceptionUtilities.GetFormattedMessage(
                    Resources.TokenDescriptor.ArgumentExceptionRawParameters,
                    nameof(tokenParameterString));

                throw new ArgumentException(formattedExMessage, nameof(tokenParameterString));
            }

            Token = tokenParameterString;
            TokenParameters = ExtractParameters(tokenParameterString);
            TokenHandlerType = ExtractTokenDataType(TokenParameters[0]);
            TokenSourceType = ExtractTokenSourceType(TokenParameters[1]);
        }

        private string[] ExtractParameters(string parameterString)
        {
            // Trim off {{ }} from parameterString and split on delimeter
            string trimmedParameters = parameterString.Substring(2, parameterString.Length - 4);
            string[] parameters = trimmedParameters.Split(Resources.TokenDescriptor.ReplacementTokenDelimeter);

            if (parameters.Length < Resources.TokenDescriptor.ExpectedParameterCount)
            {
                string formattedExMessage = ExceptionUtilities.GetFormattedMessage(
                    Resources.TokenDescriptor.ExUnexpectedNumParameters,
                    TokenParameters?.Length ?? -1,
                    Resources.TokenDescriptor.ExpectedParameterCount,
                    parameterString);

                throw new TokenDescriptorException(formattedExMessage);
            }

            return parameters;
        }

        private TokenHandlerType ExtractTokenDataType(string stringType)
        {
            return EnumUtilities.GetEnumValueFromString<TokenHandlerType>(stringType);
        }

        private EngineSourceType ExtractTokenSourceType(string stringType)
        {
            return EnumUtilities.GetEnumValueFromString<EngineSourceType>(stringType);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            Token = null;
            TokenParameters = null;
            _disposed = true;
        }
    }
}