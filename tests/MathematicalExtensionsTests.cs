using System;
using System.Numerics;
using NUnit.Framework;
using CircuitTool.Mathematics;

namespace CircuitTool.Tests
{
    [TestFixture]
    public class MathematicalExtensionsTests
    {
        [TestFixture]
        public class ComplexMatrixTests
        {
            [Test]
            public void Constructor_ValidDimensions_CreatesMatrix()
            {
                // Act
                var matrix = new MatrixOperations.ComplexMatrix(3, 4);
                
                // Assert
                Assert.That(matrix.Rows, Is.EqualTo(3));
                Assert.That(matrix.Columns, Is.EqualTo(4));
            }
            
            [Test]
            public void Constructor_InvalidDimensions_ThrowsException()
            {
                // Act & Assert
                Assert.Throws<ArgumentException>(() => new MatrixOperations.ComplexMatrix(0, 5));
                Assert.Throws<ArgumentException>(() => new MatrixOperations.ComplexMatrix(5, -1));
            }
            
            [Test]
            public void Identity_CreatesIdentityMatrix()
            {
                // Act
                var identity = MatrixOperations.ComplexMatrix.Identity(3);
                
                // Assert
                Assert.That(identity[0, 0], Is.EqualTo(Complex.One));
                Assert.That(identity[1, 1], Is.EqualTo(Complex.One));
                Assert.That(identity[2, 2], Is.EqualTo(Complex.One));
                Assert.That(identity[0, 1], Is.EqualTo(Complex.Zero));
                Assert.That(identity[1, 0], Is.EqualTo(Complex.Zero));
            }
            
            [Test]
            public void MatrixMultiplication_ValidMatrices_ReturnsCorrectResult()
            {
                // Arrange
                var a = new MatrixOperations.ComplexMatrix(2, 3);
                a[0, 0] = new Complex(1, 0);
                a[0, 1] = new Complex(2, 0);
                a[0, 2] = new Complex(3, 0);
                a[1, 0] = new Complex(4, 0);
                a[1, 1] = new Complex(5, 0);
                a[1, 2] = new Complex(6, 0);
                
                var b = new MatrixOperations.ComplexMatrix(3, 2);
                b[0, 0] = new Complex(1, 0);
                b[0, 1] = new Complex(2, 0);
                b[1, 0] = new Complex(3, 0);
                b[1, 1] = new Complex(4, 0);
                b[2, 0] = new Complex(5, 0);
                b[2, 1] = new Complex(6, 0);
                
                // Act
                var result = a * b;
                
                // Assert
                Assert.That(result.Rows, Is.EqualTo(2));
                Assert.That(result.Columns, Is.EqualTo(2));
                Assert.That(result[0, 0].Real, Is.EqualTo(22).Within(1e-10));
                Assert.That(result[0, 1].Real, Is.EqualTo(28).Within(1e-10));
                Assert.That(result[1, 0].Real, Is.EqualTo(49).Within(1e-10));
                Assert.That(result[1, 1].Real, Is.EqualTo(64).Within(1e-10));
            }
            
            [Test]
            public void MatrixAddition_ValidMatrices_ReturnsCorrectResult()
            {
                // Arrange
                var a = new MatrixOperations.ComplexMatrix(2, 2);
                a[0, 0] = new Complex(1, 2);
                a[0, 1] = new Complex(3, 4);
                a[1, 0] = new Complex(5, 6);
                a[1, 1] = new Complex(7, 8);
                
                var b = new MatrixOperations.ComplexMatrix(2, 2);
                b[0, 0] = new Complex(2, 1);
                b[0, 1] = new Complex(4, 3);
                b[1, 0] = new Complex(6, 5);
                b[1, 1] = new Complex(8, 7);
                
                // Act
                var result = a + b;
                
                // Assert
                Assert.That(result[0, 0], Is.EqualTo(new Complex(3, 3)));
                Assert.That(result[0, 1], Is.EqualTo(new Complex(7, 7)));
                Assert.That(result[1, 0], Is.EqualTo(new Complex(11, 11)));
                Assert.That(result[1, 1], Is.EqualTo(new Complex(15, 15)));
            }
            
