using System;

namespace CircuitTool
{
    /// <summary>
    /// Provides calculations for operational amplifier circuits and characteristics.
    /// </summary>
    /// <remarks>
    /// <para>Example usage:</para>
    /// <code>
    /// double gain = OpAmpCalculator.NonInvertingGain(10000, 1000); // Non-inverting gain
    /// double gain2 = OpAmpCalculator.InvertingGain(10000, 1000); // Inverting gain
    /// double bandwidth = OpAmpCalculator.GainBandwidthProduct(1e6, 100); // Closed-loop bandwidth
    /// </code>
    /// </remarks>
    public static class OpAmpCalculator
    {
        /// <summary>
        /// Calculates gain of a non-inverting amplifier.
        /// </summary>
        /// <param name="feedbackResistor">Feedback resistor (Rf) in ohms (Ω).</param>
        /// <param name="inputResistor">Input resistor (Rin) in ohms (Ω).</param>
        /// <returns>Voltage gain (linear).</returns>
        /// <example>
        /// double gain = OpAmpCalculator.NonInvertingGain(10000, 1000); // Non-inverting gain
        /// </example>
        public static double NonInvertingGain(double feedbackResistor, double inputResistor)
        {
            if (feedbackResistor < 0 || inputResistor <= 0)
                throw new ArgumentException("Feedback resistor must be non-negative and input resistor must be positive.");
            
            return 1 + (feedbackResistor / inputResistor);
        }

        /// <summary>
        /// Calculates gain of an inverting amplifier.
        /// </summary>
        /// <param name="feedbackResistor">Feedback resistor (Rf) in ohms (Ω).</param>
        /// <param name="inputResistor">Input resistor (Rin) in ohms (Ω).</param>
        /// <returns>Voltage gain magnitude (linear, positive value).</returns>
        /// <example>
        /// double gain = OpAmpCalculator.InvertingGain(10000, 1000); // Inverting gain
        /// </example>
        public static double InvertingGain(double feedbackResistor, double inputResistor)
        {
            if (feedbackResistor < 0 || inputResistor <= 0)
                throw new ArgumentException("Feedback resistor must be non-negative and input resistor must be positive.");
            
            return feedbackResistor / inputResistor;
        }

        /// <summary>
        /// Calculates closed-loop bandwidth from gain-bandwidth product.
        /// </summary>
        /// <param name="gainBandwidthProduct">Gain-bandwidth product in Hz.</param>
        /// <param name="closedLoopGain">Closed-loop gain (linear).</param>
        /// <returns>Closed-loop bandwidth in Hz.</returns>
        /// <example>
        /// double bandwidth = OpAmpCalculator.GainBandwidthProduct(1e6, 100); // Closed-loop bandwidth
        /// </example>
        public static double ClosedLoopBandwidth(double gainBandwidthProduct, double closedLoopGain)
        {
            if (gainBandwidthProduct <= 0 || closedLoopGain <= 0)
                throw new ArgumentException("Gain-bandwidth product and closed-loop gain must be positive.");
            
            return gainBandwidthProduct / closedLoopGain;
        }

        /// <summary>
        /// Calculates gain of a differential amplifier.
        /// </summary>
        /// <param name="feedbackResistor">Feedback resistor (Rf) in ohms (Ω).</param>
        /// <param name="inputResistor">Input resistor (Rin) in ohms (Ω).</param>
        /// <returns>Differential gain (linear).</returns>
        public static double DifferentialGain(double feedbackResistor, double inputResistor)
        {
            if (feedbackResistor < 0 || inputResistor <= 0)
                throw new ArgumentException("Feedback resistor must be non-negative and input resistor must be positive.");
            
            return feedbackResistor / inputResistor;
        }

