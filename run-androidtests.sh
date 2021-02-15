#!/bin/bash

nuget restore TestApp.sln

rm -rf UITest

mkdir UITest
cd UITest

UITESTPATH=$(pwd)

echo $UITESTPATH

msbuild ../TestApp.Android/TestApp.Android.csproj /p:Configuration=Release /p:AndroidPackageFormat=apk /p:OutputPath=$UITESTPATH/bin/ /t:SignAndroidPackage
msbuild ../TestApp.UITests/TestApp.UITests.csproj /p:OutputPath=$UITESTPATH

dotnet run --project=../TestClient/TestClient.csproj

dotnet test ../TestApp.UITests/TestApp.UITests.csproj -o=$UITESTPATH --no-build -r=Results --logger trx