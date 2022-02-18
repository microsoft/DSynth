/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Runtime.Serialization;

namespace DSynth.Engine.TokenHandlers
{
    [Serializable]
    internal class TokenDescriptorException : Exception
    {
        public TokenDescriptorException()
        {
        }

        public TokenDescriptorException(string message) : base(message)
        {
        }

        public TokenDescriptorException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TokenDescriptorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}