        /// <summary>
        /// Calculates common-mode rejection ratio (CMRR) in dB.
        /// </summary>
        /// <param name="differentialGain">Differential mode gain (linear).</param>
        /// <param name="commonModeGain">Common mode gain (linear).</param>
        /// <returns>CMRR in dB.</returns>
        public static double CMRR(double differentialGain, double commonModeGain)
        {
            if (differentialGain <= 0 || commonModeGain <= 0)
                throw new ArgumentException("Both gains must be positive.");
            
            return 20 * Math.Log10(differentialGain / commonModeGain);
        }

        /// <summary>
        /// Calculates input impedance of a non-inverting amplifier.
        /// </summary>
        /// <param name="opAmpInputImpedance">Op-amp input impedance in ohms (Ω).</param>
        /// <param name="openLoopGain">Open-loop gain (linear).</param>
        /// <param name="closedLoopGain">Closed-loop gain (linear).</param>
        /// <returns>Input impedance in ohms (Ω).</returns>
        public static double NonInvertingInputImpedance(double opAmpInputImpedance, double openLoopGain, double closedLoopGain)
        {
            if (opAmpInputImpedance <= 0 || openLoopGain <= 0 || closedLoopGain <= 0)
                throw new ArgumentException("All parameters must be positive.");
            
            double feedbackFactor = 1 + (openLoopGain / closedLoopGain);
            return opAmpInputImpedance * feedbackFactor;
        }

        /// <summary>
        /// Calculates output impedance of an op-amp circuit.
        /// </summary>
        /// <param name="opAmpOutputImpedance">Op-amp output impedance in ohms (Ω).</param>
        /// <param name="openLoopGain">Open-loop gain (linear).</param>
        /// <param name="feedbackFactor">Feedback factor (β).</param>
        /// <returns>Output impedance in ohms (Ω).</returns>
        public static double OutputImpedance(double opAmpOutputImpedance, double openLoopGain, double feedbackFactor)
        {
            if (opAmpOutputImpedance <= 0 || openLoopGain <= 0 || feedbackFactor < 0 || feedbackFactor > 1)
                throw new ArgumentException("Output impedance and open-loop gain must be positive, feedback factor must be between 0 and 1.");
            
            return opAmpOutputImpedance / (1 + openLoopGain * feedbackFactor);
        }

        /// <summary>
        /// Calculates slew rate limitation for sinusoidal signals.
        /// </summary>
        /// <param name="slewRate">Slew rate in V/s.</param>
        /// <param name="amplitude">Peak amplitude in volts (V).</param>
        /// <returns>Maximum frequency without slew rate limiting in Hz.</returns>
        public static double SlewRateLimit(double slewRate, double amplitude)
        {
            if (slewRate <= 0 || amplitude <= 0)
                throw new ArgumentException("Slew rate and amplitude must be positive.");
            
            return slewRate / (2 * Math.PI * amplitude);
        }

        /// <summary>
        /// Calculates gain and phase margins for stability analysis.
        /// </summary>
        /// <param name="openLoopGain">Open-loop gain at crossover frequency (linear).</param>
        /// <param name="phaseMargin">Phase margin in degrees.</param>
        /// <returns>Tuple containing (GainMargin in dB, PhaseMargin in degrees).</returns>
        public static (double GainMargin, double PhaseMargin) StabilityMargins(double openLoopGain, double phaseMargin)
        {
            if (openLoopGain <= 0)
                throw new ArgumentException("Open-loop gain must be positive.");
            
            double gainMargin = -20 * Math.Log10(openLoopGain);
            return (gainMargin, phaseMargin);
        }

        /// <summary>
        /// Calculates integrator circuit output for DC input.
        /// </summary>
        /// <param name="inputVoltage">Input voltage in volts (V).</param>
        /// <param name="inputResistor">Input resistor in ohms (Ω).</param>
        /// <param name="feedbackCapacitor">Feedback capacitor in farads (F).</param>
        /// <param name="time">Time in seconds (s).</param>
        /// <returns>Output voltage in volts (V).</returns>
        public static double IntegratorOutput(double inputVoltage, double inputResistor, double feedbackCapacitor, double time)
        {
            if (inputResistor <= 0 || feedbackCapacitor <= 0 || time < 0)
                throw new ArgumentException("Resistor and capacitor must be positive, time must be non-negative.");
            
            return -(inputVoltage * time) / (inputResistor * feedbackCapacitor);
        }

