using Microsoft.VisualStudio.TestTools.UnitTesting;
using CircuitTool;
using System;

namespace CircuitTool.Tests
{
    [TestClass]
    public class BeginnerCalculatorsTests
    {
        [TestMethod]
        public void BatteryLifeCalculator_ValidInputs_ReturnsCorrectHours()
        {
            // Arrange
            double batteryCapacity = 1000; // 1000mAh
            double loadCurrent = 50; // 50mA

            // Act
            double result = BeginnerCalculators.BatteryLifeCalculator(batteryCapacity, loadCurrent);

            // Assert
            Assert.AreEqual(20.0, result, 0.01);
        }

        [TestMethod]
        public void WireGaugeCalculator_LowCurrent_ReturnsCorrectGauge()
        {
            // Arrange
            double current = 2.0; // 2A

            // Act
            int result = BeginnerCalculators.WireGaugeCalculator(current);

            // Assert
            Assert.AreEqual(18, result);
        }

        [TestMethod]
        public void PowerRatioToDecibels_ValidRatio_ReturnsCorrectDB()
        {
            // Arrange
            double powerRatio = 10.0;

            // Act
            double result = BeginnerCalculators.PowerRatioToDecibels(powerRatio);

            // Assert
            Assert.AreEqual(10.0, result, 0.01);
        }

        [TestMethod]
        public void RCTimeConstantCapacitor_ValidInputs_ReturnsCorrectCapacitance()
        {
            // Arrange
            double resistance = 1000.0; // 1kΩ
            double timeConstant = 0.001; // 1ms

            // Act
            double result = BeginnerCalculators.RCTimeConstantCapacitor(resistance, timeConstant);

            // Assert
            Assert.AreEqual(0.000001, result, 0.0000001); // 1µF
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void BatteryLifeCalculator_InvalidCurrent_ThrowsException()
        {
            BeginnerCalculators.BatteryLifeCalculator(1000, 0);
        }
    }
}
