/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Runtime.Serialization;

namespace DSynth
{
    [Serializable]
    public class DSynthServiceException : Exception
    {
        public DSynthServiceException()
        {
        }

        public DSynthServiceException(string message) : base(message)
        {
        }

        public DSynthServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DSynthServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}