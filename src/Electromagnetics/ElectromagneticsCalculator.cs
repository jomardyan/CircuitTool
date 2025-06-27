using System;

namespace CircuitTool.Electromagnetics
{
    /// <summary>
    /// Provides calculations for electromagnetic field analysis and antenna design.
    /// </summary>
    /// <remarks>
    /// <para>Example usage:</para>
    /// <code>
    /// double efield = ElectromagneticsCalculator.ElectricFieldStrength(100, 1000); // Electric field
    /// double power = ElectromagneticsCalculator.PowerDensity(377, 1.0); // Power density
    /// double gain = ElectromagneticsCalculator.AntennaGain(10, 0.8); // Antenna gain
    /// </code>
    /// </remarks>
    public static class ElectromagneticsCalculator
    {
        private const double FreeSpaceImpedance = 376.730313668; // Ω
        private const double SpeedOfLight = 299792458.0; // m/s
        private const double VacuumPermeability = 4 * Math.PI * 1e-7; // H/m
        private const double VacuumPermittivity = 8.8541878128e-12; // F/m

        /// <summary>
        /// Calculates electric field strength from power and distance.
        /// </summary>
        /// <param name="power">Transmitted power in watts (W).</param>
        /// <param name="distance">Distance from source in meters (m).</param>
        /// <param name="gain">Antenna gain (linear, not dB). Default is 1 (isotropic).</param>
        /// <returns>Electric field strength in V/m.</returns>
        /// <example>
        /// double efield = ElectromagneticsCalculator.ElectricFieldStrength(100, 1000); // Electric field
        /// </example>
        public static double ElectricFieldStrength(double power, double distance, double gain = 1.0)
        {
            if (power < 0 || distance <= 0 || gain < 0)
                throw new ArgumentException("Power and gain must be non-negative, distance must be positive.");
            
            return Math.Sqrt((30 * gain * power)) / distance;
        }

        /// <summary>
        /// Calculates magnetic field strength from electric field.
        /// </summary>
        /// <param name="electricField">Electric field strength in V/m.</param>
        /// <param name="impedance">Wave impedance in ohms (default: free space).</param>
        /// <returns>Magnetic field strength in A/m.</returns>
        public static double MagneticFieldStrength(double electricField, double impedance = FreeSpaceImpedance)
        {
            if (electricField < 0 || impedance <= 0)
                throw new ArgumentException("Electric field must be non-negative and impedance must be positive.");
            
            return electricField / impedance;
        }

        /// <summary>
        /// Calculates power density from electric field.
        /// </summary>
        /// <param name="impedance">Wave impedance in ohms.</param>
        /// <param name="electricField">Electric field strength in V/m.</param>
        /// <returns>Power density in W/m².</returns>
        /// <example>
        /// double power = ElectromagneticsCalculator.PowerDensity(377, 1.0); // Power density
        /// </example>
        public static double PowerDensity(double impedance, double electricField)
        {
            if (impedance <= 0 || electricField < 0)
                throw new ArgumentException("Impedance must be positive and electric field must be non-negative.");
            
            return (electricField * electricField) / impedance;
        }

        /// <summary>
        /// Calculates far-field distance for an antenna.
        /// </summary>
        /// <param name="antennaSize">Largest dimension of antenna in meters.</param>
        /// <param name="frequency">Frequency in Hz.</param>
        /// <returns>Far-field distance in meters.</returns>
        public static double FarFieldDistance(double antennaSize, double frequency)
        {
            if (antennaSize <= 0 || frequency <= 0)
                throw new ArgumentException("Antenna size and frequency must be positive.");
            
            double wavelength = SpeedOfLight / frequency;
            return (2 * antennaSize * antennaSize) / wavelength;
        }

        /// <summary>
        /// Calculates antenna gain from directivity and efficiency.
        /// </summary>
        /// <param name="directivity">Directivity (linear, not dB).</param>
        /// <param name="efficiency">Antenna efficiency (0-1).</param>
        /// <returns>Antenna gain (linear).</returns>
        /// <example>
        /// double gain = ElectromagneticsCalculator.AntennaGain(10, 0.8); // Antenna gain
        /// </example>
        public static double AntennaGain(double directivity, double efficiency)
        {
            if (directivity < 0)
                throw new ArgumentException("Directivity must be non-negative.");
            if (efficiency < 0 || efficiency > 1)
                throw new ArgumentException("Efficiency must be between 0 and 1.");
            
            return directivity * efficiency;
        }

        /// <summary>
        /// Calculates effective aperture of an antenna.
        /// </summary>
        /// <param name="gain">Antenna gain (linear, not dB).</param>
        /// <param name="frequency">Frequency in Hz.</param>
        /// <returns>Effective aperture in square meters (m²).</returns>
        public static double EffectiveAperture(double gain, double frequency)
        {
            if (gain < 0 || frequency <= 0)
                throw new ArgumentException("Gain must be non-negative and frequency must be positive.");
            
            double wavelength = SpeedOfLight / frequency;
            return (gain * wavelength * wavelength) / (4 * Math.PI);
        }

