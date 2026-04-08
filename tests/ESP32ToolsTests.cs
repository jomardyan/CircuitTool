using NUnit.Framework;
using CircuitTool;
using System;

namespace CircuitTool.Tests
{
    [TestFixture]
    public class ESP32ToolsTests
    {
        [Test]
        public void AnalogToVoltage_ValidReading_ReturnsCorrectVoltage()
        {
            // Arrange
            int analogReading = 2048; // Half of 4095
            double expectedVoltage = 1.65; // Half of 3.3V

            // Act
            double result = ESP32Tools.AnalogToVoltage(analogReading);

            // Assert
            Assert.That(result, Is.EqualTo(expectedVoltage).Within(0.01));
        }

        [Test]
        public void CalculateWiFiPowerConsumption_ActiveMode_ReturnsCorrectCurrent()
        {
            // Act
            double result = ESP32Tools.CalculateWiFiPowerConsumption(WiFiMode.Active);

            // Assert
            Assert.That(result, Is.EqualTo(80.0));
        }

        [Test]
        public void CalculateBatteryLife_ValidInputs_ReturnsCorrectHours()
        {
            // Arrange
            double batteryCapacity = 1000; // 1000mAh
            double averageCurrent = 50; // 50mA
            double efficiency = 0.8;

            // Act
            double result = ESP32Tools.CalculateBatteryLife(batteryCapacity, averageCurrent, efficiency);

            // Assert
            Assert.That(result, Is.EqualTo(16.0).Within(0.01));
        }

        [Test]
        public void AnalogToVoltage_InvalidReading_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => ESP32Tools.AnalogToVoltage(4096));
        }
    }
}
