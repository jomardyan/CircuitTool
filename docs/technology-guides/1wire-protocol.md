# 1-Wire Protocol Guide

## Introduction

The 1-Wire protocol is a device communications bus system designed by Dallas Semiconductor (now part of Analog Devices) that provides low-speed data, signaling, and power over a single conductor. It's widely used for temperature sensors, memory devices, and other simple peripheral devices.

## Protocol Overview

### Key Features
- **Single data line**: Data, power, and ground on one wire
- **Parasitic power**: Devices can be powered from the data line
- **Unique 64-bit addressing**: Each device has a unique ROM code
- **Multi-drop capability**: Multiple devices on one bus
- **Master-slave architecture**: One master controls all communication
- **Distance capability**: Up to 100 meters with proper drivers

### Electrical Characteristics
- **Voltage Levels**: 0V (logic 0), 5V (logic 1)
- **Pull-up resistor**: 4.7kΩ typical
- **Open-drain outputs**: Devices pull line low
- **Parasitic power**: 5.5V stored in internal capacitor

## 1-Wire Timing

### Time Slots
All 1-Wire communication occurs in 60-90μs time slots:

#### Write 1 Time Slot
```
Master pulls line low for 1-15μs, then releases
___     ___________
   |___|           (60-120μs total)
   1-15μs
```

#### Write 0 Time Slot
```
Master pulls line low for 60-120μs
___________________
                   |___
   60-120μs
```

#### Read Time Slot
```
Master pulls line low for 1-15μs, then samples line
___     ___________
   |___|           Sample at 15μs
   1-15μs
```

### Reset and Presence Detection
```
Reset Pulse (480-960μs):
____________________                    ________
                    |__________________|
                    480-960μs    15-60μs
                              Presence Pulse
```

## Device Addressing

### 64-bit ROM Code Structure
```
[8-bit Family Code][48-bit Serial Number][8-bit CRC]
```

Example: 28-12A4B6C8D9E0-A1
- **28**: DS18B20 temperature sensor family code
- **12A4B6C8D9E0**: Unique serial number
- **A1**: CRC checksum

### Common Family Codes
| Code | Device Type |
|------|-------------|
| 01   | DS2401 Serial Number |
| 10   | DS18S20 Temperature |
| 12   | DS2406 Dual Switch |
| 23   | DS2433 EEPROM |
| 28   | DS18B20 Temperature |
| 3A   | DS2413 Dual Switch |

## Programming Examples

### Basic 1-Wire Arduino Library
```cpp
#include <OneWire.h>

// 1-Wire bus connected to digital pin 2
OneWire ds(2);

void setup() {
  Serial.begin(9600);
  Serial.println("1-Wire Device Scanner");
}

void loop() {
  byte addr[8];
  
  Serial.println("Searching for 1-Wire devices...");
  
  while (ds.search(addr)) {
    Serial.print("Found device: ");
    printAddress(addr);
    
    if (OneWire::crc8(addr, 7) != addr[7]) {
      Serial.println("CRC is not valid!");
      continue;
    }
    
    // Identify device type
    switch (addr[0]) {
      case 0x28:
        Serial.println("DS18B20 Temperature Sensor");
        readDS18B20(addr);
        break;
      case 0x10:
        Serial.println("DS18S20 Temperature Sensor");
        break;
      case 0x23:
        Serial.println("DS2433 EEPROM");
        break;
      default:
        Serial.println("Unknown device");
        break;
    }
    Serial.println();
  }
  
  ds.reset_search();
  delay(5000);
}

void printAddress(byte addr[8]) {
  for (int i = 0; i < 8; i++) {
    if (addr[i] < 16) Serial.print("0");
    Serial.print(addr[i], HEX);
    if (i < 7) Serial.print("-");
  }
  Serial.println();
}

void readDS18B20(byte addr[8]) {
  byte data[12];
  
  // Start temperature conversion
  ds.reset();
  ds.select(addr);
  ds.write(0x44); // Start conversion command
  
  delay(1000); // Wait for conversion (750ms max)
  
  // Read scratchpad
  ds.reset();
  ds.select(addr);
  ds.write(0xBE); // Read scratchpad command
  
  for (int i = 0; i < 9; i++) {
    data[i] = ds.read();
  }
  
  // Verify CRC
  if (OneWire::crc8(data, 8) != data[8]) {
    Serial.println("CRC error in temperature reading");
    return;
  }
  
  // Convert temperature
  int16_t raw = (data[1] << 8) | data[0];
  float celsius = (float)raw / 16.0;
  
  Serial.print("Temperature: ");
  Serial.print(celsius);
  Serial.println("°C");
}
```

