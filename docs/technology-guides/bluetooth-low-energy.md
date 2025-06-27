# Bluetooth® Low Energy (BLE) Guide

## Introduction to Bluetooth Low Energy

Bluetooth Low Energy (BLE), also known as Bluetooth Smart, is a wireless personal area network technology designed for applications that require minimal power consumption while maintaining reasonable communication range and data throughput. BLE is part of the Bluetooth 4.0+ specification and is optimized for devices that need to operate for months or years on a single battery.

## BLE Architecture Overview

### Key Features
- **Ultra-low power consumption**: Designed for battery-powered devices
- **Short range communication**: Typically 10-30 meters
- **Low data throughput**: Optimized for small, infrequent data transfers
- **Fast connection establishment**: Sub-second connection times
- **Smartphone compatibility**: Built into iOS, Android, and most modern devices

### BLE vs Classic Bluetooth

| Feature | Classic Bluetooth | Bluetooth Low Energy |
|---------|------------------|---------------------|
| Power consumption | High (mW) | Ultra-low (µW) |
| Data rate | Up to 2.1 Mbps | Up to 1 Mbps |
| Range | 10m (Class 2) | 10-30m |
| Connection time | ~6 seconds | <100ms |
| Peak current | ~30mA | ~15mA |
| Average current | ~1mA | 1-50µA |
| Application | Audio, file transfer | IoT sensors, health monitors |

## BLE Protocol Stack

### Generic Access Profile (GAP)
- **Roles**: Central (scanner/master) and Peripheral (advertiser/slave)
- **Advertising**: Peripheral broadcasts presence and services
- **Scanning**: Central discovers advertising peripherals
- **Connection**: Establishment and parameter negotiation

### Generic Attribute Protocol (GATT)
- **Services**: Collections of related functionality
- **Characteristics**: Data endpoints within services
- **Descriptors**: Metadata about characteristics
- **Client-Server**: Central acts as client, peripheral as server

### Advertising and Connection States
```
Standby → Advertising → Connected → Standby
    ↑        ↓              ↓
    ← ← ← ← Standby ← ← ← ← ←
```

## Arduino BLE Programming

### ESP32 BLE Server Example
```cpp
#include "BLEDevice.h"
#include "BLEServer.h"
#include "BLEUtils.h"
#include "BLE2902.h"

// Service and characteristic UUIDs
#define SERVICE_UUID        "12345678-1234-1234-1234-123456789abc"
#define CHARACTERISTIC_UUID "87654321-4321-4321-4321-cba987654321"

BLEServer* pServer = NULL;
BLECharacteristic* pCharacteristic = NULL;
bool deviceConnected = false;
bool oldDeviceConnected = false;

class MyServerCallbacks: public BLEServerCallbacks {
    void onConnect(BLEServer* pServer) {
      deviceConnected = true;
      Serial.println("Device connected");
    };

    void onDisconnect(BLEServer* pServer) {
      deviceConnected = false;
      Serial.println("Device disconnected");
    }
};

class MyCharacteristicCallbacks: public BLECharacteristicCallbacks {
    void onWrite(BLECharacteristic *pCharacteristic) {
      std::string value = pCharacteristic->getValue();
      
      if (value.length() > 0) {
        Serial.print("Received: ");
        for (int i = 0; i < value.length(); i++) {
          Serial.print(value[i]);
        }
        Serial.println();
        
        // Echo response
        String response = "Echo: " + String(value.c_str());
        pCharacteristic->setValue(response.c_str());
        pCharacteristic->notify();
      }
    }
};

void setup() {
  Serial.begin(115200);
  
  // Initialize BLE
  BLEDevice::init("ESP32-BLE-Server");
  
  // Create BLE Server
  pServer = BLEDevice::createServer();
  pServer->setCallbacks(new MyServerCallbacks());
  
  // Create BLE Service
  BLEService *pService = pServer->createService(SERVICE_UUID);
  
  // Create BLE Characteristic
  pCharacteristic = pService->createCharacteristic(
                      CHARACTERISTIC_UUID,
                      BLECharacteristic::PROPERTY_READ |
                      BLECharacteristic::PROPERTY_WRITE |
                      BLECharacteristic::PROPERTY_NOTIFY
                    );
  
  pCharacteristic->setCallbacks(new MyCharacteristicCallbacks());
  pCharacteristic->addDescriptor(new BLE2902());
  
  // Start the service
  pService->start();
  
  // Start advertising
  BLEAdvertising *pAdvertising = BLEDevice::getAdvertising();
  pAdvertising->addServiceUUID(SERVICE_UUID);
  pAdvertising->setScanResponse(false);
  pAdvertising->setMinPreferred(0x06);
  pAdvertising->setMinPreferred(0x12);
  BLEDevice::startAdvertising();
  
  Serial.println("BLE Server started, waiting for connections...");
}

void loop() {
  // Handle connection state changes
  if (!deviceConnected && oldDeviceConnected) {
    delay(500); // Give time for disconnect to complete
    pServer->startAdvertising(); // Restart advertising
    Serial.println("Restarting advertising");
    oldDeviceConnected = deviceConnected;
  }
  
  if (deviceConnected && !oldDeviceConnected) {
    oldDeviceConnected = deviceConnected;
  }
  
  // Send sensor data every 5 seconds if connected
  static unsigned long lastSensorRead = 0;
  if (deviceConnected && millis() - lastSensorRead > 5000) {
    // Read sensor data
    float temperature = 23.5 + random(-50, 50) / 10.0; // Simulate temperature
    float humidity = 65.0 + random(-100, 100) / 10.0;  // Simulate humidity
    
    // Format as JSON
    String sensorData = "{\"temp\":" + String(temperature, 1) + 
                       ",\"hum\":" + String(humidity, 1) + "}";
    
    pCharacteristic->setValue(sensorData.c_str());
    pCharacteristic->notify();
    
    Serial.println("Sent: " + sensorData);
    lastSensorRead = millis();
  }
  
  delay(100);
}
```

