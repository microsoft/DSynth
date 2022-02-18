/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Threading.Tasks;
using DSynth.Common.Models;

namespace DSynth.Sink
{
    public interface IDSynthSink
    {
        /// <summary>
        /// Sends a PayloadPackage to the configured provider's sink
        /// </summary>
        /// <param name="package"></param>
        Task SendPayloadAsync(PayloadPackage package);
    }
}