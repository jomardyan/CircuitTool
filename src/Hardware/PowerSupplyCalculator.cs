#nullable enable
using System;
using System.Collections.Generic;

namespace CircuitTool
{
    /// <summary>
    /// Provides calculations and utilities for power supply design and analysis
    /// </summary>
    public static class PowerSupplyCalculator
    {
        /// <summary>
        /// Power supply topology types
        /// </summary>
        public enum PSUTopology
        {
            Linear,
            Buck,
            Boost,
            BuckBoost,
            Flyback,
            Forward,
            SEPIC
        }

        /// <summary>
        /// Calculates linear regulator heat dissipation and efficiency
        /// </summary>
        /// <param name="inputVoltage">Input voltage</param>
        /// <param name="outputVoltage">Output voltage</param>
        /// <param name="outputCurrent">Output current</param>
        /// <returns>Power dissipation and efficiency</returns>
        public static (double powerDissipation, double efficiency) CalculateLinearRegulator(
            double inputVoltage, double outputVoltage, double outputCurrent)
        {
            double voltageDrop = inputVoltage - outputVoltage;
            double powerDissipation = voltageDrop * outputCurrent;
            double outputPower = outputVoltage * outputCurrent;
            double efficiency = outputPower / (outputPower + powerDissipation);
            
            return (powerDissipation, efficiency);
        }

        /// <summary>
        /// Calculates buck converter inductor value
        /// </summary>
        /// <param name="inputVoltage">Input voltage</param>
        /// <param name="outputVoltage">Output voltage</param>
        /// <param name="outputCurrent">Output current</param>
        /// <param name="switchingFrequency">Switching frequency in Hz</param>
        /// <param name="currentRipple">Acceptable current ripple (0.1 = 10%)</param>
        /// <returns>Required inductor value in henries</returns>
        public static double CalculateBuckInductor(double inputVoltage, double outputVoltage, 
            double outputCurrent, double switchingFrequency, double currentRipple = 0.2)
        {
            double dutyCycle = outputVoltage / inputVoltage;
            double deltaI = outputCurrent * currentRipple;
            
            return (inputVoltage - outputVoltage) * dutyCycle / (deltaI * switchingFrequency);
        }

        /// <summary>
        /// Calculates buck converter output capacitor
        /// </summary>
        /// <param name="outputCurrent">Output current</param>
        /// <param name="switchingFrequency">Switching frequency in Hz</param>
        /// <param name="voltageRipple">Acceptable voltage ripple in volts</param>
        /// <param name="currentRipple">Inductor current ripple (0.1 = 10%)</param>
        /// <returns>Required capacitance in farads</returns>
        public static double CalculateBuckCapacitor(double outputCurrent, double switchingFrequency, 
            double voltageRipple, double currentRipple = 0.2)
        {
            double deltaI = outputCurrent * currentRipple;
            return deltaI / (8 * switchingFrequency * voltageRipple);
        }

        /// <summary>
        /// Calculates boost converter inductor value
        /// </summary>
        /// <param name="inputVoltage">Input voltage</param>
        /// <param name="outputVoltage">Output voltage</param>
        /// <param name="outputCurrent">Output current</param>
        /// <param name="switchingFrequency">Switching frequency in Hz</param>
        /// <param name="currentRipple">Acceptable current ripple (0.1 = 10%)</param>
        /// <returns>Required inductor value in henries</returns>
        public static double CalculateBoostInductor(double inputVoltage, double outputVoltage, 
            double outputCurrent, double switchingFrequency, double currentRipple = 0.2)
        {
            double dutyCycle = 1 - (inputVoltage / outputVoltage);
            double inputCurrent = outputCurrent * outputVoltage / inputVoltage; // Assuming 100% efficiency
            double deltaI = inputCurrent * currentRipple;
            
            return inputVoltage * dutyCycle / (deltaI * switchingFrequency);
        }

