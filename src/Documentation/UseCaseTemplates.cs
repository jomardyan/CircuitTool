#nullable enable
using System;

namespace CircuitTool
{
    /// <summary>
    /// Common use case templates for quick implementation
    /// </summary>
    public static class UseCaseTemplates
    {
        /// <summary>
        /// Template for designing a voltage divider
        /// </summary>
        public static class VoltageDividerDesign
        {
            public static (double R1, double R2) DesignVoltageDivider(
                double inputVoltage, 
                double outputVoltage, 
                double totalCurrent = 0.001) // Default 1mA
            {
                double totalResistance = inputVoltage / totalCurrent;
                double ratio = outputVoltage / inputVoltage;
                
                double r2 = totalResistance * ratio;
                double r1 = totalResistance - r2;
                
                return (r1, r2);
            }
            
            public static string GenerateCode(double inputVoltage, double outputVoltage)
            {
                var (r1, r2) = DesignVoltageDivider(inputVoltage, outputVoltage);
                
                return $@"
// Voltage divider: {inputVoltage}V → {outputVoltage}V
double r1 = {r1:F0}; // Upper resistor (Ohms)
double r2 = {r2:F0}; // Lower resistor (Ohms)

// Verify the output
double actualOutput = VoltageDividerCalculator.Calculate(
    {inputVoltage}, r1, r2);
// Result: {{actualOutput:F2}}V
";
            }
        }

        /// <summary>
        /// Template for RC filter design
        /// </summary>
        public static class FilterDesign
        {
            public static (double R, double C) DesignLowPassFilter(double cutoffFrequency)
            {
                // Choose a standard capacitor value
                double capacitance = 1e-6; // 1µF
                double resistance = 1.0 / (2 * Math.PI * cutoffFrequency * capacitance);
                
                return (resistance, capacitance);
            }
            
            public static string GenerateLowPassCode(double cutoffFrequency)
            {
                var (r, c) = DesignLowPassFilter(cutoffFrequency);
                
                return $@"
// Low-pass filter with cutoff at {cutoffFrequency}Hz
double resistance = {r:F0}; // Ohms
double capacitance = {c:E2}; // Farads ({c*1e6:F0}µF)

// Verify cutoff frequency
double actualCutoff = FilterCalculator.RCLowPassCutoffFrequency(
    resistance, capacitance);
// Result: {{actualCutoff:F1}}Hz
";
            }
        }

        /// <summary>
        /// Template for LED current limiting resistor
        /// </summary>
        public static class LEDResistorCalculator
        {
            public static double CalculateCurrentLimitingResistor(
                double supplyVoltage,
                double ledVoltage,
                double ledCurrent)
            {
                return (supplyVoltage - ledVoltage) / ledCurrent;
            }
            
            public static string GenerateCode(double supplyVoltage, double ledVoltage, double ledCurrent)
            {
                double resistance = CalculateCurrentLimitingResistor(supplyVoltage, ledVoltage, ledCurrent);
                
                return $@"
// LED current limiting resistor
// Supply: {supplyVoltage}V, LED: {ledVoltage}V @ {ledCurrent*1000}mA
double resistance = ({supplyVoltage} - {ledVoltage}) / {ledCurrent};
// Result: {{resistance:F0}}Ω

// Power dissipation in resistor
double voltageDrop = {supplyVoltage} - {ledVoltage};
double power = voltageDrop * {ledCurrent};
// Result: {{power:F3}}W
";
            }
        }
    }
}
