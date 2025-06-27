using System;

namespace CircuitTool
{
    /// <summary>
    /// Provides calculations for digital circuit design and analysis.
    /// </summary>
    /// <remarks>
    /// <para>Example usage:</para>
    /// <code>
    /// double propDelay = DigitalCalculator.PropagationDelay(5e-9, 3e-12, 1000); // Propagation delay
    /// double risetime = DigitalCalculator.RiseTime(100e-12, 50e-12); // Rise time calculation
    /// double power = DigitalCalculator.DynamicPower(3.3, 1e-12, 100e6); // Dynamic power consumption
    /// </code>
    /// </remarks>
    public static class DigitalCalculator
    {
        /// <summary>
        /// Calculates propagation delay for a digital gate.
        /// </summary>
        /// <param name="intrinsicDelay">Intrinsic delay of the gate in seconds.</param>
        /// <param name="loadCapacitance">Load capacitance in farads (F).</param>
        /// <param name="driveStrength">Drive strength in ohms (Ω).</param>
        /// <returns>Total propagation delay in seconds.</returns>
        /// <example>
        /// double propDelay = DigitalCalculator.PropagationDelay(5e-9, 3e-12, 1000); // Propagation delay
        /// </example>
        public static double PropagationDelay(double intrinsicDelay, double loadCapacitance, double driveStrength)
        {
            if (intrinsicDelay < 0 || loadCapacitance < 0 || driveStrength <= 0)
                throw new ArgumentException("Intrinsic delay and load capacitance must be non-negative, drive strength must be positive.");
            return intrinsicDelay + (0.69 * driveStrength * loadCapacitance);
        }

        /// <summary>
        /// Calculates rise time for a digital signal.
        /// </summary>
        /// <param name="resistance">Resistance in ohms (Ω).</param>
        /// <param name="capacitance">Capacitance in farads (F).</param>
        /// <returns>Rise time (10% to 90%) in seconds.</returns>
        /// <example>
        /// double risetime = DigitalCalculator.RiseTime(100e-12, 50e-12); // Rise time calculation
        /// </example>
        public static double RiseTime(double resistance, double capacitance)
        {
            if (resistance < 0 || capacitance < 0)
                throw new ArgumentException("Resistance and capacitance must be non-negative.");
            return 2.2 * resistance * capacitance;
        }

        /// <summary>
        /// Calculates fall time for a digital signal.
        /// </summary>
        /// <param name="resistance">Resistance in ohms (Ω).</param>
        /// <param name="capacitance">Capacitance in farads (F).</param>
        /// <returns>Fall time (90% to 10%) in seconds.</returns>
        public static double FallTime(double resistance, double capacitance)
        {
            if (resistance < 0 || capacitance < 0)
                throw new ArgumentException("Resistance and capacitance must be non-negative.");
            return 2.2 * resistance * capacitance;
        }

        /// <summary>
        /// Calculates dynamic power consumption in CMOS circuits.
        /// </summary>
        /// <param name="voltage">Supply voltage in volts (V).</param>
        /// <param name="capacitance">Load capacitance in farads (F).</param>
        /// <param name="frequency">Switching frequency in Hz.</param>
        /// <returns>Dynamic power consumption in watts (W).</returns>
        /// <example>
        /// double power = DigitalCalculator.DynamicPower(3.3, 1e-12, 100e6); // Dynamic power consumption
        /// </example>
        public static double DynamicPower(double voltage, double capacitance, double frequency)
        {
            if (voltage < 0 || capacitance < 0 || frequency < 0)
                throw new ArgumentException("All parameters must be non-negative.");
            return capacitance * voltage * voltage * frequency;
        }

        /// <summary>
        /// Calculates static power consumption due to leakage current.
        /// </summary>
        /// <param name="voltage">Supply voltage in volts (V).</param>
        /// <param name="leakageCurrent">Leakage current in amperes (A).</param>
        /// <returns>Static power consumption in watts (W).</returns>
        public static double StaticPower(double voltage, double leakageCurrent)
        {
            if (voltage < 0 || leakageCurrent < 0)
                throw new ArgumentException("Voltage and leakage current must be non-negative.");
            return voltage * leakageCurrent;
        }