        /// <summary>
        /// Calculates flyback transformer turns ratio
        /// </summary>
        /// <param name="inputVoltageMin">Minimum input voltage</param>
        /// <param name="inputVoltageMax">Maximum input voltage</param>
        /// <param name="outputVoltage">Output voltage</param>
        /// <param name="forwardVoltageOutput">Output diode forward voltage</param>
        /// <param name="maxDutyCycle">Maximum duty cycle (typically 0.45)</param>
        /// <returns>Primary to secondary turns ratio</returns>
        public static double CalculateFlybackTurnsRatio(double inputVoltageMin, double inputVoltageMax, 
            double outputVoltage, double forwardVoltageOutput = 0.7, double maxDutyCycle = 0.45)
        {
            double reflectedVoltage = (outputVoltage + forwardVoltageOutput) / (1 - maxDutyCycle);
            return inputVoltageMin / reflectedVoltage;
        }

        /// <summary>
        /// Calculates power supply efficiency at different loads
        /// </summary>
        /// <param name="topology">Power supply topology</param>
        /// <param name="inputVoltage">Input voltage</param>
        /// <param name="outputVoltage">Output voltage</param>
        /// <param name="loadPercentage">Load as percentage of maximum (0-1)</param>
        /// <returns>Estimated efficiency</returns>
        public static double EstimateEfficiency(PSUTopology topology, double inputVoltage, 
            double outputVoltage, double loadPercentage)
        {
            double baseEfficiency = topology switch
            {
                PSUTopology.Linear => outputVoltage / inputVoltage,
                PSUTopology.Buck => 0.90,
                PSUTopology.Boost => 0.88,
                PSUTopology.BuckBoost => 0.85,
                PSUTopology.Flyback => 0.82,
                PSUTopology.Forward => 0.88,
                PSUTopology.SEPIC => 0.85,
                _ => 0.80
            };
            
            // Efficiency typically peaks at 50-70% load for switching supplies
            if (topology != PSUTopology.Linear)
            {
                double loadFactor = loadPercentage switch
                {
                    < 0.1 => 0.7,  // Light load penalty
                    < 0.3 => 0.85,
                    < 0.8 => 1.0,  // Peak efficiency range
                    _ => 0.95      // Heavy load penalty
                };
                
                baseEfficiency *= loadFactor;
            }
            
            return Math.Min(baseEfficiency, 0.98); // Theoretical maximum
        }

        /// <summary>
        /// Calculates current sense resistor value
        /// </summary>
        /// <param name="maxCurrent">Maximum current to sense</param>
        /// <param name="senseVoltage">Desired sense voltage (typically 0.1-0.5V)</param>
        /// <param name="tolerance">Resistor tolerance (0.01 = 1%)</param>
        /// <returns>Sense resistor value and power rating</returns>
        public static (double resistance, double powerRating) CalculateCurrentSenseResistor(
            double maxCurrent, double senseVoltage = 0.1, double tolerance = 0.01)
        {
            double resistance = senseVoltage / maxCurrent;
            double powerDissipation = senseVoltage * maxCurrent;
            
            // Derate power by 50% for reliability
            double powerRating = powerDissipation * 2;
            
            return (resistance, powerRating);
        }

        /// <summary>
        /// Calculates EMI filter components for switching power supply
        /// </summary>
        /// <param name="switchingFrequency">Switching frequency in Hz</param>
        /// <param name="inputCurrent">Input current</param>
        /// <param name="cableLength">Input cable length in meters</param>
        /// <returns>Common mode and differential mode filter values</returns>
        public static (double commonModeInductor, double differentialModeInductor, double commonModeCapacitor, double differentialModeCapacitor) 
            CalculateEMIFilter(double switchingFrequency, double inputCurrent, double cableLength = 1.0)
        {
            // Target filter corner frequency (1/10 of switching frequency)
            double filterFrequency = switchingFrequency / 10;
            
            // Common mode filter
            double commonModeInductor = 1.0 / (2 * Math.PI * filterFrequency * 1e-9); // Assuming 1nF CM cap
            double commonModeCapacitor = 1e-9; // 1nF (safety rated)
            
            // Differential mode filter
            double differentialModeCapacitor = inputCurrent / (2 * Math.PI * filterFrequency * 100); // 100V across cap
            double differentialModeInductor = 1.0 / (Math.Pow(2 * Math.PI * filterFrequency, 2) * differentialModeCapacitor);
            
            // Adjust for cable length (longer cables need more filtering)
            double cableFactor = Math.Max(1.0, cableLength);
            commonModeInductor *= cableFactor;
            differentialModeInductor *= cableFactor;
            
            return (commonModeInductor, differentialModeInductor, commonModeCapacitor, differentialModeCapacitor);
        }

