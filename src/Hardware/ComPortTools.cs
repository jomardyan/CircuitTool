#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#if NET462 || NET6_0 || NET8_0 || NETSTANDARD2_0 || NETSTANDARD2_1
using System.IO.Ports;
#endif

namespace CircuitTool
{
    /// <summary>
    /// Provides comprehensive COM port utilities for hardware communication and monitoring
    /// </summary>
    public static class ComPortTools
    {
        /// <summary>
        /// Common baud rates for serial communication
        /// </summary>
        public static readonly int[] CommonBaudRates = 
        {
            300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 28800, 38400, 
            57600, 115200, 230400, 460800, 921600, 1000000, 2000000
        };

        /// <summary>
        /// Arduino standard baud rates
        /// </summary>
        public static readonly int[] ArduinoBaudRates = 
        {
            9600, 19200, 38400, 57600, 115200
        };

        /// <summary>
        /// ESP32 standard baud rates
        /// </summary>
        public static readonly int[] ESP32BaudRates = 
        {
            9600, 19200, 38400, 57600, 115200, 230400, 460800, 921600
        };

#if NET462 || NET6_0 || NET8_0 || NETSTANDARD2_0 || NETSTANDARD2_1
        /// <summary>
        /// Serial port parity options
        /// </summary>
        public enum SerialParity
        {
            None = 0,
            Odd = 1,
            Even = 2,
            Mark = 3,
            Space = 4
        }

        /// <summary>
        /// Serial port stop bits options
        /// </summary>
        public enum SerialStopBits
        {
            None = 0,
            One = 1,
            Two = 2,
            OnePointFive = 3
        }

        /// <summary>
        /// Serial port handshake options
        /// </summary>
        public enum SerialHandshake
        {
            None = 0,
            XOnXOff = 1,
            RequestToSend = 2,
            RequestToSendXOnXOff = 3
        }
#else
        /// <summary>
        /// Serial port parity options (compatibility for older frameworks)
        /// </summary>
        public enum SerialParity
        {
            None = 0,
            Odd = 1,
            Even = 2,
            Mark = 3,
            Space = 4
        }

        /// <summary>
        /// Serial port stop bits options (compatibility for older frameworks)
        /// </summary>
        public enum SerialStopBits
        {
            None = 0,
            One = 1,
            Two = 2,
            OnePointFive = 3
        }

        /// <summary>
        /// Serial port handshake options (compatibility for older frameworks)
        /// </summary>
        public enum SerialHandshake
        {
            None = 0,
            XOnXOff = 1,
            RequestToSend = 2,
            RequestToSendXOnXOff = 3
        }
#endif

        /// <summary>
        /// Serial port configuration
        /// </summary>
        public class SerialConfig
        {
            public string PortName { get; set; } = "";
            public int BaudRate { get; set; } = 115200;
            public SerialParity Parity { get; set; } = SerialParity.None;
            public int DataBits { get; set; } = 8;
            public SerialStopBits StopBits { get; set; } = SerialStopBits.One;
            public SerialHandshake Handshake { get; set; } = SerialHandshake.None;
            public int ReadTimeout { get; set; } = 5000;
            public int WriteTimeout { get; set; } = 5000;
        }

        /// <summary>
        /// Serial port information
        /// </summary>
        public class SerialPortInfo
        {
            public string PortName { get; set; } = "";
            public string Description { get; set; } = "";
            public string Manufacturer { get; set; } = "";
            public string DeviceID { get; set; } = "";
            public bool IsAvailable { get; set; }
            public string DeviceType { get; set; } = "Unknown";
        }

