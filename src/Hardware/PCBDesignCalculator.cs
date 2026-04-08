#nullable enable
using System;
using System.Collections.Generic;

namespace CircuitTool
{
    /// <summary>
    /// Provides calculations and utilities for PCB design and layout
    /// </summary>
    public static class PCBDesignCalculator
    {
        /// <summary>
        /// PCB layer stackup types
        /// </summary>
        public enum PCBStackup
        {
            TwoLayer,
            FourLayer,
            SixLayer,
            EightLayer
        }

        /// <summary>
        /// Calculates trace width for current carrying capacity
        /// </summary>
        /// <param name="current">Current in amperes</param>
        /// <param name="copperThickness">Copper thickness in oz (typically 1 or 2)</param>
        /// <param name="temperatureRise">Allowable temperature rise in °C</param>
        /// <param name="isInternal">True if trace is internal layer</param>
        /// <returns>Required trace width in mils</returns>
        public static double CalculateTraceWidth(double current, double copperThickness = 1.0, 
            double temperatureRise = 10.0, bool isInternal = false)
        {
            // IPC-2221 formula
            double k = isInternal ? 0.024 : 0.048;
            double b = isInternal ? 0.44 : 0.44;
            double c = isInternal ? 0.725 : 0.725;
            
            // Convert current to mA for formula
            double currentMA = current * 1000;
            
            // Area in square mils
            double area = Math.Pow(currentMA / (k * Math.Pow(temperatureRise, b)), 1 / c);
            
            // Convert area to width (assuming 1.4 mils thickness for 1oz copper)
            double thickness = copperThickness * 1.4;
            return area / thickness;
        }

        /// <summary>
        /// Calculates via current carrying capacity
        /// </summary>
        /// <param name="viaDiameter">Via barrel diameter in mils</param>
        /// <param name="viaLength">Via length (PCB thickness) in mils</param>
        /// <param name="copperThickness">Plating thickness in mils</param>
        /// <param name="temperatureRise">Allowable temperature rise in °C</param>
        /// <returns>Maximum current capacity in amperes</returns>
        public static double CalculateViaCurrent(double viaDiameter, double viaLength, 
            double copperThickness = 1.0, double temperatureRise = 10.0)
        {
            // Via barrel area
            double barrelArea = Math.PI * viaDiameter * copperThickness;
            
            // Using modified IPC-2221 for vias
            double k = 0.048; // External trace constant
            double b = 0.44;
            double c = 0.725;
            
            // Current in mA
            double currentMA = k * Math.Pow(temperatureRise, b) * Math.Pow(barrelArea, c);
            
            return currentMA / 1000; // Convert to amperes
        }

        /// <summary>
        /// Calculates differential pair impedance for high-speed signals
        /// </summary>
        /// <param name="traceWidth">Trace width in mils</param>
        /// <param name="traceSpacing">Spacing between traces in mils</param>
        /// <param name="dielectricHeight">Height above ground plane in mils</param>
        /// <param name="dielectricConstant">PCB dielectric constant (typically 4.2-4.8)</param>
        /// <returns>Differential impedance in ohms</returns>
        public static double CalculateDifferentialImpedance(double traceWidth, double traceSpacing, 
            double dielectricHeight, double dielectricConstant = 4.3)
        {
            // Simplified microstrip differential pair formula
            double w = traceWidth;
            double s = traceSpacing;
            double h = dielectricHeight;
            double er = dielectricConstant;
            
            // Single-ended impedance first
            double z0 = 87 / Math.Sqrt(er + 1.41) * Math.Log(5.98 * h / (0.8 * w + traceWidth));
            
            // Differential pair coupling factor
            double couplingFactor = 1 - 0.48 * Math.Exp(-0.96 * s / h);
            
            return 2 * z0 * couplingFactor;
        }

        /// <summary>
        /// Calculates PCB conductor resistance
        /// </summary>
        /// <param name="length">Conductor length in inches</param>
        /// <param name="width">Conductor width in mils</param>
        /// <param name="thickness">Copper thickness in oz</param>
        /// <param name="temperature">Operating temperature in °C</param>
        /// <returns>Resistance in ohms</returns>
        public static double CalculateConductorResistance(double length, double width, 
            double thickness = 1.0, double temperature = 25.0)
        {
            // Copper resistivity at 20°C in ohm⋅cm
            double rho20 = 1.68e-6;
            
            // Temperature coefficient
            double tempCoeff = 0.0039; // per °C
            double rhoT = rho20 * (1 + tempCoeff * (temperature - 20));
            
            // Convert dimensions
            double lengthCm = length * 2.54;
            double widthCm = width * 2.54e-3;
            double thicknessCm = thickness * 35e-4; // 1oz = 35µm = 0.0035cm
            
            double area = widthCm * thicknessCm;
            return rhoT * lengthCm / area;
        }

