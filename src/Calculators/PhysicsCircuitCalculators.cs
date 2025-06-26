using System;

namespace CircuitTool
{
    /// <summary>
    /// Provides additional calculators for electrical engineering and physics, including impedance, resonance, Q factor, and more.
    /// </summary>
    /// <remarks>
    /// <para>Example usage:</para>
    /// <code>
    /// double xl = PhysicsCircuitCalculators.InductiveReactance(1000, 0.01); // 62.83 Ohms
    /// double zrlc = PhysicsCircuitCalculators.SeriesRLCImpedance(100, 0.01, 0.000001, 1000); // 100.0 Ohms
    /// double fres = PhysicsCircuitCalculators.ResonantFrequencyLC(0.01, 0.000001); // 1591.55 Hz
    /// double q = PhysicsCircuitCalculators.QFactor(1000, 0.01, 100); // 0.628
    /// double energyL = PhysicsCircuitCalculators.EnergyStoredInductor(0.01, 2); // 0.02 J
    /// double pf = PhysicsCircuitCalculators.PowerFactor(100, 80); // 0.8
    /// double rms = PhysicsCircuitCalculators.RMS(new[] {1.0, 2.0, 3.0}); // 2.16
    /// </code>
    /// </remarks>
    public static class PhysicsCircuitCalculators
    {
        /// <summary>
        /// Calculates inductive reactance (Xl = 2πfL).
        /// </summary>
        /// <param name="frequency">Frequency in hertz (Hz).</param>
        /// <param name="inductance">Inductance in henries (H).</param>
        /// <returns>Inductive reactance in ohms (Ω).</returns>
        public static double InductiveReactance(double frequency, double inductance)
        {
            if (frequency < 0 || inductance < 0)
                throw new ArgumentException("Frequency and inductance must be non-negative.");
            return 2 * Math.PI * frequency * inductance;
        }

        /// <summary>
        /// Calculates impedance of a series RL circuit (Z = sqrt(R^2 + (Xl)^2)).
        /// </summary>
        /// <param name="resistance">Resistance in ohms (Ω).</param>
        /// <param name="inductance">Inductance in henries (H).</param>
        /// <param name="frequency">Frequency in hertz (Hz).</param>
        /// <returns>Impedance in ohms (Ω).</returns>
        public static double SeriesRLImpedance(double resistance, double inductance, double frequency)
        {
            double xl = InductiveReactance(frequency, inductance);
            return Math.Sqrt(resistance * resistance + xl * xl);
        }

        /// <summary>
        /// Calculates impedance of a series RC circuit (Z = sqrt(R^2 + (Xc)^2)).
        /// </summary>
        /// <param name="resistance">Resistance in ohms (Ω).</param>
        /// <param name="capacitance">Capacitance in farads (F).</param>
        /// <param name="frequency">Frequency in hertz (Hz).</param>
        /// <returns>Impedance in ohms (Ω).</returns>
        public static double SeriesRCImpedance(double resistance, double capacitance, double frequency)
        {
            double xc = 1.0 / (2 * Math.PI * frequency * capacitance);
            return Math.Sqrt(resistance * resistance + xc * xc);
        }

        /// <summary>
        /// Calculates impedance of a series RLC circuit (Z = sqrt(R^2 + (Xl - Xc)^2)).
        /// </summary>
        /// <param name="resistance">Resistance in ohms (Ω).</param>
        /// <param name="inductance">Inductance in henries (H).</param>
        /// <param name="capacitance">Capacitance in farads (F).</param>
        /// <param name="frequency">Frequency in hertz (Hz).</param>
        /// <returns>Impedance in ohms (Ω).</returns>
        public static double SeriesRLCImpedance(double resistance, double inductance, double capacitance, double frequency)
        {
            double xl = InductiveReactance(frequency, inductance);
            double xc = 1.0 / (2 * Math.PI * frequency * capacitance);
            return Math.Sqrt(resistance * resistance + Math.Pow(xl - xc, 2));
        }

        /// <summary>
        /// Calculates resonant frequency for an LC circuit (f = 1 / (2π√(LC))).
        /// </summary>
        /// <param name="inductance">Inductance in henries (H).</param>
        /// <param name="capacitance">Capacitance in farads (F).</param>
        /// <returns>Resonant frequency in hertz (Hz).</returns>
        public static double ResonantFrequencyLC(double inductance, double capacitance)
        {
            if (inductance <= 0 || capacitance <= 0)
                throw new ArgumentException("Inductance and capacitance must be positive.");
            return 1.0 / (2 * Math.PI * Math.Sqrt(inductance * capacitance));
        }

        /// <summary>
        /// Calculates Q factor for a series RLC circuit (Q = 1/R * sqrt(L/C)).
        /// </summary>
        /// <param name="resistance">Resistance in ohms (Ω).</param>
        /// <param name="inductance">Inductance in henries (H).</param>
        /// <param name="capacitance">Capacitance in farads (F).</param>
        /// <returns>Q factor (dimensionless).</returns>
        public static double QFactor(double resistance, double inductance, double capacitance)
        {
            if (resistance <= 0 || inductance <= 0 || capacitance <= 0)
                throw new ArgumentException("All parameters must be positive.");
            return (1.0 / resistance) * Math.Sqrt(inductance / capacitance);
        }

        /// <summary>
        /// Calculates the energy stored in an inductor (E = 0.5 × L × I²).
        /// </summary>
        /// <param name="inductance">Inductance in henries (H).</param>
        /// <param name="current">Current in amperes (A).</param>
        /// <returns>Energy in joules (J).</returns>
        public static double EnergyStoredInductor(double inductance, double current)
        {
            if (inductance < 0 || current < 0)
                throw new ArgumentException("Inductance and current must be non-negative.");
            return 0.5 * inductance * current * current;
        }

        /// <summary>
        /// Calculates the power factor (PF = real power / apparent power).
        /// </summary>
        /// <param name="realPower">Real power in watts (W).</param>
        /// <param name="apparentPower">Apparent power in volt-amperes (VA).</param>
        /// <returns>Power factor (0 to 1).</returns>
        public static double PowerFactor(double realPower, double apparentPower)
        {
            if (apparentPower == 0)
                throw new ArgumentException("Apparent power must not be zero.");
            return realPower / apparentPower;
        }

        /// <summary>
        /// Calculates the root mean square (RMS) value of a set of values.
        /// </summary>
        /// <param name="values">Array of values.</param>
        /// <returns>RMS value.</returns>
        public static double RMS(double[] values)
        {
            if (values == null || values.Length == 0)
                throw new ArgumentException("Values array cannot be null or empty.");
            double sumSquares = 0;
            foreach (var v in values)
                sumSquares += v * v;
            return Math.Sqrt(sumSquares / values.Length);
        }
    }
}