        /// <summary>
        /// Gets all available COM ports with detailed information
        /// </summary>
        /// <returns>List of available serial ports</returns>
        public static List<SerialPortInfo> GetAvailablePorts()
        {
            var ports = new List<SerialPortInfo>();
            
            try
            {
#if NET462 || NET6_0 || NET8_0 || NETSTANDARD2_0 || NETSTANDARD2_1
                string[] portNames = SerialPort.GetPortNames();
                
                foreach (string portName in portNames)
                {
                    var portInfo = new SerialPortInfo
                    {
                        PortName = portName,
                        IsAvailable = true
                    };
                    
                    // Try to get additional information (Windows specific)
                    try
                    {
                        // Basic port information
                        portInfo.Description = $"Serial Port ({portName})";
                        
                        // Detect common device types based on port name or description
                        portInfo.DeviceType = DetectDeviceType(portName, portInfo.Description);
                    }
                    catch
                    {
                        // If we can't get detailed info, just use basic info
                    }
                    
                    ports.Add(portInfo);
                }
#else
                // For .NET Framework 4.5, provide basic COM port detection
                for (int i = 1; i <= 20; i++)
                {
                    string portName = $"COM{i}";
                    var portInfo = new SerialPortInfo
                    {
                        PortName = portName,
                        Description = $"Serial Port ({portName})",
                        DeviceType = "Unknown",
                        IsAvailable = true
                    };
                    ports.Add(portInfo);
                }
#endif
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting COM ports: {ex.Message}");
            }
            
            return ports.OrderBy(p => p.PortName).ToList();
        }

        /// <summary>
        /// Detects likely device type based on port information
        /// </summary>
        private static string DetectDeviceType(string portName, string description)
        {
            string combined = $"{portName} {description}".ToLower();
            
            if (combined.Contains("arduino") || combined.Contains("ch340") || combined.Contains("ch341"))
                return "Arduino";
            else if (combined.Contains("esp32") || combined.Contains("cp210") || combined.Contains("silicon labs"))
                return "ESP32";
            else if (combined.Contains("ftdi") || combined.Contains("ft232"))
                return "FTDI";
            else if (combined.Contains("prolific") || combined.Contains("pl2303"))
                return "USB-Serial";
            else if (combined.Contains("bluetooth"))
                return "Bluetooth";
            else
                return "Unknown";
        }

        /// <summary>
        /// Tests if a port is available and responsive
        /// </summary>
        /// <param name="portName">COM port name</param>
        /// <param name="baudRate">Baud rate to test</param>
        /// <param name="timeoutMs">Timeout in milliseconds</param>
        /// <returns>True if port is available and responsive</returns>
        public static bool TestPortAvailability(string portName, int baudRate = 115200, int timeoutMs = 1000)
        {
#if NET462 || NET6_0 || NET8_0 || NETSTANDARD2_0 || NETSTANDARD2_1
            try
            {
                using (var port = new SerialPort(portName, baudRate))
                {
                    port.ReadTimeout = timeoutMs;
                    port.WriteTimeout = timeoutMs;
                    
                    port.Open();
                    Thread.Sleep(100); // Give port time to initialize
                    
                    // Try to read any existing data
                    if (port.BytesToRead > 0)
                    {
                        port.ReadExisting();
                    }
                    
                    port.Close();
                    return true;
                }
            }
            catch
            {
                return false;
            }
#else
            // For .NET Framework 4.5, return basic availability check
            return true;
#endif
        }

#if NET462 || NET6_0 || NET8_0 || NETSTANDARD2_0 || NETSTANDARD2_1
        /// <summary>
        /// Converts custom parity enum to System.IO.Ports.Parity
        /// </summary>
        private static Parity ConvertParity(SerialParity parity)
        {
            return (Parity)(int)parity;
        }

        /// <summary>
        /// Converts custom stop bits enum to System.IO.Ports.StopBits
        /// </summary>
        private static StopBits ConvertStopBits(SerialStopBits stopBits)
        {
            return (StopBits)(int)stopBits;
        }

        /// <summary>
        /// Converts custom handshake enum to System.IO.Ports.Handshake
        /// </summary>
        private static Handshake ConvertHandshake(SerialHandshake handshake)
        {
            return (Handshake)(int)handshake;
        }
#endif

