using NUnit.Framework;
using CircuitTool;

namespace CircuitTool.Tests
{
    [TestFixture]
    public class VoltageDropCalculatorTests
    {
        [Test]
        public void Calculate_ValidInputs_ReturnsCorrectVoltageDrop()
        {
            // Arrange
            double current = 2.5;
            double resistance = 4.0;
            
            // Act
            double result = VoltageDropCalculator.Calculate(current, resistance);
            
            // Assert
            Assert.That(result, Is.EqualTo(10.0).Within(0.001));
        }

        [Test]
        public void Calculate_ZeroCurrent_ReturnsZero()
        {
            // Arrange
            double current = 0;
            double resistance = 5.0;
            
            // Act
            double result = VoltageDropCalculator.Calculate(current, resistance);
            
            // Assert
            Assert.That(result, Is.EqualTo(0).Within(0.001));
        }

        [Test]
        public void Calculate_ZeroResistance_ReturnsZero()
        {
            // Arrange
            double current = 3.0;
            double resistance = 0;
            
            // Act
            double result = VoltageDropCalculator.Calculate(current, resistance);
            
            // Assert
            Assert.That(result, Is.EqualTo(0).Within(0.001));
        }

        [Test]
        public void Calculate_SmallValues_ReturnsCorrectResult()
        {
            // Arrange
            double current = 0.001; // 1mA
            double resistance = 1000; // 1kΩ
            
            // Act
            double result = VoltageDropCalculator.Calculate(current, resistance);
            
            // Assert
            Assert.That(result, Is.EqualTo(1.0).Within(0.001));
        }
    }
}
