# ---------------------------------------------------------------------------------------------
#  Copyright (c) Microsoft Corporation. All rights reserved.
#  Licensed under the MIT License. See License.txt in the project root for license information.
# ---------------------------------------------------------------------------------------------
SHELL=/usr/bin/env bash

PROJECT_PATHS := 'src/DSynth' \
	'src/DSynth.Common' \
	'src/DSynth.Engine' \
	'src/DSynth.Engine.Tests' \
	'src/DSynth.Provider' \
	'src/DSynth.Reporter' \
	'src/DSynth.Sink' \
	'.'

.PHONY: clean
clean:
	$(call log, "Cleaning up /obj /bin and /release folders")

	@$(foreach project_path,$(PROJECT_PATHS), \
		rm -rf $(project_path)/obj \
		rm -rf $(project_path)/bin \
		rm -rf $(project_path)/release)

.PHONY: build
build: clean
build:
	$(call log, "Building for runtime osx-x64")
	@pushd src/DSynth && dotnet build --runtime osx-x64 && popd
	
	$(call log, "Building for runtime linux-x64")
	@pushd src/DSynth && dotnet build --runtime linux-x64 && popd
	
	$(call log, "Building for runtime win-x64")
	@pushd src/DSynth && dotnet build --runtime win-x64 && popd

	$(call log, "Building for runtime linux-arm64")
	@pushd src/DSynth && dotnet build --runtime linux-arm64 && popd

.PHONY: test
test:
	$(call log, "Running DSynth.Engine.Tests")
	@pushd src/DSynth.Engine.Tests && dotnet test && popd
	
	$(call log, "Running DSynth.Provider.Tests")
	@pushd src/DSynth.Provider.Tests && dotnet test && popd

.PHONY: publish
publish: clean
publish:
	$(call log, "Publishing Release for runtime osx-x64")
	@dotnet publish ./src/DSynth/DSynth.csproj --configuration Release --runtime osx-x64 --output ./release/osx-x64

	$(call log, "Publishing Release for runtime linux-x64")
	@dotnet publish ./src/DSynth/DSynth.csproj --configuration Release --runtime linux-x64 --output ./release/linux-x64

	$(call log, "Publishing Release for runtime win-x64")
	@dotnet publish ./src/DSynth/DSynth.csproj --configuration Release --runtime win-x64 --output ./release/win-x64

	$(call log, "Publishing Release for runtime linux-arm64")
	@dotnet publish ./src/DSynth/DSynth.csproj --configuration Release --runtime linux-arm64 --output ./release/linux-arm64

define log
	@echo "[`date +%T`]:$(1)"
endef