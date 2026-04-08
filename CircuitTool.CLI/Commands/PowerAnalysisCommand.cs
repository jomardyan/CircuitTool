using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Threading.Tasks;
using CircuitTool.CLI.UI;
using Spectre.Console;

namespace CircuitTool.CLI.Commands
{
    /// <summary>
    /// Power analysis command
    /// </summary>
    internal static class PowerAnalysisCommand
    {
        public static Command Create()
        {
            var command = new Command("power", "Power analysis and energy calculations")
            {
                CreateEnergyCommand(),
                CreateBillCommand(),
                CreateEfficiencyCommand()
            };

            return command;
        }

        private static Command CreateEnergyCommand()
        {
            var command = new Command("energy", "Energy consumption calculation");

            var powerOption = new Option<double>("--power", "Power consumption in watts") { IsRequired = true };
            var timeOption = new Option<double>("--time", "Time period in hours") { IsRequired = true };

            command.AddOption(powerOption);
            command.AddOption(timeOption);

            command.SetHandler(async (power, time) =>
            {
                try
                {
                    var energy = CircuitTool.EnergyCalculator.Joules(power, time * 3600); // Convert hours to seconds
                    var energyKWh = CircuitTool.EnergyCalculator.KWh(power, time);

                    var table = ConsoleUI.CreateResultsTable("Energy Consumption Analysis", "Parameter", "Value", "Unit");
                    table.AddRow("Power", power.ToString("F2"), "W");
                    table.AddRow("Time", time.ToString("F2"), "hours");
                    table.AddRow("[bold green]Energy (J)[/]", energy.ToString("F2"), "J");
                    table.AddRow("[bold green]Energy (kWh)[/]", energyKWh.ToString("F4"), "kWh");

                    // Add some comparison data
                    var dailyEnergyKWh = energyKWh * (24.0 / time);
                    var monthlyEnergyKWh = dailyEnergyKWh * 30;
                    var yearlyEnergyKWh = dailyEnergyKWh * 365;

                    table.AddRow("[dim]Daily (est.)[/]", dailyEnergyKWh.ToString("F2"), "kWh");
                    table.AddRow("[dim]Monthly (est.)[/]", monthlyEnergyKWh.ToString("F1"), "kWh");
                    table.AddRow("[dim]Yearly (est.)[/]", yearlyEnergyKWh.ToString("F0"), "kWh");

                    AnsiConsole.Write(table);

                    // Environmental impact estimation
                    var co2PerKWh = 0.5; // kg CO2 per kWh (average)
                    var yearlyCO2 = yearlyEnergyKWh * co2PerKWh;

                    var envPanel = new Panel(
                        new Markup($"[green]Environmental Impact (estimated):[/]\n" +
                                  $"• Yearly CO₂ emissions: {yearlyCO2:F1} kg\n" +
                                  $"• Equivalent to planting {(yearlyCO2 / 21):F1} trees per year"))
                    {
                        Header = new PanelHeader("🌱 Environmental Impact"),
                        Border = BoxBorder.Rounded,
                        BorderStyle = Style.Parse("green")
                    };

                    AnsiConsole.Write(envPanel);
                }
                catch (Exception ex)
                {
                    ConsoleUI.DisplayError("Calculation error", ex);
                }

            }, powerOption, timeOption);

            return command;
        }

