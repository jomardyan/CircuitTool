using System;
#if !NET20
using System.Linq;
#endif

namespace CircuitTool
{
    /// <summary>
    /// Provides methods for resistor calculations, including Ohm's Law, series, and parallel combinations.
    /// </summary>
    /// <remarks>
    /// <para>Example usage:</para>
    /// <code>
    /// double r = ResistorCalculator.Resistance(10, 2); // 5 Ohms
    /// double series = ResistorCalculator.Series(10, 20, 30); // 60 Ohms
    /// double parallel = ResistorCalculator.Parallel(10, 20); // 6.67 Ohms
    /// </code>
    /// </remarks>
    public static class ResistorCalculator
    {
        /// <summary>
        /// Calculates resistance using Ohm's Law: <c>R = V / I</c>.
        /// </summary>
        /// <param name="voltage">Voltage in volts (V).</param>
        /// <param name="current">Current in amperes (A).</param>
        /// <returns>Resistance in ohms (Ω).</returns>
        /// <example>
        /// double r = ResistorCalculator.Resistance(10, 2); // 5 Ohms
        /// </example>
        public static double Resistance(double voltage, double current) => voltage / current;

        /// <summary>
        /// Calculates total resistance for resistors in series: <c>Rtotal = R1 + R2 + ...</c>
        /// </summary>
        /// <param name="resistors">Resistor values in ohms (Ω).</param>
        /// <returns>Total series resistance in ohms (Ω).</returns>
        /// <example>
        /// double total = ResistorCalculator.Series(10, 20, 30); // 60 Ohms
        /// </example>
        public static double Series(params double[] resistors)
        {
            if (resistors == null) return 0;
#if NET20
            double total = 0;
            foreach (double resistor in resistors)
            {
                total += resistor;
            }
            return total;
#else
            return resistors.Sum();
#endif
        }

        /// <summary>
        /// Calculates total resistance for resistors in parallel: <c>1/Rtotal = 1/R1 + 1/R2 + ...</c>
        /// </summary>
        /// <param name="resistors">Resistor values in ohms (Ω).</param>
        /// <returns>Total parallel resistance in ohms (Ω).</returns>
        /// <example>
        /// double total = ResistorCalculator.Parallel(10, 20); // 6.67 Ohms
        /// </example>
        public static double Parallel(params double[] resistors)
        {
            if (resistors == null) return 0;
#if NET20
            double reciprocalSum = 0;
            foreach (double resistor in resistors)
            {
                reciprocalSum += 1.0 / resistor;
            }
            return 1.0 / reciprocalSum;
#else
            return 1.0 / resistors.Sum(r => 1.0 / r);
#endif
        }

        /// <summary>
        /// Result of series/parallel resistor network analysis
        /// </summary>
        public class ResistorNetworkResult
        {
            public double TotalResistance { get; set; }
            public double TotalPower { get; set; }
            public double TotalCurrent { get; set; }
            public double TotalVoltage { get; set; }
            public List<ResistorAnalysis> IndividualResistors { get; set; } = new List<ResistorAnalysis>();
            public string NetworkConfiguration { get; set; }
            public double EfficiencyPercent { get; set; }
        }

        /// <summary>
        /// Individual resistor analysis in a network
        /// </summary>
        public class ResistorAnalysis
        {
            public double Resistance { get; set; }
            public double Voltage { get; set; }
            public double Current { get; set; }
            public double Power { get; set; }
            public double PowerRating { get; set; }
            public double SafetyMargin { get; set; }
            public string Position { get; set; }
            public bool WithinSafeOperatingArea { get; set; }
        }

        /// <summary>
        /// Complex resistor network configuration
        /// </summary>
        public class ResistorNetwork
        {
            public enum ConnectionType { Series, Parallel }

            public class ResistorGroup
            {
                public List<double> Resistors { get; set; } = new List<double>();
                public ConnectionType Connection { get; set; }
            }

            public List<ResistorGroup> Groups { get; set; } = new List<ResistorGroup>();
            public ConnectionType MainConnection { get; set; }
        }

        /// <summary>
        /// Analyze series resistor network with applied voltage
        /// </summary>
        public static ResistorNetworkResult AnalyzeSeriesNetwork(double[] resistors, double appliedVoltage,
            double[] powerRatings = null)
        {
            var result = new ResistorNetworkResult
            {
                NetworkConfiguration = "Series",
                TotalResistance = Series(resistors),
                TotalVoltage = appliedVoltage
            };

            result.TotalCurrent = appliedVoltage / result.TotalResistance;

            for (int i = 0; i < resistors.Length; i++)
            {
                var analysis = new ResistorAnalysis
                {
                    Resistance = resistors[i],
                    Current = result.TotalCurrent,
                    Voltage = result.TotalCurrent * resistors[i],
                    Power = result.TotalCurrent * result.TotalCurrent * resistors[i],
                    Position = $"R{i + 1}",
                    PowerRating = powerRatings?[i] ?? 0.25 // Default 1/4 watt
                };

                analysis.SafetyMargin = analysis.PowerRating > 0 ?
                    (analysis.PowerRating - analysis.Power) / analysis.PowerRating * 100 : 0;
                analysis.WithinSafeOperatingArea = analysis.Power <= analysis.PowerRating * 0.8; // 80% derating

                result.IndividualResistors.Add(analysis);
            }

            result.TotalPower = result.IndividualResistors.Sum(r => r.Power);

            return result;
        }

