using System;
using System.IO;

namespace Xappium.Logging
{
    internal static class Logger
    {
        private const string defaultLog = "xappium.log";

        public static LogLevel Level { get; set; }

        private static string logDirectory;

        public static string DefaultLogPath => Path.Combine(logDirectory, defaultLog);

        public static bool HasErrors { get; private set; }

        public static void SetWorkingDirectory(string baseWorkingDirectory)
        {
            logDirectory = Path.Combine(baseWorkingDirectory, "logs");
            Directory.CreateDirectory(logDirectory);
        }

        public static void WriteLine(string message, LogLevel level, string logfile = defaultLog)
        {
            if (level <= Level)
                Console.WriteLine(message);

            File.AppendAllLines(Path.Combine(logDirectory, logfile), new[] { message });
        }

        public static void WriteWarning(string message, string logfile = defaultLog)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ResetColor();

            File.AppendAllLines(Path.Combine(logDirectory, logfile), new[] { message });
        }

        public static void WriteError(Exception ex) =>
            WriteError(ex.ToString());

        public static void WriteError(string message)
        {
            HasErrors = true;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();

            File.AppendAllLines(Path.Combine(logDirectory, "xappium-error.log"), new[] { message });
        }
    }
}