        /// <summary>
        /// Calculates holdup time for power supply
        /// </summary>
        /// <param name="inputCapacitance">Input capacitance in farads</param>
        /// <param name="nominalVoltage">Nominal input voltage</param>
        /// <param name="minimumVoltage">Minimum operating voltage</param>
        /// <param name="outputPower">Output power in watts</param>
        /// <param name="efficiency">Power supply efficiency (0-1)</param>
        /// <returns>Holdup time in seconds</returns>
        public static double CalculateHoldupTime(double inputCapacitance, double nominalVoltage, 
            double minimumVoltage, double outputPower, double efficiency = 0.85)
        {
            double storedEnergy = 0.5 * inputCapacitance * (nominalVoltage * nominalVoltage - minimumVoltage * minimumVoltage);
            double inputPower = outputPower / efficiency;
            
            return storedEnergy / inputPower;
        }

        /// <summary>
        /// Generates power supply design checklist
        /// </summary>
        /// <param name="topology">Power supply topology</param>
        /// <param name="outputPower">Output power in watts</param>
        /// <param name="isIsolated">Whether isolation is required</param>
        /// <returns>Design checklist</returns>
        public static string GenerateDesignChecklist(PSUTopology topology, double outputPower, bool isIsolated = false)
        {
            var checklist = new List<string>
            {
                $"=== {topology} Power Supply Design Checklist ({outputPower}W) ===",
                "",
                "SPECIFICATIONS:",
                "☐ Input voltage range defined",
                "☐ Output voltage tolerance specified",
                "☐ Maximum output current determined",
                "☐ Efficiency requirements established",
                "☐ Operating temperature range defined",
                "",
                "COMPONENT SELECTION:"
            };

            if (topology == PSUTopology.Linear)
            {
                checklist.AddRange(new[]
                {
                    "☐ Linear regulator selected",
                    "☐ Heat sink calculations completed",
                    "☐ Input/output capacitors sized",
                    "☐ Thermal protection considered"
                });
            }
            else
            {
                checklist.AddRange(new[]
                {
                    "☐ Switching controller selected",
                    "☐ Power MOSFETs/diodes chosen",
                    "☐ Inductor value calculated and selected",
                    "☐ Output capacitor sized for ripple",
                    "☐ Compensation network designed",
                    "☐ EMI filter components selected"
                });
            }

            if (isIsolated)
            {
                checklist.AddRange(new[]
                {
                    "",
                    "ISOLATION REQUIREMENTS:",
                    "☐ Isolation voltage rating verified",
                    "☐ Safety certifications considered",
                    "☐ Creepage and clearance distances met",
                    "☐ Isolation feedback designed"
                });
            }

            checklist.AddRange(new[]
            {
                "",
                "PROTECTION FEATURES:",
                "☐ Overcurrent protection implemented",
                "☐ Overvoltage protection added",
                "☐ Thermal protection included",
                "☐ Short circuit protection verified",
                "",
                "TESTING:",
                "☐ Load regulation tested",
                "☐ Line regulation verified",
                "☐ Efficiency measured",
                "☐ EMI compliance checked",
                "☐ Thermal testing completed"
            });

            return string.Join("\n", checklist);
        }
    }
}
