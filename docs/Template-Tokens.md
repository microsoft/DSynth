# Template Tokens
Template tokens are used to specify what values get replace inside of each template and have the following base format `{{TokenHandlerType:Source}}`. The tokens called out in the template files will be replaced with data from the `Source` with the returned (`ReplacementValue`) value. Some tokens will have additional parameters - more below.

# Supported Tokens
## Guid
Gets substituted with a guid.

|TokenHandlerType|Source|ReplacementValue|Token|
|--|--|--|--|
|Guid|NewGuid|Guid as string|{{Guid:NewGuid}}|
|Guid|Tracked|Guid as string|{{Guid:Tracked:trackedKey}}|
|Guid|Reference|Guid as string|{{Guid:Reference:trackedKey}}|

## MacAddress
Returns a random value in the format of a mac address

|TokenHandlerType|Source|ReplacementValue|Token|
|--|--|--|--|
|MacAddress|MacAddress|String in the format of a mac address|{{MacAddress:MacAddress}}|

## Timestamp
Gets substituted with a timestamp value in the desired format.

|TokenHandlerType|Source|Format|ReplacementValue|Token|
|--|--|--|--|--|
|Timestamp|DateTime|UnixTimeInMs \| UTCISO8601|Timestamp in the desired format|{{DateTime:DateTime:UnixTimeInMs}}|
|Timestamp|DateTimeTracked|UnixTimeInMs \| UTCISO8601|Timestamp in the desired format|{{DateTime:DateTimeTracked:UnixTimeInMs:trackedKey}}|
|Timestamp|DateTimeReference|Whatever the format is for `trackedKey`|The value assigned to `trackedKey` |{{DateTime:DateTimeReference:trackedKeyName}}|

## DateTime
Gets substituted with a DateTime value between a range for a given segment in the desired format.

|TokenHandlerType|Source|Format|TimeSegment|Range|Token|
|--|--|--|--|--|--|--|
|DateTime|Range|UnixTimeInMs \| UTCISO8601 \| DateISO8601|Years \| Months \| Days \| Hours \| Minutes \| Seconds \| Milliseconds|{{DateTime:Range:UTCISO8601:Years:1..5}}|

## Number
Gets substituted with a number int or double.

|TokenHandlerType|Source|ReplacementValue|Precision|Token|
|--|--|--|--|--|
|Number|Range|A random number with the specified precision between `0 and 15`|0 - 15|{{Number:Range:0..15:0}}|

|TokenHandlerType|Source|Start and End Range|Deviation Amount|Deviation Weight|Token|
|--|--|--|--|--|--|
|Number|IncrementTracked|Number value specifying start and end, i.e. `1.0..2.0` or `1..10`|The range that the value will deviate, i.e. `0.001..0.003` or `1..3`|How often the value will deviate, i.e. `1` will be 1% of the time. Setting this to `1` will deviate 100% of the time.|{{Number:IncrementTracked:1.0..2.0:0.1..0.3:0.1:trackedKeyName}}|
|Number|DecrementTracked|Number value specifying start and end, i.e. `1.0..2.0` or `1..10`|The range that the value will deviate, i.e. `0.001..0.003` or `1..3`|How often the value will deviate, i.e. `1` will be 1% of the time. Setting this to `1` will deviate 100% of the time.|{{Number:DecrementTracked:2.0..1.0:0.1..0.3:0.1:trackedKeyName}}|
|Number|Reference|N/A|N/A|N/A|{{Number:Reference:trackedKeyName}}

## JsonCollection
Gets substituted with a random value defined in a [Template Collection](./Template-Collections.md) file. For example, if I have a template collections file named `Sample.collections.json` and have a ReplacementValue of `sampleLocationNames`, `{{JsonCollection:Collection:Sample:sampleLocationNames}}`, DSynth will look for an array named `sampleLocationNames` in `Sample.collections.json` and select a random element from the array.

|TokenHandlerType|Source|ReplacementValue|Token|
|--|--|--|--|
|JsonCollection|Collection|Value from a collection file|{{JsonCollection:Collection:Sample:sampleLocationNames}}|
|JsonCollection|Tracked|Value from a collection file and tracks via key to be referenced later|{{JsonCollection:Tracked:Sample:sampleLocationNames:trackedKey}}|
|JsonCollection|Reference|A reference value for a given key|{{JsonCollection:Reference:trackedKey}}|

## CsvCollection
Gets substituted with a random value defined in a [Template Collection](./Template-Collections.md) file. For example, if I have a template collections file named `Sample.collections.csv` and have a ReplacementValue of `Model`, `{{CsvCollection:Collection:Sample:Model}}`, DSynth will look for a column named `Model` in `Sample.collections.csv` and select a random row to pull `Model` from.

|TokenHandlerType|Source|ReplacementValue|Token|
|--|--|--|--|
|CsvCollection|Collection|Value from a collection file|{{CsvCollection:Collection:Sample:Model}}|
|CsvCollection|Tracked|Value from a collection file and tracks via key to be referenced later|{{CsvCollection:Tracked:Sample:Model:trackedKey}}|
|CsvCollection|Reference|A reference value for a given key|{{CsvCollection:Reference:trackedKey}}|
|CsvCollection|TrackedLimit|A value within a specified limit. I.e., if 25 is specified, only the top 25% of rows will be used.|{{CsvCollection:TrackedLimit:25:Sample:Model:trackedKey}}|

## Nested
Allows nesting a Json template within a template to allow for dynamic collections of objects to be created
|TokenHandlerType|Source|ReplacementValue|TemplateName|Range|Token|
|--|--|--|--|--|--|
|Nested|Json|A nested template|The full name of the template|The number of nested objects to create|{{Nested:Json:JsonSampleNested.template.json:1..3}}

# Tracked and Reference Tokens
Tracked tokens provide a little less randomness and some can be referenced in other positions in a given template. There are tokens that leverage the tracked functionality to increment or decrement. Take the following example below, we use the we use the source of `TimestampTracked` with a format of `UTCISO8601`. Then we can reference this value throughout the rest of this template.

### Tracked and Reference example
```json
{
  "utcIso8601TimestampTracked": "{{Timestamp:DateTimeTracked:UTCISO8601:timestampKey}}",
  "utcIso8601TimestampReference": "{{Timestamp:DateTimeReference:timestampKey}}"
}
```

### Increment and Decrement example
```json
{
  "sampleIncrement": "{{Number:IncrementTracked:0..10:1..1:0:incrementKey}}",
  "sampleDecrement": "{{Number:DecrementTracked:0..10:10..1:0:incrementKey}}"
}
```