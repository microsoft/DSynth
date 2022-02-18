/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using DSynth.Common.Utilities;
using DSynth.Engine.TokenHandlers;

namespace DSynth.Engine
{
    public class TokenHandlerFactory
    {
        public static ITokenHandler GetHandler(TokenDescriptor tokenDescriptor, string providerName, TemplateData templateData)
        {
            switch (tokenDescriptor.TokenHandlerType)
            {
                case TokenHandlerType.Number:
                    return new NumberHandler(tokenDescriptor, providerName);

                case TokenHandlerType.JsonCollection:
                    return new JsonCollectionHandler(tokenDescriptor, providerName, templateData);

                case TokenHandlerType.CsvCollection:
                    return new CsvCollectionHandler(tokenDescriptor, providerName, templateData);

                case TokenHandlerType.Guid:
                    return new GuidHandler(tokenDescriptor, providerName);

                case TokenHandlerType.Timestamp:
                    return new TimestampHandler(tokenDescriptor, providerName);

                case TokenHandlerType.DateTime:
                    return new DateTimeHandler(tokenDescriptor, providerName);

                case TokenHandlerType.Nested:
                    return new NestedHandler(tokenDescriptor, providerName);

                case TokenHandlerType.MacAddress:
                    return new MacAddressHandler(tokenDescriptor, providerName);

                default:
                    string formattedExMessage = ExceptionUtilities.GetFormattedMessage(
                    Resources.EngineBase.ExUnsupportedType,
                    tokenDescriptor.Token);

                    throw new EngineException(formattedExMessage);
            }
        }
    }
}