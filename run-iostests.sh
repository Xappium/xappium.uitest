#!/bin/bash

# nuget restore TestApp.sln

rm -rf UITest

mkdir UITest
cd UITest

UITESTPATH=$(pwd)

echo $UITESTPATH

npm install -g appium
appium &

msbuild ../TestApp.iOS/TestApp.iOS.csproj /p:Platform=iPhoneSimulator /p:Configuration=Release /p:OutputPath=$UITESTPATH/bin/

if [ $? -ne 0 ]
then
    exit 1
fi

msbuild ../TestApp.UITests/TestApp.UITests.csproj /p:OutputPath=$UITESTPATH

if [ $? -ne 0 ]
then
    exit 1
fi

dotnet run --project=../TestClient/TestClient.csproj

if [ $? -ne 0 ]
then
    exit 1
fi

dotnet test ../TestApp.UITests/TestApp.UITests.csproj -o=$UITESTPATH --no-build -r=Results --logger trx

ExitCode=$?

AppiumPID=$(ps -A | grep appium | awk '{print $1}')
echo 'Appium PID: $AppiumPID'
kill $AppiumPID

exit $ExitCode