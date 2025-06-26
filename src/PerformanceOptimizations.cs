#nullable enable
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace CircuitTool.Performance
{
    /// <summary>
    /// Provides performance-optimized calculations using SIMD operations
    /// </summary>
    public static class VectorizedCalculations
    {
        /// <summary>
        /// Calculates multiple parallel resistances using SIMD operations
        /// </summary>
        /// <param name="resistances">Array of resistance values</param>
        /// <returns>Equivalent parallel resistance</returns>
        public static double ParallelResistanceVectorized(double[] resistances)
        {
            if (resistances == null || resistances.Length == 0)
                throw new ArgumentException("Resistances array cannot be null or empty");

            int simdLength = Vector<double>.Count;
            double reciprocalSum = 0.0;
            int i = 0;

            // Process in SIMD chunks
            if (Vector.IsHardwareAccelerated && resistances.Length >= simdLength)
            {
                var reciprocalSumVector = Vector<double>.Zero;

                for (; i <= resistances.Length - simdLength; i += simdLength)
                {
                    var resistanceVector = new Vector<double>(resistances, i);
                    var reciprocalVector = Vector<double>.One / resistanceVector;
                    reciprocalSumVector += reciprocalVector;
                }

                // Sum up the vector components
                for (int j = 0; j < simdLength; j++)
                {
                    reciprocalSum += reciprocalSumVector[j];
                }
            }

            // Process remaining elements
            for (; i < resistances.Length; i++)
            {
                reciprocalSum += 1.0 / resistances[i];
            }

            return 1.0 / reciprocalSum;
        }

        /// <summary>
        /// Calculates RMS values for multiple signals using SIMD operations
        /// </summary>
        /// <param name="signals">Array of signal arrays</param>
        /// <returns>RMS values for each signal</returns>
        public static double[] RMSVectorized(double[][] signals)
        {
            if (signals == null || signals.Length == 0)
                throw new ArgumentException("Signals array cannot be null or empty");

            var results = new double[signals.Length];

            for (int signalIndex = 0; signalIndex < signals.Length; signalIndex++)
            {
                var signal = signals[signalIndex];
                if (signal == null || signal.Length == 0) continue;

                int simdLength = Vector<double>.Count;
                double sumOfSquares = 0.0;
                int i = 0;

                // Process in SIMD chunks
                if (Vector.IsHardwareAccelerated && signal.Length >= simdLength)
                {
                    var sumVector = Vector<double>.Zero;

                    for (; i <= signal.Length - simdLength; i += simdLength)
                    {
                        var valueVector = new Vector<double>(signal, i);
                        var squaredVector = valueVector * valueVector;
                        sumVector += squaredVector;
                    }

                    // Sum up the vector components
                    for (int j = 0; j < simdLength; j++)
                    {
                        sumOfSquares += sumVector[j];
                    }
                }

                // Process remaining elements
                for (; i < signal.Length; i++)
                {
                    sumOfSquares += signal[i] * signal[i];
                }

                results[signalIndex] = Math.Sqrt(sumOfSquares / signal.Length);
            }

            return results;
        }

        /// <summary>
        /// Vectorized impedance magnitude calculation for multiple AC circuits
        /// </summary>
        /// <param name="resistances">Array of resistance values</param>
        /// <param name="reactances">Array of reactance values</param>
        /// <returns>Array of impedance magnitudes</returns>
        public static double[] ImpedanceMagnitudesVectorized(double[] resistances, double[] reactances)
        {
            if (resistances == null || reactances == null)
                throw new ArgumentNullException("Arrays cannot be null");

            if (resistances.Length != reactances.Length)
                throw new ArgumentException("Arrays must have the same length");

            var results = new double[resistances.Length];
            int simdLength = Vector<double>.Count;
            int i = 0;

            // Process in SIMD chunks
            if (Vector.IsHardwareAccelerated && resistances.Length >= simdLength)
            {
                for (; i <= resistances.Length - simdLength; i += simdLength)
                {
                    var rVector = new Vector<double>(resistances, i);
                    var xVector = new Vector<double>(reactances, i);

                    var rSquared = rVector * rVector;
                    var xSquared = xVector * xVector;
                    var magnitudeSquared = rSquared + xSquared;

                    // Calculate square root for each component
                    for (int j = 0; j < simdLength; j++)
                    {
                        results[i + j] = Math.Sqrt(magnitudeSquared[j]);
                    }
                }
            }

            // Process remaining elements
            for (; i < resistances.Length; i++)
            {
                results[i] = Math.Sqrt(resistances[i] * resistances[i] + reactances[i] * reactances[i]);
            }

            return results;
        }

        /// <summary>
        /// Vectorized power calculation for multiple circuits
        /// </summary>
        /// <param name="voltages">Array of voltage values</param>
        /// <param name="currents">Array of current values</param>
        /// <param name="powerFactors">Array of power factor values</param>
        /// <returns>Array of real power values</returns>
        public static double[] RealPowerVectorized(double[] voltages, double[] currents, double[] powerFactors)
        {
            if (voltages == null || currents == null || powerFactors == null)
                throw new ArgumentNullException("Arrays cannot be null");

            if (voltages.Length != currents.Length || voltages.Length != powerFactors.Length)
                throw new ArgumentException("All arrays must have the same length");

            var results = new double[voltages.Length];
            int simdLength = Vector<double>.Count;
            int i = 0;

            // Process in SIMD chunks
            if (Vector.IsHardwareAccelerated && voltages.Length >= simdLength)
            {
                for (; i <= voltages.Length - simdLength; i += simdLength)
                {
                    var vVector = new Vector<double>(voltages, i);
                    var iVector = new Vector<double>(currents, i);
                    var pfVector = new Vector<double>(powerFactors, i);

                    var powerVector = vVector * iVector * pfVector;

                    for (int j = 0; j < simdLength; j++)
                    {
                        results[i + j] = powerVector[j];
                    }
                }
            }

            // Process remaining elements
            for (; i < voltages.Length; i++)
            {
                results[i] = voltages[i] * currents[i] * powerFactors[i];
            }

            return results;
        }
    }

    /// <summary>
    /// Provides caching for expensive electrical calculations
    /// </summary>
    public static class CalculationCache
    {
        private static readonly ConcurrentDictionary<string, object> _cache = new();
        private static readonly object _lockObject = new();
        private static int _maxCacheSize = 1000;

        /// <summary>
        /// Gets or sets the maximum cache size
        /// </summary>
        public static int MaxCacheSize
        {
            get => _maxCacheSize;
            set
            {
                _maxCacheSize = value;
                TrimCache();
            }
        }

        /// <summary>
        /// Gets a cached result or computes and caches it
        /// </summary>
        /// <typeparam name="T">Result type</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="factory">Function to compute the result if not cached</param>
        /// <returns>Cached or computed result</returns>
        public static T GetOrCompute<T>(string key, Func<T> factory)
        {
            if (_cache.TryGetValue(key, out var cachedValue) && cachedValue is T result)
            {
                return result;
            }

            var computedValue = factory();
            
            lock (_lockObject)
            {
                _cache.TryAdd(key, computedValue!);
                TrimCache();
            }

            return computedValue;
        }

        /// <summary>
        /// Creates a cache key from multiple parameters
        /// </summary>
        /// <param name="parameters">Parameters to include in the key</param>
        /// <returns>Cache key string</returns>
        public static string CreateKey(params object[] parameters)
        {
            return string.Join("|", parameters.Select(p => p?.ToString() ?? "null"));
        }

        /// <summary>
        /// Clears the entire cache
        /// </summary>
        public static void Clear()
        {
            _cache.Clear();
        }

        /// <summary>
        /// Removes old entries when cache size exceeds limit
        /// </summary>
        private static void TrimCache()
        {
            if (_cache.Count <= _maxCacheSize) return;

            // Simple LRU-like trimming - remove 25% of entries
            var entriesToRemove = _cache.Count / 4;
            var keysToRemove = _cache.Keys.Take(entriesToRemove).ToList();

            foreach (var key in keysToRemove)
            {
                _cache.TryRemove(key, out _);
            }
        }
    }

    /// <summary>
    /// Provides memory-efficient bulk operations for circuit calculations
    /// </summary>
    public static class BulkOperations
    {
        /// <summary>
        /// Performs bulk parallel resistance calculations with minimal allocations
        /// </summary>
        /// <param name="resistanceSets">Sets of resistances to calculate in parallel</param>
        /// <returns>Equivalent parallel resistances</returns>
        public static IEnumerable<double> BulkParallelResistance(IEnumerable<double[]> resistanceSets)
        {
            foreach (var resistances in resistanceSets)
            {
                yield return VectorizedCalculations.ParallelResistanceVectorized(resistances);
            }
        }

        /// <summary>
        /// Calculates frequency response for multiple circuits efficiently
        /// </summary>
        /// <param name="frequencies">Frequency points</param>
        /// <param name="circuits">Circuit parameters (R, L, C)</param>
        /// <returns>Frequency response magnitudes</returns>
        public static double[][] BulkFrequencyResponse(double[] frequencies, (double R, double L, double C)[] circuits)
        {
            var results = new double[circuits.Length][];

            for (int i = 0; i < circuits.Length; i++)
            {
                var (R, L, C) = circuits[i];
                results[i] = new double[frequencies.Length];

                // Use caching for repeated calculations
                var cacheKey = CalculationCache.CreateKey("FreqResp", R, L, C);
                
                results[i] = CalculationCache.GetOrCompute(cacheKey, () =>
                {
                    var response = new double[frequencies.Length];
                    for (int j = 0; j < frequencies.Length; j++)
                    {
                        var omega = 2 * Math.PI * frequencies[j];
                        var xl = omega * L;
                        var xc = 1.0 / (omega * C);
                        var impedance = Math.Sqrt(R * R + (xl - xc) * (xl - xc));
                        response[j] = 1.0 / impedance; // Admittance magnitude
                    }
                    return response;
                });
            }

            return results;
        }

        /// <summary>
        /// Efficient batch processing of power calculations
        /// </summary>
        /// <param name="voltages">Voltage values</param>
        /// <param name="currents">Current values</param>
        /// <param name="phases">Phase angles in radians</param>
        /// <returns>Complex power values (real, reactive, apparent)</returns>
        public static (double real, double reactive, double apparent)[] BulkPowerCalculation(
            double[] voltages, double[] currents, double[] phases)
        {
            if (voltages.Length != currents.Length || voltages.Length != phases.Length)
                throw new ArgumentException("All arrays must have the same length");

            var results = new (double real, double reactive, double apparent)[voltages.Length];

            // Use vectorized operations where possible
            var realPowers = new double[voltages.Length];
            var reactivePowers = new double[voltages.Length];
            var apparentPowers = new double[voltages.Length];

            int simdLength = Vector<double>.Count;
            int i = 0;

            // Vectorized processing
            if (Vector.IsHardwareAccelerated && voltages.Length >= simdLength)
            {
                for (; i <= voltages.Length - simdLength; i += simdLength)
                {
                    var vVector = new Vector<double>(voltages, i);
                    var iVector = new Vector<double>(currents, i);

                    // Calculate apparent power
                    var apparentVector = vVector * iVector;

                    // Calculate phase cosines and sines (scalar for now)
                    for (int j = 0; j < simdLength && i + j < voltages.Length; j++)
                    {
                        var phase = phases[i + j];
                        var apparent = apparentVector[j];
                        
                        apparentPowers[i + j] = apparent;
                        realPowers[i + j] = apparent * Math.Cos(phase);
                        reactivePowers[i + j] = apparent * Math.Sin(phase);
                    }
                }
            }

            // Process remaining elements
            for (; i < voltages.Length; i++)
            {
                var apparent = voltages[i] * currents[i];
                apparentPowers[i] = apparent;
                realPowers[i] = apparent * Math.Cos(phases[i]);
                reactivePowers[i] = apparent * Math.Sin(phases[i]);
            }

            // Combine results
            for (i = 0; i < voltages.Length; i++)
            {
                results[i] = (realPowers[i], reactivePowers[i], apparentPowers[i]);
            }

            return results;
        }

        /// <summary>
        /// Memory-efficient streaming calculation for large datasets
        /// </summary>
        /// <param name="data">Input data stream</param>
        /// <param name="calculator">Calculation function</param>
        /// <param name="batchSize">Size of processing batches</param>
        /// <returns>Results stream</returns>
        public static IEnumerable<TResult> StreamingCalculation<TInput, TResult>(
            IEnumerable<TInput> data,
            Func<TInput[], TResult[]> calculator,
            int batchSize = 1024)
        {
            var batch = new List<TInput>(batchSize);

            foreach (var item in data)
            {
                batch.Add(item);

                if (batch.Count >= batchSize)
                {
                    var results = calculator(batch.ToArray());
                    foreach (var result in results)
                    {
                        yield return result;
                    }
                    batch.Clear();
                }
            }

            // Process remaining items
            if (batch.Count > 0)
            {
                var results = calculator(batch.ToArray());
                foreach (var result in results)
                {
                    yield return result;
                }
            }
        }
    }

    /// <summary>
    /// Performance monitoring and optimization utilities
    /// </summary>
    public static class PerformanceMonitor
    {
        private static readonly ConcurrentDictionary<string, (long count, long totalMs)> _metrics = new();

        /// <summary>
        /// Measures and records execution time for a function
        /// </summary>
        /// <typeparam name="T">Return type</typeparam>
        /// <param name="operationName">Name of the operation for tracking</param>
        /// <param name="operation">Function to execute and measure</param>
        /// <returns>Result of the operation</returns>
        public static T MeasureOperation<T>(string operationName, Func<T> operation)
        {
            var stopwatch = Stopwatch.StartNew();
            var result = operation();
            stopwatch.Stop();
            var elapsedMs = stopwatch.ElapsedMilliseconds;

            _metrics.AddOrUpdate(operationName,
                (1, elapsedMs),
                (key, value) => (value.count + 1, value.totalMs + elapsedMs));

            return result;
        }

        /// <summary>
        /// Gets performance statistics for an operation
        /// </summary>
        /// <param name="operationName">Name of the operation</param>
        /// <returns>Performance statistics</returns>
        public static (long callCount, double averageMs, long totalMs)? GetStats(string operationName)
        {
            if (_metrics.TryGetValue(operationName, out var stats))
            {
                return (stats.count, (double)stats.totalMs / stats.count, stats.totalMs);
            }
            return null;
        }

        /// <summary>
        /// Gets all performance statistics
        /// </summary>
        /// <returns>Dictionary of all performance metrics</returns>
        public static Dictionary<string, (long callCount, double averageMs, long totalMs)> GetAllStats()
        {
            var result = new Dictionary<string, (long callCount, double averageMs, long totalMs)>();
            
            foreach (var kvp in _metrics)
            {
                var stats = kvp.Value;
                result[kvp.Key] = (stats.count, (double)stats.totalMs / stats.count, stats.totalMs);
            }

            return result;
        }

        /// <summary>
        /// Clears all performance metrics
        /// </summary>
        public static void ClearStats()
        {
            _metrics.Clear();
        }
    }
}
