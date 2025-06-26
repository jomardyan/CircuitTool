using System;

namespace CircuitTool
{
    /// <summary>
    /// Provides calculations for AC circuit analysis and characteristics.
    /// </summary>
    public static class ACCircuitCalculator
    {
        /// <summary>
        /// Calculates the impedance magnitude of an RLC circuit using Z = √(R² + (XL - XC)²).
        /// </summary>
        /// <param name="resistance">Resistance in ohms (Ω).</param>
        /// <param name="inductiveReactance">Inductive reactance in ohms (Ω).</param>
        /// <param name="capacitiveReactance">Capacitive reactance in ohms (Ω).</param>
        /// <returns>Impedance magnitude in ohms (Ω).</returns>
        public static double ImpedanceMagnitude(double resistance, double inductiveReactance, double capacitiveReactance)
        {
            if (resistance < 0)
                throw new ArgumentException("Resistance must be non-negative.");
            
            double reactance = inductiveReactance - capacitiveReactance;
            return Math.Sqrt(resistance * resistance + reactance * reactance);
        }

        /// <summary>
        /// Calculates the phase angle of an RLC circuit using φ = arctan((XL - XC) / R).
        /// </summary>
        /// <param name="resistance">Resistance in ohms (Ω).</param>
        /// <param name="inductiveReactance">Inductive reactance in ohms (Ω).</param>
        /// <param name="capacitiveReactance">Capacitive reactance in ohms (Ω).</param>
        /// <returns>Phase angle in degrees.</returns>
        public static double PhaseAngle(double resistance, double inductiveReactance, double capacitiveReactance)
        {
            double reactance = inductiveReactance - capacitiveReactance;
            
            if (resistance == 0)
            {
                // Special case: purely reactive circuit
                return reactance > 0 ? 90.0 : (reactance < 0 ? -90.0 : 0.0);
            }
            
            return Math.Atan(reactance / resistance) * (180.0 / Math.PI);
        }

        /// <summary>
        /// Calculates the power factor using cos(φ) = R / Z.
        /// </summary>
        /// <param name="resistance">Resistance in ohms (Ω).</param>
        /// <param name="impedanceMagnitude">Impedance magnitude in ohms (Ω).</param>
        /// <returns>Power factor (unitless, between 0 and 1).</returns>
        public static double PowerFactor(double resistance, double impedanceMagnitude)
        {
            if (resistance < 0)
                throw new ArgumentException("Resistance must be non-negative.");
            if (impedanceMagnitude <= 0)
                throw new ArgumentException("Impedance magnitude must be positive.");
            
            return resistance / impedanceMagnitude;
        }

        /// <summary>
        /// Calculates the RMS value from peak value (RMS = Peak / √2).
        /// </summary>
        /// <param name="peakValue">Peak value.</param>
        /// <returns>RMS value.</returns>
        public static double PeakToRMS(double peakValue)
        {
            return peakValue / Math.Sqrt(2);
        }

        /// <summary>
        /// Calculates the peak value from RMS value (Peak = RMS × √2).
        /// </summary>
        /// <param name="rmsValue">RMS value.</param>
        /// <returns>Peak value.</returns>
        public static double RMSToPeak(double rmsValue)
        {
            return rmsValue * Math.Sqrt(2);
        }

        /// <summary>
        /// Calculates the average value of a sinusoidal waveform (Average = 2 × Peak / π).
        /// </summary>
        /// <param name="peakValue">Peak value.</param>
        /// <returns>Average value.</returns>
        public static double PeakToAverage(double peakValue)
        {
            return (2 * peakValue) / Math.PI;
        }

        /// <summary>
        /// Calculates the peak-to-peak value from peak value (Peak-to-Peak = 2 × Peak).
        /// </summary>
        /// <param name="peakValue">Peak value.</param>
        /// <returns>Peak-to-peak value.</returns>
        public static double PeakToPeakToPeak(double peakValue)
        {
            return 2 * peakValue;
        }

        /// <summary>
        /// Calculates the form factor of a waveform (Form Factor = RMS / Average).
        /// </summary>
        /// <param name="rmsValue">RMS value.</param>
        /// <param name="averageValue">Average value.</param>
        /// <returns>Form factor (unitless).</returns>
        public static double FormFactor(double rmsValue, double averageValue)
        {
            if (averageValue == 0)
                throw new ArgumentException("Average value cannot be zero.");
            
            return rmsValue / averageValue;
        }

        /// <summary>
        /// Calculates the crest factor of a waveform (Crest Factor = Peak / RMS).
        /// </summary>
        /// <param name="peakValue">Peak value.</param>
        /// <param name="rmsValue">RMS value.</param>
        /// <returns>Crest factor (unitless).</returns>
        public static double CrestFactor(double peakValue, double rmsValue)
        {
            if (rmsValue == 0)
                throw new ArgumentException("RMS value cannot be zero.");
            
            return peakValue / rmsValue;
        }

        /// <summary>
        /// Calculates the quality factor (Q) of a resonant circuit using Q = XL / R.
        /// </summary>
        /// <param name="inductiveReactance">Inductive reactance in ohms (Ω).</param>
        /// <param name="resistance">Resistance in ohms (Ω).</param>
        /// <returns>Quality factor (unitless).</returns>
        public static double QualityFactor(double inductiveReactance, double resistance)
        {
            if (resistance <= 0)
                throw new ArgumentException("Resistance must be positive.");
            if (inductiveReactance < 0)
                throw new ArgumentException("Inductive reactance must be non-negative.");
            
            return inductiveReactance / resistance;
        }

        /// <summary>
        /// Calculates the bandwidth of a resonant circuit using BW = f0 / Q.
        /// </summary>
        /// <param name="resonantFrequency">Resonant frequency in hertz (Hz).</param>
        /// <param name="qualityFactor">Quality factor (unitless).</param>
        /// <returns>Bandwidth in hertz (Hz).</returns>
        public static double Bandwidth(double resonantFrequency, double qualityFactor)
        {
            if (resonantFrequency <= 0)
                throw new ArgumentException("Resonant frequency must be positive.");
            if (qualityFactor <= 0)
                throw new ArgumentException("Quality factor must be positive.");
            
            return resonantFrequency / qualityFactor;
        }
    }
}
