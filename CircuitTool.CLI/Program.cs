using System;
using System.CommandLine;
using System.Linq;
using System.Threading.Tasks;
using CircuitTool.CLI.Commands;
using CircuitTool.CLI.UI;
using Spectre.Console;

namespace CircuitTool.CLI
{
    /// <summary>
    /// Main entry point for the CircuitTool CLI application
    /// </summary>
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            // Display welcome banner
            ConsoleUI.DisplayWelcomeBanner();

            // Create root command
            var rootCommand = new RootCommand("CircuitTool Interactive CLI - Test the CircuitTool framework interactively")
            {
                BasicCalculationsCommand.Create(),
                ACAnalysisCommand.Create(),
                ComponentDesignCommand.Create(),
                PowerAnalysisCommand.Create(),
                InteractiveCommand.Create(),
                BenchmarkCommand.Create(),
                ExamplesCommand.Create()
            };

            // Add global options
            var verboseOption = new Option<bool>(
                new[] { "--verbose", "-v" },
                "Enable verbose output");
            
            var outputFormatOption = new Option<string>(
                new[] { "--format", "-f" },
                getDefaultValue: () => "table",
                "Output format (table, json, csv)")
            {
                IsRequired = false
            };
            outputFormatOption.AddValidator(result =>
            {
                var value = result.GetValueForOption(outputFormatOption);
                if (value != null && !new[] { "table", "json", "csv" }.Contains(value))
                {
                    result.ErrorMessage = "Format must be one of: table, json, csv";
                }
            });

            rootCommand.AddGlobalOption(verboseOption);
            rootCommand.AddGlobalOption(outputFormatOption);

            // Configure exception handling
            rootCommand.SetHandler(async (verbose, format) =>
            {
                if (args.Length == 0)
                {
                    // No arguments - show interactive menu
                    await InteractiveMenu.RunAsync();
                }
            }, verboseOption, outputFormatOption);

            try
            {
                return await rootCommand.InvokeAsync(args);
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
                return 1;
            }
        }
    }
}
