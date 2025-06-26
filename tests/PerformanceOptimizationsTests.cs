using System;
using System.Linq;
using NUnit.Framework;
using CircuitTool.Performance;

namespace CircuitTool.Tests
{
    [TestFixture]
    public class PerformanceOptimizationsTests
    {
        [TestFixture]
        public class VectorizedCalculationsTests
        {
            [Test]
            public void ParallelResistanceVectorized_SimpleCase_ReturnsCorrectResult()
            {
                // Arrange
                var resistances = new double[] { 10.0, 20.0, 30.0 };
                
                // Act
                var result = VectorizedCalculations.ParallelResistanceVectorized(resistances);
                
                // Assert
                // 1/R_total = 1/10 + 1/20 + 1/30 = 6/60 + 3/60 + 2/60 = 11/60
                // R_total = 60/11 ≈ 5.45
                var expected = 1.0 / (1.0/10.0 + 1.0/20.0 + 1.0/30.0);
                Assert.That(result, Is.EqualTo(expected).Within(1e-10));
            }
            
            [Test]
            public void ParallelResistanceVectorized_LargeArray_ReturnsCorrectResult()
            {
                // Arrange - Create array large enough to test SIMD processing
                var resistances = Enumerable.Range(1, 100).Select(i => (double)i).ToArray();
                
                // Act
                var result = VectorizedCalculations.ParallelResistanceVectorized(resistances);
                
                // Assert - Compare with scalar calculation
                var expected = 1.0 / resistances.Sum(r => 1.0 / r);
                Assert.That(result, Is.EqualTo(expected).Within(1e-10));
            }
            
            [Test]
            public void ParallelResistanceVectorized_EmptyArray_ThrowsException()
            {
                // Arrange
                var resistances = new double[0];
                
                // Act & Assert
                Assert.Throws<ArgumentException>(() => 
                    VectorizedCalculations.ParallelResistanceVectorized(resistances));
            }
            
            [Test]
            public void RMSVectorized_MultipleSignals_ReturnsCorrectResults()
            {
                // Arrange
                var signals = new double[][]
                {
                    new double[] { 1, 2, 3, 4 },
                    new double[] { -2, -1, 1, 2 },
                    new double[] { 0, 0, 0, 0 }
                };
                
                // Act
                var results = VectorizedCalculations.RMSVectorized(signals);
                
                // Assert
                Assert.That(results.Length, Is.EqualTo(3));
                
                // Signal 1: RMS = sqrt((1² + 2² + 3² + 4²) / 4) = sqrt(30/4) = sqrt(7.5)
                Assert.That(results[0], Is.EqualTo(Math.Sqrt(7.5)).Within(1e-10));
                
                // Signal 2: RMS = sqrt((4 + 1 + 1 + 4) / 4) = sqrt(10/4) = sqrt(2.5)
                Assert.That(results[1], Is.EqualTo(Math.Sqrt(2.5)).Within(1e-10));
                
                // Signal 3: RMS = 0
                Assert.That(results[2], Is.EqualTo(0).Within(1e-10));
            }
            
            [Test]
            public void ImpedanceMagnitudesVectorized_ValidInputs_ReturnsCorrectResults()
            {
                // Arrange
                var resistances = new double[] { 3, 4, 5 };
                var reactances = new double[] { 4, 3, 12 };
                
                // Act
                var results = VectorizedCalculations.ImpedanceMagnitudesVectorized(resistances, reactances);
                
                // Assert
                Assert.That(results.Length, Is.EqualTo(3));
                Assert.That(results[0], Is.EqualTo(5).Within(1e-10)); // sqrt(3² + 4²) = 5
                Assert.That(results[1], Is.EqualTo(5).Within(1e-10)); // sqrt(4² + 3²) = 5
                Assert.That(results[2], Is.EqualTo(13).Within(1e-10)); // sqrt(5² + 12²) = 13
            }
            
            [Test]
            public void RealPowerVectorized_ValidInputs_ReturnsCorrectResults()
            {
                // Arrange
                var voltages = new double[] { 120, 240, 480 };
                var currents = new double[] { 10, 5, 2 };
                var powerFactors = new double[] { 1.0, 0.8, 0.9 };
                
                // Act
                var results = VectorizedCalculations.RealPowerVectorized(voltages, currents, powerFactors);
                
                // Assert
                Assert.That(results.Length, Is.EqualTo(3));
                Assert.That(results[0], Is.EqualTo(1200).Within(1e-10)); // 120 * 10 * 1.0
                Assert.That(results[1], Is.EqualTo(960).Within(1e-10)); // 240 * 5 * 0.8
                Assert.That(results[2], Is.EqualTo(864).Within(1e-10)); // 480 * 2 * 0.9
            }
        }
        
