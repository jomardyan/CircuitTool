using System;

namespace CircuitTool
{
    /// <summary>
    /// Provides calculations for transformer design and characteristics.
    /// </summary>
    public static class TransformerCalculator
    {
        /// <summary>
        /// Calculates the secondary voltage using the transformer ratio Vs = Vp × (Ns / Np).
        /// </summary>
        /// <param name="primaryVoltage">Primary voltage in volts (V).</param>
        /// <param name="primaryTurns">Number of primary turns.</param>
        /// <param name="secondaryTurns">Number of secondary turns.</param>
        /// <returns>Secondary voltage in volts (V).</returns>
        public static double SecondaryVoltage(double primaryVoltage, double primaryTurns, double secondaryTurns)
        {
            if (primaryTurns <= 0)
                throw new ArgumentException("Primary turns must be positive.");
            
            return primaryVoltage * (secondaryTurns / primaryTurns);
        }

        /// <summary>
        /// Calculates the primary current using the transformer ratio Ip = Is × (Ns / Np).
        /// </summary>
        /// <param name="secondaryCurrent">Secondary current in amperes (A).</param>
        /// <param name="primaryTurns">Number of primary turns.</param>
        /// <param name="secondaryTurns">Number of secondary turns.</param>
        /// <returns>Primary current in amperes (A).</returns>
        public static double PrimaryCurrent(double secondaryCurrent, double primaryTurns, double secondaryTurns)
        {
            if (primaryTurns <= 0)
                throw new ArgumentException("Primary turns must be positive.");
            
            return secondaryCurrent * (secondaryTurns / primaryTurns);
        }

        /// <summary>
        /// Calculates the turns ratio of a transformer (Np / Ns).
        /// </summary>
        /// <param name="primaryTurns">Number of primary turns.</param>
        /// <param name="secondaryTurns">Number of secondary turns.</param>
        /// <returns>Turns ratio (unitless).</returns>
        public static double TurnsRatio(double primaryTurns, double secondaryTurns)
        {
            if (primaryTurns <= 0 || secondaryTurns <= 0)
                throw new ArgumentException("Primary and secondary turns must be positive.");
            
            return primaryTurns / secondaryTurns;
        }

        /// <summary>
        /// Calculates the voltage ratio of a transformer (Vp / Vs).
        /// </summary>
        /// <param name="primaryVoltage">Primary voltage in volts (V).</param>
        /// <param name="secondaryVoltage">Secondary voltage in volts (V).</param>
        /// <returns>Voltage ratio (unitless).</returns>
        public static double VoltageRatio(double primaryVoltage, double secondaryVoltage)
        {
            if (secondaryVoltage == 0)
                throw new ArgumentException("Secondary voltage cannot be zero.");
            
            return primaryVoltage / secondaryVoltage;
        }

        /// <summary>
        /// Calculates the transformer efficiency using η = (Pout / Pin) × 100%.
        /// </summary>
        /// <param name="outputPower">Output power in watts (W).</param>
        /// <param name="inputPower">Input power in watts (W).</param>
        /// <returns>Efficiency as a percentage (%).</returns>
        public static double Efficiency(double outputPower, double inputPower)
        {
            if (inputPower <= 0)
                throw new ArgumentException("Input power must be positive.");
            if (outputPower < 0)
                throw new ArgumentException("Output power must be non-negative.");
            
            return (outputPower / inputPower) * 100.0;
        }

        /// <summary>
        /// Calculates the power losses in a transformer.
        /// </summary>
        /// <param name="inputPower">Input power in watts (W).</param>
        /// <param name="outputPower">Output power in watts (W).</param>
        /// <returns>Power losses in watts (W).</returns>
        public static double PowerLoss(double inputPower, double outputPower)
        {
            if (inputPower < 0 || outputPower < 0)
                throw new ArgumentException("Input and output power must be non-negative.");
            if (outputPower > inputPower)
                throw new ArgumentException("Output power cannot exceed input power.");
            
            return inputPower - outputPower;
        }

        /// <summary>
        /// Calculates the regulation of a transformer using Regulation = ((Vnl - Vfl) / Vfl) × 100%.
        /// </summary>
        /// <param name="noLoadVoltage">No-load voltage in volts (V).</param>
        /// <param name="fullLoadVoltage">Full-load voltage in volts (V).</param>
        /// <returns>Regulation as a percentage (%).</returns>
        public static double Regulation(double noLoadVoltage, double fullLoadVoltage)
        {
            if (fullLoadVoltage <= 0)
                throw new ArgumentException("Full-load voltage must be positive.");
            
            return ((noLoadVoltage - fullLoadVoltage) / fullLoadVoltage) * 100.0;
        }

        /// <summary>
        /// Calculates the required number of secondary turns for a desired voltage.
        /// </summary>
        /// <param name="primaryVoltage">Primary voltage in volts (V).</param>
        /// <param name="secondaryVoltage">Desired secondary voltage in volts (V).</param>
        /// <param name="primaryTurns">Number of primary turns.</param>
        /// <returns>Required number of secondary turns.</returns>
        public static double RequiredSecondaryTurns(double primaryVoltage, double secondaryVoltage, double primaryTurns)
        {
            if (primaryVoltage <= 0)
                throw new ArgumentException("Primary voltage must be positive.");
            if (primaryTurns <= 0)
                throw new ArgumentException("Primary turns must be positive.");
            
            return (secondaryVoltage * primaryTurns) / primaryVoltage;
        }

        /// <summary>
        /// Calculates the apparent power rating of a transformer.
        /// </summary>
        /// <param name="voltage">Voltage in volts (V).</param>
        /// <param name="current">Current in amperes (A).</param>
        /// <returns>Apparent power in volt-amperes (VA).</returns>
        public static double ApparentPower(double voltage, double current)
        {
            if (voltage < 0 || current < 0)
                throw new ArgumentException("Voltage and current must be non-negative.");
            
            return voltage * current;
        }
    }
}
