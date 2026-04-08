using System;
using System.Collections.Generic;
#if !NET20
using System.Linq;
#endif

namespace CircuitTool
{
    /// <summary>
    /// Provides easy-to-use methods for capacitor circuit calculations, including reactance, energy, time constants, and more.
    /// </summary>
    /// <remarks>
    /// <para>Example usage:</para>
    /// <code>
    /// double xc = CapacitorCalculator.CapacitiveReactance(1000, 0.000001); // 159.15 Ohms
    /// double energy = CapacitorCalculator.EnergyStored(0.001, 5); // 0.0125 J
    /// double tau = CapacitorCalculator.TimeConstant(1000, 0.000001); // 0.001 s
    /// double cSeries = CapacitorCalculator.SeriesCapacitance(new[] {0.000001, 0.000002});
    /// double cParallel = CapacitorCalculator.ParallelCapacitance(new[] {0.000001, 0.000002});
    /// double vCharge = CapacitorCalculator.ChargingVoltage(5, 0.001, 0.002);
    /// double vDischarge = CapacitorCalculator.DischargingVoltage(5, 0.001, 0.002);
    /// </code>
    /// </remarks>
    public static class CapacitorCalculator
    {
        /// <summary>
        /// Calculates capacitive reactance using the formula Xc = 1 / (2πfC).
        /// </summary>
        /// <param name="frequency">Frequency in hertz (Hz).</param>
        /// <param name="capacitance">Capacitance in farads (F).</param>
        /// <returns>Capacitive reactance in ohms (Ω).</returns>
        /// <example>
        /// double xc = CapacitorCalculator.CapacitiveReactance(1000, 0.000001); // 159.15 Ohms
        /// </example>
        public static double CapacitiveReactance(double frequency, double capacitance)
        {
            if (frequency <= 0 || capacitance <= 0)
                throw new ArgumentException("Frequency and capacitance must be positive values.");
            return 1.0 / (2 * Math.PI * frequency * capacitance);
        }

        /// <summary>
        /// Calculates the energy stored in a capacitor using the formula E = 0.5 × C × V².
        /// </summary>
        /// <param name="capacitance">Capacitance in farads (F).</param>
        /// <param name="voltage">Voltage across the capacitor in volts (V).</param>
        /// <returns>Energy stored in joules (J).</returns>
        /// <example>
        /// double energy = CapacitorCalculator.EnergyStored(0.001, 5); // 0.0125 J
        /// </example>
        public static double EnergyStored(double capacitance, double voltage)
        {
            if (capacitance < 0 || voltage < 0)
                throw new ArgumentException("Capacitance and voltage must be non-negative values.");
            return 0.5 * capacitance * voltage * voltage;
        }

        /// <summary>
        /// Calculates the time constant for an RC circuit using the formula τ = R × C.
        /// </summary>
        /// <param name="resistance">Resistance in ohms (Ω).</param>
        /// <param name="capacitance">Capacitance in farads (F).</param>
        /// <returns>Time constant in seconds (s).</returns>
        /// <example>
        /// double tau = CapacitorCalculator.TimeConstant(1000, 0.000001); // 0.001 s
        /// </example>
        public static double TimeConstant(double resistance, double capacitance)
        {
            if (resistance < 0 || capacitance < 0)
                throw new ArgumentException("Resistance and capacitance must be non-negative values.");
            return resistance * capacitance;
        }

        /// <summary>
        /// Calculates total capacitance for capacitors in series using the formula 1/Ctotal = 1/C1 + 1/C2 + ...
        /// </summary>
        /// <param name="capacitances">Array of capacitance values in farads (F).</param>
        /// <returns>Total capacitance in farads (F).</returns>
        /// <example>
        /// double cSeries = CapacitorCalculator.SeriesCapacitance(new[] {0.000001, 0.000002});
        /// </example>
        public static double SeriesCapacitance(double[] capacitances)
        {
            if (capacitances == null || capacitances.Length == 0)
                throw new ArgumentException("Capacitances array cannot be null or empty.");
            double reciprocalSum = 0;
            foreach (var capacitance in capacitances)
            {
                if (capacitance <= 0)
                    throw new ArgumentException("All capacitance values must be positive.");
                reciprocalSum += 1.0 / capacitance;
            }
            return 1.0 / reciprocalSum;
        }

