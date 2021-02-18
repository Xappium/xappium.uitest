#!/bin/bash

# nuget restore TestApp.sln

rm -rf UITest

mkdir UITest
cd UITest

UITESTPATH=$(pwd)

echo $UITESTPATH

msbuild ../sample/TestApp.iOS/TestApp.iOS.csproj /p:Platform=iPhoneSimulator /p:Configuration=Release /p:OutputPath=$UITESTPATH/bin/

if [ $? -ne 0 ]
then
    exit 1
fi

msbuild ../sample/TestApp.UITests/TestApp.UITests.csproj /p:OutputPath=$UITESTPATH

if [ $? -ne 0 ]
then
    exit 1
fi

dotnet run --project=../src/Xappium.Client/Xappium.Client.csproj

if [ $? -ne 0 ]
then
    exit 1
fi

dotnet test ../sample/TestApp.UITests/TestApp.UITests.csproj -o=$UITESTPATH --no-build -r=Results --logger trx

ExitCode=$?

exit $ExitCode