        /// <summary>
        /// Calculates the maximum operating frequency based on propagation delay.
        /// </summary>
        /// <param name="propagationDelay">Propagation delay in seconds.</param>
        /// <param name="setupTime">Setup time in seconds.</param>
        /// <param name="holdTime">Hold time in seconds.</param>
        /// <returns>Maximum operating frequency in Hz.</returns>
        public static double MaxFrequency(double propagationDelay, double setupTime, double holdTime)
        {
            if (propagationDelay < 0 || setupTime < 0 || holdTime < 0)
                throw new ArgumentException("All timing parameters must be non-negative.");
            double minPeriod = propagationDelay + setupTime + holdTime;
            if (minPeriod <= 0)
                throw new ArgumentException("Minimum period must be positive.");
            return 1.0 / minPeriod;
        }

        /// <summary>
        /// Calculates the fanout capability of a digital gate.
        /// </summary>
        /// <param name="outputCurrent">Output drive current in amperes (A).</param>
        /// <param name="inputCurrent">Input current requirement per gate in amperes (A).</param>
        /// <returns>Maximum fanout (number of gates that can be driven).</returns>
        public static int Fanout(double outputCurrent, double inputCurrent)
        {
            if (outputCurrent < 0 || inputCurrent <= 0)
                throw new ArgumentException("Output current must be non-negative and input current must be positive.");
            return (int)Math.Floor(outputCurrent / inputCurrent);
        }

        /// <summary>
        /// Calculates noise margin for a digital circuit.
        /// </summary>
        /// <param name="outputHighVoltage">Output high voltage (VOH) in volts.</param>
        /// <param name="inputHighThreshold">Input high threshold (VIH) in volts.</param>
        /// <param name="outputLowVoltage">Output low voltage (VOL) in volts.</param>
        /// <param name="inputLowThreshold">Input low threshold (VIL) in volts.</param>
        /// <returns>Tuple containing (NoiseMarginHigh, NoiseMarginLow) in volts.</returns>
        public static (double NoiseMarginHigh, double NoiseMarginLow) NoiseMargin(
            double outputHighVoltage, double inputHighThreshold, 
            double outputLowVoltage, double inputLowThreshold)
        {
            if (outputHighVoltage < inputHighThreshold)
                throw new ArgumentException("Output high voltage must be greater than input high threshold.");
            if (outputLowVoltage > inputLowThreshold)
                throw new ArgumentException("Output low voltage must be less than input low threshold.");
            
            double nmh = outputHighVoltage - inputHighThreshold;
            double nml = inputLowThreshold - outputLowVoltage;
            
            return (nmh, nml);
        }

        /// <summary>
        /// Calculates characteristic impedance for a digital transmission line.
        /// </summary>
        /// <param name="inductance">Inductance per unit length (H/m).</param>
        /// <param name="capacitance">Capacitance per unit length (F/m).</param>
        /// <returns>Characteristic impedance in ohms (Ω).</returns>
        public static double CharacteristicImpedance(double inductance, double capacitance)
        {
            if (inductance <= 0 || capacitance <= 0)
                throw new ArgumentException("Inductance and capacitance must be positive.");
            return Math.Sqrt(inductance / capacitance);
        }

        /// <summary>
        /// Calculates the minimum trace width for a given current carrying capacity.
        /// </summary>
        /// <param name="current">Current in amperes (A).</param>
        /// <param name="temperatureRise">Allowed temperature rise in Celsius.</param>
        /// <param name="copperThickness">Copper thickness in ounces (1 oz = 35 μm).</param>
        /// <returns>Minimum trace width in meters.</returns>
        public static double MinimumTraceWidth(double current, double temperatureRise, double copperThickness = 1.0)
        {
            if (current < 0 || temperatureRise <= 0 || copperThickness <= 0)
                throw new ArgumentException("Current must be non-negative, temperature rise and copper thickness must be positive.");
            
            // IPC-2221 formula for trace width calculation
            double k = 0.024; // Constant for internal traces
            double area = Math.Pow(current / (k * Math.Pow(temperatureRise, 0.44)), 1.0 / 0.725);
            double thickness = copperThickness * 35e-6; // Convert oz to meters
            return area / thickness;
        }
    }
}
