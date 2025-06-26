#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CircuitTool
{
    /// <summary>
    /// Provides tools and calculators for various hardware communication protocols
    /// </summary>
    public static class CommunicationProtocolTools
    {
        /// <summary>
        /// I2C communication parameters
        /// </summary>
        public class I2CConfig
        {
            public int ClockFrequency { get; set; } = 100000; // 100kHz standard mode
            public double PullUpResistance { get; set; } = 4700; // 4.7kΩ
            public double SupplyVoltage { get; set; } = 3.3; // 3.3V or 5V
            public double BusCapacitance { get; set; } = 400e-12; // 400pF typical
            public int NumberOfDevices { get; set; } = 1;
        }

        /// <summary>
        /// SPI communication parameters
        /// </summary>
        public class SPIConfig
        {
            public int ClockFrequency { get; set; } = 1000000; // 1MHz
            public int DataBits { get; set; } = 8;
            public SPIMode Mode { get; set; } = SPIMode.Mode0;
            public double CableLength { get; set; } = 0.1; // 10cm typical
            public bool UseChipSelect { get; set; } = true;
        }

        /// <summary>
        /// SPI communication modes
        /// </summary>
        public enum SPIMode
        {
            Mode0 = 0, // CPOL=0, CPHA=0
            Mode1 = 1, // CPOL=0, CPHA=1
            Mode2 = 2, // CPOL=1, CPHA=0
            Mode3 = 3  // CPOL=1, CPHA=1
        }

        /// <summary>
        /// UART communication parameters
        /// </summary>
        public class UARTConfig
        {
            public int BaudRate { get; set; } = 115200;
            public int DataBits { get; set; } = 8;
            public int StopBits { get; set; } = 1;
            public string Parity { get; set; } = "None";
            public bool UseFlowControl { get; set; } = false;
            public double CableLength { get; set; } = 1.0; // 1 meter
        }

        /// <summary>
        /// Calculates optimal I2C pull-up resistor values
        /// </summary>
        /// <param name="config">I2C configuration</param>
        /// <returns>Recommended pull-up resistance range</returns>
        public static (double minResistance, double maxResistance, double recommendedResistance) 
            CalculateI2CPullUpResistors(I2CConfig config)
        {
            // Minimum resistance to ensure sufficient current for fast rise time
            double minResistance = 0.8 * config.SupplyVoltage / 0.003; // 3mA max current

            // Maximum resistance for proper logic levels and noise immunity
            double maxResistance = config.SupplyVoltage / (config.BusCapacitance * config.ClockFrequency * 3);

            // Recommended value based on number of devices and frequency
            double recommendedResistance = config.ClockFrequency switch
            {
                <= 100000 => 4700,  // Standard mode: 4.7kΩ
                <= 400000 => 2200,  // Fast mode: 2.2kΩ
                <= 1000000 => 1000, // Fast mode plus: 1kΩ
                _ => 470             // High speed: 470Ω
            };

            // Adjust for number of devices (more devices = lower resistance needed)
            if (config.NumberOfDevices > 3)
            {
                recommendedResistance *= 0.7;
            }

            return (minResistance, maxResistance, recommendedResistance);
        }

        /// <summary>
        /// Calculates I2C bus timing parameters
        /// </summary>
        /// <param name="config">I2C configuration</param>
        /// <returns>Timing parameters in nanoseconds</returns>
        public static (double setupTime, double holdTime, double riseTime, double fallTime) 
            CalculateI2CTiming(I2CConfig config)
        {
            double setupTime, holdTime, riseTime, fallTime;

            switch (config.ClockFrequency)
            {
                case <= 100000: // Standard mode
                    setupTime = 4700; // 4.7μs
                    holdTime = 4000;  // 4.0μs
                    riseTime = 1000;  // 1000ns max
                    fallTime = 300;   // 300ns max
                    break;

                case <= 400000: // Fast mode
                    setupTime = 600;  // 600ns
                    holdTime = 600;   // 600ns
                    riseTime = 300;   // 300ns max
                    fallTime = 300;   // 300ns max
                    break;

                case <= 1000000: // Fast mode plus
                    setupTime = 260;  // 260ns
                    holdTime = 260;   // 260ns
                    riseTime = 120;   // 120ns max
                    fallTime = 120;   // 120ns max
                    break;

                default: // High speed
                    setupTime = 160;  // 160ns
                    holdTime = 160;   // 160ns
                    riseTime = 80;    // 80ns max
                    fallTime = 80;    // 80ns max
                    break;
            }

            return (setupTime, holdTime, riseTime, fallTime);
        }

        /// <summary>
        /// Calculates maximum SPI clock frequency based on cable length and capacitance
        /// </summary>
        /// <param name="config">SPI configuration</param>
        /// <returns>Maximum recommended clock frequency</returns>
        public static double CalculateMaxSPIFrequency(SPIConfig config)
        {
            // Cable capacitance estimation (pF per meter)
            double cableCapacitancePerMeter = 100e-12; // 100pF/m typical
            double totalCapacitance = config.CableLength * cableCapacitancePerMeter + 10e-12; // Add device capacitance

            // Rise time estimation (assuming 50Ω impedance)
            double riseTime = 2.2 * 50 * totalCapacitance; // 2.2 * R * C

            // Maximum frequency (10% of rise time for clean edges)
            double maxFrequency = 0.1 / riseTime;

            // Practical limits based on microcontroller capabilities
            maxFrequency = Math.Min(maxFrequency, 50_000_000); // 50MHz practical limit

            return maxFrequency;
        }

        /// <summary>
        /// Calculates UART bit error rate based on clock accuracy
        /// </summary>
        /// <param name="config">UART configuration</param>
        /// <param name="clockAccuracyPpm">Clock accuracy in parts per million</param>
        /// <returns>Estimated bit error rate</returns>
        public static double CalculateUARTBitErrorRate(UARTConfig config, double clockAccuracyPpm = 100)
        {
            // Total bits per frame
            int totalBits = 1 + config.DataBits + config.StopBits; // Start + Data + Stop
            if (config.Parity != "None") totalBits++;

            // Cumulative timing error over frame
            double frameErrorPpm = clockAccuracyPpm * totalBits;

            // Convert to percentage error
            double frameErrorPercent = frameErrorPpm / 10000.0;

            // Approximate bit error rate (simplified model)
            double bitErrorRate = frameErrorPercent / 100.0;

            return Math.Min(bitErrorRate, 0.1); // Cap at 10%
        }

        /// <summary>
        /// Generates I2C device scanning code for Arduino
        /// </summary>
        /// <param name="startAddress">Start address for scanning (default 8)</param>
        /// <param name="endAddress">End address for scanning (default 119)</param>
        /// <returns>Arduino I2C scanner code</returns>
        public static string GenerateI2CScannerCode(int startAddress = 8, int endAddress = 119)
        {
            var code = new StringBuilder();

            code.AppendLine("// I2C Device Scanner for Arduino");
            code.AppendLine("// Generated by CircuitTool");
            code.AppendLine();
            code.AppendLine("#include <Wire.h>");
            code.AppendLine();
            code.AppendLine("void setup() {");
            code.AppendLine("  Serial.begin(115200);");
            code.AppendLine("  Wire.begin();");
            code.AppendLine("  Serial.println(\"I2C Device Scanner\");");
            code.AppendLine("  Serial.println(\"Scanning for devices...\");");
            code.AppendLine("  Serial.println();");
            code.AppendLine("}");
            code.AppendLine();
            code.AppendLine("void loop() {");
            code.AppendLine("  byte error, address;");
            code.AppendLine("  int deviceCount = 0;");
            code.AppendLine();
            code.AppendLine("  Serial.println(\"Scanning I2C bus...\");");
            code.AppendLine();
            code.AppendLine($"  for (address = {startAddress}; address <= {endAddress}; address++) {{");
            code.AppendLine("    Wire.beginTransmission(address);");
            code.AppendLine("    error = Wire.endTransmission();");
            code.AppendLine();
            code.AppendLine("    if (error == 0) {");
            code.AppendLine("      Serial.print(\"Device found at address 0x\");");
            code.AppendLine("      if (address < 16) Serial.print(\"0\");");
            code.AppendLine("      Serial.print(address, HEX);");
            code.AppendLine("      Serial.print(\" (\");");
            code.AppendLine("      Serial.print(address);");
            code.AppendLine("      Serial.println(\")\");");
            code.AppendLine("      deviceCount++;");
            code.AppendLine("      ");
            code.AppendLine("      // Identify common devices");
            code.AppendLine("      identifyDevice(address);");
            code.AppendLine("    }");
            code.AppendLine("    else if (error == 4) {");
            code.AppendLine("      Serial.print(\"Unknown error at address 0x\");");
            code.AppendLine("      if (address < 16) Serial.print(\"0\");");
            code.AppendLine("      Serial.println(address, HEX);");
            code.AppendLine("    }");
            code.AppendLine("  }");
            code.AppendLine();
            code.AppendLine("  if (deviceCount == 0) {");
            code.AppendLine("    Serial.println(\"No I2C devices found\");");
            code.AppendLine("  } else {");
            code.AppendLine("    Serial.print(\"Found \");");
            code.AppendLine("    Serial.print(deviceCount);");
            code.AppendLine("    Serial.println(\" device(s)\");");
            code.AppendLine("  }");
            code.AppendLine();
            code.AppendLine("  Serial.println(\"Done scanning. Waiting 5 seconds...\");");
            code.AppendLine("  Serial.println();");
            code.AppendLine("  delay(5000);");
            code.AppendLine("}");
            code.AppendLine();
            code.AppendLine("void identifyDevice(byte address) {");
            code.AppendLine("  switch (address) {");
            code.AppendLine("    case 0x20: case 0x21: case 0x22: case 0x23:");
            code.AppendLine("    case 0x24: case 0x25: case 0x26: case 0x27:");
            code.AppendLine("      Serial.println(\"  -> Likely MCP23008/MCP23017 I/O Expander\");");
            code.AppendLine("      break;");
            code.AppendLine("    case 0x48: case 0x49: case 0x4A: case 0x4B:");
            code.AppendLine("      Serial.println(\"  -> Likely ADS1115/ADS1015 ADC\");");
            code.AppendLine("      break;");
            code.AppendLine("    case 0x68: case 0x69:");
            code.AppendLine("      Serial.println(\"  -> Likely DS1307 RTC or MPU6050 IMU\");");
            code.AppendLine("      break;");
            code.AppendLine("    case 0x3C: case 0x3D:");
            code.AppendLine("      Serial.println(\"  -> Likely SSD1306 OLED Display\");");
            code.AppendLine("      break;");
            code.AppendLine("    case 0x76: case 0x77:");
            code.AppendLine("      Serial.println(\"  -> Likely BMP280/BME280 Sensor\");");
            code.AppendLine("      break;");
            code.AppendLine("    case 0x40: case 0x41: case 0x42: case 0x43:");
            code.AppendLine("      Serial.println(\"  -> Likely PCA9685 PWM Driver\");");
            code.AppendLine("      break;");
            code.AppendLine("    case 0x50: case 0x51: case 0x52: case 0x53:");
            code.AppendLine("    case 0x54: case 0x55: case 0x56: case 0x57:");
            code.AppendLine("      Serial.println(\"  -> Likely AT24C EEPROM\");");
            code.AppendLine("      break;");
            code.AppendLine("    default:");
            code.AppendLine("      Serial.println(\"  -> Unknown device type\");");
            code.AppendLine("      break;");
            code.AppendLine("  }");
            code.AppendLine("}");

            return code.ToString();
        }

        /// <summary>
        /// Generates SPI communication test code for Arduino
        /// </summary>
        /// <param name="config">SPI configuration</param>
        /// <param name="chipSelectPin">Chip select pin number</param>
        /// <returns>Arduino SPI test code</returns>
        public static string GenerateSPITestCode(SPIConfig config, int chipSelectPin = 10)
        {
            var code = new StringBuilder();

            code.AppendLine("// SPI Communication Test for Arduino");
            code.AppendLine("// Generated by CircuitTool");
            code.AppendLine();
            code.AppendLine("#include <SPI.h>");
            code.AppendLine();
            code.AppendLine($"const int CS_PIN = {chipSelectPin};");
            code.AppendLine();
            code.AppendLine("void setup() {");
            code.AppendLine("  Serial.begin(115200);");
            code.AppendLine("  pinMode(CS_PIN, OUTPUT);");
            code.AppendLine("  digitalWrite(CS_PIN, HIGH);");
            code.AppendLine();
            code.AppendLine("  SPI.begin();");
            code.AppendLine($"  SPI.setClockDivider(SPI_CLOCK_DIV{CalculateSPIClockDivider(config.ClockFrequency)});");
            code.AppendLine($"  SPI.setDataMode(SPI_MODE{(int)config.Mode});");
            code.AppendLine("  SPI.setBitOrder(MSBFIRST);");
            code.AppendLine();
            code.AppendLine("  Serial.println(\"SPI Communication Test\");");
            code.AppendLine($"  Serial.println(\"Clock: {config.ClockFrequency} Hz\");");
            code.AppendLine($"  Serial.println(\"Mode: {(int)config.Mode}\");");
            code.AppendLine($"  Serial.println(\"CS Pin: {chipSelectPin}\");");
            code.AppendLine("  Serial.println();");
            code.AppendLine("}");
            code.AppendLine();
            code.AppendLine("void loop() {");
            code.AppendLine("  // Test data transmission");
            code.AppendLine("  byte testData[] = {0x01, 0x02, 0x03, 0x04, 0x05};");
            code.AppendLine("  byte receivedData[5];");
            code.AppendLine();
            code.AppendLine("  digitalWrite(CS_PIN, LOW);");
            code.AppendLine("  delayMicroseconds(1);");
            code.AppendLine();
            code.AppendLine("  for (int i = 0; i < 5; i++) {");
            code.AppendLine("    receivedData[i] = SPI.transfer(testData[i]);");
            code.AppendLine("  }");
            code.AppendLine();
            code.AppendLine("  delayMicroseconds(1);");
            code.AppendLine("  digitalWrite(CS_PIN, HIGH);");
            code.AppendLine();
            code.AppendLine("  // Print results");
            code.AppendLine("  Serial.print(\"Sent: \");");
            code.AppendLine("  for (int i = 0; i < 5; i++) {");
            code.AppendLine("    Serial.print(\"0x\");");
            code.AppendLine("    if (testData[i] < 16) Serial.print(\"0\");");
            code.AppendLine("    Serial.print(testData[i], HEX);");
            code.AppendLine("    Serial.print(\" \");");
            code.AppendLine("  }");
            code.AppendLine("  Serial.println();");
            code.AppendLine();
            code.AppendLine("  Serial.print(\"Received: \");");
            code.AppendLine("  for (int i = 0; i < 5; i++) {");
            code.AppendLine("    Serial.print(\"0x\");");
            code.AppendLine("    if (receivedData[i] < 16) Serial.print(\"0\");");
            code.AppendLine("    Serial.print(receivedData[i], HEX);");
            code.AppendLine("    Serial.print(\" \");");
            code.AppendLine("  }");
            code.AppendLine("  Serial.println();");
            code.AppendLine("  Serial.println();");
            code.AppendLine();
            code.AppendLine("  delay(2000);");
            code.AppendLine("}");

            return code.ToString();
        }

        /// <summary>
        /// Calculates Arduino SPI clock divider for given frequency
        /// </summary>
        /// <param name="desiredFrequency">Desired SPI frequency</param>
        /// <returns>Clock divider value</returns>
        private static int CalculateSPIClockDivider(int desiredFrequency)
        {
            int systemClock = 16_000_000; // 16MHz Arduino
            int divider = systemClock / desiredFrequency;

            // Round to nearest valid divider
            return divider switch
            {
                <= 2 => 2,
                <= 4 => 4,
                <= 8 => 8,
                <= 16 => 16,
                <= 32 => 32,
                <= 64 => 64,
                _ => 128
            };
        }

        /// <summary>
        /// Generates protocol comparison report
        /// </summary>
        /// <param name="i2cConfig">I2C configuration</param>
        /// <param name="spiConfig">SPI configuration</param>
        /// <param name="uartConfig">UART configuration</param>
        /// <returns>Comparison report</returns>
        public static string GenerateProtocolComparison(I2CConfig? i2cConfig = null, 
            SPIConfig? spiConfig = null, UARTConfig? uartConfig = null)
        {
            i2cConfig ??= new I2CConfig();
            spiConfig ??= new SPIConfig();
            uartConfig ??= new UARTConfig();

            var report = new StringBuilder();

            report.AppendLine("=== Communication Protocol Comparison ===");
            report.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            report.AppendLine();

            // I2C Analysis
            report.AppendLine("I2C (Inter-Integrated Circuit):");
            report.AppendLine($"  Clock Frequency: {i2cConfig.ClockFrequency:N0} Hz");
            report.AppendLine($"  Bus Type: Multi-master, multi-slave");
            report.AppendLine($"  Wires Required: 2 (SDA, SCL)");
            report.AppendLine($"  Max Devices: 127 (7-bit addressing)");

            var (minR, maxR, recR) = CalculateI2CPullUpResistors(i2cConfig);
            report.AppendLine($"  Pull-up Resistor: {recR:F0}Ω (range: {minR:F0}-{maxR:F0}Ω)");

            var (setup, hold, rise, fall) = CalculateI2CTiming(i2cConfig);
            report.AppendLine($"  Timing - Setup: {setup:F0}ns, Hold: {hold:F0}ns");
            report.AppendLine();

            // SPI Analysis
            report.AppendLine("SPI (Serial Peripheral Interface):");
            report.AppendLine($"  Clock Frequency: {spiConfig.ClockFrequency:N0} Hz");
            report.AppendLine($"  Bus Type: Master-slave");
            report.AppendLine($"  Wires Required: 3+ (MOSI, MISO, SCK, CS)");
            report.AppendLine($"  Mode: {spiConfig.Mode}");

            double maxSpiFreq = CalculateMaxSPIFrequency(spiConfig);
            report.AppendLine($"  Max Frequency (cable limited): {maxSpiFreq:N0} Hz");
            report.AppendLine($"  Full Duplex: Yes");
            report.AppendLine();

            // UART Analysis
            report.AppendLine("UART (Universal Asynchronous Receiver-Transmitter):");
            report.AppendLine($"  Baud Rate: {uartConfig.BaudRate:N0} bps");
            report.AppendLine($"  Bus Type: Point-to-point");
            report.AppendLine($"  Wires Required: 2-4 (TX, RX, optional RTS/CTS)");
            report.AppendLine($"  Configuration: {uartConfig.DataBits}-{uartConfig.Parity}-{uartConfig.StopBits}");

            double bitErrorRate = CalculateUARTBitErrorRate(uartConfig);
            report.AppendLine($"  Estimated Bit Error Rate: {bitErrorRate:P3}");
            report.AppendLine($"  Flow Control: {(uartConfig.UseFlowControl ? "Yes" : "No")}");
            report.AppendLine();

            // Recommendations
            report.AppendLine("=== Recommendations ===");
            report.AppendLine("I2C: Best for multiple sensors, slow-medium speed, short distances");
            report.AppendLine("SPI: Best for high-speed data, displays, single device per CS");
            report.AppendLine("UART: Best for simple point-to-point, debugging, wireless modules");
            report.AppendLine();

            // Speed Comparison
            double i2cThroughput = i2cConfig.ClockFrequency * 0.8; // Account for protocol overhead
            double spiThroughput = spiConfig.ClockFrequency * 0.95; // Minimal overhead
            double uartThroughput = uartConfig.BaudRate * 0.8; // Account for start/stop bits

            report.AppendLine("Effective Throughput Comparison:");
            report.AppendLine($"  I2C: {i2cThroughput:N0} bps");
            report.AppendLine($"  SPI: {spiThroughput:N0} bps");
            report.AppendLine($"  UART: {uartThroughput:N0} bps");

            return report.ToString();
        }
    }
}