        /// <summary>
        /// Analyze parallel resistor network with applied voltage
        /// </summary>
        public static ResistorNetworkResult AnalyzeParallelNetwork(double[] resistors, double appliedVoltage,
            double[] powerRatings = null)
        {
            var result = new ResistorNetworkResult
            {
                NetworkConfiguration = "Parallel",
                TotalResistance = Parallel(resistors),
                TotalVoltage = appliedVoltage
            };

            result.TotalCurrent = appliedVoltage / result.TotalResistance;

            for (int i = 0; i < resistors.Length; i++)
            {
                var analysis = new ResistorAnalysis
                {
                    Resistance = resistors[i],
                    Voltage = appliedVoltage,
                    Current = appliedVoltage / resistors[i],
                    Power = (appliedVoltage * appliedVoltage) / resistors[i],
                    Position = $"R{i + 1}",
                    PowerRating = powerRatings?[i] ?? 0.25 // Default 1/4 watt
                };

                analysis.SafetyMargin = analysis.PowerRating > 0 ?
                    (analysis.PowerRating - analysis.Power) / analysis.PowerRating * 100 : 0;
                analysis.WithinSafeOperatingArea = analysis.Power <= analysis.PowerRating * 0.8; // 80% derating

                result.IndividualResistors.Add(analysis);
            }

            result.TotalPower = result.IndividualResistors.Sum(r => r.Power);

            return result;
        }

        /// <summary>
        /// Calculate series-parallel combination resistance
        /// </summary>
        public static double SeriesParallelCombination(ResistorNetwork network)
        {
            if (network.Groups.Count == 0) return 0;

            var groupResistances = new List<double>();

            foreach (var group in network.Groups)
            {
                double groupResistance = group.Connection == ResistorNetwork.ConnectionType.Series ?
                    Series(group.Resistors.ToArray()) :
                    Parallel(group.Resistors.ToArray());

                groupResistances.Add(groupResistance);
            }

            return network.MainConnection == ResistorNetwork.ConnectionType.Series ?
                Series(groupResistances.ToArray()) :
                Parallel(groupResistances.ToArray());
        }

        /// <summary>
        /// Find resistor combinations to achieve target resistance
        /// </summary>
        public class ResistorCombinationResult
        {
            public double TargetResistance { get; set; }
            public double ActualResistance { get; set; }
            public double Error { get; set; }
            public double ErrorPercent { get; set; }
            public List<double> RequiredResistors { get; set; } = new List<double>();
            public string Configuration { get; set; }
            public bool UsesStandardValues { get; set; }
        }

        /// <summary>
        /// Find best resistor combination for target value using standard values
        /// </summary>
        public static List<ResistorCombinationResult> FindResistorCombinations(double targetResistance,
            string series = "E12", int maxResistors = 3)
        {
            var results = new List<ResistorCombinationResult>();
            var standardValues = GetStandardResistorValues(series);

            // Try single resistor first
            var singleBest = standardValues.OrderBy(r => Math.Abs(r - targetResistance)).First();
            results.Add(new ResistorCombinationResult
            {
                TargetResistance = targetResistance,
                ActualResistance = singleBest,
                Error = singleBest - targetResistance,
                ErrorPercent = (singleBest - targetResistance) / targetResistance * 100,
                RequiredResistors = new List<double> { singleBest },
                Configuration = "Single",
                UsesStandardValues = true
            });

            // Try series combinations
            if (maxResistors >= 2)
            {
                for (int i = 0; i < standardValues.Length; i++)
                {
                    for (int j = i; j < standardValues.Length; j++)
                    {
                        double seriesResistance = standardValues[i] + standardValues[j];
                        double error = Math.Abs(seriesResistance - targetResistance);

                        if (error < Math.Abs(results[0].Error)) // Better than single resistor
                        {
                            results.Add(new ResistorCombinationResult
                            {
                                TargetResistance = targetResistance,
                                ActualResistance = seriesResistance,
                                Error = seriesResistance - targetResistance,
                                ErrorPercent = (seriesResistance - targetResistance) / targetResistance * 100,
                                RequiredResistors = new List<double> { standardValues[i], standardValues[j] },
                                Configuration = "Series",
                                UsesStandardValues = true
                            });
                        }
                    }
                }
            }

            // Try parallel combinations
            if (maxResistors >= 2)
            {
                for (int i = 0; i < standardValues.Length; i++)
                {
                    for (int j = i; j < standardValues.Length; j++)
                    {
                        double parallelResistance = Parallel(standardValues[i], standardValues[j]);
                        double error = Math.Abs(parallelResistance - targetResistance);

                        if (error < Math.Abs(results[0].Error)) // Better than single resistor
                        {
                            results.Add(new ResistorCombinationResult
                            {
                                TargetResistance = targetResistance,
                                ActualResistance = parallelResistance,
                                Error = parallelResistance - targetResistance,
                                ErrorPercent = (parallelResistance - targetResistance) / targetResistance * 100,
                                RequiredResistors = new List<double> { standardValues[i], standardValues[j] },
                                Configuration = "Parallel",
                                UsesStandardValues = true
                            });
                        }
                    }
                }
            }

            return results.OrderBy(r => Math.Abs(r.Error)).Take(10).ToList();
        }

