using System;
using System.Numerics;
using NUnit.Framework;
using CircuitTool.Performance;

namespace CircuitTool.Tests.Performance
{
    [TestFixture]
    public class VectorizedCalculationsTests
    {
        private const double Tolerance = 1e-10;

        [Test]
        public void ParallelResistanceVectorized_SimpleCase_ReturnsCorrectResult()
        {
            // Arrange
            var resistances = new double[] { 100.0, 200.0 };
            
            // Act
            var result = VectorizedCalculations.ParallelResistanceVectorized(resistances);
            
            // Assert
            // 1/R_total = 1/100 + 1/200 = 0.01 + 0.005 = 0.015, so R_total = 1/0.015 = 66.67
            var expected = 1.0 / (1.0/100.0 + 1.0/200.0);
            Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
        }

        [Test]
        public void ParallelResistanceVectorized_LargeArray_ReturnsCorrectResult()
        {
            // Arrange - Create array larger than SIMD vector length
            var resistances = new double[16]; // Should be larger than typical SIMD vector
            for (int i = 0; i < resistances.Length; i++)
            {
                resistances[i] = 100.0 * (i + 1); // 100, 200, 300, etc.
            }
            
            // Act
            var result = VectorizedCalculations.ParallelResistanceVectorized(resistances);
            
            // Assert - Calculate expected value manually
            double reciprocalSum = 0.0;
            foreach (var r in resistances)
            {
                reciprocalSum += 1.0 / r;
            }
            var expected = 1.0 / reciprocalSum;
            
            Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
        }

