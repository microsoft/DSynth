/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Runtime.Serialization;

namespace DSynth.Sink
{
    [Serializable]
    public class SinkException : Exception
    {
        public SinkException()
        {
        }

        public SinkException(string message) : base(message)
        {
        }

        public SinkException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SinkException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}