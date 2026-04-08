using NUnit.Framework;
using CircuitTool;
using System;

namespace CircuitTool.Tests
{
    [TestFixture]
    public class CapacitorCalculatorTests
    {
        [Test]
        public void CapacitiveReactance_ValidInputs_ReturnsCorrectValue()
        {
            // Arrange
            double frequency = 60; // 60 Hz
            double capacitance = 0.000001; // 1μF
            
            // Act
            double result = CapacitorCalculator.CapacitiveReactance(frequency, capacitance);
            
            // Assert
            double expected = 1.0 / (2 * Math.PI * 60 * 0.000001);
            Assert.That(result, Is.EqualTo(expected).Within(0.001));
        }

        [Test]
        public void CapacitiveReactance_ZeroFrequency_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => CapacitorCalculator.CapacitiveReactance(0, 0.000001));
        }

        [Test]
        public void CapacitiveReactance_ZeroCapacitance_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => CapacitorCalculator.CapacitiveReactance(60, 0));
        }

        [Test]
        public void EnergyStored_ValidInputs_ReturnsCorrectValue()
        {
            // Arrange
            double capacitance = 0.001; // 1000μF
            double voltage = 12; // 12V
            
            // Act
            double result = CapacitorCalculator.EnergyStored(capacitance, voltage);
            
            // Assert
            double expected = 0.5 * 0.001 * 12 * 12; // 0.072 J
            Assert.That(result, Is.EqualTo(expected).Within(0.001));
        }

        [Test]
        public void EnergyStored_ZeroVoltage_ReturnsZero()
        {
            // Arrange & Act
            double result = CapacitorCalculator.EnergyStored(0.001, 0);
            
            // Assert
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void TimeConstant_ValidInputs_ReturnsCorrectValue()
        {
            // Arrange
            double resistance = 1000; // 1kΩ
            double capacitance = 0.000001; // 1μF
            
            // Act
            double result = CapacitorCalculator.TimeConstant(resistance, capacitance);
            
            // Assert
            Assert.That(result, Is.EqualTo(0.001).Within(0.000001)); // 1ms
        }

        [Test]
        public void SeriesCapacitance_TwoCapacitors_ReturnsCorrectValue()
        {
            // Arrange
            double[] capacitances = { 0.000010, 0.000020 }; // 10μF, 20μF
            
            // Act
            double result = CapacitorCalculator.SeriesCapacitance(capacitances);
            
            // Assert
            // 1/Ctotal = 1/10μF + 1/20μF = 3/20μF, so Ctotal = 20μF/3 ≈ 6.67μF
            double expected = 1.0 / (1.0/0.000010 + 1.0/0.000020);
            Assert.That(result, Is.EqualTo(expected).Within(0.000000001));
        }

        [Test]
        public void SeriesCapacitance_NullArray_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => CapacitorCalculator.SeriesCapacitance(null));
        }

        [Test]
        public void SeriesCapacitance_EmptyArray_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => CapacitorCalculator.SeriesCapacitance(new double[0]));
        }

        [Test]
        public void ParallelCapacitance_TwoCapacitors_ReturnsCorrectValue()
        {
            // Arrange
            double[] capacitances = { 0.000010, 0.000020 }; // 10μF, 20μF
            
            // Act
            double result = CapacitorCalculator.ParallelCapacitance(capacitances);
            
            // Assert
            Assert.That(result, Is.EqualTo(0.000030).Within(0.000000001)); // 30μF
        }

        [Test]
        public void ParallelCapacitance_NullArray_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => CapacitorCalculator.ParallelCapacitance(null));
        }

        [Test]
        public void ChargingVoltage_ValidInputs_ReturnsCorrectValue()
        {
            // Arrange
            double sourceVoltage = 12; // 12V
            double timeConstant = 0.001; // 1ms
            double time = 0.001; // 1ms (one time constant)
            
            // Act
            double result = CapacitorCalculator.ChargingVoltage(sourceVoltage, timeConstant, time);
            
            // Assert
            // At t = τ, voltage should be about 63.2% of source voltage
            double expected = 12 * (1 - Math.Exp(-1)); // ≈ 7.58V
            Assert.That(result, Is.EqualTo(expected).Within(0.01));
        }

        [Test]
        public void ChargingVoltage_ZeroTime_ReturnsZero()
        {
            // Arrange & Act
            double result = CapacitorCalculator.ChargingVoltage(12, 0.001, 0);
            
            // Assert
            Assert.That(result, Is.EqualTo(0).Within(0.001));
        }

        [Test]
        public void DischargingVoltage_ValidInputs_ReturnsCorrectValue()
        {
            // Arrange
            double initialVoltage = 12; // 12V
            double timeConstant = 0.001; // 1ms
            double time = 0.001; // 1ms (one time constant)
            
            // Act
            double result = CapacitorCalculator.DischargingVoltage(initialVoltage, timeConstant, time);
            
            // Assert
            // At t = τ, voltage should be about 36.8% of initial voltage
            double expected = 12 * Math.Exp(-1); // ≈ 4.42V
            Assert.That(result, Is.EqualTo(expected).Within(0.01));
        }

        [Test]
        public void DischargingVoltage_ZeroTime_ReturnsInitialVoltage()
        {
            // Arrange & Act
            double result = CapacitorCalculator.DischargingVoltage(12, 0.001, 0);
            
            // Assert
            Assert.That(result, Is.EqualTo(12).Within(0.001));
        }

        [Test]
        public void DischargingVoltage_NegativeTime_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => CapacitorCalculator.DischargingVoltage(12, 0.001, -1));
        }
    }
}
