# UART (Universal Asynchronous Receiver-Transmitter) Guide

## Introduction to UART

UART is one of the most fundamental and widely used serial communication protocols in embedded systems. It provides asynchronous, full-duplex communication between devices using only two wires for data transmission (plus ground). UART is the foundation for many other protocols like RS-232, RS-485, and is commonly used for debugging, sensor communication, and device interfacing.

## UART Protocol Overview

### Key Features
- **Asynchronous**: No shared clock signal required
- **Full-duplex**: Simultaneous bidirectional communication
- **Point-to-point**: Direct connection between two devices
- **Simple hardware**: Minimal pins required (TX, RX, GND)
- **Configurable**: Adjustable baud rate, data bits, parity, stop bits
- **Universal**: Supported by virtually all microcontrollers

### Signal Lines
- **TX (Transmit)**: Data output from device
- **RX (Receive)**: Data input to device
- **GND (Ground)**: Common reference voltage
- **Optional**: RTS/CTS for hardware flow control

### Data Frame Structure
```
[Start Bit][Data Bits][Parity Bit][Stop Bit(s)]
    1 bit    5-9 bits    0-1 bit     1-2 bits
```

## UART Configuration Parameters

### Common Settings
- **Baud Rate**: 9600, 19200, 38400, 57600, 115200, 230400, 460800, 921600 bps
- **Data Bits**: 5, 6, 7, 8 (most common is 8)
- **Parity**: None, Even, Odd, Mark, Space
- **Stop Bits**: 1, 1.5, 2 (most common is 1)
- **Flow Control**: None, Hardware (RTS/CTS), Software (XON/XOFF)

### Baud Rate Calculation
For most microcontrollers:
```
Baud Rate = Clock Frequency / (Prescaler × (BRR + 1))
```

Where BRR is the Baud Rate Register value.

## Arduino UART Programming

### Basic UART Communication
```cpp
void setup() {
  // Initialize hardware UART
  Serial.begin(9600);        // USB/Debug UART
  Serial1.begin(115200);     // Hardware UART 1 (pins vary by board)
  Serial2.begin(38400);      // Hardware UART 2 (if available)
  
  Serial.println("UART Communication Demo");
}

void loop() {
  // Echo data between Serial and Serial1
  if (Serial.available()) {
    String data = Serial.readString();
    data.trim();
    Serial1.println("From PC: " + data);
    Serial.println("Sent to UART1: " + data);
  }
  
  if (Serial1.available()) {
    String data = Serial1.readString();
    data.trim();
    Serial.println("From UART1: " + data);
  }
}
```

