using System;
using System.Collections.Generic;

namespace CircuitTool.Calculators
{
    /// <summary>
    /// Comprehensive calculator for capacitance unit conversions and related calculations
    /// </summary>
    public class CapacitanceUnitConverter
    {
        /// <summary>
        /// Capacitance units enumeration
        /// </summary>
        public enum CapacitanceUnit
        {
            Farads,         // F (base unit)
            Millifarads,    // mF (10^-3 F)
            Microfarads,    // μF (10^-6 F)
            Nanofarads,     // nF (10^-9 F)
            Picofarads,     // pF (10^-12 F)
            Femtofarads,    // fF (10^-15 F)
            Attofarads      // aF (10^-18 F)
        }

        /// <summary>
        /// Capacitance conversion result
        /// </summary>
        public class CapacitanceConversionResult
        {
            public double Value { get; set; }
            public CapacitanceUnit Unit { get; set; }
            public string FormattedValue { get; set; }
            public Dictionary<CapacitanceUnit, double> AllUnits { get; set; } = new Dictionary<CapacitanceUnit, double>();
        }

        /// <summary>
        /// Energy and charge calculation result
        /// </summary>
        public class CapacitorEnergyChargeResult
        {
            public double Capacitance { get; set; }         // Farads
            public double Voltage { get; set; }             // Volts
            public double Charge { get; set; }              // Coulombs
            public double Energy { get; set; }              // Joules
            public double EnergyWattHours { get; set; }     // Watt-hours
            public double Current { get; set; }             // Amperes (if time specified)
            public double PowerWatts { get; set; }          // Watts (if time specified)
        }

        /// <summary>
        /// Convert capacitance between units
        /// </summary>
        public CapacitanceConversionResult ConvertCapacitance(double value, CapacitanceUnit fromUnit, CapacitanceUnit toUnit)
        {
            // Convert to Farads first
            double valueInFarads = ConvertToFarads(value, fromUnit);
            
            // Convert from Farads to target unit
            double convertedValue = ConvertFromFarads(valueInFarads, toUnit);
            
            var result = new CapacitanceConversionResult
            {
                Value = convertedValue,
                Unit = toUnit,
                FormattedValue = FormatCapacitance(convertedValue, toUnit)
            };
            
            // Calculate all unit conversions
            foreach (CapacitanceUnit unit in Enum.GetValues(typeof(CapacitanceUnit)))
            {
                result.AllUnits[unit] = ConvertFromFarads(valueInFarads, unit);
            }
            
            return result;
        }

        /// <summary>
        /// Get the best unit for displaying a capacitance value
        /// </summary>
        public CapacitanceConversionResult GetBestUnit(double valueInFarads)
        {
            CapacitanceUnit bestUnit;
            
            if (valueInFarads >= 1e-3)
                bestUnit = CapacitanceUnit.Farads;
            else if (valueInFarads >= 1e-6)
                bestUnit = CapacitanceUnit.Millifarads;
            else if (valueInFarads >= 1e-9)
                bestUnit = CapacitanceUnit.Microfarads;
            else if (valueInFarads >= 1e-12)
                bestUnit = CapacitanceUnit.Nanofarads;
            else if (valueInFarads >= 1e-15)
                bestUnit = CapacitanceUnit.Picofarads;
            else if (valueInFarads >= 1e-18)
                bestUnit = CapacitanceUnit.Femtofarads;
            else
                bestUnit = CapacitanceUnit.Attofarads;
            
            return ConvertCapacitance(valueInFarads, CapacitanceUnit.Farads, bestUnit);
        }

        /// <summary>
        /// Calculate charge stored in capacitor: Q = C × V
        /// </summary>
        public double CalculateCharge(double capacitanceFarads, double voltageVolts)
        {
            return capacitanceFarads * voltageVolts;
        }

        /// <summary>
        /// Calculate energy stored in capacitor: E = 0.5 × C × V²
        /// </summary>
        public double CalculateEnergy(double capacitanceFarads, double voltageVolts)
        {
            return 0.5 * capacitanceFarads * voltageVolts * voltageVolts;
        }

        /// <summary>
        /// Calculate voltage from charge and capacitance: V = Q / C
        /// </summary>
        public double CalculateVoltage(double chargeCoulombs, double capacitanceFarads)
        {
            if (capacitanceFarads <= 0)
                throw new ArgumentException("Capacitance must be positive");
            
            return chargeCoulombs / capacitanceFarads;
        }

