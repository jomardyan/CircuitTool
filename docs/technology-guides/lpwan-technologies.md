# LPWAN Technologies Guide

## Introduction

Low Power Wide Area Network (LPWAN) technologies are designed for IoT applications that require long-range communication with minimal power consumption. These technologies enable devices to operate for years on a single battery while communicating over distances of several kilometers.

## Overview of LPWAN Technologies

### Key Characteristics
- **Long Range**: 2-40+ km depending on environment
- **Low Power**: Battery life of 5-20 years
- **Low Data Rate**: Typically 0.3-50 kbps
- **Low Cost**: Simplified hardware and infrastructure
- **Deep Penetration**: Good indoor and underground coverage

### Main LPWAN Technologies
1. **LoRaWAN** (Long Range Wide Area Network)
2. **Sigfox**
3. **NB-IoT** (Narrowband IoT)
4. **LTE-M** (LTE Cat-M1)

## LoRaWAN Technology

### Technical Specifications
- **Frequency Bands**: 
  - EU868: 863-870 MHz
  - US915: 902-928 MHz
  - AS923: 915-928 MHz
- **Modulation**: Chirp Spread Spectrum (CSS)
- **Data Rate**: 0.3-27 kbps
- **Range**: 2-15 km (urban), 15-40 km (rural)
- **Payload**: 11-242 bytes per message

### LoRaWAN Architecture
```
End Devices <-> Gateway <-> Network Server <-> Application Server
```

### Device Classes
- **Class A**: Bidirectional, lowest power
- **Class B**: Bidirectional with scheduled receive slots
- **Class C**: Bidirectional with maximal receive slots

### Security Features
- **AES-128 encryption** at multiple layers
- **Device authentication** with unique keys
- **Message integrity** protection
- **Anti-replay** protection

## Sigfox Technology

### Technical Specifications
- **Frequency**: 868 MHz (EU), 902 MHz (US)
- **Modulation**: Ultra Narrow Band (UNB)
- **Data Rate**: 100-600 bps
- **Range**: 3-50 km
- **Message Limit**: 140 messages/day uplink, 4 downlink
- **Payload**: 12 bytes uplink, 8 bytes downlink

### Advantages
- **Simple protocol** - easy to implement
- **Low power consumption**
- **Global coverage** through operator network
- **No gateway required** for end users

### Limitations
- **Limited data throughput**
- **Restricted message frequency**
- **Vendor lock-in** to Sigfox network

## NB-IoT (Narrowband IoT)

### Technical Specifications
- **Frequency**: Licensed LTE spectrum
- **Bandwidth**: 180 kHz
- **Data Rate**: 25-250 kbps
- **Range**: 1-10 km
- **Power Consumption**: Very low with PSM/eDRX

### Deployment Modes
- **In-band**: Within existing LTE carrier
- **Guard-band**: In LTE guard band
- **Standalone**: Dedicated spectrum

### Power Saving Features
- **PSM (Power Saving Mode)**: Deep sleep between communications
- **eDRX (Extended Discontinuous Reception)**: Extended sleep cycles
- **Coverage Enhancement**: Up to 20 dB improvement

## LTE-M (LTE Cat-M1)

### Technical Specifications
- **Bandwidth**: 1.4 MHz
- **Data Rate**: Up to 1 Mbps
- **Mobility**: Full mobility support
- **Voice Support**: VoLTE capability
- **Battery Life**: 10+ years

### Applications
- **Asset tracking** with mobility
- **Smart city** infrastructure
- **Industrial automation**
- **Emergency services**

## Programming Examples

### LoRaWAN with Arduino and RN2483
```cpp
#include <SoftwareSerial.h>

SoftwareSerial loraSerial(7, 8);

void setup() {
  Serial.begin(9600);
  loraSerial.begin(57600);
  
  // Initialize LoRaWAN module
  sendCommand("sys reset");
  delay(1000);
  
  // Configure LoRaWAN parameters
  sendCommand("mac set deveui 0004A30B001B8B39");
  sendCommand("mac set appeui 8000000000000000");
  sendCommand("mac set appkey 2B7E151628AED2A6ABF7158809CF4F3C");
  
  // Join network
  sendCommand("mac join otaa");
}

void loop() {
  // Read sensor data
  float temperature = readTemperature();
  float humidity = readHumidity();
  
  // Prepare payload
  String payload = String(temperature, 1) + "," + String(humidity, 1);
  
  // Send data
  String command = "mac tx uncnf 1 " + stringToHex(payload);
  sendCommand(command);
  
  // Sleep for 10 minutes
  delay(600000);
}

void sendCommand(String command) {
  loraSerial.println(command);
  String response = loraSerial.readString();
  Serial.println("Response: " + response);
}

String stringToHex(String str) {
  String hex = "";
  for (int i = 0; i < str.length(); i++) {
    if (str.charAt(i) < 16) hex += "0";
    hex += String(str.charAt(i), HEX);
  }
  return hex;
}
```

