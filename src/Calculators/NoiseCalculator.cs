using System;

namespace CircuitTool
{
    /// <summary>
    /// Provides calculations for noise analysis in electronic circuits.
    /// </summary>
    /// <remarks>
    /// <para>Example usage:</para>
    /// <code>
    /// double thermal = NoiseCalculator.ThermalNoise(1000, 10000, 300); // Thermal noise
    /// double shot = NoiseCalculator.ShotNoise(1e-3, 1000); // Shot noise
    /// double total = NoiseCalculator.TotalNoise(new[] {1e-9, 2e-9, 1.5e-9}); // Total noise
    /// </code>
    /// </remarks>
    public static class NoiseCalculator
    {
        private const double BoltzmannConstant = 1.380649e-23; // J/K
        private const double ElectronCharge = 1.602176634e-19; // C

        /// <summary>
        /// Calculates thermal (Johnson) noise voltage.
        /// </summary>
        /// <param name="resistance">Resistance in ohms (Ω).</param>
        /// <param name="bandwidth">Bandwidth in Hz.</param>
        /// <param name="temperature">Temperature in Kelvin (default: 290K).</param>
        /// <returns>RMS noise voltage in volts (V).</returns>
        /// <example>
        /// double thermal = NoiseCalculator.ThermalNoise(1000, 10000, 300); // Thermal noise
        /// </example>
        public static double ThermalNoise(double resistance, double bandwidth, double temperature = 290)
        {
            if (resistance < 0 || bandwidth <= 0 || temperature <= 0)
                throw new ArgumentException("Resistance must be non-negative, bandwidth and temperature must be positive.");
            
            return Math.Sqrt(4 * BoltzmannConstant * temperature * resistance * bandwidth);
        }

        /// <summary>
        /// Calculates shot noise current.
        /// </summary>
        /// <param name="current">DC current in amperes (A).</param>
        /// <param name="bandwidth">Bandwidth in Hz.</param>
        /// <returns>RMS noise current in amperes (A).</returns>
        /// <example>
        /// double shot = NoiseCalculator.ShotNoise(1e-3, 1000); // Shot noise
        /// </example>
        public static double ShotNoise(double current, double bandwidth)
        {
            if (current < 0 || bandwidth <= 0)
                throw new ArgumentException("Current must be non-negative and bandwidth must be positive.");
            
            return Math.Sqrt(2 * ElectronCharge * current * bandwidth);
        }

        /// <summary>
        /// Calculates flicker (1/f) noise voltage.
        /// </summary>
        /// <param name="current">DC current in amperes (A).</param>
        /// <param name="flickerCoefficient">Flicker noise coefficient (device dependent).</param>
        /// <param name="frequency">Frequency in Hz.</param>
        /// <param name="bandwidth">Bandwidth in Hz.</param>
        /// <returns>RMS noise voltage in volts (V).</returns>
        public static double FlickerNoise(double current, double flickerCoefficient, double frequency, double bandwidth)
        {
            if (current < 0 || flickerCoefficient < 0 || frequency <= 0 || bandwidth <= 0)
                throw new ArgumentException("All parameters must be non-negative (frequency and bandwidth must be positive).");
            
            return Math.Sqrt(flickerCoefficient * current * bandwidth / frequency);
        }

        /// <summary>
        /// Calculates total noise from multiple uncorrelated noise sources.
        /// </summary>
        /// <param name="noiseSources">Array of RMS noise values in the same units.</param>
        /// <returns>Total RMS noise in the same units.</returns>
        /// <example>
        /// double total = NoiseCalculator.TotalNoise(new[] {1e-9, 2e-9, 1.5e-9}); // Total noise
        /// </example>
        public static double TotalNoise(double[] noiseSources)
        {
            if (noiseSources == null || noiseSources.Length == 0)
                throw new ArgumentException("Noise sources array cannot be null or empty.");
            
            double sumOfSquares = 0;
            foreach (var noise in noiseSources)
            {
                if (noise < 0)
                    throw new ArgumentException("All noise values must be non-negative.");
                sumOfSquares += noise * noise;
            }
            
            return Math.Sqrt(sumOfSquares);
        }

        /// <summary>
        /// Calculates noise factor (linear) from noise figure (dB).
        /// </summary>
        /// <param name="noiseFigureDb">Noise figure in dB.</param>
        /// <returns>Noise factor (linear).</returns>
        public static double NoiseFactorFromDb(double noiseFigureDb)
        {
            return Math.Pow(10, noiseFigureDb / 10);
        }

        /// <summary>
        /// Calculates noise figure (dB) from noise factor (linear).
        /// </summary>
        /// <param name="noiseFactor">Noise factor (linear, ≥ 1).</param>
        /// <returns>Noise figure in dB.</returns>
        public static double NoiseFigureFromFactor(double noiseFactor)
        {
            if (noiseFactor < 1)
                throw new ArgumentException("Noise factor must be 1 or greater.");
            
            return 10 * Math.Log10(noiseFactor);
        }

