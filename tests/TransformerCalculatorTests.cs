using NUnit.Framework;
using CircuitTool;
using System;

namespace CircuitTool.Tests
{
    [TestFixture]
    public class TransformerCalculatorTests
    {
        [Test]
        public void SecondaryVoltage_StepDownTransformer_ReturnsCorrectValue()
        {
            // Arrange
            double primaryVoltage = 120; // 120V
            double primaryTurns = 240;
            double secondaryTurns = 24; // 10:1 ratio
            
            // Act
            double result = TransformerCalculator.SecondaryVoltage(primaryVoltage, primaryTurns, secondaryTurns);
            
            // Assert
            Assert.That(result, Is.EqualTo(12).Within(0.001)); // 12V
        }

        [Test]
        public void SecondaryVoltage_StepUpTransformer_ReturnsCorrectValue()
        {
            // Arrange
            double primaryVoltage = 12; // 12V
            double primaryTurns = 24;
            double secondaryTurns = 240; // 1:10 ratio
            
            // Act
            double result = TransformerCalculator.SecondaryVoltage(primaryVoltage, primaryTurns, secondaryTurns);
            
            // Assert
            Assert.That(result, Is.EqualTo(120).Within(0.001)); // 120V
        }

        [Test]
        public void SecondaryVoltage_ZeroPrimaryTurns_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => TransformerCalculator.SecondaryVoltage(120, 0, 24));
        }

        [Test]
        public void PrimaryCurrent_StepDownTransformer_ReturnsCorrectValue()
        {
            // Arrange
            double secondaryCurrent = 10; // 10A
            double primaryTurns = 240;
            double secondaryTurns = 24; // 10:1 ratio
            
            // Act
            double result = TransformerCalculator.PrimaryCurrent(secondaryCurrent, primaryTurns, secondaryTurns);
            
            // Assert
            Assert.That(result, Is.EqualTo(1).Within(0.001)); // 1A
        }

        [Test]
        public void TurnsRatio_ValidInputs_ReturnsCorrectValue()
        {
            // Arrange
            double primaryTurns = 240;
            double secondaryTurns = 24;
            
            // Act
            double result = TransformerCalculator.TurnsRatio(primaryTurns, secondaryTurns);
            
            // Assert
            Assert.That(result, Is.EqualTo(10).Within(0.001)); // 10:1
        }

        [Test]
        public void TurnsRatio_ZeroSecondaryTurns_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => TransformerCalculator.TurnsRatio(240, 0));
        }

        [Test]
        public void VoltageRatio_ValidInputs_ReturnsCorrectValue()
        {
            // Arrange
            double primaryVoltage = 120;
            double secondaryVoltage = 12;
            
            // Act
            double result = TransformerCalculator.VoltageRatio(primaryVoltage, secondaryVoltage);
            
            // Assert
            Assert.That(result, Is.EqualTo(10).Within(0.001)); // 10:1
        }

        [Test]
        public void VoltageRatio_ZeroSecondaryVoltage_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => TransformerCalculator.VoltageRatio(120, 0));
        }

        [Test]
        public void Efficiency_ValidInputs_ReturnsCorrectValue()
        {
            // Arrange
            double outputPower = 950; // 950W
            double inputPower = 1000; // 1000W
            
            // Act
            double result = TransformerCalculator.Efficiency(outputPower, inputPower);
            
            // Assert
            Assert.That(result, Is.EqualTo(95).Within(0.001)); // 95%
        }

        [Test]
        public void Efficiency_ZeroInputPower_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => TransformerCalculator.Efficiency(950, 0));
        }

        [Test]
        public void Efficiency_NegativeOutputPower_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => TransformerCalculator.Efficiency(-100, 1000));
        }

        [Test]
        public void PowerLoss_ValidInputs_ReturnsCorrectValue()
        {
            // Arrange
            double inputPower = 1000; // 1000W
            double outputPower = 950; // 950W
            
            // Act
            double result = TransformerCalculator.PowerLoss(inputPower, outputPower);
            
            // Assert
            Assert.That(result, Is.EqualTo(50).Within(0.001)); // 50W loss
        }

        [Test]
        public void PowerLoss_OutputExceedsInput_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => TransformerCalculator.PowerLoss(950, 1000));
        }

        [Test]
        public void Regulation_ValidInputs_ReturnsCorrectValue()
        {
            // Arrange
            double noLoadVoltage = 125.0; // 125V
            double fullLoadVoltage = 120.0; // 120V
            
            // Act
            double result = TransformerCalculator.Regulation(noLoadVoltage, fullLoadVoltage);
            
            // Assert
            double expected = ((125.0 - 120.0) / 120.0) * 100.0; // ≈ 4.17%
            Assert.That(result, Is.EqualTo(expected).Within(0.01));
        }

        [Test]
        public void Regulation_ZeroFullLoadVoltage_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => TransformerCalculator.Regulation(125, 0));
        }

        [Test]
        public void RequiredSecondaryTurns_ValidInputs_ReturnsCorrectValue()
        {
            // Arrange
            double primaryVoltage = 120; // 120V
            double secondaryVoltage = 12; // 12V
            double primaryTurns = 240;
            
            // Act
            double result = TransformerCalculator.RequiredSecondaryTurns(primaryVoltage, secondaryVoltage, primaryTurns);
            
            // Assert
            Assert.That(result, Is.EqualTo(24).Within(0.001)); // 24 turns
        }

        [Test]
        public void RequiredSecondaryTurns_ZeroPrimaryVoltage_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => TransformerCalculator.RequiredSecondaryTurns(0, 12, 240));
        }

        [Test]
        public void ApparentPower_ValidInputs_ReturnsCorrectValue()
        {
            // Arrange
            double voltage = 120; // 120V
            double current = 10; // 10A
            
            // Act
            double result = TransformerCalculator.ApparentPower(voltage, current);
            
            // Assert
            Assert.That(result, Is.EqualTo(1200).Within(0.001)); // 1200 VA
        }

        [Test]
        public void ApparentPower_NegativeVoltage_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => TransformerCalculator.ApparentPower(-120, 10));
        }

        [Test]
        public void ApparentPower_NegativeCurrent_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => TransformerCalculator.ApparentPower(120, -10));
        }
    }
}