### Advanced UART Class Implementation
```cpp
class UARTInterface {
private:
  HardwareSerial* uart;
  unsigned long baudRate;
  uint8_t config;
  
  // Buffer management
  static const int BUFFER_SIZE = 256;
  uint8_t rxBuffer[BUFFER_SIZE];
  uint8_t txBuffer[BUFFER_SIZE];
  volatile int rxHead = 0;
  volatile int rxTail = 0;
  volatile int txHead = 0;
  volatile int txTail = 0;
  
  // Statistics
  unsigned long bytesReceived = 0;
  unsigned long bytesSent = 0;
  unsigned long frameErrors = 0;
  unsigned long overrunErrors = 0;
  
public:
  UARTInterface(HardwareSerial* serial, unsigned long baud = 9600) {
    uart = serial;
    baudRate = baud;
    config = SERIAL_8N1; // 8 data bits, no parity, 1 stop bit
  }
  
  bool begin(uint8_t rxPin = -1, uint8_t txPin = -1) {
    if (rxPin != -1 && txPin != -1) {
      // ESP32 allows custom pin assignment
      uart->begin(baudRate, config, rxPin, txPin);
    } else {
      uart->begin(baudRate, config);
    }
    
    // Clear buffers
    clearBuffers();
    
    Serial.printf("UART initialized: %lu baud, pins RX=%d TX=%d\n", 
                  baudRate, rxPin, txPin);
    return true;
  }
  
  void end() {
    uart->end();
  }
  
  // Blocking write
  size_t write(const uint8_t* data, size_t length) {
    size_t written = uart->write(data, length);
    bytesSent += written;
    return written;
  }
  
  size_t write(const String& str) {
    return write((uint8_t*)str.c_str(), str.length());
  }
  
  // Non-blocking write with buffering
  bool writeNonBlocking(const uint8_t* data, size_t length) {
    for (size_t i = 0; i < length; i++) {
      int next = (txHead + 1) % BUFFER_SIZE;
      if (next == txTail) {
        return false; // Buffer full
      }
      txBuffer[txHead] = data[i];
      txHead = next;
    }
    return true;
  }
  
  // Process buffered transmission
  void processTx() {
    while (txHead != txTail && uart->availableForWrite()) {
      uart->write(txBuffer[txTail]);
      txTail = (txTail + 1) % BUFFER_SIZE;
      bytesSent++;
    }
  }
  
  // Blocking read
  int read() {
    int data = uart->read();
    if (data != -1) {
      bytesReceived++;
    }
    return data;
  }
  
  // Read with timeout
  bool readBytes(uint8_t* buffer, size_t length, unsigned long timeout = 1000) {
    unsigned long startTime = millis();
    size_t index = 0;
    
    while (index < length && (millis() - startTime) < timeout) {
      if (uart->available()) {
        buffer[index++] = uart->read();
        bytesReceived++;
        startTime = millis(); // Reset timeout on data reception
      }
    }
    
    return index == length;
  }
  
  // Read line with timeout
  String readLine(unsigned long timeout = 1000) {
    String line = "";
    unsigned long startTime = millis();
    
    while ((millis() - startTime) < timeout) {
      if (uart->available()) {
        char c = uart->read();
        bytesReceived++;
        
        if (c == '\n') {
          break;
        } else if (c != '\r') {
          line += c;
        }
        
        startTime = millis(); // Reset timeout
      }
    }
    
    return line;
  }
  
  // Available data
  int available() {
    return uart->available();
  }
  
  // Check for errors (ESP32 specific)
  void checkErrors() {
    if (uart->hasRxError()) {
      frameErrors++;
      uart->clearWriteError();
    }
    
    if (uart->hasOverrun()) {
      overrunErrors++;
    }
  }
  
  // Configuration methods
  void setBaudRate(unsigned long baud) {
    baudRate = baud;
    uart->updateBaudRate(baud);
  }
  
  void setConfig(uint8_t databits, uint8_t parity, uint8_t stopbits) {
    config = (databits << 2) | (parity << 1) | stopbits;
    uart->begin(baudRate, config);
  }
  
  // Statistics
  void printStatistics() {
    Serial.println("=== UART Statistics ===");
    Serial.printf("Bytes Received: %lu\n", bytesReceived);
    Serial.printf("Bytes Sent: %lu\n", bytesSent);
    Serial.printf("Frame Errors: %lu\n", frameErrors);
    Serial.printf("Overrun Errors: %lu\n", overrunErrors);
    Serial.printf("RX Buffer Usage: %d/%d\n", available(), BUFFER_SIZE);
    Serial.println();
  }
  
  void resetStatistics() {
    bytesReceived = 0;
    bytesSent = 0;
    frameErrors = 0;
    overrunErrors = 0;
  }
  
private:
  void clearBuffers() {
    rxHead = rxTail = 0;
    txHead = txTail = 0;
    while (uart->available()) {
      uart->read(); // Clear hardware buffer
    }
  }
};

// Usage example
UARTInterface uart1(&Serial1, 115200);
UARTInterface uart2(&Serial2, 9600);

void setup() {
  Serial.begin(115200);
  
  // Initialize UART interfaces
  uart1.begin(16, 17); // ESP32 custom pins
  uart2.begin();       // Default pins
  
  Serial.println("Multiple UART interface demo");
}

void loop() {
  // Handle communication
  if (uart1.available()) {
    String data = uart1.readLine();
    Serial.println("UART1: " + data);
    uart2.write("Relayed: " + data + "\n");
  }
  
  if (uart2.available()) {
    String data = uart2.readLine();
    Serial.println("UART2: " + data);
    uart1.write("Response: " + data + "\n");
  }
  
  // Process buffered transmissions
  uart1.processTx();
  uart2.processTx();
  
  // Check for errors periodically
  static unsigned long lastErrorCheck = 0;
  if (millis() - lastErrorCheck > 5000) {
    uart1.checkErrors();
    uart2.checkErrors();
    lastErrorCheck = millis();
  }
  
  // Print statistics every 30 seconds
  static unsigned long lastStats = 0;
  if (millis() - lastStats > 30000) {
    uart1.printStatistics();
    uart2.printStatistics();
    lastStats = millis();
  }
}
```

