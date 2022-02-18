# Template Formats
Template Formats, in most cases, look close to their intended formats. Template files reside under `Profiles/Templates` and need to have `.template.` in their file name for DSynth to load the template. I.e., `Sample.template.json`

## Required MetadataParameters that are used in the respective template files

### CSV
```
::Header=Comma,Separated,Header,Values
```

---

# Example Templates
The following examples show what each template would look like and how Template Tokens [Template Tokens](./Template-Tokens.md) are used.

```
Note:
The following examples have tokens for all parameters and if you want to use a static value, you are free
to specify one and not required to leverage a token for every value.
```

## CSV
Sample Template:

```
::Header="stringValue","numberValue","boolValue","guid","macAddress","timestampUnixMs","timestampIso8601","timestampIso8601Tracked","timestampIso8601Reference","timestampUnixMsTracked","timestampUnixMsReference","dateTimeRangeIso8601","dateTimeRangeUnixTimeInMs","jsonCollection","jsonCollectionTracked","jsonCollectionReference","csvCollection","csvCollectionTracked","csvCollectionReferenceSameFieldInRow","csvCollectionReferenceAnotherFieldInRow","numberWithNoPrecision","numberWithFivePrecision","numberIncrementTracked","numberIncrementReference","numberDecrementTracked","numberDecrementReference"
"sample static string", "100", "true", "{{Guid:NewGuid}}", "{{MacAddress:MacAddress}}", "{{Timestamp:DateTime:UnixTimeInMs}}", "{{Timestamp:DateTime:UTCISO8601}}", "{{Timestamp:Tracked:UTCISO8601:iso8601TrackedKey}}", "{{Timestamp:Reference:iso8601TrackedKey}}", "{{Timestamp:Tracked:UnixTimeInMs:unixMsTrackedKey}}", "{{Timestamp:reference:unixMsTrackedKey}}", "{{DateTime:Range:UTCISO8601:Years:1..5}}", "{{DateTime:Range:UnixTimeInMs:Years:1..5}}", "{{JsonCollection:Collection:Sample:sampleLocationNames}}", "{{JsonCollection:Tracked:Sample:sampleLocationNames:jsonTrackedKey}}", "{{JsonCollection:Reference:jsonTrackedKey}}", "{{CsvCollection:Collection:Sample:Model}}", "{{CsvCollection:Tracked:Sample:Model:csvTrackedKey}}", "{{CsvCollection:Reference:Model:csvTrackedKey}}", "{{CsvCollection:Reference:Color:csvTrackedKey}}", "{{number:Range:0..100:0}}", "{{number:Range:0..100:5}}", "{{number:IncrementTracked:1.0..2.0:0.1..0.3:100:numberIncTrackedKey}}", "{{number:Reference:numberIncTrackedKey}}", "{{Number:DecrementTracked:20..1:1..3:100:numberDecTrackedKey}}", "{{number:Reference:numberDecTrackedKey}}"
```

Output:
```csv
"stringValue","numberValue","boolValue","guid","macAddress","timestampUnixMs","timestampIso8601","timestampIso8601Tracked","timestampIso8601Reference","timestampUnixMsTracked","timestampUnixMsReference","dateTimeRangeIso8601","dateTimeRangeUnixTimeInMs","jsonCollection","jsonCollectionTracked","jsonCollectionReference","csvCollection","csvCollectionTracked","csvCollectionReferenceSameFieldInRow","csvCollectionReferenceAnotherFieldInRow","numberWithNoPrecision","numberWithFivePrecision","numberIncrementTracked","numberIncrementReference","numberDecrementTracked","numberDecrementReference"
"sample static string", "100", "true", "bb341b42-ff06-4d32-b687-294f1fe27bfc", "6F:8C:CC:88:63:F0", "1628106858378", "2021-08-04T19:54:18.3789050Z", "2021-08-04T19:54:18.3789300Z", "2021-08-04T19:54:18.3789300Z", "1628106858378", "1628106858378", "2023-08-04T19:54:18.3790040Z", "1659642858379", "Washington State", "Washington DC", "Washington DC", "Ford", "Honda", "Honda", "Green", "96", "68.80184", "1.2030234784367604", "1.2030234784367604", "18", "18"
```

---