### ESP32 BLE Client (Scanner) Example
```cpp
#include "BLEDevice.h"
#include "BLEScan.h"
#include "BLEAdvertisedDevice.h"
#include "BLEClient.h"

// Service and characteristic UUIDs (must match server)
#define SERVICE_UUID        "12345678-1234-1234-1234-123456789abc"
#define CHARACTERISTIC_UUID "87654321-4321-4321-4321-cba987654321"

BLEScan* pBLEScan;
BLEClient* pClient;
BLERemoteCharacteristic* pRemoteCharacteristic;

bool connected = false;
bool doConnect = false;
BLEAdvertisedDevice* myDevice;

// Callback for scan results
class MyAdvertisedDeviceCallbacks: public BLEAdvertisedDeviceCallbacks {
  void onResult(BLEAdvertisedDevice advertisedDevice) {
    Serial.print("Found device: ");
    Serial.println(advertisedDevice.toString().c_str());
    
    if (advertisedDevice.haveServiceUUID() && 
        advertisedDevice.isAdvertisingService(BLEUUID(SERVICE_UUID))) {
      
      Serial.println("Found our device!");
      BLEDevice::getScan()->stop();
      myDevice = new BLEAdvertisedDevice(advertisedDevice);
      doConnect = true;
    }
  }
};

// Callback for notifications
static void notifyCallback(BLERemoteCharacteristic* pBLERemoteCharacteristic,
                          uint8_t* pData, size_t length, bool isNotify) {
  Serial.print("Notification received: ");
  for (int i = 0; i < length; i++) {
    Serial.print((char)pData[i]);
  }
  Serial.println();
}

// Callback for connection events
class MyClientCallback : public BLEClientCallbacks {
  void onConnect(BLEClient* pclient) {
    Serial.println("Connected to server");
  }

  void onDisconnect(BLEClient* pclient) {
    connected = false;
    Serial.println("Disconnected from server");
  }
};

bool connectToServer() {
  Serial.print("Forming a connection to ");
  Serial.println(myDevice->getAddress().toString().c_str());
  
  pClient = BLEDevice::createClient();
  pClient->setClientCallbacks(new MyClientCallback());
  
  // Connect to the remote BLE Server
  pClient->connect(myDevice);
  Serial.println("Connected to server");
  
  // Obtain a reference to the service
  BLERemoteService* pRemoteService = pClient->getService(SERVICE_UUID);
  if (pRemoteService == nullptr) {
    Serial.print("Failed to find our service UUID: ");
    Serial.println(SERVICE_UUID);
    pClient->disconnect();
    return false;
  }
  Serial.println("Found our service");
  
  // Obtain a reference to the characteristic
  pRemoteCharacteristic = pRemoteService->getCharacteristic(CHARACTERISTIC_UUID);
  if (pRemoteCharacteristic == nullptr) {
    Serial.print("Failed to find our characteristic UUID: ");
    Serial.println(CHARACTERISTIC_UUID);
    pClient->disconnect();
    return false;
  }
  Serial.println("Found our characteristic");
  
  // Read the value of the characteristic
  if(pRemoteCharacteristic->canRead()) {
    std::string value = pRemoteCharacteristic->readValue();
    Serial.print("The characteristic value was: ");
    Serial.println(value.c_str());
  }
  
  // Register for notifications
  if(pRemoteCharacteristic->canNotify()) {
    pRemoteCharacteristic->registerForNotify(notifyCallback);
    Serial.println("Registered for notifications");
  }
  
  connected = true;
  return true;
}

void setup() {
  Serial.begin(115200);
  Serial.println("Starting BLE Client...");
  
  BLEDevice::init("");
  
  // Retrieve a Scanner and set the callback
  pBLEScan = BLEDevice::getScan();
  pBLEScan->setAdvertisedDeviceCallbacks(new MyAdvertisedDeviceCallbacks());
  pBLEScan->setInterval(1349);
  pBLEScan->setWindow(449);
  pBLEScan->setActiveScan(true);
  pBLEScan->start(5, false);
}

void loop() {
  if (doConnect == true) {
    if (connectToServer()) {
      Serial.println("We are now connected to the BLE Server.");
    } else {
      Serial.println("We have failed to connect to the server; there is nothing more we will do.");
    }
    doConnect = false;
  }
  
  if (connected) {
    // Send data to server every 10 seconds
    static unsigned long lastSend = 0;
    if (millis() - lastSend > 10000) {
      String message = "Hello from client: " + String(millis());
      pRemoteCharacteristic->writeValue(message.c_str(), message.length());
      Serial.println("Sent: " + message);
      lastSend = millis();
    }
  } else if (!doConnect) {
    // Restart scanning if not connected
    pBLEScan->start(5, false);
    delay(1000);
  }
  
  delay(100);
}
```

