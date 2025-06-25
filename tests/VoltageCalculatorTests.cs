using NUnit.Framework;
using CircuitTool;

namespace CircuitTool.Tests
{
    [TestFixture]
    public class VoltageCalculatorTests
    {
        [Test]
        public void VoltageDrop_ReturnsCorrectValue()
        {
            Assert.That(VoltageCalculator.VoltageDrop(10, 2), Is.EqualTo(20.0));
        }

        [Test]
        public void VoltageDivider_ReturnsCorrectValue()
        {
            Assert.That(VoltageCalculator.VoltageDivider(9, 2, 1), Is.EqualTo(3.0));
        }
    }
}
