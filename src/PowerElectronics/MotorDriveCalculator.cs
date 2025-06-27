using System;

namespace CircuitTool.PowerElectronics
{
    /// <summary>
    /// Provides calculations for motor drive and control systems.
    /// </summary>
    /// <remarks>
    /// <para>Example usage:</para>
    /// <code>
    /// double torque = MotorDriveCalculator.TorqueFromPower(750, 1800); // Motor torque
    /// double speed = MotorDriveCalculator.SynchronousSpeed(50, 4); // Synchronous speed
    /// double slip = MotorDriveCalculator.Slip(1800, 1750); // Motor slip
    /// </code>
    /// </remarks>
    public static class MotorDriveCalculator
    {
        /// <summary>
        /// Calculates motor torque from power and speed.
        /// </summary>
        /// <param name="power">Mechanical power in watts (W).</param>
        /// <param name="speed">Rotational speed in RPM.</param>
        /// <returns>Torque in Newton-meters (N⋅m).</returns>
        /// <example>
        /// double torque = MotorDriveCalculator.TorqueFromPower(750, 1800); // Motor torque
        /// </example>
        public static double TorqueFromPower(double power, double speed)
        {
            if (power < 0 || speed <= 0)
                throw new ArgumentException("Power must be non-negative and speed must be positive.");
            
            double angularVelocity = speed * 2 * Math.PI / 60; // Convert RPM to rad/s
            return power / angularVelocity;
        }

        /// <summary>
        /// Calculates mechanical power from torque and speed.
        /// </summary>
        /// <param name="torque">Torque in Newton-meters (N⋅m).</param>
        /// <param name="speed">Rotational speed in RPM.</param>
        /// <returns>Mechanical power in watts (W).</returns>
        public static double PowerFromTorque(double torque, double speed)
        {
            if (torque < 0 || speed < 0)
                throw new ArgumentException("Torque and speed must be non-negative.");
            
            double angularVelocity = speed * 2 * Math.PI / 60; // Convert RPM to rad/s
            return torque * angularVelocity;
        }

        /// <summary>
        /// Calculates synchronous speed for AC motor.
        /// </summary>
        /// <param name="frequency">Supply frequency in Hz.</param>
        /// <param name="poles">Number of poles.</param>
        /// <returns>Synchronous speed in RPM.</returns>
        /// <example>
        /// double speed = MotorDriveCalculator.SynchronousSpeed(50, 4); // Synchronous speed
        /// </example>
        public static double SynchronousSpeed(double frequency, int poles)
        {
            if (frequency <= 0)
                throw new ArgumentException("Frequency must be positive.");
            if (poles <= 0 || poles % 2 != 0)
                throw new ArgumentException("Number of poles must be positive and even.");
            
            return (120 * frequency) / poles;
        }

        /// <summary>
        /// Calculates motor slip.
        /// </summary>
        /// <param name="synchronousSpeed">Synchronous speed in RPM.</param>
        /// <param name="actualSpeed">Actual rotor speed in RPM.</param>
        /// <returns>Slip as a percentage.</returns>
        /// <example>
        /// double slip = MotorDriveCalculator.Slip(1800, 1750); // Motor slip
        /// </example>
        public static double Slip(double synchronousSpeed, double actualSpeed)
        {
            if (synchronousSpeed <= 0)
                throw new ArgumentException("Synchronous speed must be positive.");
            if (actualSpeed < 0)
                throw new ArgumentException("Actual speed must be non-negative.");
            
            return ((synchronousSpeed - actualSpeed) / synchronousSpeed) * 100;
        }

        /// <summary>
        /// Calculates rotor frequency for induction motor.
        /// </summary>
        /// <param name="slip">Slip as a percentage.</param>
        /// <param name="supplyFrequency">Supply frequency in Hz.</param>
        /// <returns>Rotor frequency in Hz.</returns>
        public static double RotorFrequency(double slip, double supplyFrequency)
        {
            if (slip < 0 || slip > 100)
                throw new ArgumentException("Slip must be between 0 and 100 percent.");
            if (supplyFrequency <= 0)
                throw new ArgumentException("Supply frequency must be positive.");
            
            return (slip / 100) * supplyFrequency;
        }

        /// <summary>
        /// Calculates motor efficiency.
        /// </summary>
        /// <param name="mechanicalPower">Mechanical power output in watts (W).</param>
        /// <param name="electricalPower">Electrical power input in watts (W).</param>
        /// <returns>Efficiency as a percentage.</returns>
        public static double Efficiency(double mechanicalPower, double electricalPower)
        {
            if (mechanicalPower < 0 || electricalPower <= 0)
                throw new ArgumentException("Mechanical power must be non-negative and electrical power must be positive.");
            if (mechanicalPower > electricalPower)
                throw new ArgumentException("Mechanical power cannot exceed electrical power input.");
            
            return (mechanicalPower / electricalPower) * 100;
        }

