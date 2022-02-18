/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DSynth.Engine.Engines
{
    public class ImageEngine : EngineBase
    {
        public ImageEngine(string templateName, string callingProviderName, CancellationToken token, ILogger logger)
            : base(templateName, callingProviderName, token, logger)
        {
        }

        public override object BuildPayload()
        {
            string template = GetTemplateWithReplacedTokens();
            ImageStructure imageStructure = JsonConvert.DeserializeObject<ImageStructure>(template);
            imageStructure = ImageFromText(imageStructure);

            return imageStructure;
        }

        private static ImageStructure ImageFromText(ImageStructure imageStructure)
        {
            //first, create a dummy bitmap just to get a graphics object
            using (Font font = new Font(imageStructure.FontFamily, imageStructure.FontSize))
            {
                Color fgColor = Color.FromName(imageStructure.ForegroundColor);
                Color bgColor = Color.FromName(imageStructure.BackgroundColor);
                ImageFormat imageFormat = GetImageFormat(imageStructure.ImageFormat);


                SizeF textSize;
                using (System.Drawing.Image img = new Bitmap(1, 1))
                using (Graphics drawing = Graphics.FromImage(img))
                {
                    textSize = drawing.MeasureString(imageStructure.ImageText, font);
                }

                //create a new image of the right size
                using (System.Drawing.Image img = new Bitmap((int)textSize.Width, (int)textSize.Height))
                using (Graphics drawing = Graphics.FromImage(img))
                using (Brush textBrush = new SolidBrush(fgColor))
                {
                    //paint the background
                    drawing.Clear(bgColor);
                    drawing.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                    drawing.DrawString(imageStructure.ImageText, font, textBrush, 0, 0);
                    drawing.Save();

                    using (MemoryStream ms = new MemoryStream())
                    {
                        img.Save(ms, imageFormat);
                        imageStructure.ImageBytes = ms.ToArray();
                    }
                }
            }

            return imageStructure;
        }

        public static ImageFormat GetImageFormat(string extension)
        {
            ImageFormat format = null;
            PropertyInfo prop = typeof(ImageFormat).GetProperties()
                .Where(p => p.Name.Equals(extension, StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault();

            if (prop != null)
            {
                format = prop.GetValue(prop) as ImageFormat;
            }

            return format;
        }
    }
}