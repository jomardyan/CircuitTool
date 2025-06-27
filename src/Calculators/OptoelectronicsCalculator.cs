using System;

namespace CircuitTool
{
    /// <summary>
    /// Provides calculations for optoelectronic components and fiber optic systems.
    /// </summary>
    /// <remarks>
    /// <para>Example usage:</para>
    /// <code>
    /// double current = OptoelectronicsCalculator.PhotodiodeCurrent(1e-3, 0.8); // Photodiode current
    /// double power = OptoelectronicsCalculator.LEDPower(20e-3, 1.8, 0.15); // LED optical power
    /// double na = OptoelectronicsCalculator.NumericalAperture(1.46, 1.45); // Fiber NA
    /// </code>
    /// </remarks>
    public static class OptoelectronicsCalculator
    {
        private const double PlanckConstant = 6.62607015e-34; // J⋅s
        private const double SpeedOfLight = 299792458.0; // m/s
        private const double ElectronCharge = 1.602176634e-19; // C

        /// <summary>
        /// Calculates photodiode current from optical power.
        /// </summary>
        /// <param name="opticalPower">Incident optical power in watts (W).</param>
        /// <param name="responsivity">Photodiode responsivity in A/W.</param>
        /// <returns>Photodiode current in amperes (A).</returns>
        /// <example>
        /// double current = OptoelectronicsCalculator.PhotodiodeCurrent(1e-3, 0.8); // Photodiode current
        /// </example>
        public static double PhotodiodeCurrent(double opticalPower, double responsivity)
        {
            if (opticalPower < 0 || responsivity < 0)
                throw new ArgumentException("Optical power and responsivity must be non-negative.");
            return opticalPower * responsivity;
        }

        /// <summary>
        /// Calculates responsivity from quantum efficiency and wavelength.
        /// </summary>
        /// <param name="quantumEfficiency">Quantum efficiency (0-1).</param>
        /// <param name="wavelength">Wavelength in meters.</param>
        /// <returns>Responsivity in A/W.</returns>
        public static double Responsivity(double quantumEfficiency, double wavelength)
        {
            if (quantumEfficiency < 0 || quantumEfficiency > 1)
                throw new ArgumentException("Quantum efficiency must be between 0 and 1.");
            if (wavelength <= 0)
                throw new ArgumentException("Wavelength must be positive.");
            
            double photonEnergy = PlanckConstant * SpeedOfLight / wavelength;
            return quantumEfficiency * ElectronCharge / photonEnergy;
        }

        /// <summary>
        /// Calculates LED optical power output.
        /// </summary>
        /// <param name="current">Forward current in amperes (A).</param>
        /// <param name="voltage">Forward voltage in volts (V).</param>
        /// <param name="efficiency">Wall-plug efficiency (0-1).</param>
        /// <returns>Optical power output in watts (W).</returns>
        /// <example>
        /// double power = OptoelectronicsCalculator.LEDPower(20e-3, 1.8, 0.15); // LED optical power
        /// </example>
        public static double LEDPower(double current, double voltage, double efficiency)
        {
            if (current < 0 || voltage < 0)
                throw new ArgumentException("Current and voltage must be non-negative.");
            if (efficiency < 0 || efficiency > 1)
                throw new ArgumentException("Efficiency must be between 0 and 1.");
            
            double electricalPower = current * voltage;
            return electricalPower * efficiency;
        }

        /// <summary>
        /// Calculates numerical aperture of an optical fiber.
        /// </summary>
        /// <param name="coreIndex">Refractive index of the core.</param>
        /// <param name="claddingIndex">Refractive index of the cladding.</param>
        /// <returns>Numerical aperture (dimensionless).</returns>
        /// <example>
        /// double na = OptoelectronicsCalculator.NumericalAperture(1.46, 1.45); // Fiber NA
        /// </example>
        public static double NumericalAperture(double coreIndex, double claddingIndex)
        {
            if (coreIndex <= claddingIndex)
                throw new ArgumentException("Core index must be greater than cladding index.");
            if (coreIndex <= 0 || claddingIndex <= 0)
                throw new ArgumentException("Refractive indices must be positive.");
            
            return Math.Sqrt(coreIndex * coreIndex - claddingIndex * claddingIndex);
        }

        /// <summary>
        /// Calculates acceptance angle of an optical fiber.
        /// </summary>
        /// <param name="numericalAperture">Numerical aperture of the fiber.</param>
        /// <param name="externalMediumIndex">Refractive index of external medium (default: air = 1.0).</param>
        /// <returns>Half-angle of acceptance cone in radians.</returns>
        public static double AcceptanceAngle(double numericalAperture, double externalMediumIndex = 1.0)
        {
            if (numericalAperture < 0 || externalMediumIndex <= 0)
                throw new ArgumentException("Numerical aperture must be non-negative and external medium index must be positive.");
            if (numericalAperture / externalMediumIndex > 1)
                throw new ArgumentException("Numerical aperture cannot exceed external medium index.");
            
            return Math.Asin(numericalAperture / externalMediumIndex);
        }

