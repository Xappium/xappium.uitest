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

**NOTE:** Android tools are tested and do work on Mac, however they do not currently work on Windows hosts. If you're an Android Guru and would like to help get this working on Windows hosts I'd love the PR!

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

At first it may be confusing that the configuration has both settings and capabilities. The Capabilities are explicit to the Appium Driver configuration. It is worth noting that if you need an explicit override to the way that Xappium is configuring a particular capability you have the ability to override that here. Note that the adbPort shown above is provided as an example and should not be required for Android.

Settings on the other hand have nothing specifically to do with Xappium.UITest or Appium, but rather are exposed to try to make it easier for you to provide values that you can use for your UI Tests such as user credentials.

### Integrating with your Test Framework

Each test framework is a little different. The guidance shown here is meant as an example of how you might use this with Xunit. It is however not explicitly tied to any specific test framework. Xappium.UITest has borrowed a little inspiration from FluentAssertions in those cases where Xappium.UITest is explicitly making an Assertion that the Page has loaded, or that an element is present. In these cases we are able to locate the Unit Test framework that you are using and properly raise this to your Test Framework.

#### Setting up a test with Xunit

The first thing we'll want to do is ensure that the tests run sequentially. Since we are connecting to a Simulator / Emulator we can only run a single test at a time. To do this we'll create a CollectionDefinition and disable Parallelization. This will help prevent race conditions as well as ensure we get a new Appium Session with each test.

```cs
public class XappiumTests
{
}

[CollectionDefinition(nameof(XappiumTests), DisableParallelization = true)]
public sealed class XappiumTestsCollection : ICollectionFixture<XappiumTests>
{
}
```

We'll set up our tests using the Page-Object-Model. You can get more information on how this works from [Sweeky's talk](https://www.youtube.com/watch?v=4VR861BWkiU) at the Xamarin Developer Summit. To do this we'll use a helper class from Xappium.UITest:

```cs
public class LoginPage : BasePage
{
    protected override string Trait => "LoginButton";
}
```

You'll notice that we provide a trait. This is generally the AutomationId of an element on the page that should be unique that page. Now let's add a test class to validates that our App Starts up and is on the LoginPage.

```cs
[Collection(nameof(XappiumTests))]
public class AppTests : IDisposable
{
    private ITestEngine Engine { get; }

    public AppTests()
    {
        AppManager.StartApp();
        Engine = AppManager.Engine;
    }

    [Fact]
    public void AppLaunches()
    {
        new LoginPage();
    }

    public void Dispose()
    {
        Engine.StopApp();
    }
}
```

Our Test class will be part of the XappiumTests collection which ensures our tests do not run in parallel. It will implement IDisposable so that it can start the app when the constructor is called, and stop the app when it is disposed. This will ensure that each test starts a new session with Appium and screen shots from one test will not overwrite screenshots from another test.

#### Picking a Unit Test Framework

As mentioned earlier, Xappium UITest takes some inspiration from FluentAssertions, as a result Xappium.UITest is compatible with all major Unit Test Frameworks and will help raise the appropriate Assertions with your Test Framework. The Cli Tool uses `dotnet test` under the hood, so we have zero requirements on which Unit Test framework you are required to use as long as it works with dotnet test. The examples here are with Xunit because it's what I use. But it is not a hard requirement.

## Using the Cli Tool

The Cli tool is meant to provide an easy to use runner for your UI Tests. In order to run your UI Tests you only need to supply a couple of parameters:

- (-uitest|--uitest-project-path) The path to the UI Test project
- (-app|--app-project-path) The path to the App project
- (-p|--platform) The Platform Name (iOS/Android) - this is ignored for classic Xamarin apps but required for .NET MAUI apps

It's worth noting that the tools here are designed to provide an experience that is tailored to running on an iOS Simulator or Android Emulator. These settings are not customizable.

```bash
xappiumtest -uitest sample/TestApp.UITests/TestApp.UITests.csproj \
    -app sample/TestApp.iOS/TestApp.iOS.csproj
```

**NOTE:** .NET 6 Single Projects are theoretically compatible... however this is currently untested (there is an open PR to add a .NET 6 project for testing). The Platform specification is part of the planned support for this as we will need to know which platform to build for.

In addition to the required parameters, a couple more optional parameters can also be provided:

- (-c|--configuration) The Build Configuration. This defaults to Release
- (-ui-config|--uitest-configuration) Specifies a file path that will override the base json config in the UI Test directory
- (-artifacts|--artifact-staging-directory) Overrides and specifies the default output directory for all build / test artifacts
- (-show|--show-config) This will write the generated uitest.json to the console
