using System;

namespace CircuitTool
{
    /// <summary>
    /// Provides calculations for LED circuits including resistor values and power consumption
    /// </summary>
    public static class LEDCalculator
    {
        /// <summary>
        /// Calculates the required resistor value for an LED circuit
        /// </summary>
        /// <param name="supplyVoltage">Supply voltage (V)</param>
        /// <param name="ledVoltage">LED forward voltage (V)</param>
        /// <param name="ledCurrent">Desired LED current (A)</param>
        /// <returns>Required resistor value in Ohms</returns>
        public static double CalculateResistorValue(double supplyVoltage, double ledVoltage, double ledCurrent)
        {
            if (ledCurrent <= 0)
                throw new ArgumentException("LED current must be greater than zero");
            
            if (supplyVoltage <= ledVoltage)
                throw new ArgumentException("Supply voltage must be greater than LED voltage");
            
            return (supplyVoltage - ledVoltage) / ledCurrent;
        }

        /// <summary>
        /// Calculates the power consumption of an LED circuit
        /// </summary>
        /// <param name="supplyVoltage">Supply voltage (V)</param>
        /// <param name="ledCurrent">LED current (A)</param>
        /// <returns>Power consumption in Watts</returns>
        public static double CalculateLEDPower(double supplyVoltage, double ledCurrent)
        {
            return supplyVoltage * ledCurrent;
        }

        /// <summary>
        /// Calculates the brightness ratio when dimming an LED with PWM
        /// </summary>
        /// <param name="dutyCycle">PWM duty cycle (0-100%)</param>
        /// <returns>Brightness ratio (0-1)</returns>
        public static double CalculateBrightness(double dutyCycle)
        {
            if (dutyCycle < 0 || dutyCycle > 100)
                throw new ArgumentException("Duty cycle must be between 0 and 100");
            
            return dutyCycle / 100.0;
        }

        /// <summary>
        /// Calculates resistor value for multiple LEDs in series
        /// </summary>
        /// <param name="supplyVoltage">Supply voltage (V)</param>
        /// <param name="ledVoltage">LED forward voltage (V)</param>
        /// <param name="numLEDs">Number of LEDs in series</param>
        /// <param name="ledCurrent">Desired LED current (A)</param>
        /// <returns>Required resistor value in Ohms</returns>
        public static double CalculateSeriesResistor(double supplyVoltage, double ledVoltage, int numLEDs, double ledCurrent)
        {
            double totalLEDVoltage = ledVoltage * numLEDs;
            return CalculateResistorValue(supplyVoltage, totalLEDVoltage, ledCurrent);
        }
    }
}