        /// <summary>
        /// Calculates V/Hz ratio for variable frequency drive.
        /// </summary>
        /// <param name="voltage">Motor voltage in volts (V).</param>
        /// <param name="frequency">Motor frequency in Hz.</param>
        /// <returns>V/Hz ratio.</returns>
        public static double VHzRatio(double voltage, double frequency)
        {
            if (voltage < 0 || frequency <= 0)
                throw new ArgumentException("Voltage must be non-negative and frequency must be positive.");
            
            return voltage / frequency;
        }

        /// <summary>
        /// Calculates required voltage for constant V/Hz operation.
        /// </summary>
        /// <param name="baseVoltage">Base (rated) voltage in volts (V).</param>
        /// <param name="baseFrequency">Base (rated) frequency in Hz.</param>
        /// <param name="operatingFrequency">Operating frequency in Hz.</param>
        /// <returns>Required voltage in volts (V).</returns>
        public static double VoltageForConstantVHz(double baseVoltage, double baseFrequency, double operatingFrequency)
        {
            if (baseVoltage <= 0 || baseFrequency <= 0 || operatingFrequency < 0)
                throw new ArgumentException("Base voltage and frequency must be positive, operating frequency must be non-negative.");
            
            return baseVoltage * (operatingFrequency / baseFrequency);
        }

        /// <summary>
        /// Calculates starting torque for induction motor.
        /// </summary>
        /// <param name="voltage">Applied voltage in volts (V).</param>
        /// <param name="ratedVoltage">Rated voltage in volts (V).</param>
        /// <param name="ratedStartingTorque">Rated starting torque in N⋅m.</param>
        /// <returns>Starting torque at applied voltage in N⋅m.</returns>
        public static double StartingTorque(double voltage, double ratedVoltage, double ratedStartingTorque)
        {
            if (voltage < 0 || ratedVoltage <= 0 || ratedStartingTorque < 0)
                throw new ArgumentException("Voltages must be non-negative (rated voltage positive), rated starting torque must be non-negative.");
            
            double voltageRatio = voltage / ratedVoltage;
            return ratedStartingTorque * voltageRatio * voltageRatio;
        }

        /// <summary>
        /// Calculates motor current from power and power factor.
        /// </summary>
        /// <param name="power">Mechanical power in watts (W).</param>
        /// <param name="voltage">Line voltage in volts (V).</param>
        /// <param name="efficiency">Motor efficiency (0-1).</param>
        /// <param name="powerFactor">Power factor (0-1).</param>
        /// <param name="isThreePhase">True for three-phase motor, false for single-phase.</param>
        /// <returns>Motor current in amperes (A).</returns>
        public static double MotorCurrent(double power, double voltage, double efficiency, 
                                        double powerFactor, bool isThreePhase = true)
        {
            if (power < 0 || voltage <= 0)
                throw new ArgumentException("Power must be non-negative and voltage must be positive.");
            if (efficiency <= 0 || efficiency > 1)
                throw new ArgumentException("Efficiency must be between 0 and 1.");
            if (powerFactor <= 0 || powerFactor > 1)
                throw new ArgumentException("Power factor must be between 0 and 1.");
            
            double electricalPower = power / efficiency;
            
            if (isThreePhase)
            {
                return electricalPower / (Math.Sqrt(3) * voltage * powerFactor);
            }
            else
            {
                return electricalPower / (voltage * powerFactor);
            }
        }

        /// <summary>
        /// Calculates PWM switching frequency for motor drive.
        /// </summary>
        /// <param name="fundamentalFrequency">Fundamental output frequency in Hz.</param>
        /// <param name="carrierRatio">Carrier to fundamental frequency ratio (typically 20-100).</param>
        /// <returns>PWM switching frequency in Hz.</returns>
        public static double PWMFrequency(double fundamentalFrequency, double carrierRatio)
        {
            if (fundamentalFrequency <= 0 || carrierRatio <= 1)
                throw new ArgumentException("Fundamental frequency must be positive and carrier ratio must be greater than 1.");
            
            return fundamentalFrequency * carrierRatio;
        }

        /// <summary>
        /// Calculates modulation index for PWM inverter.
        /// </summary>
        /// <param name="outputVoltage">Peak output voltage in volts (V).</param>
        /// <param name="dcBusVoltage">DC bus voltage in volts (V).</param>
        /// <returns>Modulation index (0-1).</returns>
        public static double ModulationIndex(double outputVoltage, double dcBusVoltage)
        {
            if (outputVoltage < 0 || dcBusVoltage <= 0)
                throw new ArgumentException("Output voltage must be non-negative and DC bus voltage must be positive.");
            
            return outputVoltage / (dcBusVoltage / 2);
        }
    }
}
