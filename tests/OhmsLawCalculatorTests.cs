using System;
using NUnit.Framework;
using CircuitTool;

namespace CircuitTool.Tests
{
    [TestFixture]
    public class OhmsLawCalculatorTests
    {
        [Test]
        public void Voltage_ReturnsCorrectValue()
        {
            Assert.That(OhmsLawCalculator.Voltage(2, 5), Is.EqualTo(10.0));
        }

        [Test]
        public void Current_ReturnsCorrectValue()
        {
            Assert.That(OhmsLawCalculator.Current(10, 5), Is.EqualTo(2.0));
        }

        [Test]
        public void Resistance_ReturnsCorrectValue()
        {
            Assert.That(OhmsLawCalculator.Resistance(10, 2), Is.EqualTo(5.0));
        }

        [Test]
        public void Voltage_NegativeResistance_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => OhmsLawCalculator.Voltage(2, -5));
        }

        [Test]
        public void Current_NonPositiveResistance_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => OhmsLawCalculator.Current(10, 0));
            Assert.Throws<ArgumentException>(() => OhmsLawCalculator.Current(10, -5));
        }

        [Test]
        public void Resistance_ZeroCurrent_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => OhmsLawCalculator.Resistance(10, 0));
        }
    }
}
