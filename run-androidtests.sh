#!/bin/bash

# nuget restore TestApp.sln

rm -rf UITest

mkdir UITest
cd UITest

UITESTPATH=$(pwd)

# npm install -g appium
# appium &

echo $UITESTPATH

msbuild ../TestApp.Android/TestApp.Android.csproj /p:Configuration=Release /p:AndroidPackageFormat=apk /p:AndroidSupportedAbis=x86 /p:OutputPath=$UITESTPATH/bin/ /t:SignAndroidPackage

if [ $? -ne 0 ]
then
    exit 1
fi

msbuild ../TestApp.UITests/TestApp.UITests.csproj /p:OutputPath=$UITESTPATH

if [ $? -ne 0 ]
then
    exit 1
fi

dotnet run --project=../Xappium.Client/Xappium.Client.csproj

if [ $? -ne 0 ]
then
    exit 1
fi

dotnet test ../TestApp.UITests/TestApp.UITests.csproj -o=$UITESTPATH --no-build -r=Results --logger trx

ExitCode=$?

# AppiumPID=$(ps -A | grep appium | awk '{print $1}')
# echo 'Appium PID: $AppiumPID'
# kill $AppiumPID

exit $ExitCode