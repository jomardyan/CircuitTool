using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Spectre.Console;

namespace CircuitTool.CLI.UI
{
    /// <summary>
    /// Interactive menu system for the CLI
    /// </summary>
    internal static class InteractiveMenu
    {
        /// <summary>
        /// Runs the main interactive menu
        /// </summary>
        public static async Task RunAsync()
        {
            while (true)
            {
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[blue]Select a category to explore:[/]")
                        .PageSize(10)
                        .AddChoices(new[]
                        {
                            "🧮 Basic Calculations (Ohm's Law, Power)",
                            "⚡ AC Circuit Analysis",
                            "🔧 Component Design",
                            "📊 Power Analysis",
                            "🎯 Run Examples",
                            "⏱️  Performance Benchmarks",
                            "❓ Help & Documentation",
                            "🚪 Exit"
                        }));

                try
                {
                    await HandleMenuChoice(choice);
                }
                catch (Exception ex)
                {
                    ConsoleUI.DisplayError("An error occurred", ex);
                    AnsiConsole.Write(new Markup("[dim]Press any key to continue...[/]"));
                    Console.ReadKey(true);
                }

                if (choice.Contains("Exit"))
                    break;

                AnsiConsole.WriteLine();
            }
        }

        private static async Task HandleMenuChoice(string choice)
        {
            switch (choice)
            {
                case var c when c.Contains("Basic Calculations"):
                    await ShowBasicCalculationsMenu();
                    break;

                case var c when c.Contains("AC Circuit"):
                    await ShowACAnalysisMenu();
                    break;

                case var c when c.Contains("Component Design"):
                    await ShowComponentDesignMenu();
                    break;

                case var c when c.Contains("Power Analysis"):
                    await ShowPowerAnalysisMenu();
                    break;

                case var c when c.Contains("Examples"):
                    await ShowExamplesMenu();
                    break;

                case var c when c.Contains("Benchmarks"):
                    await ShowBenchmarksMenu();
                    break;

                case var c when c.Contains("Help"):
                    ShowHelp();
                    break;

                case var c when c.Contains("Exit"):
                    AnsiConsole.Write(new Markup("[blue]Thank you for using CircuitTool CLI![/]"));
                    break;
            }
        }

