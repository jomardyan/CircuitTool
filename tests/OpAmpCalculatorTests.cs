using NUnit.Framework;
using CircuitTool;
using System;

namespace CircuitTool.Tests
{
    [TestFixture]
    public class OpAmpCalculatorTests
    {
        [Test]
        public void NonInvertingGain_ValidResistors_ReturnsCorrectValue()
        {
            // Arrange
            double feedbackResistor = 10000; // 10kΩ
            double inputResistor = 1000;     // 1kΩ
            double expected = 11.0;

            // Act
            double result = OpAmpCalculator.NonInvertingGain(feedbackResistor, inputResistor);

            // Assert
            Assert.That(result, Is.EqualTo(expected).Within(0.001));
        }

        [Test]
        public void InvertingGain_ValidResistors_ReturnsCorrectValue()
        {
            // Arrange
            double feedbackResistor = 10000; // 10kΩ
            double inputResistor = 1000;     // 1kΩ
            double expected = 10.0;

            // Act
            double result = OpAmpCalculator.InvertingGain(feedbackResistor, inputResistor);

            // Assert
            Assert.That(result, Is.EqualTo(expected).Within(0.001));
        }

        [Test]
        public void ClosedLoopBandwidth_ValidParameters_ReturnsCorrectValue()
        {
            // Arrange
            double gbp = 1e6; // 1 MHz
            double gain = 100;
            double expected = 10000; // 10 kHz

            // Act
            double result = OpAmpCalculator.ClosedLoopBandwidth(gbp, gain);

            // Assert
            Assert.That(result, Is.EqualTo(expected).Within(0.001));
        }

        [Test]
        public void SlewRateLimit_ValidParameters_ReturnsCorrectValue()
        {
            // Arrange
            double slewRate = 1e6; // 1 V/μs
            double amplitude = 5.0;
            double expected = slewRate / (2 * Math.PI * amplitude);

            // Act
            double result = OpAmpCalculator.SlewRateLimit(slewRate, amplitude);

            // Assert
            Assert.That(result, Is.EqualTo(expected).Within(0.001));
        }

        [Test]
        public void InstrumentationAmplifierGain_ValidParameters_ReturnsCorrectValue()
        {
            // Arrange
            double gainResistor = 1000; // 1kΩ
            double internalResistor = 50000; // 50kΩ
            double expected = 101.0;

            // Act
            double result = OpAmpCalculator.InstrumentationAmplifierGain(gainResistor, internalResistor);

            // Assert
            Assert.That(result, Is.EqualTo(expected).Within(0.001));
        }

        [Test]
        public void NonInvertingGain_ZeroInputResistor_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => OpAmpCalculator.NonInvertingGain(10000, 0));
        }

        [Test]
        public void ClosedLoopBandwidth_ZeroGain_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => OpAmpCalculator.ClosedLoopBandwidth(1e6, 0));
        }
    }
}
