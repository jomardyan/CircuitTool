#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Numerics;
using CircuitTool.Units;

namespace CircuitTool.Async
{
    /// <summary>
    /// Provides asynchronous versions of complex electrical calculations
    /// </summary>
    public static class AsyncCalculations
    {
        /// <summary>
        /// Performs Monte Carlo analysis of circuit with component tolerances asynchronously
        /// </summary>
        /// <param name="nominalValues">Nominal component values</param>
        /// <param name="tolerances">Component tolerances (e.g., 0.05 for 5%)</param>
        /// <param name="iterations">Number of Monte Carlo iterations</param>
        /// <param name="calculator">Function to calculate circuit response</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Monte Carlo analysis results</returns>
        public static async Task<MonteCarloResult> MonteCarloAnalysisAsync(
            double[] nominalValues,
            double[] tolerances,
            int iterations,
            Func<double[], double> calculator,
            CancellationToken cancellationToken = default)
        {
            return await Task.Run(() =>
            {
                var random = new Random();
                var results = new double[iterations];
                var componentValues = new double[nominalValues.Length];

                for (int i = 0; i < iterations; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    // Generate random component values within tolerances
                    for (int j = 0; j < nominalValues.Length; j++)
                    {
                        var variation = (random.NextDouble() - 0.5) * 2 * tolerances[j];
                        componentValues[j] = nominalValues[j] * (1 + variation);
                    }

                    results[i] = calculator(componentValues);
                }

                return new MonteCarloResult(results);
            }, cancellationToken);
        }

        /// <summary>
        /// Performs frequency sweep analysis asynchronously
        /// </summary>
        /// <param name="startFrequency">Start frequency in Hz</param>
        /// <param name="stopFrequency">Stop frequency in Hz</param>
        /// <param name="points">Number of frequency points</param>
        /// <param name="calculator">Function to calculate response at each frequency</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Frequency response results</returns>
        public static async Task<FrequencyResponse> FrequencySweepAsync(
            double startFrequency,
            double stopFrequency,
            int points,
            Func<double, Complex> calculator,
            CancellationToken cancellationToken = default)
        {
            return await Task.Run(() =>
            {
                var frequencies = new double[points];
                var responses = new Complex[points];

                for (int i = 0; i < points; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var frequency = startFrequency * Math.Pow(stopFrequency / startFrequency, (double)i / (points - 1));
                    frequencies[i] = frequency;
                    responses[i] = calculator(frequency);
                }

                return new FrequencyResponse(frequencies, responses);
            }, cancellationToken);
        }

        /// <summary>
        /// Performs harmonic analysis asynchronously
        /// </summary>
        /// <param name="fundamentalFrequency">Fundamental frequency in Hz</param>
        /// <param name="harmonics">Number of harmonics to analyze</param>
        /// <param name="calculator">Function to calculate response at each harmonic</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Harmonic analysis results</returns>
        public static async Task<HarmonicAnalysis> HarmonicAnalysisAsync(
            double fundamentalFrequency,
            int harmonics,
            Func<double, Complex> calculator,
            CancellationToken cancellationToken = default)
        {
            return await Task.Run(() =>
            {
                var frequencies = new double[harmonics];
                var responses = new Complex[harmonics];

                for (int i = 0; i < harmonics; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var frequency = fundamentalFrequency * (i + 1);
                    frequencies[i] = frequency;
                    responses[i] = calculator(frequency);
                }

                return new HarmonicAnalysis(fundamentalFrequency, frequencies, responses);
            }, cancellationToken);
        }