### Protocol Implementation Over UART
```cpp
// Simple packet protocol over UART
class UARTProtocol {
private:
  UARTInterface* uart;
  
  struct Packet {
    uint8_t header;      // 0xAA
    uint8_t length;      // Data length
    uint8_t command;     // Command byte
    uint8_t data[32];    // Payload
    uint8_t checksum;    // Simple checksum
  };
  
  static const uint8_t PACKET_HEADER = 0xAA;
  static const uint8_t MAX_PACKET_SIZE = 35;
  
public:
  UARTProtocol(UARTInterface* uartInterface) : uart(uartInterface) {}
  
  bool sendPacket(uint8_t command, const uint8_t* data, uint8_t dataLength) {
    if (dataLength > 32) return false;
    
    Packet packet;
    packet.header = PACKET_HEADER;
    packet.length = dataLength;
    packet.command = command;
    
    memcpy(packet.data, data, dataLength);
    
    // Calculate checksum
    packet.checksum = 0;
    packet.checksum ^= packet.length;
    packet.checksum ^= packet.command;
    for (int i = 0; i < dataLength; i++) {
      packet.checksum ^= packet.data[i];
    }
    
    // Send packet
    uart->write((uint8_t*)&packet, 4 + dataLength);
    return true;
  }
  
  bool receivePacket(uint8_t& command, uint8_t* data, uint8_t& dataLength, unsigned long timeout = 1000) {
    unsigned long startTime = millis();
    
    // Wait for header
    while ((millis() - startTime) < timeout) {
      if (uart->available()) {
        uint8_t byte = uart->read();
        if (byte == PACKET_HEADER) {
          break;
        }
      }
    }
    
    if ((millis() - startTime) >= timeout) return false;
    
    // Read packet
    uint8_t packetData[MAX_PACKET_SIZE];
    if (!uart->readBytes(packetData, 3, timeout)) { // length, command, first data byte or checksum
      return false;
    }
    
    uint8_t length = packetData[0];
    command = packetData[1];
    
    if (length > 32) return false; // Invalid length
    
    // Read remaining data and checksum
    if (length > 0) {
      if (!uart->readBytes(data, length, timeout)) {
        return false;
      }
    }
    
    uint8_t receivedChecksum;
    if (!uart->readBytes(&receivedChecksum, 1, timeout)) {
      return false;
    }
    
    // Verify checksum
    uint8_t calculatedChecksum = 0;
    calculatedChecksum ^= length;
    calculatedChecksum ^= command;
    for (int i = 0; i < length; i++) {
      calculatedChecksum ^= data[i];
    }
    
    if (receivedChecksum != calculatedChecksum) {
      return false; // Checksum mismatch
    }
    
    dataLength = length;
    return true;
  }
  
  // Command definitions
  enum Commands {
    CMD_PING = 0x01,
    CMD_READ_SENSOR = 0x02,
    CMD_WRITE_CONFIG = 0x03,
    CMD_GET_STATUS = 0x04,
    CMD_RESET = 0x05
  };
  
  // High-level command methods
  bool ping() {
    return sendPacket(CMD_PING, nullptr, 0);
  }
  
  bool readSensor(uint8_t sensorId, float& value) {
    if (!sendPacket(CMD_READ_SENSOR, &sensorId, 1)) {
      return false;
    }
    
    uint8_t command, data[4], dataLength;
    if (receivePacket(command, data, dataLength, 2000)) {
      if (command == CMD_READ_SENSOR && dataLength == 4) {
        memcpy(&value, data, 4);
        return true;
      }
    }
    return false;
  }
  
  bool writeConfig(uint8_t configId, uint32_t value) {
    uint8_t data[5];
    data[0] = configId;
    memcpy(&data[1], &value, 4);
    return sendPacket(CMD_WRITE_CONFIG, data, 5);
  }
};

// Example usage
UARTInterface uart(&Serial1, 115200);
UARTProtocol protocol(&uart);

void setup() {
  Serial.begin(115200);
  uart.begin();
  
  Serial.println("UART Protocol Demo");
}

void loop() {
  // Ping remote device
  if (protocol.ping()) {
    Serial.println("Ping successful");
    
    // Read sensor data
    float temperature;
    if (protocol.readSensor(1, temperature)) {
      Serial.printf("Temperature: %.2f°C\n", temperature);
    }
    
    // Write configuration
    protocol.writeConfig(0x10, 12345);
  }
  
  delay(5000);
}
```