### Advanced BLE Sensor Hub
```cpp
#include "BLEDevice.h"
#include "BLEServer.h"
#include "BLEUtils.h"
#include "BLE2902.h"
#include "DHT.h"
#include "ArduinoJson.h"

#define DHT_PIN 4
#define DHT_TYPE DHT22

DHT dht(DHT_PIN, DHT_TYPE);

// Multiple service UUIDs for different sensor types
#define ENVIRONMENTAL_SERVICE_UUID "12345678-1234-1234-1234-123456789001"
#define MOTION_SERVICE_UUID        "12345678-1234-1234-1234-123456789002"
#define BATTERY_SERVICE_UUID       "12345678-1234-1234-1234-123456789003"

// Characteristic UUIDs
#define TEMPERATURE_CHAR_UUID      "87654321-4321-4321-4321-000000000001"
#define HUMIDITY_CHAR_UUID         "87654321-4321-4321-4321-000000000002"
#define PRESSURE_CHAR_UUID         "87654321-4321-4321-4321-000000000003"
#define MOTION_CHAR_UUID           "87654321-4321-4321-4321-000000000004"
#define BATTERY_CHAR_UUID          "87654321-4321-4321-4321-000000000005"

class BLESensorHub {
private:
  BLEServer* pServer;
  BLEService* pEnvService;
  BLEService* pMotionService;
  BLEService* pBatteryService;
  
  BLECharacteristic* pTempCharacteristic;
  BLECharacteristic* pHumidityCharacteristic;
  BLECharacteristic* pPressureCharacteristic;
  BLECharacteristic* pMotionCharacteristic;
  BLECharacteristic* pBatteryCharacteristic;
  
  bool deviceConnected = false;
  int connectedDevices = 0;
  
  struct SensorData {
    float temperature;
    float humidity;
    float pressure;
    bool motionDetected;
    uint8_t batteryLevel;
    unsigned long timestamp;
  };
  
  SensorData currentData;
  
public:
  void begin(String deviceName = "BLE-SensorHub") {
    Serial.println("Initializing BLE Sensor Hub...");
    
    // Initialize sensors
    dht.begin();
    
    // Initialize BLE
    BLEDevice::init(deviceName.c_str());
    
    // Create BLE Server
    pServer = BLEDevice::createServer();
    pServer->setCallbacks(new ServerCallbacks(this));
    
    setupServices();
    startAdvertising();
    
    Serial.println("BLE Sensor Hub ready!");
  }
  
  void loop() {
    updateSensorData();
    
    if (deviceConnected) {
      publishSensorData();
    }
    
    handlePowerManagement();
    delay(1000); // 1 second update rate
  }
  
private:
  void setupServices() {
    // Environmental Service
    pEnvService = pServer->createService(ENVIRONMENTAL_SERVICE_UUID);
    
    pTempCharacteristic = pEnvService->createCharacteristic(
      TEMPERATURE_CHAR_UUID,
      BLECharacteristic::PROPERTY_READ | BLECharacteristic::PROPERTY_NOTIFY
    );
    pTempCharacteristic->addDescriptor(new BLE2902());
    
    pHumidityCharacteristic = pEnvService->createCharacteristic(
      HUMIDITY_CHAR_UUID,
      BLECharacteristic::PROPERTY_READ | BLECharacteristic::PROPERTY_NOTIFY
    );
    pHumidityCharacteristic->addDescriptor(new BLE2902());
    
    pPressureCharacteristic = pEnvService->createCharacteristic(
      PRESSURE_CHAR_UUID,
      BLECharacteristic::PROPERTY_READ | BLECharacteristic::PROPERTY_NOTIFY
    );
    pPressureCharacteristic->addDescriptor(new BLE2902());
    
    // Motion Service
    pMotionService = pServer->createService(MOTION_SERVICE_UUID);
    
    pMotionCharacteristic = pMotionService->createCharacteristic(
      MOTION_CHAR_UUID,
      BLECharacteristic::PROPERTY_READ | BLECharacteristic::PROPERTY_NOTIFY
    );
    pMotionCharacteristic->addDescriptor(new BLE2902());
    
    // Battery Service
    pBatteryService = pServer->createService(BATTERY_SERVICE_UUID);
    
    pBatteryCharacteristic = pBatteryService->createCharacteristic(
      BATTERY_CHAR_UUID,
      BLECharacteristic::PROPERTY_READ | BLECharacteristic::PROPERTY_NOTIFY
    );
    pBatteryCharacteristic->addDescriptor(new BLE2902());
    
    // Start services
    pEnvService->start();
    pMotionService->start();
    pBatteryService->start();
  }
  
  void startAdvertising() {
    BLEAdvertising *pAdvertising = BLEDevice::getAdvertising();
    
    // Add service UUIDs to advertisement
    pAdvertising->addServiceUUID(ENVIRONMENTAL_SERVICE_UUID);
    pAdvertising->addServiceUUID(MOTION_SERVICE_UUID);
    pAdvertising->addServiceUUID(BATTERY_SERVICE_UUID);
    
    // Set advertising parameters
    pAdvertising->setScanResponse(true);
    pAdvertising->setMinPreferred(0x06);  // Functions that help with iPhone connections issue
    pAdvertising->setMinPreferred(0x12);
    
    BLEDevice::startAdvertising();
    Serial.println("Started advertising");
  }
  
  void updateSensorData() {
    // Read DHT22 sensor
    currentData.temperature = dht.readTemperature();
    currentData.humidity = dht.readHumidity();
    
    // Simulate pressure sensor (replace with actual sensor)
    currentData.pressure = 1013.25 + random(-50, 50) / 10.0;
    
    // Simulate motion detection (replace with actual PIR sensor)
    currentData.motionDetected = (random(100) < 10); // 10% chance
    
    // Read battery level (ADC voltage divider)
    float batteryVoltage = analogRead(A0) * 3.3 / 4095.0 * 2; // Voltage divider
    currentData.batteryLevel = map(batteryVoltage * 100, 300, 420, 0, 100); // 3.0V-4.2V → 0-100%
    currentData.batteryLevel = constrain(currentData.batteryLevel, 0, 100);
    
    currentData.timestamp = millis();
  }
  
  void publishSensorData() {
    // Temperature
    if (!isnan(currentData.temperature)) {
      String tempStr = String(currentData.temperature, 1);
      pTempCharacteristic->setValue(tempStr.c_str());
      pTempCharacteristic->notify();
    }
    
    // Humidity
    if (!isnan(currentData.humidity)) {
      String humStr = String(currentData.humidity, 1);
      pHumidityCharacteristic->setValue(humStr.c_str());
      pHumidityCharacteristic->notify();
    }
    
    // Pressure
    String pressStr = String(currentData.pressure, 1);
    pPressureCharacteristic->setValue(pressStr.c_str());
    pPressureCharacteristic->notify();
    
    // Motion
    String motionStr = currentData.motionDetected ? "1" : "0";
    pMotionCharacteristic->setValue(motionStr.c_str());
    pMotionCharacteristic->notify();
    
    // Battery
    String battStr = String(currentData.batteryLevel);
    pBatteryCharacteristic->setValue(battStr.c_str());
    pBatteryCharacteristic->notify();
    
    // Debug output
    Serial.printf("T:%.1f H:%.1f P:%.1f M:%d B:%d%%\n",
                  currentData.temperature, currentData.humidity, 
                  currentData.pressure, currentData.motionDetected,
                  currentData.batteryLevel);
  }
  
  void handlePowerManagement() {
    // Implement power saving based on battery level
    if (currentData.batteryLevel < 10) {
      // Critical battery - reduce update rate
      delay(4000); // Additional 4 second delay
    } else if (currentData.batteryLevel < 30) {
      // Low battery - moderate power saving
      delay(1000); // Additional 1 second delay
    }
    
    // Deep sleep could be implemented here for ultra-low power operation
  }
  
  class ServerCallbacks: public BLEServerCallbacks {
  private:
    BLESensorHub* hub;
    
  public:
    ServerCallbacks(BLESensorHub* h) : hub(h) {}
    
    void onConnect(BLEServer* pServer) {
      hub->deviceConnected = true;
      hub->connectedDevices++;
      Serial.printf("Device connected. Total: %d\n", hub->connectedDevices);
    }
    
    void onDisconnect(BLEServer* pServer) {
      hub->deviceConnected = false;
      hub->connectedDevices--;
      Serial.printf("Device disconnected. Total: %d\n", hub->connectedDevices);
      
      if (hub->connectedDevices == 0) {
        delay(500); // Give time for disconnect to complete
        pServer->startAdvertising(); // Restart advertising
        Serial.println("Restarting advertising");
      }
    }
  };
};

BLESensorHub sensorHub;

void setup() {
  Serial.begin(115200);
  sensorHub.begin("ESP32-SensorHub");
}

void loop() {
  sensorHub.loop();
}
```

