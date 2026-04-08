using NUnit.Framework;
using CircuitTool.Units;
using System;

namespace CircuitTool.Tests.Units
{
    [TestFixture]
    public class TypedOhmsLawCalculatorTests
    {
        [Test]
        public void CalculateVoltage_ValidInputs_ReturnsCorrectVoltage()
        {
            // Arrange
            var current = new Current(2, CurrentUnit.Amperes);
            var resistance = new Resistance(5, ResistanceUnit.Ohms);
            
            // Act
            var voltage = TypedOhmsLawCalculator.CalculateVoltage(current, resistance);
            
            // Assert
            Assert.That(voltage.Volts, Is.EqualTo(10).Within(0.001));
        }

        [Test]
        public void CalculateCurrent_ValidInputs_ReturnsCorrectCurrent()
        {
            // Arrange
            var voltage = new Voltage(10, VoltageUnit.Volts);
            var resistance = new Resistance(5, ResistanceUnit.Ohms);
            
            // Act
            var current = TypedOhmsLawCalculator.CalculateCurrent(voltage, resistance);
            
            // Assert
            Assert.That(current.Amperes, Is.EqualTo(2).Within(0.001));
        }

        [Test]
        public void CalculateResistance_ValidInputs_ReturnsCorrectResistance()
        {
            // Arrange
            var voltage = new Voltage(10, VoltageUnit.Volts);
            var current = new Current(2, CurrentUnit.Amperes);
            
            // Act
            var resistance = TypedOhmsLawCalculator.CalculateResistance(voltage, current);
            
            // Assert
            Assert.That(resistance.Ohms, Is.EqualTo(5).Within(0.001));
        }

        [Test]
        public void CalculatePower_VoltageAndCurrent_ReturnsCorrectPower()
        {
            // Arrange
            var voltage = new Voltage(10, VoltageUnit.Volts);
            var current = new Current(2, CurrentUnit.Amperes);
            
            // Act
            var power = TypedOhmsLawCalculator.CalculatePower(voltage, current);
            
            // Assert
            Assert.That(power, Is.EqualTo(20).Within(0.001));
        }

        [Test]
        public void CalculatePower_VoltageAndResistance_ReturnsCorrectPower()
        {
            // Arrange
            var voltage = new Voltage(10, VoltageUnit.Volts);
            var resistance = new Resistance(5, ResistanceUnit.Ohms);
            
            // Act
            var power = TypedOhmsLawCalculator.CalculatePower(voltage, resistance);
            
            // Assert
            Assert.That(power, Is.EqualTo(20).Within(0.001));
        }

        [Test]
        public void CalculatePower_CurrentAndResistance_ReturnsCorrectPower()
        {
            // Arrange
            var current = new Current(2, CurrentUnit.Amperes);
            var resistance = new Resistance(5, ResistanceUnit.Ohms);
            
            // Act
            var power = TypedOhmsLawCalculator.CalculatePower(current, resistance);
            
            // Assert
            Assert.That(power, Is.EqualTo(20).Within(0.001));
        }

        [Test]
        public void CalculateCurrent_ZeroResistance_ThrowsException()
        {
            // Arrange
            var voltage = new Voltage(10, VoltageUnit.Volts);
            var resistance = new Resistance(0, ResistanceUnit.Ohms);
            
            // Act & Assert
            Assert.Throws<ArgumentException>(() => TypedOhmsLawCalculator.CalculateCurrent(voltage, resistance));
        }

        [Test]
        public void CalculateResistance_ZeroCurrent_ThrowsException()
        {
            // Arrange
            var voltage = new Voltage(10, VoltageUnit.Volts);
            var current = new Current(0, CurrentUnit.Amperes);
            
            // Act & Assert
            Assert.Throws<ArgumentException>(() => TypedOhmsLawCalculator.CalculateResistance(voltage, current));
        }

        [Test]
        public void CalculateVoltage_WithDifferentUnits_ReturnsCorrectResult()
        {
            // Arrange
            var current = new Current(500, CurrentUnit.Milliamperes); // 0.5A
            var resistance = new Resistance(2, ResistanceUnit.Kiloohms); // 2000Ω
            
            // Act
            var voltage = TypedOhmsLawCalculator.CalculateVoltage(current, resistance);
            
            // Assert
            Assert.That(voltage.Volts, Is.EqualTo(1000).Within(0.001)); // 0.5A × 2000Ω = 1000V
            Assert.That(voltage.Kilovolts, Is.EqualTo(1).Within(0.001));
        }

        [Test]
        public void CalculateCurrent_WithMixedUnits_ReturnsCorrectResult()
        {
            // Arrange
            var voltage = new Voltage(12, VoltageUnit.Volts);
            var resistance = new Resistance(2.2, ResistanceUnit.Kiloohms); // 2200Ω
            
            // Act
            var current = TypedOhmsLawCalculator.CalculateCurrent(voltage, resistance);
            
            // Assert
            Assert.That(current.Milliamperes, Is.EqualTo(5.454545).Within(0.001)); // 12V ÷ 2200Ω ≈ 5.45mA
        }
    }
}
