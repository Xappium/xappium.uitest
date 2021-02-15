#!/bin/bash

nuget restore TestApp.sln

rm -rf UITest

mkdir UITest
cd UITest

UITESTPATH=$(pwd)

echo $UITESTPATH

# npm install -g appium
# appium &

msbuild ../TestApp.iOS/TestApp.iOS.csproj /p:Platform=iPhoneSimulator /p:Configuration=Release /p:OutputPath=$UITESTPATH/bin/
msbuild ../TestApp.UITests/TestApp.UITests.csproj /p:OutputPath=$UITESTPATH

dotnet run --project=../TestClient/TestClient.csproj

dotnet test ../TestApp.UITests/TestApp.UITests.csproj -o=$UITESTPATH --no-build -r=Results --logger trx

# disown