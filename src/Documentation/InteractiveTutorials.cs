#nullable enable
using System;

namespace CircuitTool
{
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
