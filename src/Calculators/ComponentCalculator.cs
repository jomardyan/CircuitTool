using System;
using System.Linq;

namespace CircuitTool.Calculators
{
    /// <summary>
    /// Provides calculations for resistors, capacitors, and inductors.
    /// </summary>
    public static class ComponentCalculator
    {
        // =====================
        // Resistor Calculations
        // =====================
        public static double Resistance(double voltage, double current)
        {
            return voltage / current;
        }

        public static double SeriesResistance(params double[] resistors)
        {
            if (resistors == null) return 0;
            return resistors.Sum();
        }

        public static double ParallelResistance(params double[] resistors)
        {
            if (resistors == null) return 0;
            return 1.0 / resistors.Sum(r => 1.0 / r);
        }

        // =====================
        // Capacitor Calculations
        // =====================
        public static double CapacitiveReactance(double frequency, double capacitance)
        {
            if (frequency <= 0 || capacitance <= 0)
                throw new ArgumentException("Frequency and capacitance must be positive values.");
            return 1.0 / (2 * Math.PI * frequency * capacitance);
        }

        public static double CapacitorEnergyStored(double capacitance, double voltage)
        {
            if (capacitance < 0 || voltage < 0)
                throw new ArgumentException("Capacitance and voltage must be non-negative values.");
            return 0.5 * capacitance * voltage * voltage;
        }

        public static double RCTimeConstant(double resistance, double capacitance)
        {
            if (resistance < 0 || capacitance < 0)
                throw new ArgumentException("Resistance and capacitance must be non-negative values.");
            return resistance * capacitance;
        }

        // =====================
        // Inductor Calculations
        // =====================
        public static double InductiveReactance(double frequency, double inductance)
        {
            if (frequency < 0 || inductance < 0)
                throw new ArgumentException("Frequency and inductance must be non-negative values.");
            return 2 * Math.PI * frequency * inductance;
        }

        public static double InductorEnergyStored(double inductance, double current)
        {
            if (inductance < 0 || current < 0)
                throw new ArgumentException("Inductance and current must be non-negative values.");
            return 0.5 * inductance * current * current;
        }

        public static double RLTimeConstant(double inductance, double resistance)
        {
            if (inductance < 0 || resistance < 0)
                throw new ArgumentException("Inductance and resistance must be non-negative values.");
            return inductance / resistance;
        }
    }
}