        /// <summary>
        /// Calculates total capacitance for capacitors in parallel using the formula Ctotal = C1 + C2 + ...
        /// </summary>
        /// <param name="capacitances">Array of capacitance values in farads (F).</param>
        /// <returns>Total capacitance in farads (F).</returns>
        /// <example>
        /// double cParallel = CapacitorCalculator.ParallelCapacitance(new[] {0.000001, 0.000002});
        /// </example>
        public static double ParallelCapacitance(double[] capacitances)
        {
            if (capacitances == null || capacitances.Length == 0)
                throw new ArgumentException("Capacitances array cannot be null or empty.");
            double totalCapacitance = 0;
            foreach (var capacitance in capacitances)
            {
                if (capacitance < 0)
                    throw new ArgumentException("All capacitance values must be non-negative.");
                totalCapacitance += capacitance;
            }
            return totalCapacitance;
        }

        /// <summary>
        /// Calculates the charging voltage of a capacitor at time t using V(t) = Vsource × (1 - e^(-t/τ)).
        /// </summary>
        /// <param name="sourceVoltage">Source voltage in volts (V).</param>
        /// <param name="timeConstant">Time constant τ in seconds (s).</param>
        /// <param name="time">Time in seconds (s).</param>
        /// <returns>Capacitor voltage at time t in volts (V).</returns>
        /// <example>
        /// double vCharge = CapacitorCalculator.ChargingVoltage(5, 0.001, 0.002);
        /// </example>
        public static double ChargingVoltage(double sourceVoltage, double timeConstant, double time)
        {
            if (timeConstant <= 0)
                throw new ArgumentException("Time constant must be positive.");
            if (time < 0)
                throw new ArgumentException("Time must be non-negative.");
            return sourceVoltage * (1 - Math.Exp(-time / timeConstant));
        }

        /// <summary>
        /// Calculates the discharging voltage of a capacitor at time t using V(t) = Vinitial × e^(-t/τ).
        /// </summary>
        /// <param name="initialVoltage">Initial voltage in volts (V).</param>
        /// <param name="timeConstant">Time constant τ in seconds (s).</param>
        /// <param name="time">Time in seconds (s).</param>
        /// <returns>Capacitor voltage at time t in volts (V).</returns>
        /// <example>
        /// double vDischarge = CapacitorCalculator.DischargingVoltage(5, 0.001, 0.002);
        /// </example>
        public static double DischargingVoltage(double initialVoltage, double timeConstant, double time)
        {
            if (timeConstant <= 0)
                throw new ArgumentException("Time constant must be positive.");
            if (time < 0)
                throw new ArgumentException("Time must be non-negative.");
            return initialVoltage * Math.Exp(-time / timeConstant);
        }

        /// <summary>
        /// Comprehensive capacitor charge and energy analysis
        /// </summary>
        public class CapacitorChargeEnergyResult
        {
            public double Capacitance { get; set; }           // Farads
            public double Voltage { get; set; }               // Volts
            public double Charge { get; set; }                // Coulombs
            public double Energy { get; set; }                // Joules
            public double EnergyWattHours { get; set; }       // Watt-hours
            public double EnergyDensity { get; set; }         // J/cm³ (if physical size provided)
            public double SpecificEnergy { get; set; }        // J/g (if mass provided)
            public double ChargeDensity { get; set; }         // C/cm³ (if physical size provided)
            public double ElectricField { get; set; }         // V/m (if dielectric thickness provided)
            public List<string> SafetyNotes { get; set; } = new List<string>();
        }