## BLE Beacons and iBeacon Implementation

### iBeacon Transmitter
```cpp
#include "BLEDevice.h"
#include "BLEUtils.h"
#include "BLEBeacon.h"
#include "esp_sleep.h"

class iBeaconTransmitter {
private:
  BLEAdvertising *pAdvertising;
  String uuid;
  uint16_t major;
  uint16_t minor;
  int8_t txPower;
  
public:
  iBeaconTransmitter(String beaconUUID, uint16_t majorID, uint16_t minorID, int8_t power = -59) {
    uuid = beaconUUID;
    major = majorID;
    minor = minorID;
    txPower = power;
  }
  
  void begin() {
    BLEDevice::init("iBeacon");
    
    // Create BLE Beacon
    BLEBeacon oBeacon = BLEBeacon();
    oBeacon.setManufacturerId(0x004C); // Apple
    oBeacon.setProximityUUID(BLEUUID(uuid.c_str()));
    oBeacon.setMajor(major);
    oBeacon.setMinor(minor);
    oBeacon.setSignalPower(txPower);
    
    // Create advertising data
    BLEAdvertisementData oAdvertisementData = BLEAdvertisementData();
    BLEAdvertisementData oScanResponseData = BLEAdvertisementData();
    
    oAdvertisementData.setFlags(0x04); // BR_EDR_NOT_SUPPORTED 0x04
    
    std::string strServiceData = "";
    strServiceData += (char)26;     // Len
    strServiceData += (char)0xFF;   // Type
    strServiceData += oBeacon.getData(); 
    oAdvertisementData.addData(strServiceData);
    
    pAdvertising = BLEDevice::getAdvertising();
    pAdvertising->setAdvertisementData(oAdvertisementData);
    pAdvertising->setScanResponseData(oScanResponseData);
    
    // Set advertising parameters for iBeacon
    pAdvertising->setAdvertisementType(ADV_TYPE_NONCONN_IND);
    
    Serial.println("iBeacon configured");
    Serial.println("UUID: " + uuid);
    Serial.println("Major: " + String(major));
    Serial.println("Minor: " + String(minor));
  }
  
  void startAdvertising() {
    pAdvertising->start();
    Serial.println("iBeacon advertising started");
  }
  
  void stopAdvertising() {
    pAdvertising->stop();
    Serial.println("iBeacon advertising stopped");
  }
  
  void updateValues(uint16_t newMajor, uint16_t newMinor) {
    major = newMajor;
    minor = newMinor;
    
    stopAdvertising();
    begin(); // Reconfigure with new values
    startAdvertising();
  }
  
  void enterPowerSaveMode(uint32_t sleepSeconds) {
    stopAdvertising();
    Serial.println("Entering deep sleep for " + String(sleepSeconds) + " seconds");
    Serial.flush();
    
    esp_sleep_enable_timer_wakeup(sleepSeconds * 1000000);
    esp_deep_sleep_start();
  }
};

iBeaconTransmitter beacon("550e8400-e29b-41d4-a716-446655440000", 1, 1);

void setup() {
  Serial.begin(115200);
  
  beacon.begin();
  beacon.startAdvertising();
  
  // For power saving, you could implement periodic sleep
  // beacon.enterPowerSaveMode(300); // Sleep for 5 minutes
}

void loop() {
  // Update beacon values based on sensors or time
  static unsigned long lastUpdate = 0;
  if (millis() - lastUpdate > 60000) { // Every minute
    uint16_t newMinor = random(1, 100); // Random minor ID
    beacon.updateValues(1, newMinor);
    lastUpdate = millis();
  }
  
  delay(1000);
}
```