        /// <summary>
        /// Calculates PCB capacitance between layers
        /// </summary>
        /// <param name="area">Overlap area in square inches</param>
        /// <param name="dielectricThickness">Dielectric thickness in mils</param>
        /// <param name="dielectricConstant">Dielectric constant</param>
        /// <returns>Capacitance in picofarads</returns>
        public static double CalculatePCBCapacitance(double area, double dielectricThickness, 
            double dielectricConstant = 4.3)
        {
            // Convert to SI units
            double areaSI = area * 6.452e-4; // square inches to square meters
            double thicknessSI = dielectricThickness * 25.4e-6; // mils to meters
            
            // Permittivity of free space
            double epsilon0 = 8.854e-12; // F/m
            
            double capacitance = epsilon0 * dielectricConstant * areaSI / thicknessSI;
            return capacitance * 1e12; // Convert to picofarads
        }

        /// <summary>
        /// Calculates minimum annular ring for vias
        /// </summary>
        /// <param name="drillDiameter">Drill diameter in mils</param>
        /// <param name="pcbClass">PCB class (1, 2, or 3)</param>
        /// <param name="layerCount">Number of layers</param>
        /// <returns>Minimum annular ring in mils</returns>
        public static double CalculateMinimumAnnularRing(double drillDiameter, int pcbClass = 2, int layerCount = 4)
        {
            // IPC-6012 guidelines
            double baseRing = pcbClass switch
            {
                1 => 2.0, // Class 1: General electronics
                2 => 3.0, // Class 2: Dedicated service
                3 => 4.0, // Class 3: High reliability
                _ => 3.0
            };
            
            // Adjustment for layer count and drill size
            if (layerCount > 6) baseRing += 1.0;
            if (drillDiameter < 8.0) baseRing += 1.0; // Small via penalty
            
            return baseRing;
        }

        /// <summary>
        /// Calculates solder mask expansion
        /// </summary>
        /// <param name="padDimension">Pad dimension in mils</param>
        /// <param name="pitchDistance">Component pitch in mils</param>
        /// <returns>Recommended solder mask expansion in mils</returns>
        public static double CalculateSolderMaskExpansion(double padDimension, double pitchDistance)
        {
            // Fine pitch components need tighter tolerances
            if (pitchDistance < 20) return 2.0; // Fine pitch (0.5mm)
            if (pitchDistance < 40) return 3.0; // Medium pitch (1.0mm)
            return 4.0; // Standard pitch
        }

        /// <summary>
        /// Generates PCB design rules summary
        /// </summary>
        /// <param name="stackup">PCB layer stackup</param>
        /// <param name="pcbClass">PCB class</param>
        /// <param name="signalFrequency">Maximum signal frequency in MHz</param>
        /// <returns>Design rules text</returns>
        public static string GenerateDesignRules(PCBStackup stackup, int pcbClass = 2, double signalFrequency = 100)
        {
            var rules = new List<string>
            {
                $"=== PCB Design Rules ({stackup}, Class {pcbClass}) ===",
                "",
                "TRACE WIDTHS:",
                $"• Minimum trace width: {(pcbClass == 3 ? 4 : 6)} mils",
                $"• Power traces (1A): {CalculateTraceWidth(1.0):F1} mils",
                $"• Signal traces (100mA): {CalculateTraceWidth(0.1):F1} mils",
                "",
                "SPACING:",
                $"• Minimum trace spacing: {(pcbClass == 3 ? 4 : 6)} mils",
                $"• Trace to via: {(pcbClass == 3 ? 5 : 8)} mils",
                "",
                "VIAS:",
                $"• Minimum via size: {(pcbClass == 3 ? 8 : 10)} mils drill",
                $"• Minimum annular ring: {CalculateMinimumAnnularRing(10, pcbClass):F1} mils",
                "",
                "HIGH-SPEED SIGNALS:"
            };
            
            if (signalFrequency > 50)
            {
                rules.Add($"• Controlled impedance required (50Ω ±10%)");
                rules.Add($"• Maximum trace length without termination: {3000 / signalFrequency:F1} mils");
                rules.Add($"• Via stub length: < {1500 / signalFrequency:F1} mils");
            }
            
            return string.Join("\n", rules);
        }

        /// <summary>
        /// Calculates PCB thermal resistance
        /// </summary>
        /// <param name="copperArea">Copper area in square inches</param>
        /// <param name="copperThickness">Copper thickness in oz</param>
        /// <param name="airflow">Air flow velocity in m/s (0 for natural convection)</param>
        /// <returns>Thermal resistance in °C/W</returns>
        public static double CalculateThermalResistance(double copperArea, double copperThickness = 1.0, double airflow = 0)
        {
            // Thermal conductivity of copper: 400 W/(m⋅K)
            double copperThermalConductivity = 400;
            
            // Convert area to square meters
            double areaSI = copperArea * 6.452e-4;
            
            // Copper thickness in meters (1oz = 35µm)
            double thicknessSI = copperThickness * 35e-6;
            
            // Conduction resistance through copper
            double rConduction = thicknessSI / (copperThermalConductivity * areaSI);
            
            // Convection resistance (simplified)
            double hConvection = airflow > 0 ? 
                10 + 6 * Math.Sqrt(airflow) : // Forced convection
                5; // Natural convection
            
            double rConvection = 1.0 / (hConvection * areaSI);
            
            return rConduction + rConvection;
        }
    }
}
