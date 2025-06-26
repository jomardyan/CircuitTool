using System;

namespace CircuitTool
{
    /// <summary>
    /// Provides easy-to-use methods for common electrical power calculations.
    /// </summary>
    /// <remarks>
    /// <para>Example usage:</para>
    /// <code>
    /// double watts = PowerCalculator.Power(12, 2); // 24W
    /// double watts2 = PowerCalculator.PowerFromCurrentResistance(2, 10); // 40W
    /// double watts3 = PowerCalculator.PowerFromVoltageResistance(12, 6); // 24W
    /// </code>
    /// </remarks>
    public static class PowerCalculator
    {
        /// <summary>
        /// Calculates electrical power using the formula <c>P = V × I</c>.
        /// </summary>
        /// <param name="voltage">The voltage in volts (V).</param>
        /// <param name="current">The current in amperes (A).</param>
        /// <returns>The calculated power in watts (W).</returns>
        /// <example>
        /// double power = PowerCalculator.Power(5, 2); // 10W
        /// </example>
        public static double Power(double voltage, double current) => voltage * current;

        /// <summary>
        /// Calculates electrical power using the formula <c>P = I² × R</c>.
        /// </summary>
        /// <param name="current">The current in amperes (A).</param>
        /// <param name="resistance">The resistance in ohms (Ω).</param>
        /// <returns>The calculated power in watts (W).</returns>
        /// <example>
        /// double power = PowerCalculator.PowerFromCurrentResistance(3, 4); // 36W
        /// </example>
        public static double PowerFromCurrentResistance(double current, double resistance) => current * current * resistance;

        /// <summary>
        /// Calculates electrical power using the formula <c>P = V² / R</c>.
        /// </summary>
        /// <param name="voltage">The voltage in volts (V).</param>
        /// <param name="resistance">The resistance in ohms (Ω).</param>
        /// <returns>The calculated power in watts (W).</returns>
        /// <example>
        /// double power = PowerCalculator.PowerFromVoltageResistance(10, 5); // 20W
        /// </example>
        public static double PowerFromVoltageResistance(double voltage, double resistance) => (voltage * voltage) / resistance;
    }
}