### BLE Scanner for Beacons
```cpp
#include "BLEDevice.h"
#include "BLEScan.h"
#include "BLEAdvertisedDevice.h"

class BeaconScanner {
private:
  BLEScan* pBLEScan;
  
  struct BeaconInfo {
    String address;
    String uuid;
    uint16_t major;
    uint16_t minor;
    int rssi;
    unsigned long lastSeen;
  };
  
  BeaconInfo beacons[20];
  int beaconCount = 0;
  
public:
  void begin() {
    BLEDevice::init("");
    
    pBLEScan = BLEDevice::getScan();
    pBLEScan->setAdvertisedDeviceCallbacks(new AdvertisedDeviceCallbacks(this));
    pBLEScan->setActiveScan(true);
    pBLEScan->setInterval(100);
    pBLEScan->setWindow(99);
  }
  
  void startScanning() {
    Serial.println("Starting BLE scan for beacons...");
    pBLEScan->start(0, false); // Scan continuously
  }
  
  void stopScanning() {
    pBLEScan->stop();
    Serial.println("BLE scan stopped");
  }
  
  void printBeacons() {
    Serial.println("\n=== Discovered Beacons ===");
    Serial.println("Address\t\t\tUUID\t\t\t\t\tMajor\tMinor\tRSSI\tDistance");
    
    for (int i = 0; i < beaconCount; i++) {
      if (millis() - beacons[i].lastSeen < 30000) { // Show beacons seen in last 30 seconds
        Serial.print(beacons[i].address);
        Serial.print("\t");
        Serial.print(beacons[i].uuid);
        Serial.print("\t");
        Serial.print(beacons[i].major);
        Serial.print("\t");
        Serial.print(beacons[i].minor);
        Serial.print("\t");
        Serial.print(beacons[i].rssi);
        Serial.print("\t");
        Serial.print(calculateDistance(beacons[i].rssi), 1);
        Serial.println("m");
      }
    }
    Serial.println();
  }
  
private:
  void addOrUpdateBeacon(String address, String uuid, uint16_t major, uint16_t minor, int rssi) {
    // Look for existing beacon
    for (int i = 0; i < beaconCount; i++) {
      if (beacons[i].address == address) {
        // Update existing beacon
        beacons[i].rssi = rssi;
        beacons[i].lastSeen = millis();
        return;
      }
    }
    
    // Add new beacon
    if (beaconCount < 20) {
      beacons[beaconCount] = {address, uuid, major, minor, rssi, millis()};
      beaconCount++;
    }
  }
  
  float calculateDistance(int rssi) {
    // Simplified distance calculation (accuracy varies greatly)
    if (rssi == 0) return -1.0;
    
    double ratio = rssi * 1.0 / -59; // -59 is measured power at 1 meter
    if (ratio < 1.0) {
      return pow(ratio, 10);
    } else {
      double accuracy = (0.89976) * pow(ratio, 7.7095) + 0.111;
      return accuracy;
    }
  }
  
  class AdvertisedDeviceCallbacks: public BLEAdvertisedDeviceCallbacks {
  private:
    BeaconScanner* scanner;
    
  public:
    AdvertisedDeviceCallbacks(BeaconScanner* s) : scanner(s) {}
    
    void onResult(BLEAdvertisedDevice advertisedDevice) {
      if (advertisedDevice.haveManufacturerData()) {
        std::string manufacturerData = advertisedDevice.getManufacturerData();
        
        if (manufacturerData.length() >= 25 && 
            manufacturerData[0] == 0x4C && manufacturerData[1] == 0x00) { // Apple
          
          if (manufacturerData[2] == 0x02 && manufacturerData[3] == 0x15) { // iBeacon
            // Parse iBeacon data
            String uuid = "";
            for (int i = 4; i < 20; i++) {
              if (manufacturerData[i] < 16) uuid += "0";
              uuid += String(manufacturerData[i] & 0xFF, HEX);
              if (i == 7 || i == 9 || i == 11 || i == 13) uuid += "-";
            }
            
            uint16_t major = (manufacturerData[20] << 8) | manufacturerData[21];
            uint16_t minor = (manufacturerData[22] << 8) | manufacturerData[23];
            
            scanner->addOrUpdateBeacon(
              advertisedDevice.getAddress().toString().c_str(),
              uuid,
              major,
              minor,
              advertisedDevice.getRSSI()
            );
          }
        }
      }
    }
  };
};

BeaconScanner scanner;

void setup() {
  Serial.begin(115200);
  
  scanner.begin();
  scanner.startScanning();
}

void loop() {
  static unsigned long lastPrint = 0;
  if (millis() - lastPrint > 5000) { // Print every 5 seconds
    scanner.printBeacons();
    lastPrint = millis();
  }
  
  delay(100);
}
```

