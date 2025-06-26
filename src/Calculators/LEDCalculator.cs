using System;

namespace CircuitTool
{
    /// <summary>
    /// Provides easy-to-use methods for LED circuit calculations, including resistor values, power, and brightness.
    /// </summary>
    /// <remarks>
    /// <para>Example usage:</para>
    /// <code>
    /// double resistor = LEDCalculator.CalculateResistorValue(5, 2, 0.02); // 150 Ohms
    /// double power = LEDCalculator.CalculateLEDPower(5, 0.02); // 0.1W
    /// double brightness = LEDCalculator.CalculateBrightness(50); // 0.5 (50%)
    /// </code>
    /// </remarks>
    public static class LEDCalculator
    {
        /// <summary>
        /// Calculates the required resistor value for an LED circuit using <c>R = (Vs - Vf) / I</c>.
        /// </summary>
        /// <param name="supplyVoltage">Supply voltage in volts (V).</param>
        /// <param name="ledVoltage">LED forward voltage in volts (V).</param>
        /// <param name="ledCurrent">Desired LED current in amperes (A).</param>
        /// <returns>Required resistor value in ohms (Ω).</returns>
        /// <example>
        /// double resistor = LEDCalculator.CalculateResistorValue(5, 2, 0.02); // 150 Ohms
        /// </example>
        public static double CalculateResistorValue(double supplyVoltage, double ledVoltage, double ledCurrent)
        {
            if (ledCurrent <= 0)
                throw new ArgumentException("LED current must be greater than zero.", nameof(ledCurrent));
            if (supplyVoltage <= ledVoltage)
                throw new ArgumentException("Supply voltage must be greater than LED voltage.", nameof(supplyVoltage));
            return (supplyVoltage - ledVoltage) / ledCurrent;
        }

        /// <summary>
        /// Calculates the power consumption of an LED circuit using <c>P = V × I</c>.
        /// </summary>
        /// <param name="supplyVoltage">Supply voltage in volts (V).</param>
        /// <param name="ledCurrent">LED current in amperes (A).</param>
        /// <returns>Power consumption in watts (W).</returns>
        /// <example>
        /// double power = LEDCalculator.CalculateLEDPower(5, 0.02); // 0.1W
        /// </example>
        public static double CalculateLEDPower(double supplyVoltage, double ledCurrent) => supplyVoltage * ledCurrent;

        /// <summary>
        /// Calculates the brightness ratio when dimming an LED with PWM.
        /// </summary>
        /// <param name="dutyCycle">PWM duty cycle (0-100%).</param>
        /// <returns>Brightness ratio (0-1).</returns>
        /// <example>
        /// double brightness = LEDCalculator.CalculateBrightness(75); // 0.75
        /// </example>
        public static double CalculateBrightness(double dutyCycle)
        {
            if (dutyCycle < 0 || dutyCycle > 100)
                throw new ArgumentException("Duty cycle must be between 0 and 100.", nameof(dutyCycle));
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
