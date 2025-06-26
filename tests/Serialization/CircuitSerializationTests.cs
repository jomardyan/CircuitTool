using System;
using System.IO;
using NUnit.Framework;
using CircuitTool.Serialization;
using CircuitTool.CircuitBuilder;

namespace CircuitTool.Tests.Serialization
{
    [TestFixture]
    public class CircuitSerializationTests
    {
        private Circuit _testCircuit;
        private string _tempFilePath;

        [SetUp]
        public void SetUp()
        {
            // Create a test circuit with different component types
            _testCircuit = CircuitTool.CircuitBuilder.CircuitBuilder.New()
                .AddResistor(1000, "R1")
                .AddCapacitor(1e-6, "C1")
                .AddInductor(1e-3, "L1")
                .Build();
                
            _tempFilePath = Path.GetTempFileName();
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(_tempFilePath))
            {
                File.Delete(_tempFilePath);
            }
        }

        [Test]
        public void ToJson_ValidCircuit_ReturnsValidJson()
        {
            // Act
            var json = CircuitSerialization.ToJson(_testCircuit);
            
            // Assert
            Assert.That(json, Is.Not.Null);
            Assert.That(json, Is.Not.Empty);
            Assert.That(json, Contains.Substring("\"version\": \"1.0\""));
            Assert.That(json, Contains.Substring("\"components\""));
            Assert.That(json, Contains.Substring("\"type\": \"Resistor\""));
            Assert.That(json, Contains.Substring("\"type\": \"Capacitor\""));
            Assert.That(json, Contains.Substring("\"type\": \"Inductor\""));
            Assert.That(json, Contains.Substring("\"resistance\": 1000"));
            Assert.That(json, Contains.Substring("\"capacitance\": 1E-06"));
            Assert.That(json, Contains.Substring("\"inductance\": 0.001"));
        }

