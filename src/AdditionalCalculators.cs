#nullable enable
using System;

namespace CircuitTool
{
    /// <summary>
    /// Provides calculations for antenna design and analysis
    /// </summary>
    public static class AntennaCalculator
    {
        /// <summary>
        /// Calculates the physical length of a quarter-wave antenna
        /// </summary>
        /// <param name="frequency">Frequency in Hz</param>
        /// <param name="velocityFactor">Velocity factor (default 0.95 for typical wire)</param>
        /// <returns>Physical length in meters</returns>
        public static double QuarterWaveLength(double frequency, double velocityFactor = 0.95)
        {
            if (frequency <= 0) throw new ArgumentException("Frequency must be positive", nameof(frequency));
            if (velocityFactor <= 0 || velocityFactor > 1) throw new ArgumentException("Velocity factor must be between 0 and 1", nameof(velocityFactor));
            
            const double speedOfLight = 299792458; // m/s
            double wavelength = speedOfLight / frequency;
            return (wavelength / 4) * velocityFactor;
        }

        /// <summary>
        /// Calculates the physical length of a half-wave antenna
        /// </summary>
        /// <param name="frequency">Frequency in Hz</param>
        /// <param name="velocityFactor">Velocity factor (default 0.95 for typical wire)</param>
        /// <returns>Physical length in meters</returns>
        public static double HalfWaveLength(double frequency, double velocityFactor = 0.95)
        {
            return QuarterWaveLength(frequency, velocityFactor) * 2;
        }

        /// <summary>
        /// Calculates the characteristic impedance of a dipole antenna
        /// </summary>
        /// <param name="wireRadius">Wire radius in meters</param>
        /// <param name="frequency">Frequency in Hz</param>
        /// <returns>Characteristic impedance in ohms</returns>
        public static double DipoleImpedance(double wireRadius, double frequency)
        {
            if (wireRadius <= 0) throw new ArgumentException("Wire radius must be positive", nameof(wireRadius));
            if (frequency <= 0) throw new ArgumentException("Frequency must be positive", nameof(frequency));
            
            const double speedOfLight = 299792458;
            double wavelength = speedOfLight / frequency;
            double lengthToRadiusRatio = (wavelength / 2) / wireRadius;
            
            // Simplified formula for thin wire dipole
            return 73.13 + 42.55 * Math.Log10(lengthToRadiusRatio);
        }

        /// <summary>
        /// Calculates antenna gain in dB for a given directivity and efficiency
        /// </summary>
        /// <param name="directivity">Directivity in linear scale</param>
        /// <param name="efficiency">Efficiency (0-1)</param>
        /// <returns>Gain in dB</returns>
        public static double AntennaGain(double directivity, double efficiency)
        {
            if (directivity <= 0) throw new ArgumentException("Directivity must be positive", nameof(directivity));
            if (efficiency <= 0 || efficiency > 1) throw new ArgumentException("Efficiency must be between 0 and 1", nameof(efficiency));
            
            return 10 * Math.Log10(directivity * efficiency);
        }

        /// <summary>
        /// Calculates the VSWR (Voltage Standing Wave Ratio) from reflection coefficient
        /// </summary>
        /// <param name="reflectionCoefficient">Reflection coefficient magnitude (0-1)</param>
        /// <returns>VSWR</returns>
        public static double CalculateVSWR(double reflectionCoefficient)
        {
            if (reflectionCoefficient < 0 || reflectionCoefficient > 1) 
                throw new ArgumentException("Reflection coefficient must be between 0 and 1", nameof(reflectionCoefficient));
            
            return (1 + reflectionCoefficient) / (1 - reflectionCoefficient);
        }

        /// <summary>
        /// Calculates the effective radiated power (ERP)
        /// </summary>
        /// <param name="transmitterPower">Transmitter power in watts</param>
        /// <param name="antennaGainDb">Antenna gain in dB</param>
        /// <param name="feedlineLossDb">Feedline loss in dB</param>
        /// <returns>ERP in watts</returns>
        public static double EffectiveRadiatedPower(double transmitterPower, double antennaGainDb, double feedlineLossDb)
        {
            if (transmitterPower <= 0) throw new ArgumentException("Transmitter power must be positive", nameof(transmitterPower));
            
            double gainLinear = Math.Pow(10, antennaGainDb / 10);
            double lossLinear = Math.Pow(10, feedlineLossDb / 10);
            
            return transmitterPower * gainLinear / lossLinear;
        }
    }