        /// <summary>
        /// Calculates cascaded noise figure for multiple stages.
        /// </summary>
        /// <param name="noiseFactors">Array of noise factors (linear) for each stage.</param>
        /// <param name="gains">Array of power gains (linear) for each stage.</param>
        /// <returns>Overall noise factor (linear).</returns>
        public static double CascadedNoiseFactor(double[] noiseFactors, double[] gains)
        {
            if (noiseFactors == null || gains == null)
                throw new ArgumentException("Arrays cannot be null.");
            if (noiseFactors.Length != gains.Length || noiseFactors.Length == 0)
                throw new ArgumentException("Arrays must have the same non-zero length.");
            
            double totalNoiseFactor = noiseFactors[0];
            double cumulativeGain = gains[0];
            
            for (int i = 1; i < noiseFactors.Length; i++)
            {
                if (noiseFactors[i] < 1 || gains[i] <= 0)
                    throw new ArgumentException($"Noise factor must be ≥ 1 and gain must be positive at stage {i}.");
                
                totalNoiseFactor += (noiseFactors[i] - 1) / cumulativeGain;
                cumulativeGain *= gains[i];
            }
            
            return totalNoiseFactor;
        }

        /// <summary>
        /// Calculates input-referred noise voltage of an amplifier.
        /// </summary>
        /// <param name="outputNoiseVoltage">Output noise voltage in volts (V).</param>
        /// <param name="gain">Voltage gain (linear).</param>
        /// <returns>Input-referred noise voltage in volts (V).</returns>
        public static double InputReferredNoiseVoltage(double outputNoiseVoltage, double gain)
        {
            if (outputNoiseVoltage < 0 || gain <= 0)
                throw new ArgumentException("Output noise voltage must be non-negative and gain must be positive.");
            
            return outputNoiseVoltage / gain;
        }

        /// <summary>
        /// Calculates input-referred noise current of an amplifier.
        /// </summary>
        /// <param name="outputNoiseCurrent">Output noise current in amperes (A).</param>
        /// <param name="gain">Current gain (linear).</param>
        /// <returns>Input-referred noise current in amperes (A).</returns>
        public static double InputReferredNoiseCurrent(double outputNoiseCurrent, double gain)
        {
            if (outputNoiseCurrent < 0 || gain <= 0)
                throw new ArgumentException("Output noise current must be non-negative and gain must be positive.");
            
            return outputNoiseCurrent / gain;
        }

        /// <summary>
        /// Calculates noise bandwidth from 3dB bandwidth.
        /// </summary>
        /// <param name="bandwidth3dB">3dB bandwidth in Hz.</param>
        /// <param name="filterOrder">Filter order (1 for single pole, 2 for second order, etc.).</param>
        /// <returns>Noise bandwidth in Hz.</returns>
        public static double NoiseBandwidth(double bandwidth3dB, int filterOrder)
        {
            if (bandwidth3dB <= 0)
                throw new ArgumentException("3dB bandwidth must be positive.");
            if (filterOrder <= 0)
                throw new ArgumentException("Filter order must be positive.");
            
            // Approximations for common filter types
            switch (filterOrder)
            {
                case 1:
                    return Math.PI * bandwidth3dB / 2; // Single pole
                case 2:
                    return bandwidth3dB; // Second order (approximately)
                default:
                    return bandwidth3dB * Math.PI / (2 * filterOrder); // Higher order approximation
            }
        }

        /// <summary>
        /// Calculates signal-to-noise ratio in dB.
        /// </summary>
        /// <param name="signalPower">Signal power in watts or any consistent unit.</param>
        /// <param name="noisePower">Noise power in the same unit as signal power.</param>
        /// <returns>SNR in dB.</returns>
        public static double SignalToNoiseRatio(double signalPower, double noisePower)
        {
            if (signalPower < 0 || noisePower <= 0)
                throw new ArgumentException("Signal power must be non-negative and noise power must be positive.");
            
            return 10 * Math.Log10(signalPower / noisePower);
        }

        /// <summary>
        /// Calculates equivalent noise bandwidth for a given filter response.
        /// </summary>
        /// <param name="dcGain">DC gain of the filter (linear).</param>
        /// <param name="totalNoisePower">Total integrated noise power.</param>
        /// <param name="noiseSpectralDensity">Noise spectral density at DC.</param>
        /// <returns>Equivalent noise bandwidth in Hz.</returns>
        public static double EquivalentNoiseBandwidth(double dcGain, double totalNoisePower, double noiseSpectralDensity)
        {
            if (dcGain <= 0 || totalNoisePower < 0 || noiseSpectralDensity <= 0)
                throw new ArgumentException("DC gain and noise spectral density must be positive, total noise power must be non-negative.");
            
            return totalNoisePower / (dcGain * dcGain * noiseSpectralDensity);
        }
    }
}
