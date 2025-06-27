using System;
using System.CommandLine;
using System.Threading.Tasks;
using CircuitTool.CLI.UI;
using Spectre.Console;

namespace CircuitTool.CLI.Commands
{
    /// <summary>
    /// Interactive mode command
    /// </summary>
    internal static class InteractiveCommand
    {
        public static Command Create()
        {
            var command = new Command("interactive", "Enter interactive mode with guided menus")
            {
                IsHidden = false
            };

            command.SetHandler(async () =>
            {
                await InteractiveMenu.RunAsync();
            });

            return command;
        }
    }
}