            [Test]
            public void Determinant_2x2Matrix_ReturnsCorrectValue()
            {
                // Arrange
                var matrix = new MatrixOperations.ComplexMatrix(2, 2);
                matrix[0, 0] = new Complex(3, 0);
                matrix[0, 1] = new Complex(2, 0);
                matrix[1, 0] = new Complex(1, 0);
                matrix[1, 1] = new Complex(4, 0);
                
                // Act
                var det = matrix.Determinant();
                
                // Assert
                Assert.That(det.Real, Is.EqualTo(10).Within(1e-10));
                Assert.That(det.Imaginary, Is.EqualTo(0).Within(1e-10));
            }
            
            [Test]
            public void SolveLinearSystem_SimpleSystem_ReturnsCorrectSolution()
            {
                // Arrange - solve system: 2x + y = 5, x + 3y = 6
                var a = new MatrixOperations.ComplexMatrix(2, 2);
                a[0, 0] = new Complex(2, 0);
                a[0, 1] = new Complex(1, 0);
                a[1, 0] = new Complex(1, 0);
                a[1, 1] = new Complex(3, 0);
                
                var b = new Complex[] { new Complex(5, 0), new Complex(6, 0) };
                
                // Act
                var solution = MatrixOperations.ComplexMatrix.SolveLinearSystem(a, b);
                
                // Assert
                Assert.That(solution[0].Real, Is.EqualTo(1.8).Within(1e-10));
                Assert.That(solution[1].Real, Is.EqualTo(1.4).Within(1e-10));
            }
        }
        
        [TestFixture]
        public class NodalAnalysisTests
        {
            [Test]
            public void CreateNodalAdmittanceMatrix_SimpleCircuit_ReturnsCorrectMatrix()
            {
                // Arrange - Simple 3-node circuit with 2 resistors
                var impedances = new Complex[] 
                { 
                    new Complex(10, 0), // 10Ω resistor from node 0 to 1
                    new Complex(20, 0)  // 20Ω resistor from node 1 to 2
                };
                var connections = new (int from, int to)[] { (0, 1), (1, 2) };
                
                // Act
                var matrix = MatrixOperations.CreateNodalAdmittanceMatrix(impedances, connections, 3);
                
                // Assert
                Assert.That(matrix[0, 0].Real, Is.EqualTo(0.1).Within(1e-10)); // 1/10
                Assert.That(matrix[1, 1].Real, Is.EqualTo(0.15).Within(1e-10)); // 1/10 + 1/20
                Assert.That(matrix[2, 2].Real, Is.EqualTo(0.05).Within(1e-10)); // 1/20
                Assert.That(matrix[0, 1].Real, Is.EqualTo(-0.1).Within(1e-10)); // -1/10
                Assert.That(matrix[1, 2].Real, Is.EqualTo(-0.05).Within(1e-10)); // -1/20
            }
        }
        
        [TestFixture]
        public class FourierTransformTests
        {
            [Test]
            public void DFT_DCSignal_ReturnsCorrectSpectrum()
            {
                // Arrange - DC signal of magnitude 2
                var signal = new Complex[] 
                { 
                    new Complex(2, 0), 
                    new Complex(2, 0), 
                    new Complex(2, 0), 
                    new Complex(2, 0) 
                };
                
                // Act
                var spectrum = FourierTransform.DFT(signal);
                
                // Assert
                Assert.That(spectrum[0].Real, Is.EqualTo(8).Within(1e-10)); // DC component
                Assert.That(spectrum[1].Magnitude, Is.EqualTo(0).Within(1e-10)); // No AC components
                Assert.That(spectrum[2].Magnitude, Is.EqualTo(0).Within(1e-10));
                Assert.That(spectrum[3].Magnitude, Is.EqualTo(0).Within(1e-10));
            }
            
