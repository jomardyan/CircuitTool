#nullable enable
using System;
using System.Collections.Generic;
using CircuitTool.Async;

namespace CircuitTool.Analysis
{
    /// <summary>
    /// Provides tolerance and error analysis for electrical circuits
    /// </summary>
    public static class ToleranceAnalysis
    {
        /// <summary>
        /// Performs worst-case analysis for circuit parameters
        /// </summary>
        /// <param name="nominalValues">Nominal component values</param>
        /// <param name="tolerances">Component tolerances (as fractions)</param>
        /// <param name="calculator">Function to calculate circuit response</param>
        /// <returns>Worst-case analysis results</returns>
        public static WorstCaseResult WorstCaseAnalysis(
            double[] nominalValues,
            double[] tolerances,
            Func<double[], double> calculator)
        {
            if (nominalValues.Length != tolerances.Length)
                throw new ArgumentException("Nominal values and tolerances arrays must have the same length");

            var minResult = double.MaxValue;
            var maxResult = double.MinValue;
            var nominalResult = calculator(nominalValues);

            // Generate all possible combinations of min/max values
            var combinations = GenerateWorstCaseCombinations(nominalValues, tolerances);

            foreach (var combination in combinations)
            {
                var result = calculator(combination);
                minResult = Math.Min(minResult, result);
                maxResult = Math.Max(maxResult, result);
            }

            return new WorstCaseResult(nominalResult, minResult, maxResult);
        }

        /// <summary>
        /// Calculates sensitivity of output to each component
        /// </summary>
        /// <param name="nominalValues">Nominal component values</param>
        /// <param name="calculator">Function to calculate circuit response</param>
        /// <param name="deltaPercent">Percentage change for sensitivity calculation</param>
        /// <returns>Sensitivity analysis results</returns>
        public static SensitivityResult SensitivityAnalysis(
            double[] nominalValues,
            Func<double[], double> calculator,
            double deltaPercent = 1.0)
        {
            var nominalResult = calculator(nominalValues);
            var sensitivities = new double[nominalValues.Length];

            for (int i = 0; i < nominalValues.Length; i++)
            {
                var testValues = new double[nominalValues.Length];
                Array.Copy(nominalValues, testValues, nominalValues.Length);

                // Calculate sensitivity using finite difference
                var delta = nominalValues[i] * deltaPercent / 100.0;
                testValues[i] = nominalValues[i] + delta;
                var resultPlus = calculator(testValues);

                testValues[i] = nominalValues[i] - delta;
                var resultMinus = calculator(testValues);

                // Sensitivity = (ΔOutput / Output) / (ΔInput / Input)
                var outputChange = (resultPlus - resultMinus) / 2.0;
                var inputChange = 2.0 * delta;
                
                sensitivities[i] = (outputChange / nominalResult) / (inputChange / nominalValues[i]);
            }

            return new SensitivityResult(nominalResult, sensitivities);
        }

        /// <summary>
        /// Calculates statistical analysis using component tolerances
        /// </summary>
        /// <param name="nominalValues">Nominal component values</param>
        /// <param name="tolerances">Component tolerances (as fractions)</param>
        /// <param name="calculator">Function to calculate circuit response</param>
        /// <returns>Statistical analysis results</returns>
        public static StatisticalResult StatisticalAnalysis(
            double[] nominalValues,
            double[] tolerances,
            Func<double[], double> calculator)
        {
            var sensitivities = SensitivityAnalysis(nominalValues, calculator).Sensitivities;
            var nominalResult = calculator(nominalValues);

            // Root sum of squares method for statistical analysis
            var variance = 0.0;
            for (int i = 0; i < nominalValues.Length; i++)
            {
                var componentVariance = Math.Pow(sensitivities[i] * tolerances[i] * nominalResult, 2);
                variance += componentVariance;
            }

            var standardDeviation = Math.Sqrt(variance);
            var toleranceEstimate = standardDeviation / nominalResult; // As a fraction

            return new StatisticalResult(nominalResult, standardDeviation, toleranceEstimate);
        }

