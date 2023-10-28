/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Text.Json;

namespace DSynth.Common.Models
{
    public class ProtoOptions
    {
        public string Namespace { get; set; } = "DSynth.Provider.Providers";
        public string ClassName { get; set; }

        public string FullClassName => $"{Namespace}.{ClassName}";
    }

    public class ProtoPayload
    {
        public ProtoOptions ProtoOptions { get; set; }
        public dynamic Data { get; set; }

        public string DataAsString => JsonSerializer.Serialize(Data);
    }
}