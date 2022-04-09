/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using Microsoft.Extensions.Logging;
using DSynth.Common.Options;
using DSynth.Engine.Engines;
using DSynth.Engine;
using System.Threading;
using DSynth.Common.Models;
using System;

namespace DSynth.Provider.Providers
{
    public class ImageProvider : ProviderBase
    {
        private ImageStructure _imageStructure;
        public override PayloadPackage Package => PreparePayloadPackage();

        public ImageProvider(DSynthProviderOptions options, ILogger logger, CancellationToken token)
            : base(options, EngineType.Image, logger, token)
        {
        }

        public PayloadPackage PreparePayloadPackage()
        {
            try
            {
                _imageStructure = (ImageStructure)ProviderQueue.Dequeue(out long payloadCount);

                return PayloadPackage.CreateNew(_imageStructure.ImageBytes, payloadCount, null, null, overrides =>
                {
                    overrides.Add("FileNameSuffix", $".{_imageStructure.ImageFormat}");
                });
            }
            catch (Exception ex)
            {
                throw new ProviderException(ex.Message, ex);
            }
        }
    }
}