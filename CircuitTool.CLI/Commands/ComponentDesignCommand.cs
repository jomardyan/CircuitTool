using System;
using System.CommandLine;
using System.Linq;
using System.Threading.Tasks;
using CircuitTool.CLI.UI;
using Spectre.Console;

namespace CircuitTool.CLI.Commands
{
    /// <summary>
    /// Component design command
    /// </summary>
    internal static class ComponentDesignCommand
    {
        public static Command Create()
        {
            var command = new Command("component", "Component design and calculations")
            {
                CreateLEDCommand(),
                CreateVoltageDividerCommand(),
                CreateFilterCommand()
            };

            return command;
        }

        private static Command CreateLEDCommand()
        {
            var command = new Command("led", "LED current limiting resistor calculator");

            var supplyOption = new Option<double>("--supply", "Supply voltage in volts") { IsRequired = true };
            var forwardOption = new Option<double>("--forward", "LED forward voltage in volts") { IsRequired = true };
            var currentOption = new Option<double>("--current", "Desired LED current in amperes") { IsRequired = true };

            command.AddOption(supplyOption);
            command.AddOption(forwardOption);
            command.AddOption(currentOption);

            command.SetHandler(async (supply, forward, current) =>
            {
                try
                {
                    var resistorValue = CircuitTool.LEDCalculator.CalculateResistorValue(supply, forward, current);
                    var ledPower = CircuitTool.LEDCalculator.CalculateLEDPower(forward, current);
                    var resistorPower = Math.Pow(current, 2) * resistorValue;

                    var table = ConsoleUI.CreateResultsTable("LED Current Limiting Resistor", "Parameter", "Value", "Unit");
                    table.AddRow("Supply Voltage", supply.ToString("F2"), "V");
                    table.AddRow("LED Forward Voltage", forward.ToString("F2"), "V");
                    table.AddRow("LED Current", (current * 1000).ToString("F1"), "mA");
                    table.AddRow("[bold green]Resistor Value[/]", resistorValue.ToString("F1"), "Ω");
                    table.AddRow("[bold yellow]LED Power[/]", (ledPower * 1000).ToString("F1"), "mW");
                    table.AddRow("[bold yellow]Resistor Power[/]", (resistorPower * 1000).ToString("F1"), "mW");

                    AnsiConsole.Write(table);

                    // Power rating recommendation
                    var recommendedPower = resistorPower * 2; // Safety factor
                    var standardRatings = new[] { 0.125, 0.25, 0.5, 1.0, 2.0, 5.0 };
                    var recommendedRating = standardRatings.FirstOrDefault(r => r >= recommendedPower);
                    if (recommendedRating == 0) recommendedRating = 5.0;

                    var infoPanel = new Panel(
                        new Markup($"[blue]Recommendations:[/]\n" +
                                  $"• Use a {recommendedRating:F3}W resistor (safety factor of 2x)\n" +
                                  $"• Standard resistor values: {GetNearestStandardResistor(resistorValue):F0}Ω\n" +
                                  $"• Voltage drop across resistor: {(supply - forward):F2}V"))
                    {
                        Header = new PanelHeader("💡 Design Notes"),
                        Border = BoxBorder.Rounded,
                        BorderStyle = Style.Parse("blue")
                    };

                    AnsiConsole.Write(infoPanel);
                }
                catch (Exception ex)
                {
                    ConsoleUI.DisplayError("Calculation error", ex);
                }

            }, supplyOption, forwardOption, currentOption);

            return command;
        }

