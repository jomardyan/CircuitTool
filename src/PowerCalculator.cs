using System;

namespace CircuitTool
{
    /// <summary>
    /// Provides electrical power calculations using various formulas.
    /// </summary>
    public static class PowerCalculator
    {
        /// <summary>
        /// Calculates power using the formula P = V × I.
        /// </summary>
        /// <param name="voltage">Voltage in volts (V).</param>
        /// <param name="current">Current in amperes (A).</param>
        /// <returns>Power in watts (W).</returns>
        public static double Power(double voltage, double current) 
        {
            return voltage * current;
        }

        /// <summary>
        /// Calculates power using the formula P = I² × R.
        /// </summary>
        /// <param name="current">Current in amperes (A).</param>
        /// <param name="resistance">Resistance in ohms (Ω).</param>
        /// <returns>Power in watts (W).</returns>
        public static double PowerFromCurrentResistance(double current, double resistance) 
        {
            return current * current * resistance;
        }

        /// <summary>
        /// Calculates power using the formula P = V² / R.
        /// </summary>
        /// <param name="voltage">Voltage in volts (V).</param>
        /// <param name="resistance">Resistance in ohms (Ω).</param>
        /// <returns>Power in watts (W).</returns>
        public static double PowerFromVoltageResistance(double voltage, double resistance) 
        {
            return (voltage * voltage) / resistance;
        }
    }
}