## Image
Images are a unique provider type and we use JSON format to describe what an image will look like. For all available colors, font families and image formats, please see the collections under `src/DSynth/Profiles/Collections/Sample.collections.json`
```
{
  "imageText": "{{JsonCollection:Collection:Sample:availableText}}",
  "fontSize": {{Number:Range:70..200:0}},
  "fontFamily": "{{JsonCollection:Collection:Sample:availableFontFamilies}}",
  "foregroundColor": "{{JsonCollection:Collection:Sample:availableColors}}",
  "backgroundColor": "{{JsonCollection:Collection:Sample:availableColors}}",
  "imageFormat": "{{JsonCollection:Collection:Sample:availableFormats}}"
}
```

Output:
![image.png](/.attachments/image-7dbc1ca7-45d3-4fff-ad3f-14137fafa048.png)

---

## JSON
Sample Template:
```json
{
  "staticExamples":
  {
    "stringValue": "sample static string",
    "numberValue": 100,
    "boolValue": true
  },
  "guidExamples": {
    "guid": "{{Guid:NewGuid}}"
  },
  "macAddressExamples":
  {
    "macAddress": "{{MacAddress:MacAddress}}"
  },
  "dateTimeExamples": 
  {
    "timestampUnixMs": "{{Timestamp:DateTime:UnixTimeInMs}}",
    "timestampIso8601": "{{Timestamp:DateTime:UTCISO8601}}",
    "timestampIso8601Tracked": "{{Timestamp:Tracked:UTCISO8601:iso8601TrackedKey}}",
    "timestampIso8601Reference": "{{Timestamp:Reference:iso8601TrackedKey}}",
    "timestampUnixMsTracked": "{{Timestamp:Tracked:UnixTimeInMs:unixMsTrackedKey}}",
    "timestampUnixMsReference": "{{Timestamp:reference:unixMsTrackedKey}}",
    "dateTimeRangeIso8601": "{{DateTime:Range:UTCISO8601:Years:1..5}}",
    "dateTimeRangeUnixMs": "{{DateTime:Range:UnixTimeInMs:Years:1..5}}"
  },
  "jsonCollectionsExamples": 
  {
    "jsonCollection": "{{JsonCollection:Collection:Sample:sampleLocationNames}}",
    "jsonCollectionTracked": "{{JsonCollection:Tracked:Sample:sampleLocationNames:jsonTrackedKey}}",
    "jsonCollectionReference": "{{JsonCollection:Reference:jsonTrackedKey}}"
  },
  "csvCollectionsExamples":
  {
    "csvCollection": "{{CsvCollection:Collection:Sample:Model}}",
    "csvCollectionTracked": "{{CsvCollection:Tracked:Sample:Model:csvTrackedKey}}",
    "csvCollectionReferenceSameFieldInRow": "{{CsvCollection:Reference:Model:csvTrackedKey}}",
    "csvCollectionReferenceAnotherFieldInRow": "{{CsvCollection:Reference:Color:csvTrackedKey}}"
  },
  "numberExamples":
  {
    "numberWithNoPrecision": "{{Number:Range:0..100:0}}",
    "numberWithFivePrecision": "{{Number:Range:0..100:5}}",
    "numberIncrementTracked": "{{Number:IncrementTracked:1.0..2.0:0.1..0.3:100:numberIncTrackedKey}}",
    "numberIncrementReference": "{{Number:Reference:numberIncTrackedKey}}",
    "numberDecrementTracked": "{{Number:DecrementTracked:20..1:1..3:100:numberDecTrackedKey}}",
    "numberDecrementReference": "{{Number:Reference:numberDecTrackedKey}}"
  },
  "nestedJsonExamples":
  {
    "nestedArray": 
    [
      {{Nested:Json:JsonSampleNested.template.json:1..3}}
    ]
  }
}
```

