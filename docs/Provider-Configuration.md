# Provider Configuration
A provider defines the type of payload that gets sent to a specified sink and/or the type of payload that is made available via endpoint.

Given the flexibility of DSynth, you are not bound to having a single instance per use case in most cases and can leverage a single instance to provide payloads to many different sinks at a time. Once you start pushing very high throughput payloads you may want to consider scaling out DSynth. There is no guidance on numbers and when to scale out - this is research that still needs to be done.

# dsynth.json
Out of the box, DSynth will look for providers in the `sample-profile.json` file under the `./Profiles` folder. This setting can be changed in `dsynth.json`.

## Structure of `dsynth.json`
```json
{
  "providersFile": "sample-providers.json"
}
```

|Parameter Name|Available Values|Description|
|--|--|--|
|providersFile|File name value|The file name of the providers configuration located under the `./Profiles` directory|

## Structure of `appsettings.json`
|Parameter Name|Available Values|Description|
|--|--|--|
|dsynthEnvironmentName|Non-Spaced string value|Provides a friendly environment name that is mainly used to help query Application Insights, **Default: EnvironmentNameNotSet**|
|applicationInsights.connectionString|Non-Spaced string value|When specified, logs will be shipped to Application Insights. Environment variable is also suported as outlined here: https://docs.microsoft.com/en-us/azure/azure-monitor/app/sdk-connection-string?tabs=net|

# Providers Configuration Parameters
The following parameters are available for each Provider. You can have multiple profiles that describe certain workloads and to switch between the different profiles would just require updating the `providersFile` setting in `dsynth.json` or via the API.

## Structure

```json
{
  "providers": [
    {
    "isPushEnabled": true,
    "type": "JSON",
    "providerName": "ProviderName",
    "sinks": [{}],
    "intervalInMs": 10000,
    "templateName": "JsonSample.template.csv",
    "minBatchSize": 1,
    "maxBatchSize": 0,
    "maxIterations": 0,
    "terminateWhenComplete": false,
    "advancedOptions": {
      "pushDisabledIntervalInMs": 10000,
      "targetQueueSize": 50000,
      "queueWorkers": 1
    }
  ]
}
```

## Standard Configuration Parameters
|Parameter Name|Available Values|Description|
|--|--|--|
|isPushEnabled|true \| false|Enables or disables pushing to the configured sink. Regardless if push is enabled or disabled, you can still pull from the endpoint|
|type|JSON \| XML \| CSV \| RAW \| Image \| JSONL|The payload type that will be generated|
|providerName|Non-Spaced string value|Will be used to map to the sink configuration, as well as the endpoint name to perform pull operations. **This needs to be unique**|
|sinks|A collection of sink configurations|Sink configuration parameters (See [Sinks](./docs/Sinks.md) for details)|
|intervalInMs|Integer value|How often to ship payloads to the configured sink|
|templateName|Non-Spaced string value|The name of the template file under `./Profiles/Templates`|
|minBatchSize|Integer value|The minimum number of payloads that get sent to a configured sink|
|maxBatchSize|Integer value and if specified, needs to be >= minBatchSize|The maximum number of payloads that gets sent to a configured sink|
|maxIterations|Integer value|The maximum number of times payloads will be shipped to a sink. If set to 0, it will continue to ship until stopped or the application is terminated.|
|terminateWhenComplete (optional)|true \| false |Allows the terminating DSynth once iterations are completed. **Only available when running in headless mode**. Default false|

## Advanced Configuration Parameters
These should not need to be touched in most regular use cases. They have defaults assigned and do not require to be defined in the Payload Provider Configuration file.
|Parameter Name|Available Values|Description|
|--|--|--|
|pushDisabledIntervalInMs|Integer value|When a provider goes from isPushEnabled = true to false, we reduce the frequency that we check for it to be enabled to reduce additional overhead. When isPushEnabled gets reenabled, there will be a maximum of a 10 second delay before the payload provider starts sending requests again. **Default 10000**|
|targetQueueSize|Integer value|The number of items to keep in the payload queue. It is recommended to set this to the number minBatchSize or maxBatchSize, whichever is greater. **Default 50000**|
|queueWorkers|Integer value|The number of queue workers populating the payload queue. As throughput increases, 1 worker may have troubles re populating the queue and this number may need to be adjusted. **Default 1**|


## Supported Sink Configurations by Provider Type

|Provider|Azure Blob|Azure Cosmos DB|Azure Event Hubs|Azure Service Bus|File|Console|Http|Socket Server|
|--:|:--:|:--:|:--:|:--:|:--:|:--:|:--:|:--:|
|CSV|<span style="color:green">Yes</span>|<span style="color:orange">No</span>|<span style="color:orange">No</span>|<span style="color:orange">No</span>|<span style="color:green">Yes</span>|<span style="color:green">Yes</span>|<span style="color:green">Yes</span>|<span style="color:green">Yes</span>|
|Image|<span style="color:green">Yes</span>|<span style="color:orange">No</span>|<span style="color:orange">No</span>|<span style="color:orange">No</span>|<span style="color:green">Yes</span>|<span style="color:orange">No</span>|<span style="color:orange">No</span>|<span style="color:orange">No</span>|
|JSON|<span style="color:green">Yes</span>|<span style="color:green">Yes</span>|<span style="color:green">Yes</span>|<span style="color:green">Yes</span>|<span style="color:green">Yes</span>|<span style="color:green">Yes</span>|<span style="color:green">Yes</span>|<span style="color:green">Yes</span>|
|JSONL (Json Lines)|<span style="color:green">Yes</span>|<span style="color:orange">No</span>|<span style="color:green">Yes</span>|<span style="color:green">Yes</span>|<span style="color:green">Yes</span>|<span style="color:green">Yes</span>|<span style="color:green">Yes</span>|<span style="color:green">Yes</span>|
|RAW|<span style="color:green">Yes</span>|<span style="color:orange">No</span>|<span style="color:orange">No</span>|<span style="color:orange">No</span>|<span style="color:green">Yes</span>|<span style="color:green">Yes</span>|<span style="color:green">Yes</span>|<span style="color:green">Yes</span>|
|XML|<span style="color:green">Yes</span>|<span style="color:orange">No</span>|<span style="color:green">Yes</span>|<span style="color:orange">No</span>|<span style="color:green">Yes</span>|<span style="color:green">Yes</span>|<span style="color:green">Yes</span>|<span style="color:green">Yes</span>|

## Special Provider Considerations
### Image
The image provider type does not support batching.

|Parameter Name|Available Values|Description|
|--|--|--|
|payloadMinBatchSize|1|Required to be set to 1|
|payloadMaxBatchSize|0|Required to be set to 0|

```
Note:
When using either AzureBlob or File sink, the "fileNameSuffix" will be overridden by "imageFormat" from the image template.
```







