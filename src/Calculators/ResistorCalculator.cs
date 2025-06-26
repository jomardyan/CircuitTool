using System;
#if !NET20
using System.Linq;
#endif

namespace CircuitTool
{
    /// <summary>
    /// Provides methods for resistor calculations, including Ohm's Law, series, and parallel combinations.
    /// </summary>
    /// <remarks>
    /// <para>Example usage:</para>
    /// <code>
    /// double r = ResistorCalculator.Resistance(10, 2); // 5 Ohms
    /// double series = ResistorCalculator.Series(10, 20, 30); // 60 Ohms
    /// double parallel = ResistorCalculator.Parallel(10, 20); // 6.67 Ohms
    /// </code>
    /// </remarks>
    public static class ResistorCalculator
    {
        /// <summary>
        /// Calculates resistance using Ohm's Law: <c>R = V / I</c>.
        /// </summary>
        /// <param name="voltage">Voltage in volts (V).</param>
        /// <param name="current">Current in amperes (A).</param>
        /// <returns>Resistance in ohms (Ω).</returns>
        /// <example>
        /// double r = ResistorCalculator.Resistance(10, 2); // 5 Ohms
        /// </example>
        public static double Resistance(double voltage, double current) => voltage / current;

        /// <summary>
        /// Calculates total resistance for resistors in series: <c>Rtotal = R1 + R2 + ...</c>
        /// </summary>
        /// <param name="resistors">Resistor values in ohms (Ω).</param>
        /// <returns>Total series resistance in ohms (Ω).</returns>
        /// <example>
        /// double total = ResistorCalculator.Series(10, 20, 30); // 60 Ohms
        /// </example>
        public static double Series(params double[] resistors)
        {
            if (resistors == null) return 0;
#if NET20
            double total = 0;
            foreach (double resistor in resistors)
            {
                total += resistor;
            }
            return total;
#else
            return resistors.Sum();
#endif
        }

        /// <summary>
        /// Calculates total resistance for resistors in parallel: <c>1/Rtotal = 1/R1 + 1/R2 + ...</c>
        /// </summary>
        /// <param name="resistors">Resistor values in ohms (Ω).</param>
        /// <returns>Total parallel resistance in ohms (Ω).</returns>
        /// <example>
        /// double total = ResistorCalculator.Parallel(10, 20); // 6.67 Ohms
        /// </example>
        public static double Parallel(params double[] resistors)
        {
            if (resistors == null) return 0;
#if NET20
            double reciprocalSum = 0;
            foreach (double resistor in resistors)
            {
                reciprocalSum += 1.0 / resistor;
            }
            return 1.0 / reciprocalSum;
#else
            return 1.0 / resistors.Sum(r => 1.0 / r);
#endif
        }
    }
}
