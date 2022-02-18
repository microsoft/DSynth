<h1 id="dsynth">DSynth</h1>

## GET /api/v{version}/DSynth/Stop

```shell
# You can also use wget
curl -X GET /api/v{version}/DSynth/Stop

```

`GET /api/v{version}/DSynth/Stop`

*Stops DSynth*

<h3 id="get__api_v{version}_dsynth_stop-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|version|path|string|true|none|

<h3 id="get__api_v{version}_dsynth_stop-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|Success|None|

<aside class="success">
This operation does not require authentication
</aside>

<br/>
<br/>
<br/>

## /api/v{version}/DSynth/Start

> Code samples

```shell
# You can also use wget
curl -X GET /api/v{version}/DSynth/Start

```

`GET /api/v{version}/DSynth/Start`

*Starts DSynth*

<h3 id="get__api_v{version}_dsynth_start-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|version|path|string|true|none|

<h3 id="get__api_v{version}_dsynth_start-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|Success|None|

<aside class="success">
This operation does not require authentication
</aside>

<br/>
<br/>
<br/>

## GET /api/v{version}/DSynth/Restart

> Code samples

```shell
# You can also use wget
curl -X GET /api/v{version}/DSynth/Restart

```

`GET /api/v{version}/DSynth/Restart`

*Restarts DSynth*

<h3 id="get__api_v{version}_dsynth_restart-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|version|path|string|true|none|

<h3 id="get__api_v{version}_dsynth_restart-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|Success|None|

<aside class="success">
This operation does not require authentication
</aside>

<br/>
<br/>
<br/>

## /api/v{version}/DSynth/ProvidersOptions

> Code samples

```shell
# You can also use wget
curl -X GET /api/v{version}/DSynth/ProvidersOptions

```

`GET /api/v{version}/DSynth/ProvidersOptions`

*Retrieves the currently running provider options*

<h3 id="get__api_v{version}_dsynth_providersoptions-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|version|path|string|true|none|

<h3 id="get__api_v{version}_dsynth_providersoptions-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|Success|None|

<aside class="success">
This operation does not require authentication
</aside>

<br/>
<br/>
<br/>

## POST /api/v{version}/DSynth/ProviderOptions

> Code samples

```shell
# You can also use wget
curl -X POST /api/v{version}/DSynth/ProviderOptions \
  -H 'Content-Type: application/json-patch+json'

```

`POST /api/v{version}/DSynth/ProviderOptions`

*Updates the currently running provider options*

> Body parameter

```json
[
  {
    "isPushEnabled": true,
    "type": "string",
    "providerName": "string",
    "sink": null,
    "intervalInMs": 0,
    "templateName": "string",
    "minBatchSize": 0,
    "maxBatchSize": 0,
    "maxIterations": 0,
    "advancedOptions": {
      "pushDisabledIntervalInMs": 0,
      "queueFillDelayInMs": 0,
      "queueWorkers": 0,
      "paddingMultiplier": 0,
      "warnQueueExhaustion": true
    }
  }
]
```

