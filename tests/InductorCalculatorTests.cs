using NUnit.Framework;
using CircuitTool;
using System;

namespace CircuitTool.Tests
{
    [TestFixture]
    public class InductorCalculatorTests
    {
        [Test]
        public void InductiveReactance_ValidInputs_ReturnsCorrectValue()
        {
            // Arrange
            double frequency = 60; // 60 Hz
            double inductance = 0.1; // 100mH
            
            // Act
            double result = InductorCalculator.InductiveReactance(frequency, inductance);
            
            // Assert
            double expected = 2 * Math.PI * 60 * 0.1; // ≈ 37.7 Ω
            Assert.That(result, Is.EqualTo(expected).Within(0.001));
        }

        [Test]
        public void InductiveReactance_ZeroFrequency_ReturnsZero()
        {
            // Arrange & Act
            double result = InductorCalculator.InductiveReactance(0, 0.1);
            
            // Assert
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void InductiveReactance_NegativeInductance_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => InductorCalculator.InductiveReactance(60, -0.1));
        }

        [Test]
        public void EnergyStored_ValidInputs_ReturnsCorrectValue()
        {
            // Arrange
            double inductance = 0.01; // 10mH
            double current = 2; // 2A
            
            // Act
            double result = InductorCalculator.EnergyStored(inductance, current);
            
            // Assert
            double expected = 0.5 * 0.01 * 2 * 2; // 0.02 J
            Assert.That(result, Is.EqualTo(expected).Within(0.001));
        }

        [Test]
        public void EnergyStored_ZeroCurrent_ReturnsZero()
        {
            // Arrange & Act
            double result = InductorCalculator.EnergyStored(0.01, 0);
            
            // Assert
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void TimeConstant_ValidInputs_ReturnsCorrectValue()
        {
            // Arrange
            double inductance = 0.01; // 10mH
            double resistance = 10; // 10Ω
            
            // Act
            double result = InductorCalculator.TimeConstant(inductance, resistance);
            
            // Assert
            Assert.That(result, Is.EqualTo(0.001).Within(0.000001)); // 1ms
        }

        [Test]
        public void TimeConstant_ZeroResistance_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => InductorCalculator.TimeConstant(0.01, 0));
        }

        [Test]
        public void SeriesInductance_TwoInductors_ReturnsCorrectValue()
        {
            // Arrange
            double[] inductances = { 0.01, 0.02 }; // 10mH, 20mH
            
            // Act
            double result = InductorCalculator.SeriesInductance(inductances);
            
            // Assert
            Assert.That(result, Is.EqualTo(0.03).Within(0.000001)); // 30mH
        }

        [Test]
        public void SeriesInductance_NullArray_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => InductorCalculator.SeriesInductance(null));
        }

        [Test]
        public void SeriesInductance_EmptyArray_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => InductorCalculator.SeriesInductance(new double[0]));
        }

        [Test]
        public void ParallelInductance_TwoInductors_ReturnsCorrectValue()
        {
            // Arrange
            double[] inductances = { 0.02, 0.02 }; // 20mH, 20mH
            
            // Act
            double result = InductorCalculator.ParallelInductance(inductances);
            
            // Assert
            // 1/Ltotal = 1/20mH + 1/20mH = 2/20mH, so Ltotal = 10mH
            Assert.That(result, Is.EqualTo(0.01).Within(0.000001));
        }

        [Test]
        public void ParallelInductance_NullArray_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => InductorCalculator.ParallelInductance(null));
        }

        [Test]
        public void CurrentBuildup_ValidInputs_ReturnsCorrectValue()
        {
            // Arrange
            double finalCurrent = 5; // 5A
            double timeConstant = 0.001; // 1ms
            double time = 0.001; // 1ms (one time constant)
            
            // Act
            double result = InductorCalculator.CurrentBuildup(finalCurrent, timeConstant, time);
            
            // Assert
            // At t = τ, current should be about 63.2% of final current
            double expected = 5 * (1 - Math.Exp(-1)); // ≈ 3.16A
            Assert.That(result, Is.EqualTo(expected).Within(0.01));
        }

        [Test]
        public void CurrentBuildup_ZeroTime_ReturnsZero()
        {
            // Arrange & Act
            double result = InductorCalculator.CurrentBuildup(5, 0.001, 0);
            
            // Assert
            Assert.That(result, Is.EqualTo(0).Within(0.001));
        }

        [Test]
        public void CurrentDecay_ValidInputs_ReturnsCorrectValue()
        {
            // Arrange
            double initialCurrent = 5; // 5A
            double timeConstant = 0.001; // 1ms
            double time = 0.001; // 1ms (one time constant)
            
            // Act
            double result = InductorCalculator.CurrentDecay(initialCurrent, timeConstant, time);
            
            // Assert
            // At t = τ, current should be about 36.8% of initial current
            double expected = 5 * Math.Exp(-1); // ≈ 1.84A
            Assert.That(result, Is.EqualTo(expected).Within(0.01));
        }

        [Test]
        public void CurrentDecay_ZeroTime_ReturnsInitialCurrent()
        {
            // Arrange & Act
            double result = InductorCalculator.CurrentDecay(5, 0.001, 0);
            
            // Assert
            Assert.That(result, Is.EqualTo(5).Within(0.001));
        }

        [Test]
        public void ResonantFrequency_ValidInputs_ReturnsCorrectValue()
        {
            // Arrange
            double inductance = 0.001; // 1mH
            double capacitance = 0.000001; // 1μF
            
            // Act
            double result = InductorCalculator.ResonantFrequency(inductance, capacitance);
            
            // Assert
            double expected = 1.0 / (2 * Math.PI * Math.Sqrt(0.001 * 0.000001)); // ≈ 5033 Hz
            Assert.That(result, Is.EqualTo(expected).Within(1));
        }

        [Test]
        public void ResonantFrequency_ZeroInductance_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => InductorCalculator.ResonantFrequency(0, 0.000001));
        }

        [Test]
        public void ResonantFrequency_ZeroCapacitance_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => InductorCalculator.ResonantFrequency(0.001, 0));
        }
    }
}
