using NUnit.Framework;
using CircuitTool;

namespace CircuitTool.Tests
{
    [TestFixture]
    public class ResistorCalculatorTests
    {
        [Test]
        public void Resistance_ReturnsCorrectValue()
        {
            Assert.That(ResistorCalculator.Resistance(10, 2), Is.EqualTo(5.0));
        }

        [Test]
        public void Series_ReturnsSumOfResistors()
        {
            Assert.That(ResistorCalculator.Series(5, 3, 7), Is.EqualTo(15.0));
        }

        [Test]
        public void Series_NullInput_ReturnsZero()
        {
            Assert.That(ResistorCalculator.Series(null), Is.EqualTo(0.0));
        }

        [Test]
        public void Parallel_ReturnsCorrectValue()
        {
            // 1/(1/2 + 1/3) = 1.2
            Assert.That(ResistorCalculator.Parallel(2, 3), Is.EqualTo(1.2).Within(1e-6));
        }

        [Test]
        public void Parallel_NullInput_ReturnsZero()
        {
            Assert.That(ResistorCalculator.Parallel(null), Is.EqualTo(0.0));
        }
    }
}
