using NUnit.Framework;
using CircuitTool;
using System;

namespace CircuitTool.Tests
{
    [TestFixture]
    public class ACCircuitCalculatorTests
    {
        [Test]
        public void ImpedanceMagnitude_ResistiveCircuit_ReturnsResistance()
        {
            // Arrange
            double resistance = 50; // 50Ω
            double inductiveReactance = 0;
            double capacitiveReactance = 0;
            
            // Act
            double result = ACCircuitCalculator.ImpedanceMagnitude(resistance, inductiveReactance, capacitiveReactance);
            
            // Assert
            Assert.That(result, Is.EqualTo(50).Within(0.001));
        }

        [Test]
        public void ImpedanceMagnitude_SeriesRLCircuit_ReturnsCorrectValue()
        {
            // Arrange
            double resistance = 30; // 30Ω
            double inductiveReactance = 40; // 40Ω
            double capacitiveReactance = 0;
            
            // Act
            double result = ACCircuitCalculator.ImpedanceMagnitude(resistance, inductiveReactance, capacitiveReactance);
            
            // Assert
            double expected = Math.Sqrt(30*30 + 40*40); // 50Ω
            Assert.That(result, Is.EqualTo(expected).Within(0.001));
        }

        [Test]
        public void ImpedanceMagnitude_SeriesRCCircuit_ReturnsCorrectValue()
        {
            // Arrange
            double resistance = 30; // 30Ω
            double inductiveReactance = 0;
            double capacitiveReactance = 40; // 40Ω
            
            // Act
            double result = ACCircuitCalculator.ImpedanceMagnitude(resistance, inductiveReactance, capacitiveReactance);
            
            // Assert
            double expected = Math.Sqrt(30*30 + 40*40); // 50Ω
            Assert.That(result, Is.EqualTo(expected).Within(0.001));
        }

