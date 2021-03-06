using System.Linq;
using System.Threading.Tasks;
using Xappium.Android;
using Xunit;

namespace Xappium.Cli.Tests
{
    public class AvdManagerTests
    {
        [Fact]
        public async Task InstallsEmulator()
        {
            var devices = await Emulator.ListEmulators(default);
            if (devices.Any(x => x == AvdManager.DefaultUITestEmulatorName))
            {
                await AvdManager.DeleteEmulator(default);
            }

            devices = await Emulator.ListEmulators(default);

            Assert.DoesNotContain(AvdManager.DefaultUITestEmulatorName, devices);
            await AvdManager.InstallEmulator(29, default);
            devices = await Emulator.ListEmulators(default);

            Assert.Contains(AvdManager.DefaultUITestEmulatorName, devices);
        }
    }
}