        [TestFixture]
        public class CalculationCacheTests
        {
            [SetUp]
            public void Setup()
            {
                CalculationCache.Clear();
            }
            
            [Test]
            public void GetOrCompute_FirstCall_ComputesAndCaches()
            {
                // Arrange
                var callCount = 0;
                var key = "test_key";
                
                // Act
                var result1 = CalculationCache.GetOrCompute(key, () => { callCount++; return 42; });
                var result2 = CalculationCache.GetOrCompute(key, () => { callCount++; return 99; });
                
                // Assert
                Assert.That(result1, Is.EqualTo(42));
                Assert.That(result2, Is.EqualTo(42)); // Should return cached value
                Assert.That(callCount, Is.EqualTo(1)); // Function should be called only once
            }
            
            [Test]
            public void CreateKey_MultipleParameters_CreatesUniqueKey()
            {
                // Arrange & Act
                var key1 = CalculationCache.CreateKey(1, 2.5, "test");
                var key2 = CalculationCache.CreateKey(1, 2.5, "different");
                var key3 = CalculationCache.CreateKey(1, 2.5, "test");
                
                // Assert
                Assert.That(key1, Is.Not.EqualTo(key2));
                Assert.That(key1, Is.EqualTo(key3));
            }
            
            [Test]
            public void Clear_RemovesAllEntries()
            {
                // Arrange
                CalculationCache.GetOrCompute("key1", () => 1);
                CalculationCache.GetOrCompute("key2", () => 2);
                
                // Act
                CalculationCache.Clear();
                
                // Assert
                var callCount = 0;
                CalculationCache.GetOrCompute("key1", () => { callCount++; return 99; });
                Assert.That(callCount, Is.EqualTo(1)); // Should compute again after clear
            }
        }
        
        [TestFixture]
        public class BulkOperationsTests
        {
            [Test]
            public void BulkParallelResistance_MultipleSets_ReturnsCorrectResults()
            {
                // Arrange
                var resistanceSets = new double[][]
                {
                    new double[] { 10, 20 },
                    new double[] { 30, 60, 90 },
                    new double[] { 100 }
                };
                
                // Act
                var results = BulkOperations.BulkParallelResistance(resistanceSets).ToArray();
                
                // Assert
                Assert.That(results.Length, Is.EqualTo(3));
                Assert.That(results[0], Is.EqualTo(20.0/3.0).Within(1e-10)); // 10||20 = 6.67
                Assert.That(results[1], Is.EqualTo(180.0/11.0).Within(1e-10)); // 30||60||90 = 16.36
                Assert.That(results[2], Is.EqualTo(100).Within(1e-10)); // Single resistor
            }
            
            [Test]
            public void BulkFrequencyResponse_MultipleCircuits_ReturnsCorrectDimensions()
            {
                // Arrange
                var frequencies = new double[] { 100, 1000, 10000 };
                var circuits = new (double R, double L, double C)[]
                {
                    (1000, 0.001, 1e-6), // R=1kΩ, L=1mH, C=1μF
                    (500, 0.0005, 2e-6)  // R=500Ω, L=0.5mH, C=2μF
                };
                
                // Act
                var results = BulkOperations.BulkFrequencyResponse(frequencies, circuits);
                
                // Assert
                Assert.That(results.Length, Is.EqualTo(2)); // Two circuits
                Assert.That(results[0].Length, Is.EqualTo(3)); // Three frequencies each
                Assert.That(results[1].Length, Is.EqualTo(3));
                
                // All results should be positive (admittance magnitudes)
                Assert.That(results[0].All(r => r > 0), Is.True);
                Assert.That(results[1].All(r => r > 0), Is.True);
            }
            
