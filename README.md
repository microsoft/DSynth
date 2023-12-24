# DSynth
DSynth is a template driven data generator. DSynth accepts templates of various formats, generates templated data and outputs the generated data into a configured sink[s]. DSynth also exposes an endpoint for each configured provider to allow for services to pull generated data when push is not supported. DSynth is very flexible and can easily be extended to support other template formats and sinks. Comes complete with API which allows control of DSynth, including the importing and exporting of profiles. DSynth also comes with out of the box support with Application Insights which can be paired with Grafana to provide rich visual telemetry.

## Supported Sinks:
```
- Azure Blob
- Azure Cosmos DB
- Azure Custom Logs
- Azure Event Hubs
- Azure IoT Hub
- Azure Log Analytics
- Azure Service Bus
- Console
- File
- Http
- SocketServer
```

## Supported Template Formats:
```
- CSV
- Images
  - Jpeg
  - Bmp
  - Tiff
  - Gif
  - Png
- JSON
- JSONL
- RAW
- XML
```

# To Run
The project is "mostly" setup to run out of the box and requires prerequisites to support the image generation feature of DSynth (for Mac and Linux). After starting DSynth, you will see payloads in the format of CSV, JSON, XML and RAW written to console. Images are turned off by default, but can easily be enabled by setting `"isPushEnabled": true,` in `sample-providers.json` for the `ImageTemplateSample` provider.

### Mac Prerequisites:
```
brew install mono-libgdiplus
```
### Ubuntu Prerequisites:
```
sudo apt-get install libgdiplus
```

### Build and Run DSynth
```
-- Run the following command appropriate for your environment --

(OSX)
cd DSynth
dotnet publish ./src/DSynth/DSynth.csproj --configuration Release --runtime osx-x64 --output ./release/osx-x64
cd release/osx-64
./DSynth

(Linux)
cd DSynth
dotnet publish ./src/DSynth/DSynth.csproj --configuration Release --runtime linux-x64 --output ./release/linux-x64
cd release/linux-64
./DSynth

(Windows)
cd DSynth
dotnet publish ./src/DSynth/DSynth.csproj --configuration Release --runtime win-x64 --output ./release/win-x64
cd release/win-64
./DSynth.exe

(Linux-ARM64)
cd DSynth
dotnet publish ./src/DSynth/DSynth.csproj --configuration Release --runtime linux-arm64 --output ./release/linux-arm64
cd release/linux-arm64
./DSynth
```

### Additional Startup Arguments
|Switch|Description|
|--|--|
|--headless|Starts DSynth in headless mode without the API|


# Documentation Sections
The steps above will get DSynth running with default sample values. To explore further and create your own profiles and templates, please follow the links below.

1. [Template Tokens](./docs/Template-Tokens.md)
2. [Template Formats](./docs/Template-Formats.md)
3. [Template Collections](./docs/Template-Collections.md)
4. [Provider Configurations](./docs/Provider-Configuration.md)
5. [Sink Configurations](./docs/Sinks.md)
6. [API Endpoints](./docs/API-Endpoints.md)