## Power Optimization Techniques

### Ultra-Low Power BLE Implementation
```cpp
#include "esp_sleep.h"
#include "driver/gpio.h"

class LowPowerBLE {
private:
  unsigned long lastAdvertisement = 0;
  unsigned long advertisementInterval = 1000; // 1 second
  unsigned long sleepDuration = 30000; // 30 seconds
  bool deepSleepEnabled = true;
  
public:
  void begin() {
    // Configure wake-up source
    esp_sleep_enable_timer_wakeup(sleepDuration * 1000);
    
    // Configure GPIO wake-up (optional)
    esp_sleep_enable_ext0_wakeup(GPIO_NUM_0, 0); // Boot button
    
    // Reduce CPU frequency
    setCpuFrequencyMhz(80); // Reduce from 240MHz to 80MHz
    
    // Initialize BLE with power-optimized settings
    BLEDevice::init("LowPowerDevice");
    setupLowPowerAdvertising();
  }
  
  void setupLowPowerAdvertising() {
    BLEAdvertising *pAdvertising = BLEDevice::getAdvertising();
    
    // Set longer advertising intervals (power saving)
    pAdvertising->setMinInterval(1600); // 1000ms
    pAdvertising->setMaxInterval(1600); // 1000ms
    
    // Reduce TX power
    esp_ble_tx_power_set(ESP_BLE_PWR_TYPE_ADV, ESP_PWR_LVL_N12); // -12dBm
    
    // Minimal advertising data
    BLEAdvertisementData advertisementData;
    advertisementData.setName("LowPwr");
    advertisementData.setFlags(0x06); // BR_EDR_NOT_SUPPORTED | GENERAL_DISC_MODE
    
    pAdvertising->setAdvertisementData(advertisementData);
    pAdvertising->setScanResponse(false); // Disable scan response
  }
  
  void periodicAdvertise() {
    if (millis() - lastAdvertisement > advertisementInterval) {
      // Wake up BLE
      BLEDevice::startAdvertising();
      Serial.println("Advertising started");
      
      // Advertise for 5 seconds
      delay(5000);
      
      // Stop advertising
      BLEDevice::getAdvertising()->stop();
      Serial.println("Advertising stopped");
      
      lastAdvertisement = millis();
      
      if (deepSleepEnabled) {
        enterDeepSleep();
      }
    }
  }
  
  void enterDeepSleep() {
    Serial.println("Entering deep sleep...");
    Serial.flush();
    
    // Turn off BLE
    BLEDevice::deinit(false);
    
    // Enter deep sleep
    esp_deep_sleep_start();
  }
  
  void enablePeriodicWakeup(bool enable) {
    deepSleepEnabled = enable;
  }
  
  void setAdvertisingInterval(unsigned long interval) {
    advertisementInterval = interval;
  }
};

LowPowerBLE lowPowerBLE;

void setup() {
  Serial.begin(115200);
  
  // Print wake-up reason
  esp_sleep_wakeup_cause_t wakeup_reason = esp_sleep_get_wakeup_cause();
  switch(wakeup_reason) {
    case ESP_SLEEP_WAKEUP_EXT0: 
      Serial.println("Wakeup caused by external signal using RTC_IO"); 
      break;
    case ESP_SLEEP_WAKEUP_TIMER: 
      Serial.println("Wakeup caused by timer"); 
      break;
    default: 
      Serial.println("Wakeup was not caused by deep sleep"); 
      break;
  }
  
  lowPowerBLE.begin();
}

void loop() {
  lowPowerBLE.periodicAdvertise();
  delay(100);
}
```