    /// <summary>
    /// Provides calculations for signal integrity analysis
    /// </summary>
    public static class SignalIntegrityCalculator
    {
        /// <summary>
        /// Calculates the characteristic impedance of a microstrip transmission line
        /// </summary>
        /// <param name="traceWidth">Trace width in meters</param>
        /// <param name="dielectricThickness">Dielectric thickness in meters</param>
        /// <param name="dielectricConstant">Relative dielectric constant</param>
        /// <returns>Characteristic impedance in ohms</returns>
        public static double MicrostripImpedance(double traceWidth, double dielectricThickness, double dielectricConstant)
        {
            if (traceWidth <= 0) throw new ArgumentException("Trace width must be positive", nameof(traceWidth));
            if (dielectricThickness <= 0) throw new ArgumentException("Dielectric thickness must be positive", nameof(dielectricThickness));
            if (dielectricConstant <= 1) throw new ArgumentException("Dielectric constant must be greater than 1", nameof(dielectricConstant));
            
            double widthToHeightRatio = traceWidth / dielectricThickness;
            double effectiveDielectric = (dielectricConstant + 1) / 2 + 
                                       (dielectricConstant - 1) / 2 * Math.Pow(1 + 12 / widthToHeightRatio, -0.5);
            
            double z0;
            if (widthToHeightRatio < 1)
            {
                z0 = 60 / Math.Sqrt(effectiveDielectric) * Math.Log(8 / widthToHeightRatio + widthToHeightRatio / 4);
            }
            else
            {
                z0 = 120 * Math.PI / (Math.Sqrt(effectiveDielectric) * 
                     (widthToHeightRatio + 1.393 + 0.667 * Math.Log(widthToHeightRatio + 1.444)));
            }
            
            return z0;
        }

        /// <summary>
        /// Calculates the propagation delay of a transmission line
        /// </summary>
        /// <param name="length">Line length in meters</param>
        /// <param name="effectiveDielectric">Effective dielectric constant</param>
        /// <returns>Propagation delay in seconds</returns>
        public static double PropagationDelay(double length, double effectiveDielectric)
        {
            if (length <= 0) throw new ArgumentException("Length must be positive", nameof(length));
            if (effectiveDielectric <= 1) throw new ArgumentException("Effective dielectric must be greater than 1", nameof(effectiveDielectric));
            
            const double speedOfLight = 299792458; // m/s
            double velocity = speedOfLight / Math.Sqrt(effectiveDielectric);
            return length / velocity;
        }

        /// <summary>
        /// Calculates crosstalk coupling coefficient between parallel traces
        /// </summary>
        /// <param name="traceSpacing">Spacing between traces in meters</param>
        /// <param name="traceWidth">Trace width in meters</param>
        /// <param name="dielectricHeight">Height above ground plane in meters</param>
        /// <returns>Coupling coefficient (0-1)</returns>
        public static double CrosstalkCoupling(double traceSpacing, double traceWidth, double dielectricHeight)
        {
            if (traceSpacing <= 0) throw new ArgumentException("Trace spacing must be positive", nameof(traceSpacing));
            if (traceWidth <= 0) throw new ArgumentException("Trace width must be positive", nameof(traceWidth));
            if (dielectricHeight <= 0) throw new ArgumentException("Dielectric height must be positive", nameof(dielectricHeight));
            
            // Simplified coupling model
            double spacingRatio = traceSpacing / dielectricHeight;
            double widthRatio = traceWidth / dielectricHeight;
            
            // Empirical formula for edge-coupled microstrip
            return 0.1 * Math.Exp(-1.5 * spacingRatio) * (1 + 0.5 * widthRatio);
        }

        /// <summary>
        /// Calculates the rise time degradation due to transmission line effects
        /// </summary>
        /// <param name="originalRiseTime">Original rise time in seconds</param>
        /// <param name="propagationDelay">Propagation delay in seconds</param>
        /// <returns>Degraded rise time in seconds</returns>
        public static double RiseTimeDegradation(double originalRiseTime, double propagationDelay)
        {
            if (originalRiseTime <= 0) throw new ArgumentException("Rise time must be positive", nameof(originalRiseTime));
            if (propagationDelay <= 0) throw new ArgumentException("Propagation delay must be positive", nameof(propagationDelay));
            
            // Rule of thumb: if propagation delay > rise_time/6, significant degradation occurs
            if (propagationDelay > originalRiseTime / 6)
            {
                return Math.Sqrt(originalRiseTime * originalRiseTime + (2 * propagationDelay) * (2 * propagationDelay));
            }
            return originalRiseTime;
        }
    }

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

    /// <summary>
    /// Provides electromagnetic compatibility (EMC) calculations
    /// </summary>
    public static class EMCCalculator
    {
        /// <summary>
        /// Calculates the electric field strength at a given distance from a radiating source
        /// </summary>
        /// <param name="power">Radiated power in watts</param>
        /// <param name="distance">Distance from source in meters</param>
        /// <param name="antennaGain">Antenna gain in linear scale (default 1 for isotropic)</param>
        /// <returns>Electric field strength in V/m</returns>
        public static double ElectricFieldStrength(double power, double distance, double antennaGain = 1.0)
        {
            if (power <= 0) throw new ArgumentException("Power must be positive", nameof(power));
            if (distance <= 0) throw new ArgumentException("Distance must be positive", nameof(distance));
            if (antennaGain <= 0) throw new ArgumentException("Antenna gain must be positive", nameof(antennaGain));
            
            const double impedanceOfFreeSpace = 376.73; // ohms
            double powerDensity = power * antennaGain / (4 * Math.PI * distance * distance);
            return Math.Sqrt(powerDensity * impedanceOfFreeSpace);
        }