        [Test]
        public void ParallelResistanceVectorized_EmptyArray_ThrowsException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                VectorizedCalculations.ParallelResistanceVectorized(new double[0]));
        }

        [Test]
        public void ParallelResistanceVectorized_NullArray_ThrowsException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                VectorizedCalculations.ParallelResistanceVectorized(null));
        }

        [Test]
        public void ParallelResistanceVectorized_SingleElement_ReturnsOriginalValue()
        {
            // Arrange
            var resistances = new double[] { 150.0 };
            
            // Act
            var result = VectorizedCalculations.ParallelResistanceVectorized(resistances);
            
            // Assert
            Assert.That(result, Is.EqualTo(150.0).Within(Tolerance));
        }

        [Test]
        public void RMSVectorized_MultipleSignals_ReturnsCorrectResults()
        {
            // Arrange
            var signals = new double[][]
            {
                new double[] { 1.0, -1.0, 1.0, -1.0 }, // Square wave
                new double[] { 0.0, 1.0, 0.0, -1.0 },  // Another pattern
                new double[] { 2.0, 2.0, 2.0, 2.0 }    // DC signal
            };
            
            // Act
            var results = VectorizedCalculations.RMSVectorized(signals);
            
            // Assert
            Assert.That(results.Length, Is.EqualTo(3));
            
            // Square wave RMS = 1.0
            Assert.That(results[0], Is.EqualTo(1.0).Within(Tolerance));
            
            // Second signal RMS = sqrt((0^2 + 1^2 + 0^2 + 1^2)/4) = sqrt(0.5) = 0.707...
            var expectedRms2 = Math.Sqrt(0.5);
            Assert.That(results[1], Is.EqualTo(expectedRms2).Within(Tolerance));
            
            // DC signal RMS = 2.0
            Assert.That(results[2], Is.EqualTo(2.0).Within(Tolerance));
        }

        [Test]
        public void RMSVectorized_EmptySignalsArray_ThrowsException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                VectorizedCalculations.RMSVectorized(new double[0][]));
        }

        [Test]
        public void RMSVectorized_NullSignalsArray_ThrowsException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                VectorizedCalculations.RMSVectorized(null));
        }

        [Test]
        public void RMSVectorized_SignalWithNullElements_HandlesGracefully()
        {
            // Arrange
            var signals = new double[][]
            {
                new double[] { 1.0, 2.0, 3.0 },
                null, // Null signal should be handled
                new double[] { 2.0, 2.0 }
            };
            
            // Act
            var results = VectorizedCalculations.RMSVectorized(signals);
            
            // Assert
            Assert.That(results.Length, Is.EqualTo(3));
            Assert.That(results[0], Is.GreaterThan(0)); // Should have valid RMS
            Assert.That(results[1], Is.EqualTo(0.0));   // Null signal should result in 0
            Assert.That(results[2], Is.EqualTo(2.0).Within(Tolerance)); // DC level = 2
        }

        [Test]
        public void RMSVectorized_LargeSignal_ProcessesCorrectly()
        {
            // Arrange - Create a large signal to test SIMD processing
            var largeSignal = new double[32]; // Should be processed in SIMD chunks
            for (int i = 0; i < largeSignal.Length; i++)
            {
                largeSignal[i] = Math.Sin(2 * Math.PI * i / largeSignal.Length); // Sine wave
            }
            var signals = new double[][] { largeSignal };
            
            // Act
            var results = VectorizedCalculations.RMSVectorized(signals);
            
            // Assert
            Assert.That(results.Length, Is.EqualTo(1));
            // RMS of a sine wave should be approximately 1/sqrt(2) ≈ 0.707
            Assert.That(results[0], Is.EqualTo(1.0/Math.Sqrt(2)).Within(0.1)); // More tolerance for discrete approximation
        }

        [Test]
        public void ImpedanceMagnitudesVectorized_ValidInputs_ReturnsCorrectResults()
        {
            // Arrange
            var resistances = new double[] { 3.0, 5.0, 0.0 };
            var reactances = new double[] { 4.0, 12.0, 10.0 };
            
            // Act
            var results = VectorizedCalculations.ImpedanceMagnitudesVectorized(resistances, reactances);
            
            // Assert
            Assert.That(results.Length, Is.EqualTo(3));
            
            // |Z1| = sqrt(3^2 + 4^2) = sqrt(9 + 16) = 5
            Assert.That(results[0], Is.EqualTo(5.0).Within(Tolerance));
            
            // |Z2| = sqrt(5^2 + 12^2) = sqrt(25 + 144) = 13
            Assert.That(results[1], Is.EqualTo(13.0).Within(Tolerance));
            
            // |Z3| = sqrt(0^2 + 10^2) = 10 (pure reactance)
            Assert.That(results[2], Is.EqualTo(10.0).Within(Tolerance));
        }

        [Test]
        public void ImpedanceMagnitudesVectorized_LargeArrays_ProcessesCorrectly()
        {
            // Arrange - Create arrays larger than SIMD vector length
            var size = 20;
            var resistances = new double[size];
            var reactances = new double[size];
            
            for (int i = 0; i < size; i++)
            {
                resistances[i] = i + 1; // 1, 2, 3, ...
                reactances[i] = i + 1; // 1, 2, 3, ...
            }
            
            // Act
            var results = VectorizedCalculations.ImpedanceMagnitudesVectorized(resistances, reactances);
            
            // Assert
            Assert.That(results.Length, Is.EqualTo(size));
            
            for (int i = 0; i < size; i++)
            {
                var expected = Math.Sqrt(resistances[i] * resistances[i] + reactances[i] * reactances[i]);
                Assert.That(results[i], Is.EqualTo(expected).Within(Tolerance));
            }
        }

        [Test]
        public void ImpedanceMagnitudesVectorized_NullArrays_ThrowsException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                VectorizedCalculations.ImpedanceMagnitudesVectorized(null, new double[1]));
            
            Assert.Throws<ArgumentNullException>(() => 
                VectorizedCalculations.ImpedanceMagnitudesVectorized(new double[1], null));
        }

        [Test]
        public void ImpedanceMagnitudesVectorized_MismatchedArrayLengths_ThrowsException()
        {
            // Arrange
            var resistances = new double[] { 1.0, 2.0 };
            var reactances = new double[] { 1.0, 2.0, 3.0 };
            
            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                VectorizedCalculations.ImpedanceMagnitudesVectorized(resistances, reactances));
        }

        [Test]
        public void RealPowerVectorized_ValidInputs_ReturnsCorrectResults()
        {
            // Arrange
            var voltages = new double[] { 120.0, 240.0, 12.0 };
            var currents = new double[] { 10.0, 5.0, 2.0 };
            var powerFactors = new double[] { 1.0, 0.8, 0.9 };
            
            // Act
            var results = VectorizedCalculations.RealPowerVectorized(voltages, currents, powerFactors);
            
            // Assert
            Assert.That(results.Length, Is.EqualTo(3));
            
            // P1 = 120 * 10 * 1.0 = 1200W
            Assert.That(results[0], Is.EqualTo(1200.0).Within(Tolerance));
            
            // P2 = 240 * 5 * 0.8 = 960W
            Assert.That(results[1], Is.EqualTo(960.0).Within(Tolerance));
            
            // P3 = 12 * 2 * 0.9 = 21.6W
            Assert.That(results[2], Is.EqualTo(21.6).Within(Tolerance));
        }

        [Test]
        public void RealPowerVectorized_LargeArrays_ProcessesCorrectly()
        {
            // Arrange - Test SIMD processing with larger arrays
            var size = 24;
            var voltages = new double[size];
            var currents = new double[size];
            var powerFactors = new double[size];
            
            for (int i = 0; i < size; i++)
            {
                voltages[i] = 120.0;
                currents[i] = i + 1;
                powerFactors[i] = 0.8;
            }
            
            // Act
            var results = VectorizedCalculations.RealPowerVectorized(voltages, currents, powerFactors);
            
            // Assert
            Assert.That(results.Length, Is.EqualTo(size));
            
            for (int i = 0; i < size; i++)
            {
                var expected = voltages[i] * currents[i] * powerFactors[i];
                Assert.That(results[i], Is.EqualTo(expected).Within(Tolerance));
            }
        }

        [Test]
        public void RealPowerVectorized_NullArrays_ThrowsException()
        {
            // Arrange
            var validArray = new double[] { 1.0 };
            
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                VectorizedCalculations.RealPowerVectorized(null, validArray, validArray));
            
            Assert.Throws<ArgumentNullException>(() => 
                VectorizedCalculations.RealPowerVectorized(validArray, null, validArray));
            
            Assert.Throws<ArgumentNullException>(() => 
                VectorizedCalculations.RealPowerVectorized(validArray, validArray, null));
        }

        [Test]
        public void RealPowerVectorized_MismatchedArrayLengths_ThrowsException()
        {
            // Arrange
            var voltages = new double[] { 120.0, 240.0 };
            var currents = new double[] { 10.0 };
            var powerFactors = new double[] { 1.0, 0.8 };
            
            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                VectorizedCalculations.RealPowerVectorized(voltages, currents, powerFactors));
        }

        [Test]
        public void VectorizedCalculations_PerformanceComparison_SIMDVersionShouldBeEfficient()
        {
            // This test verifies that the vectorized operations work correctly with realistic data sizes
            // Arrange - Create realistic circuit analysis data
            var size = 1000;
            var resistances = new double[size];
            var reactances = new double[size];
            
            var random = new Random(42); // Fixed seed for reproducible results
            for (int i = 0; i < size; i++)
            {
                resistances[i] = 10.0 + random.NextDouble() * 1000.0; // 10-1010 ohms
                reactances[i] = -500.0 + random.NextDouble() * 1000.0; // -500 to +500 ohms
            }
            
            // Act
            var impedanceMagnitudes = VectorizedCalculations.ImpedanceMagnitudesVectorized(resistances, reactances);
            
            // Assert - Verify all results are reasonable
            Assert.That(impedanceMagnitudes.Length, Is.EqualTo(size));
            
            for (int i = 0; i < size; i++)
            {
                Assert.That(impedanceMagnitudes[i], Is.GreaterThan(0));
                Assert.That(impedanceMagnitudes[i], Is.GreaterThanOrEqualTo(Math.Abs(resistances[i])));
                Assert.That(impedanceMagnitudes[i], Is.GreaterThanOrEqualTo(Math.Abs(reactances[i])));
                
                // Verify against manual calculation
                var expected = Math.Sqrt(resistances[i] * resistances[i] + reactances[i] * reactances[i]);
                Assert.That(impedanceMagnitudes[i], Is.EqualTo(expected).Within(Tolerance));
            }
        }

        [Test]
        public void VectorizedCalculations_EdgeCases_HandlesZeroValues()
        {
            // Test with zero values to ensure robustness
            
            // Test parallel resistance with one very large resistance (should approach other value)
            var resistances = new double[] { 100.0, 1e12 };
            var parallelResult = VectorizedCalculations.ParallelResistanceVectorized(resistances);
            Assert.That(parallelResult, Is.EqualTo(100.0).Within(1e-6)); // Should be very close to 100
            
            // Test impedance with zero resistance
            var zeroR = new double[] { 0.0 };
            var nonZeroX = new double[] { 10.0 };
            var impedanceResult = VectorizedCalculations.ImpedanceMagnitudesVectorized(zeroR, nonZeroX);
            Assert.That(impedanceResult[0], Is.EqualTo(10.0).Within(Tolerance));
            
            // Test power with zero power factor
            var voltages = new double[] { 120.0 };
            var currents = new double[] { 10.0 };
            var zeroPF = new double[] { 0.0 };
            var powerResult = VectorizedCalculations.RealPowerVectorized(voltages, currents, zeroPF);
            Assert.That(powerResult[0], Is.EqualTo(0.0).Within(Tolerance));
        }
    }
}