### UART Bridge and Data Logger
```cpp
#include <SD.h>
#include <WiFi.h>
#include <WebServer.h>

class UARTBridge {
private:
  UARTInterface* uart;
  File logFile;
  WebServer server;
  String logBuffer;
  bool wifiEnabled = false;
  bool sdEnabled = false;
  
public:
  UARTBridge(UARTInterface* uartInterface) : uart(uartInterface), server(80) {}
  
  void begin() {
    // Initialize SD card
    if (SD.begin()) {
      sdEnabled = true;
      Serial.println("SD card initialized");
      
      // Create log file
      String filename = "/uart_log_" + String(millis()) + ".txt";
      logFile = SD.open(filename, FILE_WRITE);
      if (logFile) {
        logFile.println("UART Log Started");
        logFile.close();
      }
    }
    
    // Initialize WiFi (optional)
    setupWiFi();
    
    // Setup web server
    if (wifiEnabled) {
      setupWebServer();
    }
  }
  
  void loop() {
    // Handle UART data
    while (uart->available()) {
      String data = uart->readLine();
      if (data.length() > 0) {
        processUARTData(data);
      }
    }
    
    // Handle web server
    if (wifiEnabled) {
      server.handleClient();
    }
    
    // Periodic log flushing
    static unsigned long lastFlush = 0;
    if (millis() - lastFlush > 10000) { // Flush every 10 seconds
      flushLog();
      lastFlush = millis();
    }
  }
  
private:
  void processUARTData(const String& data) {
    String timestamp = String(millis());
    String logEntry = timestamp + ": " + data;
    
    // Add to buffer
    logBuffer += logEntry + "\n";
    
    // Print to serial
    Serial.println("UART> " + data);
    
    // If buffer is large, flush to SD
    if (logBuffer.length() > 1024) {
      flushLog();
    }
  }
  
  void flushLog() {
    if (sdEnabled && logBuffer.length() > 0) {
      logFile = SD.open("/uart_log.txt", FILE_APPEND);
      if (logFile) {
        logFile.print(logBuffer);
        logFile.close();
        logBuffer = "";
      }
    }
  }
  
  void setupWiFi() {
    WiFi.begin("YourSSID", "YourPassword");
    
    int attempts = 0;
    while (WiFi.status() != WL_CONNECTED && attempts < 20) {
      delay(500);
      Serial.print(".");
      attempts++;
    }
    
    if (WiFi.status() == WL_CONNECTED) {
      wifiEnabled = true;
      Serial.println();
      Serial.print("WiFi connected: ");
      Serial.println(WiFi.localIP());
    } else {
      Serial.println("WiFi connection failed");
    }
  }
  
  void setupWebServer() {
    // Serve log data
    server.on("/", [this]() {
      String html = "<html><head><title>UART Bridge</title></head><body>";
      html += "<h1>UART Data Bridge</h1>";
      html += "<h2>Recent Data:</h2>";
      html += "<pre>" + logBuffer + "</pre>";
      html += "<br><a href='/download'>Download Full Log</a>";
      html += "</body></html>";
      server.send(200, "text/html", html);
    });
    
    // Download log file
    server.on("/download", [this]() {
      if (sdEnabled) {
        File file = SD.open("/uart_log.txt");
        if (file) {
          server.sendHeader("Content-Disposition", "attachment; filename=uart_log.txt");
          server.streamFile(file, "text/plain");
          file.close();
        } else {
          server.send(404, "text/plain", "Log file not found");
        }
      } else {
        server.send(503, "text/plain", "SD card not available");
      }
    });
    
    // Send data to UART
    server.on("/send", [this]() {
      if (server.hasArg("data")) {
        String data = server.arg("data");
        uart->write(data + "\n");
        server.send(200, "text/plain", "Data sent: " + data);
      } else {
        server.send(400, "text/plain", "Missing 'data' parameter");
      }
    });
    
    server.begin();
    Serial.println("Web server started");
  }
};
```

