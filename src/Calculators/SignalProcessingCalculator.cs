using System;
using System.Linq;

namespace CircuitTool
{
    /// <summary>
    /// Provides calculations for signal processing applications.
    /// </summary>
    /// <remarks>
    /// <para>Example usage:</para>
    /// <code>
    /// double snr = SignalProcessingCalculator.SNR(100, 10); // 20 dB
    /// double thd = SignalProcessingCalculator.THD(100, new[] {5.0, 3.0, 2.0}); // Total Harmonic Distortion
    /// double bw = SignalProcessingCalculator.NoiseBandwidth(1000, 0.707); // Noise bandwidth
    /// </code>
    /// </remarks>
    public static class SignalProcessingCalculator
    {
        /// <summary>
        /// Calculates Signal-to-Noise Ratio (SNR) in dB.
        /// </summary>
        /// <param name="signalPower">Signal power in watts or any unit.</param>
        /// <param name="noisePower">Noise power in the same unit as signal power.</param>
        /// <returns>SNR in dB.</returns>
        /// <example>
        /// double snr = SignalProcessingCalculator.SNR(100, 10); // 20 dB
        /// </example>
        public static double SNR(double signalPower, double noisePower)
        {
            if (signalPower <= 0 || noisePower <= 0)
                throw new ArgumentException("Signal and noise power must be positive.");
            return 10 * Math.Log10(signalPower / noisePower);
        }

        /// <summary>
        /// Calculates Signal-to-Noise and Distortion Ratio (SINAD) in dB.
        /// </summary>
        /// <param name="signalPower">Signal power in watts or any unit.</param>
        /// <param name="noiseAndDistortionPower">Combined noise and distortion power.</param>
        /// <returns>SINAD in dB.</returns>
        public static double SINAD(double signalPower, double noiseAndDistortionPower)
        {
            if (signalPower <= 0 || noiseAndDistortionPower <= 0)
                throw new ArgumentException("Signal and noise+distortion power must be positive.");
            return 10 * Math.Log10(signalPower / noiseAndDistortionPower);
        }

        /// <summary>
        /// Calculates Total Harmonic Distortion (THD) as a percentage.
        /// </summary>
        /// <param name="fundamentalAmplitude">Amplitude of the fundamental frequency.</param>
        /// <param name="harmonicAmplitudes">Array of harmonic amplitudes.</param>
        /// <returns>THD as a percentage.</returns>
        /// <example>
        /// double thd = SignalProcessingCalculator.THD(100, new[] {5.0, 3.0, 2.0}); // Total Harmonic Distortion
        /// </example>
        public static double THD(double fundamentalAmplitude, double[] harmonicAmplitudes)
        {
            if (fundamentalAmplitude <= 0)
                throw new ArgumentException("Fundamental amplitude must be positive.");
            if (harmonicAmplitudes == null || harmonicAmplitudes.Length == 0)
                throw new ArgumentException("Harmonic amplitudes array cannot be null or empty.");

            double harmonicPowerSum = harmonicAmplitudes.Sum(h => h * h);
            double fundamentalPower = fundamentalAmplitude * fundamentalAmplitude;
            
            return 100 * Math.Sqrt(harmonicPowerSum / fundamentalPower);
        }

        /// <summary>
        /// Calculates the dynamic range in dB.
        /// </summary>
        /// <param name="maxSignal">Maximum signal level.</param>
        /// <param name="noiseFloor">Noise floor level.</param>
        /// <returns>Dynamic range in dB.</returns>
        public static double DynamicRange(double maxSignal, double noiseFloor)
        {
            if (maxSignal <= 0 || noiseFloor <= 0)
                throw new ArgumentException("Signal levels must be positive.");
            return 20 * Math.Log10(maxSignal / noiseFloor);
        }

        /// <summary>
        /// Calculates the effective number of bits (ENOB) for an ADC.
        /// </summary>
        /// <param name="sinad">SINAD value in dB.</param>
        /// <returns>Effective number of bits.</returns>
        public static double ENOB(double sinad)
        {
            return (sinad - 1.76) / 6.02;
        }

        /// <summary>
        /// Calculates noise bandwidth for a given filter.
        /// </summary>
        /// <param name="centerFrequency">Center frequency in Hz.</param>
        /// <param name="qualityFactor">Quality factor (Q) of the filter.</param>
        /// <returns>Noise bandwidth in Hz.</returns>
        /// <example>
        /// double bw = SignalProcessingCalculator.NoiseBandwidth(1000, 0.707); // Noise bandwidth
        /// </example>
        public static double NoiseBandwidth(double centerFrequency, double qualityFactor)
        {
            if (centerFrequency <= 0 || qualityFactor <= 0)
                throw new ArgumentException("Center frequency and quality factor must be positive.");
            return centerFrequency / qualityFactor;
        }

        /// <summary>
        /// Calculates the settling time for a step response.
        /// </summary>
        /// <param name="timeConstant">Time constant in seconds.</param>
        /// <param name="accuracyPercent">Desired accuracy as a percentage (e.g., 1 for 1%).</param>
        /// <returns>Settling time in seconds.</returns>
        public static double SettlingTime(double timeConstant, double accuracyPercent)
        {
            if (timeConstant <= 0 || accuracyPercent <= 0 || accuracyPercent >= 100)
                throw new ArgumentException("Time constant must be positive and accuracy must be between 0 and 100.");
            return -timeConstant * Math.Log(accuracyPercent / 100);
        }

        /// <summary>
        /// Calculates the Johnson noise voltage for a resistor.
        /// </summary>
        /// <param name="resistance">Resistance in ohms (Ω).</param>
        /// <param name="bandwidth">Bandwidth in Hz.</param>
        /// <param name="temperature">Temperature in Kelvin (default: 290K).</param>
        /// <returns>RMS noise voltage in volts (V).</returns>
        public static double JohnsonNoise(double resistance, double bandwidth, double temperature = 290)
        {
            if (resistance < 0 || bandwidth <= 0 || temperature <= 0)
                throw new ArgumentException("Resistance must be non-negative, bandwidth and temperature must be positive.");
            const double kB = 1.380649e-23; // Boltzmann constant
            return Math.Sqrt(4 * kB * temperature * resistance * bandwidth);
        }

        /// <summary>
        /// Calculates the slew rate required for a given frequency and amplitude.
        /// </summary>
        /// <param name="frequency">Frequency in Hz.</param>
        /// <param name="amplitude">Peak amplitude in volts.</param>
        /// <returns>Required slew rate in V/s.</returns>
        public static double SlewRate(double frequency, double amplitude)
        {
            if (frequency <= 0 || amplitude < 0)
                throw new ArgumentException("Frequency must be positive and amplitude must be non-negative.");
            return 2 * Math.PI * frequency * amplitude;
        }
    }
}
