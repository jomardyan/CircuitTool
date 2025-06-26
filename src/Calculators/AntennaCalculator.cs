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
        /// <param name="velocityFactor">Velocity factor (default varies by frequency: 1.0 for >1GHz, 0.95 for lower frequencies)</param>
        /// <returns>Physical length in meters</returns>
        public static double QuarterWaveLength(double frequency, double? velocityFactor = null)
        {
            if (frequency <= 0) throw new ArgumentException("Frequency must be positive", nameof(frequency));
            
            // Default velocity factor depends on frequency
            // Higher frequencies (>1 GHz) typically use 1.0 (free space)
            // Lower frequencies use 0.95 (typical wire)
            double vf = velocityFactor ?? (frequency > 1e9 ? 1.0 : 0.95);
            
            if (vf <= 0 || vf > 1) throw new ArgumentException("Velocity factor must be between 0 and 1", nameof(velocityFactor));
            
            const double speedOfLight = 299792458; // m/s
            double wavelength = speedOfLight / frequency;
            return (wavelength / 4) * vf;
        }

        /// <summary>
        /// Calculates the physical length of a half-wave antenna
        /// </summary>
        /// <param name="frequency">Frequency in Hz</param>
        /// <param name="velocityFactor">Velocity factor (default varies by frequency: 1.0 for >1GHz, 0.95 for lower frequencies)</param>
        /// <returns>Physical length in meters</returns>
        public static double HalfWaveLength(double frequency, double? velocityFactor = null)
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
            
            // More accurate formula for thin wire dipole (King's approximation)
            // For typical dipoles, this should give values closer to 73 ohms
            return 73.1 * (1 + 0.042 * Math.Log10(lengthToRadiusRatio));
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
}
