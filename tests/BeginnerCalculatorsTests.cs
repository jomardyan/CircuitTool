using NUnit.Framework;
using CircuitTool;
using System;

namespace CircuitTool.Tests
{
    [TestFixture]
    public class BeginnerCalculatorsTests
    {
        [Test]
        public void BatteryLifeCalculator_ValidInputs_ReturnsCorrectHours()
        {
            // Arrange
            double batteryCapacity = 1000; // 1000mAh
            double loadCurrent = 50; // 50mA

            // Act
            double result = BeginnerCalculators.BatteryLifeCalculator(batteryCapacity, loadCurrent);

            // Assert
            Assert.That(result, Is.EqualTo(20.0).Within(0.01));
        }

        [Test]
        public void WireGaugeCalculator_LowCurrent_ReturnsCorrectGauge()
        {
            // Arrange
            double current = 2.0; // 2A

            // Act
            int result = BeginnerCalculators.WireGaugeCalculator(current);

            // Assert
            Assert.That(result, Is.EqualTo(20)); // 2A * 1.5 safety factor = 3A, which requires AWG 20
        }

        [Test]
        public void PowerRatioToDecibels_ValidRatio_ReturnsCorrectDB()
        {
            // Arrange
            double powerRatio = 10.0;

            // Act
            double result = BeginnerCalculators.PowerRatioToDecibels(powerRatio);

            // Assert
            Assert.That(result, Is.EqualTo(10.0).Within(0.01));
        }

        [Test]
        public void RCTimeConstantCapacitor_ValidInputs_ReturnsCorrectCapacitance()
        {
            // Arrange
            double resistance = 1000.0; // 1kΩ
            double timeConstant = 0.001; // 1ms

            // Act
            double result = BeginnerCalculators.RCTimeConstantCapacitor(resistance, timeConstant);

            // Assert
            Assert.That(result, Is.EqualTo(0.000001).Within(0.0000001)); // 1µF
        }

        [Test]
        public void BatteryLifeCalculator_InvalidCurrent_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => BeginnerCalculators.BatteryLifeCalculator(1000, 0));
        }
    }
}
