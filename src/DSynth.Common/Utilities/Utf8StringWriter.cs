/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.IO;
using System.Text;

/// <summary>
/// The default encoding of StringWriter is UTF16 and the XML serializer usese this value.
/// There is no way to adjust this and need to extend the functionality for UTF8.
/// </summary>
namespace DSynth.Common.Utilities
{
    public sealed class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;

        public Utf8StringWriter(StringBuilder sb) : base(sb)
        {
        }
    }
}