### Advanced Temperature Sensor Array
```cpp
#include <OneWire.h>
#include <DallasTemperature.h>

#define ONE_WIRE_BUS 2
#define TEMPERATURE_PRECISION 12

OneWire oneWire(ONE_WIRE_BUS);
DallasTemperature sensors(&oneWire);

struct TemperatureSensor {
  DeviceAddress address;
  String name;
  float lastTemperature;
  bool isConnected;
};

class TemperatureArray {
private:
  TemperatureSensor sensorArray[10];
  int sensorCount = 0;
  
public:
  void begin() {
    sensors.begin();
    
    Serial.print("Found ");
    Serial.print(sensors.getDeviceCount());
    Serial.println(" temperature sensors");
    
    // Discover and register sensors
    DeviceAddress tempAddr;
    for (int i = 0; i < sensors.getDeviceCount(); i++) {
      if (sensors.getAddress(tempAddr, i)) {
        addSensor(tempAddr, "Sensor_" + String(i + 1));
      }
    }
    
    // Set precision for all sensors
    for (int i = 0; i < sensorCount; i++) {
      sensors.setResolution(sensorArray[i].address, TEMPERATURE_PRECISION);
    }
  }
  
  void addSensor(DeviceAddress addr, String name) {
    if (sensorCount < 10) {
      memcpy(sensorArray[sensorCount].address, addr, 8);
      sensorArray[sensorCount].name = name;
      sensorArray[sensorCount].isConnected = true;
      sensorCount++;
      
      Serial.print("Added sensor: ");
      Serial.print(name);
      Serial.print(" Address: ");
      printAddress(addr);
    }
  }
  
  void readAllSensors() {
    sensors.requestTemperatures(); // Send command to all sensors
    
    Serial.println("=== Temperature Readings ===");
    for (int i = 0; i < sensorCount; i++) {
      float temp = sensors.getTempC(sensorArray[i].address);
      
      if (temp == DEVICE_DISCONNECTED_C) {
        Serial.print(sensorArray[i].name);
        Serial.println(": DISCONNECTED");
        sensorArray[i].isConnected = false;
      } else {
        sensorArray[i].lastTemperature = temp;
        sensorArray[i].isConnected = true;
        
        Serial.print(sensorArray[i].name);
        Serial.print(": ");
        Serial.print(temp, 2);
        Serial.println("°C");
      }
    }
    Serial.println();
  }
  
  float getTemperature(int index) {
    if (index >= 0 && index < sensorCount && sensorArray[index].isConnected) {
      return sensorArray[index].lastTemperature;
    }
    return DEVICE_DISCONNECTED_C;
  }
  
  bool isSensorConnected(int index) {
    return (index >= 0 && index < sensorCount && sensorArray[index].isConnected);
  }
  
  void printSensorInfo() {
    Serial.println("=== Sensor Information ===");
    for (int i = 0; i < sensorCount; i++) {
      Serial.print("Index: "); Serial.println(i);
      Serial.print("Name: "); Serial.println(sensorArray[i].name);
      Serial.print("Address: "); printAddress(sensorArray[i].address);
      Serial.print("Resolution: "); 
      Serial.print(sensors.getResolution(sensorArray[i].address));
      Serial.println(" bits");
      Serial.print("Status: ");
      Serial.println(sensorArray[i].isConnected ? "Connected" : "Disconnected");
      Serial.println();
    }
  }
  
private:
  void printAddress(DeviceAddress addr) {
    for (int i = 0; i < 8; i++) {
      if (addr[i] < 16) Serial.print("0");
      Serial.print(addr[i], HEX);
      if (i < 7) Serial.print(":");
    }
    Serial.println();
  }
};

TemperatureArray tempArray;

void setup() {
  Serial.begin(9600);
  Serial.println("Advanced 1-Wire Temperature Array");
  
  tempArray.begin();
  tempArray.printSensorInfo();
}

void loop() {
  tempArray.readAllSensors();
  
  // Example of using individual sensor data
  for (int i = 0; i < 5; i++) {
    if (tempArray.isSensorConnected(i)) {
      float temp = tempArray.getTemperature(i);
      
      // Temperature threshold checking
      if (temp > 30.0) {
        Serial.print("WARNING: Sensor ");
        Serial.print(i);
        Serial.print(" temperature high: ");
        Serial.println(temp);
      }
    }
  }
  
  delay(5000);
}
```

