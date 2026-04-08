#nullable enable
using System;

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
}