<h3 id="post__api_v{version}_dsynth_provideroptions-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|version|path|string|true|none|
|body|body|[DSynthProviderOptions](#schemadsynthprovideroptions)|false|none|

<h3 id="post__api_v{version}_dsynth_provideroptions-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|Success|None|

<aside class="success">
This operation does not require authentication
</aside>

<br/>
<br/>
<br/>

## GET /api/v{version}/DSynth/Profiles

> Code samples

```shell
# You can also use wget
curl -X GET /api/v{version}/DSynth/Profiles

```

`GET /api/v{version}/DSynth/Profiles`

*Gets the available profiles that can be activated*

<h3 id="get__api_v{version}_dsynth_profiles-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|version|path|string|true|none|

<h3 id="get__api_v{version}_dsynth_profiles-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|Success|None|

<aside class="success">
This operation does not require authentication
</aside>

<br/>
<br/>
<br/>

## POST /api/v{version}/DSynth/Profiles/Import

> Code samples

```shell
# You can also use wget
curl -X POST /api/v{version}/DSynth/Profiles/Import \
  -H 'Content-Type: multipart/form-data'

```

`POST /api/v{version}/DSynth/Profiles/Import`

*Imports packaged profile[s]*

> Body parameter

```yaml
file: string

```

<h3 id="post__api_v{version}_dsynth_profiles_import-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|version|path|string|true|none|
|body|body|object|false|none|
|» file|body|string(binary)|false|none|

<h3 id="post__api_v{version}_dsynth_profiles_import-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|Success|None|

<aside class="success">
This operation does not require authentication
</aside>

<br/>
<br/>
<br/>

## GET /api/v{version}/DSynth/Profiles/Export

> Code samples

```shell
# You can also use wget
curl -X GET /api/v{version}/DSynth/Profiles/Export

```

`GET /api/v{version}/DSynth/Profiles/Export`

*Exports all profiles as a package*

<h3 id="get__api_v{version}_dsynth_profiles_export-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|version|path|string|true|none|

<h3 id="get__api_v{version}_dsynth_profiles_export-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|Success|None|

<aside class="success">
This operation does not require authentication
</aside>

<br/>
<br/>
<br/>

## POST /api/v{version}/DSynth/Profiles/Activate/{profileName}

> Code samples

```shell
# You can also use wget
curl -X POST /api/v{version}/DSynth/Profiles/Activate/{profileName}

```

`POST /api/v{version}/DSynth/Profiles/Activate/{profileName}`

*Activates a given profile*

<h3 id="post__api_v{version}_dsynth_profiles_activate_{profilename}-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|profileName|path|string|true|none|
|version|path|string|true|none|

<h3 id="post__api_v{version}_dsynth_profiles_activate_{profilename}-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|Success|None|

<aside class="success">
This operation does not require authentication
</aside>

<br/>
<br/>
<br/>

## GET /api/v{version}/DSynth/Status

> Code samples

```shell
# You can also use wget
curl -X GET /api/v{version}/DSynth/Status

```

`GET /api/v{version}/DSynth/Status`

*Retrieves the status of DSynth*

<h3 id="get__api_v{version}_dsynth_status-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|version|path|string|true|none|

<h3 id="get__api_v{version}_dsynth_status-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|Success|None|

<aside class="success">
This operation does not require authentication
</aside>

<br/>
<br/>
<br/>

## GET /api/v{version}/DSynth/Providers

> Code samples

```shell
# You can also use wget
curl -X GET /api/v{version}/DSynth/Providers

```

`GET /api/v{version}/DSynth/Providers`

*Retrieves all providers that can be used with GetNextPayload*

<h3 id="get__api_v{version}_dsynth_providers-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|version|path|string|true|none|

<h3 id="get__api_v{version}_dsynth_providers-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|Success|None|

<aside class="success">
This operation does not require authentication
</aside>

<br/>
<br/>
<br/>

## GET /api/v{version}/DSynth/Providers/{providerName}/GetNextPayload

> Code samples

```shell
# You can also use wget
curl -X GET /api/v{version}/DSynth/Providers/{providerName}/GetNextPayload

```

`GET /api/v{version}/DSynth/Providers/{providerName}/GetNextPayload`

*Retrieves a payload for a given provider*

<h3 id="get__api_v{version}_dsynth_providers_{providername}_getnextpayload-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|providerName|path|string|true|none|
|version|path|string|true|none|

<h3 id="get__api_v{version}_dsynth_providers_{providername}_getnextpayload-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|Success|None|

<aside class="success">
This operation does not require authentication
</aside>

<br/>
<br/>
<br/>

# Schemas

<h2 id="tocS_AdvancedOptions">AdvancedOptions</h2>
<!-- backwards compatibility -->
<a id="schemaadvancedoptions"></a>
<a id="schema_AdvancedOptions"></a>
<a id="tocSadvancedoptions"></a>
<a id="tocsadvancedoptions"></a>

```json
{
  "pushDisabledIntervalInMs": 0,
  "targetQueueSize": 0,
  "queueWorkers": 0
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|pushDisabledIntervalInMs|integer(int32)|false|none|none|
|targetQueueSize|integer(int32)|false|none|none|
|queueWorkers|integer(int32)|false|none|none|

<h2 id="tocS_DSynthProviderOptions">DSynthProviderOptions</h2>
<!-- backwards compatibility -->
<a id="schemadsynthprovideroptions"></a>
<a id="schema_DSynthProviderOptions"></a>
<a id="tocSdsynthprovideroptions"></a>
<a id="tocsdsynthprovideroptions"></a>

```json
{
  "isPushEnabled": true,
  "type": "string",
  "providerName": "string",
  "sink": null,
  "intervalInMs": 0,
  "templateName": "string",
  "minBatchSize": 0,
  "maxBatchSize": 0,
  "maxIterations": 0,
  "advancedOptions": {
    "pushDisabledIntervalInMs": 0,
    "targetQueueSize": 0,
    "queueWorkers": 0
  },
  "targetQueueSize": 0
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|isPushEnabled|boolean|false|none|none|
|type|string¦null|false|none|none|
|providerName|string¦null|false|none|none|
|sink|any|false|none|none|
|intervalInMs|integer(int32)|false|none|none|
|templateName|string¦null|false|none|none|
|minBatchSize|integer(int32)|false|none|none|
|maxBatchSize|integer(int32)|false|none|none|
|maxIterations|integer(int32)|false|none|none|
|advancedOptions|[AdvancedOptions](#schemaadvancedoptions)|false|none|none|
|targetQueueSize|integer(int32)|false|read-only|none|