            [Test]
            public void DFT_RealValuedSignal_ReturnsCorrectSpectrum()
            {
                // Arrange
                var signal = new double[] { 1, 0, -1, 0 };
                
                // Act
                var spectrum = FourierTransform.DFT(signal);
                
                // Assert
                Assert.That(spectrum[0].Magnitude, Is.EqualTo(0).Within(1e-10)); // DC = 0
                Assert.That(spectrum[1].Magnitude, Is.GreaterThan(0)); // Fundamental component
            }
            
            [Test]
            public void IDFT_InverseOfDFT_ReturnsOriginalSignal()
            {
                // Arrange
                var original = new Complex[] 
                { 
                    new Complex(1, 0), 
                    new Complex(2, 0.5), 
                    new Complex(-1, 0), 
                    new Complex(0.5, -0.5) 
                };
                
                // Act
                var spectrum = FourierTransform.DFT(original);
                var reconstructed = FourierTransform.IDFT(spectrum);
                
                // Assert
                for (int i = 0; i < original.Length; i++)
                {
                    Assert.That(reconstructed[i].Real, Is.EqualTo(original[i].Real).Within(1e-10));
                    Assert.That(reconstructed[i].Imaginary, Is.EqualTo(original[i].Imaginary).Within(1e-10));
                }
            }
            
            [Test]
            public void PowerSpectralDensity_SineWave_ShowsPeakAtCorrectFrequency()
            {
                // Arrange - Create a sine wave at bin 1
                var signal = new double[8];
                for (int i = 0; i < 8; i++)
                {
                    signal[i] = Math.Sin(2 * Math.PI * i / 8);
                }
                
                // Act
                var psd = FourierTransform.PowerSpectralDensity(signal);
                
                // Assert
                Assert.That(psd[1], Is.GreaterThan(psd[0])); // Peak at frequency bin 1
                Assert.That(psd[1], Is.GreaterThan(psd[2]));
            }
            
            [Test]
            public void ExtractHarmonics_SineWaveWithHarmonics_ReturnsCorrectContent()
            {
                // Arrange - Fundamental + 3rd harmonic
                var signal = new double[64];
                double fundamentalFreq = 50; // 50 Hz
                double sampleRate = 3200; // 3.2 kHz
                
                for (int i = 0; i < signal.Length; i++)
                {
                    double t = i / sampleRate;
                    signal[i] = Math.Sin(2 * Math.PI * fundamentalFreq * t) + 
                               0.3 * Math.Sin(2 * Math.PI * 3 * fundamentalFreq * t);
                }
                
                // Act
                var (magnitudes, phases) = FourierTransform.ExtractHarmonics(signal, fundamentalFreq, sampleRate, 5);
                
                // Assert
                Assert.That(magnitudes[1], Is.GreaterThan(0.5)); // Fundamental
                Assert.That(magnitudes[3], Is.GreaterThan(0.1)); // 3rd harmonic
                Assert.That(magnitudes[2], Is.LessThan(0.1)); // No 2nd harmonic
            }
            
            [Test]
            public void CalculateTHD_HarmonicContent_ReturnsCorrectTHD()
            {
                // Arrange
                var harmonics = new double[] { 0, 1.0, 0.2, 0.1, 0.05 }; // Fundamental = 1, harmonics follow
                
                // Act
                var thd = FourierTransform.CalculateTHD(harmonics);
                
                // Assert
                double expectedTHD = Math.Sqrt(0.2*0.2 + 0.1*0.1 + 0.05*0.05) / 1.0;
                Assert.That(thd, Is.EqualTo(expectedTHD).Within(1e-10));
            }
            
            [Test]
            public void ApplyWindow_HanningWindow_ModifiesSignalCorrectly()
            {
                // Arrange
                var signal = new double[] { 1, 1, 1, 1, 1, 1, 1, 1 };
                
                // Act
                var windowed = FourierTransform.ApplyWindow(signal, WindowType.Hanning);
                
                // Assert
                Assert.That(windowed[0], Is.EqualTo(0).Within(1e-10)); // Window starts at 0
                Assert.That(windowed[4], Is.GreaterThan(windowed[0])); // Peak in middle
                Assert.That(windowed[7], Is.EqualTo(0).Within(1e-10)); // Window ends at 0
            }
        }
    }
}
