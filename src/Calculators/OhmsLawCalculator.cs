using System;

namespace CircuitTool
{
    /// <summary>
    /// Provides calculations for Ohm's Law (V = I × R).
    /// </summary>
    public static class OhmsLawCalculator
    {
        /// <summary>
        /// Calculates voltage using Ohm's Law (V = I × R).
        /// </summary>
        /// <param name="current">Current in amperes (A).</param>
        /// <param name="resistance">Resistance in ohms (Ω).</param>
        /// <returns>Voltage in volts (V).</returns>
        public static double Voltage(double current, double resistance)
        {
            if (resistance < 0)
                throw new ArgumentException("Resistance must be non-negative.");

            return current * resistance;
        }

        /// <summary>
        /// Calculates current using Ohm's Law (I = V / R).
        /// </summary>
        /// <param name="voltage">Voltage in volts (V).</param>
        /// <param name="resistance">Resistance in ohms (Ω).</param>
        /// <returns>Current in amperes (A).</returns>
        public static double Current(double voltage, double resistance)
        {
            if (resistance <= 0)
                throw new ArgumentException("Resistance must be greater than zero.");

            return voltage / resistance;
        }

        /// <summary>
        /// Calculates resistance using Ohm's Law (R = V / I).
        /// </summary>
        /// <param name="voltage">Voltage in volts (V).</param>
        /// <param name="current">Current in amperes (A).</param>
        /// <returns>Resistance in ohms (Ω).</returns>
        public static double Resistance(double voltage, double current)
        {
            if (current == 0)
                throw new ArgumentException("Current cannot be zero.");

            return voltage / current;
        }
    }
}
