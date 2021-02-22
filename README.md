# Xappium.UITest

Writing Cross Platform (X-Plat) apps can be a challenge. Finding a way to more easily write and run integrated UI Tests can be even more challenging. Xappium.UITest aims to help solve some of these issues by building an additional layer of abstraction over Appium that makes it easier to write X-Plat UI Tests. This project is a combination of the work done independently by [@dansiegel](https://github.com/sponsors/dansiegel) & [@redth](https://github.com/redth) taking the best pieces of both.

This is meant to make it easier for Developers to write and integrate UI Tests with their mobile apps.

## Current State

This repo is currently evolving and may experience breaking API changes. Community feedback and contributions are welcome and encouraged. We hope that this project will serve both as a model and backbone for UI Testing for .NET Developers with classic Xamarin / Xamarin.Forms applications as well as newer .NET 6 / .NET MAUI applications.

### Roadmap

| Description | Status |
|-------------|:------:|
| iOS Support | Working locally |
| Android Support | In process |
| WinUI Support | Community Welcome |
| GTK Support | Community Welcome |
| macOS Support | Community Welcome |
| Tizen Support | Community Welcome |
| CLI tool | \*In process |

\* NOTE: The repo currently utilizes a combination of scripts to build and test. It is the aim of the Xappium.Client to replace this with a single CLI tool to run tests on a single Simulator / Emulator as part of a CI Build. An additional CLI Tool will be developed for Enterprise Support for self hosted Device Clouds.

## Configuration

The UI Test AppManager is configured through a combination of local, embedded, and environment. This gives you a wide variety of configuration options and ways to customize your build. For example you may provide a `uitest.json` that contains common attributes for all builds... this will be automatically copied to the UI Test output directory. You may also conditionally embed a `uitest.ios.json` or `uitest.android.json` with the logical name `uitest.json`. Finally you may use various Environment variables to set the configuration dynamically in a CI build. The order of precedence is as follows:

- Local uitest.json on disk
- Embedded uitest.json in the UITest project assembly
- Environment Variables

Any of these properties can be set by you, however the Xappium.Client will automatically help you build out a completed config.

```json
{
  "platform": "Android",
  "appPath": "/Users/dansiegel/repos/Xappium.UITest/UITest/bin/com.avantipoint.testapp-Signed.apk",
  "appId": "com.avantipoint.testapp",
  "deviceName": "sdk_gphone_x86",
  "udid": "emulator-5554",
  "osVersion": "29",
  "settings": {
    "name": "Dan"
  },
  "capabilities": {
    "appWaitPackage": "com.avantipoint.testapp"
  }
}
```

### Settings vs Capabilities

At first it may be confusing that the configuration has both settings and capabilities. The Capabilities are explicit to the Appium Driver configuration. It is worth noting that if you need an explicit override to the way that Xappium is configuring a particular capability you have the ability to override that here.

Settings on the other hand have nothing specifically to do with Xappium.UITest or Appium, but rather are exposed to try to make it easier for you to provide values that you can use for your UI Tests such as user credentials.