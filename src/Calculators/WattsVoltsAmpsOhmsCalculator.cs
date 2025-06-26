using System;

namespace CircuitTool
{
    /// <summary>
    /// Provides quick conversions between watts, volts, amps, and ohms.
    /// </summary>
    /// <remarks>
    /// <para>Example usage:</para>
    /// <code>
    /// double w = WattsVoltsAmpsOhmsCalculator.Watts(12, 2); // 24W
    /// double v = WattsVoltsAmpsOhmsCalculator.Volts(24, 2); // 12V
    /// double a = WattsVoltsAmpsOhmsCalculator.Amps(24, 12); // 2A
    /// double r = WattsVoltsAmpsOhmsCalculator.Ohms(12, 2); // 6 Ohms
    /// </code>
    /// </remarks>
    public static class WattsVoltsAmpsOhmsCalculator
    {
        /// <summary>
        /// Calculates power in watts from volts and amps: <c>W = V × A</c>.
        /// </summary>
        /// <param name="volts">Voltage in volts (V).</param>
        /// <param name="amps">Current in amperes (A).</param>
        /// <returns>Power in watts (W).</returns>
        public static double Watts(double volts, double amps) => volts * amps;

        /// <summary>
        /// Calculates voltage from watts and amps: <c>V = W / A</c>.
        /// </summary>
        /// <param name="watts">Power in watts (W).</param>
        /// <param name="amps">Current in amperes (A).</param>
        /// <returns>Voltage in volts (V).</returns>
        public static double Volts(double watts, double amps) => watts / amps;

        /// <summary>
        /// Calculates current from watts and volts: <c>A = W / V</c>.
        /// </summary>
        /// <param name="watts">Power in watts (W).</param>
        /// <param name="volts">Voltage in volts (V).</param>
        /// <returns>Current in amperes (A).</returns>
        public static double Amps(double watts, double volts) => watts / volts;

        /// <summary>
        /// Calculates resistance from volts and amps: <c>R = V / A</c>.
        /// </summary>
        /// <param name="volts">Voltage in volts (V).</param>
        /// <param name="amps">Current in amperes (A).</param>
        /// <returns>Resistance in ohms (Ω).</returns>
        public static double Ohms(double volts, double amps) => volts / amps;
    }
}