        /// <summary>
        /// Auto-detects the correct baud rate for a device
        /// </summary>
        /// <param name="portName">COM port name</param>
        /// <param name="testBaudRates">Array of baud rates to test</param>
        /// <param name="testTimeMs">Time to test each baud rate</param>
        /// <returns>Detected baud rate or -1 if none found</returns>
        public static int AutoDetectBaudRate(string portName, int[]? testBaudRates = null, int testTimeMs = 2000)
        {
#if NET462 || NET6_0 || NET8_0 || NETSTANDARD2_0 || NETSTANDARD2_1
            testBaudRates ??= CommonBaudRates;
            
            foreach (int baudRate in testBaudRates.OrderByDescending(b => b))
            {
                try
                {
                    using (var port = new SerialPort(portName, baudRate))
                    {
                        port.ReadTimeout = 500;
                        port.Open();
                        
                        var startTime = DateTime.Now;
                        int dataReceived = 0;
                        
                        while ((DateTime.Now - startTime).TotalMilliseconds < testTimeMs)
                        {
                            if (port.BytesToRead > 0)
                            {
                                port.ReadExisting();
                                dataReceived++;
                                
                                if (dataReceived > 3) // Got consistent data
                                {
                                    port.Close();
                                    return baudRate;
                                }
                            }
                            Thread.Sleep(100);
                        }
                        
                        port.Close();
                    }
                }
                catch
                {
                    // Try next baud rate
                }
            }
            
            return -1; // No baud rate detected
#else
            // For .NET Framework 4.5, return default baud rate
            return 115200;
#endif
        }

        /// <summary>
        /// Monitors a COM port and logs data
        /// </summary>
        /// <param name="config">Serial port configuration</param>
        /// <param name="durationSeconds">Duration to monitor in seconds</param>
        /// <param name="dataCallback">Callback for received data</param>
        /// <returns>Monitoring results</returns>
        public static string MonitorPort(SerialConfig config, int durationSeconds = 30, 
            Action<string>? dataCallback = null)
        {
            var log = new StringBuilder();
            var startTime = DateTime.Now;
            
            try
            {
#if NET462 || NET6_0 || NET8_0 || NETSTANDARD2_0 || NETSTANDARD2_1
                using (var port = new SerialPort(config.PortName, config.BaudRate, ConvertParity(config.Parity), 
                    config.DataBits, ConvertStopBits(config.StopBits)))
                {
                    port.Handshake = ConvertHandshake(config.Handshake);
                    port.ReadTimeout = 1000;
                    port.WriteTimeout = config.WriteTimeout;
                    
                    port.Open();
                    log.AppendLine($"=== COM Port Monitor Started ===");
                    log.AppendLine($"Port: {config.PortName}");
                    log.AppendLine($"Baud Rate: {config.BaudRate}");
                    log.AppendLine($"Configuration: {config.DataBits}{config.Parity.ToString()[0]}{(int)config.StopBits}");
                    log.AppendLine($"Start Time: {startTime:yyyy-MM-dd HH:mm:ss}");
                    log.AppendLine("=====================================");
                    
                    while ((DateTime.Now - startTime).TotalSeconds < durationSeconds)
                    {
                        try
                        {
                            if (port.BytesToRead > 0)
                            {
                                string data = port.ReadExisting();
                                string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
                                string logEntry = $"[{timestamp}] {data}";
                                
                                log.Append(logEntry);
                                dataCallback?.Invoke(data);
                            }
                            else
                            {
                                Thread.Sleep(10);
                            }
                        }
                        catch (TimeoutException)
                        {
                            // Normal timeout, continue monitoring
                        }
                    }
                    
                    port.Close();
                }
#else
                log.AppendLine($"=== COM Port Monitor (Limited Mode) ===");
                log.AppendLine($"Port: {config.PortName}");
                log.AppendLine($"Baud Rate: {config.BaudRate}");
                log.AppendLine($"Note: Full monitoring requires .NET Framework 4.6.2 or higher");
                log.AppendLine($"Start Time: {startTime:yyyy-MM-dd HH:mm:ss}");
                log.AppendLine("=====================================");
                Thread.Sleep(durationSeconds * 1000);
#endif
            }
            catch (Exception ex)
            {
                log.AppendLine($"Error: {ex.Message}");
            }
            
            log.AppendLine($"\n=== Monitoring Completed at {DateTime.Now:HH:mm:ss} ===");
            return log.ToString();
        }

