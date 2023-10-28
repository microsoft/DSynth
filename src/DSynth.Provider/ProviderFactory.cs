/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using Microsoft.Extensions.Logging;
using DSynth.Common.Options;
using DSynth.Provider.Providers;
using System;
using System.Threading;
using DSynth.Common.Utilities;

namespace DSynth.Provider
{
    public class ProviderFactory
    {
        public static ProviderBase GetDSynthProvider(
            DSynthProviderOptions providerOptions,
            ILogger logger,
            CancellationToken token)
        {
            try
            {
                ProviderType type = EnumUtilities.GetEnumValueFromString<ProviderType>(providerOptions.Type);
                
                switch (type)
                {
                    case ProviderType.JSON:
                        return new JsonProvider(providerOptions, logger, token);

                    case ProviderType.XML:
                        return new XmlProvider(providerOptions, logger, token);

                    case ProviderType.CSV:
                        return new CsvProvider(providerOptions, logger, token);

                    case ProviderType.Image:
                        return new ImageProvider(providerOptions, logger, token);

                    case ProviderType.Raw:
                        return new RawProvider(providerOptions, logger, token);

                    case ProviderType.JSONL:
                        return new JsonlProvider(providerOptions, logger, token);

                    case ProviderType.Proto:
                        return new ProtoProvider(providerOptions, logger, token);

                    default:
                        return null;
                }
            }
            catch (Exception ex)
            {
                throw new ProviderException(ex.Message, ex);
            }
        }
    }
}