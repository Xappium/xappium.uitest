using System;
using System.Collections.Generic;
using System.Text;
using McMaster.Extensions.CommandLineUtils;

namespace Xappium.Commands
{
    [Command(
           Name = "xappium",
           FullName = "Xappium UITest CLI",
           Description = "Project Home: https://github.com/xappium/xappium.uitest")]
    [Subcommand(typeof(PrepareCommand))]
    [Subcommand(typeof(TestCommand))]
    public class XappiumCommand
    {
        private CommandLineApplication _app { get; }
        private IConsole _console { get; }

        public XappiumCommand(CommandLineApplication app, IConsole console)
        {
            _app = app;
            _console = console;
        }

        private void OnExecute() => _console.WriteLine(_app.GetHelpText());
    }
}