            [Test]
            public void BulkPowerCalculation_ValidInputs_ReturnsCorrectResults()
            {
                // Arrange
                var voltages = new double[] { 120, 240 };
                var currents = new double[] { 10, 5 };
                var phases = new double[] { 0, Math.PI / 6 }; // 0° and 30°
                
                // Act
                var results = BulkOperations.BulkPowerCalculation(voltages, currents, phases);
                
                // Assert
                Assert.That(results.Length, Is.EqualTo(2));
                
                // First circuit: V=120V, I=10A, φ=0° => P=1200W, Q=0VAR, S=1200VA
                Assert.That(results[0].real, Is.EqualTo(1200).Within(1e-10));
                Assert.That(results[0].reactive, Is.EqualTo(0).Within(1e-10));
                Assert.That(results[0].apparent, Is.EqualTo(1200).Within(1e-10));
                
                // Second circuit: V=240V, I=5A, φ=30° => S=1200VA, P=S*cos(30°), Q=S*sin(30°)
                var expectedApparent = 240 * 5; // 1200VA
                var expectedReal = expectedApparent * Math.Cos(Math.PI / 6); // 1200 * √3/2 ≈ 1039.23W
                var expectedReactive = expectedApparent * Math.Sin(Math.PI / 6); // 1200 * 0.5 = 600VAR
                
                Assert.That(results[1].apparent, Is.EqualTo(expectedApparent).Within(1e-10));
                Assert.That(results[1].real, Is.EqualTo(expectedReal).Within(1e-10));
                Assert.That(results[1].reactive, Is.EqualTo(expectedReactive).Within(1e-10));
            }
            
            [Test]
            public void StreamingCalculation_LargeDataset_ProcessesInBatches()
            {
                // Arrange
                var data = Enumerable.Range(1, 10000).Select(i => (double)i).ToList();
                var batchSize = 100;
                var processedBatches = 0;
                
                // Act
                var results = BulkOperations.StreamingCalculation(
                    data,
                    batch => { processedBatches++; return batch.Select(x => x * 2).ToArray(); },
                    batchSize
                ).ToArray();
                
                // Assert
                Assert.That(results.Length, Is.EqualTo(10000));
                Assert.That(results[0], Is.EqualTo(2)); // First item: 1 * 2 = 2
                Assert.That(results[9999], Is.EqualTo(20000)); // Last item: 10000 * 2 = 20000
                Assert.That(processedBatches, Is.EqualTo(100)); // 10000 / 100 = 100 batches
            }
        }
        
        [TestFixture]
        public class PerformanceMonitorTests
        {
            [SetUp]
            public void Setup()
            {
                PerformanceMonitor.ClearStats();
            }
            
            [Test]
            public void MeasureOperation_RecordsExecutionTime()
            {
                // Arrange
                var operationName = "test_operation";
                
                // Act
                var result = PerformanceMonitor.MeasureOperation(operationName, () =>
                {
                    System.Threading.Thread.Sleep(10); // Simulate work
                    return 42;
                });
                
                // Assert
                Assert.That(result, Is.EqualTo(42));
                
                var stats = PerformanceMonitor.GetStats(operationName);
                Assert.That(stats, Is.Not.Null);
                Assert.That(stats.Value.callCount, Is.EqualTo(1));
                Assert.That(stats.Value.averageMs, Is.GreaterThan(5)); // Should take at least 5ms
                Assert.That(stats.Value.totalMs, Is.EqualTo(stats.Value.averageMs));
            }
            
            [Test]
            public void MeasureOperation_MultipleCalls_AggregatesStats()
            {
                // Arrange
                var operationName = "repeated_operation";
                
                // Act
                for (int i = 0; i < 3; i++)
                {
                    PerformanceMonitor.MeasureOperation(operationName, () => i * 2);
                }
                
                // Assert
                var stats = PerformanceMonitor.GetStats(operationName);
                Assert.That(stats, Is.Not.Null);
                Assert.That(stats.Value.callCount, Is.EqualTo(3));
                Assert.That(stats.Value.totalMs, Is.GreaterThanOrEqualTo(0));
            }
            
            [Test]
            public void GetAllStats_ReturnsAllOperations()
            {
                // Arrange
                PerformanceMonitor.MeasureOperation("op1", () => 1);
                PerformanceMonitor.MeasureOperation("op2", () => 2);
                
                // Act
                var allStats = PerformanceMonitor.GetAllStats();
                
                // Assert
                Assert.That(allStats.Count, Is.EqualTo(2));
                Assert.That(allStats.ContainsKey("op1"), Is.True);
                Assert.That(allStats.ContainsKey("op2"), Is.True);
            }
            
            [Test]
            public void ClearStats_RemovesAllStats()
            {
                // Arrange
                PerformanceMonitor.MeasureOperation("test", () => 1);
                
                // Act
                PerformanceMonitor.ClearStats();
                
                // Assert
                var stats = PerformanceMonitor.GetStats("test");
                Assert.That(stats, Is.Null);
            }
        }
    }
}
