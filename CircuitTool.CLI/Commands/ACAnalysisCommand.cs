using System;
using System.CommandLine;
using System.Threading.Tasks;
using CircuitTool.CLI.UI;
using Spectre.Console;

namespace CircuitTool.CLI.Commands
{
    /// <summary>
    /// AC circuit analysis command
    /// </summary>
    internal static class ACAnalysisCommand
    {
        public static Command Create()
        {
            var command = new Command("ac", "AC circuit analysis")
            {
                CreateReactanceCommand(),
                CreateImpedanceCommand(),
                CreateResonanceCommand()
            };

            return command;
        }

        private static Command CreateReactanceCommand()
        {
            var command = new Command("reactance", "Calculate inductive and capacitive reactance");

            var frequencyOption = new Option<double>("--frequency", "Frequency in Hz") { IsRequired = true };
            var inductanceOption = new Option<double?>("--inductance", "Inductance in henries");
            var capacitanceOption = new Option<double?>("--capacitance", "Capacitance in farads");

            command.AddOption(frequencyOption);
            command.AddOption(inductanceOption);
            command.AddOption(capacitanceOption);

            command.SetHandler(async (frequency, inductance, capacitance) =>
            {
                var table = ConsoleUI.CreateResultsTable("Reactance Calculations", "Parameter", "Value", "Unit");
                table.AddRow("Frequency", frequency.ToString("F2"), "Hz");

                if (inductance.HasValue)
                {
                    var xl = CircuitTool.PhysicsCircuitCalculators.InductiveReactance(frequency, inductance.Value);
                    table.AddRow("Inductance", inductance.Value.ToString("E3"), "H");
                    table.AddRow("[bold green]Inductive Reactance (XL)[/]", xl.ToString("F3"), "Ω");
                }

                if (capacitance.HasValue)
                {
                    var xc = CircuitTool.CapacitorCalculator.CapacitiveReactance(frequency, capacitance.Value);
                    table.AddRow("Capacitance", capacitance.Value.ToString("E3"), "F");
                    table.AddRow("[bold green]Capacitive Reactance (XC)[/]", xc.ToString("F3"), "Ω");
                }

                if (!inductance.HasValue && !capacitance.HasValue)
                {
                    ConsoleUI.DisplayError("Please provide either inductance or capacitance (or both)");
                    return;
                }

                AnsiConsole.Write(table);

            }, frequencyOption, inductanceOption, capacitanceOption);

            return command;
        }

        private static Command CreateImpedanceCommand()
        {
            var command = new Command("impedance", "Calculate RLC circuit impedance");

            var resistanceOption = new Option<double>("--resistance", "Resistance in ohms") { IsRequired = true };
            var inductanceOption = new Option<double>("--inductance", "Inductance in henries") { IsRequired = true };
            var capacitanceOption = new Option<double>("--capacitance", "Capacitance in farads") { IsRequired = true };
            var frequencyOption = new Option<double>("--frequency", "Frequency in Hz") { IsRequired = true };

            command.AddOption(resistanceOption);
            command.AddOption(inductanceOption);
            command.AddOption(capacitanceOption);
            command.AddOption(frequencyOption);

            command.SetHandler(async (resistance, inductance, capacitance, frequency) =>
            {
                var impedance = CircuitTool.PhysicsCircuitCalculators.SeriesRLCImpedance(resistance, inductance, capacitance, frequency);
                var xl = CircuitTool.PhysicsCircuitCalculators.InductiveReactance(frequency, inductance);
                var xc = CircuitTool.CapacitorCalculator.CapacitiveReactance(frequency, capacitance);
                var x = xl - xc; // Net reactance

                var table = ConsoleUI.CreateResultsTable("RLC Impedance Analysis", "Parameter", "Value", "Unit");
                table.AddRow("Frequency", frequency.ToString("F2"), "Hz");
                table.AddRow("Resistance (R)", resistance.ToString("F3"), "Ω");
                table.AddRow("Inductance (L)", inductance.ToString("E3"), "H");
                table.AddRow("Capacitance (C)", capacitance.ToString("E3"), "F");
                table.AddRow("Inductive Reactance (XL)", xl.ToString("F3"), "Ω");
                table.AddRow("Capacitive Reactance (XC)", xc.ToString("F3"), "Ω");
                table.AddRow("Net Reactance (X)", x.ToString("F3"), "Ω");
                table.AddRow("[bold green]Impedance (Z)[/]", impedance.ToString("F3"), "Ω");

                AnsiConsole.Write(table);

            }, resistanceOption, inductanceOption, capacitanceOption, frequencyOption);

            return command;
        }

        private static Command CreateResonanceCommand()
        {
            var command = new Command("resonance", "Calculate resonant frequency");

            var inductanceOption = new Option<double>("--inductance", "Inductance in henries") { IsRequired = true };
            var capacitanceOption = new Option<double>("--capacitance", "Capacitance in farads") { IsRequired = true };

            command.AddOption(inductanceOption);
            command.AddOption(capacitanceOption);

            command.SetHandler(async (inductance, capacitance) =>
            {
                var resonantFreq = CircuitTool.PhysicsCircuitCalculators.ResonantFrequencyLC(inductance, capacitance);

                var table = ConsoleUI.CreateResultsTable("Resonant Frequency Calculation", "Parameter", "Value", "Unit");
                table.AddRow("Inductance (L)", inductance.ToString("E3"), "H");
                table.AddRow("Capacitance (C)", capacitance.ToString("E3"), "F");
                table.AddRow("[bold green]Resonant Frequency[/]", resonantFreq.ToString("F2"), "Hz");

                if (resonantFreq > 1000)
                {
                    table.AddRow("[dim]Resonant Frequency[/]", (resonantFreq / 1000).ToString("F2"), "kHz");
                }

                AnsiConsole.Write(table);

            }, inductanceOption, capacitanceOption);

            return command;
        }
    }
}
