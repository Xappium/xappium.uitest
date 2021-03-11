using System.Threading.Tasks;
using Xappium.Android;
using Xunit;

namespace Xappium.Cli.Tests.Android
{
    public class AvdManagerTests
    {
        public AvdManagerTests()
        {
            XUnitLogger.Init();
        }

        [Fact]
        public async Task InstallsEmulator()
        {
            var devices = await Emulator.ListEmulators(default);
            var emulatorName = $"{nameof(AvdManagerTests)}_{nameof(InstallsEmulator)}";
            foreach (var device in devices)
            {
                if (device.StartsWith(AvdManager.DefaultUITestEmulatorName) || device == emulatorName)
                    await AvdManager.DeleteEmulator(device, default);
            }

            devices = await Emulator.ListEmulators(default);

            Assert.DoesNotContain(AvdManager.DefaultUITestEmulatorName, devices);
            await AvdManager.InstallEmulator(emulatorName, 29, default);
            devices = await Emulator.ListEmulators(default);

            Assert.Contains(emulatorName, devices);
        }
    }
}