## BLE Security Implementation

### Pairing and Bonding
```cpp
#include "BLEDevice.h"
#include "BLEServer.h"
#include "BLESecurity.h"

class SecureBLEServer {
private:
  BLEServer* pServer;
  BLEService* pService;
  BLECharacteristic* pCharacteristic;
  bool deviceConnected = false;
  
  // Security callback class
  class SecurityCallbacks : public BLESecurityCallbacks {
    uint32_t onPassKeyRequest() {
      Serial.println("PassKeyRequest");
      return 123456; // Static passkey (use random in production)
    }
    
    void onPassKeyNotify(uint32_t pass_key) {
      Serial.printf("PassKeyNotify: %d\n", pass_key);
    }
    
    bool onConfirmPIN(uint32_t pass_key) {
      Serial.printf("Confirm PIN: %d\n", pass_key);
      return true; // Auto-confirm (implement user confirmation in production)
    }
    
    bool onSecurityRequest() {
      Serial.println("Security Request");
      return true;
    }
    
    void onAuthenticationComplete(esp_ble_auth_cmpl_t auth_cmpl) {
      if (auth_cmpl.success) {
        Serial.println("Authentication Success");
      } else {
        Serial.printf("Authentication Failed: %d\n", auth_cmpl.fail_reason);
      }
    }
  };
  
public:
  void begin() {
    BLEDevice::init("SecureBLE");
    BLEDevice::setEncryptionLevel(ESP_BLE_SEC_ENCRYPT);
    BLEDevice::setSecurityCallbacks(new SecurityCallbacks());
    
    // Configure security
    BLESecurity *pSecurity = new BLESecurity();
    pSecurity->setAuthenticationMode(ESP_LE_AUTH_REQ_SC_MITM_BOND);
    pSecurity->setCapability(ESP_IO_CAP_OUT);
    pSecurity->setRespEncryptionKey(ESP_BLE_ENC_KEY_MASK | ESP_BLE_ID_KEY_MASK);
    
    setupServer();
    startAdvertising();
  }
  
private:
  void setupServer() {
    pServer = BLEDevice::createServer();
    pServer->setCallbacks(new ServerCallbacks(this));
    
    pService = pServer->createService("12345678-1234-1234-1234-123456789abc");
    
    pCharacteristic = pService->createCharacteristic(
      "87654321-4321-4321-4321-cba987654321",
      BLECharacteristic::PROPERTY_READ |
      BLECharacteristic::PROPERTY_WRITE |
      BLECharacteristic::PROPERTY_NOTIFY
    );
    
    // Require encryption for this characteristic
    pCharacteristic->setAccessPermissions(ESP_GATT_PERM_READ_ENCRYPTED | ESP_GATT_PERM_WRITE_ENCRYPTED);
    
    pCharacteristic->setCallbacks(new CharacteristicCallbacks());
    pCharacteristic->addDescriptor(new BLE2902());
    
    pService->start();
  }
  
  void startAdvertising() {
    BLEAdvertising *pAdvertising = BLEDevice::getAdvertising();
    pAdvertising->addServiceUUID("12345678-1234-1234-1234-123456789abc");
    pAdvertising->setScanResponse(true);
    pAdvertising->setMinPreferred(0x06);
    pAdvertising->setMinPreferred(0x12);
    BLEDevice::startAdvertising();
    
    Serial.println("Secure BLE server started, waiting for connections...");
  }
  
  class ServerCallbacks: public BLEServerCallbacks {
  private:
    SecureBLEServer* server;
    
  public:
    ServerCallbacks(SecureBLEServer* s) : server(s) {}
    
    void onConnect(BLEServer* pServer) {
      server->deviceConnected = true;
      Serial.println("Device connected - starting security negotiation");
    }
    
    void onDisconnect(BLEServer* pServer) {
      server->deviceConnected = false;
      Serial.println("Device disconnected");
      pServer->startAdvertising(); // Restart advertising
    }
  };
  
  class CharacteristicCallbacks: public BLECharacteristicCallbacks {
    void onWrite(BLECharacteristic *pCharacteristic) {
      std::string value = pCharacteristic->getValue();
      Serial.printf("Received secure data: %s\n", value.c_str());
      
      // Echo back with timestamp
      String response = "Secure Echo: " + String(value.c_str()) + " at " + String(millis());
      pCharacteristic->setValue(response.c_str());
      pCharacteristic->notify();
    }
  };
};
```

