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

        /// <summary>
        /// LED array configuration result
        /// </summary>
        public class LEDArrayResult
        {
            public int SeriesLEDs { get; set; }
            public int ParallelBranches { get; set; }
            public double ResistorValue { get; set; }
            public double ResistorPower { get; set; }
            public double TotalPower { get; set; }
            public double Efficiency { get; set; }
            public double CurrentPerBranch { get; set; }
            public double VoltageDropAcrossResistor { get; set; }
            public List<string> Warnings { get; set; } = new List<string>();
        }

        /// <summary>
        /// LED specifications for calculations
        /// </summary>
        public class LEDSpec
        {
            public double ForwardVoltage { get; set; }      // Typical forward voltage (V)
            public double ForwardCurrent { get; set; }      // Rated forward current (A)
            public double MaxCurrent { get; set; }          // Maximum forward current (A)
            public double LuminousFlux { get; set; }        // Light output (lumens)
            public string Color { get; set; }               // LED color
        }

        /// <summary>
        /// Calculate optimal LED array configuration
        /// </summary>
        public static LEDArrayResult CalculateLEDArray(double supplyVoltage, int totalLEDs, 
            LEDSpec ledSpec, double targetCurrent = 0)
        {
            var result = new LEDArrayResult();
            
            // Use rated current if target not specified
            if (targetCurrent <= 0)
                targetCurrent = ledSpec.ForwardCurrent;
            
            // Validate target current
            if (targetCurrent > ledSpec.MaxCurrent)
            {
                result.Warnings.Add($"Target current ({targetCurrent:0.000}A) exceeds maximum LED current ({ledSpec.MaxCurrent:0.000}A)");
                targetCurrent = ledSpec.MaxCurrent;
            }
            
            // Calculate maximum LEDs in series
            double voltagePerLED = ledSpec.ForwardVoltage;
            int maxSeriesLEDs = (int)Math.Floor((supplyVoltage - 2.0) / voltagePerLED); // Leave 2V for resistor
            
            if (maxSeriesLEDs <= 0)
            {
                result.Warnings.Add("Supply voltage too low for even one LED");
                return result;
            }
            
            // Find optimal configuration
            int bestSeries = 1;
            double bestEfficiency = 0;
            
            for (int series = 1; series <= Math.Min(maxSeriesLEDs, totalLEDs); series++)
            {
                int parallel = (int)Math.Ceiling((double)totalLEDs / series);
                double totalLEDVoltage = series * voltagePerLED;
                double resistorVoltage = supplyVoltage - totalLEDVoltage;
                
                if (resistorVoltage >= 0.5) // Minimum voltage for stable operation
                {
                    double efficiency = totalLEDVoltage / supplyVoltage;
                    if (efficiency > bestEfficiency)
                    {
                        bestEfficiency = efficiency;
                        bestSeries = series;
                    }
                }
            }
            
            // Calculate final configuration
            result.SeriesLEDs = bestSeries;
            result.ParallelBranches = (int)Math.Ceiling((double)totalLEDs / bestSeries);
            
            double totalLEDVoltageOptimal = result.SeriesLEDs * voltagePerLED;
            result.VoltageDropAcrossResistor = supplyVoltage - totalLEDVoltageOptimal;
            result.ResistorValue = result.VoltageDropAcrossResistor / targetCurrent;
            result.CurrentPerBranch = targetCurrent;
            result.ResistorPower = targetCurrent * targetCurrent * result.ResistorValue;
            
            // Total power calculation
            double ledPowerPerBranch = result.SeriesLEDs * voltagePerLED * targetCurrent;
            result.TotalPower = (ledPowerPerBranch + result.ResistorPower) * result.ParallelBranches;
            result.Efficiency = (ledPowerPerBranch * result.ParallelBranches) / result.TotalPower * 100;
            
            // Add warnings for efficiency
            if (result.Efficiency < 70)
            {
                result.Warnings.Add($"Low efficiency ({result.Efficiency:0.1}%) - consider higher supply voltage or different LED configuration");
            }
            
            if (result.ResistorPower > 0.5)
            {
                result.Warnings.Add($"High resistor power dissipation ({result.ResistorPower:0.2}W) - use appropriate power rating");
            }
            
            return result;
        }

        /// <summary>
        /// Calculate resistor for specific LED configuration
        /// </summary>
        public static double CalculateArrayResistor(double supplyVoltage, int seriesLEDs, 
            double ledForwardVoltage, double targetCurrent)
        {
            double totalLEDVoltage = seriesLEDs * ledForwardVoltage;
            if (supplyVoltage <= totalLEDVoltage)
                throw new ArgumentException("Supply voltage insufficient for LED configuration");
            
            return (supplyVoltage - totalLEDVoltage) / targetCurrent;
        }

        /// <summary>
        /// Calculate LED current with known resistor
        /// </summary>
        public static double CalculateLEDCurrent(double supplyVoltage, int seriesLEDs, 
            double ledForwardVoltage, double resistorValue)
        {
            double totalLEDVoltage = seriesLEDs * ledForwardVoltage;
            if (supplyVoltage <= totalLEDVoltage)
                return 0;
            
            return (supplyVoltage - totalLEDVoltage) / resistorValue;
        }

        /// <summary>
        /// LED thermal analysis result
        /// </summary>
        public class LEDThermalResult
        {
            public double JunctionTemperature { get; set; }    // °C
            public double ThermalResistance { get; set; }      // °C/W
            public double PowerDissipation { get; set; }       // W
            public double AmbientTemperature { get; set; }     // °C
            public double MaxSafePower { get; set; }           // W
            public bool RequiresHeatsink { get; set; }
            public List<string> Recommendations { get; set; } = new List<string>();
        }

        /// <summary>
        /// Analyze LED thermal characteristics
        /// </summary>
        public static LEDThermalResult AnalyzeLEDThermal(double forwardVoltage, double forwardCurrent,
            double thermalResistance = 300, double ambientTemp = 25, double maxJunctionTemp = 85)
        {
            var result = new LEDThermalResult
            {
                ThermalResistance = thermalResistance,
                AmbientTemperature = ambientTemp,
                PowerDissipation = forwardVoltage * forwardCurrent
            };
            
            // Calculate junction temperature: Tj = Ta + (P × Rth)
            result.JunctionTemperature = ambientTemp + (result.PowerDissipation * thermalResistance);
            
            // Calculate maximum safe power
            result.MaxSafePower = (maxJunctionTemp - ambientTemp) / thermalResistance;
            
            // Determine if heatsink is required
            result.RequiresHeatsink = result.JunctionTemperature > maxJunctionTemp;
            
            // Add recommendations
            if (result.RequiresHeatsink)
            {
                result.Recommendations.Add("Heatsink or better thermal management required");
                double requiredThermalResistance = (maxJunctionTemp - ambientTemp) / result.PowerDissipation;
                result.Recommendations.Add($"Required thermal resistance: {requiredThermalResistance:0.1} °C/W or better");
            }
            
            if (result.PowerDissipation > result.MaxSafePower)
            {
                result.Recommendations.Add($"Reduce current to stay within thermal limits (max safe power: {result.MaxSafePower:0.3}W)");
            }
            
            if (result.JunctionTemperature > maxJunctionTemp * 0.9)
            {
                result.Recommendations.Add("Operating close to thermal limits - monitor temperature");
            }
            
            return result;
        }

        /// <summary>
        /// Common LED specifications database
        /// </summary>
        public static class CommonLEDs
        {
            public static LEDSpec Red3mm => new LEDSpec { ForwardVoltage = 2.0, ForwardCurrent = 0.020, MaxCurrent = 0.030, Color = "Red" };
            public static LEDSpec Green3mm => new LEDSpec { ForwardVoltage = 2.1, ForwardCurrent = 0.020, MaxCurrent = 0.030, Color = "Green" };
            public static LEDSpec Blue3mm => new LEDSpec { ForwardVoltage = 3.2, ForwardCurrent = 0.020, MaxCurrent = 0.030, Color = "Blue" };
            public static LEDSpec Yellow3mm => new LEDSpec { ForwardVoltage = 2.1, ForwardCurrent = 0.020, MaxCurrent = 0.030, Color = "Yellow" };
            public static LEDSpec White3mm => new LEDSpec { ForwardVoltage = 3.2, ForwardCurrent = 0.020, MaxCurrent = 0.030, Color = "White" };
            
            public static LEDSpec Red5mm => new LEDSpec { ForwardVoltage = 2.0, ForwardCurrent = 0.020, MaxCurrent = 0.030, Color = "Red" };
            public static LEDSpec Green5mm => new LEDSpec { ForwardVoltage = 2.1, ForwardCurrent = 0.020, MaxCurrent = 0.030, Color = "Green" };
            public static LEDSpec Blue5mm => new LEDSpec { ForwardVoltage = 3.2, ForwardCurrent = 0.020, MaxCurrent = 0.030, Color = "Blue" };
            public static LEDSpec Yellow5mm => new LEDSpec { ForwardVoltage = 2.1, ForwardCurrent = 0.020, MaxCurrent = 0.030, Color = "Yellow" };
            public static LEDSpec White5mm => new LEDSpec { ForwardVoltage = 3.2, ForwardCurrent = 0.020, MaxCurrent = 0.030, Color = "White" };
            
            public static LEDSpec HighPowerWhite1W => new LEDSpec { ForwardVoltage = 3.2, ForwardCurrent = 0.350, MaxCurrent = 0.700, Color = "White", LuminousFlux = 100 };
            public static LEDSpec HighPowerWhite3W => new LEDSpec { ForwardVoltage = 3.4, ForwardCurrent = 0.700, MaxCurrent = 1.000, Color = "White", LuminousFlux = 280 };
            
            public static LEDSpec InfraredIR => new LEDSpec { ForwardVoltage = 1.2, ForwardCurrent = 0.100, MaxCurrent = 0.200, Color = "Infrared" };
            public static LEDSpec UVLed => new LEDSpec { ForwardVoltage = 3.4, ForwardCurrent = 0.020, MaxCurrent = 0.030, Color = "UV" };
        }

        /// <summary>
        /// Calculate LED strip resistor values
        /// </summary>
        public class LEDStripResult
        {
            public double ResistorValuePerSegment { get; set; }
            public double PowerPerSegment { get; set; }
            public double TotalPower { get; set; }
            public int SegmentCount { get; set; }
            public double CurrentPerSegment { get; set; }
            public double EfficiencyPercent { get; set; }
        }

        /// <summary>
        /// Calculate resistors for LED strip segments
        /// </summary>
        public static LEDStripResult CalculateLEDStripResistors(double supplyVoltage, int ledsPerSegment,
            LEDSpec ledSpec, int totalSegments, double targetCurrent = 0)
        {
            if (targetCurrent <= 0)
                targetCurrent = ledSpec.ForwardCurrent;
            
            var result = new LEDStripResult
            {
                SegmentCount = totalSegments,
                CurrentPerSegment = targetCurrent
            };
            
            double totalLEDVoltagePerSegment = ledsPerSegment * ledSpec.ForwardVoltage;
            result.ResistorValuePerSegment = (supplyVoltage - totalLEDVoltagePerSegment) / targetCurrent;
            result.PowerPerSegment = targetCurrent * targetCurrent * result.ResistorValuePerSegment;
            result.TotalPower = (totalLEDVoltagePerSegment * targetCurrent + result.PowerPerSegment) * totalSegments;
            
            double ledPowerPerSegment = totalLEDVoltagePerSegment * targetCurrent;
            result.EfficiencyPercent = (ledPowerPerSegment * totalSegments) / result.TotalPower * 100;
            
            return result;
        }
    }
}