        private static Command CreateBillCommand()
        {
            var command = new Command("bill", "Electricity bill calculation");

            var energyOption = new Option<double>("--energy", "Energy consumption in kWh") { IsRequired = true };
            var rateOption = new Option<double>("--rate", "Rate per kWh in currency units") { IsRequired = true };
            var fixedOption = new Option<double>("--fixed", getDefaultValue: () => 0.0, "Fixed monthly charge");

            command.AddOption(energyOption);
            command.AddOption(rateOption);
            command.AddOption(fixedOption);

            command.SetHandler(async (energy, rate, fixedCharge) =>
            {
                try
                {
                    var energyCost = CircuitTool.ElectricityBillCalculator.CalculateBill(energy, rate);
                    var totalCost = energyCost + fixedCharge;

                    var table = ConsoleUI.CreateResultsTable("Electricity Bill Analysis", "Parameter", "Value", "Unit");
                    table.AddRow("Energy Consumption", energy.ToString("F2"), "kWh");
                    table.AddRow("Rate per kWh", rate.ToString("C4"), "");
                    table.AddRow("Fixed Charge", fixedCharge.ToString("C2"), "");
                    table.AddRow("[bold green]Energy Cost[/]", energyCost.ToString("C2"), "");
                    table.AddRow("[bold green]Total Bill[/]", totalCost.ToString("C2"), "");

                    AnsiConsole.Write(table);

                    // Cost breakdown and savings analysis
                    var avgDailyUsage = energy / 30; // Assuming monthly bill
                    var avgDailyCost = totalCost / 30;

                    var savingsPanel = new Panel(
                        new Markup($"[blue]Cost Analysis:[/]\n" +
                                  $"• Average daily usage: {avgDailyUsage:F2} kWh\n" +
                                  $"• Average daily cost: {avgDailyCost:C2}\n" +
                                  $"• Cost per kWh (with fixed): {(totalCost / energy):C4}\n" +
                                  $"• Reduce usage by 10% to save: {(totalCost * 0.1):C2}"))
                    {
                        Header = new PanelHeader("💰 Cost Breakdown"),
                        Border = BoxBorder.Rounded,
                        BorderStyle = Style.Parse("blue")
                    };

                    AnsiConsole.Write(savingsPanel);
                }
                catch (Exception ex)
                {
                    ConsoleUI.DisplayError("Calculation error", ex);
                }

            }, energyOption, rateOption, fixedOption);

            return command;
        }

        private static Command CreateEfficiencyCommand()
        {
            var command = new Command("efficiency", "Power efficiency calculation");

            var inputOption = new Option<double>("--input", "Input power in watts") { IsRequired = true };
            var outputOption = new Option<double>("--output", "Output power in watts") { IsRequired = true };

            command.AddOption(inputOption);
            command.AddOption(outputOption);

            command.SetHandler(async (input, output) =>
            {
                try
                {
                    if (output > input)
                    {
                        ConsoleUI.DisplayWarning("Output power cannot exceed input power in a passive system");
                    }

                    var efficiency = (output / input) * 100;
                    var powerLoss = input - output;
                    var lossPercentage = (powerLoss / input) * 100;

                    var table = ConsoleUI.CreateResultsTable("Power Efficiency Analysis", "Parameter", "Value", "Unit");
                    table.AddRow("Input Power", input.ToString("F2"), "W");
                    table.AddRow("Output Power", output.ToString("F2"), "W");
                    table.AddRow("Power Loss", powerLoss.ToString("F2"), "W");
                    table.AddRow("[bold green]Efficiency[/]", efficiency.ToString("F2"), "%");
                    table.AddRow("[bold red]Loss Percentage[/]", lossPercentage.ToString("F2"), "%");

                    AnsiConsole.Write(table);

                    // Efficiency rating and thermal analysis
                    var efficiencyRating = GetEfficiencyRating(efficiency);
                    var thermalPower = powerLoss; // Assuming all loss becomes heat

                    var analysisPanel = new Panel(
                        new Markup($"[blue]Performance Analysis:[/]\n" +
                                  $"• Efficiency rating: {efficiencyRating}\n" +
                                  $"• Heat generated: {thermalPower:F2}W\n" +
                                  $"• Energy wasted per hour: {(powerLoss / 1000):F3} kWh\n" +
                                  $"• Improve by 5% to save: {(powerLoss * 0.05):F2}W"))
                    {
                        Header = new PanelHeader("⚡ Performance Rating"),
                        Border = BoxBorder.Rounded,
                        BorderStyle = Style.Parse("blue")
                    };

                    AnsiConsole.Write(analysisPanel);
                }
                catch (Exception ex)
                {
                    ConsoleUI.DisplayError("Calculation error", ex);
                }

            }, inputOption, outputOption);

            return command;
        }

        private static string GetEfficiencyRating(double efficiency)
        {
            return efficiency switch
            {
                >= 95 => "[bold green]Excellent (>95%)[/]",
                >= 90 => "[green]Very Good (90-95%)[/]",
                >= 85 => "[yellow]Good (85-90%)[/]",
                >= 80 => "[orange1]Fair (80-85%)[/]",
                >= 70 => "[red]Poor (70-80%)[/]",
                _ => "[bold red]Very Poor (<70%)[/]"
            };
        }
    }
}
