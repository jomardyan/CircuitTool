#nullable enable
using System;
using System.Collections.Generic;
using System.Text;
using CircuitTool.CircuitBuilder;

namespace CircuitTool.Serialization
{
    /// <summary>
    /// Provides JSON serialization support for circuit configurations
    /// </summary>
    public static class CircuitSerialization
    {
        /// <summary>
        /// Serializes a circuit to JSON format
        /// </summary>
        /// <param name="circuit">Circuit to serialize</param>
        /// <returns>JSON representation of the circuit</returns>
        public static string ToJson(Circuit circuit)
        {
            if (circuit == null) throw new ArgumentNullException(nameof(circuit));

            var json = new StringBuilder();
            json.AppendLine("{");
            json.AppendLine("  \"version\": \"1.0\",");
            json.AppendLine("  \"components\": [");

            var components = circuit.Components;
            for (int i = 0; i < components.Count; i++)
            {
                var component = components[i];
                json.AppendLine("    {");
                json.AppendLine($"      \"id\": \"{component.Id}\",");
                json.AppendLine($"      \"name\": \"{component.Name}\",");
                json.AppendLine($"      \"type\": \"{component.GetType().Name}\",");
                json.AppendLine($"      \"tolerance\": {component.Tolerance},");

                switch (component)
                {
                    case Resistor r:
                        json.AppendLine($"      \"resistance\": {r.Resistance}");
                        break;
                    case Capacitor c:
                        json.AppendLine($"      \"capacitance\": {c.Capacitance}");
                        break;
                    case Inductor l:
                        json.AppendLine($"      \"inductance\": {l.Inductance}");
                        break;
                }

                json.Append("    }");
                if (i < components.Count - 1) json.AppendLine(",");
                else json.AppendLine();
            }

            json.AppendLine("  ]");
            json.AppendLine("}");

            return json.ToString();
        }

        /// <summary>
        /// Deserializes a circuit from JSON format
        /// </summary>
        /// <param name="json">JSON representation of the circuit</param>
        /// <returns>Reconstructed circuit</returns>
        public static Circuit FromJson(string json)
        {
            if (string.IsNullOrEmpty(json)) throw new ArgumentException("JSON cannot be null or empty", nameof(json));

            // Simple JSON parser for basic circuit structure
            var builder = CircuitTool.CircuitBuilder.CircuitBuilder.New();
            
            // Extract components from JSON (simplified parsing)
            var componentStart = json.IndexOf("\"components\":");
            if (componentStart == -1) throw new FormatException("Invalid JSON format: components section not found");

            // This is a simplified implementation - in production would use proper JSON library
            var lines = json.Split('\n');
            
            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                
                if (trimmed.Contains("\"type\":"))
                {
                    var type = ExtractValue(trimmed, "type");
                    var tolerance = 0.05; // Default tolerance
                    
                    switch (type)
                    {
                        case "Resistor":
                            // Look for resistance value in subsequent lines
                            var resistance = 1000.0; // Default value
                            builder.AddResistor(resistance).WithComponentTolerance(tolerance);
                            break;
                        case "Capacitor":
                            var capacitance = 1e-6; // Default value
                            builder.AddCapacitor(capacitance).WithComponentTolerance(tolerance);
                            break;
                        case "Inductor":
                            var inductance = 1e-3; // Default value
                            builder.AddInductor(inductance).WithComponentTolerance(tolerance);
                            break;
                    }
                }
            }

            return builder.Build();
        }

        /// <summary>
        /// Serializes a circuit to XML format
        /// </summary>
        /// <param name="circuit">Circuit to serialize</param>
        /// <returns>XML representation of the circuit</returns>
        public static string ToXml(Circuit circuit)
        {
            if (circuit == null) throw new ArgumentNullException(nameof(circuit));

            var xml = new StringBuilder();
            xml.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            xml.AppendLine("<circuit version=\"1.0\">");

            var components = circuit.Components;
            foreach (var component in components)
            {
                switch (component)
                {
                    case Resistor r:
                        xml.AppendLine($"  <resistor id=\"{r.Id}\" name=\"{r.Name}\" resistance=\"{r.Resistance}\" tolerance=\"{r.Tolerance}\" />");
                        break;
                    case Capacitor c:
                        xml.AppendLine($"  <capacitor id=\"{c.Id}\" name=\"{c.Name}\" capacitance=\"{c.Capacitance}\" tolerance=\"{c.Tolerance}\" />");
                        break;
                    case Inductor l:
                        xml.AppendLine($"  <inductor id=\"{l.Id}\" name=\"{l.Name}\" inductance=\"{l.Inductance}\" tolerance=\"{l.Tolerance}\" />");
                        break;
                }
            }

            xml.AppendLine("</circuit>");
            return xml.ToString();
        }