Output:
```json
{
  "staticExamples": {
    "stringValue": "sample static string",
    "numberValue": 100,
    "boolValue": true
  },
  "guidExamples": {
    "guid": "ff4562ca-7567-4fc1-9353-4a8838d58dad"
  },
  "macAddressExamples": {
    "macAddress": "3C:91:72:06:46:07"
  },
  "dateTimeExamples": {
    "timestampUnixMs": "1628106975356",
    "timestampIso8601": "2021-08-04T19:56:15.35631Z",
    "timestampIso8601Tracked": "2021-08-04T19:56:15.356326Z",
    "timestampIso8601Reference": "2021-08-04T19:56:15.356326Z",
    "timestampUnixMsTracked": "1628106975356",
    "timestampUnixMsReference": "1628106975356",
    "dateTimeRangeIso8601": "2023-08-04T19:56:15.356375Z",
    "dateTimeRangeUnixMs": "1659642975356"
  },
  "jsonCollectionsExamples": {
    "jsonCollection": "Washington State",
    "jsonCollectionTracked": "Washington State",
    "jsonCollectionReference": "Washington State"
  },
  "csvCollectionsExamples": {
    "csvCollection": "Honda",
    "csvCollectionTracked": "Toyota",
    "csvCollectionReferenceSameFieldInRow": "Toyota",
    "csvCollectionReferenceAnotherFieldInRow": "Red"
  },
  "numberExamples": {
    "numberWithNoPrecision": "6",
    "numberWithFivePrecision": "65.71819",
    "numberIncrementTracked": "1.2979179065664848",
    "numberIncrementReference": "1.2979179065664848",
    "numberDecrementTracked": "18",
    "numberDecrementReference": "18"
  },
  "nestedJsonExamples": {
    "nestedArray": [
      {
        "nestedIdExample": "6d7c130e-db92-4b8d-845b-bedf83bb11c2",
        "nestedTimestampExample": "2021-08-04T19:56:15.356798Z",
        "nestedCollectionExample": "Washington DC"
      }
    ]
  }
}
```

