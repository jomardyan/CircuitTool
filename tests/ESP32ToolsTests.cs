using Microsoft.VisualStudio.TestTools.UnitTesting;
using CircuitTool;
using System;

namespace CircuitTool.Tests
{
    [TestClass]
    public class ESP32ToolsTests
    {
        [TestMethod]
        public void AnalogToVoltage_ValidReading_ReturnsCorrectVoltage()
        {
            // Arrange
            int analogReading = 2048; // Half of 4095
            double expectedVoltage = 1.65; // Half of 3.3V

            // Act
            double result = ESP32Tools.AnalogToVoltage(analogReading);

            // Assert
            Assert.AreEqual(expectedVoltage, result, 0.01);
        }

        [TestMethod]
        public void CalculateWiFiPowerConsumption_ActiveMode_ReturnsCorrectCurrent()
        {
            // Act
            double result = ESP32Tools.CalculateWiFiPowerConsumption(WiFiMode.Active);

            // Assert
            Assert.AreEqual(80.0, result);
        }

        [TestMethod]
        public void CalculateBatteryLife_ValidInputs_ReturnsCorrectHours()
        {
            // Arrange
            double batteryCapacity = 1000; // 1000mAh
            double averageCurrent = 50; // 50mA
            double efficiency = 0.8;

            // Act
            double result = ESP32Tools.CalculateBatteryLife(batteryCapacity, averageCurrent, efficiency);

            // Assert
            Assert.AreEqual(16.0, result, 0.01);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AnalogToVoltage_InvalidReading_ThrowsException()
        {
            ESP32Tools.AnalogToVoltage(4096);
        }
    }
}
