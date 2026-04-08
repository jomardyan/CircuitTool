using System;
using System.Numerics;
using NUnit.Framework;
using CircuitTool.Mathematics;

namespace CircuitTool.Tests.Mathematics
{
    [TestFixture]
    public class FourierTransformTests
    {
        private const double Tolerance = 1e-10;

        [Test]
        public void DFT_SimpleSignal_ReturnsCorrectSpectrum()
        {
            // Arrange - Simple impulse signal
            var signal = new Complex[] { Complex.One, Complex.Zero, Complex.Zero, Complex.Zero };
            
            // Act
            var spectrum = FourierTransform.DFT(signal);
            
            // Assert - DFT of impulse should be all ones
            Assert.That(spectrum.Length, Is.EqualTo(4));
            for (int i = 0; i < spectrum.Length; i++)
            {
                Assert.That(spectrum[i].Real, Is.EqualTo(1.0).Within(Tolerance));
                Assert.That(spectrum[i].Imaginary, Is.EqualTo(0.0).Within(Tolerance));
            }
        }

        [Test]
        public void DFT_RealValuedSignal_ReturnsCorrectSpectrum()
        {
            // Arrange - Simple real signal
            var signal = new double[] { 1.0, 0.0, 0.0, 0.0 };
            
            // Act
            var spectrum = FourierTransform.DFT(signal);
            
            // Assert
            Assert.That(spectrum.Length, Is.EqualTo(4));
            for (int i = 0; i < spectrum.Length; i++)
            {
                Assert.That(spectrum[i].Real, Is.EqualTo(1.0).Within(Tolerance));
                Assert.That(spectrum[i].Imaginary, Is.EqualTo(0.0).Within(Tolerance));
            }
        }

        [Test]
        public void IDFT_ReturnsOriginalSignal()
        {
            // Arrange - Original signal
            var original = new Complex[] 
            { 
                new Complex(1.0, 0.5), 
                new Complex(2.0, -0.3), 
                new Complex(0.5, 1.2), 
                new Complex(-0.8, 0.7) 
            };
            
            // Act - Forward then inverse transform
            var spectrum = FourierTransform.DFT(original);
            var reconstructed = FourierTransform.IDFT(spectrum);
            
            // Assert - Should get back original signal
            Assert.That(reconstructed.Length, Is.EqualTo(original.Length));
            for (int i = 0; i < original.Length; i++)
            {
                Assert.That(reconstructed[i].Real, Is.EqualTo(original[i].Real).Within(Tolerance));
                Assert.That(reconstructed[i].Imaginary, Is.EqualTo(original[i].Imaginary).Within(Tolerance));
            }
        }

        [Test]
        public void DFT_SineWave_ReturnsCorrectFrequencyContent()
        {
            // Arrange - Create a sine wave at specific frequency
            int N = 64;
            int frequency = 8; // Frequency bin
            var signal = new Complex[N];
            
            for (int n = 0; n < N; n++)
            {
                double angle = 2.0 * Math.PI * frequency * n / N;
                signal[n] = new Complex(Math.Sin(angle), 0);
            }
            
            // Act
            var spectrum = FourierTransform.DFT(signal);
            
            // Assert - Peak should be at the specified frequency
            double maxMagnitude = 0;
            int maxIndex = 0;
            for (int i = 0; i < spectrum.Length; i++)
            {
                if (spectrum[i].Magnitude > maxMagnitude)
                {
                    maxMagnitude = spectrum[i].Magnitude;
                    maxIndex = i;
                }
            }
            
            // Peak should be at frequency bin or its negative frequency counterpart
            Assert.That(maxIndex == frequency || maxIndex == N - frequency, Is.True, 
                $"Expected peak at frequency {frequency} or {N - frequency}, but found at {maxIndex}");
        }

        [Test]
        public void PowerSpectralDensity_ReturnsPositiveValues()
        {
            // Arrange
            var signal = new double[] { 1.0, 2.0, 1.5, 0.5, -0.5, 1.0, 0.8, 1.2 };
            
            // Act
            var psd = FourierTransform.PowerSpectralDensity(signal);
            
            // Assert
            Assert.That(psd.Length, Is.EqualTo(signal.Length));
            for (int i = 0; i < psd.Length; i++)
            {
                Assert.That(psd[i], Is.GreaterThanOrEqualTo(0.0), $"PSD value at index {i} should be non-negative");
            }
        }

