using Microsoft.VisualStudio.TestTools.UnitTesting;
using CircuitTool;
using System;

namespace CircuitTool.Tests
{
    [TestClass]
    public class ArduinoToolsTests
    {
        [TestMethod]
        public void AnalogToVoltage_ValidReading_ReturnsCorrectVoltage()
        {
            // Arrange
            int analogReading = 512; // Half of 1023
            double expectedVoltage = 2.5; // Half of 5V

            // Act
            double result = ArduinoTools.AnalogToVoltage(analogReading);

            // Assert
            Assert.AreEqual(expectedVoltage, result, 0.01);
        }

        [TestMethod]
        public void VoltageToAnalog_ValidVoltage_ReturnsCorrectReading()
        {
            // Arrange
            double voltage = 2.5;
            int expectedReading = 512;

            // Act
            int result = ArduinoTools.VoltageToAnalog(voltage);

            // Assert
            Assert.AreEqual(expectedReading, result);
        }

        [TestMethod]
        public void ServoAngleToPulseWidth_ValidAngle_ReturnsCorrectPulseWidth()
        {
            // Arrange
            double angle = 90.0; // Middle position
            double expectedPulseWidth = 1500.0; // 1.5ms

            // Act
            double result = ArduinoTools.ServoAngleToPulseWidth(angle);

            // Assert
            Assert.AreEqual(expectedPulseWidth, result, 0.01);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AnalogToVoltage_InvalidReading_ThrowsException()
        {
            ArduinoTools.AnalogToVoltage(1024);
        }
    }
}
