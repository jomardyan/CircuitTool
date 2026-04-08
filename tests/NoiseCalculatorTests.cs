using NUnit.Framework;
using CircuitTool;
using System;

namespace CircuitTool.Tests
{
    [TestFixture]
    public class NoiseCalculatorTests
    {
        [Test]
        public void ThermalNoise_ValidParameters_ReturnsCorrectValue()
        {
            // Arrange
            double resistance = 1000; // 1kΩ
            double bandwidth = 10000; // 10kHz
            double temperature = 300; // K

            // Act
            double result = NoiseCalculator.ThermalNoise(resistance, bandwidth, temperature);

            // Assert
            Assert.IsTrue(result > 0);
            Assert.IsTrue(result < 1e-6); // Should be in microvolts range
        }

        [Test]
        public void ShotNoise_ValidParameters_ReturnsCorrectValue()
        {
            // Arrange
            double current = 1e-3; // 1mA
            double bandwidth = 1000; // 1kHz

            // Act
            double result = NoiseCalculator.ShotNoise(current, bandwidth);

            // Assert
            Assert.IsTrue(result > 0);
            Assert.IsTrue(result < 1e-9); // Should be in nanoamps range
        }

        [Test]
        public void TotalNoise_MultipleNoiseSourcesUncorrelated_ReturnsCorrectValue()
        {
            // Arrange
            double[] noiseSources = { 1e-9, 2e-9, 1.5e-9 };
            double expected = Math.Sqrt(1e-18 + 4e-18 + 2.25e-18);

            // Act
            double result = NoiseCalculator.TotalNoise(noiseSources);

            // Assert
            Assert.That(result, Is.EqualTo(expected).Within(1e-20));
        }

        [Test]
        public void SignalToNoiseRatio_ValidParameters_ReturnsCorrectValue()
        {
            // Arrange
            double signalPower = 100;
            double noisePower = 10;
            double expected = 10; // 10 dB

            // Act
            double result = NoiseCalculator.SignalToNoiseRatio(signalPower, noisePower);

            // Assert
            Assert.That(result, Is.EqualTo(expected).Within(0.001));
        }

        [Test]
        public void ThermalNoise_NegativeResistance_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => NoiseCalculator.ThermalNoise(-100, 1000, 300));
        }

        [Test]
        public void ShotNoise_NegativeCurrent_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => NoiseCalculator.ShotNoise(-1e-3, 1000));
        }
    }
}
