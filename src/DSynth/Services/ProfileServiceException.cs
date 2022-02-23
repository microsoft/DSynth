/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Runtime.Serialization;

namespace DSynth.Services
{
    [Serializable]
    public class ProfileServiceException : Exception
    {
        public ProfileServiceException()
        {
        }

        public ProfileServiceException(string message) : base(message)
        {
        }

        public ProfileServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ProfileServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}