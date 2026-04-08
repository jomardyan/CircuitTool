using NUnit.Framework;
using CircuitTool;
using System;

namespace CircuitTool.Tests
{
    [TestFixture]
    public class ArduinoToolsTests
    {
        [Test]
        public void AnalogToVoltage_ValidReading_ReturnsCorrectVoltage()
        {
            // Arrange
            int analogReading = 512; // Half of 1023
            double expectedVoltage = 2.5; // Half of 5V

            // Act
            double result = ArduinoTools.AnalogToVoltage(analogReading);

            // Assert
            Assert.That(result, Is.EqualTo(expectedVoltage).Within(0.01));
        }

        [Test]
        public void VoltageToAnalog_ValidVoltage_ReturnsCorrectReading()
        {
            // Arrange
            double voltage = 2.5;
            int expectedReading = 512;

            // Act
            int result = ArduinoTools.VoltageToAnalog(voltage);

            // Assert
            Assert.That(result, Is.EqualTo(expectedReading));
        }

        [Test]
        public void ServoAngleToPulseWidth_ValidAngle_ReturnsCorrectPulseWidth()
        {
            // Arrange
            double angle = 90.0; // Middle position
            double expectedPulseWidth = 1500.0; // 1.5ms

            // Act
            double result = ArduinoTools.ServoAngleToPulseWidth(angle);

            // Assert
            Assert.That(result, Is.EqualTo(expectedPulseWidth).Within(0.01));
        }

        [Test]
        public void AnalogToVoltage_InvalidReading_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => ArduinoTools.AnalogToVoltage(1024));
        }
    }
}
