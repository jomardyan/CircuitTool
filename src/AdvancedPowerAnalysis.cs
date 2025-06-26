#nullable enable
using System;
using System.Numerics;
using CircuitTool.Units;

namespace CircuitTool.PowerAnalysis
{
    /// <summary>
    /// Advanced power analysis including three-phase systems and power quality
    /// </summary>
    public static class AdvancedPowerAnalysis
    {
        /// <summary>
        /// Calculates three-phase power values
        /// </summary>
        /// <param name="voltageA">Phase A voltage</param>
        /// <param name="voltageB">Phase B voltage</param>
        /// <param name="voltageC">Phase C voltage</param>
        /// <param name="currentA">Phase A current</param>
        /// <param name="currentB">Phase B current</param>
        /// <param name="currentC">Phase C current</param>
        /// <returns>Three-phase power analysis results</returns>
        public static ThreePhasePower CalculateThreePhasePower(
            ACVoltage voltageA, ACVoltage voltageB, ACVoltage voltageC,
            ACCurrent currentA, ACCurrent currentB, ACCurrent currentC)
        {
            // Calculate complex power for each phase
            var powerA = voltageA.Complex * Complex.Conjugate(currentA.Complex);
            var powerB = voltageB.Complex * Complex.Conjugate(currentB.Complex);
            var powerC = voltageC.Complex * Complex.Conjugate(currentC.Complex);

            // Total three-phase power
            var totalPower = powerA + powerB + powerC;

            // Line-to-line voltages
            var vab = voltageA.Complex - voltageB.Complex;
            var vbc = voltageB.Complex - voltageC.Complex;
            var vca = voltageC.Complex - voltageA.Complex;

            var lineToLineVoltage = (vab.Magnitude + vbc.Magnitude + vca.Magnitude) / 3.0;

            // Calculate sequence components
            var sequenceComponents = CalculateSequenceComponents(voltageA, voltageB, voltageC);

            return new ThreePhasePower(
                totalPower.Real, totalPower.Imaginary, totalPower.Magnitude,
                powerA, powerB, powerC,
                lineToLineVoltage, sequenceComponents);
        }

        /// <summary>
        /// Calculates balanced three-phase power from line values
        /// </summary>
        /// <param name="lineToLineVoltage">Line-to-line voltage (RMS)</param>
        /// <param name="lineCurrent">Line current (RMS)</param>
        /// <param name="powerFactor">Power factor</param>
        /// <param name="isLeading">True if leading power factor</param>
        /// <returns>Balanced three-phase power results</returns>
        public static BalancedThreePhasePower CalculateBalancedThreePhasePower(
            double lineToLineVoltage,
            double lineCurrent,
            double powerFactor,
            bool isLeading = false)
        {
            var phaseAngle = Math.Acos(powerFactor);
            if (isLeading) phaseAngle = -phaseAngle;

            var realPower = Math.Sqrt(3) * lineToLineVoltage * lineCurrent * powerFactor;
            var reactivePower = Math.Sqrt(3) * lineToLineVoltage * lineCurrent * Math.Sin(phaseAngle);
            var apparentPower = Math.Sqrt(3) * lineToLineVoltage * lineCurrent;

            var phaseVoltage = lineToLineVoltage / Math.Sqrt(3);
            var phaseCurrent = lineCurrent; // For wye connection

            return new BalancedThreePhasePower(
                realPower, reactivePower, apparentPower,
                lineToLineVoltage, phaseVoltage,
                lineCurrent, phaseCurrent,
                powerFactor, isLeading);
        }