        /// <summary>
        /// Calculate capacitance from charge and voltage: C = Q / V
        /// </summary>
        public double CalculateCapacitance(double chargeCoulombs, double voltageVolts)
        {
            if (voltageVolts <= 0)
                throw new ArgumentException("Voltage must be positive");
            
            return chargeCoulombs / voltageVolts;
        }

        /// <summary>
        /// Comprehensive capacitor energy and charge analysis
        /// </summary>
        public CapacitorEnergyChargeResult AnalyzeCapacitor(double capacitanceFarads, double voltageVolts, 
            double timeSeconds = 0, bool isCharging = true)
        {
            var result = new CapacitorEnergyChargeResult
            {
                Capacitance = capacitanceFarads,
                Voltage = voltageVolts,
                Charge = CalculateCharge(capacitanceFarads, voltageVolts),
                Energy = CalculateEnergy(capacitanceFarads, voltageVolts)
            };
            
            // Convert energy to watt-hours
            result.EnergyWattHours = result.Energy / 3600.0;
            
            // If time is specified, calculate current and power
            if (timeSeconds > 0)
            {
                if (isCharging)
                {
                    // For charging: I = C × (dV/dt)
                    // Assuming linear charging from 0V to final voltage
                    result.Current = capacitanceFarads * (voltageVolts / timeSeconds);
                }
                else
                {
                    // For discharging: I = V / R, but we need to know the resistance
                    // We'll calculate average current for complete discharge
                    result.Current = result.Charge / timeSeconds;
                }
                
                result.PowerWatts = voltageVolts * result.Current;
            }
            
            return result;
        }

        /// <summary>
        /// Calculate capacitor charging time to reach a percentage of final voltage
        /// </summary>
        public class ChargingTimeResult
        {
            public double TimeToPercentage { get; set; }    // seconds
            public double TimeConstant { get; set; }        // RC time constant in seconds
            public double VoltageAtTime { get; set; }       // voltage at specified time
            public double ChargeAtTime { get; set; }        // charge at specified time
            public double CurrentAtTime { get; set; }       // current at specified time
        }

        /// <summary>
        /// Calculate RC charging characteristics
        /// </summary>
        public ChargingTimeResult CalculateChargingTime(double capacitanceFarads, double resistanceOhms, 
            double sourceVoltage, double targetPercentage = 63.2, double atTime = 0)
        {
            var result = new ChargingTimeResult();
            
            // Calculate time constant
            result.TimeConstant = resistanceOhms * capacitanceFarads;
            
            // Time to reach target percentage: t = -τ × ln(1 - percentage/100)
            if (targetPercentage > 0 && targetPercentage < 100)
            {
                result.TimeToPercentage = -result.TimeConstant * Math.Log(1 - targetPercentage / 100.0);
            }
            
            // If specific time is given, calculate values at that time
            if (atTime > 0)
            {
                // Voltage: V(t) = Vs × (1 - e^(-t/τ))
                result.VoltageAtTime = sourceVoltage * (1 - Math.Exp(-atTime / result.TimeConstant));
                
                // Charge: Q(t) = C × V(t)
                result.ChargeAtTime = capacitanceFarads * result.VoltageAtTime;
                
                // Current: I(t) = (Vs/R) × e^(-t/τ)
                result.CurrentAtTime = (sourceVoltage / resistanceOhms) * Math.Exp(-atTime / result.TimeConstant);
            }
            
            return result;
        }

        /// <summary>
        /// Calculate capacitor discharge characteristics
        /// </summary>
        public ChargingTimeResult CalculateDischargeTime(double capacitanceFarads, double resistanceOhms, 
            double initialVoltage, double targetPercentage = 36.8, double atTime = 0)
        {
            var result = new ChargingTimeResult();
            
            // Calculate time constant
            result.TimeConstant = resistanceOhms * capacitanceFarads;
            
            // Time to reach target percentage of initial voltage: t = -τ × ln(percentage/100)
            if (targetPercentage > 0 && targetPercentage <= 100)
            {
                result.TimeToPercentage = -result.TimeConstant * Math.Log(targetPercentage / 100.0);
            }
            
            // If specific time is given, calculate values at that time
            if (atTime > 0)
            {
                // Voltage: V(t) = Vi × e^(-t/τ)
                result.VoltageAtTime = initialVoltage * Math.Exp(-atTime / result.TimeConstant);
                
                // Charge: Q(t) = C × V(t)
                result.ChargeAtTime = capacitanceFarads * result.VoltageAtTime;
                
                // Current: I(t) = -(Vi/R) × e^(-t/τ) (negative because discharging)
                result.CurrentAtTime = -(initialVoltage / resistanceOhms) * Math.Exp(-atTime / result.TimeConstant);
            }
            
            return result;
        }

