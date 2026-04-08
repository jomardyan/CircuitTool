#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace CircuitTool.Performance
{
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
}
