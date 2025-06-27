using System;

namespace CircuitTool
{
    /// <summary>
    /// Provides calculations for RF (Radio Frequency) and microwave circuit design.
    /// </summary>
    /// <remarks>
    /// <para>Example usage:</para>
    /// <code>
    /// double wavelength = RFCalculator.Wavelength(2.4e9); // 0.125m for 2.4 GHz
    /// double vswr = RFCalculator.VSWR(0.5); // VSWR from reflection coefficient
    /// double rl = RFCalculator.ReturnLoss(2.0); // Return loss from VSWR
    /// double pl = RFCalculator.PathLoss(2.4e9, 100); // Free space path loss
    /// </code>
    /// </remarks>
    public static class RFCalculator
    {
        private const double SpeedOfLight = 299792458.0; // m/s

        /// <summary>
        /// Calculates wavelength from frequency using λ = c/f.
        /// </summary>
        /// <param name="frequency">Frequency in hertz (Hz).</param>
        /// <returns>Wavelength in meters (m).</returns>
        /// <example>
        /// double wavelength = RFCalculator.Wavelength(2.4e9); // 0.125m for 2.4 GHz
        /// </example>
        public static double Wavelength(double frequency)
        {
            if (frequency <= 0)
                throw new ArgumentException("Frequency must be positive.");
            return SpeedOfLight / frequency;
        }

        /// <summary>
        /// Calculates frequency from wavelength using f = c/λ.
        /// </summary>
        /// <param name="wavelength">Wavelength in meters (m).</param>
        /// <returns>Frequency in hertz (Hz).</returns>
        public static double Frequency(double wavelength)
        {
            if (wavelength <= 0)
                throw new ArgumentException("Wavelength must be positive.");
            return SpeedOfLight / wavelength;
        }

        /// <summary>
        /// Calculates VSWR (Voltage Standing Wave Ratio) from reflection coefficient.
        /// </summary>
        /// <param name="reflectionCoefficient">Reflection coefficient magnitude (0-1).</param>
        /// <returns>VSWR value.</returns>
        /// <example>
        /// double vswr = RFCalculator.VSWR(0.5); // VSWR from reflection coefficient
        /// </example>
        public static double VSWR(double reflectionCoefficient)
        {
            if (reflectionCoefficient < 0 || reflectionCoefficient > 1)
                throw new ArgumentException("Reflection coefficient must be between 0 and 1.");
            return (1 + reflectionCoefficient) / (1 - reflectionCoefficient);
        }

        /// <summary>
        /// Calculates reflection coefficient from VSWR.
        /// </summary>
        /// <param name="vswr">VSWR value (≥ 1).</param>
        /// <returns>Reflection coefficient magnitude.</returns>
        public static double ReflectionCoefficient(double vswr)
        {
            if (vswr < 1)
                throw new ArgumentException("VSWR must be 1 or greater.");
            return (vswr - 1) / (vswr + 1);
        }

        /// <summary>
        /// Calculates return loss from VSWR in dB.
        /// </summary>
        /// <param name="vswr">VSWR value (≥ 1).</param>
        /// <returns>Return loss in dB.</returns>
        /// <example>
        /// double rl = RFCalculator.ReturnLoss(2.0); // Return loss from VSWR
        /// </example>
        public static double ReturnLoss(double vswr)
        {
            if (vswr < 1)
                throw new ArgumentException("VSWR must be 1 or greater.");
            double reflectionCoeff = ReflectionCoefficient(vswr);
            return -20 * Math.Log10(reflectionCoeff);
        }

        /// <summary>
        /// Calculates free space path loss using Friis formula.
        /// </summary>
        /// <param name="frequency">Frequency in hertz (Hz).</param>
        /// <param name="distance">Distance in meters (m).</param>
        /// <returns>Path loss in dB.</returns>
        /// <example>
        /// double pl = RFCalculator.PathLoss(2.4e9, 100); // Free space path loss
        /// </example>
        public static double PathLoss(double frequency, double distance)
        {
            if (frequency <= 0 || distance <= 0)
                throw new ArgumentException("Frequency and distance must be positive.");
            return 20 * Math.Log10(4 * Math.PI * distance * frequency / SpeedOfLight);
        }

        /// <summary>
        /// Calculates skin depth for a conductor at given frequency.
        /// </summary>
        /// <param name="frequency">Frequency in hertz (Hz).</param>
        /// <param name="conductivity">Conductivity in S/m (default: copper).</param>
        /// <param name="permeability">Relative permeability (default: 1 for non-magnetic).</param>
        /// <returns>Skin depth in meters (m).</returns>
        public static double SkinDepth(double frequency, double conductivity = 5.96e7, double permeability = 1.0)
        {
            if (frequency <= 0 || conductivity <= 0 || permeability <= 0)
                throw new ArgumentException("All parameters must be positive.");
            const double mu0 = 4 * Math.PI * 1e-7; // Permeability of free space
            return Math.Sqrt(2 / (2 * Math.PI * frequency * mu0 * permeability * conductivity));
        }

        /// <summary>
        /// Calculates characteristic impedance of a transmission line.
        /// </summary>
        /// <param name="inductancePerLength">Inductance per unit length (H/m).</param>
        /// <param name="capacitancePerLength">Capacitance per unit length (F/m).</param>
        /// <returns>Characteristic impedance in ohms (Ω).</returns>
        public static double CharacteristicImpedance(double inductancePerLength, double capacitancePerLength)
        {
            if (inductancePerLength <= 0 || capacitancePerLength <= 0)
                throw new ArgumentException("Inductance and capacitance per length must be positive.");
            return Math.Sqrt(inductancePerLength / capacitancePerLength);
        }
    }
}
