/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;

namespace DSynth.Engine.TokenHandlers
{
    [Serializable]
    public class TokenHandlerException : Exception
    {
        public TokenHandlerException()
        {
        }

        public TokenHandlerException(string message) : base(message)
        {
        }

        public TokenHandlerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}