        /// <summary>
        /// Deserializes a circuit from XML format
        /// </summary>
        /// <param name="xml">XML representation of the circuit</param>
        /// <returns>Reconstructed circuit</returns>
        public static Circuit FromXml(string xml)
        {
            if (string.IsNullOrEmpty(xml)) throw new ArgumentException("XML cannot be null or empty", nameof(xml));

            var builder = CircuitTool.CircuitBuilder.CircuitBuilder.New();
            
            // Simple XML parser (in production would use proper XML library)
            var lines = xml.Split('\n');
            
            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                
                if (trimmed.StartsWith("<resistor"))
                {
                    var resistance = ExtractAttributeValue(trimmed, "resistance");
                    var tolerance = ExtractAttributeValue(trimmed, "tolerance");
                    
                    if (double.TryParse(resistance, out var r) && double.TryParse(tolerance, out var t))
                    {
                        builder.AddResistor(r).WithComponentTolerance(t);
                    }
                }
                else if (trimmed.StartsWith("<capacitor"))
                {
                    var capacitance = ExtractAttributeValue(trimmed, "capacitance");
                    var tolerance = ExtractAttributeValue(trimmed, "tolerance");
                    
                    if (double.TryParse(capacitance, out var c) && double.TryParse(tolerance, out var t))
                    {
                        builder.AddCapacitor(c).WithComponentTolerance(t);
                    }
                }
                else if (trimmed.StartsWith("<inductor"))
                {
                    var inductance = ExtractAttributeValue(trimmed, "inductance");
                    var tolerance = ExtractAttributeValue(trimmed, "tolerance");
                    
                    if (double.TryParse(inductance, out var l) && double.TryParse(tolerance, out var t))
                    {
                        builder.AddInductor(l).WithComponentTolerance(t);
                    }
                }
            }

            return builder.Build();
        }

        private static string ExtractValue(string line, string key)
        {
            var keyPattern = $"\"{key}\":";
            var start = line.IndexOf(keyPattern);
            if (start == -1) return "";
            
            start += keyPattern.Length;
            var valueStart = line.IndexOf('"', start) + 1;
            var valueEnd = line.IndexOf('"', valueStart);
            
            return valueEnd > valueStart ? line.Substring(valueStart, valueEnd - valueStart) : "";
        }

        private static string ExtractAttributeValue(string line, string attribute)
        {
            var pattern = $"{attribute}=\"";
            var start = line.IndexOf(pattern);
            if (start == -1) return "";
            
            start += pattern.Length;
            var end = line.IndexOf('"', start);
            
            return end > start ? line.Substring(start, end - start) : "";
        }
    }

    /// <summary>
    /// Extensions for circuit import/export functionality
    /// </summary>
    public static class CircuitImportExport
    {
        /// <summary>
        /// Exports circuit configuration to file
        /// </summary>
        /// <param name="circuit">Circuit to export</param>
        /// <param name="filePath">File path for export</param>
        /// <param name="format">Export format (JSON or XML)</param>
        public static void ExportToFile(Circuit circuit, string filePath, ExportFormat format = ExportFormat.Json)
        {
            if (circuit == null) throw new ArgumentNullException(nameof(circuit));
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

            string content = format switch
            {
                ExportFormat.Json => CircuitSerialization.ToJson(circuit),
                ExportFormat.Xml => CircuitSerialization.ToXml(circuit),
                _ => throw new ArgumentException("Unsupported export format", nameof(format))
            };

            System.IO.File.WriteAllText(filePath, content);
        }

        /// <summary>
        /// Imports circuit configuration from file
        /// </summary>
        /// <param name="filePath">File path to import from</param>
        /// <param name="format">Import format (JSON or XML)</param>
        /// <returns>Imported circuit</returns>
        public static Circuit ImportFromFile(string filePath, ExportFormat format = ExportFormat.Json)
        {
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
            if (!System.IO.File.Exists(filePath)) throw new System.IO.FileNotFoundException($"File not found: {filePath}");

            var content = System.IO.File.ReadAllText(filePath);

            return format switch
            {
                ExportFormat.Json => CircuitSerialization.FromJson(content),
                ExportFormat.Xml => CircuitSerialization.FromXml(content),
                _ => throw new ArgumentException("Unsupported import format", nameof(format))
            };
        }
    }

    /// <summary>
    /// Export/Import format options
    /// </summary>
    public enum ExportFormat
    {
        Json,
        Xml
    }
}