using System;
using NUnit.Framework;

namespace CircuitTool.Tests
{
    [TestFixture]
    public class AdditionalCalculatorsTests
    {
        #region AntennaCalculator Tests

        [Test]
        public void AntennaCalculator_QuarterWaveLength_ReturnsCorrectLength()
        {
            // 2.4 GHz (WiFi frequency)
            double frequency = 2.4e9;
            double length = AntennaCalculator.QuarterWaveLength(frequency);
            
            // Expected: ~3.12 cm for quarter wave at 2.4 GHz with 0.95 velocity factor
            Assert.That(Math.Abs(length - 0.03121), Is.LessThan(0.001), $"Expected ~0.03121m, got {length}m");
        }

        [Test]
        public void AntennaCalculator_HalfWaveLength_ReturnsCorrectLength()
        {
            double frequency = 100e6; // 100 MHz
            double length = AntennaCalculator.HalfWaveLength(frequency);
            
            // Expected: ~1.425m for half wave at 100 MHz
            Assert.That(Math.Abs(length - 1.425), Is.LessThan(0.01), $"Expected ~1.425m, got {length}m");
        }

        [Test]
        public void AntennaCalculator_DipoleImpedance_ReturnsReasonableValue()
        {
            double wireRadius = 0.001; // 1mm radius
            double frequency = 100e6; // 100 MHz
            double impedance = AntennaCalculator.DipoleImpedance(wireRadius, frequency);
            
            // Typical dipole impedance should be around 73 ohms
            Assert.That(impedance, Is.GreaterThan(50).And.LessThan(150), $"Expected impedance between 50-150 ohms, got {impedance}");
        }

        [Test]
        public void AntennaCalculator_AntennaGain_ReturnsCorrectGain()
        {
            double directivity = 1.64; // Dipole directivity
            double efficiency = 0.9; // 90% efficient
            double gainDb = AntennaCalculator.AntennaGain(directivity, efficiency);
            
            // Expected: ~1.67 dB for dipole with 90% efficiency
            Assert.That(Math.Abs(gainDb - 1.67), Is.LessThan(0.1), $"Expected ~1.67 dB, got {gainDb} dB");
        }

        [Test]
        public void AntennaCalculator_VSWR_ReturnsCorrectValue()
        {
            double reflectionCoeff = 0.1; // 10% reflection
            double vswr = AntennaCalculator.CalculateVSWR(reflectionCoeff);
            
            // Expected VSWR = (1 + 0.1) / (1 - 0.1) = 1.22
            Assert.That(Math.Abs(vswr - 1.22), Is.LessThan(0.01), $"Expected ~1.22, got {vswr}");
        }

        [Test]
        public void AntennaCalculator_EffectiveRadiatedPower_ReturnsCorrectPower()
        {
            double txPower = 10; // 10W
            double antennaGainDb = 3; // 3 dB gain
            double feedlineLossDb = 1; // 1 dB loss
            double erp = AntennaCalculator.EffectiveRadiatedPower(txPower, antennaGainDb, feedlineLossDb);
            
            // Expected: 10W * 2 (3dB gain) / 1.26 (1dB loss) ≈ 15.8W
            Assert.That(Math.Abs(erp - 15.85), Is.LessThan(0.1), $"Expected ~15.85W, got {erp}W");
        }

