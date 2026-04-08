using NUnit.Framework;
using CircuitTool.Units;
using System;

namespace CircuitTool.Tests.Units
{
    [TestFixture]
    public class VoltageTests
    {
        [Test]
        public void Constructor_ValidValue_CreatesCorrectVoltage()
        {
            // Arrange & Act
            var voltage = new Voltage(5, VoltageUnit.Volts);
            
            // Assert
            Assert.That(voltage.Volts, Is.EqualTo(5).Within(0.001));
        }

        [Test]
        public void Constructor_Kilovolts_ConvertsCorrectly()
        {
            // Arrange & Act
            var voltage = new Voltage(1, VoltageUnit.Kilovolts);
            
            // Assert
            Assert.That(voltage.Volts, Is.EqualTo(1000).Within(0.001));
            Assert.That(voltage.Kilovolts, Is.EqualTo(1).Within(0.001));
        }

        [Test]
        public void Constructor_Millivolts_ConvertsCorrectly()
        {
            // Arrange & Act
            var voltage = new Voltage(1500, VoltageUnit.Millivolts);
            
            // Assert
            Assert.That(voltage.Volts, Is.EqualTo(1.5).Within(0.001));
            Assert.That(voltage.Millivolts, Is.EqualTo(1500).Within(0.001));
        }

        [Test]
        public void GetValue_DifferentUnits_ReturnsCorrectValues()
        {
            // Arrange
            var voltage = new Voltage(5, VoltageUnit.Volts);
            
            // Act & Assert
            Assert.That(voltage.GetValue(VoltageUnit.Volts), Is.EqualTo(5).Within(0.001));
            Assert.That(voltage.GetValue(VoltageUnit.Millivolts), Is.EqualTo(5000).Within(0.001));
            Assert.That(voltage.GetValue(VoltageUnit.Kilovolts), Is.EqualTo(0.005).Within(0.000001));
        }

        [Test]
        public void Addition_TwoVoltages_ReturnsCorrectSum()
        {
            // Arrange
            var v1 = new Voltage(5, VoltageUnit.Volts);
            var v2 = new Voltage(2000, VoltageUnit.Millivolts);
            
            // Act
            var result = v1 + v2;
            
            // Assert
            Assert.That(result.Volts, Is.EqualTo(7).Within(0.001));
        }

        [Test]
        public void Subtraction_TwoVoltages_ReturnsCorrectDifference()
        {
            // Arrange
            var v1 = new Voltage(5, VoltageUnit.Volts);
            var v2 = new Voltage(2, VoltageUnit.Volts);
            
            // Act
            var result = v1 - v2;
            
            // Assert
            Assert.That(result.Volts, Is.EqualTo(3).Within(0.001));
        }

        [Test]
        public void Multiplication_VoltageByScalar_ReturnsCorrectResult()
        {
            // Arrange
            var voltage = new Voltage(5, VoltageUnit.Volts);
            
            // Act
            var result = voltage * 2;
            
            // Assert
            Assert.That(result.Volts, Is.EqualTo(10).Within(0.001));
        }

        [Test]
        public void Division_VoltageByScalar_ReturnsCorrectResult()
        {
            // Arrange
            var voltage = new Voltage(10, VoltageUnit.Volts);
            
            // Act
            var result = voltage / 2;
            
            // Assert
            Assert.That(result.Volts, Is.EqualTo(5).Within(0.001));
        }

        [Test]
        public void Comparison_EqualVoltages_ReturnsTrue()
        {
            // Arrange
            var v1 = new Voltage(5, VoltageUnit.Volts);
            var v2 = new Voltage(5000, VoltageUnit.Millivolts);
            
            // Act & Assert
            Assert.That(v1 == v2, Is.True);
            Assert.That(v1.Equals(v2), Is.True);
        }

        [Test]
        public void Comparison_DifferentVoltages_WorksCorrectly()
        {
            // Arrange
            var v1 = new Voltage(5, VoltageUnit.Volts);
            var v2 = new Voltage(3, VoltageUnit.Volts);
            
            // Act & Assert
            Assert.That(v1 > v2, Is.True);
            Assert.That(v2 < v1, Is.True);
            Assert.That(v1 != v2, Is.True);
        }

        [Test]
        public void ToString_DefaultFormat_ReturnsCorrectString()
        {
            // Arrange
            var voltage = new Voltage(5.123, VoltageUnit.Volts);
            
            // Act
            var result = voltage.ToString();
            
            // Assert
            Assert.That(result, Is.EqualTo("5.123 V"));
        }

        [Test]
        public void ToString_SpecificUnit_ReturnsCorrectString()
        {
            // Arrange
            var voltage = new Voltage(5, VoltageUnit.Volts);
            
            // Act
            var result = voltage.ToString(VoltageUnit.Millivolts);
            
            // Assert
            Assert.That(result, Is.EqualTo("5000.000 mV"));
        }

        [Test]
        public void ImplicitConversion_FromDouble_WorksCorrectly()
        {
            // Arrange & Act
            Voltage voltage = 5.0; // Implicit conversion from double
            
            // Assert
            Assert.That(voltage.Volts, Is.EqualTo(5.0).Within(0.001));
        }

        [Test]
        public void ImplicitConversion_ToDouble_WorksCorrectly()
        {
            // Arrange
            var voltage = new Voltage(5, VoltageUnit.Volts);
            
            // Act
            double value = voltage; // Implicit conversion to double
            
            // Assert
            Assert.That(value, Is.EqualTo(5.0).Within(0.001));
        }
    }
}
