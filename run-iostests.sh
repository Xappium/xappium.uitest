#!/bin/bash

dotnet run --project src/Xappium.Cli/Xappium.Cli.csproj -c Release -- -uitest sample/TestApp.UITests/TestApp.UITests.csproj -app sample/TestApp.iOS/TestApp.iOS.csproj --logger normal
