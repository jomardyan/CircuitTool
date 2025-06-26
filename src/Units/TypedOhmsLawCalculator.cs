using System;
using CircuitTool.Units;

namespace CircuitTool.Units
{
    /// <summary>
    /// Provides strongly-typed Ohm's Law calculations using units to prevent errors
    /// </summary>
    public static class TypedOhmsLawCalculator
    {
        /// <summary>
        /// Calculates voltage using Ohm's Law (V = I × R)
        /// </summary>
        /// <param name="current">Current</param>
        /// <param name="resistance">Resistance</param>
        /// <returns>Voltage</returns>
        public static Voltage CalculateVoltage(Current current, Resistance resistance)
        {
            return new Voltage(current.Amperes * resistance.Ohms, VoltageUnit.Volts);
        }

        /// <summary>
        /// Calculates current using Ohm's Law (I = V / R)
        /// </summary>
        /// <param name="voltage">Voltage</param>
        /// <param name="resistance">Resistance</param>
        /// <returns>Current</returns>
        public static Current CalculateCurrent(Voltage voltage, Resistance resistance)
        {
            if (resistance.Ohms <= 0)
                throw new ArgumentException("Resistance must be greater than zero");
            
            return new Current(voltage.Volts / resistance.Ohms, CurrentUnit.Amperes);
        }

        /// <summary>
        /// Calculates resistance using Ohm's Law (R = V / I)
        /// </summary>
        /// <param name="voltage">Voltage</param>
        /// <param name="current">Current</param>
        /// <returns>Resistance</returns>
        public static Resistance CalculateResistance(Voltage voltage, Current current)
        {
            if (current.Amperes == 0)
                throw new ArgumentException("Current cannot be zero");
            
            return new Resistance(voltage.Volts / current.Amperes, ResistanceUnit.Ohms);
        }

        /// <summary>
        /// Calculates power using P = V × I
        /// </summary>
        /// <param name="voltage">Voltage</param>
        /// <param name="current">Current</param>
        /// <returns>Power in watts</returns>
        public static double CalculatePower(Voltage voltage, Current current)
        {
            return voltage.Volts * current.Amperes;
        }

        /// <summary>
        /// Calculates power using P = V² / R
        /// </summary>
        /// <param name="voltage">Voltage</param>
        /// <param name="resistance">Resistance</param>
        /// <returns>Power in watts</returns>
        public static double CalculatePower(Voltage voltage, Resistance resistance)
        {
            if (resistance.Ohms <= 0)
                throw new ArgumentException("Resistance must be greater than zero");
            
            return (voltage.Volts * voltage.Volts) / resistance.Ohms;
        }

        /// <summary>
        /// Calculates power using P = I² × R
        /// </summary>
        /// <param name="current">Current</param>
        /// <param name="resistance">Resistance</param>
        /// <returns>Power in watts</returns>
        public static double CalculatePower(Current current, Resistance resistance)
        {
            return current.Amperes * current.Amperes * resistance.Ohms;
        }
    }
}