### 1-Wire EEPROM Interface
```cpp
class OneWireEEPROM {
private:
  OneWire* wire;
  byte address[8];
  
public:
  OneWireEEPROM(OneWire* ow, byte addr[8]) {
    wire = ow;
    memcpy(address, addr, 8);
  }
  
  bool writeByte(uint16_t memAddr, byte data) {
    if (!wire->reset()) return false;
    
    wire->select(address);
    wire->write(0x0F); // Write scratchpad command
    wire->write(memAddr & 0xFF); // Address low
    wire->write((memAddr >> 8) & 0xFF); // Address high
    wire->write(data); // Data byte
    
    // Read CRC
    uint16_t crc = wire->read() | (wire->read() << 8);
    
    if (!verifyCRC16(memAddr, data, crc)) {
      return false;
    }
    
    // Copy scratchpad to EEPROM
    if (!wire->reset()) return false;
    
    wire->select(address);
    wire->write(0x55); // Copy scratchpad command
    wire->write(memAddr & 0xFF);
    wire->write((memAddr >> 8) & 0xFF);
    wire->write(0x1F); // Authorization code
    
    // Wait for copy completion
    delay(10);
    
    return true;
  }
  
  byte readByte(uint16_t memAddr) {
    if (!wire->reset()) return 0xFF;
    
    wire->select(address);
    wire->write(0xF0); // Read memory command
    wire->write(memAddr & 0xFF); // Address low
    wire->write((memAddr >> 8) & 0xFF); // Address high
    
    return wire->read();
  }
  
  bool writeBlock(uint16_t startAddr, byte* data, int length) {
    for (int i = 0; i < length; i++) {
      if (!writeByte(startAddr + i, data[i])) {
        return false;
      }
      delay(5); // Allow EEPROM write time
    }
    return true;
  }
  
  bool readBlock(uint16_t startAddr, byte* buffer, int length) {
    if (!wire->reset()) return false;
    
    wire->select(address);
    wire->write(0xF0); // Read memory command
    wire->write(startAddr & 0xFF);
    wire->write((startAddr >> 8) & 0xFF);
    
    for (int i = 0; i < length; i++) {
      buffer[i] = wire->read();
    }
    
    return true;
  }
  
private:
  bool verifyCRC16(uint16_t addr, byte data, uint16_t receivedCRC) {
    // Simplified CRC16 verification
    // In practice, implement full CRC16 calculation
    return true; // Placeholder
  }
};

// Usage example
void eepromExample() {
  OneWire ds(2);
  byte eepromAddr[8] = {0x23, 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE};
  
  OneWireEEPROM eeprom(&ds, eepromAddr);
  
  // Write some data
  String message = "Hello 1-Wire!";
  eeprom.writeBlock(0, (byte*)message.c_str(), message.length());
  
  // Read it back
  byte buffer[20];
  eeprom.readBlock(0, buffer, message.length());
  buffer[message.length()] = 0; // Null terminate
  
  Serial.print("Read from EEPROM: ");
  Serial.println((char*)buffer);
}
```

## Custom 1-Wire Device Implementation