## Best Practices and Guidelines

### 1. Connection Management
```cpp
class BLEConnectionManager {
private:
  static const int MAX_CONNECTIONS = 4;
  static const unsigned long CONNECTION_TIMEOUT = 30000; // 30 seconds
  
  struct Connection {
    bool active;
    unsigned long lastActivity;
    String clientAddress;
  };
  
  Connection connections[MAX_CONNECTIONS];
  
public:
  void manageConnections() {
    unsigned long now = millis();
    
    for (int i = 0; i < MAX_CONNECTIONS; i++) {
      if (connections[i].active && 
          (now - connections[i].lastActivity > CONNECTION_TIMEOUT)) {
        // Disconnect inactive clients
        disconnectClient(i);
      }
    }
  }
  
  bool addConnection(String address) {
    for (int i = 0; i < MAX_CONNECTIONS; i++) {
      if (!connections[i].active) {
        connections[i].active = true;
        connections[i].lastActivity = millis();
        connections[i].clientAddress = address;
        return true;
      }
    }
    return false; // No slots available
  }
  
private:
  void disconnectClient(int index) {
    connections[index].active = false;
    Serial.println("Disconnected inactive client: " + connections[index].clientAddress);
  }
};
```

### 2. Data Rate Optimization
```cpp
void optimizeDataTransmission() {
  // Use connection parameter update for better throughput
  esp_ble_conn_update_params_t conn_params;
  conn_params.min_int = 6;    // 7.5ms
  conn_params.max_int = 12;   // 15ms
  conn_params.latency = 0;    // No slave latency
  conn_params.timeout = 400;  // 4s supervision timeout
  
  // This would be called after connection establishment
  // esp_ble_gap_update_conn_params(&conn_params);
}
```

### 3. Error Handling and Recovery
```cpp
class BLEErrorHandler {
public:
  static void handleBLEError(esp_err_t error, const char* context) {
    switch (error) {
      case ESP_OK:
        break;
      case ESP_ERR_NO_MEM:
        Serial.printf("BLE Error in %s: Out of memory\n", context);
        break;
      case ESP_ERR_INVALID_ARG:
        Serial.printf("BLE Error in %s: Invalid argument\n", context);
        break;
      case ESP_ERR_INVALID_STATE:
        Serial.printf("BLE Error in %s: Invalid state\n", context);
        restartBLE();
        break;
      default:
        Serial.printf("BLE Error in %s: Code %d\n", context, error);
        break;
    }
  }
  
private:
  static void restartBLE() {
    Serial.println("Restarting BLE stack...");
    BLEDevice::deinit(true);
    delay(1000);
    BLEDevice::init("Restarted-Device");
    // Reinitialize services and advertising
  }
};
```

This comprehensive BLE guide covers the fundamental concepts, practical implementations, and advanced features needed to develop robust Bluetooth Low Energy applications with Arduino/ESP32 platforms.
