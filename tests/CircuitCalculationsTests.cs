using NUnit.Framework;
using CircuitTool;

namespace CircuitTool.Tests
{
    [TestFixture]
    public class CircuitCalculationsTests
    {
        [Test]
        public void CalculateTotalResistance_Series_ReturnsCorrectSum()
        {
            // Arrange
            double[] resistances = { 10, 20, 30 };
            
            // Act
            double result = CircuitCalculations.CalculateTotalResistance(resistances, true);
            
            // Assert
            Assert.That(result, Is.EqualTo(60).Within(0.001));
        }

        [Test]
        public void CalculateTotalResistance_Parallel_ReturnsCorrectValue()
        {
            // Arrange
            double[] resistances = { 10, 10 };
            
            // Act
            double result = CircuitCalculations.CalculateTotalResistance(resistances, false);
            
            // Assert
            Assert.That(result, Is.EqualTo(5).Within(0.001));
        }

        [Test]
        public void CalculateTotalResistance_ParallelThreeResistors_ReturnsCorrectValue()
        {
            // Arrange
            double[] resistances = { 6, 3, 2 };
            
            // Act
            double result = CircuitCalculations.CalculateTotalResistance(resistances, false);
            
            // Assert
            Assert.That(result, Is.EqualTo(1).Within(0.001));
        }

        [Test]
        public void CalculatePower_ValidInputs_ReturnsCorrectPower()
        {
            // Arrange
            double voltage = 12;
            double current = 2;
            
            // Act
            double result = CircuitCalculations.CalculatePower(voltage, current);
            
            // Assert
            Assert.That(result, Is.EqualTo(24).Within(0.001));
        }

        [Test]
        public void CalculateEnergy_ValidInputs_ReturnsCorrectEnergy()
        {
            // Arrange
            double power = 100;
            double time = 2.5;
            
            // Act
            double result = CircuitCalculations.CalculateEnergy(power, time);
            
            // Assert
            Assert.That(result, Is.EqualTo(250).Within(0.001));
        }

        [Test]
        public void CalculateTotalResistance_SingleResistor_ReturnsSameValue()
        {
            // Arrange
            double[] resistances = { 15 };
            
            // Act
            double resultSeries = CircuitCalculations.CalculateTotalResistance(resistances, true);
            double resultParallel = CircuitCalculations.CalculateTotalResistance(resistances, false);
            
            // Assert
            Assert.That(resultSeries, Is.EqualTo(15).Within(0.001));
            Assert.That(resultParallel, Is.EqualTo(15).Within(0.001));
        }
    }
}
