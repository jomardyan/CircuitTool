using NUnit.Framework;
using System;
using System.IO;
using CircuitTool;

namespace CircuitTool.Tests
{
    [TestFixture]
    public class DocumentationExamplesTests
    {
        private StringWriter _stringWriter;
        private TextWriter _originalOut;

        [SetUp]
        public void Setup()
        {
            _stringWriter = new StringWriter();
            _originalOut = Console.Out;
            Console.SetOut(_stringWriter);
        }

        [TearDown]
        public void TearDown()
        {
            Console.SetOut(_originalOut);
            _stringWriter?.Dispose();
        }

        [Test]
        public void BasicOhmsLaw_RunExample_ProducesOutput()
        {
            // Act
            DocumentationExamples.BasicOhmsLaw.RunExample();
            var output = _stringWriter.ToString();

            // Assert
            Assert.That(output, Contains.Substring("Basic Ohm's Law Calculations"));
            Assert.That(output, Contains.Substring("V = 12V"));
            Assert.That(output, Contains.Substring("R = 100"));
            Assert.That(output, Contains.Substring("I = V/R"));
            Assert.That(output, Contains.Substring("P = V × I"));
        }

        [Test]
        public void CircuitBuilding_RunExample_ProducesOutput()
        {
            // Act
            DocumentationExamples.CircuitBuilding.RunExample();
            var output = _stringWriter.ToString();

            // Assert
            Assert.That(output, Contains.Substring("Circuit Building Example"));
            Assert.That(output, Contains.Substring("voltage divider"));
            Assert.That(output, Contains.Substring("Input: 12V"));
            Assert.That(output, Contains.Substring("Output:"));
        }

        [Test]
        public void ACAnalysis_RunExample_ProducesOutput()
        {
            // Act
            DocumentationExamples.ACAnalysis.RunExample();
            var output = _stringWriter.ToString();

            // Assert
            Assert.That(output, Contains.Substring("AC Circuit Analysis"));
            Assert.That(output, Contains.Substring("RC Low-pass filter"));
            Assert.That(output, Contains.Substring("Cutoff frequency:"));
        }

        [Test]
        public void EnergyCalculations_RunExample_ProducesOutput()
        {
            // Act
            DocumentationExamples.EnergyCalculations.RunExample();
            var output = _stringWriter.ToString();

            // Assert
            Assert.That(output, Contains.Substring("Energy Calculations"));
            Assert.That(output, Contains.Substring("LED Array"));
            Assert.That(output, Contains.Substring("Total power:"));
            Assert.That(output, Contains.Substring("Monthly energy"));
            Assert.That(output, Contains.Substring("Monthly cost"));
        }

        [Test]
        public void RunAllExamples_ExecutesWithoutError()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => DocumentationExamples.RunAllExamples());
            
            var output = _stringWriter.ToString();
            Assert.That(output, Contains.Substring("CircuitTool Documentation Examples"));
            Assert.That(output, Contains.Substring("All examples completed!"));
        }

        [Test]
        public void InteractiveTutorials_RunBasicElectronicsTutorial_ProducesOutput()
        {
            // Act
            InteractiveTutorials.RunBasicElectronicsTutorial();
            var output = _stringWriter.ToString();

            // Assert
            Assert.That(output, Contains.Substring("Basic Electronics Tutorial"));
            Assert.That(output, Contains.Substring("Step 1: Understanding Ohm's Law"));
            Assert.That(output, Contains.Substring("Step 2: Power Calculation"));
            Assert.That(output, Contains.Substring("Step 3: Series Circuits"));
            Assert.That(output, Contains.Substring("Step 4: Parallel Circuits"));
            Assert.That(output, Contains.Substring("Tutorial completed!"));
        }
    }

    [TestFixture]
    public class UseCaseTemplatesTests
    {
        [Test]
        public void VoltageDividerDesign_ValidInputs_ReturnsCorrectValues()
        {
            // Arrange
            double inputVoltage = 12.0;
            double outputVoltage = 4.0;
            
            // Act
            var (r1, r2) = UseCaseTemplates.VoltageDividerDesign.DesignVoltageDivider(
                inputVoltage, outputVoltage, 0.001);
            
            // Assert
            Assert.That(r1, Is.GreaterThan(0));
            Assert.That(r2, Is.GreaterThan(0));
            
            // Verify the design produces the correct output voltage
            double actualOutput = VoltageDividerCalculator.Calculate(inputVoltage, r1, r2);
            Assert.That(actualOutput, Is.EqualTo(outputVoltage).Within(0.01));
        }

        [Test]
        public void VoltageDividerDesign_GenerateCode_ReturnsValidCode()
        {
            // Act
            string code = UseCaseTemplates.VoltageDividerDesign.GenerateCode(12.0, 4.0);
            
            // Assert
            Assert.That(code, Is.Not.Null);
            Assert.That(code, Contains.Substring("Voltage divider"));
            Assert.That(code, Contains.Substring("VoltageDividerCalculator"));
        }

        [Test]
        public void FilterDesign_ValidCutoffFrequency_ReturnsCorrectValues()
        {
            // Arrange
            double cutoffFrequency = 1000; // 1kHz
            
            // Act
            var (r, c) = UseCaseTemplates.FilterDesign.DesignLowPassFilter(cutoffFrequency);
            
            // Assert
            Assert.That(r, Is.GreaterThan(0));
            Assert.That(c, Is.GreaterThan(0));
            
            // Verify the design produces the correct cutoff frequency
            double actualCutoff = FilterCalculator.RCLowPassCutoffFrequency(r, c);
            Assert.That(actualCutoff, Is.EqualTo(cutoffFrequency).Within(1));
        }

        [Test]
        public void FilterDesign_GenerateLowPassCode_ReturnsValidCode()
        {
            // Act
            string code = UseCaseTemplates.FilterDesign.GenerateLowPassCode(1000);
            
            // Assert
            Assert.That(code, Is.Not.Null);
            Assert.That(code, Contains.Substring("Low-pass filter"));
            Assert.That(code, Contains.Substring("FilterCalculator"));
            Assert.That(code, Contains.Substring("1000Hz"));
        }

        [Test]
        public void LEDResistorCalculator_ValidInputs_ReturnsCorrectResistance()
        {
            // Arrange
            double supplyVoltage = 5.0;
            double ledVoltage = 2.0;
            double ledCurrent = 0.02; // 20mA
            
            // Act
            double resistance = UseCaseTemplates.LEDResistorCalculator
                .CalculateCurrentLimitingResistor(supplyVoltage, ledVoltage, ledCurrent);
            
            // Assert
            double expected = (supplyVoltage - ledVoltage) / ledCurrent;
            Assert.That(resistance, Is.EqualTo(expected).Within(0.01));
            Assert.That(resistance, Is.EqualTo(150).Within(0.01)); // (5-2)/0.02 = 150Ω
        }

        [Test]
        public void LEDResistorCalculator_GenerateCode_ReturnsValidCode()
        {
            // Act
            string code = UseCaseTemplates.LEDResistorCalculator.GenerateCode(5.0, 2.0, 0.02);
            
            // Assert
            Assert.That(code, Is.Not.Null);
            Assert.That(code, Contains.Substring("LED current limiting resistor"));
            Assert.That(code, Contains.Substring("20mA"));
        }
    }
}