        /// <summary>
        /// Performs design centering to minimize sensitivity
        /// </summary>
        /// <param name="initialValues">Initial component values</param>
        /// <param name="tolerances">Component tolerances</param>
        /// <param name="calculator">Function to calculate circuit response</param>
        /// <param name="targetValue">Target output value</param>
        /// <param name="maxIterations">Maximum optimization iterations</param>
        /// <returns>Design centering results</returns>
        public static DesignCenteringResult DesignCentering(
            double[] initialValues,
            double[] tolerances,
            Func<double[], double> calculator,
            double targetValue,
            int maxIterations = 100)
        {
            var currentValues = new double[initialValues.Length];
            Array.Copy(initialValues, currentValues, initialValues.Length);

            var bestValues = new double[initialValues.Length];
            var bestYield = 0.0;

            for (int iteration = 0; iteration < maxIterations; iteration++)
            {
                // Calculate current yield using Monte Carlo
                var monteCarloResult = AsyncCalculations.MonteCarloAnalysisAsync(
                    currentValues, tolerances, 1000, calculator).Result;

                // Calculate yield (percentage within 3-sigma of target)
                var tolerance3Sigma = 3 * monteCarloResult.StandardDeviation;
                var lowerBound = targetValue - tolerance3Sigma;
                var upperBound = targetValue + tolerance3Sigma;
                
                var yield = CalculateYield(monteCarloResult, lowerBound, upperBound);

                if (yield > bestYield)
                {
                    bestYield = yield;
                    Array.Copy(currentValues, bestValues, currentValues.Length);
                }

                // Simple gradient-based optimization
                var sensitivities = SensitivityAnalysis(currentValues, calculator).Sensitivities;
                
                for (int i = 0; i < currentValues.Length; i++)
                {
                    var adjustment = 0.01 * sensitivities[i] * (targetValue - calculator(currentValues));
                    currentValues[i] += adjustment;
                }
            }

            return new DesignCenteringResult(bestValues, bestYield, maxIterations);
        }

        private static double CalculateYield(MonteCarloResult result, double lowerBound, double upperBound)
        {
            // Simplified yield calculation - in practice would use actual sample data
            var mean = result.Mean;
            var sigma = result.StandardDeviation;

            // Estimate using normal distribution
            var lowerZ = (lowerBound - mean) / sigma;
            var upperZ = (upperBound - mean) / sigma;

            // Simplified normal CDF approximation
            return ApproxNormalCDF(upperZ) - ApproxNormalCDF(lowerZ);
        }

        private static double ApproxNormalCDF(double z)
        {
            // Approximation of standard normal CDF
            return 0.5 * (1 + Math.Sign(z) * Math.Sqrt(1 - Math.Exp(-2 * z * z / Math.PI)));
        }

        private static IEnumerable<double[]> GenerateWorstCaseCombinations(double[] nominalValues, double[] tolerances)
        {
            var count = 1 << nominalValues.Length; // 2^n combinations
            
            for (int i = 0; i < count; i++)
            {
                var combination = new double[nominalValues.Length];
                
                for (int j = 0; j < nominalValues.Length; j++)
                {
                    var useMax = (i & (1 << j)) != 0;
                    var tolerance = tolerances[j];
                    
                    combination[j] = useMax 
                        ? nominalValues[j] * (1 + tolerance)
                        : nominalValues[j] * (1 - tolerance);
                }
                
                yield return combination;
            }
        }
    }

    /// <summary>
    /// Results from worst-case analysis
    /// </summary>
    public readonly struct WorstCaseResult
    {
        public double NominalValue { get; }
        public double MinimumValue { get; }
        public double MaximumValue { get; }
        public double Range => MaximumValue - MinimumValue;
        public double TolerancePercent => (Range / 2) / Math.Abs(NominalValue) * 100;

        public WorstCaseResult(double nominalValue, double minimumValue, double maximumValue)
        {
            NominalValue = nominalValue;
            MinimumValue = minimumValue;
            MaximumValue = maximumValue;
        }

        public override string ToString() => 
            $"Nominal: {NominalValue:F6}, Range: [{MinimumValue:F6}, {MaximumValue:F6}], Tolerance: ±{TolerancePercent:F2}%";
    }