        /// <summary>
        /// Calculate voltage divider with load analysis
        /// </summary>
        public class VoltageDividerResult
        {
            public double OutputVoltageNoLoad { get; set; }
            public double OutputVoltageWithLoad { get; set; }
            public double LoadingError { get; set; }
            public double LoadingErrorPercent { get; set; }
            public double DividerCurrent { get; set; }
            public double LoadCurrent { get; set; }
            public double TotalCurrent { get; set; }
            public double PowerConsumption { get; set; }
            public double Efficiency { get; set; }
            public List<string> Recommendations { get; set; } = new List<string>();
        }

        /// <summary>
        /// Analyze voltage divider with load effects
        /// </summary>
        public static VoltageDividerResult AnalyzeVoltageDivider(double inputVoltage, double r1, double r2,
            double loadResistance = double.PositiveInfinity)
        {
            var result = new VoltageDividerResult();

            // No-load output voltage
            result.OutputVoltageNoLoad = inputVoltage * r2 / (r1 + r2);

            // With load (parallel combination of R2 and load)
            double r2Parallel = double.IsPositiveInfinity(loadResistance) ? r2 : Parallel(r2, loadResistance);
            result.OutputVoltageWithLoad = inputVoltage * r2Parallel / (r1 + r2Parallel);

            // Loading analysis
            result.LoadingError = result.OutputVoltageNoLoad - result.OutputVoltageWithLoad;
            result.LoadingErrorPercent = (result.LoadingError / result.OutputVoltageNoLoad) * 100;

            // Current analysis
            result.DividerCurrent = inputVoltage / (r1 + r2Parallel);
            result.LoadCurrent = double.IsPositiveInfinity(loadResistance) ? 0 :
                result.OutputVoltageWithLoad / loadResistance;
            result.TotalCurrent = result.DividerCurrent;

            // Power analysis
            result.PowerConsumption = inputVoltage * result.DividerCurrent;
            double outputPower = result.OutputVoltageWithLoad * result.LoadCurrent;
            result.Efficiency = outputPower / result.PowerConsumption * 100;

            // Recommendations
            if (result.LoadingErrorPercent > 10)
            {
                result.Recommendations.Add("High loading error - consider using lower value divider resistors");
            }

            if (result.Efficiency < 50)
            {
                result.Recommendations.Add("Low efficiency - consider using a voltage regulator instead");
            }

            if (result.PowerConsumption > 0.1)
            {
                result.Recommendations.Add($"High power consumption ({result.PowerConsumption:0.3}W) - verify thermal design");
            }

            return result;
        }

        /// <summary>
        /// Get standard resistor values for specified series
        /// </summary>
        private static double[] GetStandardResistorValues(string series)
        {
            var baseValues = new Dictionary<string, double[]>
            {
            ["E12"] = new[] { 1.0, 1.2, 1.5, 1.8, 2.2, 2.7, 3.3, 3.9, 4.7, 5.6, 6.8, 8.2 },
            ["E24"] = new[] { 1.0, 1.1, 1.2, 1.3, 1.5, 1.6, 1.8, 2.0, 2.2, 2.4, 2.7, 3.0,
                             3.3, 3.6, 3.9, 4.3, 4.7, 5.1, 5.6, 6.2, 6.8, 7.5, 8.2, 9.1 }
            };

            if (!baseValues.ContainsKey(series))
                series = "E12"; // Default fallback

            var values = new List<double>();
            var baseArray = baseValues[series];

            // Generate values for different decades (1Ω to 10MΩ)
            for (int decade = 0; decade <= 6; decade++)
            {
                double multiplier = Math.Pow(10, decade);
                foreach (var baseValue in baseArray)
                {
                    values.Add(baseValue * multiplier);
                }
            }

            return values.OrderBy(v => v).ToArray();
        }
    }
}
