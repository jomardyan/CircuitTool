using NUnit.Framework;
using CircuitTool;
using System;

namespace CircuitTool.Tests
{
    [TestFixture]
    public class FilterCalculatorTests
    {
        [Test]
        public void RCLowPassCutoffFrequency_ValidInputs_ReturnsCorrectValue()
        {
            // Arrange
            double resistance = 1000; // 1kΩ
            double capacitance = 0.000001; // 1μF
            
            // Act
            double result = FilterCalculator.RCLowPassCutoffFrequency(resistance, capacitance);
            
            // Assert
            double expected = 1.0 / (2 * Math.PI * 1000 * 0.000001); // ≈ 159.15 Hz
            Assert.That(result, Is.EqualTo(expected).Within(0.01));
        }

        [Test]
        public void RCLowPassCutoffFrequency_ZeroResistance_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => FilterCalculator.RCLowPassCutoffFrequency(0, 0.000001));
        }

        [Test]
        public void RCLowPassCutoffFrequency_ZeroCapacitance_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => FilterCalculator.RCLowPassCutoffFrequency(1000, 0));
        }

        [Test]
        public void RCHighPassCutoffFrequency_ValidInputs_ReturnsCorrectValue()
        {
            // Arrange
            double resistance = 1000; // 1kΩ
            double capacitance = 0.000001; // 1μF
            
            // Act
            double result = FilterCalculator.RCHighPassCutoffFrequency(resistance, capacitance);
            
            // Assert
            // Should be same formula as low-pass
            double expected = 1.0 / (2 * Math.PI * 1000 * 0.000001); // ≈ 159.15 Hz
            Assert.That(result, Is.EqualTo(expected).Within(0.01));
        }

        [Test]
        public void RLLowPassCutoffFrequency_ValidInputs_ReturnsCorrectValue()
        {
            // Arrange
            double resistance = 100; // 100Ω
            double inductance = 0.01; // 10mH
            
            // Act
            double result = FilterCalculator.RLLowPassCutoffFrequency(resistance, inductance);
            
            // Assert
            double expected = 100 / (2 * Math.PI * 0.01); // ≈ 1591.5 Hz
            Assert.That(result, Is.EqualTo(expected).Within(0.1));
        }

        [Test]
        public void RLLowPassCutoffFrequency_ZeroInductance_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => FilterCalculator.RLLowPassCutoffFrequency(100, 0));
        }

        [Test]
        public void RLHighPassCutoffFrequency_ValidInputs_ReturnsCorrectValue()
        {
            // Arrange
            double resistance = 100; // 100Ω
            double inductance = 0.01; // 10mH
            
            // Act
            double result = FilterCalculator.RLHighPassCutoffFrequency(resistance, inductance);
            
            // Assert
            // Should be same formula as RL low-pass
            double expected = 100 / (2 * Math.PI * 0.01); // ≈ 1591.5 Hz
            Assert.That(result, Is.EqualTo(expected).Within(0.1));
        }

        [Test]
        public void GainInDecibels_UnityGain_ReturnsZero()
        {
            // Arrange
            double outputVoltage = 1.0;
            double inputVoltage = 1.0;
            
            // Act
            double result = FilterCalculator.GainInDecibels(outputVoltage, inputVoltage);
            
            // Assert
            Assert.That(result, Is.EqualTo(0).Within(0.001));
        }

        [Test]
        public void GainInDecibels_ValidInputs_ReturnsCorrectValue()
        {
            // Arrange
            double outputVoltage = 0.707; // -3dB point
            double inputVoltage = 1.0;
            
            // Act
            double result = FilterCalculator.GainInDecibels(outputVoltage, inputVoltage);
            
            // Assert
            double expected = 20 * Math.Log10(0.707); // ≈ -3.01 dB
            Assert.That(result, Is.EqualTo(expected).Within(0.01));
        }

        [Test]
        public void GainInDecibels_ZeroInput_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => FilterCalculator.GainInDecibels(1.0, 0));
        }

        [Test]
        public void RCLowPassPhaseShift_AtCutoffFrequency_ReturnsMinus45Degrees()
        {
            // Arrange
            double resistance = 1000; // 1kΩ
            double capacitance = 0.000001; // 1μF
            double cutoffFreq = 1.0 / (2 * Math.PI * resistance * capacitance);
            
            // Act
            double result = FilterCalculator.RCLowPassPhaseShift(cutoffFreq, resistance, capacitance);
            
            // Assert
            Assert.That(result, Is.EqualTo(-45).Within(0.01));
        }

        [Test]
        public void RCLowPassPhaseShift_ZeroFrequency_ReturnsZero()
        {
            // Arrange & Act
            double result = FilterCalculator.RCLowPassPhaseShift(0, 1000, 0.000001);
            
            // Assert
            Assert.That(result, Is.EqualTo(0).Within(0.001));
        }

        [Test]
        public void RCHighPassPhaseShift_AtCutoffFrequency_Returns45Degrees()
        {
            // Arrange
            double resistance = 1000; // 1kΩ
            double capacitance = 0.000001; // 1μF
            double cutoffFreq = 1.0 / (2 * Math.PI * resistance * capacitance);
            
            // Act
            double result = FilterCalculator.RCHighPassPhaseShift(cutoffFreq, resistance, capacitance);
            
            // Assert
            Assert.That(result, Is.EqualTo(45).Within(0.01));
        }

        [Test]
        public void RCLowPassMagnitudeResponse_AtCutoffFrequency_ReturnsPoint707()
        {
            // Arrange
            double frequency = 1000; // 1kHz
            double cutoffFrequency = 1000; // 1kHz
            
            // Act
            double result = FilterCalculator.RCLowPassMagnitudeResponse(frequency, cutoffFrequency);
            
            // Assert
            double expected = 1.0 / Math.Sqrt(2); // ≈ 0.707
            Assert.That(result, Is.EqualTo(expected).Within(0.001));
        }

        [Test]
        public void RCLowPassMagnitudeResponse_ZeroFrequency_ReturnsOne()
        {
            // Arrange & Act
            double result = FilterCalculator.RCLowPassMagnitudeResponse(0, 1000);
            
            // Assert
            Assert.That(result, Is.EqualTo(1).Within(0.001));
        }

        [Test]
        public void RCHighPassMagnitudeResponse_AtCutoffFrequency_ReturnsPoint707()
        {
            // Arrange
            double frequency = 1000; // 1kHz
            double cutoffFrequency = 1000; // 1kHz
            
            // Act
            double result = FilterCalculator.RCHighPassMagnitudeResponse(frequency, cutoffFrequency);
            
            // Assert
            double expected = 1.0 / Math.Sqrt(2); // ≈ 0.707
            Assert.That(result, Is.EqualTo(expected).Within(0.001));
        }

        [Test]
        public void RCHighPassMagnitudeResponse_ZeroFrequency_ReturnsZero()
        {
            // Arrange & Act
            double result = FilterCalculator.RCHighPassMagnitudeResponse(0, 1000);
            
            // Assert
            Assert.That(result, Is.EqualTo(0).Within(0.001));
        }

        [Test]
        public void RequiredCapacitanceForCutoff_ValidInputs_ReturnsCorrectValue()
        {
            // Arrange
            double cutoffFrequency = 159.15; // Hz
            double resistance = 1000; // 1kΩ
            
            // Act
            double result = FilterCalculator.RequiredCapacitanceForCutoff(cutoffFrequency, resistance);
            
            // Assert
            double expected = 1.0 / (2 * Math.PI * 159.15 * 1000); // ≈ 1μF
            Assert.That(result, Is.EqualTo(expected).Within(0.000000001));
        }

        [Test]
        public void RequiredCapacitanceForCutoff_ZeroCutoffFrequency_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => FilterCalculator.RequiredCapacitanceForCutoff(0, 1000));
        }

        [Test]
        public void RequiredResistanceForCutoff_ValidInputs_ReturnsCorrectValue()
        {
            // Arrange
            double cutoffFrequency = 159.15; // Hz
            double capacitance = 0.000001; // 1μF
            
            // Act
            double result = FilterCalculator.RequiredResistanceForCutoff(cutoffFrequency, capacitance);
            
            // Assert
            double expected = 1.0 / (2 * Math.PI * 159.15 * 0.000001); // ≈ 1kΩ
            Assert.That(result, Is.EqualTo(expected).Within(0.1));
        }

        [Test]
        public void RequiredResistanceForCutoff_ZeroCapacitance_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => FilterCalculator.RequiredResistanceForCutoff(159.15, 0));
        }
    }
}
