using System;
using System.CommandLine;
using System.Threading.Tasks;
using CircuitTool.CLI.UI;
using Spectre.Console;

namespace CircuitTool.CLI.Commands
{
    /// <summary>
    /// Basic electrical calculations command
    /// </summary>
    internal static class BasicCalculationsCommand
    {
        public static Command Create()
        {
            var command = new Command("basic", "Basic electrical calculations (Ohm's Law, Power)")
            {
                CreateOhmsLawCommand(),
                CreatePowerCommand(),
                CreateResistanceCommand()
            };

            return command;
        }

        private static Command CreateOhmsLawCommand()
        {
            var command = new Command("ohms", "Ohm's Law calculations");

            var voltageOption = new Option<double?>("--voltage", "Voltage in volts");
            var currentOption = new Option<double?>("--current", "Current in amperes");
            var resistanceOption = new Option<double?>("--resistance", "Resistance in ohms");

            command.AddOption(voltageOption);
            command.AddOption(currentOption);
            command.AddOption(resistanceOption);

            command.SetHandler(async (voltage, current, resistance) =>
            {
                var providedCount = (voltage.HasValue ? 1 : 0) + 
                                   (current.HasValue ? 1 : 0) + 
                                   (resistance.HasValue ? 1 : 0);

                if (providedCount != 2)
                {
                    ConsoleUI.DisplayError("Please provide exactly 2 of the 3 values (voltage, current, resistance)");
                    return;
                }

                var table = ConsoleUI.CreateResultsTable("Ohm's Law Calculation", "Parameter", "Value", "Unit");

                if (!voltage.HasValue)
                {
                    var result = CircuitTool.OhmsLawCalculator.Voltage(current!.Value, resistance!.Value);
                    table.AddRow("Current", current.Value.ToString("F3"), "A");
                    table.AddRow("Resistance", resistance.Value.ToString("F3"), "Ω");
                    table.AddRow("[bold green]Voltage[/]", result.ToString("F3"), "V");
                }
                else if (!current.HasValue)
                {
                    var result = CircuitTool.OhmsLawCalculator.Current(voltage!.Value, resistance!.Value);
                    table.AddRow("Voltage", voltage.Value.ToString("F3"), "V");
                    table.AddRow("Resistance", resistance.Value.ToString("F3"), "Ω");
                    table.AddRow("[bold green]Current[/]", result.ToString("F3"), "A");
                }
                else // !resistance.HasValue
                {
                    var result = CircuitTool.OhmsLawCalculator.Resistance(voltage!.Value, current!.Value);
                    table.AddRow("Voltage", voltage.Value.ToString("F3"), "V");
                    table.AddRow("Current", current.Value.ToString("F3"), "A");
                    table.AddRow("[bold green]Resistance[/]", result.ToString("F3"), "Ω");
                }

                AnsiConsole.Write(table);

            }, voltageOption, currentOption, resistanceOption);

            return command;
        }

        private static Command CreatePowerCommand()
        {
            var command = new Command("power", "Power calculations");

            var voltageOption = new Option<double>("--voltage", "Voltage in volts") { IsRequired = true };
            var currentOption = new Option<double>("--current", "Current in amperes") { IsRequired = true };

            command.AddOption(voltageOption);
            command.AddOption(currentOption);

            command.SetHandler(async (voltage, current) =>
            {
                var power = CircuitTool.PowerCalculator.Power(voltage, current);
                var resistance = CircuitTool.OhmsLawCalculator.Resistance(voltage, current);

                var table = ConsoleUI.CreateResultsTable("Power Calculation", "Parameter", "Value", "Unit");
                table.AddRow("Voltage", voltage.ToString("F3"), "V");
                table.AddRow("Current", current.ToString("F3"), "A");
                table.AddRow("Resistance", resistance.ToString("F3"), "Ω");
                table.AddRow("[bold green]Power[/]", power.ToString("F3"), "W");

                AnsiConsole.Write(table);

            }, voltageOption, currentOption);

            return command;
        }

        private static Command CreateResistanceCommand()
        {
            var command = new Command("resistance", "Series and parallel resistance calculations");

            var valuesArgument = new Argument<double[]>("values", "Resistance values in ohms");
            var seriesOption = new Option<bool>("--series", "Calculate series resistance (default)");
            var parallelOption = new Option<bool>("--parallel", "Calculate parallel resistance");

            command.AddArgument(valuesArgument);
            command.AddOption(seriesOption);
            command.AddOption(parallelOption);

            command.SetHandler(async (values, series, parallel) =>
            {
                if (values == null || values.Length == 0)
                {
                    ConsoleUI.DisplayError("Please provide at least one resistance value");
                    return;
                }

                var isSeries = !parallel; // Default to series unless parallel is specified

                var totalResistance = CircuitTool.CircuitCalculations.CalculateTotalResistance(values, isSeries);
                var configType = isSeries ? "Series" : "Parallel";

                var table = ConsoleUI.CreateResultsTable($"{configType} Resistance Calculation", "Parameter", "Value", "Unit");
                
                for (int i = 0; i < values.Length; i++)
                {
                    table.AddRow($"R{i + 1}", values[i].ToString("F3"), "Ω");
                }
                
                table.AddRow($"[bold green]{configType} Total[/]", totalResistance.ToString("F3"), "Ω");

                AnsiConsole.Write(table);

            }, valuesArgument, seriesOption, parallelOption);

            return command;
        }
    }
}
