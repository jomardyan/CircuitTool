using Microsoft.VisualStudio.TestTools.UnitTesting;
using CircuitTool;
using System;

namespace CircuitTool.Tests
{
    [TestClass]
    public class LEDCalculatorTests
    {
        [TestMethod]
        public void CalculateResistorValue_ValidInputs_ReturnsCorrectValue()
        {
            // Arrange
            double supplyVoltage = 9.0;
            double ledVoltage = 2.0;
            double ledCurrent = 0.02; // 20mA

            // Act
            double result = LEDCalculator.CalculateResistorValue(supplyVoltage, ledVoltage, ledCurrent);

            // Assert
            Assert.AreEqual(350.0, result, 0.01);
        }

        [TestMethod]
        public void CalculateLEDPower_ValidInputs_ReturnsCorrectValue()
        {
            // Arrange
            double supplyVoltage = 5.0;
            double ledCurrent = 0.02;

            // Act
            double result = LEDCalculator.CalculateLEDPower(supplyVoltage, ledCurrent);

            // Assert
            Assert.AreEqual(0.1, result, 0.01);
        }

        [TestMethod]
        public void CalculateBrightness_ValidDutyCycle_ReturnsCorrectRatio()
        {
            // Arrange
            double dutyCycle = 75.0;

            // Act
            double result = LEDCalculator.CalculateBrightness(dutyCycle);

            // Assert
            Assert.AreEqual(0.75, result, 0.01);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CalculateResistorValue_InvalidCurrent_ThrowsException()
        {
            LEDCalculator.CalculateResistorValue(9.0, 2.0, 0.0);
        }
    }
}