    /// <summary>
    /// Results from sensitivity analysis
    /// </summary>
    public readonly struct SensitivityResult
    {
        public double NominalValue { get; }
        public double[] Sensitivities { get; }

        public SensitivityResult(double nominalValue, double[] sensitivities)
        {
            NominalValue = nominalValue;
            Sensitivities = sensitivities ?? throw new ArgumentNullException(nameof(sensitivities));
        }

        /// <summary>
        /// Gets the index of the component with highest sensitivity
        /// </summary>
        public int MostSensitiveComponent
        {
            get
            {
                var maxIndex = 0;
                var maxSensitivity = Math.Abs(Sensitivities[0]);
                
                for (int i = 1; i < Sensitivities.Length; i++)
                {
                    var sensitivity = Math.Abs(Sensitivities[i]);
                    if (sensitivity > maxSensitivity)
                    {
                        maxSensitivity = sensitivity;
                        maxIndex = i;
                    }
                }
                
                return maxIndex;
            }
        }

        public override string ToString() => 
            $"Nominal: {NominalValue:F6}, Most sensitive component: {MostSensitiveComponent} (S={Sensitivities[MostSensitiveComponent]:F4})";
    }

    /// <summary>
    /// Results from statistical analysis
    /// </summary>
    public readonly struct StatisticalResult
    {
        public double NominalValue { get; }
        public double StandardDeviation { get; }
        public double ToleranceEstimate { get; }
        public double TolerancePercent => ToleranceEstimate * 100;

        public StatisticalResult(double nominalValue, double standardDeviation, double toleranceEstimate)
        {
            NominalValue = nominalValue;
            StandardDeviation = standardDeviation;
            ToleranceEstimate = toleranceEstimate;
        }

        /// <summary>
        /// Gets the 3-sigma bounds (99.7% confidence)
        /// </summary>
        public (double lower, double upper) ThreeSigmaBounds => 
            (NominalValue - 3 * StandardDeviation, NominalValue + 3 * StandardDeviation);

        /// <summary>
        /// Gets the 6-sigma bounds (99.9999% confidence)
        /// </summary>
        public (double lower, double upper) SixSigmaBounds => 
            (NominalValue - 6 * StandardDeviation, NominalValue + 6 * StandardDeviation);

        public override string ToString() => 
            $"Nominal: {NominalValue:F6}, σ: {StandardDeviation:F6}, Tolerance: ±{TolerancePercent:F2}%";
    }

    /// <summary>
    /// Results from design centering optimization
    /// </summary>
    public readonly struct DesignCenteringResult
    {
        public double[] OptimizedValues { get; }
        public double Yield { get; }
        public int Iterations { get; }

        public DesignCenteringResult(double[] optimizedValues, double yield, int iterations)
        {
            OptimizedValues = optimizedValues ?? throw new ArgumentNullException(nameof(optimizedValues));
            Yield = yield;
            Iterations = iterations;
        }

        public double YieldPercent => Yield * 100;

        public override string ToString() => 
            $"Yield: {YieldPercent:F2}%, Iterations: {Iterations}";
    }

    /// <summary>
    /// Component tolerance standards for common component types
    /// </summary>
    public static class StandardTolerances
    {
        /// <summary>
        /// Standard resistor tolerances
        /// </summary>
        public static class Resistor
        {
            public const double E96_1Percent = 0.01;      // ±1%
            public const double E48_2Percent = 0.02;      // ±2%
            public const double E24_5Percent = 0.05;      // ±5%
            public const double E12_10Percent = 0.10;     // ±10%
            public const double E6_20Percent = 0.20;      // ±20%
        }

        /// <summary>
        /// Standard capacitor tolerances
        /// </summary>
        public static class Capacitor
        {
            public const double C0G_5Percent = 0.05;      // ±5% (C0G/NP0)
            public const double C0G_10Percent = 0.10;     // ±10% (C0G/NP0)
            public const double X7R_10Percent = 0.10;     // ±10% (X7R)
            public const double X7R_20Percent = 0.20;     // ±20% (X7R)
            public const double Electrolytic_20Percent = 0.20; // ±20% (Electrolytic)
            public const double Electrolytic_Minus20Plus80 = 0.50; // -20%/+80% (Electrolytic)
        }

        /// <summary>
        /// Standard inductor tolerances
        /// </summary>
        public static class Inductor
        {
            public const double Precision_1Percent = 0.01;  // ±1%
            public const double Standard_5Percent = 0.05;   // ±5%
            public const double Standard_10Percent = 0.10;  // ±10%
            public const double Standard_20Percent = 0.20;  // ±20%
        }
    }
}
