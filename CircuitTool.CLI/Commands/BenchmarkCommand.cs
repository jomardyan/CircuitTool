using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CircuitTool.CLI.UI;
using Spectre.Console;

namespace CircuitTool.CLI.Commands
{
    /// <summary>
    /// Performance benchmark command
    /// </summary>
    internal static class BenchmarkCommand
    {
        public static Command Create()
        {
            var command = new Command("benchmark", "Performance benchmarks for CircuitTool calculations");

            var iterationsOption = new Option<int>("--iterations", getDefaultValue: () => 10000, "Number of iterations to run");
            var testOption = new Option<string>("--test", getDefaultValue: () => "all", "Specific test to run (all, ohms, power, ac)");

            command.AddOption(iterationsOption);
            command.AddOption(testOption);

            command.SetHandler(async (iterations, test) =>
            {
                await RunBenchmarks(iterations, test);
            }, iterationsOption, testOption);

            return command;
        }

        private static async Task RunBenchmarks(int iterations, string test)
        {
            ConsoleUI.DisplaySuccess($"Running benchmarks with {iterations:N0} iterations");
            AnsiConsole.WriteLine();

            var results = new List<BenchmarkResult>();

            if (test == "all" || test == "ohms")
            {
                results.Add(await BenchmarkOhmsLaw(iterations));
            }

            if (test == "all" || test == "power")
            {
                results.Add(await BenchmarkPowerCalculations(iterations));
            }

            if (test == "all" || test == "ac")
            {
                results.Add(await BenchmarkACCalculations(iterations));
            }

            // Display results
            var table = ConsoleUI.CreateResultsTable("Performance Benchmark Results", 
                "Test", "Iterations", "Total Time", "Avg Time", "Operations/sec");

            foreach (var result in results)
            {
                table.AddRow(
                    result.TestName,
                    result.Iterations.ToString("N0"),
                    result.TotalTime.ToString("F2") + " ms",
                    result.AverageTime.ToString("F4") + " ms",
                    result.OperationsPerSecond.ToString("N0"));
            }

            AnsiConsole.Write(table);

            // Performance summary
            var totalOps = results.Sum(r => r.OperationsPerSecond);
            var avgTime = results.Average(r => r.AverageTime);

            var performanceRating = avgTime < 0.01 ? "[green]performing well[/]" : "[yellow]adequate[/]";
            var summaryPanel = new Panel(
                new Markup($"[blue]Performance Summary:[/]\n" +
                          $"• Total operations/second: {totalOps:N0}\n" +
                          $"• Average calculation time: {avgTime:F4} ms\n" +
                          $"• Framework is {performanceRating}"))
            {
                Header = new PanelHeader("⚡ Benchmark Summary"),
                Border = BoxBorder.Rounded,
                BorderStyle = Style.Parse("blue")
            };

            AnsiConsole.Write(summaryPanel);
        }

        private static async Task<BenchmarkResult> BenchmarkOhmsLaw(int iterations)
        {
            var result = new BenchmarkResult();
            
            await ConsoleUI.WithProgress("Benchmarking Ohm's Law calculations", async (task) =>
            {
                var sw = Stopwatch.StartNew();
                var random = new Random(12345); // Fixed seed for consistency

                for (int i = 0; i < iterations; i++)
                {
                    var voltage = random.NextDouble() * 100 + 1; // 1-101V
                    var current = random.NextDouble() * 10 + 0.1; // 0.1-10.1A
                    var resistance = random.NextDouble() * 1000 + 1; // 1-1001Ω

                    // Perform calculations
                    CircuitTool.OhmsLawCalculator.Voltage(current, resistance);
                    CircuitTool.OhmsLawCalculator.Current(voltage, resistance);
                    CircuitTool.OhmsLawCalculator.Resistance(voltage, current);

                    if (i % Math.Max(1, iterations / 100) == 0)
                    {
                        task.Increment(1);
                    }
                }

                sw.Stop();
                task.Value = 100;

                result.TestName = "Ohm's Law";
                result.Iterations = iterations * 3; // 3 operations per iteration
                result.TotalTime = sw.Elapsed.TotalMilliseconds;
                result.AverageTime = sw.Elapsed.TotalMilliseconds / (iterations * 3);
                result.OperationsPerSecond = (iterations * 3) / sw.Elapsed.TotalSeconds;
            });
            
            return result;
        }

        private static async Task<BenchmarkResult> BenchmarkPowerCalculations(int iterations)
        {
            var result = new BenchmarkResult();
            
            await ConsoleUI.WithProgress("Benchmarking Power calculations", async (task) =>
            {
                var sw = Stopwatch.StartNew();
                var random = new Random(12345);

                for (int i = 0; i < iterations; i++)
                {
                    var voltage = random.NextDouble() * 100 + 1;
                    var current = random.NextDouble() * 10 + 0.1;
                    var resistance = random.NextDouble() * 1000 + 1;

                    // Perform power calculations
                    CircuitTool.PowerCalculator.Power(voltage, current);
                    CircuitTool.PowerCalculator.PowerFromCurrentResistance(current, resistance);
                    CircuitTool.PowerCalculator.PowerFromVoltageResistance(voltage, resistance);

                    if (i % Math.Max(1, iterations / 100) == 0)
                    {
                        task.Increment(1);
                    }
                }

                sw.Stop();
                task.Value = 100;

                result.TestName = "Power Calculations";
                result.Iterations = iterations * 3;
                result.TotalTime = sw.Elapsed.TotalMilliseconds;
                result.AverageTime = sw.Elapsed.TotalMilliseconds / (iterations * 3);
                result.OperationsPerSecond = (iterations * 3) / sw.Elapsed.TotalSeconds;
            });
            
            return result;
        }

        private static async Task<BenchmarkResult> BenchmarkACCalculations(int iterations)
        {
            var result = new BenchmarkResult();
            
            await ConsoleUI.WithProgress("Benchmarking AC Circuit calculations", async (task) =>
            {
                var sw = Stopwatch.StartNew();
                var random = new Random(12345);

                for (int i = 0; i < iterations; i++)
                {
                    var frequency = random.NextDouble() * 10000 + 100; // 100-10100 Hz
                    var inductance = random.NextDouble() * 0.1 + 0.001; // 1mH - 101mH
                    var capacitance = random.NextDouble() * 0.000001 + 0.0000001; // 0.1μF - 1.1μF
                    var resistance = random.NextDouble() * 1000 + 10; // 10-1010Ω

                    // Perform AC calculations
                    CircuitTool.InductorCalculator.InductiveReactance(frequency, inductance);
                    CircuitTool.CapacitorCalculator.CapacitiveReactance(frequency, capacitance);
                    // Note: SeriesRLCImpedance might need to be checked for actual method name

                    if (i % Math.Max(1, iterations / 100) == 0)
                    {
                        task.Increment(1);
                    }
                }

                sw.Stop();
                task.Value = 100;

                result.TestName = "AC Circuits";
                result.Iterations = iterations * 2;
                result.TotalTime = sw.Elapsed.TotalMilliseconds;
                result.AverageTime = sw.Elapsed.TotalMilliseconds / (iterations * 2);
                result.OperationsPerSecond = (iterations * 2) / sw.Elapsed.TotalSeconds;
            });
            
            return result;
        }

        private class BenchmarkResult
        {
            public string TestName { get; set; } = "";
            public int Iterations { get; set; }
            public double TotalTime { get; set; }
            public double AverageTime { get; set; }
            public double OperationsPerSecond { get; set; }
        }
    }
}
