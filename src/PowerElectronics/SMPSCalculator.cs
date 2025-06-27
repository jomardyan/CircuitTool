using System;

namespace CircuitTool.PowerElectronics
{
    /// <summary>
    /// Provides calculations for switching mode power supplies (SMPS) design.
    /// </summary>
    /// <remarks>
    /// <para>Example usage:</para>
    /// <code>
    /// double duty = SMPSCalculator.DutyCycle(12, 5); // Buck converter duty cycle
    /// double ripple = SMPSCalculator.OutputRipple(100e-6, 1, 100e3); // Output voltage ripple
    /// double inductance = SMPSCalculator.MinimumInductance(5, 2, 100e3, 0.3); // CCM inductance
    /// </code>
    /// </remarks>
    public static class SMPSCalculator
    {
        /// <summary>
        /// Calculates duty cycle for a buck converter.
        /// </summary>
        /// <param name="inputVoltage">Input voltage in volts (V).</param>
        /// <param name="outputVoltage">Output voltage in volts (V).</param>
        /// <returns>Duty cycle (0-1).</returns>
        /// <example>
        /// double duty = SMPSCalculator.DutyCycle(12, 5); // Buck converter duty cycle
        /// </example>
        public static double BuckDutyCycle(double inputVoltage, double outputVoltage)
        {
            if (inputVoltage <= 0 || outputVoltage <= 0)
                throw new ArgumentException("Input and output voltages must be positive.");
            if (outputVoltage > inputVoltage)
                throw new ArgumentException("Output voltage cannot exceed input voltage for buck converter.");
            
            return outputVoltage / inputVoltage;
        }

        /// <summary>
        /// Calculates duty cycle for a boost converter.
        /// </summary>
        /// <param name="inputVoltage">Input voltage in volts (V).</param>
        /// <param name="outputVoltage">Output voltage in volts (V).</param>
        /// <returns>Duty cycle (0-1).</returns>
        public static double BoostDutyCycle(double inputVoltage, double outputVoltage)
        {
            if (inputVoltage <= 0 || outputVoltage <= 0)
                throw new ArgumentException("Input and output voltages must be positive.");
            if (outputVoltage < inputVoltage)
                throw new ArgumentException("Output voltage must exceed input voltage for boost converter.");
            
            return 1 - (inputVoltage / outputVoltage);
        }

        /// <summary>
        /// Calculates duty cycle for a buck-boost converter.
        /// </summary>
        /// <param name="inputVoltage">Input voltage in volts (V).</param>
        /// <param name="outputVoltage">Output voltage in volts (V).</param>
        /// <returns>Duty cycle (0-1).</returns>
        public static double BuckBoostDutyCycle(double inputVoltage, double outputVoltage)
        {
            if (inputVoltage <= 0 || outputVoltage <= 0)
                throw new ArgumentException("Input and output voltages must be positive.");
            
            return outputVoltage / (inputVoltage + outputVoltage);
        }

        /// <summary>
        /// Calculates minimum inductance for continuous conduction mode (CCM).
        /// </summary>
        /// <param name="outputVoltage">Output voltage in volts (V).</param>
        /// <param name="outputCurrent">Output current in amperes (A).</param>
        /// <param name="switchingFrequency">Switching frequency in Hz.</param>
        /// <param name="currentRippleFactor">Current ripple factor (ΔI/I, typically 0.2-0.4).</param>
        /// <returns>Minimum inductance in henries (H).</returns>
        /// <example>
        /// double inductance = SMPSCalculator.MinimumInductance(5, 2, 100e3, 0.3); // CCM inductance
        /// </example>
        public static double MinimumInductanceBuck(double outputVoltage, double outputCurrent, 
                                                  double switchingFrequency, double currentRippleFactor)
        {
            if (outputVoltage <= 0 || outputCurrent <= 0 || switchingFrequency <= 0)
                throw new ArgumentException("Output voltage, current, and switching frequency must be positive.");
            if (currentRippleFactor <= 0 || currentRippleFactor >= 1)
                throw new ArgumentException("Current ripple factor must be between 0 and 1.");
            
            double deltaI = currentRippleFactor * outputCurrent;
            return outputVoltage / (deltaI * switchingFrequency);
        }

        /// <summary>
        /// Calculates output capacitance for desired voltage ripple.
        /// </summary>
        /// <param name="outputCurrent">Output current in amperes (A).</param>
        /// <param name="switchingFrequency">Switching frequency in Hz.</param>
        /// <param name="voltageRipple">Desired voltage ripple in volts (V).</param>
        /// <returns>Required output capacitance in farads (F).</returns>
        public static double OutputCapacitance(double outputCurrent, double switchingFrequency, double voltageRipple)
        {
            if (outputCurrent <= 0 || switchingFrequency <= 0 || voltageRipple <= 0)
                throw new ArgumentException("All parameters must be positive.");
            
            return outputCurrent / (8 * switchingFrequency * voltageRipple);
        }