        /// <summary>
        /// Calculates shielding effectiveness of a conductive enclosure
        /// </summary>
        /// <param name="frequency">Frequency in Hz</param>
        /// <param name="thickness">Shield thickness in meters</param>
        /// <param name="conductivity">Material conductivity in S/m</param>
        /// <param name="permeability">Relative permeability (default 1 for non-magnetic materials)</param>
        /// <returns>Shielding effectiveness in dB</returns>
        public static double ShieldingEffectiveness(double frequency, double thickness, double conductivity, double permeability = 1.0)
        {
            if (frequency <= 0) throw new ArgumentException("Frequency must be positive", nameof(frequency));
            if (thickness <= 0) throw new ArgumentException("Thickness must be positive", nameof(thickness));
            if (conductivity <= 0) throw new ArgumentException("Conductivity must be positive", nameof(conductivity));
            if (permeability <= 0) throw new ArgumentException("Permeability must be positive", nameof(permeability));
            
            const double mu0 = 4 * Math.PI * 1e-7; // H/m
            double omega = 2 * Math.PI * frequency;
            double skinDepth = Math.Sqrt(2 / (omega * mu0 * permeability * conductivity));
            
            // Absorption loss (dominant for thick shields)
            double absorptionLoss = 20 * Math.Log10(Math.E) * thickness / skinDepth;
            
            // Reflection loss (for far-field plane wave)
            double reflectionLoss = 20 * Math.Log10(Math.Sqrt(omega * mu0 * permeability / (8 * conductivity)));
            
            return absorptionLoss + reflectionLoss;
        }

        /// <summary>
        /// Calculates the maximum allowed radiated emission for FCC Class B devices
        /// </summary>
        /// <param name="frequency">Frequency in Hz</param>
        /// <param name="measurementDistance">Measurement distance in meters (typically 3m or 10m)</param>
        /// <returns>Maximum allowed field strength in dB(μV/m)</returns>
        public static double FCCClassBLimit(double frequency, double measurementDistance = 3.0)
        {
            if (frequency <= 0) throw new ArgumentException("Frequency must be positive", nameof(frequency));
            if (measurementDistance <= 0) throw new ArgumentException("Measurement distance must be positive", nameof(measurementDistance));
            
            double limit;
            
            if (frequency >= 30e6 && frequency < 88e6)
            {
                // 30-88 MHz
                limit = 40; // dB(μV/m) at 3m
            }
            else if (frequency >= 88e6 && frequency < 216e6)
            {
                // 88-216 MHz  
                limit = 43.5; // dB(μV/m) at 3m
            }
            else if (frequency >= 216e6 && frequency < 960e6)
            {
                // 216-960 MHz
                limit = 46; // dB(μV/m) at 3m
            }
            else if (frequency >= 960e6)
            {
                // Above 960 MHz
                limit = 54; // dB(μV/m) at 3m
            }
            else
            {
                throw new ArgumentException("Frequency must be 30 MHz or higher for FCC Class B limits", nameof(frequency));
            }
            
            // Adjust for measurement distance (inverse distance relationship)
            double distanceCorrection = 20 * Math.Log10(3.0 / measurementDistance);
            return limit + distanceCorrection;
        }

        /// <summary>
        /// Calculates the loop inductance for EMI analysis
        /// </summary>
        /// <param name="loopArea">Loop area in m²</param>
        /// <param name="wireRadius">Wire radius in meters</param>
        /// <returns>Loop inductance in henries</returns>
        public static double LoopInductance(double loopArea, double wireRadius)
        {
            if (loopArea <= 0) throw new ArgumentException("Loop area must be positive", nameof(loopArea));
            if (wireRadius <= 0) throw new ArgumentException("Wire radius must be positive", nameof(wireRadius));
            
            const double mu0 = 4 * Math.PI * 1e-7; // H/m
            double radius = Math.Sqrt(loopArea / Math.PI);
            
            // Neumann's formula for circular loop
            return mu0 * radius * (Math.Log(8 * radius / wireRadius) - 2);
        }

        /// <summary>
        /// Calculates common-mode choke impedance
        /// </summary>
        /// <param name="inductance">Choke inductance in henries</param>
        /// <param name="frequency">Frequency in Hz</param>
        /// <param name="resistanceDC">DC resistance in ohms</param>
        /// <returns>Complex impedance magnitude in ohms</returns>
        public static double CommonModeChokeImpedance(double inductance, double frequency, double resistanceDC = 0)
        {
            if (inductance <= 0) throw new ArgumentException("Inductance must be positive", nameof(inductance));
            if (frequency <= 0) throw new ArgumentException("Frequency must be positive", nameof(frequency));
            if (resistanceDC < 0) throw new ArgumentException("Resistance cannot be negative", nameof(resistanceDC));
            
            double reactance = 2 * Math.PI * frequency * inductance;
            return Math.Sqrt(resistanceDC * resistanceDC + reactance * reactance);
        }
    }
}