        /// <summary>
        /// Calculate equivalent series resistance (ESR) effects
        /// </summary>
        public class ESRAnalysisResult
        {
            public double ESR { get; set; }                 // ohms
            public double PowerLoss { get; set; }           // watts
            public double EfficiencyPercent { get; set; }   // percentage
            public double VoltageRipple { get; set; }       // volts
            public double QualityFactor { get; set; }       // dimensionless
        }

        /// <summary>
        /// Analyze ESR effects on capacitor performance
        /// </summary>
        public ESRAnalysisResult AnalyzeESR(double capacitanceFarads, double esrOhms, double rmsCurrentAmps, 
            double frequency = 0)
        {
            var result = new ESRAnalysisResult
            {
                ESR = esrOhms
            };
            
            // Power loss in ESR: P = I²R
            result.PowerLoss = rmsCurrentAmps * rmsCurrentAmps * esrOhms;
            
            // Voltage ripple due to ESR: Vripple = I × ESR
            result.VoltageRipple = rmsCurrentAmps * esrOhms;
            
            // If frequency is provided, calculate quality factor
            if (frequency > 0)
            {
                double reactance = 1.0 / (2 * Math.PI * frequency * capacitanceFarads);
                result.QualityFactor = reactance / esrOhms;
                
                // Efficiency considering ESR
                double totalImpedance = Math.Sqrt(reactance * reactance + esrOhms * esrOhms);
                result.EfficiencyPercent = (reactance / totalImpedance) * 100.0;
            }
            
            return result;
        }

        #region Private Helper Methods

        /// <summary>
        /// Convert any unit to Farads
        /// </summary>
        private double ConvertToFarads(double value, CapacitanceUnit fromUnit)
        {
            switch (fromUnit)
            {
                case CapacitanceUnit.Farads:
                    return value;
                case CapacitanceUnit.Millifarads:
                    return value * 1e-3;
                case CapacitanceUnit.Microfarads:
                    return value * 1e-6;
                case CapacitanceUnit.Nanofarads:
                    return value * 1e-9;
                case CapacitanceUnit.Picofarads:
                    return value * 1e-12;
                case CapacitanceUnit.Femtofarads:
                    return value * 1e-15;
                case CapacitanceUnit.Attofarads:
                    return value * 1e-18;
                default:
                    throw new ArgumentException($"Unknown capacitance unit: {fromUnit}");
            }
        }

        /// <summary>
        /// Convert from Farads to any unit
        /// </summary>
        private double ConvertFromFarads(double valueInFarads, CapacitanceUnit toUnit)
        {
            switch (toUnit)
            {
                case CapacitanceUnit.Farads:
                    return valueInFarads;
                case CapacitanceUnit.Millifarads:
                    return valueInFarads / 1e-3;
                case CapacitanceUnit.Microfarads:
                    return valueInFarads / 1e-6;
                case CapacitanceUnit.Nanofarads:
                    return valueInFarads / 1e-9;
                case CapacitanceUnit.Picofarads:
                    return valueInFarads / 1e-12;
                case CapacitanceUnit.Femtofarads:
                    return valueInFarads / 1e-15;
                case CapacitanceUnit.Attofarads:
                    return valueInFarads / 1e-18;
                default:
                    throw new ArgumentException($"Unknown capacitance unit: {toUnit}");
            }
        }

        /// <summary>
        /// Format capacitance value with appropriate unit symbol
        /// </summary>
        private string FormatCapacitance(double value, CapacitanceUnit unit)
        {
            string unitSymbol = GetUnitSymbol(unit);
            
            if (Math.Abs(value) >= 1000)
            {
                return $"{value:0.##e0} {unitSymbol}";
            }
            else
            {
                return $"{value:0.###} {unitSymbol}";
            }
        }

        /// <summary>
        /// Get unit symbol for display
        /// </summary>
        private string GetUnitSymbol(CapacitanceUnit unit)
        {
            switch (unit)
            {
                case CapacitanceUnit.Farads:
                    return "F";
                case CapacitanceUnit.Millifarads:
                    return "mF";
                case CapacitanceUnit.Microfarads:
                    return "μF";
                case CapacitanceUnit.Nanofarads:
                    return "nF";
                case CapacitanceUnit.Picofarads:
                    return "pF";
                case CapacitanceUnit.Femtofarads:
                    return "fF";
                case CapacitanceUnit.Attofarads:
                    return "aF";
                default:
                    return "F";
            }
        }

        #endregion
    }
}
