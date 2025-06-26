using System;

namespace CircuitTool
{
    /// <summary>
    /// Provides calculations for capacitor circuits and characteristics.
    /// </summary>
    public static class CapacitorCalculator
    {
        /// <summary>
        /// Calculates capacitive reactance using the formula Xc = 1 / (2πfC).
        /// </summary>
        /// <param name="frequency">Frequency in hertz (Hz).</param>
        /// <param name="capacitance">Capacitance in farads (F).</param>
        /// <returns>Capacitive reactance in ohms (Ω).</returns>
        public static double CapacitiveReactance(double frequency, double capacitance)
        {
            if (frequency <= 0 || capacitance <= 0)
                throw new ArgumentException("Frequency and capacitance must be positive values.");
            
            return 1.0 / (2 * Math.PI * frequency * capacitance);
        }

        /// <summary>
        /// Calculates the energy stored in a capacitor using the formula E = 0.5 × C × V².
        /// </summary>
        /// <param name="capacitance">Capacitance in farads (F).</param>
        /// <param name="voltage">Voltage across the capacitor in volts (V).</param>
        /// <returns>Energy stored in joules (J).</returns>
        public static double EnergyStored(double capacitance, double voltage)
        {
            if (capacitance < 0 || voltage < 0)
                throw new ArgumentException("Capacitance and voltage must be non-negative values.");
            
            return 0.5 * capacitance * voltage * voltage;
        }

        /// <summary>
        /// Calculates the time constant for an RC circuit using the formula τ = R × C.
        /// </summary>
        /// <param name="resistance">Resistance in ohms (Ω).</param>
        /// <param name="capacitance">Capacitance in farads (F).</param>
        /// <returns>Time constant in seconds (s).</returns>
        public static double TimeConstant(double resistance, double capacitance)
        {
            if (resistance < 0 || capacitance < 0)
                throw new ArgumentException("Resistance and capacitance must be non-negative values.");
            
            return resistance * capacitance;
        }

        /// <summary>
        /// Calculates total capacitance for capacitors in series using the formula 1/Ctotal = 1/C1 + 1/C2 + ...
        /// </summary>
        /// <param name="capacitances">Array of capacitance values in farads (F).</param>
        /// <returns>Total capacitance in farads (F).</returns>
        public static double SeriesCapacitance(double[] capacitances)
        {
            if (capacitances == null || capacitances.Length == 0)
                throw new ArgumentException("Capacitances array cannot be null or empty.");
            
            double reciprocalSum = 0;
            foreach (var capacitance in capacitances)
            {
                if (capacitance <= 0)
                    throw new ArgumentException("All capacitance values must be positive.");
                reciprocalSum += 1.0 / capacitance;
            }
            return 1.0 / reciprocalSum;
        }

        /// <summary>
        /// Calculates total capacitance for capacitors in parallel using the formula Ctotal = C1 + C2 + ...
        /// </summary>
        /// <param name="capacitances">Array of capacitance values in farads (F).</param>
        /// <returns>Total capacitance in farads (F).</returns>
        public static double ParallelCapacitance(double[] capacitances)
        {
            if (capacitances == null || capacitances.Length == 0)
                throw new ArgumentException("Capacitances array cannot be null or empty.");
            
            double totalCapacitance = 0;
            foreach (var capacitance in capacitances)
            {
                if (capacitance < 0)
                    throw new ArgumentException("All capacitance values must be non-negative.");
                totalCapacitance += capacitance;
            }
            return totalCapacitance;
        }

        /// <summary>
        /// Calculates the charging voltage of a capacitor at time t using V(t) = Vsource × (1 - e^(-t/τ)).
        /// </summary>
        /// <param name="sourceVoltage">Source voltage in volts (V).</param>
        /// <param name="timeConstant">Time constant τ in seconds (s).</param>
        /// <param name="time">Time in seconds (s).</param>
        /// <returns>Capacitor voltage at time t in volts (V).</returns>
        public static double ChargingVoltage(double sourceVoltage, double timeConstant, double time)
        {
            if (timeConstant <= 0)
                throw new ArgumentException("Time constant must be positive.");
            if (time < 0)
                throw new ArgumentException("Time must be non-negative.");
            
            return sourceVoltage * (1 - Math.Exp(-time / timeConstant));
        }

        /// <summary>
        /// Calculates the discharging voltage of a capacitor at time t using V(t) = Vinitial × e^(-t/τ).
        /// </summary>
        /// <param name="initialVoltage">Initial voltage in volts (V).</param>
        /// <param name="timeConstant">Time constant τ in seconds (s).</param>
        /// <param name="time">Time in seconds (s).</param>
        /// <returns>Capacitor voltage at time t in volts (V).</returns>
        public static double DischargingVoltage(double initialVoltage, double timeConstant, double time)
        {
            if (timeConstant <= 0)
                throw new ArgumentException("Time constant must be positive.");
            if (time < 0)
                throw new ArgumentException("Time must be non-negative.");
            
            return initialVoltage * Math.Exp(-time / timeConstant);
        }
    }
}
