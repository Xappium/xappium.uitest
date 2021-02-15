using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TestClient
{
    public static class ProcessHelper
    {
        public static ProcessResult Run(string toolPath, string arguments)
        {
            Console.WriteLine($"{toolPath} {arguments}");

            using var process = new Process
            {
                StartInfo = new ProcessStartInfo(toolPath, arguments)
                {
                    CreateNoWindow = true,
                    RedirectStandardOutput = true
                }
            };
            process.Start();

            var output = new List<string>();
            while (!process.StandardOutput.EndOfStream)
                output.Add(process.StandardOutput.ReadLine());

            return new ProcessResult(output, process.StandardError.ReadToEnd());
        }
    }
}