### Sigfox with Arduino and MKRFOX1200
```cpp
#include <SigFox.h>

void setup() {
  Serial.begin(9600);
  
  if (!SigFox.begin()) {
    Serial.println("Shield error or not present!");
    return;
  }
  
  // Print device information
  String version = SigFox.SigVersion();
  String ID = SigFox.ID();
  String PAC = SigFox.PAC();
  
  Serial.println("SigFox FW version: " + version);
  Serial.println("ID: " + ID);
  Serial.println("PAC: " + PAC);
  
  SigFox.end();
}

void loop() {
  // Start SigFox module
  SigFox.begin();
  
  // Prepare data packet
  uint8_t msg[12];
  float temperature = 23.5;
  float humidity = 65.2;
  
  // Pack data (little endian)
  memcpy(msg, &temperature, 4);
  memcpy(msg + 4, &humidity, 4);
  
  // Send message
  SigFox.beginPacket();
  SigFox.write(msg, 12);
  int ret = SigFox.endPacket();
  
  if (ret > 0) {
    Serial.println("No transmission");
  } else {
    Serial.println("Transmission ok");
  }
  
  SigFox.end();
  
  // Sleep for 15 minutes (respect daily message limit)
  delay(900000);
}
```

### NB-IoT with SIM7020
```cpp
#include <SoftwareSerial.h>

SoftwareSerial nbiot(7, 8);

void setup() {
  Serial.begin(9600);
  nbiot.begin(9600);
  
  // Initialize NB-IoT module
  sendATCommand("AT+CFUN=1");        // Set full functionality
  delay(5000);
  
  sendATCommand("AT+CBAND=20");      // Set band 20 (800MHz)
  sendATCommand("AT+CGDCONT=1,\"IP\",\"nb-iot.apn\""); // Set APN
  sendATCommand("AT+COPS=1,2,\"26201\""); // Select operator
  
  // Attach to network
  sendATCommand("AT+CGATT=1");
  
  // Wait for network attachment
  while (!checkNetworkAttachment()) {
    delay(5000);
  }
  
  Serial.println("Connected to NB-IoT network");
}

void loop() {
  // Read sensor data
  String data = "{\"temp\":23.5,\"hum\":65.2}";
  
  // Send data via UDP
  sendUDPData("server.example.com", 1234, data);
  
  // Enter PSM mode for power saving
  sendATCommand("AT+CPSMS=1,,,\"00000001\",\"00000001\"");
  
  delay(3600000); // 1 hour
}

void sendATCommand(String command) {
  nbiot.println(command);
  String response = nbiot.readString();
  Serial.println("AT Response: " + response);
}

bool checkNetworkAttachment() {
  nbiot.println("AT+CGATT?");
  String response = nbiot.readString();
  return response.indexOf("+CGATT: 1") != -1;
}

void sendUDPData(String server, int port, String data) {
  // Create UDP socket
  sendATCommand("AT+CSOC=1,2,1");
  
  // Connect to server
  String connectCmd = "AT+CSOCON=0,1234,\"" + server + "\"";
  sendATCommand(connectCmd);
  
  // Send data
  String sendCmd = "AT+CSOSEND=0," + String(data.length()) + ",\"" + data + "\"";
  sendATCommand(sendCmd);
  
  // Close socket
  sendATCommand("AT+CSOCL=0");
}
```

## Network Architecture and Protocols

### LoRaWAN Protocol Stack
```
Application Layer    | User Application
Presentation Layer   | Application Server
Session Layer       | Network Server
Transport Layer     | LoRaWAN MAC
Network Layer       | LoRaWAN PHY
Data Link Layer     | LoRa Modulation
Physical Layer      | Radio Hardware
```

### Message Types
- **Join Request/Accept**: Device activation
- **Unconfirmed Data**: No acknowledgment required
- **Confirmed Data**: Acknowledgment required
- **MAC Commands**: Network management

## Power Consumption Analysis

