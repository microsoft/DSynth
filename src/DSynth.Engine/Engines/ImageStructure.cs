/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

namespace DSynth.Engine.Engines
{
    public class ImageStructure
    {
        public string ImageText { get; set; }
        public float FontSize { get; set; }
        public string FontFamily { get; set; }
        public string ForegroundColor { get; set; }
        public string BackgroundColor { get; set; }
        public string ImageFormat { get; set; }
        public byte[] ImageBytes { get; set; }
    }
}