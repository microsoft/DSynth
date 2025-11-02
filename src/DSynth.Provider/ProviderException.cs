/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;

namespace DSynth.Provider
{
    [Serializable]
    public class ProviderException : Exception
    {
        public ProviderException()
        {
        }

        public ProviderException(string message) : base(message)
        {
        }

        public ProviderException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}