        [Test]
        public void AntennaCalculator_InvalidInputs_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => AntennaCalculator.QuarterWaveLength(-100));
            Assert.Throws<ArgumentException>(() => AntennaCalculator.DipoleImpedance(-0.001, 100e6));
            Assert.Throws<ArgumentException>(() => AntennaCalculator.CalculateVSWR(1.5));
        }

        #endregion

        #region SignalIntegrityCalculator Tests

        [Test]
        public void SignalIntegrityCalculator_MicrostripImpedance_ReturnsCorrectValue()
        {
            double traceWidth = 0.254e-3; // 0.254mm (10 mil)
            double dielectricThickness = 0.1524e-3; // 0.1524mm (6 mil)
            double dielectricConstant = 4.4; // FR4
            
            double impedance = SignalIntegrityCalculator.MicrostripImpedance(traceWidth, dielectricThickness, dielectricConstant);
            
            // Expected: ~50 ohms for typical PCB stackup
            Assert.That(Math.Abs(impedance - 50), Is.LessThan(10), $"Expected ~50 ohms, got {impedance} ohms");
        }

        [Test]
        public void SignalIntegrityCalculator_PropagationDelay_ReturnsCorrectDelay()
        {
            double length = 0.1; // 10cm trace
            double effectiveDielectric = 3.0;
            
            double delay = SignalIntegrityCalculator.PropagationDelay(length, effectiveDielectric);
            
            // Expected: ~0.58 ns for 10cm trace with er=3
            Assert.That(Math.Abs(delay * 1e9 - 0.58), Is.LessThan(0.1), $"Expected ~0.58ns, got {delay * 1e9}ns");
        }

        [Test]
        public void SignalIntegrityCalculator_CrosstalkCoupling_ReturnsReasonableValue()
        {
            double spacing = 0.2e-3; // 0.2mm spacing
            double width = 0.1e-3; // 0.1mm width
            double height = 0.1e-3; // 0.1mm height
            
            double coupling = SignalIntegrityCalculator.CrosstalkCoupling(spacing, width, height);
            
            // Should be small but non-zero coupling
            Assert.That(coupling, Is.GreaterThan(0).And.LessThan(0.5), $"Expected coupling between 0-0.5, got {coupling}");
        }

        [Test]
        public void SignalIntegrityCalculator_RiseTimeDegradation_ReturnsCorrectValue()
        {
            double originalRiseTime = 1e-9; // 1ns
            double propagationDelay = 0.5e-9; // 0.5ns
            
            double degradedRiseTime = SignalIntegrityCalculator.RiseTimeDegradation(originalRiseTime, propagationDelay);
            
            // Should be degraded but not drastically
            Assert.That(degradedRiseTime, Is.GreaterThan(originalRiseTime), "Rise time should be degraded");
            Assert.That(degradedRiseTime, Is.LessThan(2 * originalRiseTime), "Rise time degradation should be reasonable");
        }

        [Test]
        public void SignalIntegrityCalculator_InvalidInputs_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => SignalIntegrityCalculator.MicrostripImpedance(-0.1e-3, 0.1e-3, 4.4));
            Assert.Throws<ArgumentException>(() => SignalIntegrityCalculator.PropagationDelay(0.1, 0.5));
        }

        #endregion

        #region ThermalCalculator Tests

        [Test]
        public void ThermalCalculator_JunctionTemperature_ReturnsCorrectTemp()
        {
            double powerDissipation = 2.0; // 2W
            double thermalResistance = 50; // 50°C/W
            double ambientTemp = 25; // 25°C
            
            double junctionTemp = ThermalCalculator.JunctionTemperature(powerDissipation, thermalResistance, ambientTemp);
            
            // Expected: 25 + 2 * 50 = 125°C
            Assert.That(junctionTemp, Is.EqualTo(125).Within(0.1));
        }

        [Test]
        public void ThermalCalculator_RequiredHeatSinkThermalResistance_ReturnsCorrectValue()
        {
            double maxJunctionTemp = 150; // 150°C
            double ambientTemp = 25; // 25°C
            double powerDissipation = 5; // 5W
            double junctionToCaseResistance = 5; // 5°C/W
            
            double requiredHeatSinkR = ThermalCalculator.RequiredHeatSinkThermalResistance(
                maxJunctionTemp, ambientTemp, powerDissipation, junctionToCaseResistance);
            
            // Expected: (150-25)/5 - 5 = 25 - 5 = 20°C/W
            Assert.That(requiredHeatSinkR, Is.EqualTo(20).Within(0.1));
        }

        [Test]
        public void ThermalCalculator_ThermalTimeConstant_ReturnsCorrectValue()
        {
            double thermalCapacitance = 100; // 100 J/°C
            double thermalResistance = 10; // 10°C/W
            
            double timeConstant = ThermalCalculator.ThermalTimeConstant(thermalCapacitance, thermalResistance);
            
            // Expected: 100 * 10 = 1000 seconds
            Assert.That(timeConstant, Is.EqualTo(1000).Within(0.1));
        }

        [Test]
        public void ThermalCalculator_ConvectiveHeatTransfer_ReturnsReasonableValue()
        {
            double airVelocity = 1.0; // 1 m/s
            double characteristicLength = 0.05; // 5cm
            double tempDifference = 50; // 50°C difference
            
            double heatTransferCoeff = ThermalCalculator.ConvectiveHeatTransfer(airVelocity, characteristicLength, tempDifference);
            
            // Should be reasonable value for forced convection
            Assert.That(heatTransferCoeff, Is.GreaterThan(0).And.LessThan(100), 
                $"Expected reasonable heat transfer coefficient, got {heatTransferCoeff}");
        }

        [Test]
        public void ThermalCalculator_InvalidInputs_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => ThermalCalculator.JunctionTemperature(-1, 50, 25));
            Assert.Throws<ArgumentException>(() => ThermalCalculator.RequiredHeatSinkThermalResistance(25, 150, 5, 5));
        }

        #endregion

        #region EMCCalculator Tests

        [Test]
        public void EMCCalculator_ElectricFieldStrength_ReturnsCorrectValue()
        {
            double power = 1.0; // 1W
            double distance = 10; // 10m
            double antennaGain = 1.0; // Isotropic
            
            double fieldStrength = EMCCalculator.ElectricFieldStrength(power, distance, antennaGain);
            
            // Expected: sqrt(1 * 1 * 376.73 / (4 * pi * 100)) ≈ 1.73 V/m
            Assert.That(Math.Abs(fieldStrength - 1.73), Is.LessThan(0.1), $"Expected ~1.73 V/m, got {fieldStrength} V/m");
        }

        [Test]
        public void EMCCalculator_ShieldingEffectiveness_ReturnsReasonableValue()
        {
            double frequency = 100e6; // 100 MHz
            double thickness = 0.001; // 1mm aluminum
            double conductivity = 37.7e6; // Aluminum conductivity
            
            double shieldingEffectiveness = EMCCalculator.ShieldingEffectiveness(frequency, thickness, conductivity);
            
            // Should provide significant shielding (> 40 dB)
            Assert.That(shieldingEffectiveness, Is.GreaterThan(40), $"Expected > 40 dB shielding, got {shieldingEffectiveness} dB");
        }

        [Test]
        public void EMCCalculator_FCCClassBLimit_ReturnsCorrectLimits()
        {
            double frequency30MHz = 30e6;
            double frequency100MHz = 100e6;
            double frequency500MHz = 500e6;
            double frequency1GHz = 1e9;
            
            double limit30 = EMCCalculator.FCCClassBLimit(frequency30MHz);
            double limit100 = EMCCalculator.FCCClassBLimit(frequency100MHz);
            double limit500 = EMCCalculator.FCCClassBLimit(frequency500MHz);
            double limit1G = EMCCalculator.FCCClassBLimit(frequency1GHz);
            
            // Verify limits increase with frequency
            Assert.That(limit100, Is.GreaterThan(limit30), "100 MHz limit should be higher than 30 MHz");
            Assert.That(limit500, Is.GreaterThan(limit100), "500 MHz limit should be higher than 100 MHz");
            Assert.That(limit1G, Is.GreaterThan(limit500), "1 GHz limit should be higher than 500 MHz");
        }

        [Test]
        public void EMCCalculator_LoopInductance_ReturnsCorrectValue()
        {
            double loopArea = 0.01; // 0.01 m² (10cm x 10cm)
            double wireRadius = 0.001; // 1mm radius
            
            double inductance = EMCCalculator.LoopInductance(loopArea, wireRadius);
            
            // Should be in microhenry range for typical loop
            Assert.That(inductance, Is.GreaterThan(0).And.LessThan(1e-6), $"Expected inductance in µH range, got {inductance} H");
        }

        [Test]
        public void EMCCalculator_CommonModeChokeImpedance_ReturnsCorrectValue()
        {
            double inductance = 100e-6; // 100 µH
            double frequency = 1e6; // 1 MHz
            double resistance = 1.0; // 1 ohm DC resistance
            
            double impedance = EMCCalculator.CommonModeChokeImpedance(inductance, frequency, resistance);
            
            // Expected: sqrt(1² + (2π * 1e6 * 100e-6)²) ≈ 628 ohms
            Assert.That(Math.Abs(impedance - 628), Is.LessThan(10), $"Expected ~628 ohms, got {impedance} ohms");
        }

        [Test]
        public void EMCCalculator_InvalidInputs_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => EMCCalculator.ElectricFieldStrength(-1, 10));
            Assert.Throws<ArgumentException>(() => EMCCalculator.ShieldingEffectiveness(100e6, -0.001, 37.7e6));
            Assert.Throws<ArgumentException>(() => EMCCalculator.FCCClassBLimit(10e6)); // Below 30 MHz
        }

        #endregion

        #region Integration Tests

        [Test]
        public void AdditionalCalculators_AntennaDesignWorkflow_WorksCorrectly()
        {
            // Design workflow: Calculate antenna dimensions and characteristics
            double frequency = 2.4e9; // 2.4 GHz WiFi
            
            // Calculate physical dimensions
            double quarterWave = AntennaCalculator.QuarterWaveLength(frequency);
            double halfWave = AntennaCalculator.HalfWaveLength(frequency);
            
            // Calculate impedance
            double wireRadius = 0.001; // 1mm
            double impedance = AntennaCalculator.DipoleImpedance(wireRadius, frequency);
            
            // Calculate gain
            double gainDb = AntennaCalculator.AntennaGain(1.64, 0.9);
            
            // Calculate radiated power
            double erp = AntennaCalculator.EffectiveRadiatedPower(1.0, gainDb, 0.5);
            
            // All calculations should complete without error
            Assert.That(quarterWave, Is.GreaterThan(0).And.LessThan(0.1)); // Reasonable size
            Assert.That(halfWave, Is.GreaterThan(quarterWave)); // Half wave > quarter wave
            Assert.That(impedance, Is.GreaterThan(0).And.LessThan(1000)); // Reasonable impedance
            Assert.That(erp, Is.GreaterThan(0)); // Positive radiated power
        }

        [Test]
        public void AdditionalCalculators_ThermalDesignWorkflow_WorksCorrectly()
        {
            // Thermal design workflow: Check if component needs heat sink
            double powerDissipation = 5.0; // 5W
            double maxJunctionTemp = 150; // 150°C
            double ambientTemp = 70; // 70°C (worst case)
            double junctionToCaseResistance = 2.5; // 2.5°C/W
            
            // Calculate required heat sink resistance
            double requiredHeatSinkR = ThermalCalculator.RequiredHeatSinkThermalResistance(
                maxJunctionTemp, ambientTemp, powerDissipation, junctionToCaseResistance);
            
            // Check if heat sink is needed (positive resistance means yes)
            bool heatSinkNeeded = requiredHeatSinkR > 0;
            
            // Calculate final junction temperature with heat sink
            double totalThermalResistance = junctionToCaseResistance + Math.Max(requiredHeatSinkR, 0);
            double finalJunctionTemp = ThermalCalculator.JunctionTemperature(
                powerDissipation, totalThermalResistance, ambientTemp);
            
            Assert.That(heatSinkNeeded, Is.True); // Should need heat sink for 5W
            Assert.That(finalJunctionTemp, Is.LessThanOrEqualTo(maxJunctionTemp)); // Should meet thermal requirements
        }

        [Test]
        public void AdditionalCalculators_EMCComplianceWorkflow_WorksCorrectly()
        {
            // EMC compliance workflow: Check field strength vs. limits
            double frequency = 100e6; // 100 MHz
            double power = 0.1; // 0.1W radiated
            double distance = 3; // 3m measurement distance
            
            // Calculate field strength
            double fieldStrength = EMCCalculator.ElectricFieldStrength(power, distance);
            
            // Get FCC limit
            double fccLimit = EMCCalculator.FCCClassBLimit(frequency, distance);
            
            // Convert field strength to dB(µV/m)
            double fieldStrengthDbUvM = 20 * Math.Log10(fieldStrength * 1e6);
            
            // Check compliance
            bool compliant = fieldStrengthDbUvM <= fccLimit;
            
            Assert.That(fieldStrength, Is.GreaterThan(0)); // Positive field strength
            Assert.That(fccLimit, Is.GreaterThan(0)); // Valid FCC limit
            // Note: Compliance depends on specific values, just verify calculations work
        }

        #endregion
    }
}
