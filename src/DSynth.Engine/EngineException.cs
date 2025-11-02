/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;

namespace DSynth.Engine
{
    [Serializable]
    public class EngineException : Exception
    {
        public EngineException()
        {
        }

        public EngineException(string message) : base(message)
        {
        }

        public EngineException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}