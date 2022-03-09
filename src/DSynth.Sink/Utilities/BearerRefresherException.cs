/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Runtime.Serialization;

namespace DSynth.Sink.Utilities
{
    [Serializable]
    public class BearerRefresherException : Exception
    {
        public BearerRefresherException()
        {
        }

        public BearerRefresherException(string message) : base(message)
        {
        }

        public BearerRefresherException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BearerRefresherException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}