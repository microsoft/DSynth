/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

namespace DSynth.Sink.Utilities
{
    public class BearerOptions
    {
        public string TenantId { get; set; }
        public string AppId { get; set; }
        public string AppSecret { get; set; }
        public string Scope { get; set; }
        public int ExpBufferSeconds { get; set; }
        public string AzureManagemenrDns { get; set; }
    }
}