        /// <summary>
        /// Analyzes harmonic content and calculates power quality metrics
        /// </summary>
        /// <param name="fundamentalVoltage">Fundamental voltage component</param>
        /// <param name="fundamentalCurrent">Fundamental current component</param>
        /// <param name="voltageHarmonics">Voltage harmonic components</param>
        /// <param name="currentHarmonics">Current harmonic components</param>
        /// <returns>Harmonic power analysis results</returns>
        public static HarmonicPowerAnalysis AnalyzeHarmonicPower(
            ACVoltage fundamentalVoltage,
            ACCurrent fundamentalCurrent,
            ACVoltage[] voltageHarmonics,
            ACCurrent[] currentHarmonics)
        {
            if (voltageHarmonics.Length != currentHarmonics.Length)
                throw new ArgumentException("Voltage and current harmonic arrays must have the same length");

            // Calculate fundamental power
            var fundamentalPower = fundamentalVoltage.Complex * Complex.Conjugate(fundamentalCurrent.Complex);

            // Calculate harmonic powers
            var harmonicPowers = new Complex[voltageHarmonics.Length];
            var totalHarmonicPower = Complex.Zero;

            for (int i = 0; i < voltageHarmonics.Length; i++)
            {
                harmonicPowers[i] = voltageHarmonics[i].Complex * Complex.Conjugate(currentHarmonics[i].Complex);
                totalHarmonicPower += harmonicPowers[i];
            }

            // Calculate distortion power
            var distortionPower = CalculateDistortionPower(fundamentalVoltage, fundamentalCurrent, 
                                                         voltageHarmonics, currentHarmonics);

            // Calculate THD values
            var voltageThd = CalculateTHD(fundamentalVoltage.Magnitude, 
                                        voltageHarmonics.Select(v => v.Magnitude).ToArray());
            var currentThd = CalculateTHD(fundamentalCurrent.Magnitude, 
                                        currentHarmonics.Select(i => i.Magnitude).ToArray());

            // Calculate power factor components
            var totalRealPower = fundamentalPower.Real + totalHarmonicPower.Real;
            var totalApparentPower = Math.Sqrt(Math.Pow(fundamentalPower.Magnitude, 2) + 
                                             Math.Pow(totalHarmonicPower.Magnitude, 2) + 
                                             Math.Pow(distortionPower.Magnitude, 2));

            var truePowerFactor = totalRealPower / totalApparentPower;
            var displacementPowerFactor = fundamentalPower.Real / fundamentalPower.Magnitude;
            var distortionFactor = fundamentalPower.Magnitude / totalApparentPower;

            return new HarmonicPowerAnalysis(
                fundamentalPower, totalHarmonicPower, distortionPower,
                harmonicPowers, voltageThd, currentThd,
                truePowerFactor, displacementPowerFactor, distortionFactor);
        }

        /// <summary>
        /// Calculates power quality indices
        /// </summary>
        /// <param name="voltageWaveform">Voltage waveform samples</param>
        /// <param name="currentWaveform">Current waveform samples</param>
        /// <param name="nominalVoltage">Nominal voltage RMS value</param>
        /// <param name="nominalFrequency">Nominal frequency in Hz</param>
        /// <returns>Power quality analysis results</returns>
        public static PowerQualityAnalysis AnalyzePowerQuality(
            double[] voltageWaveform,
            double[] currentWaveform,
            double nominalVoltage,
            double nominalFrequency)
        {
            if (voltageWaveform.Length != currentWaveform.Length)
                throw new ArgumentException("Voltage and current waveforms must have the same length");

            // Calculate RMS values
            var voltageRms = CalculateRMS(voltageWaveform);
            var currentRms = CalculateRMS(currentWaveform);

            // Calculate voltage regulation
            var voltageRegulation = Math.Abs(voltageRms - nominalVoltage) / nominalVoltage;

            // Calculate crest factors
            var voltageCrestFactor = voltageWaveform.Max() / voltageRms;
            var currentCrestFactor = currentWaveform.Max() / currentRms;

            // Calculate form factor
            var voltageFormFactor = voltageRms / CalculateAverageRectified(voltageWaveform);

            // Calculate power factor using correlation
            var powerFactor = CalculatePowerFactor(voltageWaveform, currentWaveform);

            // Estimate frequency deviation (simplified)
            var frequencyDeviation = EstimateFrequencyDeviation(voltageWaveform, nominalFrequency);

            // Calculate unbalance (simplified - assumes single phase input)
            var voltageUnbalance = 0.0; // Would need three-phase data for actual calculation

            return new PowerQualityAnalysis(
                voltageRms, currentRms, voltageRegulation,
                voltageCrestFactor, currentCrestFactor, voltageFormFactor,
                powerFactor, frequencyDeviation, voltageUnbalance);
        }