        /// <summary>
        /// Calculates attenuation in optical fiber.
        /// </summary>
        /// <param name="inputPower">Input optical power in watts (W).</param>
        /// <param name="outputPower">Output optical power in watts (W).</param>
        /// <param name="length">Fiber length in kilometers.</param>
        /// <returns>Attenuation in dB/km.</returns>
        public static double FiberAttenuation(double inputPower, double outputPower, double length)
        {
            if (inputPower <= 0 || outputPower <= 0 || length <= 0)
                throw new ArgumentException("Powers and length must be positive.");
            if (outputPower > inputPower)
                throw new ArgumentException("Output power cannot exceed input power.");
            
            return (10 * Math.Log10(inputPower / outputPower)) / length;
        }

        /// <summary>
        /// Calculates optical power from attenuation.
        /// </summary>
        /// <param name="inputPower">Input optical power in watts (W).</param>
        /// <param name="attenuation">Attenuation in dB/km.</param>
        /// <param name="length">Fiber length in kilometers.</param>
        /// <returns>Output optical power in watts (W).</returns>
        public static double OutputPower(double inputPower, double attenuation, double length)
        {
            if (inputPower <= 0 || attenuation < 0 || length < 0)
                throw new ArgumentException("Input power must be positive, attenuation and length must be non-negative.");
            
            double totalLoss = attenuation * length;
            return inputPower * Math.Pow(10, -totalLoss / 10);
        }

        /// <summary>
        /// Calculates photodetector noise equivalent power (NEP).
        /// </summary>
        /// <param name="noiseCurrentDensity">Noise current density in A/√Hz.</param>
        /// <param name="responsivity">Photodetector responsivity in A/W.</param>
        /// <returns>Noise equivalent power in W/√Hz.</returns>
        public static double NoiseEquivalentPower(double noiseCurrentDensity, double responsivity)
        {
            if (noiseCurrentDensity < 0 || responsivity <= 0)
                throw new ArgumentException("Noise current density must be non-negative and responsivity must be positive.");
            
            return noiseCurrentDensity / responsivity;
        }

        /// <summary>
        /// Calculates detectivity (D*) of a photodetector.
        /// </summary>
        /// <param name="area">Detector area in square meters (m²).</param>
        /// <param name="noiseEquivalentPower">Noise equivalent power in W/√Hz.</param>
        /// <returns>Detectivity in cm⋅Hz^(1/2)/W.</returns>
        public static double Detectivity(double area, double noiseEquivalentPower)
        {
            if (area <= 0 || noiseEquivalentPower <= 0)
                throw new ArgumentException("Area and noise equivalent power must be positive.");
            
            // Convert area from m² to cm²
            double areaCm2 = area * 10000;
            return Math.Sqrt(areaCm2) / noiseEquivalentPower;
        }

        /// <summary>
        /// Calculates modal dispersion in multimode fiber.
        /// </summary>
        /// <param name="coreIndex">Core refractive index.</param>
        /// <param name="claddingIndex">Cladding refractive index.</param>
        /// <param name="length">Fiber length in kilometers.</param>
        /// <returns>Modal dispersion in nanoseconds.</returns>
        public static double ModalDispersion(double coreIndex, double claddingIndex, double length)
        {
            if (coreIndex <= claddingIndex)
                throw new ArgumentException("Core index must be greater than cladding index.");
            if (length < 0)
                throw new ArgumentException("Length must be non-negative.");
            
            double deltaIndex = (coreIndex - claddingIndex) / coreIndex;
            return (length * 1000 * coreIndex * deltaIndex) / (2 * SpeedOfLight) * 1e9; // Convert to ns
        }

        /// <summary>
        /// Calculates chromatic dispersion broadening.
        /// </summary>
        /// <param name="dispersionParameter">Dispersion parameter in ps/(nm⋅km).</param>
        /// <param name="spectralWidth">Source spectral width in nm.</param>
        /// <param name="length">Fiber length in kilometers.</param>
        /// <returns>Pulse broadening in picoseconds.</returns>
        public static double ChromaticDispersion(double dispersionParameter, double spectralWidth, double length)
        {
            if (spectralWidth < 0 || length < 0)
                throw new ArgumentException("Spectral width and length must be non-negative.");
            
            return Math.Abs(dispersionParameter) * spectralWidth * length;
        }

        /// <summary>
        /// Calculates link budget for fiber optic system.
        /// </summary>
        /// <param name="transmitterPower">Transmitter power in dBm.</param>
        /// <param name="receiverSensitivity">Receiver sensitivity in dBm.</param>
        /// <param name="systemMargin">System margin in dB.</param>
        /// <returns>Available loss budget in dB.</returns>
        public static double LinkBudget(double transmitterPower, double receiverSensitivity, double systemMargin)
        {
            if (systemMargin < 0)
                throw new ArgumentException("System margin must be non-negative.");
            
            return transmitterPower - receiverSensitivity - systemMargin;
        }
    }
}