        /// <summary>
        /// Calculates differentiator circuit output for step input.
        /// </summary>
        /// <param name="inputCapacitor">Input capacitor in farads (F).</param>
        /// <param name="feedbackResistor">Feedback resistor in ohms (Ω).</param>
        /// <param name="inputVoltageStep">Input voltage step in volts (V).</param>
        /// <returns>Initial output voltage in volts (V).</returns>
        public static double DifferentiatorOutput(double inputCapacitor, double feedbackResistor, double inputVoltageStep)
        {
            if (inputCapacitor <= 0 || feedbackResistor <= 0)
                throw new ArgumentException("Capacitor and resistor must be positive.");
            
            // For ideal step input, output is theoretically infinite, but practically limited
            // This returns the time constant effect
            return -inputCapacitor * feedbackResistor * inputVoltageStep;
        }

        /// <summary>
        /// Calculates Sallen-Key filter cutoff frequency.
        /// </summary>
        /// <param name="r1">First resistor in ohms (Ω).</param>
        /// <param name="r2">Second resistor in ohms (Ω).</param>
        /// <param name="c1">First capacitor in farads (F).</param>
        /// <param name="c2">Second capacitor in farads (F).</param>
        /// <returns>Cutoff frequency in Hz.</returns>
        public static double SallenKeyCutoffFrequency(double r1, double r2, double c1, double c2)
        {
            if (r1 <= 0 || r2 <= 0 || c1 <= 0 || c2 <= 0)
                throw new ArgumentException("All component values must be positive.");
            
            return 1.0 / (2 * Math.PI * Math.Sqrt(r1 * r2 * c1 * c2));
        }

        /// <summary>
        /// Calculates multiple feedback filter cutoff frequency.
        /// </summary>
        /// <param name="r1">Input resistor in ohms (Ω).</param>
        /// <param name="r2">Feedback resistor in ohms (Ω).</param>
        /// <param name="c1">Input capacitor in farads (F).</param>
        /// <param name="c2">Feedback capacitor in farads (F).</param>
        /// <returns>Cutoff frequency in Hz.</returns>
        public static double MultipleFeedbackCutoffFrequency(double r1, double r2, double c1, double c2)
        {
            if (r1 <= 0 || r2 <= 0 || c1 <= 0 || c2 <= 0)
                throw new ArgumentException("All component values must be positive.");
            
            return 1.0 / (2 * Math.PI * Math.Sqrt(r1 * r2 * c1 * c2));
        }

        /// <summary>
        /// Calculates instrumentation amplifier gain.
        /// </summary>
        /// <param name="gainResistor">Gain setting resistor in ohms (Ω).</param>
        /// <param name="internalResistor">Internal resistor value in ohms (Ω) - typically 50kΩ.</param>
        /// <returns>Instrumentation amplifier gain (linear).</returns>
        public static double InstrumentationAmplifierGain(double gainResistor, double internalResistor = 50000)
        {
            if (gainResistor <= 0 || internalResistor <= 0)
                throw new ArgumentException("Both resistor values must be positive.");
            
            return 1 + (2 * internalResistor / gainResistor);
        }

        /// <summary>
        /// Calculates offset voltage referred to input.
        /// </summary>
        /// <param name="outputOffset">Output offset voltage in volts (V).</param>
        /// <param name="gain">Amplifier gain (linear).</param>
        /// <returns>Input-referred offset voltage in volts (V).</returns>
        public static double InputReferredOffset(double outputOffset, double gain)
        {
            if (gain <= 0)
                throw new ArgumentException("Gain must be positive.");
            
            return outputOffset / gain;
        }
    }
}
