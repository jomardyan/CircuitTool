using System;

namespace CircuitTool
{
    /// <summary>
    /// Provides calculations for inductor circuits and characteristics.
    /// </summary>
    public static class InductorCalculator
    {
        /// <summary>
        /// Calculates inductive reactance using the formula XL = 2πfL.
        /// </summary>
        /// <param name="frequency">Frequency in hertz (Hz).</param>
        /// <param name="inductance">Inductance in henries (H).</param>
        /// <returns>Inductive reactance in ohms (Ω).</returns>
        public static double InductiveReactance(double frequency, double inductance)
        {
            if (frequency < 0 || inductance < 0)
                throw new ArgumentException("Frequency and inductance must be non-negative values.");
            
            return 2 * Math.PI * frequency * inductance;
        }

        /// <summary>
        /// Calculates the energy stored in an inductor using the formula E = 0.5 × L × I².
        /// </summary>
        /// <param name="inductance">Inductance in henries (H).</param>
        /// <param name="current">Current through the inductor in amperes (A).</param>
        /// <returns>Energy stored in joules (J).</returns>
        public static double EnergyStored(double inductance, double current)
        {
            if (inductance < 0 || current < 0)
                throw new ArgumentException("Inductance and current must be non-negative values.");
            
            return 0.5 * inductance * current * current;
        }

        /// <summary>
        /// Calculates the time constant for an RL circuit using the formula τ = L / R.
        /// </summary>
        /// <param name="inductance">Inductance in henries (H).</param>
        /// <param name="resistance">Resistance in ohms (Ω).</param>
        /// <returns>Time constant in seconds (s).</returns>
        public static double TimeConstant(double inductance, double resistance)
        {
            if (inductance < 0 || resistance <= 0)
                throw new ArgumentException("Inductance must be non-negative and resistance must be positive.");
            
            return inductance / resistance;
        }

        /// <summary>
        /// Calculates total inductance for inductors in series using the formula Ltotal = L1 + L2 + ...
        /// </summary>
        /// <param name="inductances">Array of inductance values in henries (H).</param>
        /// <returns>Total inductance in henries (H).</returns>
        public static double SeriesInductance(double[] inductances)
        {
            if (inductances == null || inductances.Length == 0)
                throw new ArgumentException("Inductances array cannot be null or empty.");
            
            double totalInductance = 0;
            foreach (var inductance in inductances)
            {
                if (inductance < 0)
                    throw new ArgumentException("All inductance values must be non-negative.");
                totalInductance += inductance;
            }
            return totalInductance;
        }

        /// <summary>
        /// Calculates total inductance for inductors in parallel using the formula 1/Ltotal = 1/L1 + 1/L2 + ...
        /// </summary>
        /// <param name="inductances">Array of inductance values in henries (H).</param>
        /// <returns>Total inductance in henries (H).</returns>
        public static double ParallelInductance(double[] inductances)
        {
            if (inductances == null || inductances.Length == 0)
                throw new ArgumentException("Inductances array cannot be null or empty.");
            
            double reciprocalSum = 0;
            foreach (var inductance in inductances)
            {
                if (inductance <= 0)
                    throw new ArgumentException("All inductance values must be positive.");
                reciprocalSum += 1.0 / inductance;
            }
            return 1.0 / reciprocalSum;
        }

        /// <summary>
        /// Calculates the current buildup in an inductor at time t using I(t) = Ifinal × (1 - e^(-t/τ)).
        /// </summary>
        /// <param name="finalCurrent">Final steady-state current in amperes (A).</param>
        /// <param name="timeConstant">Time constant τ in seconds (s).</param>
        /// <param name="time">Time in seconds (s).</param>
        /// <returns>Inductor current at time t in amperes (A).</returns>
        public static double CurrentBuildup(double finalCurrent, double timeConstant, double time)
        {
            if (timeConstant <= 0)
                throw new ArgumentException("Time constant must be positive.");
            if (time < 0)
                throw new ArgumentException("Time must be non-negative.");
            
            return finalCurrent * (1 - Math.Exp(-time / timeConstant));
        }

        /// <summary>
        /// Calculates the current decay in an inductor at time t using I(t) = Iinitial × e^(-t/τ).
        /// </summary>
        /// <param name="initialCurrent">Initial current in amperes (A).</param>
        /// <param name="timeConstant">Time constant τ in seconds (s).</param>
        /// <param name="time">Time in seconds (s).</param>
        /// <returns>Inductor current at time t in amperes (A).</returns>
        public static double CurrentDecay(double initialCurrent, double timeConstant, double time)
        {
            if (timeConstant <= 0)
                throw new ArgumentException("Time constant must be positive.");
            if (time < 0)
                throw new ArgumentException("Time must be non-negative.");
            
            return initialCurrent * Math.Exp(-time / timeConstant);
        }

        /// <summary>
        /// Calculates the resonant frequency of an LC circuit using f = 1 / (2π√(LC)).
        /// </summary>
        /// <param name="inductance">Inductance in henries (H).</param>
        /// <param name="capacitance">Capacitance in farads (F).</param>
        /// <returns>Resonant frequency in hertz (Hz).</returns>
        public static double ResonantFrequency(double inductance, double capacitance)
        {
            if (inductance <= 0 || capacitance <= 0)
                throw new ArgumentException("Inductance and capacitance must be positive values.");
            
            return 1.0 / (2 * Math.PI * Math.Sqrt(inductance * capacitance));
        }
    }
}
