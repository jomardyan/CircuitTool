using NUnit.Framework;
using CircuitTool;

namespace CircuitTool.Tests
{
    [TestFixture]
    public class WattsVoltsAmpsOhmsCalculatorTests
    {
        [Test]
        public void Watts_ValidInputs_ReturnsCorrectPower()
        {
            // Arrange
            double volts = 12.0;
            double amps = 2.0;
            
            // Act
            double result = WattsVoltsAmpsOhmsCalculator.Watts(volts, amps);
            
            // Assert
            Assert.That(result, Is.EqualTo(24.0).Within(0.001));
        }

        [Test]
        public void Volts_ValidInputs_ReturnsCorrectVoltage()
        {
            // Arrange
            double watts = 100.0;
            double amps = 5.0;
            
            // Act
            double result = WattsVoltsAmpsOhmsCalculator.Volts(watts, amps);
            
            // Assert
            Assert.That(result, Is.EqualTo(20.0).Within(0.001));
        }

        [Test]
        public void Amps_ValidInputs_ReturnsCorrectCurrent()
        {
            // Arrange
            double watts = 60.0;
            double volts = 12.0;
            
            // Act
            double result = WattsVoltsAmpsOhmsCalculator.Amps(watts, volts);
            
            // Assert
            Assert.That(result, Is.EqualTo(5.0).Within(0.001));
        }

        [Test]
        public void Ohms_ValidInputs_ReturnsCorrectResistance()
        {
            // Arrange
            double volts = 9.0;
            double amps = 3.0;
            
            // Act
            double result = WattsVoltsAmpsOhmsCalculator.Ohms(volts, amps);
            
            // Assert
            Assert.That(result, Is.EqualTo(3.0).Within(0.001));
        }

        [Test]
        public void Watts_ZeroValues_ReturnsZero()
        {
            // Arrange & Act & Assert
            Assert.That(WattsVoltsAmpsOhmsCalculator.Watts(0, 5), Is.EqualTo(0).Within(0.001));
            Assert.That(WattsVoltsAmpsOhmsCalculator.Watts(5, 0), Is.EqualTo(0).Within(0.001));
            Assert.That(WattsVoltsAmpsOhmsCalculator.Watts(0, 0), Is.EqualTo(0).Within(0.001));
        }

        [Test]
        public void CalculationsWithSmallValues_ReturnsCorrectResults()
        {
            // Test with milliamps and millivolts
            double watts = WattsVoltsAmpsOhmsCalculator.Watts(0.001, 0.001); // 1mV * 1mA
            Assert.That(watts, Is.EqualTo(0.000001).Within(0.000000001)); // 1µW

            double volts = WattsVoltsAmpsOhmsCalculator.Volts(0.001, 0.001); // 1mW / 1mA
            Assert.That(volts, Is.EqualTo(1.0).Within(0.001)); // 1V
        }
    }
}
