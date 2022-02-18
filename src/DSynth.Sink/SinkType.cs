/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

namespace DSynth.Sink
{
    public enum SinkType
    {
        Console,
        HTTP,
        File,
        AzureBlob,
        SocketServer,
        AzureEventHub,
        AzureServiceBus,
        AzureCosmosDb,
        AzureLogAnalytics
    }
}