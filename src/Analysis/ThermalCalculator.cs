#nullable enable
using System;

namespace CircuitTool
{
    /// <summary>
    /// Provides thermal analysis calculations for electronic components
    /// </summary>
    public static class ThermalCalculator
    {
        /// <summary>
        /// Calculates junction temperature of a component
        /// </summary>
        /// <param name="powerDissipation">Power dissipation in watts</param>
        /// <param name="thermalResistanceJunctionToAmbient">Thermal resistance from junction to ambient in °C/W</param>
        /// <param name="ambientTemperature">Ambient temperature in °C</param>
        /// <returns>Junction temperature in °C</returns>
        public static double JunctionTemperature(double powerDissipation, double thermalResistanceJunctionToAmbient, double ambientTemperature)
        {
            if (powerDissipation < 0) throw new ArgumentException("Power dissipation cannot be negative", nameof(powerDissipation));
            if (thermalResistanceJunctionToAmbient <= 0) throw new ArgumentException("Thermal resistance must be positive", nameof(thermalResistanceJunctionToAmbient));
            
            return ambientTemperature + powerDissipation * thermalResistanceJunctionToAmbient;
        }

        /// <summary>
        /// Calculates the required heat sink thermal resistance
        /// </summary>
        /// <param name="maxJunctionTemp">Maximum allowable junction temperature in °C</param>
        /// <param name="ambientTemp">Ambient temperature in °C</param>
        /// <param name="powerDissipation">Power dissipation in watts</param>
        /// <param name="thermalResistanceJunctionToCase">Junction-to-case thermal resistance in °C/W</param>
        /// <returns>Required heat sink thermal resistance in °C/W</returns>
        public static double RequiredHeatSinkThermalResistance(double maxJunctionTemp, double ambientTemp, 
                                                              double powerDissipation, double thermalResistanceJunctionToCase)
        {
            if (powerDissipation <= 0) throw new ArgumentException("Power dissipation must be positive", nameof(powerDissipation));
            if (maxJunctionTemp <= ambientTemp) throw new ArgumentException("Max junction temp must be greater than ambient", nameof(maxJunctionTemp));
            
            double allowableTempRise = maxJunctionTemp - ambientTemp;
            double totalAllowableThermalResistance = allowableTempRise / powerDissipation;
            
            return totalAllowableThermalResistance - thermalResistanceJunctionToCase;
        }

        /// <summary>
        /// Calculates thermal time constant for transient analysis
        /// </summary>
        /// <param name="thermalCapacitance">Thermal capacitance in J/°C</param>
        /// <param name="thermalResistance">Thermal resistance in °C/W</param>
        /// <returns>Thermal time constant in seconds</returns>
        public static double ThermalTimeConstant(double thermalCapacitance, double thermalResistance)
        {
            if (thermalCapacitance <= 0) throw new ArgumentException("Thermal capacitance must be positive", nameof(thermalCapacitance));
            if (thermalResistance <= 0) throw new ArgumentException("Thermal resistance must be positive", nameof(thermalResistance));
            
            return thermalCapacitance * thermalResistance;
        }

        /// <summary>
        /// Calculates convective heat transfer coefficient
        /// </summary>
        /// <param name="airVelocity">Air velocity in m/s</param>
        /// <param name="characteristicLength">Characteristic length in meters</param>
        /// <param name="temperatureDifference">Temperature difference in °C</param>
        /// <returns>Heat transfer coefficient in W/(m²·°C)</returns>
        public static double ConvectiveHeatTransfer(double airVelocity, double characteristicLength, double temperatureDifference)
        {
            if (airVelocity < 0) throw new ArgumentException("Air velocity cannot be negative", nameof(airVelocity));
            if (characteristicLength <= 0) throw new ArgumentException("Characteristic length must be positive", nameof(characteristicLength));
            
            // Simplified empirical correlation for forced convection over flat plate
            if (airVelocity == 0)
            {
                // Natural convection
                return 2.5 + 4 * Math.Pow(Math.Abs(temperatureDifference), 0.25);
            }
            else
            {
                // Forced convection
                double reynolds = airVelocity * characteristicLength / 15e-6; // Assuming air at 20°C
                double nusselt = 0.664 * Math.Pow(reynolds, 0.5) * Math.Pow(0.7, 1.0/3.0); // Pr for air ≈ 0.7
                return nusselt * 0.026 / characteristicLength; // k for air ≈ 0.026 W/(m·K)
            }
        }
    }
}
