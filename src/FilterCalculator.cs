using System;

namespace CircuitTool
{
    /// <summary>
    /// Provides calculations for electronic filter design and analysis.
    /// </summary>
    public static class FilterCalculator
    {
        /// <summary>
        /// Calculates the cutoff frequency of an RC low-pass filter using fc = 1 / (2πRC).
        /// </summary>
        /// <param name="resistance">Resistance in ohms (Ω).</param>
        /// <param name="capacitance">Capacitance in farads (F).</param>
        /// <returns>Cutoff frequency in hertz (Hz).</returns>
        public static double RCLowPassCutoffFrequency(double resistance, double capacitance)
        {
            if (resistance <= 0 || capacitance <= 0)
                throw new ArgumentException("Resistance and capacitance must be positive values.");
            
            return 1.0 / (2 * Math.PI * resistance * capacitance);
        }

        /// <summary>
        /// Calculates the cutoff frequency of an RC high-pass filter using fc = 1 / (2πRC).
        /// </summary>
        /// <param name="resistance">Resistance in ohms (Ω).</param>
        /// <param name="capacitance">Capacitance in farads (F).</param>
        /// <returns>Cutoff frequency in hertz (Hz).</returns>
        public static double RCHighPassCutoffFrequency(double resistance, double capacitance)
        {
            if (resistance <= 0 || capacitance <= 0)
                throw new ArgumentException("Resistance and capacitance must be positive values.");
            
            return 1.0 / (2 * Math.PI * resistance * capacitance);
        }

        /// <summary>
        /// Calculates the cutoff frequency of an RL low-pass filter using fc = R / (2πL).
        /// </summary>
        /// <param name="resistance">Resistance in ohms (Ω).</param>
        /// <param name="inductance">Inductance in henries (H).</param>
        /// <returns>Cutoff frequency in hertz (Hz).</returns>
        public static double RLLowPassCutoffFrequency(double resistance, double inductance)
        {
            if (resistance <= 0 || inductance <= 0)
                throw new ArgumentException("Resistance and inductance must be positive values.");
            
            return resistance / (2 * Math.PI * inductance);
        }

        /// <summary>
        /// Calculates the cutoff frequency of an RL high-pass filter using fc = R / (2πL).
        /// </summary>
        /// <param name="resistance">Resistance in ohms (Ω).</param>
        /// <param name="inductance">Inductance in henries (H).</param>
        /// <returns>Cutoff frequency in hertz (Hz).</returns>
        public static double RLHighPassCutoffFrequency(double resistance, double inductance)
        {
            if (resistance <= 0 || inductance <= 0)
                throw new ArgumentException("Resistance and inductance must be positive values.");
            
            return resistance / (2 * Math.PI * inductance);
        }

        /// <summary>
        /// Calculates the gain of a filter in decibels using Gain(dB) = 20 × log10(Vout/Vin).
        /// </summary>
        /// <param name="outputVoltage">Output voltage in volts (V).</param>
        /// <param name="inputVoltage">Input voltage in volts (V).</param>
        /// <returns>Gain in decibels (dB).</returns>
        public static double GainInDecibels(double outputVoltage, double inputVoltage)
        {
            if (inputVoltage <= 0)
                throw new ArgumentException("Input voltage must be positive.");
            if (outputVoltage <= 0)
                throw new ArgumentException("Output voltage must be positive.");
            
            return 20 * Math.Log10(outputVoltage / inputVoltage);
        }

        /// <summary>
        /// Calculates the phase shift of an RC low-pass filter using φ = -arctan(2πfRC).
        /// </summary>
        /// <param name="frequency">Frequency in hertz (Hz).</param>
        /// <param name="resistance">Resistance in ohms (Ω).</param>
        /// <param name="capacitance">Capacitance in farads (F).</param>
        /// <returns>Phase shift in degrees.</returns>
        public static double RCLowPassPhaseShift(double frequency, double resistance, double capacitance)
        {
            if (frequency < 0 || resistance <= 0 || capacitance <= 0)
                throw new ArgumentException("Frequency must be non-negative, resistance and capacitance must be positive.");
            
            return -Math.Atan(2 * Math.PI * frequency * resistance * capacitance) * (180.0 / Math.PI);
        }

