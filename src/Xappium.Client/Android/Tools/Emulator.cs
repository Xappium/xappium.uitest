using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Xappium.Client.Extensions;
using static Xappium.Client.Android.AndroidTool;

namespace Xappium.Client.Android
{
    internal static class Emulator
    {
        public static readonly string ToolPath = LocateUtility("emulator");

        public static void StartEmulator()
        {
            var emulators = ListEmulators();
            StartEmulator(emulators.FirstOrDefault());
        }

        public static void StartEmulator(string emulatorName)
        {
            ValidateStartEmulator(emulatorName);

            // -no-window
#pragma warning disable CS4014 // Launch Emulator without waiting
            using var _ = new Process
            {
                StartInfo = new ProcessStartInfo(ToolPath, $"-avd {emulatorName} -no-snapshot -wipe-data")
                {
                    CreateNoWindow = true,
                    RedirectStandardOutput = true
                }
            };
            _.Start();
#pragma warning restore CS4014 // Launch Emulator without waiting

            // Wait for boot
            WaitForBoot();

            Console.WriteLine("Giving the device 20 seconds to finish getting ready.");
            Thread.Sleep(TimeSpan.FromSeconds(20));
        }

        private static void WaitForBoot()
        {
            var i = 0;
            while (true)
            {
                var result = ProcessHelper.Run(Adb.ToolPath, "wait-for-device shell getprop sys.boot_completed");

                if (result.IsErred)
                    throw new Exception(result.Error);

                if(result.Output.Any(x => x.Trim() == "1"))
                {
                    // Wait a few seconds to ensure the OS has fully loaded
                    Console.WriteLine("Emulator Started... waiting 10 seconds for full boot");
                    Thread.Sleep(TimeSpan.FromSeconds(10));
                    return;
                }

                if (i++ > 180)
                {
                    throw new TimeoutException($"The Emulator does not appear to have started after 3 minutes");
                }

                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
        }

        //private static void EnsureDeviceIsReady()
        //{
        //    var i = 0;
        //    while (true)
        //    {
        //        var result = ProcessHelper.Run(Adb.ToolPath, "wait-for-device shell input keyevent 82");

        //        if (result.IsErred)
        //            throw new Exception(result.Error);

        //        if (result.Output.Any(x => !string.IsNullOrEmpty(x)))
        //            return;

        //        if (i++ > 60)
        //            throw new TimeoutException("The Emulator is not ready after waiting 60 seconds from the boot completion.");

        //        Thread.Sleep(TimeSpan.FromSeconds(1));
        //    }
        //}

        private static void ValidateStartEmulator(string emulatorName)
        {
            ThrowIfNull(ToolPath, nameof(Emulator));
            ThrowIfNull(Adb.ToolPath, nameof(Adb));

            if (string.IsNullOrEmpty(emulatorName))
            {
                throw new Exception($"No Installed Emulator could be found");
            }

            var emulators = ListEmulators();
            if (!emulators.Any(x => x == emulatorName))
            {
                throw new Exception($"No such emulator exists with the name: '{emulatorName}'");
            }
        }

        public static bool HasInstalledEmulator()
        {
            ThrowIfNull(ToolPath, nameof(Emulator));

            try
            {
                var emulators = ListEmulators();
                return emulators != null && emulators.Any();
            }
            catch
            {
                return false;
            }
        }

        public static IEnumerable<string> ListEmulators()
        {
            ThrowIfNull(ToolPath, nameof(Emulator));
            var result = ProcessHelper.Run(ToolPath, "-list-avds");
            if (result.IsErred)
                throw new Exception("Error listing emulators");

            return result.Output.Where(x => !string.IsNullOrEmpty(x));
        }
    }
}