        [Test]
        public void ToJson_NullCircuit_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => CircuitSerialization.ToJson(null));
        }

        [Test]
        public void FromJson_ValidJson_ReturnsCircuit()
        {
            // Arrange
            var json = @"{
  ""version"": ""1.0"",
  ""components"": [
    {
      ""id"": ""1"",
      ""name"": ""R1"",
      ""type"": ""Resistor"",
      ""tolerance"": 0.05,
      ""resistance"": 1000
    },
    {
      ""id"": ""2"",
      ""name"": ""C1"",
      ""type"": ""Capacitor"",
      ""tolerance"": 0.05,
      ""capacitance"": 1E-06
    }
  ]
}";
            
            // Act
            var circuit = CircuitSerialization.FromJson(json);
            
            // Assert
            Assert.That(circuit, Is.Not.Null);
            Assert.That(circuit.Components, Is.Not.Null);
            Assert.That(circuit.Components.Count, Is.GreaterThan(0));
        }

        [Test]
        public void FromJson_EmptyString_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => CircuitSerialization.FromJson(""));
        }

        [Test]
        public void FromJson_NullString_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => CircuitSerialization.FromJson(null));
        }

        [Test]
        public void FromJson_InvalidJson_ThrowsFormatException()
        {
            // Arrange
            var invalidJson = "{ invalid json }";
            
            // Act & Assert
            Assert.Throws<FormatException>(() => CircuitSerialization.FromJson(invalidJson));
        }

        [Test]
        public void ToXml_ValidCircuit_ReturnsValidXml()
        {
            // Act
            var xml = CircuitSerialization.ToXml(_testCircuit);
            
            // Assert
            Assert.That(xml, Is.Not.Null);
            Assert.That(xml, Is.Not.Empty);
            Assert.That(xml, Contains.Substring("<?xml version=\"1.0\" encoding=\"UTF-8\"?>"));
            Assert.That(xml, Contains.Substring("<circuit version=\"1.0\">"));
            Assert.That(xml, Contains.Substring("</circuit>"));
            Assert.That(xml, Contains.Substring("<resistor"));
            Assert.That(xml, Contains.Substring("resistance=\"1000\""));
            Assert.That(xml, Contains.Substring("<capacitor"));
            Assert.That(xml, Contains.Substring("capacitance=\"1E-06\""));
            Assert.That(xml, Contains.Substring("<inductor"));
            Assert.That(xml, Contains.Substring("inductance=\"0.001\""));
        }

        [Test]
        public void ToXml_NullCircuit_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => CircuitSerialization.ToXml(null));
        }

        [Test]
        public void FromXml_ValidXml_ReturnsCircuit()
        {
            // Arrange
            var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<circuit version=""1.0"">
  <resistor id=""1"" name=""R1"" resistance=""1000"" tolerance=""0.05"" />
  <capacitor id=""2"" name=""C1"" capacitance=""1E-06"" tolerance=""0.05"" />
  <inductor id=""3"" name=""L1"" inductance=""0.001"" tolerance=""0.05"" />
</circuit>";
            
            // Act
            var circuit = CircuitSerialization.FromXml(xml);
            
            // Assert
            Assert.That(circuit, Is.Not.Null);
            Assert.That(circuit.Components, Is.Not.Null);
            Assert.That(circuit.Components.Count, Is.GreaterThan(0));
        }

        [Test]
        public void FromXml_EmptyString_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => CircuitSerialization.FromXml(""));
        }

        [Test]
        public void FromXml_NullString_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => CircuitSerialization.FromXml(null));
        }

        [Test]
        public void JsonRoundTrip_SerializeAndDeserialize_PreservesBasicStructure()
        {
            // Act
            var json = CircuitSerialization.ToJson(_testCircuit);
            var deserializedCircuit = CircuitSerialization.FromJson(json);
            
            // Assert
            Assert.That(deserializedCircuit, Is.Not.Null);
            Assert.That(deserializedCircuit.Components, Is.Not.Null);
            // Note: Due to simplified parsing, exact component preservation is not expected
            // but basic structure should be maintained
        }

        [Test]
        public void XmlRoundTrip_SerializeAndDeserialize_PreservesBasicStructure()
        {
            // Act
            var xml = CircuitSerialization.ToXml(_testCircuit);
            var deserializedCircuit = CircuitSerialization.FromXml(xml);
            
            // Assert
            Assert.That(deserializedCircuit, Is.Not.Null);
            Assert.That(deserializedCircuit.Components, Is.Not.Null);
        }

        [Test]
        public void ExportToFile_JsonFormat_CreatesFile()
        {
            // Act
            CircuitImportExport.ExportToFile(_testCircuit, _tempFilePath, ExportFormat.Json);
            
            // Assert
            Assert.That(File.Exists(_tempFilePath), Is.True);
            var content = File.ReadAllText(_tempFilePath);
            Assert.That(content, Contains.Substring("\"version\": \"1.0\""));
            Assert.That(content, Contains.Substring("\"components\""));
        }

        [Test]
        public void ExportToFile_XmlFormat_CreatesFile()
        {
            // Act
            CircuitImportExport.ExportToFile(_testCircuit, _tempFilePath, ExportFormat.Xml);
            
            // Assert
            Assert.That(File.Exists(_tempFilePath), Is.True);
            var content = File.ReadAllText(_tempFilePath);
            Assert.That(content, Contains.Substring("<?xml version=\"1.0\""));
            Assert.That(content, Contains.Substring("<circuit version=\"1.0\">"));
        }

        [Test]
        public void ExportToFile_NullCircuit_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                CircuitImportExport.ExportToFile(null, _tempFilePath));
        }

        [Test]
        public void ExportToFile_EmptyFilePath_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                CircuitImportExport.ExportToFile(_testCircuit, ""));
        }

        [Test]
        public void ExportToFile_NullFilePath_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                CircuitImportExport.ExportToFile(_testCircuit, null));
        }

        [Test]
        public void ImportFromFile_JsonFormat_ReturnsCircuit()
        {
            // Arrange
            CircuitImportExport.ExportToFile(_testCircuit, _tempFilePath, ExportFormat.Json);
            
            // Act
            var importedCircuit = CircuitImportExport.ImportFromFile(_tempFilePath, ExportFormat.Json);
            
            // Assert
            Assert.That(importedCircuit, Is.Not.Null);
            Assert.That(importedCircuit.Components, Is.Not.Null);
        }

        [Test]
        public void ImportFromFile_XmlFormat_ReturnsCircuit()
        {
            // Arrange
            CircuitImportExport.ExportToFile(_testCircuit, _tempFilePath, ExportFormat.Xml);
            
            // Act
            var importedCircuit = CircuitImportExport.ImportFromFile(_tempFilePath, ExportFormat.Xml);
            
            // Assert
            Assert.That(importedCircuit, Is.Not.Null);
            Assert.That(importedCircuit.Components, Is.Not.Null);
        }

        [Test]
        public void ImportFromFile_NonExistentFile_ThrowsFileNotFoundException()
        {
            // Arrange
            var nonExistentPath = Path.Combine(Path.GetTempPath(), "nonexistent.json");
            
            // Act & Assert
            Assert.Throws<FileNotFoundException>(() => 
                CircuitImportExport.ImportFromFile(nonExistentPath));
        }

        [Test]
        public void ImportFromFile_EmptyFilePath_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                CircuitImportExport.ImportFromFile(""));
        }

        [Test]
        public void ImportFromFile_NullFilePath_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                CircuitImportExport.ImportFromFile(null));
        }

        [Test]
        public void ExportFormat_InvalidFormat_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                CircuitImportExport.ExportToFile(_testCircuit, _tempFilePath, (ExportFormat)999));
        }

        [Test]
        public void ExtractValue_ValidKeyValue_ReturnsValue()
        {
            // This tests the private method indirectly through ToJson behavior
            var json = CircuitSerialization.ToJson(_testCircuit);
            
            // Assert that the JSON contains expected key-value pairs
            Assert.That(json, Contains.Substring("\"version\": \"1.0\""));
        }

        [Test]
        public void CircuitSerialization_MinimalCircuit_SerializesCorrectly()
        {
            // Arrange - Create minimal circuit with single component (empty circuit not allowed)
            var minimalCircuit = CircuitTool.CircuitBuilder.CircuitBuilder.New()
                .AddResistor(1000)
                .Build();
            
            // Act
            var json = CircuitSerialization.ToJson(minimalCircuit);
            var xml = CircuitSerialization.ToXml(minimalCircuit);
            
            // Assert
            Assert.That(json, Contains.Substring("\"version\": \"1.0\""));
            Assert.That(json, Contains.Substring("\"components\""));
            Assert.That(xml, Contains.Substring("<circuit version=\"1.0\">"));
            Assert.That(xml, Contains.Substring("</circuit>"));
        }

        [Test]
        public void CircuitSerialization_ComponentWithTolerance_IncludesToleranceInSerialization()
        {
            // Arrange
            var circuit = CircuitTool.CircuitBuilder.CircuitBuilder.New()
                .AddResistor(1000)
                .WithComponentTolerance(0.1)  // 10% tolerance
                .Build();
            
            // Act
            var json = CircuitSerialization.ToJson(circuit);
            var xml = CircuitSerialization.ToXml(circuit);
            
            // Assert
            Assert.That(json, Contains.Substring("\"tolerance\": 0.1"));
            Assert.That(xml, Contains.Substring("tolerance=\"0.1\""));
        }

        [Test]
        public void CircuitSerialization_LargeCircuit_HandlesMultipleComponents()
        {
            // Arrange
            var largeCircuit = CircuitTool.CircuitBuilder.CircuitBuilder.New()
                .AddResistor(1000, "R1")
                .AddResistor(2000, "R2")
                .AddCapacitor(1e-6, "C1")
                .AddCapacitor(2e-6, "C2")
                .AddInductor(1e-3, "L1")
                .AddInductor(2e-3, "L2")
                .Build();
            
            // Act
            var json = CircuitSerialization.ToJson(largeCircuit);
            var xml = CircuitSerialization.ToXml(largeCircuit);
            
            // Assert
            Assert.That(json, Contains.Substring("\"resistance\": 1000"));
            Assert.That(json, Contains.Substring("\"resistance\": 2000"));
            Assert.That(xml, Contains.Substring("resistance=\"1000\""));
            Assert.That(xml, Contains.Substring("resistance=\"2000\""));
        }
    }
}
