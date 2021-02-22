using Xunit;
using Xappium.Android;
using System.Linq;

namespace Xappium.Cli.Tests
{
    public class AvdManagerTests
    {
        [Fact]
        public void InstallsEmulator()
        {
            var devices = Emulator.ListEmulators();
            if (devices.Any(x => x == AvdManager.DefaultUITestEmulatorName))
            {
                AvdManager.DeleteEmulator();
            }

            devices = Emulator.ListEmulators();

            Assert.DoesNotContain(AvdManager.DefaultUITestEmulatorName, devices);
            AvdManager.InstallEmulator();
            devices = Emulator.ListEmulators();

            Assert.Contains(AvdManager.DefaultUITestEmulatorName, devices);
        }
    }
}