### Multi-Device UART Communication Hub
```cpp
class UARTHub {
private:
  static const int MAX_DEVICES = 4;
  
  struct Device {
    UARTInterface* uart;
    String name;
    bool active;
    unsigned long lastActivity;
    uint8_t deviceId;
  };
  
  Device devices[MAX_DEVICES];
  int deviceCount = 0;
  
public:
  bool addDevice(const String& name, UARTInterface* uart, uint8_t id) {
    if (deviceCount < MAX_DEVICES) {
      devices[deviceCount] = {uart, name, true, millis(), id};
      deviceCount++;
      Serial.println("Added device: " + name);
      return true;
    }
    return false;
  }
  
  void broadcastMessage(const String& message, uint8_t excludeId = 0xFF) {
    for (int i = 0; i < deviceCount; i++) {
      if (devices[i].active && devices[i].deviceId != excludeId) {
        devices[i].uart->write("[BROADCAST] " + message + "\n");
      }
    }
  }
  
  void sendToDevice(uint8_t deviceId, const String& message) {
    for (int i = 0; i < deviceCount; i++) {
      if (devices[i].deviceId == deviceId && devices[i].active) {
        devices[i].uart->write(message + "\n");
        devices[i].lastActivity = millis();
        break;
      }
    }
  }
  
  void processMessages() {
    for (int i = 0; i < deviceCount; i++) {
      if (devices[i].active && devices[i].uart->available()) {
        String message = devices[i].uart->readLine();
        if (message.length() > 0) {
          devices[i].lastActivity = millis();
          handleMessage(devices[i].deviceId, devices[i].name, message);
        }
      }
    }
  }
  
  void checkDeviceHealth() {
    unsigned long now = millis();
    
    for (int i = 0; i < deviceCount; i++) {
      if (devices[i].active && (now - devices[i].lastActivity > 30000)) {
        Serial.println("Device " + devices[i].name + " appears inactive");
        // Could implement ping or disable device
      }
    }
  }
  
private:
  void handleMessage(uint8_t fromId, const String& deviceName, const String& message) {
    Serial.println("[" + deviceName + "] " + message);
    
    // Parse and route messages
    if (message.startsWith("@")) {
      // Direct message to specific device
      int spaceIndex = message.indexOf(' ');
      if (spaceIndex > 1) {
        uint8_t targetId = message.substring(1, spaceIndex).toInt();
        String content = message.substring(spaceIndex + 1);
        sendToDevice(targetId, "[" + deviceName + "] " + content);
      }
    } else if (message.startsWith("!")) {
      // Broadcast message
      String content = message.substring(1);
      broadcastMessage("[" + deviceName + "] " + content, fromId);
    } else {
      // Regular message - could be logged or processed
      logMessage(deviceName, message);
    }
  }
  
  void logMessage(const String& device, const String& message) {
    // Implement message logging
    Serial.println("LOG: [" + device + "] " + message);
  }
};

// Example setup
UARTInterface device1(&Serial1, 115200);
UARTInterface device2(&Serial2, 9600);
UARTHub hub;

void setup() {
  Serial.begin(115200);
  
  device1.begin(16, 17);
  device2.begin(25, 26);
  
  hub.addDevice("Sensor Node", &device1, 1);
  hub.addDevice("Display Unit", &device2, 2);
  
  Serial.println("UART Hub initialized");
}

void loop() {
  hub.processMessages();
  hub.checkDeviceHealth();
  
  // Example: Send periodic status
  static unsigned long lastStatus = 0;
  if (millis() - lastStatus > 60000) {
    hub.broadcastMessage("System Status: OK");
    lastStatus = millis();
  }
  
  delay(10);
}
```

## UART Best Practices

### 1. Baud Rate Selection
```cpp
void selectOptimalBaudRate(float cableLength, bool highSpeed = false) {
  // Guidelines for baud rate selection
  if (cableLength > 100) { // > 100 meters
    Serial.println("Recommended: 9600 bps or lower");
  } else if (cableLength > 15) { // 15-100 meters
    Serial.println("Recommended: 38400 bps or lower");
  } else if (cableLength > 3) { // 3-15 meters
    Serial.println("Recommended: 115200 bps or lower");
  } else { // < 3 meters
    if (highSpeed) {
      Serial.println("Recommended: Up to 921600 bps");
    } else {
      Serial.println("Recommended: Up to 230400 bps");
    }
  }
}
```

### 2. Error Detection and Recovery
```cpp
class RobustUART {
private:
  UARTInterface* uart;
  unsigned long timeoutMs;
  int maxRetries;
  
public:
  RobustUART(UARTInterface* u, unsigned long timeout = 1000, int retries = 3) 
    : uart(u), timeoutMs(timeout), maxRetries(retries) {}
  
  bool sendWithAck(const String& message) {
    for (int attempt = 0; attempt < maxRetries; attempt++) {
      uart->write(message + "\n");
      
      String response = uart->readLine(timeoutMs);
      if (response == "ACK") {
        return true;
      }
      
      Serial.println("Retry " + String(attempt + 1));
      delay(100 * (attempt + 1)); // Exponential backoff
    }
    
    return false;
  }
  
  bool receiveWithAck(String& message) {
    message = uart->readLine(timeoutMs);
    if (message.length() > 0) {
      uart->write("ACK\n");
      return true;
    }
    return false;
  }
};
```