        /// <summary>
        /// Calculates resonant length of a dipole antenna.
        /// </summary>
        /// <param name="frequency">Frequency in Hz.</param>
        /// <param name="velocityFactor">Velocity factor (default: 0.95 for wire in air).</param>
        /// <returns>Resonant length in meters.</returns>
        public static double DipoleLength(double frequency, double velocityFactor = 0.95)
        {
            if (frequency <= 0)
                throw new ArgumentException("Frequency must be positive.");
            if (velocityFactor <= 0 || velocityFactor > 1)
                throw new ArgumentException("Velocity factor must be between 0 and 1.");
            
            double wavelength = (SpeedOfLight * velocityFactor) / frequency;
            return wavelength / 2;
        }

        /// <summary>
        /// Calculates radiation resistance of a short dipole.
        /// </summary>
        /// <param name="length">Antenna length in meters.</param>
        /// <param name="frequency">Frequency in Hz.</param>
        /// <returns>Radiation resistance in ohms (Ω).</returns>
        public static double ShortDipoleRadiationResistance(double length, double frequency)
        {
            if (length <= 0 || frequency <= 0)
                throw new ArgumentException("Length and frequency must be positive.");
            
            double wavelength = SpeedOfLight / frequency;
            double ratio = length / wavelength;
            
            if (ratio > 0.1)
                throw new ArgumentException("This formula is only valid for short dipoles (length < λ/10).");
            
            return 20 * Math.PI * Math.PI * ratio * ratio;
        }

        /// <summary>
        /// Calculates skin depth in a conductor.
        /// </summary>
        /// <param name="frequency">Frequency in Hz.</param>
        /// <param name="conductivity">Conductivity in S/m (default: copper).</param>
        /// <param name="relativePermeability">Relative permeability (default: 1).</param>
        /// <returns>Skin depth in meters.</returns>
        public static double SkinDepth(double frequency, double conductivity = 5.96e7, double relativePermeability = 1.0)
        {
            if (frequency <= 0 || conductivity <= 0 || relativePermeability <= 0)
                throw new ArgumentException("All parameters must be positive.");
            
            return Math.Sqrt(2 / (2 * Math.PI * frequency * VacuumPermeability * relativePermeability * conductivity));
        }

        /// <summary>
        /// Calculates wave impedance in a medium.
        /// </summary>
        /// <param name="relativePermittivity">Relative permittivity.</param>
        /// <param name="relativePermeability">Relative permeability (default: 1).</param>
        /// <returns>Wave impedance in ohms (Ω).</returns>
        public static double WaveImpedance(double relativePermittivity, double relativePermeability = 1.0)
        {
            if (relativePermittivity <= 0 || relativePermeability <= 0)
                throw new ArgumentException("Relative permittivity and permeability must be positive.");
            
            return FreeSpaceImpedance * Math.Sqrt(relativePermeability / relativePermittivity);
        }

        /// <summary>
        /// Calculates Friis transmission formula for power received.
        /// </summary>
        /// <param name="transmittedPower">Transmitted power in watts (W).</param>
        /// <param name="transmitGain">Transmit antenna gain (linear).</param>
        /// <param name="receiveGain">Receive antenna gain (linear).</param>
        /// <param name="frequency">Frequency in Hz.</param>
        /// <param name="distance">Distance between antennas in meters.</param>
        /// <returns>Received power in watts (W).</returns>
        public static double FriisTransmission(double transmittedPower, double transmitGain, 
                                             double receiveGain, double frequency, double distance)
        {
            if (transmittedPower < 0 || transmitGain < 0 || receiveGain < 0 || frequency <= 0 || distance <= 0)
                throw new ArgumentException("Power and gains must be non-negative, frequency and distance must be positive.");
            
            double wavelength = SpeedOfLight / frequency;
            double pathLossFactor = Math.Pow(wavelength / (4 * Math.PI * distance), 2);
            
            return transmittedPower * transmitGain * receiveGain * pathLossFactor;
        }

        /// <summary>
        /// Calculates loop antenna inductance.
        /// </summary>
        /// <param name="radius">Loop radius in meters.</param>
        /// <param name="wireRadius">Wire radius in meters.</param>
        /// <returns>Inductance in henries (H).</returns>
        public static double LoopInductance(double radius, double wireRadius)
        {
            if (radius <= 0 || wireRadius <= 0 || wireRadius >= radius)
                throw new ArgumentException("Radii must be positive and wire radius must be less than loop radius.");
            
            return VacuumPermeability * radius * (Math.Log(8 * radius / wireRadius) - 2);
        }

        /// <summary>
        /// Calculates monopole antenna impedance.
        /// </summary>
        /// <param name="height">Monopole height in meters.</param>
        /// <param name="frequency">Frequency in Hz.</param>
        /// <param name="groundRadius">Ground plane radius in meters (if applicable).</param>
        /// <returns>Input impedance in ohms (Ω).</returns>
        public static double MonopoleImpedance(double height, double frequency, double groundRadius = double.PositiveInfinity)
        {
            if (height <= 0 || frequency <= 0)
                throw new ArgumentException("Height and frequency must be positive.");
            
            double wavelength = SpeedOfLight / frequency;
            double electricalHeight = 2 * Math.PI * height / wavelength;
            
            // Simplified formula for quarter-wave monopole
            if (Math.Abs(electricalHeight - Math.PI / 2) < 0.1)
            {
                return 36.6; // Approximate radiation resistance for quarter-wave monopole
            }
            
            // General case (simplified)
            return 36.6 * Math.Sin(electricalHeight / 2) * Math.Sin(electricalHeight / 2);
        }
    }
}
