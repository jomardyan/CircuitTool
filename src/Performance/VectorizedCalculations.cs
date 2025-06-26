#nullable enable
using System;
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
}