        /// <summary>
        /// Enhanced charge and energy calculator with physical properties
        /// </summary>
        public static CapacitorChargeEnergyResult CalculateChargeAndEnergy(double capacitance, double voltage,
            double volumeCm3 = 0, double massGrams = 0, double dielectricThicknessMm = 0)
        {
            var result = new CapacitorChargeEnergyResult
            {
                Capacitance = capacitance,
                Voltage = voltage,
                Charge = capacitance * voltage,
                Energy = 0.5 * capacitance * voltage * voltage
            };

            // Convert to other energy units
            result.EnergyWattHours = result.Energy / 3600.0;

            // Calculate densities if physical properties provided
            if (volumeCm3 > 0)
            {
                result.EnergyDensity = result.Energy / (volumeCm3 / 1000000.0); // J/m³ then convert to J/cm³
                result.ChargeDensity = result.Charge / (volumeCm3 / 1000000.0); // C/m³ then convert to C/cm³
            }

            if (massGrams > 0)
            {
                result.SpecificEnergy = result.Energy / (massGrams / 1000.0); // J/kg then convert
            }

            if (dielectricThicknessMm > 0)
            {
                result.ElectricField = voltage / (dielectricThicknessMm / 1000.0); // V/m
            }

            // Safety warnings
            if (voltage > 50)
            {
                result.SafetyNotes.Add("HIGH VOLTAGE: Take appropriate safety precautions");
            }

            if (result.Energy > 1.0)
            {
                result.SafetyNotes.Add($"HIGH ENERGY: {result.Energy:0.1}J stored - potentially dangerous discharge");
            }

            if (result.ElectricField > 1000000) // 1 MV/m
            {
                result.SafetyNotes.Add("High electric field - verify dielectric breakdown voltage");
            }

            return result;
        }

        /// <summary>
        /// Capacitor bank analysis for series and parallel combinations
        /// </summary>
        public class CapacitorBankResult
        {
            public double TotalCapacitance { get; set; }
            public double TotalVoltageRating { get; set; }
            public double TotalEnergy { get; set; }
            public double TotalCharge { get; set; }
            public string Configuration { get; set; }
            public List<CapacitorInBank> IndividualCapacitors { get; set; } = new List<CapacitorInBank>();
            public double RippleCurrent { get; set; }
            public double ESR { get; set; }
            public List<string> DesignNotes { get; set; } = new List<string>();
        }

        /// <summary>
        /// Individual capacitor in a bank
        /// </summary>
        public class CapacitorInBank
        {
            public double Capacitance { get; set; }
            public double VoltageRating { get; set; }
            public double ActualVoltage { get; set; }
            public double Charge { get; set; }
            public double Energy { get; set; }
            public double ESR { get; set; }
            public string Position { get; set; }
            public double VoltageDeratingPercent { get; set; }
            public bool WithinSafeOperatingArea { get; set; }
        }

        /// <summary>
        /// Analyze series capacitor bank
        /// </summary>
        public static CapacitorBankResult AnalyzeSeriesCapacitorBank(double[] capacitances, 
            double[] voltageRatings, double appliedVoltage, double[] esrValues = null)
        {
            var result = new CapacitorBankResult
            {
                Configuration = "Series",
                TotalCapacitance = SeriesCapacitance(capacitances)
            };

            // In series: voltage divides inversely proportional to capacitance
            double totalCapacitance = result.TotalCapacitance;
            
            for (int i = 0; i < capacitances.Length; i++)
            {
                var cap = new CapacitorInBank
                {
                    Capacitance = capacitances[i],
                    VoltageRating = voltageRatings[i],
                    Position = $"C{i + 1}",
                    ESR = esrValues?[i] ?? 0
                };

                // Voltage across each capacitor (inverse proportion)
                cap.ActualVoltage = appliedVoltage * (totalCapacitance / capacitances[i]);
                cap.Charge = totalCapacitance * appliedVoltage; // Same charge for all in series
                cap.Energy = 0.5 * capacitances[i] * cap.ActualVoltage * cap.ActualVoltage;
                
                cap.VoltageDeratingPercent = (cap.ActualVoltage / cap.VoltageRating) * 100;
                cap.WithinSafeOperatingArea = cap.VoltageDeratingPercent <= 80; // 80% derating

                result.IndividualCapacitors.Add(cap);
            }

            result.TotalVoltageRating = voltageRatings.Sum();
            result.TotalCharge = result.TotalCapacitance * appliedVoltage;
            result.TotalEnergy = result.IndividualCapacitors.Sum(c => c.Energy);
            
            if (esrValues != null)
            {
                result.ESR = esrValues.Sum(); // ESRs add in series
            }

            // Design notes
            if (appliedVoltage > result.TotalVoltageRating * 0.8)
            {
                result.DesignNotes.Add("Applied voltage close to total rating - verify individual capacitor voltages");
            }

            var unbalanced = result.IndividualCapacitors.Where(c => c.VoltageDeratingPercent > 90).ToList();
            if (unbalanced.Any())
            {
                result.DesignNotes.Add($"Voltage imbalance detected on {unbalanced.Count} capacitor(s) - consider balancing resistors");
            }

            return result;
        }