        /// <summary>
        /// Sends AT commands to test modem/ESP devices
        /// </summary>
        /// <param name="portName">COM port name</param>
        /// <param name="baudRate">Baud rate</param>
        /// <returns>AT command test results</returns>
        public static string TestATCommands(string portName, int baudRate = 115200)
        {
            var results = new StringBuilder();
            string[] atCommands = { "AT", "ATI", "AT+GMR", "AT+CIPSTA?", "AT+CWMODE?" };
            
            try
            {
#if NET462 || NET6_0 || NET8_0 || NETSTANDARD2_0 || NETSTANDARD2_1
                using (var port = new SerialPort(portName, baudRate))
                {
                    port.ReadTimeout = 2000;
                    port.WriteTimeout = 1000;
                    port.NewLine = "\r\n";
                    
                    port.Open();
                    Thread.Sleep(1000); // Wait for device to initialize
                    
                    results.AppendLine("=== AT Command Test Results ===");
                    results.AppendLine($"Port: {portName}, Baud: {baudRate}");
                    results.AppendLine();
                    
                    foreach (string command in atCommands)
                    {
                        try
                        {
                            // Clear input buffer
                            port.DiscardInBuffer();
                            
                            // Send command
                            port.WriteLine(command);
                            Thread.Sleep(500);
                            
                            // Read response
                            string response = "";
                            while (port.BytesToRead > 0)
                            {
                                response += port.ReadExisting();
                                Thread.Sleep(100);
                            }
                            
                            results.AppendLine($"Command: {command}");
                            results.AppendLine($"Response: {response.Trim()}");
                            results.AppendLine();
                        }
                        catch (Exception ex)
                        {
                            results.AppendLine($"Command: {command}");
                            results.AppendLine($"Error: {ex.Message}");
                            results.AppendLine();
                        }
                    }
                    
                    port.Close();
                }
#else
                results.AppendLine("=== AT Command Test (Limited Mode) ===");
                results.AppendLine($"Port: {portName}, Baud: {baudRate}");
                results.AppendLine("Note: AT command testing requires .NET Framework 4.6.2 or higher");
                results.AppendLine();
                
                foreach (string command in atCommands)
                {
                    results.AppendLine($"Command: {command}");
                    results.AppendLine("Response: [Limited mode - cannot execute]");
                    results.AppendLine();
                }
#endif
            }
            catch (Exception ex)
            {
                results.AppendLine($"Connection Error: {ex.Message}");
            }
            
            return results.ToString();
        }