        private static SequenceComponents CalculateSequenceComponents(
            ACVoltage voltageA, ACVoltage voltageB, ACVoltage voltageC)
        {
            // Unit operator for sequence component calculation
            var a = Complex.FromPolarCoordinates(1.0, 2 * Math.PI / 3); // 120° rotation
            var a2 = a * a; // 240° rotation

            // Positive sequence
            var positiveSequence = (voltageA.Complex + a * voltageB.Complex + a2 * voltageC.Complex) / 3.0;

            // Negative sequence  
            var negativeSequence = (voltageA.Complex + a2 * voltageB.Complex + a * voltageC.Complex) / 3.0;

            // Zero sequence
            var zeroSequence = (voltageA.Complex + voltageB.Complex + voltageC.Complex) / 3.0;

            return new SequenceComponents(
                new ACVoltage(positiveSequence),
                new ACVoltage(negativeSequence),
                new ACVoltage(zeroSequence));
        }

        private static Complex CalculateDistortionPower(
            ACVoltage fundamentalVoltage, ACCurrent fundamentalCurrent,
            ACVoltage[] voltageHarmonics, ACCurrent[] currentHarmonics)
        {
            // Simplified distortion power calculation
            var vRmsSquared = Math.Pow(fundamentalVoltage.Magnitude, 2) + 
                             voltageHarmonics.Sum(v => Math.Pow(v.Magnitude, 2));
            var iRmsSquared = Math.Pow(fundamentalCurrent.Magnitude, 2) + 
                             currentHarmonics.Sum(i => Math.Pow(i.Magnitude, 2));

            var apparentPowerSquared = vRmsSquared * iRmsSquared;
            var fundamentalPowerSquared = Math.Pow(fundamentalVoltage.Magnitude * fundamentalCurrent.Magnitude, 2);
            var harmonicPowerSquared = voltageHarmonics.Zip(currentHarmonics, (v, i) => 
                Math.Pow(v.Magnitude * i.Magnitude, 2)).Sum();

            var distortionPowerSquared = apparentPowerSquared - fundamentalPowerSquared - harmonicPowerSquared;
            
            return new Complex(Math.Sqrt(Math.Max(0, distortionPowerSquared)), 0);
        }

        private static double CalculateTHD(double fundamental, double[] harmonics)
        {
            var harmonicSum = harmonics.Sum(h => h * h);
            return Math.Sqrt(harmonicSum) / fundamental;
        }

        private static double CalculateRMS(double[] waveform)
        {
            var sumSquares = waveform.Sum(x => x * x);
            return Math.Sqrt(sumSquares / waveform.Length);
        }

        private static double CalculateAverageRectified(double[] waveform)
        {
            return waveform.Select(Math.Abs).Average();
        }

        private static double CalculatePowerFactor(double[] voltage, double[] current)
        {
            // Cross-correlation at zero lag
            var vRms = CalculateRMS(voltage);
            var iRms = CalculateRMS(current);
            
            var correlation = 0.0;
            for (int i = 0; i < voltage.Length; i++)
            {
                correlation += voltage[i] * current[i];
            }
            
            return correlation / (voltage.Length * vRms * iRms);
        }

        private static double EstimateFrequencyDeviation(double[] waveform, double nominalFrequency)
        {
            // Simplified frequency estimation using zero crossings
            var zeroCrossings = 0;
            for (int i = 1; i < waveform.Length; i++)
            {
                if ((waveform[i] >= 0) != (waveform[i - 1] >= 0))
                    zeroCrossings++;
            }

            // Assume sample rate allows for frequency estimation
            var estimatedFrequency = zeroCrossings * nominalFrequency / (2 * waveform.Length);
            return (estimatedFrequency - nominalFrequency) / nominalFrequency;
        }
    }

