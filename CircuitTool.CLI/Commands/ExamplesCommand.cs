using System;
using System.CommandLine;
using System.Threading.Tasks;
using CircuitTool.CLI.UI;
using Spectre.Console;

namespace CircuitTool.CLI.Commands
{
    /// <summary>
    /// Examples command to run built-in CircuitTool examples
    /// </summary>
    internal static class ExamplesCommand
    {
        public static Command Create()
        {
            var command = new Command("examples", "Run built-in CircuitTool examples and tutorials");

            var exampleOption = new Option<string>("--example", getDefaultValue: () => "all", 
                "Specific example to run (all, basic, circuit, ac, energy)");

            command.AddOption(exampleOption);

            command.SetHandler(async (example) =>
            {
                await RunExamples(example);
            }, exampleOption);

            return command;
        }

        private static async Task RunExamples(string example)
        {
            try
            {
                var rule = new Rule("[bold blue]CircuitTool Examples[/]")
                {
                    Style = Style.Parse("blue")
                };
                AnsiConsole.Write(rule);
                AnsiConsole.WriteLine();

                switch (example.ToLower())
                {
                    case "all":
                        await RunAllExamples();
                        break;
                    case "basic":
                        await RunBasicExample();
                        break;
                    case "circuit":
                        await RunCircuitExample();
                        break;
                    case "ac":
                        await RunACExample();
                        break;
                    case "energy":
                        await RunEnergyExample();
                        break;
                    default:
                        ConsoleUI.DisplayError($"Unknown example: {example}. Available: all, basic, circuit, ac, energy");
                        return;
                }

                ConsoleUI.DisplaySuccess("Examples completed successfully!");
            }
            catch (Exception ex)
            {
                ConsoleUI.DisplayError("Error running examples", ex);
            }
        }

        private static async Task RunAllExamples()
        {
            await ConsoleUI.WithProgress("Running all examples", async (task) =>
            {
                task.MaxValue = 4;
                
                await RunBasicExample();
                task.Increment(1);
                
                await RunCircuitExample();
                task.Increment(1);
                
                await RunACExample();
                task.Increment(1);
                
                await RunEnergyExample();
                task.Increment(1);
            });
        }

        private static async Task RunBasicExample()
        {
            var panel = new Panel(
                new Markup("[bold]Basic Ohm's Law Example[/]\n\n" +
                          "Calculating electrical parameters for a simple circuit"))
            {
                Header = new PanelHeader("🔍 Basic Calculations"),
                Border = BoxBorder.Rounded,
                BorderStyle = Style.Parse("green")
            };
            AnsiConsole.Write(panel);

            // Run the actual CircuitTool example
            AnsiConsole.WriteLine("Running built-in basic example...");
            CircuitTool.DocumentationExamples.BasicOhmsLaw.RunExample();
            
            AnsiConsole.WriteLine();
            await Task.Delay(1000);
        }

        private static async Task RunCircuitExample()
        {
            var panel = new Panel(
                new Markup("[bold]Circuit Building Example[/]\n\n" +
                          "Demonstrating circuit analysis and component calculations"))
            {
                Header = new PanelHeader("🔧 Circuit Building"),
                Border = BoxBorder.Rounded,
                BorderStyle = Style.Parse("yellow")
            };
            AnsiConsole.Write(panel);

            AnsiConsole.WriteLine("Running built-in circuit building example...");
            CircuitTool.DocumentationExamples.CircuitBuilding.RunExample();
            
            AnsiConsole.WriteLine();
            await Task.Delay(1000);
        }

        private static async Task RunACExample()
        {
            var panel = new Panel(
                new Markup("[bold]AC Circuit Analysis Example[/]\n\n" +
                          "AC circuit calculations including impedance and reactance"))
            {
                Header = new PanelHeader("⚡ AC Analysis"),
                Border = BoxBorder.Rounded,
                BorderStyle = Style.Parse("blue")
            };
            AnsiConsole.Write(panel);

            AnsiConsole.WriteLine("Running built-in AC analysis example...");
            CircuitTool.DocumentationExamples.ACAnalysis.RunExample();
            
            AnsiConsole.WriteLine();
            await Task.Delay(1000);
        }

        private static async Task RunEnergyExample()
        {
            var panel = new Panel(
                new Markup("[bold]Energy Calculations Example[/]\n\n" +
                          "Power consumption and energy efficiency analysis"))
            {
                Header = new PanelHeader("🔋 Energy Analysis"),
                Border = BoxBorder.Rounded,
                BorderStyle = Style.Parse("red")
            };
            AnsiConsole.Write(panel);

            AnsiConsole.WriteLine("Running built-in energy calculations example...");
            CircuitTool.DocumentationExamples.EnergyCalculations.RunExample();
            
            AnsiConsole.WriteLine();
            await Task.Delay(1000);
        }
    }
}