        /// <summary>
        /// Generates Arduino serial monitor code
        /// </summary>
        /// <param name="baudRate">Serial baud rate</param>
        /// <param name="includeDebug">Include debug output</param>
        /// <returns>Arduino code</returns>
        public static string GenerateArduinoSerialCode(int baudRate = 115200, bool includeDebug = true)
        {
            var code = new StringBuilder();
            
            code.AppendLine("// Arduino Serial Communication Template");
            code.AppendLine("// Generated by CircuitTool");
            code.AppendLine();
            code.AppendLine("void setup() {");
            code.AppendLine($"  Serial.begin({baudRate});");
            code.AppendLine("  while (!Serial) {");
            code.AppendLine("    ; // Wait for serial port to connect");
            code.AppendLine("  }");
            code.AppendLine("  Serial.println(\"Arduino Serial Monitor Ready\");");
            if (includeDebug)
            {
                code.AppendLine("  Serial.println(\"Debug mode enabled\");");
                code.AppendLine("  Serial.print(\"Free RAM: \");");
                code.AppendLine("  Serial.println(freeRam());");
            }
            code.AppendLine("}");
            code.AppendLine();
            code.AppendLine("void loop() {");
            code.AppendLine("  // Check for incoming serial data");
            code.AppendLine("  if (Serial.available() > 0) {");
            code.AppendLine("    String command = Serial.readStringUntil('\\n');");
            code.AppendLine("    command.trim();");
            code.AppendLine("    processCommand(command);");
            code.AppendLine("  }");
            code.AppendLine("  ");
            code.AppendLine("  // Your main code here");
            if (includeDebug)
            {
                code.AppendLine("  // Send periodic status updates");
                code.AppendLine("  static unsigned long lastUpdate = 0;");
                code.AppendLine("  if (millis() - lastUpdate > 5000) {");
                code.AppendLine("    Serial.print(\"Status: Running, Uptime: \");");
                code.AppendLine("    Serial.print(millis() / 1000);");
                code.AppendLine("    Serial.println(\" seconds\");");
                code.AppendLine("    lastUpdate = millis();");
                code.AppendLine("  }");
            }
            code.AppendLine("  ");
            code.AppendLine("  delay(100);");
            code.AppendLine("}");
            code.AppendLine();
            code.AppendLine("void processCommand(String cmd) {");
            code.AppendLine("  cmd.toUpperCase();");
            code.AppendLine("  ");
            code.AppendLine("  if (cmd == \"PING\") {");
            code.AppendLine("    Serial.println(\"PONG\");");
            code.AppendLine("  } else if (cmd == \"STATUS\") {");
            code.AppendLine("    Serial.println(\"Device: Arduino\");");
            code.AppendLine("    Serial.print(\"Uptime: \");");
            code.AppendLine("    Serial.print(millis() / 1000);");
            code.AppendLine("    Serial.println(\" seconds\");");
            code.AppendLine("  } else if (cmd == \"RESET\") {");
            code.AppendLine("    Serial.println(\"Resetting...\");");
            code.AppendLine("    delay(1000);");
            code.AppendLine("    asm volatile (\"  jmp 0\");");
            code.AppendLine("  } else {");
            code.AppendLine("    Serial.print(\"Unknown command: \");");
            code.AppendLine("    Serial.println(cmd);");
            code.AppendLine("  }");
            code.AppendLine("}");
            
            if (includeDebug)
            {
                code.AppendLine();
                code.AppendLine("int freeRam() {");
                code.AppendLine("  extern int __heap_start, *__brkval;");
                code.AppendLine("  int v;");
                code.AppendLine("  return (int) &v - (__brkval == 0 ? (int) &__heap_start : (int) __brkval);");
                code.AppendLine("}");
            }
            
            return code.ToString();
        }

