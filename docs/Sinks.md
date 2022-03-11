# Sink Configurations
The following documentation will outline how to configure various sinks. DSynth will take any generated payloads and send those payloads to supported sinks. If desired, multiple sinks can be configured for given provider[s]

---
## Quick Links
[Azure Blob](#azureblob)</br>
[Azure Cosmos DB](#azurecosmosdb)</br>
[Azure Custom Logs](#azurecustomlogs)</br>
[Azure Event Hub](#azureeventhub)</br>
[Azure Log Analytics](#azureloganalytics)</br>
[Azure Service Bus](#azureservicebus)</br>
[File](#file)</br>
[Console](#console)</br>
[Http](#http)</br>
[Socket Server](#socketserver)</br>
---

## <a id="azureblob"></a>Azure Blob
Provides functionality to save payloads to Azure Blob Store. Accepts 2 types of authentication, connection string or user assigned managed identity (UAMI). If connection string is specified with UAMI, connection string will be used. You only need to specify either `connectionString` OR `storageEndpoint AND managedIdentityClientId`

### Structure
```json
"sinks": [
  {
    "type": "azureblob",
    "connectionString": "{CONNECTION_STRING}",
    "storageEndpoint": "https://{STORAGE_ACCOUNT_NAME}.blob.core.windows.net",
    "managedIdentityClientId": "{UAMI_CLIENT_ID}",
    "blobContainerName": "mycontainer",
    "subfolderPattern": "{year}/{month}/{day}/{hour}/{minute}/{second}/{millisecond}",
    "filenamePattern": "json-{year}_{month}_{day}_{hour}_{minute}_{second}_{millisecond}",
    "fileNameSuffix": ".json"
  }
]
```

### Parameters
|Parameter Name|Available Values|Description|
|--|--|--|
|type|AzureBlob|Specifies the sink type of AzureBlob|
|connectionString|String value|Connection string for the storage account where payloads get written to. Supports retrieving secret from environment variable using `env:MY_ENV_VAR_NAME` as the connectionString value.|
|storageEndpoint|https://{STORAGE_ACCOUNT_NAME}.blob.core.windows.net|Use when you want to use UAMI to authenticate and to be used with managedIdentityClientId|
|managedIdentityClientId|Client Id of the UAMI|Use when you want to use UAMI to authenticate and to be used with storageEndpoint|
|blobContainerName|Non-Spaced string value|The blob container where the payload gets written to|
|subfolderPattern (optional)|Non-Spaced string value with or without tokens|The following tokens are available: {year}, {month}, {day}, {hour}, {minute}, {second}, {millisecond}|
|filenamePattern|Non-Spaced string value with or without tokens|The following tokens are available: {year}, {month}, {day}, {hour}, {minute}, {second}, {millisecond}|
|fileNameSuffix|Non-Spaced string to put at the end of the file name, i.e. .json|Suffixes the name of each file (file extension)| 

---

## <a id="azurecosmosdb"></a>Azure Cosmos DB
Provides functionality to save payloads to Azure Cosmos DB

### Structure
```json
"sinks": [
  {
    "type": "AzureCosmosDb",
    "endpoint": "https://mycosmosendpoint.documents.azure.com:443/",
    "authorizationKey": "{AUTHORIZATION_KEY}",
    "database": "mydatabasename",
    "collection": "mycollectionname",
    "partitionKey": "/id",
    "enableUpsert": true,
    "disableAutoIdGeneration": false,
    "batchSize": 2000,
    "maxInMemorySortingBatchSize": 1000,
    "offerThroughput": 400
  }
]
```
### Documentation
[Use the bulk executor .NET library to perform bulk operations in Azure Cosmos DB](https://docs.microsoft.com/en-us/azure/cosmos-db/bulk-executor-dot-net)

### Parameters
|Parameter Name|Available Values|Description|
|--|--|--|
|type|AzureCosmosDb|Specifies the sink type of AzureCosmosDb|
|endpoint|String value|The endpoint to your Cosmos DB instance|
|authorizationKey|String value|Authorization key required to access your Cosmos DB instance. Supports retrieving secret from environment variable using `env:MY_ENV_VAR_NAME` as the connectionString value.|
|database|Non-Spaced string value|The database that will be used|
|collection|Non-Spaced string|The collection that will get written to (collection gets created if it does not exist)|
|partitionKey|Non-Spaced string|The partition key of the collection|
|batchSize|Int value|This is the internal to DSynth and specifies the amount of records that will get queued before sending the bulk executor|
|maxInMemorySortingBatchSize|Int? value|Specific to Cosmos bulk executor and specifies how many documents it sends at a time to Cosmos. **Default 1000000**|
|offerThroughput|Int value|The throughput setting of the collection getting written to|
|enableUpsert|Bool value|If enabled, any documents with duplicate Ids will get upserted|
|disableAutoIdGeneration|Bool value|If disabled, the Id that is specified in templates will be honored, else Cosmos generates a Guid for you|

---

## <a id="azurecustomlogs"></a>Azure Custom Logs
Provides functionality to write payloads to Azure Custom Logs via Data Connection Endpoint (DCE) and Data Connection Rules (DCR). **Please note that this is still in preview - please be sure to read over the documentation first.**

### Structure
```json
"sinks": [
  {
    "type": "AzureCustomLogs",
    "tenantId": "00000000-0000-0000-0000-000000000000",
    "appId": "00000000-0000-0000-0000-000000000000",
    "appSecret": "MyAppSecret",
    "dcrImmutableId": "dcr-00000000000000000000000000000000",
    "dataCollectionEndpoint": "https://DCE.ingest.monitor.azure.com",
    "customTableName": "MyCustomTableName_CL",
    "enableCompression": true,
    "bearerScope": "https://monitor.azure.com//.default",
    "bearerExpBufferSeconds": 60,
    "requestTimeoutMs": 60000,
    "azureManagementDns": "login.microsoftonline.com",
    "apiVersion": "2021-11-01-preview"
  }
]
```
### Documentation
[Sending Custom Data (Preview)](https://docs.microsoft.com/en-us/azure/azure-monitor/logs/tutorial-custom-logs#overview-of-tutorial)

[Structure of a data collection rule in Azure Monitor (preview)](https://docs.microsoft.com/en-us/azure/azure-monitor/essentials/data-collection-rule-structure#custom-logs)

### Parameters
|Parameter Name|Available Values|Description|
|--|--|--|
|type|AzureCustomLogs|Specifies the sink type of AzureCustomLogs|
|tenantId|Guid value as string|The tenant id of your application registration|
|appId|Guid value as string|The app id that has permissions of [Monitoring Contributor](https://docs.microsoft.com/en-us/azure/azure-monitor/essentials/data-collection-rule-overview#permissions)|
|appSecret|String value|App secret for the appId. Supports retrieving secret from environment variable using `env:MY_ENV_VAR_NAME` as the appSecret value.|
|dcrImmutableId|Non-Spaced string value|The id of the data collection rule|
|dataCollectionEndpoint|Non-Spaced string value|The data collection endpoint where logs will be sent to|
|customTableName|Non-Spaced string value|The name of the custom table to receive the custom logs, including the '_CL' suffix|
|enableCompression (Optional)|bool value|Enables compressing the payload with GZip, **Default false**|
|bearerScope (Optional)|Non-Spaced string value|The scope of the bearer token, **Default https://monitor.azure.com//.default**|
|bearerExpBufferSeconds (Optional)|Int value|Allows getting a bearer token sooner than the expiration time to help avoid race conditions, **Default 60**|
|requestTimeoutMs (Optional)|Int value|How long before the HttpClient waits before timing out. **Default 60000**|
|azureManagementDns (Optional)|Non-Spaced string value|Specifies the DNS of the endpoint where the bearer token will be retrieved, **Default login.microsoftonline.com**|
|apiVersion (Optional)|Non-Spaced string value|The API version of the data collection endpoint to use, **Default 2021-11-01-preview**|

---

## <a id="azureeventhub"></a>Azure Event Hub
Provides functionality to write to Azure Event Hubs

### Structure
```json
"sinks": [
  {
    "type": "AzureEventHub",
    "connectionString": "{CONECTIONSTRING}",
    "eventBatchSizeInBytes": 131072
  }
]
```
### Documentation
[Microsoft.Azure.EventHubs Namespace](https://docs.microsoft.com/en-us/dotnet/api/microsoft.azure.eventhubs?view=azure-dotnet)

### Parameters
|Parameter Name|Available Values|Description|
|--|--|--|
|type|AzureEventHub|Specifies the sink type of AzureEventHub|
|connectionString|String value|Connection string for the desired Event Hub where payloads get written to. Supports retrieving secret from environment variable using `env:MY_ENV_VAR_NAME` as the connectionString value.|
|eventBatchSizeInBytes|long value|How large the batch size is that gets sent to Event Hubs|

---

## <a id="azureloganalytics"></a>Azure Log Analytics
Provides functionality to write to Azure Log Analytics workspace custom logs via the data collector API.

### Documentation
[Send log data to Azure Monitor by using the HTTP Data Collector API (preview)](https://docs.microsoft.com/en-us/azure/azure-monitor/logs/data-collector-api)
### Structure
```json
"sinks": [
  {
    "type": "azureloganalytics",
    "workspaceId": "00000000-0000-0000-0000-000000000000",
    "sharedKey": "{SHARED_KEY}",
    "logType": "MyCustomTableName",
    "timestampField": "timestampFieldName",
    "requestTimeoutMs": 60000,
    "dnsSuffix": "ods.opinsights.azure.com",
    "apiVersion": "2016-04-01"
  }
]
```

### Parameters
|Parameter Name|Available Values|Description|
|--|--|--|
|type|AzureLogAnalytics|Specifies the sink type of AzureLogAnalytics|
|workspaceId|String value|The Id of the workspace|
|sharedKey|String value|The workspace shared key. Supports retrieving secret from environment variable using `env:MY_ENV_VAR_NAME` as the sharedKey value.|
|logType|String value|The name of the custom log table|
|timestampField (optional)|String value|The timestamp field to use|
|requestTimeoutMs (optional)|Int value|How long before the HttpClient waits before timing out. **Default 60000**|
|dnsSuffix (optional)|String value|The DNS suffix of the workspace URI. **Default ods.opinsights.azure.com**|
|apiVersion (optional)|String value|The version of the data collector API. **Default 2016-04-01**|

---

## <a id="azureservicebus"></a>Azure Service Bus
Provides functionality to write to Azure Service Bus

### Documentation
[CreateMessageBatchOptions Class](https://docs.microsoft.com/en-us/dotnet/api/azure.messaging.servicebus.createmessagebatchoptions?view=azure-dotnet)

### Structure
```json
"sinks": [
  {
    "type": "AzureEventHub",
    "connectionString": "{CONECTIONSTRING}",
    "maxSizeInBytes": "131072",
    "topicOrQueueName": "mytopic"
  }
]
```

### Parameters
|Parameter Name|Available Values|Description|
|--|--|--|
|type|AzureEventHub|Specifies the sink type of AzureEventHub|
|connectionString|String value|Connection string for the desired Event Hub where payloads get written to. Supports retrieving secret from environment variable using `env:MY_ENV_VAR_NAME` as the connectionString value.|
|maxSizeInBytes|Long? value|Provides max byte size for batching messages. If null, the maximum size allowed by the transport is used.|
|topicOrQueueName|String value|Topic or queue name where messages get sent|

---

## <a id="file"></a>File
Provides functionality to save payloads to the local file system

### Structure
```json
"sinks": [
  {
    "type": "file",
    "baseFolderPath": "/path/to/payload-output",
    "subfolderPattern": "{year}/{month}/{day}/{hour}/{minute}/{second}/{millisecond}",
    "filenamePattern": "json-{year}_{month}_{day}_{hour}_{minute}_{second}_{millisecond}",
    "fileNameSuffix": ".json",
    "fileMode": "Create"
  }
]
```

### Parameters
|Parameter Name|Available Values|Description|
|--|--|--|
|type|File|Specifies the sink type of File|
|baseFolderPath|String value|The base path to the folder where the payload get written to|
|subfolderPattern (optional)|Non-Spaced string value with or without tokens|The following tokens are available: {year}, {month}, {day}, {hour}, {minute}, {second}, {millisecond}|
|filenamePattern|Non-Spaced string value with or without tokens|The following tokens are available: {year}, {month}, {day}, {hour}, {minute}, {second}, {millisecond}|
|fileNameSuffix|Non-Spaced string to put at the end of the file name, i.e. .json|Suffixes the name of each file (file extension)|
|fileMode|Append, Create, CreateNew, Open, OpenOrCreate, Truncate|**Default Create**; Please see [FileMode Enum](https://docs.microsoft.com/en-us/dotnet/api/system.io.filemode?view=net-5.0) for details|

---

## <a id="console"></a>Console
Provides functionality to write payloads to the console and/or logger. If an Application Insights connection string is specified in `appsettings.json` and writeToLog is true, payload output will also be sent to Application Insights.

### Structure
```json
"sinks": [
  {
    "type": "Console",
    "writeToConsole": true,
    "writeToLog": false
  }
]
```

### Parameters
|Parameter Name|Available Values|Description|
|--|--|--|
|type|Console|Specifies the sink type of Console|
|writeToConsole|true \| false|**Default true** to output to console as Console.Writeline|
|writeToLog|true \| false|**Default false**, but if set to true, this will log out via ILogger as Logger.LogInformation|

---

## <a id="http"></a>Http
Provides functionality to send payloads to an Http endpoint

### Structure
```json
"sinks": [
  {
    "type": "http",
    "endPointScheme": "http",
    "endpointDns": "my.azure.endpoint.net",
    "endpointPath": "/api/v1/payloads",
    "endpointPort": 8080,
    "mimeType": "application/json"
  }
]
```

### Parameters
|Parameter Name|Available Values|Description|
|--|--|--|
|type|Http|Specifies the sink type of Http|
|endPointScheme|http \| https|Used to form the scheme portion of the URI|
|endpointDns|Non-Spaced string DNS format, i.e. my.azure.endpoint.net|Used to form the host portion of the URI|
|endpointPath|Non-Spaced string that specifies the path, i.e. /api/v1/payloads|Used to form the path portion of the URI|
|endpointPort|Integer value|Port of the endpoint|
|mimeType|application/json \| text/xml \| text/csv|Specifies the Content-Type of the payload|

---

## <a id="socketserver"></a>Socket Server
Exposes a TCP SocketServer that payloads get sent to and any connected clients will receive the configured payloads.

### Structure
```json
"sinks": [
  {
    "type": "SocketServer",
    "endPointPort": 9000
  }
]
```

### Parameters
|Parameter Name|Available Values|Description|
|--|--|--|
|type|SocketServer|Specifies the sink type of SocketServer|
|endPointPort|1024 - 65535|Any available port you'd like the server to listen on|