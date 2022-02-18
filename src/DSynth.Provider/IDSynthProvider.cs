/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Threading;
using DSynth.Common.Models;

namespace DSynth.Provider
{
    public interface IDSynthProvider
    {
        /// <summary>
        /// The payload package
        /// </summary>
        /// <value>PayloadPackage</value>
        PayloadPackage Package { get; }
        
        /// <summary>
        /// Initializes provider
        /// </summary>
        /// <param name="token"></param>
        void Initialize(CancellationToken token);
    }
}