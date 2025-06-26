using NUnit.Framework;
using CircuitTool;

namespace CircuitTool.Tests
{
    [TestFixture]
    public class VoltageDividerCalculatorTests
    {
        [Test]
        public void Calculate_EqualResistors_ReturnsHalfVoltage()
        {
            // Arrange
            double vin = 10.0;
            double r1 = 1000;
            double r2 = 1000;
            
            // Act
            double result = VoltageDividerCalculator.Calculate(vin, r1, r2);
            
            // Assert
            Assert.That(result, Is.EqualTo(5.0).Within(0.001));
        }

        [Test]
        public void Calculate_R2TenTimesR1_ReturnsCorrectRatio()
        {
            // Arrange
            double vin = 12.0;
            double r1 = 1000;  // 1kΩ
            double r2 = 10000; // 10kΩ
            
            // Act
            double result = VoltageDividerCalculator.Calculate(vin, r1, r2);
            
            // Assert
            double expected = 12.0 * (10000.0 / (1000.0 + 10000.0));
            Assert.That(result, Is.EqualTo(expected).Within(0.001));
        }

        [Test]
        public void Calculate_R2ZeroOhms_ReturnsZero()
        {
            // Arrange
            double vin = 9.0;
            double r1 = 500;
            double r2 = 0;
            
            // Act
            double result = VoltageDividerCalculator.Calculate(vin, r1, r2);
            
            // Assert
            Assert.That(result, Is.EqualTo(0).Within(0.001));
        }

        [Test]
        public void Calculate_R1ZeroOhms_ReturnsInputVoltage()
        {
            // Arrange
            double vin = 15.0;
            double r1 = 0;
            double r2 = 2000;
            
            // Act
            double result = VoltageDividerCalculator.Calculate(vin, r1, r2);
            
            // Assert
            Assert.That(result, Is.EqualTo(15.0).Within(0.001));
        }

        [Test]
        public void Calculate_ThreeToOneRatio_ReturnsCorrectVoltage()
        {
            // Arrange
            double vin = 20.0;
            double r1 = 3000;  // 3kΩ
            double r2 = 1000;  // 1kΩ
            
            // Act
            double result = VoltageDividerCalculator.Calculate(vin, r1, r2);
            
            // Assert
            double expected = 20.0 * (1000.0 / (3000.0 + 1000.0));
            Assert.That(result, Is.EqualTo(expected).Within(0.001));
        }
    }
}