### Software 1-Wire Master
```cpp
class SoftwareOneWire {
private:
  int pin;
  
public:
  SoftwareOneWire(int dataPin) : pin(dataPin) {
    pinMode(pin, INPUT_PULLUP);
  }
  
  bool reset() {
    // Pull line low for reset pulse
    pinMode(pin, OUTPUT);
    digitalWrite(pin, LOW);
    delayMicroseconds(480);
    
    // Release line and wait for presence pulse
    pinMode(pin, INPUT_PULLUP);
    delayMicroseconds(70);
    
    bool presence = !digitalRead(pin);
    delayMicroseconds(410);
    
    return presence;
  }
  
  void writeBit(bool bit) {
    pinMode(pin, OUTPUT);
    digitalWrite(pin, LOW);
    
    if (bit) {
      delayMicroseconds(6);
      pinMode(pin, INPUT_PULLUP);
      delayMicroseconds(64);
    } else {
      delayMicroseconds(60);
      pinMode(pin, INPUT_PULLUP);
      delayMicroseconds(10);
    }
  }
  
  bool readBit() {
    pinMode(pin, OUTPUT);
    digitalWrite(pin, LOW);
    delayMicroseconds(6);
    
    pinMode(pin, INPUT_PULLUP);
    delayMicroseconds(9);
    
    bool bit = digitalRead(pin);
    delayMicroseconds(55);
    
    return bit;
  }
  
  void writeByte(byte data) {
    for (int i = 0; i < 8; i++) {
      writeBit(data & 1);
      data >>= 1;
    }
  }
  
  byte readByte() {
    byte result = 0;
    
    for (int i = 0; i < 8; i++) {
      result >>= 1;
      if (readBit()) {
        result |= 0x80;
      }
    }
    
    return result;
  }
  
  void select(byte addr[8]) {
    writeByte(0x55); // Match ROM command
    for (int i = 0; i < 8; i++) {
      writeByte(addr[i]);
    }
  }
  
  void skip() {
    writeByte(0xCC); // Skip ROM command
  }
  
  bool search(byte addr[8]) {
    // Implement ROM search algorithm
    // This is a simplified version
    static byte lastAddr[8] = {0};
    static int lastDiscrepancy = 0;
    
    // ROM search implementation would go here
    // Returns true if device found, false if search complete
    
    return false; // Placeholder
  }
};
```

## Power Management and Parasitic Power

### Parasitic Power Configuration
```cpp
class ParasiticPowerManager {
private:
  int dataPin;
  int powerPin; // Optional strong pull-up pin
  
public:
  ParasiticPowerManager(int data, int power = -1) 
    : dataPin(data), powerPin(power) {
    pinMode(dataPin, INPUT_PULLUP);
    if (powerPin >= 0) {
      pinMode(powerPin, OUTPUT);
      digitalWrite(powerPin, LOW);
    }
  }
  
  void enableStrongPullup() {
    if (powerPin >= 0) {
      digitalWrite(powerPin, HIGH);
    }
  }
  
  void disableStrongPullup() {
    if (powerPin >= 0) {
      digitalWrite(powerPin, LOW);
    }
  }
  
  void performConversion() {
    OneWire ds(dataPin);
    
    // Start conversion on all sensors
    ds.reset();
    ds.skip();
    ds.write(0x44); // Convert T command
    
    // Enable strong pull-up for parasitic power
    enableStrongPullup();
    
    // Wait for conversion (750ms max for 12-bit)
    delay(750);
    
    // Disable strong pull-up
    disableStrongPullup();
  }
  
  bool checkPowerSupply(byte addr[8]) {
    OneWire ds(dataPin);
    
    ds.reset();
    ds.select(addr);
    ds.write(0xB4); // Read power supply command
    
    return ds.read_bit(); // 1 = external power, 0 = parasitic
  }
};
```

## Troubleshooting and Diagnostics

### 1-Wire Bus Diagnostics
```cpp
class OneWireDiagnostics {
private:
  OneWire* wire;
  
public:
  OneWireDiagnostics(OneWire* ow) : wire(ow) {}
  
  void runDiagnostics() {
    Serial.println("=== 1-Wire Bus Diagnostics ===");
    
    // Test bus presence
    if (wire->reset()) {
      Serial.println("✓ Bus presence detected");
    } else {
      Serial.println("✗ No devices detected on bus");
      Serial.println("  Check connections and pull-up resistor");
      return;
    }
    
    // Count devices
    int deviceCount = countDevices();
    Serial.print("Device count: ");
    Serial.println(deviceCount);
    
    // Check for shorts
    checkForShorts();
    
    // Test timing
    testTiming();
    
    // Power supply test
    testPowerSupply();
  }
  
private:
  int countDevices() {
    int count = 0;
    byte addr[8];
    
    wire->reset_search();
    while (wire->search(addr)) {
      count++;
    }
    
    return count;
  }
  
  void checkForShorts() {
    // Pull line low and check if it stays low
    pinMode(2, OUTPUT); // Assuming pin 2
    digitalWrite(2, LOW);
    delayMicroseconds(100);
    pinMode(2, INPUT);
    
    if (digitalRead(2) == LOW) {
      Serial.println("✗ Possible short circuit detected");
    } else {
      Serial.println("✓ No short circuit detected");
    }
  }
  
  void testTiming() {
    unsigned long start = micros();
    wire->reset();
    unsigned long resetTime = micros() - start;
    
    Serial.print("Reset pulse timing: ");
    Serial.print(resetTime);
    Serial.println(" μs");
    
    if (resetTime < 480 || resetTime > 960) {
      Serial.println("⚠ Reset timing may be incorrect");
    }
  }
  
  void testPowerSupply() {
    byte addr[8];
    
    wire->reset_search();
    while (wire->search(addr)) {
      if (addr[0] == 0x28) { // DS18B20
        wire->reset();
        wire->select(addr);
        wire->write(0xB4); // Read power supply
        
        bool externalPower = wire->read_bit();
        Serial.print("Device ");
        printAddress(addr);
        Serial.print(" power: ");
        Serial.println(externalPower ? "External" : "Parasitic");
      }
    }
  }
  
  void printAddress(byte addr[8]) {
    for (int i = 0; i < 8; i++) {
      if (addr[i] < 16) Serial.print("0");
      Serial.print(addr[i], HEX);
    }
  }
};
```