        private static Command CreateVoltageDividerCommand()
        {
            var command = new Command("divider", "Voltage divider calculator");

            var inputOption = new Option<double>("--input", "Input voltage in volts") { IsRequired = true };
            var r1Option = new Option<double>("--r1", "R1 resistance in ohms") { IsRequired = true };
            var r2Option = new Option<double>("--r2", "R2 resistance in ohms") { IsRequired = true };

            command.AddOption(inputOption);
            command.AddOption(r1Option);
            command.AddOption(r2Option);

            command.SetHandler(async (input, r1, r2) =>
            {
                try
                {
                    var output = CircuitTool.VoltageDividerCalculator.Calculate(input, r1, r2);
                    var current = input / (r1 + r2);
                    var totalPower = input * current;
                    var r1Power = Math.Pow(current, 2) * r1;
                    var r2Power = Math.Pow(current, 2) * r2;

                    var table = ConsoleUI.CreateResultsTable("Voltage Divider Analysis", "Parameter", "Value", "Unit");
                    table.AddRow("Input Voltage", input.ToString("F3"), "V");
                    table.AddRow("R1", r1.ToString("F1"), "Ω");
                    table.AddRow("R2", r2.ToString("F1"), "Ω");
                    table.AddRow("[bold green]Output Voltage[/]", output.ToString("F3"), "V");
                    table.AddRow("Current", (current * 1000).ToString("F2"), "mA");
                    table.AddRow("Total Power", (totalPower * 1000).ToString("F1"), "mW");
                    table.AddRow("R1 Power", (r1Power * 1000).ToString("F1"), "mW");
                    table.AddRow("R2 Power", (r2Power * 1000).ToString("F1"), "mW");

                    AnsiConsole.Write(table);

                    var ratio = output / input;
                    var infoPanel = new Panel(
                        new Markup($"[blue]Analysis:[/]\n" +
                                  $"• Voltage ratio: {ratio:F3} ({ratio * 100:F1}%)\n" +
                                  $"• Total resistance: {(r1 + r2):F1}Ω\n" +
                                  $"• Loading effect: Consider load impedance >> R2"))
                    {
                        Header = new PanelHeader("📊 Design Analysis"),
                        Border = BoxBorder.Rounded,
                        BorderStyle = Style.Parse("blue")
                    };

                    AnsiConsole.Write(infoPanel);
                }
                catch (Exception ex)
                {
                    ConsoleUI.DisplayError("Calculation error", ex);
                }

            }, inputOption, r1Option, r2Option);

            return command;
        }

        private static Command CreateFilterCommand()
        {
            var command = new Command("filter", "Simple RC filter calculations");

            var resistanceOption = new Option<double>("--resistance", "Resistance in ohms") { IsRequired = true };
            var capacitanceOption = new Option<double>("--capacitance", "Capacitance in farads") { IsRequired = true };
            var typeOption = new Option<string>("--type", getDefaultValue: () => "lowpass", "Filter type (lowpass, highpass)");

            command.AddOption(resistanceOption);
            command.AddOption(capacitanceOption);
            command.AddOption(typeOption);

            command.SetHandler(async (resistance, capacitance, type) =>
            {
                try
                {
                    var cutoffFreq = 1.0 / (2 * Math.PI * resistance * capacitance);
                    var timeConstant = resistance * capacitance;

                    var table = ConsoleUI.CreateResultsTable($"{type.ToUpper()} RC Filter Analysis", "Parameter", "Value", "Unit");
                    table.AddRow("Resistance", resistance.ToString("F1"), "Ω");
                    table.AddRow("Capacitance", capacitance.ToString("E3"), "F");
                    table.AddRow("[bold green]Cutoff Frequency[/]", cutoffFreq.ToString("F2"), "Hz");
                    table.AddRow("Time Constant", (timeConstant * 1000).ToString("F2"), "ms");

                    if (cutoffFreq > 1000)
                    {
                        table.AddRow("[dim]Cutoff Frequency[/]", (cutoffFreq / 1000).ToString("F2"), "kHz");
                    }

                    AnsiConsole.Write(table);

                    var filterType = type.ToLower() == "highpass" ? "High-pass" : "Low-pass";
                    var description = type.ToLower() == "highpass" 
                        ? "Blocks low frequencies, passes high frequencies"
                        : "Passes low frequencies, blocks high frequencies";

                    var infoPanel = new Panel(
                        new Markup($"[blue]{filterType} Filter:[/]\n" +
                                  $"• {description}\n" +
                                  $"• -3dB point at cutoff frequency\n" +
                                  $"• Roll-off: 20dB/decade (6dB/octave)"))
                    {
                        Header = new PanelHeader("🔊 Filter Characteristics"),
                        Border = BoxBorder.Rounded,
                        BorderStyle = Style.Parse("blue")
                    };

                    AnsiConsole.Write(infoPanel);
                }
                catch (Exception ex)
                {
                    ConsoleUI.DisplayError("Calculation error", ex);
                }

            }, resistanceOption, capacitanceOption, typeOption);

            return command;
        }

        private static double GetNearestStandardResistor(double value)
        {
            // E12 series standard resistor values
            var e12Series = new[] { 1.0, 1.2, 1.5, 1.8, 2.2, 2.7, 3.3, 3.9, 4.7, 5.6, 6.8, 8.2 };
            
            // Find the appropriate decade
            var decade = Math.Pow(10, Math.Floor(Math.Log10(value)));
            var normalizedValue = value / decade;
            
            // Find nearest E12 value
            var nearest = e12Series.Aggregate((x, y) => Math.Abs(x - normalizedValue) < Math.Abs(y - normalizedValue) ? x : y);
            
            return nearest * decade;
        }
    }
}
