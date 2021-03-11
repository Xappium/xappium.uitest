# Xappium.UITest

Writing Cross Platform (X-Plat) apps can be a challenge. Finding a way to more easily write and run integrated UI Tests can be even more challenging. Xappium.UITest aims to help solve some of these issues by building an additional layer of abstraction over Appium that makes it easier to write X-Plat UI Tests. This project is a combination of the work done independently by [@dansiegel](https://github.com/sponsors/dansiegel) & [@redth](https://github.com/redth) taking the best pieces of both.

This is meant to make it easier for Developers to write and integrate UI Tests with their mobile apps.

## Current State

This repo is currently evolving and may experience breaking API changes. Community feedback and contributions are welcome and encouraged. We hope that this project will serve both as a model and backbone for UI Testing for .NET Developers with classic Xamarin / Xamarin.Forms applications as well as newer .NET 6 / .NET MAUI applications.

### Roadmap

| Description | Status |
|-------------|:------:|
| iOS Support | Done |
| Android Support | In process |
| WinUI Support | PR Welcome |
| GTK Support | PR Welcome |
| macOS Support | PR Welcome |
| Tizen Support | PR Welcome |
| CLI tool | Done |
| Azure Pipelines | Needs Help |

**NOTE** Android tools are tested and do work on Mac, however they do not currently work on Windows hosts. If you're an Android Guru and would like to help get this working on Windows hosts I'd love the PR!

## Configuration

The UI Test AppManager is configured through a combination of local, embedded, and environment. This gives you a wide variety of configuration options and ways to customize your build. For example you may provide a `uitest.json` that contains common attributes for all builds... this will be automatically copied to the UI Test output directory. You may also conditionally embed a `uitest.ios.json` or `uitest.android.json` with the logical name `uitest.json`. Finally you may use various Environment variables to set the configuration dynamically in a CI build. The order of precedence is as follows:

- Local uitest.json on disk (this can be overridden in the CLI)
- Embedded uitest.json in the UITest project assembly
- Environment Variables

Any of these properties can be set by you, however the Xappium.Cli will automatically help you build out a completed config.

```json
{
  "platform": "Android",
  "appPath": "/<Project Path>/UITest/bin/com.avantipoint.testapp-Signed.apk",
  "appId": "com.avantipoint.testapp",
  "deviceName": "sdk_gphone_x86",
  "udid": "emulator-5554",
  "osVersion": "29",
  "settings": {
    "name": "Dan"
  },
  "capabilities": {
    "adbPort": "5037"
  }
}
```

### Settings vs Capabilities

At first it may be confusing that the configuration has both settings and capabilities. The Capabilities are explicit to the Appium Driver configuration. It is worth noting that if you need an explicit override to the way that Xappium is configuring a particular capability you have the ability to override that here. Note that the adbPort shown above is provided as an example and is not a required for Android.

Settings on the other hand have nothing specifically to do with Xappium.UITest or Appium, but rather are exposed to try to make it easier for you to provide values that you can use for your UI Tests such as user credentials.

### Integrating with your Test Framework

Each test framework is a little different. The guidance shown here is meant as an example of how you might use this with Xunit. It is however not explicitly tied to any specific test framework. First we'll create a fixture that we can inject into our Test class. This will manage starting the app when it is created and stopping the app when we're done and the fixture is disposed by Xunit.

```cs
public sealed class AppFixture : IDisposable
{
    public AppFixture()
    {
        AppManager.StartApp();
        Engine = AppManager.Engine;
    }

    public ITestEngine Engine { get; }

    public void Dispose()
    {
        Engine.StopApp();
    }
}
```

Next we'll create a CollectionDefinition. This is used by Xunit to help us share our fixture between tests.

```cs
[CollectionDefinition(nameof(AppFixture))]
public sealed class AppFixtureCollection : ICollectionFixture<AppFixture>
{
}
```

Finally we'll create a test class and write some tests. In this case we'll assume that we have a LoginPage as our initial launch window in the app. For more information on writing UI Tests with the Page-Object-Model see [Sweeky's talk](https://www.youtube.com/watch?v=4VR861BWkiU) from XamDevSummit.

```cs
public class AppTests : IClassFixture<AppFixture>
{
    private ITestEngine Engine { get; }

    public AppTests(AppFixture fixture)
    {
        Engine = fixture.Engine;
    }

    [Fact]
    public void AppLaunches()
    {
        new LoginPage();
    }
}
```

## Using the Cli Tool

The Cli tool is meant to provide an easy to use runner for your UI Tests. In order to run your UI Tests you only need to supply a couple of parameters:

- (-uitest|--uitest-project-path) The path to the UI Test project
- (-app|--app-project-path) The path to the App project
- (-p|--platform) The Platform Name (iOS/Android) - this is ignored for classic Xamarin apps but required for .NET MAUI apps

It's worth noting that the tools here are designed to provide an experience that is tailored to running on an iOS Simulator or Android Emulator. These settings are not customizable.

```bash
xappiumtest -uitest sample/TestApp.UITests/TestApp.UITests.csproj -app sample/TestApp.iOS/TestApp.iOS.csproj
```

**NOTE** While support for .NET 6 Single Projects is planned it is not currently supported. The Platform specification is part of the planned support for this as we will need to know which platform to build for.

In addition to the required parameters, a couple more optional parameters can also be provided:

- (-c|--configuration) The Build Configuration. This defaults to Release
- (-ui-config|--uitest-configuration) Specifies a file path that will override the base json config in the UI Test directory
- (-artifacts|--artifact-staging-directory) Overrides and specifies the default output directory for all build / test artifacts
- (-show|--show-config) This will write the generated uitest.json to the console