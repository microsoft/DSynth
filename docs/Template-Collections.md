# Template Collections
Template Collections provide the functionality to define values in collection files that can be referenced by [Template Tokens](./Template-Tokens.md). Template collection files reside under `Profiles/Templates/Collections` and need to have `.collections.json` or `.collections.csv` appended onto the file name for DSynth to load collections. I.e., `Sample.collections.json`

## Json Collections
Assume the following collections defined in a json collection file named `Sample.collections.json` with the following content
```json
{
  "collections": {
    "sampleLocationNames": [
      "Washington DC",
      "North Dakota",
      "Washington State"
    ]
  }
}
```
The following template token would get a random value from the sampleLocationNames array: `{{JsonCollection:Collection:Sample:sampleLocationNames}}`

**Structure:**

The following structure is required for any user defined collection. The collection needs to be saved under the `./Profiles/Templates/Collections` folder with `.collections.json` in the name, i.e., `Sample.collections.json` as this is how DSynth identifies collections to load during startup.

```json
{
  "collections": {
    "collectionName1": [],
    "collectionName2": [],
    ...
    ...
    ...
  }
}
```

## Csv Collections
Assume the following defined in a csv collection file named `Sample.collections.csv` with the following content
```csv
"Model","Color"
"Ford","Blue"
"Honda","Green"
"Toyota","Red"
```
The following template token would get a random row with the column value of Model: `{{CsvCollection:Collection:Sample:Model}}`