        /// <summary>
        /// Generates ESP32 serial communication code
        /// </summary>
        /// <param name="baudRate">Serial baud rate</param>
        /// <param name="includeWiFi">Include WiFi status in output</param>
        /// <returns>ESP32 code</returns>
        public static string GenerateESP32SerialCode(int baudRate = 115200, bool includeWiFi = true)
        {
            var code = new StringBuilder();
            
            code.AppendLine("// ESP32 Serial Communication Template");
            code.AppendLine("// Generated by CircuitTool");
            code.AppendLine();
            if (includeWiFi)
            {
                code.AppendLine("#include <WiFi.h>");
                code.AppendLine();
            }
            code.AppendLine("void setup() {");
            code.AppendLine($"  Serial.begin({baudRate});");
            code.AppendLine("  delay(1000);");
            code.AppendLine("  Serial.println(\"ESP32 Serial Monitor Ready\");");
            code.AppendLine("  Serial.println(\"Chip Model: \" + String(ESP.getChipModel()));");
            code.AppendLine("  Serial.println(\"Chip Revision: \" + String(ESP.getChipRevision()));");
            code.AppendLine("  Serial.println(\"CPU Frequency: \" + String(ESP.getCpuFreqMHz()) + \" MHz\");");
            code.AppendLine("  Serial.println(\"Flash Size: \" + String(ESP.getFlashChipSize()) + \" bytes\");");
            code.AppendLine("  Serial.println(\"Free Heap: \" + String(ESP.getFreeHeap()) + \" bytes\");");
            if (includeWiFi)
            {
                code.AppendLine("  Serial.println(\"MAC Address: \" + WiFi.macAddress());");
            }
            code.AppendLine("}");
            code.AppendLine();
            code.AppendLine("void loop() {");
            code.AppendLine("  // Check for incoming serial data");
            code.AppendLine("  if (Serial.available() > 0) {");
            code.AppendLine("    String command = Serial.readStringUntil('\\n');");
            code.AppendLine("    command.trim();");
            code.AppendLine("    processCommand(command);");
            code.AppendLine("  }");
            code.AppendLine("  ");
            code.AppendLine("  // Your main code here");
            code.AppendLine("  ");
            code.AppendLine("  delay(100);");
            code.AppendLine("}");
            code.AppendLine();
            code.AppendLine("void processCommand(String cmd) {");
            code.AppendLine("  cmd.toUpperCase();");
            code.AppendLine("  ");
            code.AppendLine("  if (cmd == \"PING\") {");
            code.AppendLine("    Serial.println(\"PONG\");");
            code.AppendLine("  } else if (cmd == \"STATUS\") {");
            code.AppendLine("    Serial.println(\"Device: ESP32\");");
            code.AppendLine("    Serial.println(\"Uptime: \" + String(millis() / 1000) + \" seconds\");");
            code.AppendLine("    Serial.println(\"Free Heap: \" + String(ESP.getFreeHeap()) + \" bytes\");");
            code.AppendLine("    Serial.println(\"CPU Frequency: \" + String(ESP.getCpuFreqMHz()) + \" MHz\");");
            if (includeWiFi)
            {
                code.AppendLine("    Serial.println(\"WiFi Status: \" + String(WiFi.status()));");
            }
            code.AppendLine("  } else if (cmd == \"RESET\") {");
            code.AppendLine("    Serial.println(\"Resetting...\");");
            code.AppendLine("    delay(1000);");
            code.AppendLine("    ESP.restart();");
            code.AppendLine("  } else if (cmd == \"HEAP\") {");
            code.AppendLine("    Serial.println(\"Free Heap: \" + String(ESP.getFreeHeap()) + \" bytes\");");
            code.AppendLine("    Serial.println(\"Heap Size: \" + String(ESP.getHeapSize()) + \" bytes\");");
            code.AppendLine("    Serial.println(\"Min Free Heap: \" + String(ESP.getMinFreeHeap()) + \" bytes\");");
            if (includeWiFi)
            {
                code.AppendLine("  } else if (cmd == \"WIFI\") {");
                code.AppendLine("    Serial.println(\"WiFi Status: \" + String(WiFi.status()));");
                code.AppendLine("    Serial.println(\"SSID: \" + WiFi.SSID());");
                code.AppendLine("    Serial.println(\"IP Address: \" + WiFi.localIP().toString());");
                code.AppendLine("    Serial.println(\"Signal Strength: \" + String(WiFi.RSSI()) + \" dBm\");");
            }
            code.AppendLine("  } else {");
            code.AppendLine("    Serial.print(\"Unknown command: \");");
            code.AppendLine("    Serial.println(cmd);");
            code.AppendLine("    Serial.println(\"Available commands: PING, STATUS, RESET, HEAP\" + ");
            if (includeWiFi)
            {
                code.AppendLine("                      String(\", WIFI\"));");
            }
            else
            {
                code.AppendLine("                      String(\"\"));");
            }
            code.AppendLine("  }");
            code.AppendLine("}");
            
            return code.ToString();
        }

