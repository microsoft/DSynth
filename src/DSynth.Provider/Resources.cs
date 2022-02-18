/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

namespace DSynth.Provider
{
    public static class Resources
    {
        public static class ProviderQueue
        {
            // Exceptions
            public const string ExUnableToGetNextPayload = "Dequeue :: Unable to dequeue the next payload for provider '{0}', see inner exception for details...";
            public const string ExUnableToCreateQueue = "ProviderQueueCtor :: Unable to create queue for provider '{0}'";
            public const string ExUnableToPopulateCollection = "PopulateCollectionAsync :: Engine type '{0}' is unable to build payload for provider '{1}'";

            // Log messages
            public const string WarnQueueIsEmpty = "Dequeue :: Provider queue for provider '{ProviderName}' is being starved. Try increasing 'intervalInMs'.";
        }
    }
}