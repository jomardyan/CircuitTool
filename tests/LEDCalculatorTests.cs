using NUnit.Framework;
using CircuitTool;
using System;

namespace CircuitTool.Tests
{
    [TestFixture]
    public class LEDCalculatorTests
    {
        [Test]
        public void CalculateResistorValue_ValidInputs_ReturnsCorrectValue()
        {
            // Arrange
            double supplyVoltage = 9.0;
            double ledVoltage = 2.0;
            double ledCurrent = 0.02; // 20mA

            // Act
            double result = LEDCalculator.CalculateResistorValue(supplyVoltage, ledVoltage, ledCurrent);

            // Assert
            Assert.That(result, Is.EqualTo(350.0).Within(0.01));
        }

        [Test]
        public void CalculateLEDPower_ValidInputs_ReturnsCorrectValue()
        {
            // Arrange
            double supplyVoltage = 5.0;
            double ledCurrent = 0.02;

            // Act
            double result = LEDCalculator.CalculateLEDPower(supplyVoltage, ledCurrent);

            // Assert
            Assert.That(result, Is.EqualTo(0.1).Within(0.01));
        }

        [Test]
        public void CalculateBrightness_ValidDutyCycle_ReturnsCorrectRatio()
        {
            // Arrange
            double dutyCycle = 75.0;

            // Act
            double result = LEDCalculator.CalculateBrightness(dutyCycle);

            // Assert
            Assert.That(result, Is.EqualTo(0.75).Within(0.01));
        }

        [Test]
        public void CalculateResistorValue_InvalidCurrent_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => LEDCalculator.CalculateResistorValue(9.0, 2.0, 0.0));
        }
    }
}