### 3. Flow Control Implementation
```cpp
class UARTFlowControl {
private:
  UARTInterface* uart;
  int rtsPin, ctsPin;
  bool hardwareFlowControl;
  
public:
  UARTFlowControl(UARTInterface* u, int rts = -1, int cts = -1) 
    : uart(u), rtsPin(rts), ctsPin(cts) {
    hardwareFlowControl = (rts != -1 && cts != -1);
    
    if (hardwareFlowControl) {
      pinMode(rtsPin, OUTPUT);
      pinMode(ctsPin, INPUT);
      digitalWrite(rtsPin, LOW); // Ready to receive
    }
  }
  
  bool canSend() {
    if (hardwareFlowControl) {
      return digitalRead(ctsPin) == LOW; // Clear to send
    }
    return true;
  }
  
  void setReady(bool ready) {
    if (hardwareFlowControl) {
      digitalWrite(rtsPin, ready ? LOW : HIGH);
    }
  }
  
  bool sendData(const uint8_t* data, size_t length) {
    if (!canSend()) {
      return false;
    }
    
    for (size_t i = 0; i < length; i++) {
      while (!canSend()) {
        delay(1); // Wait for clear to send
      }
      uart->write(&data[i], 1);
    }
    
    return true;
  }
};
```

### 4. Power Management
```cpp
class UARTPowerManager {
private:
  UARTInterface* uart;
  bool autoSleep;
  unsigned long inactivityTimeout;
  unsigned long lastActivity;
  
public:
  UARTPowerManager(UARTInterface* u, unsigned long timeout = 30000) 
    : uart(u), inactivityTimeout(timeout), autoSleep(false) {
    lastActivity = millis();
  }
  
  void enableAutoSleep(bool enable) {
    autoSleep = enable;
  }
  
  void updateActivity() {
    lastActivity = millis();
  }
  
  void checkPowerState() {
    if (autoSleep && (millis() - lastActivity > inactivityTimeout)) {
      enterSleepMode();
    }
  }
  
private:
  void enterSleepMode() {
    Serial.println("UART entering sleep mode");
    uart->end();
    
    // Configure wake-up source (e.g., RX pin interrupt)
    esp_sleep_enable_ext0_wakeup(GPIO_NUM_16, 0); // RX pin low
    esp_light_sleep_start();
    
    // Wake up
    uart->begin();
    updateActivity();
    Serial.println("UART woke up");
  }
};
```

## Common Issues and Troubleshooting

### Problem: No Communication
**Check:**
- Baud rate mismatch between devices
- Crossed TX/RX connections (TX to RX, RX to TX)
- Common ground connection
- Voltage level compatibility (3.3V vs 5V)

### Problem: Garbled Data
**Solutions:**
- Verify baud rate accuracy
- Check for electromagnetic interference
- Reduce cable length or baud rate
- Add termination resistors for long cables

### Problem: Data Loss
**Solutions:**
- Implement flow control
- Increase buffer sizes
- Reduce transmission rate
- Add error detection/correction

### Problem: Timing Issues
**Solutions:**
- Use hardware timers for precise timing
- Implement proper timeout handling
- Account for processing delays
- Use DMA for high-speed transfers

## UART vs Other Protocols

| Feature | UART | I2C | SPI | CAN |
|---------|------|-----|-----|-----|
| Wires | 3 (TX,RX,GND) | 2 (SDA,SCL) | 4+ (MOSI,MISO,SCK,CS) | 2 (CAN_H,CAN_L) |
| Speed | Up to 1Mbps | Up to 3.4Mbps | Up to 50Mbps | Up to 1Mbps |
| Distance | Long (km with RS485) | Short (<1m) | Short (<1m) | Long (km) |
| Complexity | Low | Medium | Medium | High |
| Multi-drop | No (point-to-point) | Yes | Yes | Yes |
| Error detection | Optional | ACK/NACK | None | CRC + ACK |

This comprehensive UART guide covers everything from basic communication to advanced features like protocol implementation, error handling, and power management.