### Current Consumption Examples
```cpp
// LoRaWAN power consumption calculation
class PowerCalculator {
public:
  static float calculateBatteryLife(
    float batteryCapacity,    // mAh
    float sleepCurrent,       // mA
    float txCurrent,          // mA
    float txTime,             // seconds
    int messagesPerDay,
    float rxCurrent = 0,      // mA
    float rxTime = 0          // seconds
  ) {
    float dailyTxCharge = (txCurrent * txTime * messagesPerDay) / 3600;
    float dailyRxCharge = (rxCurrent * rxTime * messagesPerDay) / 3600;
    float dailySleepCharge = sleepCurrent * 24;
    
    float totalDailyCharge = dailyTxCharge + dailyRxCharge + dailySleepCharge;
    
    return batteryCapacity / totalDailyCharge; // Days
  }
};

// Example calculation
void calculateLoRaBatteryLife() {
  float batteryLife = PowerCalculator::calculateBatteryLife(
    2000,    // 2000 mAh battery
    0.002,   // 2 µA sleep current
    120,     // 120 mA TX current
    1.0,     // 1 second TX time
    24       // 24 messages per day
  );
  
  Serial.print("Battery life: ");
  Serial.print(batteryLife / 365, 1);
  Serial.println(" years");
}
```

## Application Examples

### Environmental Monitoring Station
```cpp
struct SensorData {
  float temperature;
  float humidity;
  float pressure;
  float batteryVoltage;
  uint16_t messageCounter;
};

void sendEnvironmentalData() {
  SensorData data;
  data.temperature = bme280.readTemperature();
  data.humidity = bme280.readHumidity();
  data.pressure = bme280.readPressure() / 100.0F;
  data.batteryVoltage = readBatteryVoltage();
  data.messageCounter = messageCount++;
  
  // Encode data efficiently
  uint8_t payload[12];
  encodeFloat16(payload, 0, data.temperature, -40, 85);
  encodeFloat16(payload + 2, 0, data.humidity, 0, 100);
  encodeFloat16(payload + 4, 0, data.pressure, 800, 1200);
  encodeFloat16(payload + 6, 0, data.batteryVoltage, 2.5, 4.2);
  payload[8] = data.messageCounter & 0xFF;
  payload[9] = (data.messageCounter >> 8) & 0xFF;
  
  sendLoRaMessage(payload, 10);
}

void encodeFloat16(uint8_t* buffer, int offset, float value, float min, float max) {
  uint16_t encoded = (uint16_t)((value - min) / (max - min) * 65535);
  buffer[offset] = encoded & 0xFF;
  buffer[offset + 1] = (encoded >> 8) & 0xFF;
}
```

### Asset Tracking Device
```cpp
struct LocationData {
  float latitude;
  float longitude;
  uint8_t satellites;
  float speed;
  uint16_t heading;
  uint8_t batteryLevel;
};

void sendLocationUpdate() {
  if (gps.location.isValid()) {
    LocationData location;
    location.latitude = gps.location.lat();
    location.longitude = gps.location.lng();
    location.satellites = gps.satellites.value();
    location.speed = gps.speed.kmph();
    location.heading = gps.course.deg();
    location.batteryLevel = readBatteryPercentage();
    
    // Efficient encoding for GPS coordinates
    uint8_t payload[15];
    encodeGPSCoordinate(payload, 0, location.latitude);
    encodeGPSCoordinate(payload + 4, 0, location.longitude);
    payload[8] = location.satellites;
    payload[9] = (uint8_t)location.speed;
    payload[10] = location.heading & 0xFF;
    payload[11] = (location.heading >> 8) & 0xFF;
    payload[12] = location.batteryLevel;
    
    sendLoRaMessage(payload, 13);
  }
}
```

## Best Practices

### Power Optimization
1. **Use efficient encoding** to minimize air time
2. **Implement adaptive data rates** based on conditions
3. **Use confirmed messages** only when necessary
4. **Optimize transmission timing** to avoid collisions
5. **Implement proper sleep modes** between transmissions

### Network Optimization
1. **Choose appropriate spreading factor** for range vs. speed
2. **Implement frequency hopping** where available
3. **Monitor network quality** and adjust parameters
4. **Use network analytics** to optimize performance

### Security Considerations
1. **Protect device keys** with secure storage
2. **Implement secure key provisioning**
3. **Monitor for replay attacks**
4. **Use end-to-end encryption** for sensitive data
5. **Regular security audits** and updates

## Future Trends

### Technology Evolution
- **5G integration** with massive IoT capabilities
- **Satellite LPWAN** for global coverage
- **Mesh networking** for extended range
- **AI-powered optimization** for network management
- **Energy harvesting** integration for perpetual operation

### Market Drivers
- **Smart city** deployments
- **Industrial IoT** automation
- **Environmental monitoring**
- **Supply chain tracking**
- **Precision agriculture**