        [Test]
        public void ApplyWindow_HanningWindow_ReturnsCorrectLength()
        {
            // Arrange
            var signal = new double[] { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0 };
            
            // Act
            var windowed = FourierTransform.ApplyWindow(signal, WindowType.Hanning);
            
            // Assert
            Assert.That(windowed.Length, Is.EqualTo(signal.Length));
            
            // Hanning window should reduce values at edges
            Assert.That(windowed[0], Is.LessThan(signal[0]));
            Assert.That(windowed[windowed.Length - 1], Is.LessThan(signal[signal.Length - 1]));
        }

        [Test]
        public void ApplyWindow_BlackmanWindow_ReducesEdgeValues()
        {
            // Arrange
            var signal = new double[] { 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0 };
            
            // Act
            var windowed = FourierTransform.ApplyWindow(signal, WindowType.Blackman);
            
            // Assert
            Assert.That(windowed.Length, Is.EqualTo(signal.Length));
            
            // Blackman window should have near-zero values at edges
            Assert.That(windowed[0], Is.LessThan(0.1));
            Assert.That(windowed[windowed.Length - 1], Is.LessThan(0.1));
        }

        [Test]
        public void DFT_EmptyArray_ThrowsArgumentException()
        {
            // Arrange
            var emptySignal = new Complex[0];
            
            // Act & Assert
            Assert.Throws<ArgumentException>(() => FourierTransform.DFT(emptySignal));
        }

        [Test]
        public void DFT_NullArray_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => FourierTransform.DFT((Complex[])null));
            Assert.Throws<ArgumentNullException>(() => FourierTransform.DFT((double[])null));
        }

        [Test]
        public void PowerSpectralDensity_HasCorrectPowerConservation()
        {
            // Arrange - White noise-like signal
            var signal = new double[] { 1.0, -0.5, 0.8, -0.3, 0.6, -0.7, 0.4, -0.9 };
            
            // Act
            var psd = FourierTransform.PowerSpectralDensity(signal);
            
            // Assert - Total power should be conserved (Parseval's theorem)
            double signalPower = 0;
            for (int i = 0; i < signal.Length; i++)
            {
                signalPower += signal[i] * signal[i];
            }
            
            double spectralPower = 0;
            for (int i = 0; i < psd.Length; i++)
            {
                spectralPower += psd[i];
            }
            
            Assert.That(spectralPower, Is.EqualTo(signalPower).Within(Tolerance * signal.Length));
        }

        [Test]
        public void DFT_Linearity_SatisfiesLinearityProperty()
        {
            // Arrange
            var signal1 = new Complex[] { Complex.One, new Complex(0, 1), Complex.Zero, new Complex(-1, 0) };
            var signal2 = new Complex[] { new Complex(2, 0), Complex.Zero, new Complex(0, -1), Complex.One };
            double a = 2.0, b = 3.0;
            
            var combinedSignal = new Complex[signal1.Length];
            for (int i = 0; i < signal1.Length; i++)
            {
                combinedSignal[i] = a * signal1[i] + b * signal2[i];
            }
            
            // Act
            var spectrum1 = FourierTransform.DFT(signal1);
            var spectrum2 = FourierTransform.DFT(signal2);
            var combinedSpectrum = FourierTransform.DFT(combinedSignal);
            
            // Assert - DFT should satisfy linearity: DFT(a*x + b*y) = a*DFT(x) + b*DFT(y)
            for (int i = 0; i < spectrum1.Length; i++)
            {
                var expectedSpectrum = a * spectrum1[i] + b * spectrum2[i];
                Assert.That(combinedSpectrum[i].Real, Is.EqualTo(expectedSpectrum.Real).Within(Tolerance));
                Assert.That(combinedSpectrum[i].Imaginary, Is.EqualTo(expectedSpectrum.Imaginary).Within(Tolerance));
            }
        }
    }
}