        /// <summary>
        /// Calculates the phase shift of an RC high-pass filter using φ = arctan(1/(2πfRC)).
        /// </summary>
        /// <param name="frequency">Frequency in hertz (Hz).</param>
        /// <param name="resistance">Resistance in ohms (Ω).</param>
        /// <param name="capacitance">Capacitance in farads (F).</param>
        /// <returns>Phase shift in degrees.</returns>
        public static double RCHighPassPhaseShift(double frequency, double resistance, double capacitance)
        {
            if (frequency <= 0 || resistance <= 0 || capacitance <= 0)
                throw new ArgumentException("Frequency, resistance, and capacitance must be positive values.");
            
            return Math.Atan(1.0 / (2 * Math.PI * frequency * resistance * capacitance)) * (180.0 / Math.PI);
        }

        /// <summary>
        /// Calculates the magnitude response of an RC low-pass filter using |H(f)| = 1 / √(1 + (f/fc)²).
        /// </summary>
        /// <param name="frequency">Frequency in hertz (Hz).</param>
        /// <param name="cutoffFrequency">Cutoff frequency in hertz (Hz).</param>
        /// <returns>Magnitude response (unitless, between 0 and 1).</returns>
        public static double RCLowPassMagnitudeResponse(double frequency, double cutoffFrequency)
        {
            if (frequency < 0 || cutoffFrequency <= 0)
                throw new ArgumentException("Frequency must be non-negative and cutoff frequency must be positive.");
            
            double ratio = frequency / cutoffFrequency;
            return 1.0 / Math.Sqrt(1 + ratio * ratio);
        }

        /// <summary>
        /// Calculates the magnitude response of an RC high-pass filter using |H(f)| = (f/fc) / √(1 + (f/fc)²).
        /// </summary>
        /// <param name="frequency">Frequency in hertz (Hz).</param>
        /// <param name="cutoffFrequency">Cutoff frequency in hertz (Hz).</param>
        /// <returns>Magnitude response (unitless, between 0 and 1).</returns>
        public static double RCHighPassMagnitudeResponse(double frequency, double cutoffFrequency)
        {
            if (frequency < 0 || cutoffFrequency <= 0)
                throw new ArgumentException("Frequency must be non-negative and cutoff frequency must be positive.");
            
            double ratio = frequency / cutoffFrequency;
            return ratio / Math.Sqrt(1 + ratio * ratio);
        }

        /// <summary>
        /// Calculates the required capacitance for a desired RC filter cutoff frequency.
        /// </summary>
        /// <param name="cutoffFrequency">Desired cutoff frequency in hertz (Hz).</param>
        /// <param name="resistance">Resistance in ohms (Ω).</param>
        /// <returns>Required capacitance in farads (F).</returns>
        public static double RequiredCapacitanceForCutoff(double cutoffFrequency, double resistance)
        {
            if (cutoffFrequency <= 0 || resistance <= 0)
                throw new ArgumentException("Cutoff frequency and resistance must be positive values.");
            
            return 1.0 / (2 * Math.PI * cutoffFrequency * resistance);
        }

        /// <summary>
        /// Calculates the required resistance for a desired RC filter cutoff frequency.
        /// </summary>
        /// <param name="cutoffFrequency">Desired cutoff frequency in hertz (Hz).</param>
        /// <param name="capacitance">Capacitance in farads (F).</param>
        /// <returns>Required resistance in ohms (Ω).</returns>
        public static double RequiredResistanceForCutoff(double cutoffFrequency, double capacitance)
        {
            if (cutoffFrequency <= 0 || capacitance <= 0)
                throw new ArgumentException("Cutoff frequency and capacitance must be positive values.");
            
            return 1.0 / (2 * Math.PI * cutoffFrequency * capacitance);
        }
    }
}
