using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Xappium
{
    internal static class ProcessHelper
    {
        public static ProcessResult Run(string toolPath, string arguments, bool displayRealtimeOutput = false, IEnumerable<string> inputs = null)
        {
            Console.WriteLine($"{toolPath} {arguments}");

            using var process = new Process
            {
                StartInfo = new ProcessStartInfo(toolPath, arguments)
                {
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardError = true,
                    RedirectStandardInput = true
                }
            };
            process.Start();
            if(inputs != null && inputs.Any())
            {
                foreach(var input in inputs)
                {
                    process.StandardInput.WriteLine(input);
                }
            }

            var output = new List<string>();
            while (!process.StandardOutput.EndOfStream)
            {
                var line = process.StandardOutput.ReadLine();
                output.Add(line);
                if (displayRealtimeOutput)
                    Console.WriteLine(line);
            }

            return new ProcessResult(output, process.StandardError.ReadToEnd());
        }
    }
}