        /// <summary>
        /// Calculates output voltage ripple.
        /// </summary>
        /// <param name="outputCapacitance">Output capacitance in farads (F).</param>
        /// <param name="outputCurrent">Output current in amperes (A).</param>
        /// <param name="switchingFrequency">Switching frequency in Hz.</param>
        /// <returns>Output voltage ripple in volts (V).</returns>
        /// <example>
        /// double ripple = SMPSCalculator.OutputRipple(100e-6, 1, 100e3); // Output voltage ripple
        /// </example>
        public static double OutputRipple(double outputCapacitance, double outputCurrent, double switchingFrequency)
        {
            if (outputCapacitance <= 0 || outputCurrent <= 0 || switchingFrequency <= 0)
                throw new ArgumentException("All parameters must be positive.");
            
            return outputCurrent / (8 * outputCapacitance * switchingFrequency);
        }

        /// <summary>
        /// Calculates switching losses in a MOSFET.
        /// </summary>
        /// <param name="voltage">Drain-source voltage in volts (V).</param>
        /// <param name="current">Drain current in amperes (A).</param>
        /// <param name="switchingFrequency">Switching frequency in Hz.</param>
        /// <param name="riseTime">Rise time in seconds.</param>
        /// <param name="fallTime">Fall time in seconds.</param>
        /// <returns>Switching power loss in watts (W).</returns>
        public static double SwitchingLoss(double voltage, double current, double switchingFrequency, 
                                         double riseTime, double fallTime)
        {
            if (voltage < 0 || current < 0 || switchingFrequency <= 0 || riseTime < 0 || fallTime < 0)
                throw new ArgumentException("Voltage and current must be non-negative, other parameters must be positive or non-negative.");
            
            double switchingTime = riseTime + fallTime;
            return 0.5 * voltage * current * switchingTime * switchingFrequency;
        }

        /// <summary>
        /// Calculates conduction losses in a MOSFET.
        /// </summary>
        /// <param name="current">RMS current in amperes (A).</param>
        /// <param name="onResistance">On-state resistance in ohms (Ω).</param>
        /// <param name="dutyCycle">Duty cycle (0-1).</param>
        /// <returns>Conduction power loss in watts (W).</returns>
        public static double ConductionLoss(double current, double onResistance, double dutyCycle)
        {
            if (current < 0 || onResistance < 0)
                throw new ArgumentException("Current and resistance must be non-negative.");
            if (dutyCycle < 0 || dutyCycle > 1)
                throw new ArgumentException("Duty cycle must be between 0 and 1.");
            
            return current * current * onResistance * dutyCycle;
        }

        /// <summary>
        /// Calculates transformer turns ratio for flyback converter.
        /// </summary>
        /// <param name="inputVoltageMin">Minimum input voltage in volts (V).</param>
        /// <param name="outputVoltage">Output voltage in volts (V).</param>
        /// <param name="forwardDrop">Forward voltage drop of output diode in volts (V).</param>
        /// <param name="maxDutyCycle">Maximum allowable duty cycle (typically 0.45-0.5).</param>
        /// <returns>Transformer turns ratio (Np/Ns).</returns>
        public static double FlybackTurnsRatio(double inputVoltageMin, double outputVoltage, 
                                             double forwardDrop, double maxDutyCycle)
        {
            if (inputVoltageMin <= 0 || outputVoltage <= 0 || forwardDrop < 0)
                throw new ArgumentException("Voltages must be positive, forward drop must be non-negative.");
            if (maxDutyCycle <= 0 || maxDutyCycle >= 1)
                throw new ArgumentException("Maximum duty cycle must be between 0 and 1.");
            
            return (inputVoltageMin * maxDutyCycle) / ((outputVoltage + forwardDrop) * (1 - maxDutyCycle));
        }

        /// <summary>
        /// Calculates magnetizing inductance for flyback converter.
        /// </summary>
        /// <param name="inputVoltage">Input voltage in volts (V).</param>
        /// <param name="dutyCycle">Duty cycle (0-1).</param>
        /// <param name="switchingFrequency">Switching frequency in Hz.</param>
        /// <param name="peakCurrent">Peak magnetizing current in amperes (A).</param>
        /// <returns>Magnetizing inductance in henries (H).</returns>
        public static double FlybackMagnetizingInductance(double inputVoltage, double dutyCycle, 
                                                         double switchingFrequency, double peakCurrent)
        {
            if (inputVoltage <= 0 || switchingFrequency <= 0 || peakCurrent <= 0)
                throw new ArgumentException("Input voltage, switching frequency, and peak current must be positive.");
            if (dutyCycle <= 0 || dutyCycle >= 1)
                throw new ArgumentException("Duty cycle must be between 0 and 1.");
            
            return (inputVoltage * dutyCycle) / (peakCurrent * switchingFrequency);
        }
    }
}
