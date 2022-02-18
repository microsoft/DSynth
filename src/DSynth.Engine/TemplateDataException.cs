/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Runtime.Serialization;

namespace DSynth.Engine
{
    [Serializable]
    internal class TemplateDataException : Exception
    {
        public TemplateDataException()
        {
        }

        public TemplateDataException(string message) : base(message)
        {
        }

        public TemplateDataException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TemplateDataException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}