using NUnit.Framework;
using CircuitTool;

namespace CircuitTool.Tests
{
    [TestFixture]
    public class EnergyCalculatorTests
    {
        [Test]
        public void Joules_ReturnsCorrectValue()
        {
            Assert.That(EnergyCalculator.Joules(10, 10), Is.EqualTo(100.0));
        }

        [Test]
        public void KWh_ReturnsCorrectValue()
        {
            Assert.That(EnergyCalculator.KWh(1000, 2), Is.EqualTo(2.0));
        }

        [Test]
        public void EnergyCost_ReturnsCorrectValue()
        {
            Assert.That(EnergyCalculator.EnergyCost(5, 3), Is.EqualTo(15.0));
        }
    }
}
