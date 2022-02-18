/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Collections.Concurrent;

namespace DSynth.Engine
{
    public interface IDSynthEngine
    {
        /// <exception cref="DSynth.Engine.EngineException"></exception>
        object BuildPayload();

        /// <summary>
        /// Provides access to the template data dictionary
        /// to read metadata about a given template.
        /// </summary>
        ConcurrentDictionary<string, TemplateData> TemplateDataDictionary { get; }
    }
}