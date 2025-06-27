using System;

namespace CircuitTool
{
    /// <summary>
    /// Provides calculations for Electromagnetic Interference (EMI) and Electromagnetic Compatibility (EMC) analysis.
    /// </summary>
    /// <remarks>
    /// <para>Example usage:</para>
    /// <code>
    /// double field = EMICalculator.NearFieldStrength(1.0, 0.1); // Near field strength
    /// double shielding = EMICalculator.ShieldingEffectiveness(0.001, 1e6, 100); // SE calculation
    /// double cutoff = EMICalculator.WaveguideCutoff(0.02, 0.01); // Waveguide cutoff frequency
    /// </code>
    /// </remarks>
    public static class EMICalculator
    {
        private const double FreeSpaceImpedance = 376.730313668; // Ω
        private const double SpeedOfLight = 299792458.0; // m/s
        private const double VacuumPermeability = 4 * Math.PI * 1e-7; // H/m

        /// <summary>
        /// Calculates near-field electric field strength from current.
        /// </summary>
        /// <param name="current">Current in amperes (A).</param>
        /// <param name="distance">Distance from source in meters (m).</param>
        /// <returns>Electric field strength in V/m.</returns>
        /// <example>
        /// double field = EMICalculator.NearFieldStrength(1.0, 0.1); // Near field strength
        /// </example>
        public static double NearFieldStrength(double current, double distance)
        {
            if (current < 0 || distance <= 0)
                throw new ArgumentException("Current must be non-negative and distance must be positive.");
            
            return (VacuumPermeability * current) / (2 * Math.PI * distance);
        }

        /// <summary>
        /// Calculates shielding effectiveness of a metallic enclosure.
        /// </summary>
        /// <param name="thickness">Shield thickness in meters.</param>
        /// <param name="frequency">Frequency in Hz.</param>
        /// <param name="conductivity">Conductivity in S/m (default: copper).</param>
        /// <param name="relativePermeability">Relative permeability (default: 1).</param>
        /// <returns>Shielding effectiveness in dB.</returns>
        /// <example>
        /// double shielding = EMICalculator.ShieldingEffectiveness(0.001, 1e6, 100); // SE calculation
        /// </example>
        public static double ShieldingEffectiveness(double thickness, double frequency, 
                                                   double conductivity = 5.96e7, double relativePermeability = 1.0)
        {
            if (thickness <= 0 || frequency <= 0 || conductivity <= 0 || relativePermeability <= 0)
                throw new ArgumentException("All parameters must be positive.");
            
            // Skin depth
            double skinDepth = Math.Sqrt(2 / (2 * Math.PI * frequency * VacuumPermeability * relativePermeability * conductivity));
            
            // Absorption loss (dominant for good conductors)
            double absorptionLoss = 8.686 * thickness / skinDepth; // dB
            
            // Reflection loss (near field, magnetic source)
            double reflectionLoss = 20 * Math.Log10(skinDepth * Math.Sqrt(frequency * VacuumPermeability * relativePermeability / conductivity) / (2 * 3.336e-3));
            
            return absorptionLoss + reflectionLoss;
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
        /// Calculates crosstalk between parallel traces.
        /// </summary>
        /// <param name="length">Trace length in meters.</param>
        /// <param name="spacing">Trace spacing in meters.</param>
        /// <param name="height">Height above ground plane in meters.</param>
        /// <param name="frequency">Frequency in Hz.</param>
        /// <returns>Crosstalk in dB.</returns>
        public static double Crosstalk(double length, double spacing, double height, double frequency)
        {
            if (length <= 0 || spacing <= 0 || height <= 0 || frequency <= 0)
                throw new ArgumentException("All parameters must be positive.");
            
            // Simplified crosstalk calculation for microstrip lines
            double capacitiveCoupling = 20 * Math.Log10(height / spacing);
            double inductiveCoupling = 20 * Math.Log10(2 * height / spacing);
            
            // Use worst case (minimum of the two)
            double coupling = Math.Min(capacitiveCoupling, inductiveCoupling);
            
            // Frequency and length dependence
            double lengthFactor = 20 * Math.Log10(2 * Math.PI * frequency * length / SpeedOfLight);
            
            return -(coupling + lengthFactor); // Negative because it's attenuation
        }

        /// <summary>
        /// Calculates waveguide cutoff frequency for rectangular apertures.
        /// </summary>
        /// <param name="width">Aperture width in meters.</param>
        /// <param name="height">Aperture height in meters.</param>
        /// <returns>Cutoff frequency in Hz.</returns>
        /// <example>
        /// double cutoff = EMICalculator.WaveguideCutoff(0.02, 0.01); // Waveguide cutoff frequency
        /// </example>
        public static double WaveguideCutoff(double width, double height)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentException("Width and height must be positive.");
            
            // Dominant mode TE10 for rectangular waveguide
            return SpeedOfLight / (2 * Math.Max(width, height));
        }

        /// <summary>
        /// Calculates attenuation below cutoff frequency (evanescent mode).
        /// </summary>
        /// <param name="frequency">Operating frequency in Hz.</param>
        /// <param name="cutoffFrequency">Cutoff frequency in Hz.</param>
        /// <param name="length">Aperture depth in meters.</param>
        /// <returns>Attenuation in dB.</returns>
        public static double EvanescentModeAttenuation(double frequency, double cutoffFrequency, double length)
        {
            if (frequency <= 0 || cutoffFrequency <= 0 || length <= 0)
                throw new ArgumentException("All parameters must be positive.");
            if (frequency >= cutoffFrequency)
                return 0; // No attenuation above cutoff
            
            double ratio = frequency / cutoffFrequency;
            double attenuationConstant = (2 * Math.PI * cutoffFrequency / SpeedOfLight) * Math.Sqrt(1 - ratio * ratio);
            
            return 8.686 * attenuationConstant * length; // Convert Np to dB
        }

