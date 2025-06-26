#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CircuitTool
{
    /// <summary>
    /// Provides hardware debugging and troubleshooting tools for embedded systems
    /// </summary>
    public static class HardwareDebuggingTools
    {
        /// <summary>
        /// Common hardware problem categories
        /// </summary>
        public enum ProblemCategory
        {
            Power,
            Communication,
            Timing,
            Signal,
            Thermal,
            Mechanical,
            Software
        }

        /// <summary>
        /// Debugging test result
        /// </summary>
        public class DebugResult
        {
            public string TestName { get; set; } = "";
            public bool Passed { get; set; }
            public string Details { get; set; } = "";
            public string Recommendation { get; set; } = "";
            public ProblemCategory Category { get; set; }
        }

        /// <summary>
        /// Hardware configuration for debugging
        /// </summary>
        public class HardwareConfig
        {
            public double SupplyVoltage { get; set; } = 3.3;
            public double OperatingFrequency { get; set; } = 16_000_000;
            public double AmbientTemperature { get; set; } = 25.0;
            public List<string> ConnectedDevices { get; set; } = new List<string>();
            public bool HasCrystal { get; set; } = true;
            public bool HasBrownoutDetection { get; set; } = true;
        }

        /// <summary>
        /// Performs comprehensive hardware diagnostic tests
        /// </summary>
        /// <param name="config">Hardware configuration</param>
        /// <param name="symptoms">Observed symptoms</param>
        /// <returns>List of diagnostic results</returns>
        public static List<DebugResult> PerformDiagnostics(HardwareConfig config, List<string> symptoms)
        {
            var results = new List<DebugResult>();

            // Power supply tests
            results.AddRange(TestPowerSupply(config, symptoms));

            // Clock and timing tests
            results.AddRange(TestClockAndTiming(config, symptoms));

            // Communication tests
            results.AddRange(TestCommunication(config, symptoms));

            // Signal integrity tests
            results.AddRange(TestSignalIntegrity(config, symptoms));

            // Thermal tests
            results.AddRange(TestThermal(config, symptoms));

            return results;
        }

        /// <summary>
        /// Tests power supply related issues
        /// </summary>
        private static List<DebugResult> TestPowerSupply(HardwareConfig config, List<string> symptoms)
        {
            var results = new List<DebugResult>();

            // Voltage level test
            var voltageTest = new DebugResult
            {
                TestName = "Supply Voltage Level",
                Category = ProblemCategory.Power
            };

            if (config.SupplyVoltage < 2.7 || config.SupplyVoltage > 5.5)
            {
                voltageTest.Passed = false;
                voltageTest.Details = $"Supply voltage {config.SupplyVoltage}V is outside typical range (2.7V-5.5V)";
                voltageTest.Recommendation = "Check power supply regulation and connections. Verify no voltage drops across connectors.";
            }
            else if (config.SupplyVoltage < 3.0 || config.SupplyVoltage > 5.2)
            {
                voltageTest.Passed = false;
                voltageTest.Details = $"Supply voltage {config.SupplyVoltage}V may be marginal";
                voltageTest.Recommendation = "Consider using regulated power supply. Check for voltage ripple.";
            }
            else
            {
                voltageTest.Passed = true;
                voltageTest.Details = $"Supply voltage {config.SupplyVoltage}V is within acceptable range";
                voltageTest.Recommendation = "Voltage level appears normal.";
            }

            results.Add(voltageTest);

            // Brownout detection test
            if (symptoms.Any(s => s.ToLower().Contains("random reset") || s.ToLower().Contains("unexpected restart")))
            {
                var brownoutTest = new DebugResult
                {
                    TestName = "Brownout Detection",
                    Category = ProblemCategory.Power,
                    Passed = config.HasBrownoutDetection,
                    Details = config.HasBrownoutDetection 
                        ? "Brownout detection is enabled - good for detecting power issues"
                        : "Brownout detection is disabled - may cause unexpected resets",
                    Recommendation = config.HasBrownoutDetection
                        ? "Check power supply stability and capacitor values"
                        : "Enable brownout detection to help diagnose power-related resets"
                };
                results.Add(brownoutTest);
            }

            return results;
        }

        /// <summary>
        /// Tests clock and timing related issues
        /// </summary>
        private static List<DebugResult> TestClockAndTiming(HardwareConfig config, List<string> symptoms)
        {
            var results = new List<DebugResult>();

            // Crystal oscillator test
            if (config.HasCrystal)
            {
                var crystalTest = new DebugResult
                {
                    TestName = "Crystal Oscillator",
                    Category = ProblemCategory.Timing
                };

                if (symptoms.Any(s => s.ToLower().Contains("timing") || s.ToLower().Contains("baud") || s.ToLower().Contains("communication")))
                {
                    crystalTest.Passed = false;
                    crystalTest.Details = "Timing-related symptoms detected with crystal oscillator";
                    crystalTest.Recommendation = "Check crystal connections, load capacitors (typically 15-22pF), and ensure no broken traces.";
                }
                else
                {
                    crystalTest.Passed = true;
                    crystalTest.Details = "Crystal oscillator configuration appears normal";
                    crystalTest.Recommendation = "Monitor for timing accuracy if precision is critical.";
                }

                results.Add(crystalTest);
            }

            // Frequency range test
            var frequencyTest = new DebugResult
            {
                TestName = "Operating Frequency",
                Category = ProblemCategory.Timing,
                Details = $"Operating at {config.OperatingFrequency / 1_000_000:F1} MHz"
            };

            if (config.OperatingFrequency > 100_000_000)
            {
                frequencyTest.Passed = false;
                frequencyTest.Recommendation = "High frequency operation requires careful PCB design, proper grounding, and decoupling.";
            }
            else
            {
                frequencyTest.Passed = true;
                frequencyTest.Recommendation = "Operating frequency is within normal range for most microcontrollers.";
            }

            results.Add(frequencyTest);

            return results;
        }

        /// <summary>
        /// Tests communication related issues
        /// </summary>
        private static List<DebugResult> TestCommunication(HardwareConfig config, List<string> symptoms)
        {
            var results = new List<DebugResult>();

            var commSymptoms = symptoms.Where(s => 
                s.ToLower().Contains("i2c") || 
                s.ToLower().Contains("spi") || 
                s.ToLower().Contains("uart") || 
                s.ToLower().Contains("serial") ||
                s.ToLower().Contains("communication")).ToList();

            if (commSymptoms.Any())
            {
                var commTest = new DebugResult
                {
                    TestName = "Communication Protocol",
                    Category = ProblemCategory.Communication,
                    Passed = false,
                    Details = $"Communication issues reported: {string.Join(", ", commSymptoms)}"
                };

                var recommendations = new List<string>();

                if (commSymptoms.Any(s => s.ToLower().Contains("i2c")))
                {
                    recommendations.Add("I2C: Check pull-up resistors (2.2kΩ-10kΩ), verify addresses, check for bus conflicts");
                }

                if (commSymptoms.Any(s => s.ToLower().Contains("spi")))
                {
                    recommendations.Add("SPI: Verify MOSI/MISO connections, check clock polarity/phase, ensure proper CS timing");
                }

                if (commSymptoms.Any(s => s.ToLower().Contains("uart") || s.ToLower().Contains("serial")))
                {
                    recommendations.Add("UART: Check baud rate accuracy, verify TX/RX connections, ensure proper ground reference");
                }

                commTest.Recommendation = string.Join(". ", recommendations);
                results.Add(commTest);
            }

            return results;
        }

        /// <summary>
        /// Tests signal integrity issues
        /// </summary>
        private static List<DebugResult> TestSignalIntegrity(HardwareConfig config, List<string> symptoms)
        {
            var results = new List<DebugResult>();

            var signalSymptoms = symptoms.Where(s => 
                s.ToLower().Contains("noise") || 
                s.ToLower().Contains("glitch") || 
                s.ToLower().Contains("interference") ||
                s.ToLower().Contains("unstable")).ToList();

            if (signalSymptoms.Any())
            {
                var signalTest = new DebugResult
                {
                    TestName = "Signal Integrity",
                    Category = ProblemCategory.Signal,
                    Passed = false,
                    Details = $"Signal integrity issues: {string.Join(", ", signalSymptoms)}",
                    Recommendation = "Add decoupling capacitors (0.1μF ceramic + 10μF electrolytic), improve grounding, check for proper PCB layout"
                };

                results.Add(signalTest);
            }

            // High frequency signal integrity test
            if (config.OperatingFrequency > 20_000_000)
            {
                var hfTest = new DebugResult
                {
                    TestName = "High Frequency Signal Integrity",
                    Category = ProblemCategory.Signal,
                    Passed = config.OperatingFrequency <= 50_000_000,
                    Details = $"Operating at {config.OperatingFrequency / 1_000_000:F1} MHz",
                    Recommendation = "Use ground planes, minimize trace lengths, add termination if needed, consider impedance matching"
                };

                results.Add(hfTest);
            }

            return results;
        }

        /// <summary>
        /// Tests thermal related issues
        /// </summary>
        private static List<DebugResult> TestThermal(HardwareConfig config, List<string> symptoms)
        {
            var results = new List<DebugResult>();

            var thermalSymptoms = symptoms.Where(s => 
                s.ToLower().Contains("hot") || 
                s.ToLower().Contains("thermal") || 
                s.ToLower().Contains("temperature") ||
                s.ToLower().Contains("overheating")).ToList();

            var thermalTest = new DebugResult
            {
                TestName = "Thermal Management",
                Category = ProblemCategory.Thermal
            };

            if (thermalSymptoms.Any() || config.AmbientTemperature > 60)
            {
                thermalTest.Passed = false;
                thermalTest.Details = $"Thermal issues detected. Ambient: {config.AmbientTemperature}°C";
                thermalTest.Recommendation = "Add heat sinks, improve ventilation, check for short circuits, reduce power consumption";
            }
            else if (config.AmbientTemperature > 40)
            {
                thermalTest.Passed = false;
                thermalTest.Details = $"High ambient temperature: {config.AmbientTemperature}°C";
                thermalTest.Recommendation = "Monitor component temperatures, consider thermal derating";
            }
            else
            {
                thermalTest.Passed = true;
                thermalTest.Details = $"Ambient temperature {config.AmbientTemperature}°C is within normal range";
                thermalTest.Recommendation = "Thermal environment appears normal";
            }

            results.Add(thermalTest);

            return results;
        }

        /// <summary>
        /// Generates Arduino diagnostic sketch
        /// </summary>
        /// <param name="includeTests">List of tests to include</param>
        /// <returns>Arduino diagnostic code</returns>
        public static string GenerateArduinoDiagnosticSketch(List<string>? includeTests = null)
        {
            includeTests ??= new List<string> { "power", "timing", "memory", "pins" };

            var code = new StringBuilder();

            code.AppendLine("// Arduino Hardware Diagnostic Sketch");
            code.AppendLine("// Generated by CircuitTool");
            code.AppendLine();
            code.AppendLine("void setup() {");
            code.AppendLine("  Serial.begin(115200);");
            code.AppendLine("  while (!Serial) delay(10);");
            code.AppendLine("  ");
            code.AppendLine("  Serial.println(\"===== Arduino Hardware Diagnostics =====\");");
            code.AppendLine("  Serial.println();");
            code.AppendLine("  ");

            if (includeTests.Contains("power"))
            {
                code.AppendLine("  // Power supply diagnostics");
                code.AppendLine("  runPowerDiagnostics();");
                code.AppendLine("  ");
            }

            if (includeTests.Contains("timing"))
            {
                code.AppendLine("  // Timing diagnostics");
                code.AppendLine("  runTimingDiagnostics();");
                code.AppendLine("  ");
            }

            if (includeTests.Contains("memory"))
            {
                code.AppendLine("  // Memory diagnostics");
                code.AppendLine("  runMemoryDiagnostics();");
                code.AppendLine("  ");
            }

            if (includeTests.Contains("pins"))
            {
                code.AppendLine("  // Pin diagnostics");
                code.AppendLine("  runPinDiagnostics();");
                code.AppendLine("  ");
            }

            code.AppendLine("  Serial.println(\"===== Diagnostics Complete =====\");");
            code.AppendLine("}");
            code.AppendLine();
            code.AppendLine("void loop() {");
            code.AppendLine("  // Continuous monitoring");
            code.AppendLine("  delay(5000);");
            code.AppendLine("  ");
            code.AppendLine("  Serial.println(\"System Status:\");");
            code.AppendLine("  Serial.print(\"Uptime: \");");
            code.AppendLine("  Serial.print(millis() / 1000);");
            code.AppendLine("  Serial.println(\" seconds\");");
            code.AppendLine("  Serial.print(\"Free RAM: \");");
            code.AppendLine("  Serial.print(freeRam());");
            code.AppendLine("  Serial.println(\" bytes\");");
            code.AppendLine("  Serial.println();");
            code.AppendLine("}");
            code.AppendLine();

            if (includeTests.Contains("power"))
            {
                code.AppendLine("void runPowerDiagnostics() {");
                code.AppendLine("  Serial.println(\"=== Power Supply Diagnostics ===\");");
                code.AppendLine("  ");
                code.AppendLine("  // Read internal voltage reference");
                code.AppendLine("  float vcc = readVcc() / 1000.0;");
                code.AppendLine("  Serial.print(\"VCC Voltage: \");");
                code.AppendLine("  Serial.print(vcc);");
                code.AppendLine("  Serial.println(\"V\");");
                code.AppendLine("  ");
                code.AppendLine("  if (vcc < 3.0) {");
                code.AppendLine("    Serial.println(\"WARNING: Low supply voltage detected!\");");
                code.AppendLine("  } else if (vcc > 5.5) {");
                code.AppendLine("    Serial.println(\"WARNING: High supply voltage detected!\");");
                code.AppendLine("  } else {");
                code.AppendLine("    Serial.println(\"Supply voltage is within normal range.\");");
                code.AppendLine("  }");
                code.AppendLine("  Serial.println();");
                code.AppendLine("}");
                code.AppendLine();
                code.AppendLine("long readVcc() {");
                code.AppendLine("  long result;");
                code.AppendLine("  // Read 1.1V reference against AVcc");
                code.AppendLine("  ADMUX = _BV(REFS0) | _BV(MUX3) | _BV(MUX2) | _BV(MUX1);");
                code.AppendLine("  delay(2);");
                code.AppendLine("  ADCSRA |= _BV(ADSC);");
                code.AppendLine("  while (bit_is_set(ADCSRA,ADSC));");
                code.AppendLine("  result = ADCL;");
                code.AppendLine("  result |= ADCH<<8;");
                code.AppendLine("  result = 1125300L / result;");
                code.AppendLine("  return result;");
                code.AppendLine("}");
                code.AppendLine();
            }

            if (includeTests.Contains("timing"))
            {
                code.AppendLine("void runTimingDiagnostics() {");
                code.AppendLine("  Serial.println(\"=== Timing Diagnostics ===\");");
                code.AppendLine("  ");
                code.AppendLine("  unsigned long start = micros();");
                code.AppendLine("  delay(1000);");
                code.AppendLine("  unsigned long elapsed = micros() - start;");
                code.AppendLine("  ");
                code.AppendLine("  Serial.print(\"1 second delay actual time: \");");
                code.AppendLine("  Serial.print(elapsed);");
                code.AppendLine("  Serial.println(\" microseconds\");");
                code.AppendLine("  ");
                code.AppendLine("  float accuracy = (float)elapsed / 1000000.0;");
                code.AppendLine("  Serial.print(\"Timing accuracy: \");");
                code.AppendLine("  Serial.print(accuracy * 100);");
                code.AppendLine("  Serial.println(\"%\");");
                code.AppendLine("  ");
                code.AppendLine("  if (accuracy < 0.99 || accuracy > 1.01) {");
                code.AppendLine("    Serial.println(\"WARNING: Timing inaccuracy detected!\");");
                code.AppendLine("  } else {");
                code.AppendLine("    Serial.println(\"Timing accuracy is acceptable.\");");
                code.AppendLine("  }");
                code.AppendLine("  Serial.println();");
                code.AppendLine("}");
                code.AppendLine();
            }

            if (includeTests.Contains("memory"))
            {
                code.AppendLine("void runMemoryDiagnostics() {");
                code.AppendLine("  Serial.println(\"=== Memory Diagnostics ===\");");
                code.AppendLine("  ");
                code.AppendLine("  int freeMemory = freeRam();");
                code.AppendLine("  Serial.print(\"Free SRAM: \");");
                code.AppendLine("  Serial.print(freeMemory);");
                code.AppendLine("  Serial.println(\" bytes\");");
                code.AppendLine("  ");
                code.AppendLine("  if (freeMemory < 256) {");
                code.AppendLine("    Serial.println(\"WARNING: Low memory available!\");");
                code.AppendLine("  } else {");
                code.AppendLine("    Serial.println(\"Memory usage appears normal.\");");
                code.AppendLine("  }");
                code.AppendLine("  Serial.println();");
                code.AppendLine("}");
                code.AppendLine();
            }

            if (includeTests.Contains("pins"))
            {
                code.AppendLine("void runPinDiagnostics() {");
                code.AppendLine("  Serial.println(\"=== Pin Diagnostics ===\");");
                code.AppendLine("  ");
                code.AppendLine("  // Test digital pins for shorts to ground or VCC");
                code.AppendLine("  for (int pin = 2; pin <= 13; pin++) {");
                code.AppendLine("    pinMode(pin, INPUT_PULLUP);");
                code.AppendLine("    delay(1);");
                code.AppendLine("    int highState = digitalRead(pin);");
                code.AppendLine("    ");
                code.AppendLine("    pinMode(pin, INPUT);");
                code.AppendLine("    delay(1);");
                code.AppendLine("    int floatState = digitalRead(pin);");
                code.AppendLine("    ");
                code.AppendLine("    Serial.print(\"Pin \");");
                code.AppendLine("    Serial.print(pin);");
                code.AppendLine("    Serial.print(\": \");");
                code.AppendLine("    ");
                code.AppendLine("    if (!highState) {");
                code.AppendLine("      Serial.println(\"SHORTED TO GROUND!\");");
                code.AppendLine("    } else {");
                code.AppendLine("      Serial.println(\"OK\");");
                code.AppendLine("    }");
                code.AppendLine("  }");
                code.AppendLine("  Serial.println();");
                code.AppendLine("}");
                code.AppendLine();
            }

            code.AppendLine("int freeRam() {");
            code.AppendLine("  extern int __heap_start, *__brkval;");
            code.AppendLine("  int v;");
            code.AppendLine("  return (int) &v - (__brkval == 0 ? (int) &__heap_start : (int) __brkval);");
            code.AppendLine("}");

            return code.ToString();
        }

        /// <summary>
        /// Generates troubleshooting guide based on symptoms
        /// </summary>
        /// <param name="symptoms">List of observed symptoms</param>
        /// <returns>Troubleshooting guide</returns>
        public static string GenerateTroubleshootingGuide(List<string> symptoms)
        {
            var guide = new StringBuilder();

            guide.AppendLine("=== Hardware Troubleshooting Guide ===");
            guide.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            guide.AppendLine();

            if (symptoms.Count == 0)
            {
                guide.AppendLine("No specific symptoms provided. Here's a general troubleshooting approach:");
                guide.AppendLine();
                guide.AppendLine("1. POWER SUPPLY CHECK:");
                guide.AppendLine("   - Verify correct voltage levels");
                guide.AppendLine("   - Check for voltage ripple");
                guide.AppendLine("   - Ensure adequate current capacity");
                guide.AppendLine();
                guide.AppendLine("2. CONNECTIONS:");
                guide.AppendLine("   - Verify all connections are secure");
                guide.AppendLine("   - Check for cold solder joints");
                guide.AppendLine("   - Test continuity with multimeter");
                guide.AppendLine();
                guide.AppendLine("3. COMPONENT INTEGRITY:");
                guide.AppendLine("   - Check for physical damage");
                guide.AppendLine("   - Verify proper component orientation");
                guide.AppendLine("   - Test critical components individually");
                
                return guide.ToString();
            }

            guide.AppendLine($"Analyzing {symptoms.Count} reported symptom(s):");
            foreach (var symptom in symptoms)
            {
                guide.AppendLine($"  - {symptom}");
            }
            guide.AppendLine();

            // Categorize symptoms and provide specific guidance
            var powerSymptoms = symptoms.Where(s => 
                s.ToLower().Contains("power") || 
                s.ToLower().Contains("reset") || 
                s.ToLower().Contains("restart") ||
                s.ToLower().Contains("voltage")).ToList();

            var commSymptoms = symptoms.Where(s => 
                s.ToLower().Contains("communication") || 
                s.ToLower().Contains("i2c") || 
                s.ToLower().Contains("spi") ||
                s.ToLower().Contains("uart") ||
                s.ToLower().Contains("serial")).ToList();

            var timingSymptoms = symptoms.Where(s => 
                s.ToLower().Contains("timing") || 
                s.ToLower().Contains("clock") || 
                s.ToLower().Contains("baud") ||
                s.ToLower().Contains("frequency")).ToList();

            if (powerSymptoms.Any())
            {
                guide.AppendLine("POWER SUPPLY ISSUES DETECTED:");
                guide.AppendLine("1. Check supply voltage with multimeter");
                guide.AppendLine("2. Verify power connections are secure");
                guide.AppendLine("3. Add/check decoupling capacitors (0.1μF + 10μF)");
                guide.AppendLine("4. Enable brownout detection if available");
                guide.AppendLine("5. Check for ground loops or poor grounding");
                guide.AppendLine();
            }

            if (commSymptoms.Any())
            {
                guide.AppendLine("COMMUNICATION ISSUES DETECTED:");
                guide.AppendLine("1. Verify correct pin connections (TX↔RX for UART)");
                guide.AppendLine("2. Check pull-up resistors for I2C (2.2kΩ-10kΩ)");
                guide.AppendLine("3. Verify baud rates match exactly");
                guide.AppendLine("4. Use oscilloscope to check signal integrity");
                guide.AppendLine("5. Check for address conflicts (I2C)");
                guide.AppendLine("6. Verify SPI clock polarity and phase settings");
                guide.AppendLine();
            }

            if (timingSymptoms.Any())
            {
                guide.AppendLine("TIMING ISSUES DETECTED:");
                guide.AppendLine("1. Check crystal oscillator connections");
                guide.AppendLine("2. Verify load capacitors (typically 15-22pF)");
                guide.AppendLine("3. Check for broken traces near crystal");
                guide.AppendLine("4. Measure actual clock frequency with oscilloscope");
                guide.AppendLine("5. Consider using internal oscillator for testing");
                guide.AppendLine();
            }

            // General recommendations
            guide.AppendLine("GENERAL DEBUGGING STEPS:");
            guide.AppendLine("1. Use a logic analyzer or oscilloscope for signal analysis");
            guide.AppendLine("2. Check all solder joints under magnification");
            guide.AppendLine("3. Verify component values and orientations");
            guide.AppendLine("4. Test with minimal circuit first, then add complexity");
            guide.AppendLine("5. Check PCB layout for proper grounding and signal routing");
            guide.AppendLine("6. Consider EMI/EMC issues if intermittent problems occur");
            guide.AppendLine();

            guide.AppendLine("TOOLS RECOMMENDED:");
            guide.AppendLine("- Digital multimeter");
            guide.AppendLine("- Oscilloscope (2+ channels)");
            guide.AppendLine("- Logic analyzer");
            guide.AppendLine("- Magnifying glass or microscope");
            guide.AppendLine("- Function generator (for testing)");

            return guide.ToString();
        }
    }
}