        [Test]
        public void ImpedanceMagnitude_NegativeResistance_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => ACCircuitCalculator.ImpedanceMagnitude(-10, 20, 0));
        }

        [Test]
        public void PhaseAngle_ResistiveCircuit_ReturnsZero()
        {
            // Arrange
            double resistance = 50;
            double inductiveReactance = 0;
            double capacitiveReactance = 0;
            
            // Act
            double result = ACCircuitCalculator.PhaseAngle(resistance, inductiveReactance, capacitiveReactance);
            
            // Assert
            Assert.That(result, Is.EqualTo(0).Within(0.001));
        }

        [Test]
        public void PhaseAngle_InductiveCircuit_ReturnsPositiveAngle()
        {
            // Arrange
            double resistance = 30;
            double inductiveReactance = 40;
            double capacitiveReactance = 0;
            
            // Act
            double result = ACCircuitCalculator.PhaseAngle(resistance, inductiveReactance, capacitiveReactance);
            
            // Assert
            double expected = Math.Atan(40.0 / 30.0) * (180.0 / Math.PI); // ≈ 53.13°
            Assert.That(result, Is.EqualTo(expected).Within(0.01));
        }

        [Test]
        public void PhaseAngle_CapacitiveCircuit_ReturnsNegativeAngle()
        {
            // Arrange
            double resistance = 30;
            double inductiveReactance = 0;
            double capacitiveReactance = 40;
            
            // Act
            double result = ACCircuitCalculator.PhaseAngle(resistance, inductiveReactance, capacitiveReactance);
            
            // Assert
            double expected = Math.Atan(-40.0 / 30.0) * (180.0 / Math.PI); // ≈ -53.13°
            Assert.That(result, Is.EqualTo(expected).Within(0.01));
        }

        [Test]
        public void PhaseAngle_PurelyInductive_Returns90Degrees()
        {
            // Arrange
            double resistance = 0;
            double inductiveReactance = 40;
            double capacitiveReactance = 0;
            
            // Act
            double result = ACCircuitCalculator.PhaseAngle(resistance, inductiveReactance, capacitiveReactance);
            
            // Assert
            Assert.That(result, Is.EqualTo(90).Within(0.001));
        }

        [Test]
        public void PowerFactor_ResistiveCircuit_ReturnsOne()
        {
            // Arrange
            double resistance = 50;
            double impedanceMagnitude = 50;
            
            // Act
            double result = ACCircuitCalculator.PowerFactor(resistance, impedanceMagnitude);
            
            // Assert
            Assert.That(result, Is.EqualTo(1).Within(0.001));
        }

        [Test]
        public void PowerFactor_ValidInputs_ReturnsCorrectValue()
        {
            // Arrange
            double resistance = 30;
            double impedanceMagnitude = 50;
            
            // Act
            double result = ACCircuitCalculator.PowerFactor(resistance, impedanceMagnitude);
            
            // Assert
            Assert.That(result, Is.EqualTo(0.6).Within(0.001)); // cos(53.13°) = 0.6
        }

        [Test]
        public void PowerFactor_ZeroImpedance_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => ACCircuitCalculator.PowerFactor(30, 0));
        }

        [Test]
        public void PeakToRMS_ValidInput_ReturnsCorrectValue()
        {
            // Arrange
            double peakValue = 100;
            
            // Act
            double result = ACCircuitCalculator.PeakToRMS(peakValue);
            
            // Assert
            double expected = 100 / Math.Sqrt(2); // ≈ 70.71
            Assert.That(result, Is.EqualTo(expected).Within(0.01));
        }

        [Test]
        public void RMSToPeak_ValidInput_ReturnsCorrectValue()
        {
            // Arrange
            double rmsValue = 70.71;
            
            // Act
            double result = ACCircuitCalculator.RMSToPeak(rmsValue);
            
            // Assert
            double expected = 70.71 * Math.Sqrt(2); // ≈ 100
            Assert.That(result, Is.EqualTo(expected).Within(0.01));
        }

        [Test]
        public void PeakToAverage_ValidInput_ReturnsCorrectValue()
        {
            // Arrange
            double peakValue = 100;
            
            // Act
            double result = ACCircuitCalculator.PeakToAverage(peakValue);
            
            // Assert
            double expected = (2 * 100) / Math.PI; // ≈ 63.66
            Assert.That(result, Is.EqualTo(expected).Within(0.01));
        }

        [Test]
        public void PeakToPeakToPeak_ValidInput_ReturnsCorrectValue()
        {
            // Arrange
            double peakValue = 100;
            
            // Act
            double result = ACCircuitCalculator.PeakToPeakToPeak(peakValue);
            
            // Assert
            Assert.That(result, Is.EqualTo(200).Within(0.001));
        }

        [Test]
        public void FormFactor_SinusoidalWaveform_ReturnsCorrectValue()
        {
            // Arrange
            double rmsValue = 70.71;
            double averageValue = 63.66;
            
            // Act
            double result = ACCircuitCalculator.FormFactor(rmsValue, averageValue);
            
            // Assert
            double expected = 70.71 / 63.66; // ≈ 1.11 (π/(2√2))
            Assert.That(result, Is.EqualTo(expected).Within(0.01));
        }

        [Test]
        public void FormFactor_ZeroAverage_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => ACCircuitCalculator.FormFactor(70.71, 0));
        }

        [Test]
        public void CrestFactor_SinusoidalWaveform_ReturnsCorrectValue()
        {
            // Arrange
            double peakValue = 100;
            double rmsValue = 70.71;
            
            // Act
            double result = ACCircuitCalculator.CrestFactor(peakValue, rmsValue);
            
            // Assert
            double expected = 100 / 70.71; // ≈ 1.414 (√2)
            Assert.That(result, Is.EqualTo(expected).Within(0.01));
        }

        [Test]
        public void CrestFactor_ZeroRMS_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => ACCircuitCalculator.CrestFactor(100, 0));
        }

        [Test]
        public void QualityFactor_ValidInputs_ReturnsCorrectValue()
        {
            // Arrange
            double inductiveReactance = 100;
            double resistance = 10;
            
            // Act
            double result = ACCircuitCalculator.QualityFactor(inductiveReactance, resistance);
            
            // Assert
            Assert.That(result, Is.EqualTo(10).Within(0.001));
        }

        [Test]
        public void QualityFactor_ZeroResistance_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => ACCircuitCalculator.QualityFactor(100, 0));
        }

        [Test]
        public void Bandwidth_ValidInputs_ReturnsCorrectValue()
        {
            // Arrange
            double resonantFrequency = 1000; // 1kHz
            double qualityFactor = 10;
            
            // Act
            double result = ACCircuitCalculator.Bandwidth(resonantFrequency, qualityFactor);
            
            // Assert
            Assert.That(result, Is.EqualTo(100).Within(0.001)); // 100Hz
        }

        [Test]
        public void Bandwidth_ZeroQualityFactor_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => ACCircuitCalculator.Bandwidth(1000, 0));
        }
    }
}
