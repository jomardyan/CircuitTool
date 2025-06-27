using System;

namespace CircuitTool
{
    /// <summary>
    /// Provides calculations for transmission line analysis and design.
    /// </summary>
    /// <remarks>
    /// <para>Example usage:</para>
    /// <code>
    /// double z0 = TransmissionLineCalculator.CharacteristicImpedance(100e-9, 100e-12); // 31.6 ohms
    /// double vel = TransmissionLineCalculator.PropagationVelocity(2.2); // Velocity factor
    /// double delay = TransmissionLineCalculator.PropagationDelay(0.1, 2.2); // Time delay
    /// </code>
    /// </remarks>
    public static class TransmissionLineCalculator
    {
        private const double SpeedOfLight = 299792458.0; // m/s

        /// <summary>
        /// Calculates characteristic impedance of a transmission line.
        /// </summary>
        /// <param name="inductancePerLength">Inductance per unit length (H/m).</param>
        /// <param name="capacitancePerLength">Capacitance per unit length (F/m).</param>
        /// <returns>Characteristic impedance in ohms (Ω).</returns>
        /// <example>
        /// double z0 = TransmissionLineCalculator.CharacteristicImpedance(100e-9, 100e-12); // 31.6 ohms
        /// </example>
        public static double CharacteristicImpedance(double inductancePerLength, double capacitancePerLength)
        {
            if (inductancePerLength <= 0 || capacitancePerLength <= 0)
                throw new ArgumentException("Inductance and capacitance per length must be positive.");
            return Math.Sqrt(inductancePerLength / capacitancePerLength);
        }

        /// <summary>
        /// Calculates propagation velocity in a transmission line.
        /// </summary>
        /// <param name="relativePermittivity">Relative permittivity (εr) of the dielectric.</param>
        /// <param name="relativePermeability">Relative permeability (μr) of the material (default: 1).</param>
        /// <returns>Propagation velocity in m/s.</returns>
        /// <example>
        /// double vel = TransmissionLineCalculator.PropagationVelocity(2.2); // Velocity factor
        /// </example>
        public static double PropagationVelocity(double relativePermittivity, double relativePermeability = 1.0)
        {
            if (relativePermittivity <= 0 || relativePermeability <= 0)
                throw new ArgumentException("Relative permittivity and permeability must be positive.");
            return SpeedOfLight / Math.Sqrt(relativePermittivity * relativePermeability);
        }

        /// <summary>
        /// Calculates propagation delay for a transmission line.
        /// </summary>
        /// <param name="length">Length of the transmission line in meters.</param>
        /// <param name="relativePermittivity">Relative permittivity (εr) of the dielectric.</param>
        /// <param name="relativePermeability">Relative permeability (μr) of the material (default: 1).</param>
        /// <returns>Propagation delay in seconds.</returns>
        /// <example>
        /// double delay = TransmissionLineCalculator.PropagationDelay(0.1, 2.2); // Time delay
        /// </example>
        public static double PropagationDelay(double length, double relativePermittivity, double relativePermeability = 1.0)
        {
            if (length < 0)
                throw new ArgumentException("Length must be non-negative.");
            double velocity = PropagationVelocity(relativePermittivity, relativePermeability);
            return length / velocity;
        }

        /// <summary>
        /// Calculates coaxial cable characteristic impedance.
        /// </summary>
        /// <param name="outerDiameter">Outer conductor inner diameter in meters.</param>
        /// <param name="innerDiameter">Inner conductor diameter in meters.</param>
        /// <param name="relativePermittivity">Relative permittivity of dielectric (default: 1).</param>
        /// <returns>Characteristic impedance in ohms (Ω).</returns>
        public static double CoaxialImpedance(double outerDiameter, double innerDiameter, double relativePermittivity = 1.0)
        {
            if (outerDiameter <= innerDiameter || innerDiameter <= 0)
                throw new ArgumentException("Outer diameter must be greater than inner diameter, and inner diameter must be positive.");
            if (relativePermittivity <= 0)
                throw new ArgumentException("Relative permittivity must be positive.");
            
            return (377.0 / Math.Sqrt(relativePermittivity)) * Math.Log(outerDiameter / innerDiameter) / (2 * Math.PI);
        }