## JSONL (JSON Lines)
[JSON Lines](https://jsonlines.org/) is a format where each line in a file is valid JSON. It is important to note that your JSON Lines template needs to be a single line. The overhead to convert compact JSON to JSON Lines is expensive (both memory and compute) when working with larger JSON Lines collections, i.e., a file size of 128Mb (200k rows+), a file size of 256Mb (400k rows+), etc...

The following example is for brevity and JSON Lines supports all template tokens. When referencing nested JSON, make sure your nested template is also single line.

Sample Template:
```json
{"guidExamples":{"guid":"{{Guid:NewGuid}}"}
```
Output:
```json
{"guidExamples":{"guid":"de1234ca-7567-4fc1-9353-4a4538d58dad"}
```

## XML
Sample Template:
```xml
<?xml version="1.0" encoding="utf-8"?>
<Template>
    <StaticExamples>
        <StringValue>sample static string</StringValue>
        <NumberValue>100</NumberValue>
        <BoolValue>true</BoolValue>
    </StaticExamples>
    <GuidExamples>
        <Guid>{{Guid:NewGuid}}</Guid>
    </GuidExamples>
    <MacAddressExamples>
        <MacAddress>{{MacAddress:MacAddress}}</MacAddress>
    </MacAddressExamples>
    <DateTimeExamples>
        <TimestampUnixMs>{{Timestamp:DateTime:UnixTimeInMs}}</TimestampUnixMs>
        <TimestampIso8601>{{Timestamp:DateTime:UTCISO8601}}</TimestampIso8601>
        <TimestampIso8601Tracked>{{Timestamp:Tracked:UTCISO8601:iso8601TrackedKey}}</TimestampIso8601Tracked>
        <TimestampIso8601Reference>{{Timestamp:Reference:iso8601TrackedKey}}</TimestampIso8601Reference>
        <TimestampUnixMsTracked>{{Timestamp:Tracked:UnixTimeInMs:unixMsTrackedKey}}</TimestampUnixMsTracked>
        <TimestampUnixMsReference>{{Timestamp:reference:unixMsTrackedKey}}</TimestampUnixMsReference>
        <DateTimeRangeIso8601>{{DateTime:Range:UTCISO8601:Years:1..5}}</DateTimeRangeIso8601>
        <DateTimeRangeUnixTimeMs>{{DateTime:Range:UnixTimeInMs:Years:1..5}}</DateTimeRangeUnixTimeMs>
    </DateTimeExamples>
    <JsonCollectionsExamples>
        <JsonCollection>{{JsonCollection:Collection:Sample:sampleLocationNames}}</JsonCollection>
        <JsonCollectionTracked>{{JsonCollection:Tracked:Sample:sampleLocationNames:jsonTrackedKey}}</JsonCollectionTracked>
        <JsonCollectionReference>{{JsonCollection:Reference:jsonTrackedKey}}</JsonCollectionReference>
    </JsonCollectionsExamples>
    <CsvCollectionsExamples>
        <CsvCollection>{{CsvCollection:Collection:Sample:Model}}</CsvCollection>
        <CsvCollectionTracked>{{CsvCollection:Tracked:Sample:Model:csvTrackedKey}}</CsvCollectionTracked>
        <CsvCollectionReferenceSameFieldInRow>{{CsvCollection:Reference:Model:csvTrackedKey}}</CsvCollectionReferenceSameFieldInRow>
        <CsvCollectionReferenceAnotherFieldInRow>{{CsvCollection:Reference:Color:csvTrackedKey}}</CsvCollectionReferenceAnotherFieldInRow>
    </CsvCollectionsExamples>
    <DoubleExamples>
        <NumberWithNoPrecision>{{Number:Range:0..100:0}}</NumberWithNoPrecision>
        <NumberWithFivePrecision>{{Number:Range:0..100:5}}</NumberWithFivePrecision>
        <NumberIncrementTracked>{{Number:IncrementTracked:1.0..2.0:0.1..0.3:100:numberIncTrackedKey}}</NumberIncrementTracked>
        <NumberIncrementReference>{{Number:Reference:numberIncTrackedKey}}</NumberIncrementReference>
        <NumberDecrementTracked>{{Number:DecrementTracked:2.0..1.0:0.1..0.3:100:numberDecTrackedKey}}</NumberDecrementTracked>
        <NumberDecrementReference>{{Number:Reference:numberDecTrackedKey}}</NumberDecrementReference>
    </DoubleExamples>
</Template>
```

Output:
```xml
<?xml version="1.0" encoding="utf-8"?>
<ArrayOfXmlNode xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <XmlNode>
    <StaticExamples>
      <StringValue>sample static string</StringValue>
      <NumberValue>100</NumberValue>
      <BoolValue>true</BoolValue>
    </StaticExamples>
  </XmlNode>
  <XmlNode>
    <GuidExamples>
      <Guid>73c891dd-4a00-47cb-b829-c90cf6013e81</Guid>
    </GuidExamples>
  </XmlNode>
  <XmlNode>
    <MacAddressExamples>
      <MacAddress>9F:DE:16:69:38:81</MacAddress>
    </MacAddressExamples>
  </XmlNode>
  <XmlNode>
    <DateTimeExamples>
      <TimestampUnixMs>1628106975357</TimestampUnixMs>
      <TimestampIso8601>2021-08-04T19:56:15.3574550Z</TimestampIso8601>
      <TimestampIso8601Tracked>2021-08-04T19:56:15.3574710Z</TimestampIso8601Tracked>
      <TimestampIso8601Reference>2021-08-04T19:56:15.3574710Z</TimestampIso8601Reference>
      <TimestampUnixMsTracked>1628106975357</TimestampUnixMsTracked>
      <TimestampUnixMsReference>1628106975357</TimestampUnixMsReference>
      <DateTimeRangeIso8601>2024-08-04T19:56:15.3575330Z</DateTimeRangeIso8601>
      <DateTimeRangeUnixTimeMs>1691178975357</DateTimeRangeUnixTimeMs>
    </DateTimeExamples>
  </XmlNode>
  <XmlNode>
    <JsonCollectionsExamples>
      <JsonCollection>North Dakota</JsonCollection>
      <JsonCollectionTracked>North Dakota</JsonCollectionTracked>
      <JsonCollectionReference>North Dakota</JsonCollectionReference>
    </JsonCollectionsExamples>
  </XmlNode>
  <XmlNode>
    <CsvCollectionsExamples>
      <CsvCollection>Honda</CsvCollection>
      <CsvCollectionTracked>Toyota</CsvCollectionTracked>
      <CsvCollectionReferenceSameFieldInRow>Toyota</CsvCollectionReferenceSameFieldInRow>
      <CsvCollectionReferenceAnotherFieldInRow>Red</CsvCollectionReferenceAnotherFieldInRow>
    </CsvCollectionsExamples>
  </XmlNode>
  <XmlNode>
    <DoubleExamples>
      <NumberWithNoPrecision>92</NumberWithNoPrecision>
      <NumberWithFivePrecision>33.00682</NumberWithFivePrecision>
      <NumberIncrementTracked>1.1038638126123062</NumberIncrementTracked>
      <NumberIncrementReference>1.1038638126123062</NumberIncrementReference>
      <NumberDecrementTracked>1.7647642785984856</NumberDecrementTracked>
      <NumberDecrementReference>1.7647642785984856</NumberDecrementReference>
    </DoubleExamples>
  </XmlNode>
</ArrayOfXmlNode>
```

---

## RAW
Sample Template:

```
This is a sample raw file with token replacement.
At this current timestamp '{{Timestamp:DateTime:UTCISO8601}}' the following sample results were produced:
    stringValue:                                sample static string
    numberValue:                                100
    boolValue:                                  true
    guid:                                       {{Guid:NewGuid}}
    macAddress:                                 {{MacAddress:MacAddress}}
    timestampTimeUnixMs:                        {{Timestamp:DateTime:UnixTimeInMs}}
    timestampTimeIso8601:                       {{Timestamp:DateTime:UTCISO8601}}
    timestampTimeIso8601Tracked:                {{Timestamp:Tracked:UTCISO8601:iso8601TrackedKey}}
    timestampTimeIso8601Reference:              {{Timestamp:Reference:iso8601TrackedKey}}
    timestampTimeUnixMsTracked:                 {{Timestamp:Tracked:UnixTimeInMs:unixMsTrackedKey}}
    timestampTimeUnixMsReference:               {{Timestamp:reference:unixMsTrackedKey}}
    dateTimeRangeIso8601                        {{DateTime:Range:UTCISO8601:Years:1..5}}
    dateTimeRangeUnixMs                         {{DateTime:Range:UnixTimeInMs:Years:1..5}}
    jsonCollection:                             {{JsonCollection:Collection:Sample:sampleLocationNames}}
    jsonCollectionTracked:                      {{JsonCollection:Tracked:Sample:sampleLocationNames:jsonTrackedKey}}
    jsonCollectionReference:                    {{JsonCollection:Reference:jsonTrackedKey}}
    csvCollection:                              {{CsvCollection:Collection:Sample:Model}}
    csvCollectionTracked:                       {{CsvCollection:Tracked:Sample:Model:csvTrackedKey}}
    csvCollectionReferenceSameFieldInRow:       {{CsvCollection:Reference:Model:csvTrackedKey}}
    csvCollectionReferenceAnotherFieldInRow:    {{CsvCollection:Reference:Color:csvTrackedKey}}
    numberWithNoPrecision:                      {{Number:Range:0..100:0}}
    numberWithFivePrecision:                    {{Number:Range:0..100:5}}
    numberIncrementTracked:                     {{Number:IncrementTracked:1.0..2.0:0.1..0.3:100:numberIncTrackedKey}}
    numberIncrementReference:                   {{Number:Reference:numberIncTrackedKey}}
    numberDecrementTracked:                     {{Number:DecrementTracked:20..1:1..3:100:numberDecTrackedKey}}
    numberDecrementReference:                   {{Number:Reference:numberDecTrackedKey}}
```

Output:
```
This is a sample raw file with token replacement.
At this current timestamp '2021-08-04T19:56:15.3526850Z' the following sample results were produced:
    stringValue:                                sample static string
    numberValue:                                100
    boolValue:                                  true
    guid:                                       a501953a-fed2-46fc-bf4a-5fe61072a853
    macAddress:                                 3F:BF:09:3D:3C:CF
    timestampTimeUnixMs:                        1628106975352
    timestampTimeIso8601:                       2021-08-04T19:56:15.3527610Z
    timestampTimeIso8601Tracked:                2021-08-04T19:56:15.3527770Z
    timestampTimeIso8601Reference:              2021-08-04T19:56:15.3527770Z
    timestampTimeUnixMsTracked:                 1628106975352
    timestampTimeUnixMsReference:               1628106975352
    dateTimeRangeIso8601                        2022-08-04T19:56:15.3528480Z
    dateTimeRangeUnixMs                         1722801375352
    jsonCollection:                             North Dakota
    jsonCollectionTracked:                      North Dakota
    jsonCollectionReference:                    North Dakota
    csvCollection:                              Honda
    csvCollectionTracked:                       Toyota
    csvCollectionReferenceSameFieldInRow:       Toyota
    csvCollectionReferenceAnotherFieldInRow:    Red
    numberWithNoPrecision:                      52
    numberWithFivePrecision:                    50.81489
    numberIncrementTracked:                     1.1898660024115193
    numberIncrementReference:                   1.1898660024115193
    numberDecrementTracked:                     19
    numberDecrementReference:                   19
```

