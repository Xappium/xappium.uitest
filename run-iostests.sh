#!/bin/bash

dotnet run --project src/Xappium.Cli/Xappium.Cli.csproj -- -uitest sample/TestApp.UITests/TestApp.UITests.csproj -app sample/TestApp.iOS/TestApp.iOS.csproj