        /// <summary>
        /// Calculates microstrip characteristic impedance (simple formula).
        /// </summary>
        /// <param name="width">Trace width in meters.</param>
        /// <param name="height">Dielectric height in meters.</param>
        /// <param name="relativePermittivity">Relative permittivity of substrate.</param>
        /// <returns>Characteristic impedance in ohms (Ω).</returns>
        public static double MicrostripImpedance(double width, double height, double relativePermittivity)
        {
            if (width <= 0 || height <= 0 || relativePermittivity <= 0)
                throw new ArgumentException("Width, height, and relative permittivity must be positive.");
            
            double ratio = width / height;
            double effectivePermittivity = (relativePermittivity + 1) / 2 + 
                                         ((relativePermittivity - 1) / 2) * Math.Pow(1 + 12 / ratio, -0.5);
            
            double z0;
            if (ratio >= 1)
            {
                z0 = (377 / (2 * Math.PI * Math.Sqrt(effectivePermittivity))) * 
                     Math.Log(8 / ratio + ratio / 4);
            }
            else
            {
                z0 = (377 / Math.Sqrt(effectivePermittivity)) * 
                     (1.0 / (ratio + 1.393 + 0.667 * Math.Log(ratio + 1.444)));
            }
            
            return z0;
        }

        /// <summary>
        /// Calculates stripline characteristic impedance.
        /// </summary>
        /// <param name="width">Trace width in meters.</param>
        /// <param name="height">Dielectric height in meters.</param>
        /// <param name="relativePermittivity">Relative permittivity of substrate.</param>
        /// <returns>Characteristic impedance in ohms (Ω).</returns>
        public static double StriplineImpedance(double width, double height, double relativePermittivity)
        {
            if (width <= 0 || height <= 0 || relativePermittivity <= 0)
                throw new ArgumentException("Width, height, and relative permittivity must be positive.");
            
            double ratio = width / height;
            double cf;
            
            if (ratio >= 0.35)
            {
                cf = ratio - 0.35;
            }
            else
            {
                cf = 0.35 - ratio;
            }
            
            return (377 / (2 * Math.Sqrt(relativePermittivity))) * 
                   Math.Log(4 * height / width) / Math.PI;
        }

        /// <summary>
        /// Calculates reflection coefficient at a load.
        /// </summary>
        /// <param name="loadImpedance">Load impedance in ohms (Ω).</param>
        /// <param name="characteristicImpedance">Characteristic impedance of line in ohms (Ω).</param>
        /// <returns>Complex reflection coefficient magnitude.</returns>
        public static double ReflectionCoefficient(double loadImpedance, double characteristicImpedance)
        {
            if (characteristicImpedance <= 0)
                throw new ArgumentException("Characteristic impedance must be positive.");
            
            return Math.Abs((loadImpedance - characteristicImpedance) / (loadImpedance + characteristicImpedance));
        }

        /// <summary>
        /// Calculates input impedance of a transmission line.
        /// </summary>
        /// <param name="loadImpedance">Load impedance in ohms (Ω).</param>
        /// <param name="characteristicImpedance">Characteristic impedance in ohms (Ω).</param>
        /// <param name="electricalLength">Electrical length in radians.</param>
        /// <returns>Input impedance in ohms (Ω).</returns>
        public static double InputImpedance(double loadImpedance, double characteristicImpedance, double electricalLength)
        {
            if (characteristicImpedance <= 0)
                throw new ArgumentException("Characteristic impedance must be positive.");
            
            double tanBetaL = Math.Tan(electricalLength);
            double numerator = loadImpedance + characteristicImpedance * tanBetaL;
            double denominator = characteristicImpedance + loadImpedance * tanBetaL;
            
            return characteristicImpedance * Math.Abs(numerator / denominator);
        }

        /// <summary>
        /// Calculates electrical length from physical length.
        /// </summary>
        /// <param name="physicalLength">Physical length in meters.</param>
        /// <param name="frequency">Frequency in Hz.</param>
        /// <param name="relativePermittivity">Relative permittivity of dielectric.</param>
        /// <returns>Electrical length in radians.</returns>
        public static double ElectricalLength(double physicalLength, double frequency, double relativePermittivity)
        {
            if (physicalLength < 0 || frequency <= 0 || relativePermittivity <= 0)
                throw new ArgumentException("Physical length must be non-negative, frequency and permittivity must be positive.");
            
            double wavelength = SpeedOfLight / (frequency * Math.Sqrt(relativePermittivity));
            return 2 * Math.PI * physicalLength / wavelength;
        }

        /// <summary>
        /// Calculates losses in a transmission line.
        /// </summary>
        /// <param name="length">Length of transmission line in meters.</param>
        /// <param name="attenuationConstant">Attenuation constant in Np/m or dB/m.</param>
        /// <param name="inDecibels">True if attenuation constant is in dB/m, false if in Np/m.</param>
        /// <returns>Total loss in dB.</returns>
        public static double TransmissionLoss(double length, double attenuationConstant, bool inDecibels = true)
        {
            if (length < 0 || attenuationConstant < 0)
                throw new ArgumentException("Length and attenuation constant must be non-negative.");
            
            if (inDecibels)
            {
                return length * attenuationConstant;
            }
            else
            {
                return length * attenuationConstant * 8.686; // Convert from Np to dB
            }
        }
    }
}