        /// <summary>
        /// Analyze parallel capacitor bank
        /// </summary>
        public static CapacitorBankResult AnalyzeParallelCapacitorBank(double[] capacitances, 
            double[] voltageRatings, double appliedVoltage, double[] esrValues = null)
        {
            var result = new CapacitorBankResult
            {
                Configuration = "Parallel",
                TotalCapacitance = ParallelCapacitance(capacitances)
            };

            for (int i = 0; i < capacitances.Length; i++)
            {
                var cap = new CapacitorInBank
                {
                    Capacitance = capacitances[i],
                    VoltageRating = voltageRatings[i],
                    ActualVoltage = appliedVoltage, // Same voltage for all in parallel
                    Position = $"C{i + 1}",
                    ESR = esrValues?[i] ?? 0
                };

                cap.Charge = capacitances[i] * appliedVoltage;
                cap.Energy = 0.5 * capacitances[i] * appliedVoltage * appliedVoltage;
                
                cap.VoltageDeratingPercent = (cap.ActualVoltage / cap.VoltageRating) * 100;
                cap.WithinSafeOperatingArea = cap.VoltageDeratingPercent <= 80; // 80% derating

                result.IndividualCapacitors.Add(cap);
            }

            result.TotalVoltageRating = voltageRatings.Min(); // Limited by lowest rating
            result.TotalCharge = result.IndividualCapacitors.Sum(c => c.Charge);
            result.TotalEnergy = result.IndividualCapacitors.Sum(c => c.Energy);
            
            if (esrValues != null)
            {
                // ESRs in parallel: 1/ESR_total = 1/ESR1 + 1/ESR2 + ...
                result.ESR = 1.0 / esrValues.Sum(esr => 1.0 / esr);
            }

            // Design notes
            var overstressed = result.IndividualCapacitors.Where(c => !c.WithinSafeOperatingArea).ToList();
            if (overstressed.Any())
            {
                result.DesignNotes.Add($"{overstressed.Count} capacitor(s) operating above 80% voltage rating");
            }

            return result;
        }

        /// <summary>
        /// Calculate capacitor ripple current handling
        /// </summary>
        public class RippleCurrentResult
        {
            public double RMSCurrent { get; set; }
            public double PowerLoss { get; set; }
            public double TemperatureRise { get; set; }
            public double MaxAllowableCurrent { get; set; }
            public double SafetyMargin { get; set; }
            public bool WithinRating { get; set; }
            public List<string> Recommendations { get; set; } = new List<string>();
        }

        /// <summary>
        /// Analyze capacitor ripple current capability
        /// </summary>
        public static RippleCurrentResult AnalyzeRippleCurrent(double capacitance, double esr, 
            double rmsRippleCurrent, double maxRippleCurrentRating, double thermalResistance = 50)
        {
            var result = new RippleCurrentResult
            {
                RMSCurrent = rmsRippleCurrent,
                MaxAllowableCurrent = maxRippleCurrentRating
            };

            // Power loss due to ESR: P = I²R
            result.PowerLoss = rmsRippleCurrent * rmsRippleCurrent * esr;

            // Temperature rise: ΔT = P × Rth
            result.TemperatureRise = result.PowerLoss * thermalResistance;

            // Safety analysis
            result.WithinRating = rmsRippleCurrent <= maxRippleCurrentRating;
            result.SafetyMargin = (maxRippleCurrentRating - rmsRippleCurrent) / maxRippleCurrentRating * 100;

            // Recommendations
            if (!result.WithinRating)
            {
                result.Recommendations.Add("Ripple current exceeds rating - use larger capacitor or parallel combination");
            }

            if (result.SafetyMargin < 20)
            {
                result.Recommendations.Add("Low safety margin - consider derating for reliability");
            }

            if (result.TemperatureRise > 10)
            {
                result.Recommendations.Add($"High temperature rise ({result.TemperatureRise:0.1}°C) - improve thermal management");
            }

            if (result.PowerLoss > 0.1)
            {
                result.Recommendations.Add($"Significant power loss ({result.PowerLoss:0.3}W) - verify thermal design");
            }

            return result;
        }
    }
}
