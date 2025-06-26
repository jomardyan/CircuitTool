#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CircuitTool
{
    /// <summary>
    /// Interactive documentation examples demonstrating common use cases
    /// </summary>
    public static class DocumentationExamples
    {
        /// <summary>
        /// Basic Ohm's Law calculations example
        /// </summary>
        public static class BasicOhmsLaw
        {
            public static void RunExample()
            {
                Console.WriteLine("=== Basic Ohm's Law Calculations ===");
                
                // Example 1: Calculate current from voltage and resistance
                double voltage = 12.0; // 12V
                double resistance = 100.0; // 100Ω
                double current = OhmsLawCalculator.Current(voltage, resistance);
                
                Console.WriteLine($"V = {voltage}V, R = {resistance}Ω");
                Console.WriteLine($"I = V/R = {current:F3}A");
                Console.WriteLine();
                
                // Example 2: Calculate power consumption
                double power = PowerCalculator.Power(voltage, current);
                Console.WriteLine($"P = V × I = {power:F3}W");
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Circuit building example using basic calculations
        /// </summary>
        public static class CircuitBuilding
        {
            public static void RunExample()
            {
                Console.WriteLine("=== Circuit Building Example ===");
                
                // Voltage divider calculation
                double inputVoltage = 12.0;
                double r1 = 1000; // 1kΩ
                double r2 = 2000; // 2kΩ
                
                Console.WriteLine("Built a voltage divider:");
                Console.WriteLine("VIN ──[R1: 1kΩ]──[R2: 2kΩ]── GND");
                Console.WriteLine();
                
                // Calculate voltage division
                double outputVoltage = VoltageDividerCalculator.Calculate(inputVoltage, r1, r2);
                
                Console.WriteLine($"Input: {inputVoltage}V");
                Console.WriteLine($"Output: {outputVoltage:F2}V");
                Console.WriteLine();
            }
        }

        /// <summary>
        /// AC circuit analysis example
        /// </summary>
        public static class ACAnalysis
        {
            public static void RunExample()
            {
                Console.WriteLine("=== AC Circuit Analysis ===");
                
                // RC low-pass filter analysis
                double resistance = 1000; // 1kΩ
                double capacitance = 1e-6; // 1µF
                double frequency = 1000;   // 1kHz
                
                var cutoffFreq = FilterCalculator.RCLowPassCutoffFrequency(resistance, capacitance);
                
                Console.WriteLine($"RC Low-pass filter: R={resistance}Ω, C={capacitance*1e6}µF");
                Console.WriteLine($"At f={frequency}Hz:");
                Console.WriteLine($"  Cutoff frequency: {cutoffFreq:F1}Hz");
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Energy calculation example
        /// </summary>
        public static class EnergyCalculations
        {
            public static void RunExample()
            {
                Console.WriteLine("=== Energy Calculations ===");
                
                // LED array power consumption
                int numLEDs = 50;
                double ledVoltage = 3.3;
                double ledCurrent = 0.02; // 20mA
                double hoursPerDay = 8;
                double daysPerMonth = 30;
                
                var totalPower = PowerCalculator.Power(ledVoltage * numLEDs, ledCurrent);
                var monthlyEnergy = EnergyCalculator.KWh(totalPower, hoursPerDay * daysPerMonth) * 1000; // Convert back to Wh
                
                Console.WriteLine($"LED Array: {numLEDs} LEDs @ {ledVoltage}V, {ledCurrent*1000}mA each");
                Console.WriteLine($"Total power: {totalPower:F1}W");
                Console.WriteLine($"Monthly energy ({hoursPerDay}h/day): {monthlyEnergy:F2}Wh");
                
                // Cost calculation  
                double costPerKWh = 0.12; // $0.12 per kWh
                double monthlyCost = ElectricityBillCalculator.CalculateBill(monthlyEnergy / 1000, costPerKWh);
                Console.WriteLine($"Monthly cost @ ${costPerKWh}/kWh: ${monthlyCost:F2}");
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Runs all documentation examples
        /// </summary>
        public static void RunAllExamples()
        {
            Console.WriteLine("CircuitTool Documentation Examples");
            Console.WriteLine("==================================");
            Console.WriteLine();
            
            BasicOhmsLaw.RunExample();
            CircuitBuilding.RunExample();
            ACAnalysis.RunExample();
            EnergyCalculations.RunExample();
            
            Console.WriteLine("All examples completed!");
        }
    }

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

    /// <summary>
    /// Interactive tutorial system
    /// </summary>
    public static class InteractiveTutorials
    {
        public static void RunBasicElectronicsTutorial()
        {
            Console.WriteLine("=== Basic Electronics Tutorial ===");
            Console.WriteLine();
            
            // Step 1: Ohm's Law
            Console.WriteLine("Step 1: Understanding Ohm's Law");
            Console.WriteLine("Ohm's Law states: V = I × R");
            Console.WriteLine("Where V = Voltage, I = Current, R = Resistance");
            Console.WriteLine();
            
            // Interactive example
            Console.WriteLine("Let's calculate the current through a 100Ω resistor with 5V applied:");
            double voltage = 5.0;
            double resistance = 100.0;
            double current = OhmsLawCalculator.Current(voltage, resistance);
            
            Console.WriteLine($"I = V/R = {voltage}V/{resistance}Ω = {current:F3}A");
            Console.WriteLine();
            
            // Step 2: Power calculation
            Console.WriteLine("Step 2: Power Calculation");
            Console.WriteLine("Power can be calculated as: P = V × I = I² × R = V²/R");
            Console.WriteLine();
            
            double power = PowerCalculator.Power(voltage, current);
            Console.WriteLine($"P = V × I = {voltage}V × {current:F3}A = {power:F3}W");
            Console.WriteLine();
            
            // Step 3: Series circuits
            Console.WriteLine("Step 3: Series Circuits");
            Console.WriteLine("In series: Total resistance = R1 + R2 + R3...");
            Console.WriteLine("Current is the same through all components");
            Console.WriteLine();
            
            double r1 = 100, r2 = 200, r3 = 300;
            double totalSeries = Calculators.ComponentCalculator.SeriesResistance(r1, r2, r3);
            Console.WriteLine($"Series: {r1}Ω + {r2}Ω + {r3}Ω = {totalSeries}Ω");
            Console.WriteLine();
            
            // Step 4: Parallel circuits
            Console.WriteLine("Step 4: Parallel Circuits");
            Console.WriteLine("In parallel: 1/Total = 1/R1 + 1/R2 + 1/R3...");
            Console.WriteLine("Voltage is the same across all components");
            Console.WriteLine();
            
            double totalParallel = Calculators.ComponentCalculator.ParallelResistance(r1, r2, r3);
            Console.WriteLine($"Parallel: {r1}Ω || {r2}Ω || {r3}Ω = {totalParallel:F1}Ω");
            Console.WriteLine();
            
            Console.WriteLine("Tutorial completed! Try experimenting with different values.");
        }
    }
}