## Best Practices

### Network Design Guidelines

1. **Bus Length**: Keep total bus length under 300m for reliable operation
2. **Branch Length**: Limit individual branches to 25m
3. **Pull-up Resistor**: Use 4.7kΩ for short networks, 2.2kΩ for longer ones
4. **Cable Type**: Use twisted pair cable for noise immunity
5. **Star vs. Linear**: Linear topology preferred over star configuration

### Software Implementation

1. **Error Handling**: Always check CRC and implement retry logic
2. **Timing Critical**: Use hardware timers for precise timing
3. **Parasitic Power**: Allow extra time for capacitor charging
4. **Device Management**: Maintain device list and monitor connectivity
5. **Temperature Sensors**: Account for conversion time based on resolution

### Common Issues and Solutions

| Problem | Symptoms | Solution |
|---------|----------|----------|
| No devices found | Search returns no results | Check wiring, pull-up resistor |
| CRC errors | Intermittent read failures | Improve wiring, add filtering |
| Timing issues | Unreliable communication | Use hardware timers, check CPU load |
| Power problems | Devices drop out | Verify power supply, use strong pull-up |
| Long bus issues | Communication fails | Use active drivers, reduce capacitance |

## Advanced Applications

### Multi-Zone Temperature Monitoring
```cpp
struct ThermalZone {
  String name;
  byte sensorAddresses[5][8];
  int sensorCount;
  float targetTemp;
  float tolerance;
  bool heatingEnabled;
  bool coolingEnabled;
};

class ThermalController {
private:
  OneWire wire;
  ThermalZone zones[4];
  int zoneCount = 0;
  
public:
  void addZone(String name, float target, float tol) {
    if (zoneCount < 4) {
      zones[zoneCount].name = name;
      zones[zoneCount].targetTemp = target;
      zones[zoneCount].tolerance = tol;
      zones[zoneCount].sensorCount = 0;
      zoneCount++;
    }
  }
  
  void controlTemperature() {
    for (int z = 0; z < zoneCount; z++) {
      float avgTemp = getZoneTemperature(z);
      
      if (avgTemp < zones[z].targetTemp - zones[z].tolerance) {
        // Enable heating
        zones[z].heatingEnabled = true;
        zones[z].coolingEnabled = false;
      } else if (avgTemp > zones[z].targetTemp + zones[z].tolerance) {
        // Enable cooling
        zones[z].heatingEnabled = false;
        zones[z].coolingEnabled = true;
      } else {
        // Temperature OK
        zones[z].heatingEnabled = false;
        zones[z].coolingEnabled = false;
      }
    }
  }
  
private:
  float getZoneTemperature(int zone) {
    float total = 0;
    int validReadings = 0;
    
    for (int s = 0; s < zones[zone].sensorCount; s++) {
      float temp = readTemperature(zones[zone].sensorAddresses[s]);
      if (temp != DEVICE_DISCONNECTED_C) {
        total += temp;
        validReadings++;
      }
    }
    
    return validReadings > 0 ? total / validReadings : 0;
  }
  
  float readTemperature(byte addr[8]) {
    // Implementation similar to previous examples
    return 25.0; // Placeholder
  }
};
```