    /// <summary>
    /// Three-phase power analysis results
    /// </summary>
    public readonly struct ThreePhasePower
    {
        public double TotalRealPower { get; }
        public double TotalReactivePower { get; }
        public double TotalApparentPower { get; }
        public Complex PhaseAPower { get; }
        public Complex PhaseBPower { get; }
        public Complex PhaseCPower { get; }
        public double LineToLineVoltage { get; }
        public SequenceComponents SequenceComponents { get; }

        public ThreePhasePower(double totalRealPower, double totalReactivePower, double totalApparentPower,
                              Complex phaseAPower, Complex phaseBPower, Complex phaseCPower,
                              double lineToLineVoltage, SequenceComponents sequenceComponents)
        {
            TotalRealPower = totalRealPower;
            TotalReactivePower = totalReactivePower;
            TotalApparentPower = totalApparentPower;
            PhaseAPower = phaseAPower;
            PhaseBPower = phaseBPower;
            PhaseCPower = phaseCPower;
            LineToLineVoltage = lineToLineVoltage;
            SequenceComponents = sequenceComponents;
        }

        public double PowerFactor => TotalRealPower / TotalApparentPower;

        public override string ToString() =>
            $"3φ Power: {TotalRealPower:F1} W, {TotalReactivePower:F1} VAR, {TotalApparentPower:F1} VA, PF={PowerFactor:F3}";
    }

    /// <summary>
    /// Balanced three-phase power results
    /// </summary>
    public readonly struct BalancedThreePhasePower
    {
        public double RealPower { get; }
        public double ReactivePower { get; }
        public double ApparentPower { get; }
        public double LineToLineVoltage { get; }
        public double PhaseVoltage { get; }
        public double LineCurrent { get; }
        public double PhaseCurrent { get; }
        public double PowerFactor { get; }
        public bool IsLeading { get; }

        public BalancedThreePhasePower(double realPower, double reactivePower, double apparentPower,
                                     double lineToLineVoltage, double phaseVoltage,
                                     double lineCurrent, double phaseCurrent,
                                     double powerFactor, bool isLeading)
        {
            RealPower = realPower;
            ReactivePower = reactivePower;
            ApparentPower = apparentPower;
            LineToLineVoltage = lineToLineVoltage;
            PhaseVoltage = phaseVoltage;
            LineCurrent = lineCurrent;
            PhaseCurrent = phaseCurrent;
            PowerFactor = powerFactor;
            IsLeading = isLeading;
        }

        public string PowerFactorType => IsLeading ? "Leading" : "Lagging";

        public override string ToString() =>
            $"Balanced 3φ: {RealPower:F1} W, PF={PowerFactor:F3} {PowerFactorType}";
    }

    /// <summary>
    /// Sequence components for three-phase analysis
    /// </summary>
    public readonly struct SequenceComponents
    {
        public ACVoltage PositiveSequence { get; }
        public ACVoltage NegativeSequence { get; }
        public ACVoltage ZeroSequence { get; }

        public SequenceComponents(ACVoltage positiveSequence, ACVoltage negativeSequence, ACVoltage zeroSequence)
        {
            PositiveSequence = positiveSequence;
            NegativeSequence = negativeSequence;
            ZeroSequence = zeroSequence;
        }

        public double UnbalanceFactor => NegativeSequence.Magnitude / PositiveSequence.Magnitude;

        public override string ToString() =>
            $"Seq: V₁={PositiveSequence.Magnitude:F2}V, V₂={NegativeSequence.Magnitude:F2}V, V₀={ZeroSequence.Magnitude:F2}V";
    }

