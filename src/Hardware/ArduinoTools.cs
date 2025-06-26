using System;

namespace CircuitTool
{
    /// <summary>
    /// Provides Arduino-specific calculations and utilities
    /// </summary>
    public static class ArduinoTools
    {
        /// <summary>
        /// Converts analog reading to voltage for Arduino Uno (10-bit ADC, 5V reference)
        /// </summary>
        /// <param name="analogReading">Analog reading value (0-1023)</param>
        /// <param name="referenceVoltage">Reference voltage (default 5V for Arduino Uno)</param>
        /// <returns>Voltage value</returns>
        public static double AnalogToVoltage(int analogReading, double referenceVoltage = 5.0)
        {
            if (analogReading < 0 || analogReading > 1023)
                throw new ArgumentException("Analog reading must be between 0 and 1023");
            
            return (analogReading / 1023.0) * referenceVoltage;
        }

        /// <summary>
        /// Converts voltage to analog reading for Arduino Uno
        /// </summary>
        /// <param name="voltage">Voltage value</param>
        /// <param name="referenceVoltage">Reference voltage (default 5V for Arduino Uno)</param>
        /// <returns>Analog reading value (0-1023)</returns>
        public static int VoltageToAnalog(double voltage, double referenceVoltage = 5.0)
        {
            if (voltage < 0 || voltage > referenceVoltage)
                throw new ArgumentException($"Voltage must be between 0 and {referenceVoltage}V");
            
            return (int)Math.Round((voltage / referenceVoltage) * 1023.0);
        }

        /// <summary>
        /// Calculates delay time between servo pulses for specific angle
        /// </summary>
        /// <param name="angle">Servo angle in degrees (0-180)</param>
        /// <returns>Pulse width in microseconds</returns>
        public static double ServoAngleToPulseWidth(double angle)
        {
            if (angle < 0 || angle > 180)
                throw new ArgumentException("Servo angle must be between 0 and 180 degrees");
            
            // Standard servo: 1ms (0°) to 2ms (180°) pulse width
            return 1000 + (angle / 180.0) * 1000;
        }

        /// <summary>
        /// Calculates current consumption for Arduino projects
        /// </summary>
        /// <param name="cpuCurrent">CPU current consumption (mA)</param>
        /// <param name="digitalPins">Number of active digital pins</param>
        /// <param name="analogPins">Number of active analog pins</param>
        /// <param name="additionalCurrent">Additional current from external components (mA)</param>
        /// <returns>Total current consumption in mA</returns>
        public static double CalculateCurrentConsumption(double cpuCurrent = 20, int digitalPins = 0, int analogPins = 0, double additionalCurrent = 0)
        {
            double pinCurrent = digitalPins * 1.0 + analogPins * 0.5; // Approximate pin consumption
            return cpuCurrent + pinCurrent + additionalCurrent;
        }

        /// <summary>
        /// Calculates PWM frequency for Arduino Timer
        /// </summary>
        /// <param name="prescaler">Timer prescaler value</param>
        /// <param name="clockFrequency">Arduino clock frequency in Hz (default 16MHz)</param>
        /// <returns>PWM frequency in Hz</returns>
        public static double CalculatePWMFrequency(int prescaler, double clockFrequency = 16000000)
        {
            return clockFrequency / (prescaler * 256); // For 8-bit PWM
        }
    }
}