        /// <summary>
        /// Calculates required aperture size for given shielding effectiveness.
        /// </summary>
        /// <param name="targetSE">Target shielding effectiveness in dB.</param>
        /// <param name="frequency">Frequency in Hz.</param>
        /// <param name="depth">Aperture depth in meters.</param>
        /// <returns>Maximum aperture dimension in meters.</returns>
        public static double RequiredApertureSize(double targetSE, double frequency, double depth)
        {
            if (targetSE <= 0 || frequency <= 0 || depth <= 0)
                throw new ArgumentException("All parameters must be positive.");
            
            // Iterate to find aperture size that gives target SE
            double wavelength = SpeedOfLight / frequency;
            double maxDimension = wavelength / 2; // Start with half wavelength
            
            for (int i = 0; i < 100; i++) // Iterative solution
            {
                double cutoff = SpeedOfLight / (2 * maxDimension);
                double se = EvanescentModeAttenuation(frequency, cutoff, depth);
                
                if (Math.Abs(se - targetSE) < 0.1) // Within 0.1 dB
                    return maxDimension;
                
                if (se > targetSE)
                    maxDimension *= 1.1; // Increase size
                else
                    maxDimension *= 0.9; // Decrease size
            }
            
            return maxDimension;
        }

        /// <summary>
        /// Calculates radiated emissions from a current loop.
        /// </summary>
        /// <param name="current">Current in amperes (A).</param>
        /// <param name="loopArea">Loop area in square meters (m²).</param>
        /// <param name="frequency">Frequency in Hz.</param>
        /// <param name="distance">Distance from loop in meters (m).</param>
        /// <returns>Electric field strength in V/m.</returns>
        public static double RadiatedEmissions(double current, double loopArea, double frequency, double distance)
        {
            if (current < 0 || loopArea <= 0 || frequency <= 0 || distance <= 0)
                throw new ArgumentException("Current must be non-negative, other parameters must be positive.");
            
            double wavelength = SpeedOfLight / frequency;
            
            // Small loop antenna radiation
            if (Math.Sqrt(loopArea) < wavelength / 10) // Small loop condition
            {
                double magneticDipole = current * loopArea;
                return (2 * Math.PI * frequency * VacuumPermeability * magneticDipole) / (4 * Math.PI * distance * SpeedOfLight);
            }
            else
            {
                throw new ArgumentException("Loop is too large for small loop approximation.");
            }
        }

        /// <summary>
        /// Calculates common mode choke impedance.
        /// </summary>
        /// <param name="inductance">Choke inductance in henries (H).</param>
        /// <param name="frequency">Frequency in Hz.</param>
        /// <returns>Impedance magnitude in ohms (Ω).</returns>
        public static double CommonModeChokeImpedance(double inductance, double frequency)
        {
            if (inductance <= 0 || frequency <= 0)
                throw new ArgumentException("Inductance and frequency must be positive.");
            
            return 2 * Math.PI * frequency * inductance;
        }

        /// <summary>
        /// Calculates ferrite bead impedance approximation.
        /// </summary>
        /// <param name="impedanceAt100MHz">Impedance at 100 MHz in ohms (Ω).</param>
        /// <param name="frequency">Operating frequency in Hz.</param>
        /// <returns>Approximate impedance in ohms (Ω).</returns>
        public static double FerriteBeadImpedance(double impedanceAt100MHz, double frequency)
        {
            if (impedanceAt100MHz <= 0 || frequency <= 0)
                throw new ArgumentException("Reference impedance and frequency must be positive.");
            
            // Simplified model: impedance proportional to sqrt(frequency) up to resonance
            double referenceFreq = 100e6; // 100 MHz
            double ratio = frequency / referenceFreq;
            
            if (ratio <= 1)
                return impedanceAt100MHz * Math.Sqrt(ratio);
            else
                return impedanceAt100MHz / Math.Sqrt(ratio); // Above resonance
        }

        /// <summary>
        /// Calculates differential mode filter attenuation.
        /// </summary>
        /// <param name="inductance">Filter inductance in henries (H).</param>
        /// <param name="capacitance">Filter capacitance in farads (F).</param>
        /// <param name="frequency">Frequency in Hz.</param>
        /// <param name="sourceImpedance">Source impedance in ohms (Ω) (default: 50Ω).</param>
        /// <param name="loadImpedance">Load impedance in ohms (Ω) (default: 50Ω).</param>
        /// <returns>Attenuation in dB.</returns>
        public static double DifferentialModeFilterAttenuation(double inductance, double capacitance, double frequency,
                                                              double sourceImpedance = 50, double loadImpedance = 50)
        {
            if (inductance <= 0 || capacitance <= 0 || frequency <= 0)
                throw new ArgumentException("Inductance, capacitance, and frequency must be positive.");
            if (sourceImpedance <= 0 || loadImpedance <= 0)
                throw new ArgumentException("Source and load impedances must be positive.");
            
            double omega = 2 * Math.PI * frequency;
            double xl = omega * inductance;
            double xc = 1 / (omega * capacitance);
            
            // Simple L-C low-pass filter
            double transferFunction = 1 / Math.Sqrt(1 + Math.Pow(omega * Math.Sqrt(inductance * capacitance), 4));
            
            return -20 * Math.Log10(transferFunction);
        }
    }
}