        /// <summary>
        /// Calculates optimal buffer sizes for serial communication
        /// </summary>
        /// <param name="baudRate">Baud rate</param>
        /// <param name="expectedDataRate">Expected data rate in bytes/second</param>
        /// <param name="latencyMs">Maximum acceptable latency in milliseconds</param>
        /// <returns>Recommended buffer sizes</returns>
        public static (int rxBufferSize, int txBufferSize) CalculateBufferSizes(int baudRate, 
            double expectedDataRate = 0, int latencyMs = 100)
        {
            // Calculate theoretical maximum data rate (80% of baud rate for overhead)
            double maxDataRate = (baudRate / 10) * 0.8; // 10 bits per byte (8 data + start + stop)
            
            if (expectedDataRate == 0)
                expectedDataRate = maxDataRate * 0.5; // Assume 50% utilization
            
            // Calculate buffer size based on latency requirements
            int baseBufferSize = (int)(expectedDataRate * latencyMs / 1000);
            
            // Add safety margin and round to power of 2
            int rxBufferSize = NextPowerOfTwo(Math.Max(baseBufferSize * 2, 256));
            int txBufferSize = NextPowerOfTwo(Math.Max(baseBufferSize, 128));
            
            // Cap at reasonable maximums
            rxBufferSize = Math.Min(rxBufferSize, 8192);
            txBufferSize = Math.Min(txBufferSize, 4096);
            
            return (rxBufferSize, txBufferSize);
        }

        /// <summary>
        /// Finds the next power of 2 greater than or equal to the input
        /// </summary>
        private static int NextPowerOfTwo(int value)
        {
            if (value <= 1) return 1;
            
            value--;
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            value++;
            
            return value;
        }

        /// <summary>
        /// Generates COM port diagnostic report
        /// </summary>
        /// <returns>Comprehensive diagnostic report</returns>
        public static string GenerateDiagnosticReport()
        {
            var report = new StringBuilder();
            
            report.AppendLine("=== COM Port Diagnostic Report ===");
            report.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            report.AppendLine();
            
            // Available ports
            var ports = GetAvailablePorts();
            report.AppendLine($"Available COM Ports: {ports.Count}");
            report.AppendLine();
            
            foreach (var port in ports)
            {
                report.AppendLine($"Port: {port.PortName}");
                report.AppendLine($"  Description: {port.Description}");
                report.AppendLine($"  Device Type: {port.DeviceType}");
                report.AppendLine($"  Available: {port.IsAvailable}");
                
                // Test common baud rates
                var workingBaudRates = new List<int>();
                foreach (int baudRate in new[] { 9600, 115200, 230400 })
                {
                    if (TestPortAvailability(port.PortName, baudRate, 500))
                    {
                        workingBaudRates.Add(baudRate);
                    }
                }
                
                if (workingBaudRates.Any())
                {
                    report.AppendLine($"  Working Baud Rates: {string.Join(", ", workingBaudRates)}");
                }
                else
                {
                    report.AppendLine("  Status: Port may be in use or device not responding");
                }
                
                report.AppendLine();
            }
            
            // System information
            report.AppendLine("=== System Information ===");
            report.AppendLine($"Operating System: {Environment.OSVersion}");
            report.AppendLine($"Platform: {Environment.OSVersion.Platform}");
            report.AppendLine($"64-bit OS: {Environment.Is64BitOperatingSystem}");
            report.AppendLine();
            
            // Recommendations
            report.AppendLine("=== Recommendations ===");
            if (ports.Count == 0)
            {
                report.AppendLine("• No COM ports detected. Check device connections and drivers.");
            }
            else
            {
                report.AppendLine($"• {ports.Count} COM port(s) detected.");
                var arduinoPorts = ports.Where(p => p.DeviceType == "Arduino").Count();
                var esp32Ports = ports.Where(p => p.DeviceType == "ESP32").Count();
                
                if (arduinoPorts > 0)
                    report.AppendLine($"• {arduinoPorts} Arduino-compatible device(s) detected.");
                if (esp32Ports > 0)
                    report.AppendLine($"• {esp32Ports} ESP32-compatible device(s) detected.");
                
                report.AppendLine("• Use auto-detection features to find optimal baud rates.");
                report.AppendLine("• Consider using hardware flow control for high-speed communication.");
            }
            
            return report.ToString();
        }
    }
}