        private static async Task ShowBasicCalculationsMenu()
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]Basic Calculations:[/]")
                    .AddChoices(new[]
                    {
                        "Calculate Voltage (V = I × R)",
                        "Calculate Current (I = V / R)",
                        "Calculate Resistance (R = V / I)",
                        "Calculate Power (P = V × I)",
                        "Series/Parallel Resistance",
                        "← Back to main menu"
                    }));

            if (choice.Contains("Back")) return;

            switch (choice)
            {
                case var c when c.Contains("Voltage"):
                    await CalculateVoltage();
                    break;
                case var c when c.Contains("Current"):
                    await CalculateCurrent();
                    break;
                case var c when c.Contains("Resistance") && !c.Contains("Series"):
                    await CalculateResistance();
                    break;
                case var c when c.Contains("Power"):
                    await CalculatePower();
                    break;
                case var c when c.Contains("Series"):
                    await CalculateSeriesParallel();
                    break;
            }
        }

        private static async Task ShowACAnalysisMenu()
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]AC Circuit Analysis:[/]")
                    .AddChoices(new[]
                    {
                        "Reactance Calculations",
                        "Impedance Analysis",
                        "Resonant Frequency",
                        "Q Factor",
                        "RMS/Peak Conversions",
                        "← Back to main menu"
                    }));

            if (choice.Contains("Back")) return;

            // Implement AC analysis functions
            AnsiConsole.Write(new Markup($"[dim]AC Analysis: {choice} - Coming soon![/]"));
            await Task.Delay(1000);
        }

        private static async Task ShowComponentDesignMenu()
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[magenta]Component Design:[/]")
                    .AddChoices(new[]
                    {
                        "LED Current Limiting Resistor",
                        "Voltage Divider Design",
                        "Filter Design",
                        "Transformer Calculations",
                        "← Back to main menu"
                    }));

            if (choice.Contains("Back")) return;

            switch (choice)
            {
                case var c when c.Contains("LED"):
                    await CalculateLEDResistor();
                    break;
                case var c when c.Contains("Voltage Divider"):
                    await CalculateVoltageDivider();
                    break;
                default:
                    AnsiConsole.Write(new Markup($"[dim]Component Design: {choice} - Coming soon![/]"));
                    await Task.Delay(1000);
                    break;
            }
        }

        private static async Task ShowPowerAnalysisMenu()
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[red]Power Analysis:[/]")
                    .AddChoices(new[]
                    {
                        "Energy Consumption",
                        "Electricity Bill Calculator",
                        "Power Factor Analysis",
                        "Efficiency Calculations",
                        "← Back to main menu"
                    }));

            if (choice.Contains("Back")) return;

            switch (choice)
            {
                case var c when c.Contains("Energy"):
                    await CalculateEnergyConsumption();
                    break;
                case var c when c.Contains("Bill"):
                    await CalculateElectricityBill();
                    break;
                default:
                    AnsiConsole.Write(new Markup($"[dim]Power Analysis: {choice} - Coming soon![/]"));
                    await Task.Delay(1000);
                    break;
            }
        }

        private static async Task ShowExamplesMenu()
        {
            AnsiConsole.Write(new Markup("[blue]Running CircuitTool Examples...[/]"));
            AnsiConsole.WriteLine();

            // Run the built-in documentation examples
            try
            {
                CircuitTool.DocumentationExamples.RunAllExamples();
                ConsoleUI.DisplaySuccess("Examples completed successfully!");
            }
            catch (Exception ex)
            {
                ConsoleUI.DisplayError("Error running examples", ex);
            }

            await Task.Delay(2000);
        }

        private static async Task ShowBenchmarksMenu()
        {
            AnsiConsole.Write(new Markup("[cyan]Performance Benchmarks - Coming soon![/]"));
            await Task.Delay(1000);
        }

        private static void ShowHelp()
        {
            var helpPanel = new Panel(
                new Markup(
                    "[bold]CircuitTool CLI Help[/]\n\n" +
                    "[blue]Available Commands:[/]\n" +
                    "• basic - Basic electrical calculations\n" +
                    "• ac - AC circuit analysis\n" +
                    "• component - Component design tools\n" +
                    "• power - Power analysis tools\n" +
                    "• examples - Run built-in examples\n" +
                    "• benchmark - Performance tests\n\n" +
                    "[blue]Global Options:[/]\n" +
                    "• --verbose, -v - Enable verbose output\n" +
                    "• --format, -f - Output format (table, json, csv)\n\n" +
                    "[blue]Interactive Mode:[/]\n" +
                    "Run without arguments to enter interactive mode"))
            {
                Header = new PanelHeader("📖 Help"),
                Border = BoxBorder.Rounded,
                BorderStyle = Style.Parse("blue")
            };

            AnsiConsole.Write(helpPanel);
        }

        // Calculation methods
        private static async Task CalculateVoltage()
        {
            var current = ConsoleUI.PromptForDouble("Enter current (A):", 0);
            var resistance = ConsoleUI.PromptForDouble("Enter resistance (Ω):", 0);

            var voltage = CircuitTool.OhmsLawCalculator.Voltage(current, resistance);

            var table = ConsoleUI.CreateResultsTable("Voltage Calculation", "Parameter", "Value", "Unit");
            table.AddRow("Current", current.ToString("F3"), "A");
            table.AddRow("Resistance", resistance.ToString("F3"), "Ω");
            table.AddRow("[bold green]Voltage[/]", voltage.ToString("F3"), "V");

            AnsiConsole.Write(table);
            await Task.Delay(100);
        }

        private static async Task CalculateCurrent()
        {
            var voltage = ConsoleUI.PromptForDouble("Enter voltage (V):");
            var resistance = ConsoleUI.PromptForDouble("Enter resistance (Ω):", 0.001);

            var current = CircuitTool.OhmsLawCalculator.Current(voltage, resistance);

            var table = ConsoleUI.CreateResultsTable("Current Calculation", "Parameter", "Value", "Unit");
            table.AddRow("Voltage", voltage.ToString("F3"), "V");
            table.AddRow("Resistance", resistance.ToString("F3"), "Ω");
            table.AddRow("[bold green]Current[/]", current.ToString("F3"), "A");

            AnsiConsole.Write(table);
            await Task.Delay(100);
        }

        private static async Task CalculateResistance()
        {
            var voltage = ConsoleUI.PromptForDouble("Enter voltage (V):");
            var current = ConsoleUI.PromptForDouble("Enter current (A):", 0.001);

            var resistance = CircuitTool.OhmsLawCalculator.Resistance(voltage, current);

            var table = ConsoleUI.CreateResultsTable("Resistance Calculation", "Parameter", "Value", "Unit");
            table.AddRow("Voltage", voltage.ToString("F3"), "V");
            table.AddRow("Current", current.ToString("F3"), "A");
            table.AddRow("[bold green]Resistance[/]", resistance.ToString("F3"), "Ω");

            AnsiConsole.Write(table);
            await Task.Delay(100);
        }

        private static async Task CalculatePower()
        {
            var voltage = ConsoleUI.PromptForDouble("Enter voltage (V):");
            var current = ConsoleUI.PromptForDouble("Enter current (A):");

            var power = CircuitTool.PowerCalculator.Power(voltage, current);

            var table = ConsoleUI.CreateResultsTable("Power Calculation", "Parameter", "Value", "Unit");
            table.AddRow("Voltage", voltage.ToString("F3"), "V");
            table.AddRow("Current", current.ToString("F3"), "A");
            table.AddRow("[bold green]Power[/]", power.ToString("F3"), "W");

            AnsiConsole.Write(table);
            await Task.Delay(100);
        }

        private static async Task CalculateSeriesParallel()
        {
            var resistorCount = AnsiConsole.Prompt(
                new TextPrompt<int>("How many resistors?")
                    .ValidationErrorMessage("[red]Invalid number[/]")
                    .Validate(count => count > 0 && count <= 10 
                        ? ValidationResult.Success() 
                        : ValidationResult.Error("Must be between 1 and 10")));

            var resistors = new double[resistorCount];
            for (int i = 0; i < resistorCount; i++)
            {
                resistors[i] = ConsoleUI.PromptForDouble($"Enter resistor {i + 1} value (Ω):", 0.001);
            }

            var seriesTotal = CircuitTool.CircuitCalculations.CalculateTotalResistance(resistors, true);
            var parallelTotal = CircuitTool.CircuitCalculations.CalculateTotalResistance(resistors, false);

            var table = ConsoleUI.CreateResultsTable("Series/Parallel Resistance", "Configuration", "Total Resistance", "Unit");
            table.AddRow("[bold blue]Series[/]", seriesTotal.ToString("F3"), "Ω");
            table.AddRow("[bold blue]Parallel[/]", parallelTotal.ToString("F3"), "Ω");

            AnsiConsole.Write(table);
            
            // Show individual resistor values
            var detailTable = new Table()
                .Title("Individual Resistors")
                .Border(TableBorder.Simple)
                .AddColumn("Resistor")
                .AddColumn("Value (Ω)");

            for (int i = 0; i < resistors.Length; i++)
            {
                detailTable.AddRow($"R{i + 1}", resistors[i].ToString("F3"));
            }

            AnsiConsole.Write(detailTable);
            await Task.Delay(100);
        }

        private static async Task CalculateLEDResistor()
        {
            var supplyVoltage = ConsoleUI.PromptForDouble("Enter supply voltage (V):", 0);
            var ledVoltage = ConsoleUI.PromptForDouble("Enter LED forward voltage (V):", 0);
            var ledCurrent = ConsoleUI.PromptForDouble("Enter desired LED current (A):", 0.001);

            var resistorValue = CircuitTool.LEDCalculator.CalculateResistorValue(supplyVoltage, ledVoltage, ledCurrent);
            var ledPower = CircuitTool.LEDCalculator.CalculateLEDPower(ledVoltage, ledCurrent);
            var resistorPower = Math.Pow(ledCurrent, 2) * resistorValue;

            var table = ConsoleUI.CreateResultsTable("LED Current Limiting Resistor", "Parameter", "Value", "Unit");
            table.AddRow("Supply Voltage", supplyVoltage.ToString("F2"), "V");
            table.AddRow("LED Forward Voltage", ledVoltage.ToString("F2"), "V");
            table.AddRow("LED Current", (ledCurrent * 1000).ToString("F1"), "mA");
            table.AddRow("[bold green]Resistor Value[/]", resistorValue.ToString("F1"), "Ω");
            table.AddRow("[bold yellow]LED Power[/]", (ledPower * 1000).ToString("F1"), "mW");
            table.AddRow("[bold yellow]Resistor Power[/]", (resistorPower * 1000).ToString("F1"), "mW");

            AnsiConsole.Write(table);
            await Task.Delay(100);
        }

        private static async Task CalculateVoltageDivider()
        {
            var inputVoltage = ConsoleUI.PromptForDouble("Enter input voltage (V):", 0);
            var r1 = ConsoleUI.PromptForDouble("Enter R1 value (Ω):", 0.001);
            var r2 = ConsoleUI.PromptForDouble("Enter R2 value (Ω):", 0.001);

            var outputVoltage = CircuitTool.VoltageDividerCalculator.Calculate(inputVoltage, r1, r2);
            var current = inputVoltage / (r1 + r2);
            var totalPower = inputVoltage * current;

            var table = ConsoleUI.CreateResultsTable("Voltage Divider", "Parameter", "Value", "Unit");
            table.AddRow("Input Voltage", inputVoltage.ToString("F3"), "V");
            table.AddRow("R1", r1.ToString("F1"), "Ω");
            table.AddRow("R2", r2.ToString("F1"), "Ω");
            table.AddRow("[bold green]Output Voltage[/]", outputVoltage.ToString("F3"), "V");
            table.AddRow("Current", (current * 1000).ToString("F2"), "mA");
            table.AddRow("Total Power", (totalPower * 1000).ToString("F1"), "mW");

            AnsiConsole.Write(table);
            await Task.Delay(100);
        }

        private static async Task CalculateEnergyConsumption()
        {
            var power = ConsoleUI.PromptForDouble("Enter power consumption (W):", 0);
            var hours = ConsoleUI.PromptForDouble("Enter time period (hours):", 0);

            var energy = CircuitTool.EnergyCalculator.KWh(power, hours);
            var energyKWh = energy / 1000;

            var table = ConsoleUI.CreateResultsTable("Energy Consumption", "Parameter", "Value", "Unit");
            table.AddRow("Power", power.ToString("F2"), "W");
            table.AddRow("Time", hours.ToString("F2"), "hours");
            table.AddRow("[bold green]Energy (Wh)[/]", energy.ToString("F2"), "Wh");
            table.AddRow("[bold green]Energy (kWh)[/]", energyKWh.ToString("F4"), "kWh");

            AnsiConsole.Write(table);
            await Task.Delay(100);
        }

        private static async Task CalculateElectricityBill()
        {
            var energyKWh = ConsoleUI.PromptForDouble("Enter energy consumption (kWh):", 0);
            var ratePerKWh = ConsoleUI.PromptForDouble("Enter rate per kWh ($):", 0);

            var cost = CircuitTool.ElectricityBillCalculator.CalculateBill(energyKWh, ratePerKWh);

            var table = ConsoleUI.CreateResultsTable("Electricity Bill", "Parameter", "Value", "Unit");
            table.AddRow("Energy Consumption", energyKWh.ToString("F2"), "kWh");
            table.AddRow("Rate", ratePerKWh.ToString("F4"), "$/kWh");
            table.AddRow("[bold green]Total Cost[/]", cost.ToString("C2"), "");

            AnsiConsole.Write(table);
            await Task.Delay(100);
        }
    }
}
