using NUnit.Framework;
using CircuitTool;
using System;

namespace CircuitTool.Tests
{
    [TestFixture]
    public class RFCalculatorTests
    {
        [Test]
        public void Wavelength_ValidFrequency_ReturnsCorrectValue()
        {
            // Arrange
            double frequency = 2.4e9; // 2.4 GHz
            double expected = 0.125; // meters

            // Act
            double result = RFCalculator.Wavelength(frequency);

            // Assert
            Assert.That(result, Is.EqualTo(expected).Within(0.001));
        }

        [Test]
        public void VSWR_ValidReflectionCoefficient_ReturnsCorrectValue()
        {
            // Arrange
            double reflectionCoeff = 0.5;
            double expected = 3.0;

            // Act
            double result = RFCalculator.VSWR(reflectionCoeff);

            // Assert
            Assert.That(result, Is.EqualTo(expected).Within(0.001));
        }

        [Test]
        public void PathLoss_ValidParameters_ReturnsCorrectValue()
        {
            // Arrange
            double frequency = 2.4e9;
            double distance = 100;

            // Act
            double result = RFCalculator.PathLoss(frequency, distance);

            // Assert
            Assert.IsTrue(result > 0);
        }

        [Test]
        public void Wavelength_NegativeFrequency_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => RFCalculator.Wavelength(-1000));
        }

        [Test]
        public void VSWR_InvalidReflectionCoefficient_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => RFCalculator.VSWR(1.5)); // Greater than 1
        }
    }
}
