/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

namespace DSynth.Engine
{
    public enum EngineSourceType
    {
        Range,
        Collection,
        Tracked,
        Reference,
        NewGuid,
        IncrementTracked,
        DecrementTracked,
        DateTime,
        Timestamp,
        Json,
        MacAddress,
        TrackedLimit
    }
}