        /// <summary>
        /// Performs thermal analysis with iterative calculations asynchronously
        /// </summary>
        /// <param name="power">Power dissipation in watts</param>
        /// <param name="thermalResistance">Thermal resistance in °C/W</param>
        /// <param name="ambientTemperature">Ambient temperature in °C</param>
        /// <param name="temperatureCoefficient">Temperature coefficient</param>
        /// <param name="maxIterations">Maximum number of iterations</param>
        /// <param name="tolerance">Convergence tolerance</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Thermal analysis results</returns>
        public static async Task<ThermalResult> ThermalAnalysisAsync(
            double power,
            double thermalResistance,
            double ambientTemperature,
            double temperatureCoefficient,
            int maxIterations = 100,
            double tolerance = 1e-6,
            CancellationToken cancellationToken = default)
        {
            return await Task.Run(() =>
            {
                double temperature = ambientTemperature + power * thermalResistance;
                double previousTemperature;
                int iterations = 0;

                do
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    previousTemperature = temperature;
                    var adjustedPower = power * (1 + temperatureCoefficient * (temperature - 25));
                    temperature = ambientTemperature + adjustedPower * thermalResistance;
                    iterations++;
                } while (Math.Abs(temperature - previousTemperature) > tolerance && iterations < maxIterations);

                return new ThermalResult(temperature, iterations, iterations < maxIterations);
            }, cancellationToken);
        }
    }

    /// <summary>
    /// Results from Monte Carlo analysis
    /// </summary>
    public readonly struct MonteCarloResult
    {
        private readonly double[] _results;

        public MonteCarloResult(double[] results)
        {
            _results = results ?? throw new ArgumentNullException(nameof(results));
            
            Array.Sort(_results);
            
            Mean = _results.Average();
            StandardDeviation = Math.Sqrt(_results.Select(x => Math.Pow(x - Mean, 2)).Average());
            Minimum = _results[0];
            Maximum = _results[_results.Length - 1];
        }

        public double Mean { get; }
        public double StandardDeviation { get; }
        public double Minimum { get; }
        public double Maximum { get; }
        public int SampleCount => _results.Length;

        public double GetPercentile(double percentile)
        {
            if (percentile < 0 || percentile > 100)
                throw new ArgumentOutOfRangeException(nameof(percentile));

            var index = (int)Math.Ceiling(percentile / 100.0 * _results.Length) - 1;
            return _results[Math.Max(0, Math.Min(index, _results.Length - 1))];
        }

        public override string ToString() => 
            $"Mean: {Mean:F6}, StdDev: {StandardDeviation:F6}, Range: [{Minimum:F6}, {Maximum:F6}]";
    }

    /// <summary>
    /// Results from frequency response analysis
    /// </summary>
    public readonly struct FrequencyResponse
    {
        public FrequencyResponse(double[] frequencies, Complex[] responses)
        {
            Frequencies = frequencies ?? throw new ArgumentNullException(nameof(frequencies));
            Responses = responses ?? throw new ArgumentNullException(nameof(responses));

            if (frequencies.Length != responses.Length)
                throw new ArgumentException("Frequencies and responses arrays must have the same length");
        }

        public double[] Frequencies { get; }
        public Complex[] Responses { get; }

        public double[] Magnitudes => Responses.Select(r => r.Magnitude).ToArray();
        public double[] Phases => Responses.Select(r => r.Phase).ToArray();
        public double[] PhasesInDegrees => Responses.Select(r => r.Phase * 180.0 / Math.PI).ToArray();

        public (double frequency, double magnitude) GetMagnitudePeak()
        {
            var maxIndex = 0;
            var maxMagnitude = Responses[0].Magnitude;

            for (int i = 1; i < Responses.Length; i++)
            {
                if (Responses[i].Magnitude > maxMagnitude)
                {
                    maxMagnitude = Responses[i].Magnitude;
                    maxIndex = i;
                }
            }

            return (Frequencies[maxIndex], maxMagnitude);
        }
    }

    /// <summary>
    /// Results from harmonic analysis
    /// </summary>
    public readonly struct HarmonicAnalysis
    {
        public HarmonicAnalysis(double fundamentalFrequency, double[] harmonicFrequencies, Complex[] responses)
        {
            FundamentalFrequency = fundamentalFrequency;
            HarmonicFrequencies = harmonicFrequencies ?? throw new ArgumentNullException(nameof(harmonicFrequencies));
            Responses = responses ?? throw new ArgumentNullException(nameof(responses));
        }

        public double FundamentalFrequency { get; }
        public double[] HarmonicFrequencies { get; }
        public Complex[] Responses { get; }

        public double[] Magnitudes => Responses.Select(r => r.Magnitude).ToArray();
        public double TotalHarmonicDistortion => 
            Math.Sqrt(Responses.Skip(1).Sum(r => r.Magnitude * r.Magnitude)) / Responses[0].Magnitude;
    }

    /// <summary>
    /// Results from thermal analysis
    /// </summary>
    public readonly struct ThermalResult
    {
        public ThermalResult(double temperature, int iterations, bool converged)
        {
            Temperature = temperature;
            Iterations = iterations;
            Converged = converged;
        }

        public double Temperature { get; }
        public int Iterations { get; }
        public bool Converged { get; }

        public override string ToString() => 
            $"Temperature: {Temperature:F2}°C, Iterations: {Iterations}, Converged: {Converged}";
    }
}

// Extension method for older frameworks that don't have LINQ
internal static class EnumerableExtensions
{
    public static double Average(this double[] values)
    {
        if (values.Length == 0) throw new InvalidOperationException("Sequence contains no elements");
        
        double sum = 0;
        for (int i = 0; i < values.Length; i++)
        {
            sum += values[i];
        }
        return sum / values.Length;
    }

    public static TResult[] Select<T, TResult>(this T[] source, Func<T, TResult> selector)
    {
        var result = new TResult[source.Length];
        for (int i = 0; i < source.Length; i++)
        {
            result[i] = selector(source[i]);
        }
        return result;
    }

    public static T[] Skip<T>(this T[] source, int count)
    {
        if (count >= source.Length) return new T[0];
        
        var result = new T[source.Length - count];
        Array.Copy(source, count, result, 0, result.Length);
        return result;
    }

    public static double Sum<T>(this T[] source, Func<T, double> selector)
    {
        double sum = 0;
        for (int i = 0; i < source.Length; i++)
        {
            sum += selector(source[i]);
        }
        return sum;
    }
}
