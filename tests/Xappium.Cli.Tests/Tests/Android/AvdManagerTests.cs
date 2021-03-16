using System.Threading.Tasks;
using Xappium.Android;
using Xappium.Cli.Tests.Tools;
using Xunit;

namespace Xappium.Cli.Tests.Android
{
    [Collection(nameof(Tool))]
    public class AvdManagerTests
    {
        public AvdManagerTests()
        {
            TestEnvironmentHost.Init();
        }

        [Theory]
        [InlineData(29)]
        [InlineData(30)]
        public async Task InstallsEmulator(int sdkVersion)
        {
            // We need to ensure the sdk is installed...
            await SdkManager.EnsureSdkIsInstalled(sdkVersion, default);

            var devices = await Emulator.ListEmulators(default);
            var emulatorName = $"{nameof(AvdManagerTests)}_{nameof(InstallsEmulator)}{sdkVersion}";
            foreach (var device in devices)
            {
                if (device.StartsWith(AvdManager.DefaultUITestEmulatorName) || device == emulatorName)
                    await AvdManager.DeleteEmulator(device, default);
            }

            devices = await Emulator.ListEmulators(default);

            Assert.DoesNotContain(AvdManager.DefaultUITestEmulatorName, devices);
            await AvdManager.InstallEmulator(emulatorName, sdkVersion, default);
            devices = await Emulator.ListEmulators(default);

            Assert.Contains(emulatorName, devices);
        }
    }
}