    /// <summary>
    /// Harmonic power analysis results
    /// </summary>
    public readonly struct HarmonicPowerAnalysis
    {
        public Complex FundamentalPower { get; }
        public Complex TotalHarmonicPower { get; }
        public Complex DistortionPower { get; }
        public Complex[] HarmonicPowers { get; }
        public double VoltageTHD { get; }
        public double CurrentTHD { get; }
        public double TruePowerFactor { get; }
        public double DisplacementPowerFactor { get; }
        public double DistortionFactor { get; }

        public HarmonicPowerAnalysis(Complex fundamentalPower, Complex totalHarmonicPower, Complex distortionPower,
                                   Complex[] harmonicPowers, double voltageTHD, double currentTHD,
                                   double truePowerFactor, double displacementPowerFactor, double distortionFactor)
        {
            FundamentalPower = fundamentalPower;
            TotalHarmonicPower = totalHarmonicPower;
            DistortionPower = distortionPower;
            HarmonicPowers = harmonicPowers ?? throw new ArgumentNullException(nameof(harmonicPowers));
            VoltageTHD = voltageTHD;
            CurrentTHD = currentTHD;
            TruePowerFactor = truePowerFactor;
            DisplacementPowerFactor = displacementPowerFactor;
            DistortionFactor = distortionFactor;
        }

        public double VoltageTHDPercent => VoltageTHD * 100;
        public double CurrentTHDPercent => CurrentTHD * 100;

        public override string ToString() =>
            $"Harmonic Analysis: VTHD={VoltageTHDPercent:F1}%, ITHD={CurrentTHDPercent:F1}%, TPF={TruePowerFactor:F3}";
    }

    /// <summary>
    /// Power quality analysis results
    /// </summary>
    public readonly struct PowerQualityAnalysis
    {
        public double VoltageRMS { get; }
        public double CurrentRMS { get; }
        public double VoltageRegulation { get; }
        public double VoltageCrestFactor { get; }
        public double CurrentCrestFactor { get; }
        public double VoltageFormFactor { get; }
        public double PowerFactor { get; }
        public double FrequencyDeviation { get; }
        public double VoltageUnbalance { get; }

        public PowerQualityAnalysis(double voltageRMS, double currentRMS, double voltageRegulation,
                                  double voltageCrestFactor, double currentCrestFactor, double voltageFormFactor,
                                  double powerFactor, double frequencyDeviation, double voltageUnbalance)
        {
            VoltageRMS = voltageRMS;
            CurrentRMS = currentRMS;
            VoltageRegulation = voltageRegulation;
            VoltageCrestFactor = voltageCrestFactor;
            CurrentCrestFactor = currentCrestFactor;
            VoltageFormFactor = voltageFormFactor;
            PowerFactor = powerFactor;
            FrequencyDeviation = frequencyDeviation;
            VoltageUnbalance = voltageUnbalance;
        }

        public double VoltageRegulationPercent => VoltageRegulation * 100;
        public double FrequencyDeviationPercent => FrequencyDeviation * 100;
        public double VoltageUnbalancePercent => VoltageUnbalance * 100;

        public override string ToString() =>
            $"PQ: Vreg={VoltageRegulationPercent:F2}%, CF={VoltageCrestFactor:F2}, PF={PowerFactor:F3}";
    }
}

// Extension methods for compatibility
internal static class PowerAnalysisExtensions
{
    public static double[] ToArray(this IEnumerable<double> source)
    {
        return source.ToArray();
    }

    public static double Sum(this double[] source)
    {
        double sum = 0;
        for (int i = 0; i < source.Length; i++)
        {
            sum += source[i];
        }
        return sum;
    }

    public static double Sum(this double[] source, Func<double, double> selector)
    {
        double sum = 0;
        for (int i = 0; i < source.Length; i++)
        {
            sum += selector(source[i]);
        }
        return sum;
    }

    public static double Max(this double[] source)
    {
        if (source.Length == 0) throw new InvalidOperationException();
        
        double max = source[0];
        for (int i = 1; i < source.Length; i++)
        {
            if (source[i] > max) max = source[i];
        }
        return max;
    }

    public static double Average(this double[] source)
    {
        return source.Sum() / source.Length;
    }

    public static IEnumerable<TResult> Zip<T1, T2, TResult>(this T1[] first, T2[] second, Func<T1, T2, TResult> func)
    {
        int minLength = Math.Min(first.Length, second.Length);
        for (int i = 0; i < minLength; i++)
        {
            yield return func(first[i], second[i]);
        }
    }

    public static double[] Select(this double[] source, Func<double, double> selector)
    {
        var result = new double[source.Length];
        for (int i = 0; i < source.Length; i++)
        {
            result[i] = selector(source[i]);
        }
        return result;
    }
}
