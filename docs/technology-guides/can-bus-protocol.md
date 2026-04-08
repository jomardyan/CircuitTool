# CAN Bus (Controller Area Network) Guide

## Introduction to CAN Bus

CAN (Controller Area Network) is a robust vehicle bus standard designed for automotive applications but widely used in industrial automation, medical equipment, and embedded systems. Developed by Bosch in the 1980s, CAN provides reliable, real-time communication in environments with high electromagnetic interference. It's known for its excellent error detection capabilities, priority-based message transmission, and fault tolerance.

## CAN Protocol Overview

### Key Features
- **Multi-master**: Any node can initiate communication
- **Message-based**: Data is transmitted in frames, not addresses
- **Priority-based**: High-priority messages get bus access first
- **Error detection**: Multiple error detection mechanisms
- **Fault tolerant**: Can operate with one wire broken (CAN-FD)
- **Deterministic**: Predictable message timing
- **Real-time**: Low latency communication

### CAN Frame Types
1. **Data Frame**: Carries actual data (0-8 bytes)
2. **Remote Frame**: Requests data from another node
3. **Error Frame**: Indicates transmission errors
4. **Overload Frame**: Introduces delays between frames

### CAN Versions
- **CAN 2.0A**: Standard 11-bit identifier (2048 IDs)
- **CAN 2.0B**: Extended 29-bit identifier (536M IDs)
- **CAN-FD**: Flexible Data Rate with up to 64 bytes payload

## CAN Physical Layer

### Differential Signaling
CAN uses differential signaling on two wires:
- **CAN_H (CAN High)**: Positive signal line
- **CAN_L (CAN Low)**: Negative signal line

### Bus States
- **Dominant (0)**: CAN_H = 3.5V, CAN_L = 1.5V (Difference = 2V)
- **Recessive (1)**: CAN_H = 2.5V, CAN_L = 2.5V (Difference = 0V)

### Bit Rates
- **Low Speed CAN**: 125 kbps
- **High Speed CAN**: 250 kbps, 500 kbps, 1 Mbps
- **CAN-FD**: Up to 8 Mbps (data phase)

## Arduino CAN Implementation

### Basic CAN Setup (MCP2515)
```cpp
#include <SPI.h>
#include <mcp2515.h>

// MCP2515 CAN controller
struct can_frame canMsg;
MCP2515 mcp2515(10); // CS pin

void setup() {
  Serial.begin(115200);
  
  mcp2515.reset();
  mcp2515.setBitrate(CAN_500KBPS, MCP_8MHZ);
  mcp2515.setNormalMode();
  
  Serial.println("CAN Bus initialized - 500kbps");
}

void loop() {
  // Send CAN message
  canMsg.can_id = 0x123;
  canMsg.can_dlc = 8;
  canMsg.data[0] = 0x01;
  canMsg.data[1] = 0x02;
  canMsg.data[2] = 0x03;
  canMsg.data[3] = 0x04;
  canMsg.data[4] = 0x05;
  canMsg.data[5] = 0x06;
  canMsg.data[6] = 0x07;
  canMsg.data[7] = 0x08;
  
  mcp2515.sendMessage(&canMsg);
  Serial.println("Message Sent");
  
  // Receive CAN message
  if (mcp2515.readMessage(&canMsg) == MCP2515::ERROR_OK) {
    Serial.print("ID: 0x");
    Serial.print(canMsg.can_id, HEX);
    Serial.print(" DLC: ");
    Serial.print(canMsg.can_dlc);
    Serial.print(" Data: ");
    
    for (int i = 0; i < canMsg.can_dlc; i++) {
      Serial.print("0x");
      Serial.print(canMsg.data[i], HEX);
      Serial.print(" ");
    }
    Serial.println();
  }
  
  delay(1000);
}
```

### Advanced CAN Class Implementation
```cpp
#include <SPI.h>
#include <mcp2515.h>

class CANInterface {
private:
  MCP2515* mcp;
  
  // Statistics
  unsigned long messagesSent = 0;
  unsigned long messagesReceived = 0;
  unsigned long transmissionErrors = 0;
  unsigned long receptionErrors = 0;
  unsigned long busOffEvents = 0;
  
  // Filters and masks
  struct Filter {
    uint32_t id;
    uint32_t mask;
    bool enabled;
  };
  
  Filter filters[6]; // MCP2515 has 6 filters
  
  // Message queues
  static const int TX_QUEUE_SIZE = 16;
  static const int RX_QUEUE_SIZE = 32;
  
  struct QueuedMessage {
    can_frame frame;
    unsigned long timestamp;
    int priority;
  };
  
  QueuedMessage txQueue[TX_QUEUE_SIZE];
  QueuedMessage rxQueue[RX_QUEUE_SIZE];
  volatile int txHead = 0, txTail = 0;
  volatile int rxHead = 0, rxTail = 0;
  
public:
  CANInterface(int csPin) {
    mcp = new MCP2515(csPin);
    initializeFilters();
  }
  
  ~CANInterface() {
    delete mcp;
  }
  
  bool begin(CAN_SPEED speed = CAN_500KBPS, CAN_CLOCK clock = MCP_8MHZ) {
    mcp->reset();
    
    if (mcp->setBitrate(speed, clock) != MCP2515::ERROR_OK) {
      Serial.println("Failed to set bitrate");
      return false;
    }
    
    if (mcp->setNormalMode() != MCP2515::ERROR_OK) {
      Serial.println("Failed to set normal mode");
      return false;
    }
    
    setupFilters();
    clearStatistics();
    
    Serial.printf("CAN Bus initialized - Speed: %d kbps\n", 
                  getBitrateKbps(speed));
    return true;
  }
  
  // Send message with priority queuing
  bool sendMessage(uint32_t id, const uint8_t* data, uint8_t length, 
                   int priority = 0, bool extended = false) {
    if (length > 8) return false;
    
    QueuedMessage msg;
    msg.frame.can_id = id;
    if (extended) {
      msg.frame.can_id |= CAN_EFF_FLAG;
    }
    msg.frame.can_dlc = length;
    memcpy(msg.frame.data, data, length);
    msg.timestamp = millis();
    msg.priority = priority;
    
    return enqueueMessage(msg, true);
  }
  
  // Send remote frame
  bool sendRemoteFrame(uint32_t id, uint8_t length, bool extended = false) {
    can_frame frame;
    frame.can_id = id | CAN_RTR_FLAG;
    if (extended) {
      frame.can_id |= CAN_EFF_FLAG;
    }
    frame.can_dlc = length;
    
    return transmitFrame(frame);
  }
  
  // Receive message (non-blocking)
  bool receiveMessage(uint32_t& id, uint8_t* data, uint8_t& length, 
                      bool& extended, bool& remote, unsigned long& timestamp) {
    if (isRxQueueEmpty()) {
      processIncomingMessages(); // Check for new messages
    }
    
    if (!isRxQueueEmpty()) {
      QueuedMessage msg = dequeueMessage(false);
      
      id = msg.frame.can_id & CAN_EFF_MASK;
      extended = (msg.frame.can_id & CAN_EFF_FLAG) != 0;
      remote = (msg.frame.can_id & CAN_RTR_FLAG) != 0;
      length = msg.frame.can_dlc;
      timestamp = msg.timestamp;
      
      if (!remote) {
        memcpy(data, msg.frame.data, length);
      }
      
      messagesReceived++;
      return true;
    }
    
    return false;
  }
  
  // Process transmission queue
  void processTxQueue() {
    while (!isTxQueueEmpty()) {
      QueuedMessage msg = peekTxMessage();
      
      if (transmitFrame(msg.frame)) {
        dequeueTxMessage();
        messagesSent++;
      } else {
        break; // Transmission failed, try again later
      }
    }
  }
  
  // Filter management
  void setFilter(int filterNum, uint32_t id, uint32_t mask, bool extended = false) {
    if (filterNum >= 0 && filterNum < 6) {
      filters[filterNum].id = id;
      filters[filterNum].mask = mask;
      filters[filterNum].enabled = true;
      
      // Apply filter to MCP2515
      if (filterNum < 2) {
        // RXB0 filters
        mcp->setFilter((MCP2515::RXF)filterNum, extended, id);
        mcp->setFilterMask(MCP2515::MASK0, extended, mask);
      } else {
        // RXB1 filters
        mcp->setFilter((MCP2515::RXF)filterNum, extended, id);
        mcp->setFilterMask(MCP2515::MASK1, extended, mask);
      }
    }
  }
  
  void disableFilter(int filterNum) {
    if (filterNum >= 0 && filterNum < 6) {
      filters[filterNum].enabled = false;
      // Set filter to accept all messages
      setFilter(filterNum, 0, 0);
    }
  }
  
  // Error handling
  void checkErrors() {
    uint8_t errorFlags = mcp->getErrorFlags();
    
    if (errorFlags & MCP2515::EFLG_RX1OVR) {
      Serial.println("RX1 Buffer Overflow");
      receptionErrors++;
    }
    
    if (errorFlags & MCP2515::EFLG_RX0OVR) {
      Serial.println("RX0 Buffer Overflow");
      receptionErrors++;
    }
    
    if (errorFlags & MCP2515::EFLG_TXBO) {
      Serial.println("Bus Off Error");
      busOffEvents++;
      // Attempt recovery
      mcp->setNormalMode();
    }
    
    if (errorFlags & MCP2515::EFLG_TXEP) {
      Serial.println("TX Error Passive");
      transmissionErrors++;
    }
    
    if (errorFlags & MCP2515::EFLG_RXEP) {
      Serial.println("RX Error Passive");
      receptionErrors++;
    }
  }
  
  // Diagnostics
  void printStatistics() {
    Serial.println("=== CAN Bus Statistics ===");
    Serial.printf("Messages Sent: %lu\n", messagesSent);
    Serial.printf("Messages Received: %lu\n", messagesReceived);
    Serial.printf("TX Errors: %lu\n", transmissionErrors);
    Serial.printf("RX Errors: %lu\n", receptionErrors);
    Serial.printf("Bus Off Events: %lu\n", busOffEvents);
    Serial.printf("TX Queue: %d/%d\n", getTxQueueCount(), TX_QUEUE_SIZE);
    Serial.printf("RX Queue: %d/%d\n", getRxQueueCount(), RX_QUEUE_SIZE);
    
    // Error counters
    uint8_t txErrors = mcp->getErrorCountTX();
    uint8_t rxErrors = mcp->getErrorCountRX();
    Serial.printf("Error Counters - TX: %d, RX: %d\n", txErrors, rxErrors);
    Serial.println();
  }
  
  void clearStatistics() {
    messagesSent = 0;
    messagesReceived = 0;
    transmissionErrors = 0;
    receptionErrors = 0;
    busOffEvents = 0;
  }
  
  // Bus monitoring
  bool setBusMonitorMode() {
    return mcp->setListenOnlyMode() == MCP2515::ERROR_OK;
  }
  
  bool setNormalMode() {
    return mcp->setNormalMode() == MCP2515::ERROR_OK;
  }
  
  bool setLoopbackMode() {
    return mcp->setLoopbackMode() == MCP2515::ERROR_OK;
  }
  
private:
  void initializeFilters() {
    for (int i = 0; i < 6; i++) {
      filters[i].enabled = false;
      filters[i].id = 0;
      filters[i].mask = 0;
    }
  }
  
  void setupFilters() {
    // Default: accept all messages
    for (int i = 0; i < 6; i++) {
      disableFilter(i);
    }
  }
  
  bool transmitFrame(const can_frame& frame) {
    MCP2515::ERROR result = mcp->sendMessage(&frame);
    
    if (result == MCP2515::ERROR_OK) {
      return true;
    } else {
      transmissionErrors++;
      return false;
    }
  }
  
  void processIncomingMessages() {
    can_frame frame;
    
    while (mcp->readMessage(&frame) == MCP2515::ERROR_OK) {
      QueuedMessage msg;
      msg.frame = frame;
      msg.timestamp = millis();
      msg.priority = 0;
      
      enqueueMessage(msg, false);
    }
  }
  
  bool enqueueMessage(const QueuedMessage& msg, bool isTx) {
    if (isTx) {
      int next = (txHead + 1) % TX_QUEUE_SIZE;
      if (next == txTail) return false; // Queue full
      
      txQueue[txHead] = msg;
      txHead = next;
    } else {
      int next = (rxHead + 1) % RX_QUEUE_SIZE;
      if (next == rxTail) return false; // Queue full
      
      rxQueue[rxHead] = msg;
      rxHead = next;
    }
    
    return true;
  }
  
  QueuedMessage dequeueMessage(bool isTx) {
    if (isTx) {
      QueuedMessage msg = txQueue[txTail];
      txTail = (txTail + 1) % TX_QUEUE_SIZE;
      return msg;
    } else {
      QueuedMessage msg = rxQueue[rxTail];
      rxTail = (rxTail + 1) % RX_QUEUE_SIZE;
      return msg;
    }
  }
  
  QueuedMessage peekTxMessage() {
    return txQueue[txTail];
  }
  
  void dequeueTxMessage() {
    txTail = (txTail + 1) % TX_QUEUE_SIZE;
  }
  
  bool isTxQueueEmpty() { return txHead == txTail; }
  bool isRxQueueEmpty() { return rxHead == rxTail; }
  
  int getTxQueueCount() {
    return (txHead - txTail + TX_QUEUE_SIZE) % TX_QUEUE_SIZE;
  }
  
  int getRxQueueCount() {
    return (rxHead - rxTail + RX_QUEUE_SIZE) % RX_QUEUE_SIZE;
  }
  
  int getBitrateKbps(CAN_SPEED speed) {
    switch (speed) {
      case CAN_5KBPS: return 5;
      case CAN_10KBPS: return 10;
      case CAN_20KBPS: return 20;
      case CAN_31K25BPS: return 31;
      case CAN_33KBPS: return 33;
      case CAN_40KBPS: return 40;
      case CAN_50KBPS: return 50;
      case CAN_80KBPS: return 80;
      case CAN_95KBPS: return 95;
      case CAN_100KBPS: return 100;
      case CAN_125KBPS: return 125;
      case CAN_200KBPS: return 200;
      case CAN_250KBPS: return 250;
      case CAN_500KBPS: return 500;
      case CAN_1000KBPS: return 1000;
      default: return 0;
    }
  }
};

// Usage example
CANInterface can(10); // CS pin 10

void setup() {
  Serial.begin(115200);
  
  if (!can.begin(CAN_500KBPS)) {
    Serial.println("CAN initialization failed!");
    while (1);
  }
  
  // Set up some filters
  can.setFilter(0, 0x100, 0x700); // Accept IDs 0x100-0x1FF
  can.setFilter(1, 0x200, 0x7F0); // Accept IDs 0x200-0x20F
  
  Serial.println("CAN Bus ready");
}

void loop() {
  // Send test message
  uint8_t data[] = {0x01, 0x02, 0x03, 0x04};
  can.sendMessage(0x123, data, 4);
  
  // Process queues
  can.processTxQueue();
  
  // Check for received messages
  uint32_t id;
  uint8_t rxData[8], length;
  bool extended, remote;
  unsigned long timestamp;
  
  while (can.receiveMessage(id, rxData, length, extended, remote, timestamp)) {
    Serial.printf("RX: ID=0x%X, DLC=%d, Time=%lu\n", id, length, timestamp);
    if (!remote) {
      Serial.print("Data: ");
      for (int i = 0; i < length; i++) {
        Serial.printf("0x%02X ", rxData[i]);
      }
      Serial.println();
    }
  }
  
  // Check for errors
  can.checkErrors();
  
  // Print statistics every 10 seconds
  static unsigned long lastStats = 0;
  if (millis() - lastStats > 10000) {
    can.printStatistics();
    lastStats = millis();
  }
  
  delay(100);
}
```

### CAN Protocol Stack Implementation
```cpp
// Higher-level CAN protocol implementation
class CANProtocol {
private:
  CANInterface* can;
  
  // Protocol definitions
  enum MessageTypes {
    MSG_HEARTBEAT = 0x100,
    MSG_SENSOR_DATA = 0x200,
    MSG_ACTUATOR_CMD = 0x300,
    MSG_DIAGNOSTIC = 0x400,
    MSG_CONFIGURATION = 0x500,
    MSG_EMERGENCY = 0x600
  };
  
  struct SensorData {
    uint8_t sensorId;
    float value;
    uint8_t status;
  } __attribute__((packed));
  
  struct ActuatorCommand {
    uint8_t actuatorId;
    uint8_t command;
    uint16_t value;
  } __attribute__((packed));
  
  struct DiagnosticData {
    uint8_t nodeId;
    uint8_t errorCode;
    uint16_t errorData;
  } __attribute__((packed));
  
  // Node management
  uint8_t nodeId;
  bool nodeActive = false;
  unsigned long lastHeartbeat = 0;
  unsigned long heartbeatInterval = 1000; // 1 second
  
  // Callback functions
  typedef void (*MessageCallback)(uint32_t id, const uint8_t* data, uint8_t length);
  MessageCallback callbacks[16];
  uint32_t callbackIds[16];
  int callbackCount = 0;
  
public:
  CANProtocol(CANInterface* canInterface, uint8_t id) 
    : can(canInterface), nodeId(id) {}
  
  void begin() {
    nodeActive = true;
    lastHeartbeat = millis();
    
    // Set up message filters
    can->setFilter(0, MSG_HEARTBEAT, 0x700);     // Heartbeat messages
    can->setFilter(1, MSG_SENSOR_DATA, 0x700);   // Sensor data
    can->setFilter(2, MSG_ACTUATOR_CMD, 0x700);  // Actuator commands
    can->setFilter(3, MSG_DIAGNOSTIC, 0x700);    // Diagnostics
    can->setFilter(4, MSG_EMERGENCY, 0x700);     // Emergency messages
    
    Serial.printf("CAN Protocol started - Node ID: %d\n", nodeId);
  }
  
  void loop() {
    // Send heartbeat
    if (millis() - lastHeartbeat > heartbeatInterval) {
      sendHeartbeat();
      lastHeartbeat = millis();
    }
    
    // Process incoming messages
    processMessages();
    
    // Process transmission queue
    can->processTxQueue();
  }
  
  // Message sending methods
  bool sendSensorData(uint8_t sensorId, float value, uint8_t status = 0) {
    SensorData data = {sensorId, value, status};
    uint32_t msgId = MSG_SENSOR_DATA | nodeId;
    
    return can->sendMessage(msgId, (uint8_t*)&data, sizeof(data), 1);
  }
  
  bool sendActuatorCommand(uint8_t targetNode, uint8_t actuatorId, 
                          uint8_t command, uint16_t value) {
    ActuatorCommand cmd = {actuatorId, command, value};
    uint32_t msgId = MSG_ACTUATOR_CMD | targetNode;
    
    return can->sendMessage(msgId, (uint8_t*)&cmd, sizeof(cmd), 2);
  }
  
  bool sendDiagnostic(uint8_t errorCode, uint16_t errorData = 0) {
    DiagnosticData diag = {nodeId, errorCode, errorData};
    uint32_t msgId = MSG_DIAGNOSTIC | nodeId;
    
    return can->sendMessage(msgId, (uint8_t*)&diag, sizeof(diag), 1);
  }
  
  bool sendEmergency(uint8_t emergencyCode, const uint8_t* data = nullptr, 
                    uint8_t dataLength = 0) {
    uint8_t payload[8] = {nodeId, emergencyCode};
    uint8_t totalLength = 2;
    
    if (data && dataLength > 0 && dataLength <= 6) {
      memcpy(&payload[2], data, dataLength);
      totalLength += dataLength;
    }
    
    uint32_t msgId = MSG_EMERGENCY | 0x7F; // Broadcast emergency
    return can->sendMessage(msgId, payload, totalLength, 3); // High priority
  }
  
  // Configuration methods
  void setHeartbeatInterval(unsigned long interval) {
    heartbeatInterval = interval;
  }
  
  void registerCallback(uint32_t messageId, MessageCallback callback) {
    if (callbackCount < 16) {
      callbackIds[callbackCount] = messageId;
      callbacks[callbackCount] = callback;
      callbackCount++;
    }
  }
  
  // Network management
  bool requestNodeInfo(uint8_t targetNode) {
    uint8_t request = 0x01; // Node info request
    uint32_t msgId = MSG_DIAGNOSTIC | targetNode;
    
    return can->sendMessage(msgId, &request, 1);
  }
  
  bool pingNode(uint8_t targetNode) {
    uint8_t ping = 0xFF; // Ping command
    uint32_t msgId = MSG_HEARTBEAT | targetNode;
    
    return can->sendMessage(msgId, &ping, 1);
  }
  
private:
  void sendHeartbeat() {
    uint8_t status = nodeActive ? 0x01 : 0x00;
    uint32_t msgId = MSG_HEARTBEAT | nodeId;
    
    can->sendMessage(msgId, &status, 1, 0); // Low priority
  }
  
  void processMessages() {
    uint32_t id;
    uint8_t data[8], length;
    bool extended, remote;
    unsigned long timestamp;
    
    while (can->receiveMessage(id, data, length, extended, remote, timestamp)) {
      if (remote) continue; // Skip remote frames
      
      // Extract message type and source node
      uint32_t msgType = id & 0x700;
      uint8_t sourceNode = id & 0xFF;
      
      // Handle different message types
      switch (msgType) {
        case MSG_HEARTBEAT:
          handleHeartbeat(sourceNode, data, length);
          break;
          
        case MSG_SENSOR_DATA:
          handleSensorData(sourceNode, data, length);
          break;
          
        case MSG_ACTUATOR_CMD:
          if ((id & 0xFF) == nodeId) { // Message for this node
            handleActuatorCommand(sourceNode, data, length);
          }
          break;
          
        case MSG_DIAGNOSTIC:
          handleDiagnostic(sourceNode, data, length);
          break;
          
        case MSG_EMERGENCY:
          handleEmergency(sourceNode, data, length);
          break;
      }
      
      // Call registered callbacks
      for (int i = 0; i < callbackCount; i++) {
        if ((id & 0x700) == (callbackIds[i] & 0x700)) {
          callbacks[i](id, data, length);
        }
      }
    }
  }
  
  void handleHeartbeat(uint8_t sourceNode, const uint8_t* data, uint8_t length) {
    if (length > 0) {
      Serial.printf("Heartbeat from Node %d, Status: 0x%02X\n", 
                    sourceNode, data[0]);
    }
  }
  
  void handleSensorData(uint8_t sourceNode, const uint8_t* data, uint8_t length) {
    if (length >= sizeof(SensorData)) {
      SensorData* sensorData = (SensorData*)data;
      Serial.printf("Sensor Data from Node %d: ID=%d, Value=%.2f, Status=0x%02X\n",
                    sourceNode, sensorData->sensorId, sensorData->value, 
                    sensorData->status);
    }
  }
  
  void handleActuatorCommand(uint8_t sourceNode, const uint8_t* data, uint8_t length) {
    if (length >= sizeof(ActuatorCommand)) {
      ActuatorCommand* cmd = (ActuatorCommand*)data;
      Serial.printf("Actuator Command from Node %d: ID=%d, Cmd=0x%02X, Value=%d\n",
                    sourceNode, cmd->actuatorId, cmd->command, cmd->value);
      
      // Execute actuator command here
      executeActuatorCommand(cmd->actuatorId, cmd->command, cmd->value);
    }
  }
  
  void handleDiagnostic(uint8_t sourceNode, const uint8_t* data, uint8_t length) {
    if (length >= sizeof(DiagnosticData)) {
      DiagnosticData* diag = (DiagnosticData*)data;
      Serial.printf("Diagnostic from Node %d: Error=0x%02X, Data=0x%04X\n",
                    sourceNode, diag->errorCode, diag->errorData);
    }
  }
  
  void handleEmergency(uint8_t sourceNode, const uint8_t* data, uint8_t length) {
    if (length >= 2) {
      uint8_t emergencyNode = data[0];
      uint8_t emergencyCode = data[1];
      
      Serial.printf("EMERGENCY from Node %d: Code=0x%02X\n", 
                    emergencyNode, emergencyCode);
      
      // Handle emergency (stop operations, safe state, etc.)
      handleEmergencyCondition(emergencyCode, &data[2], length - 2);
    }
  }
  
  void executeActuatorCommand(uint8_t actuatorId, uint8_t command, uint16_t value) {
    // Implement actuator control logic
    Serial.printf("Executing: Actuator %d, Command 0x%02X, Value %d\n",
                  actuatorId, command, value);
  }
  
  void handleEmergencyCondition(uint8_t code, const uint8_t* data, uint8_t length) {
    // Implement emergency handling
    Serial.printf("Emergency condition 0x%02X detected\n", code);
    
    switch (code) {
      case 0x01: // Emergency stop
        Serial.println("EMERGENCY STOP activated");
        break;
      case 0x02: // Overheat
        Serial.println("Overheating detected");
        break;
      case 0x03: // Power failure
        Serial.println("Power failure detected");
        break;
      default:
        Serial.println("Unknown emergency");
        break;
    }
  }
};

// Example usage
CANInterface can(10);
CANProtocol protocol(&can, 1); // Node ID 1

void sensorDataCallback(uint32_t id, const uint8_t* data, uint8_t length) {
  Serial.println("Custom sensor data handler called");
}

void setup() {
  Serial.begin(115200);
  
  if (!can.begin(CAN_500KBPS)) {
    Serial.println("CAN initialization failed!");
    while (1);
  }
  
  protocol.begin();
  protocol.setHeartbeatInterval(2000); // 2 seconds
  protocol.registerCallback(0x200, sensorDataCallback);
  
  Serial.println("CAN Protocol stack ready");
}

void loop() {
  protocol.loop();
  
  // Send sensor data periodically
  static unsigned long lastSensor = 0;
  if (millis() - lastSensor > 5000) {
    float temperature = 25.5 + random(-50, 50) / 10.0;
    protocol.sendSensorData(1, temperature, 0x00);
    lastSensor = millis();
  }
  
  // Send actuator commands occasionally
  static unsigned long lastActuator = 0;
  if (millis() - lastActuator > 15000) {
    protocol.sendActuatorCommand(2, 1, 0x01, 100);
    lastActuator = millis();
  }
  
  delay(10);
}
```

### CAN Gateway and Bridge
```cpp
#include <WiFi.h>
#include <WebServer.h>
#include <ArduinoJson.h>

class CANGateway {
private:
  CANInterface* can;
  WebServer server;
  bool wifiConnected = false;
  
  // Message logging
  struct LogEntry {
    unsigned long timestamp;
    uint32_t id;
    uint8_t data[8];
    uint8_t length;
    bool transmitted; // true = TX, false = RX
  };
  
  static const int LOG_SIZE = 100;
  LogEntry messageLog[LOG_SIZE];
  int logIndex = 0;
  
public:
  CANGateway(CANInterface* canInterface) : can(canInterface), server(80) {}
  
  void begin(const char* ssid, const char* password) {
    // Connect to WiFi
    WiFi.begin(ssid, password);
    
    int attempts = 0;
    while (WiFi.status() != WL_CONNECTED && attempts < 20) {
      delay(500);
      Serial.print(".");
      attempts++;
    }
    
    if (WiFi.status() == WL_CONNECTED) {
      wifiConnected = true;
      Serial.println();
      Serial.print("WiFi connected: ");
      Serial.println(WiFi.localIP());
      
      setupWebServer();
    } else {
      Serial.println("WiFi connection failed");
    }
  }
  
  void loop() {
    if (wifiConnected) {
      server.handleClient();
    }
    
    // Log CAN messages
    logCANMessages();
  }
  
private:
  void setupWebServer() {
    // Serve main page
    server.on("/", [this]() {
      String html = generateWebInterface();
      server.send(200, "text/html", html);
    });
    
    // API endpoints
    server.on("/api/send", HTTP_POST, [this]() {
      handleSendMessage();
    });
    
    server.on("/api/messages", HTTP_GET, [this]() {
      handleGetMessages();
    });
    
    server.on("/api/status", HTTP_GET, [this]() {
      handleGetStatus();
    });
    
    server.on("/api/clear", HTTP_POST, [this]() {
      clearLog();
      server.send(200, "application/json", "{\"status\":\"ok\"}");
    });
    
    server.begin();
    Serial.println("Web server started");
  }
  
  void handleSendMessage() {
    if (server.hasArg("plain")) {
      DynamicJsonDocument doc(1024);
      deserializeJson(doc, server.arg("plain"));
      
      uint32_t id = doc["id"];
      JsonArray dataArray = doc["data"];
      
      uint8_t data[8];
      uint8_t length = min((int)dataArray.size(), 8);
      
      for (int i = 0; i < length; i++) {
        data[i] = dataArray[i];
      }
      
      bool success = can->sendMessage(id, data, length);
      
      if (success) {
        logMessage(id, data, length, true);
        server.send(200, "application/json", "{\"status\":\"sent\"}");
      } else {
        server.send(500, "application/json", "{\"status\":\"error\"}");
      }
    } else {
      server.send(400, "application/json", "{\"status\":\"invalid request\"}");
    }
  }
  
  void handleGetMessages() {
    DynamicJsonDocument doc(4096);
    JsonArray messages = doc.createNestedArray("messages");
    
    for (int i = 0; i < LOG_SIZE; i++) {
      int index = (logIndex + i) % LOG_SIZE;
      const LogEntry& entry = messageLog[index];
      
      if (entry.timestamp > 0) { // Valid entry
        JsonObject msg = messages.createNestedObject();
        msg["timestamp"] = entry.timestamp;
        msg["id"] = entry.id;
        msg["length"] = entry.length;
        msg["direction"] = entry.transmitted ? "TX" : "RX";
        
        JsonArray dataArray = msg.createNestedArray("data");
        for (int j = 0; j < entry.length; j++) {
          dataArray.add(entry.data[j]);
        }
      }
    }
    
    String response;
    serializeJson(doc, response);
    server.send(200, "application/json", response);
  }
  
  void handleGetStatus() {
    DynamicJsonDocument doc(512);
    
    doc["uptime"] = millis();
    doc["free_heap"] = ESP.getFreeHeap();
    doc["wifi_rssi"] = WiFi.RSSI();
    
    // Add CAN statistics if available
    // This would require extending CANInterface to expose statistics
    
    String response;
    serializeJson(doc, response);
    server.send(200, "application/json", response);
  }
  
  void logCANMessages() {
    uint32_t id;
    uint8_t data[8], length;
    bool extended, remote;
    unsigned long timestamp;
    
    while (can->receiveMessage(id, data, length, extended, remote, timestamp)) {
      if (!remote) {
        logMessage(id, data, length, false);
      }
    }
  }
  
  void logMessage(uint32_t id, const uint8_t* data, uint8_t length, bool transmitted) {
    LogEntry& entry = messageLog[logIndex];
    entry.timestamp = millis();
    entry.id = id;
    entry.length = length;
    entry.transmitted = transmitted;
    memcpy(entry.data, data, length);
    
    logIndex = (logIndex + 1) % LOG_SIZE;
  }
  
  void clearLog() {
    for (int i = 0; i < LOG_SIZE; i++) {
      messageLog[i].timestamp = 0;
    }
    logIndex = 0;
  }
  
  String generateWebInterface() {
    return R"(
<!DOCTYPE html>
<html>
<head>
    <title>CAN Bus Gateway</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 20px; }
        .container { max-width: 1200px; margin: 0 auto; }
        .section { margin: 20px 0; padding: 15px; border: 1px solid #ccc; }
        .message { margin: 5px 0; padding: 5px; background: #f5f5f5; }
        .tx { background: #e7f3ff; }
        .rx { background: #f0fff0; }
        input, button { margin: 5px; padding: 5px; }
        #messages { height: 400px; overflow-y: scroll; }
    </style>
</head>
<body>
    <div class="container">
        <h1>CAN Bus Gateway</h1>
        
        <div class="section">
            <h3>Send CAN Message</h3>
            <input type="text" id="canId" placeholder="CAN ID (hex)" value="0x123">
            <input type="text" id="canData" placeholder="Data bytes (hex, space separated)" value="01 02 03 04">
            <button onclick="sendMessage()">Send</button>
        </div>
        
        <div class="section">
            <h3>CAN Messages</h3>
            <button onclick="refreshMessages()">Refresh</button>
            <button onclick="clearMessages()">Clear</button>
            <div id="messages"></div>
        </div>
        
        <div class="section">
            <h3>System Status</h3>
            <div id="status"></div>
            <button onclick="refreshStatus()">Refresh Status</button>
        </div>
    </div>

    <script>
        function sendMessage() {
            const id = parseInt(document.getElementById('canId').value, 16);
            const dataStr = document.getElementById('canData').value;
            const data = dataStr.split(' ').map(b => parseInt(b, 16));
            
            fetch('/api/send', {
                method: 'POST',
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify({id: id, data: data})
            })
            .then(response => response.json())
            .then(data => {
                console.log('Message sent:', data);
                refreshMessages();
            });
        }
        
        function refreshMessages() {
            fetch('/api/messages')
            .then(response => response.json())
            .then(data => {
                const container = document.getElementById('messages');
                container.innerHTML = '';
                
                data.messages.forEach(msg => {
                    const div = document.createElement('div');
                    div.className = 'message ' + (msg.direction === 'TX' ? 'tx' : 'rx');
                    
                    const dataStr = msg.data.map(b => b.toString(16).padStart(2, '0')).join(' ');
                    div.innerHTML = `[${msg.timestamp}] ${msg.direction} ID:0x${msg.id.toString(16)} DLC:${msg.length} Data:${dataStr}`;
                    
                    container.appendChild(div);
                });
                
                container.scrollTop = container.scrollHeight;
            });
        }
        
        function clearMessages() {
            fetch('/api/clear', {method: 'POST'})
            .then(() => refreshMessages());
        }
        
        function refreshStatus() {
            fetch('/api/status')
            .then(response => response.json())
            .then(data => {
                document.getElementById('status').innerHTML = 
                    `Uptime: ${data.uptime}ms<br>
                     Free Heap: ${data.free_heap} bytes<br>
                     WiFi RSSI: ${data.wifi_rssi} dBm`;
            });
        }
        
        // Auto-refresh messages every 2 seconds
        setInterval(refreshMessages, 2000);
        
        // Initial load
        refreshMessages();
        refreshStatus();
    </script>
</body>
</html>
    )";
  }
};

// Example usage
CANInterface can(10);
CANGateway gateway(&can);

void setup() {
  Serial.begin(115200);
  
  if (!can.begin(CAN_500KBPS)) {
    Serial.println("CAN initialization failed!");
    while (1);
  }
  
  gateway.begin("YourSSID", "YourPassword");
  
  Serial.println("CAN Gateway ready");
}

void loop() {
  gateway.loop();
  can.processTxQueue();
  delay(10);
}
```

## CAN Bus Best Practices

### 1. Network Topology
- **Bus topology**: Single bus with termination resistors (120Ω) at both ends
- **Maximum bus length**: 40m at 1Mbps, 1000m at 50kbps
- **Maximum nodes**: 64 for CAN 2.0A, 110 for CAN 2.0B
- **Stub length**: Keep as short as possible (<0.3m)

### 2. Message Design
```cpp
// Good message design principles
struct StandardMessage {
  uint32_t id;        // Use meaningful IDs
  uint8_t priority;   // Lower number = higher priority
  uint8_t source;     // Source node ID
  uint8_t target;     // Target node ID (0 = broadcast)
  uint8_t function;   // Message function
  uint8_t data[4];    // Actual payload
} __attribute__((packed));

// Message ID allocation strategy
#define EMERGENCY_BASE    0x000  // 0x000-0x0FF (highest priority)
#define CONTROL_BASE      0x100  // 0x100-0x1FF
#define SENSOR_BASE       0x200  // 0x200-0x2FF
#define STATUS_BASE       0x300  // 0x300-0x3FF
#define HEARTBEAT_BASE    0x700  // 0x700-0x7FF (lowest priority)
```

### 3. Error Handling
```cpp
void robustCANOperation() {
  // Implement proper error handling
  
  // 1. Check bus-off condition
  if (can.isBusOff()) {
    Serial.println("Bus-off detected, attempting recovery");
    can.reset();
    delay(100);
    can.setNormalMode();
  }
  
  // 2. Monitor error counters
  uint8_t txErrors = can.getTxErrorCount();
  uint8_t rxErrors = can.getRxErrorCount();
  
  if (txErrors > 96 || rxErrors > 96) {
    Serial.println("High error count detected");
    // Implement recovery strategy
  }
  
  // 3. Implement message timeouts
  const unsigned long MSG_TIMEOUT = 5000;
  static unsigned long lastHeartbeat = 0;
  
  if (millis() - lastHeartbeat > MSG_TIMEOUT) {
    Serial.println("Communication timeout");
    // Handle timeout condition
  }
}
```

### 4. Real-time Considerations
```cpp
class RealTimeCANScheduler {
private:
  struct ScheduledMessage {
    uint32_t id;
    uint8_t data[8];
    uint8_t length;
    unsigned long period;
    unsigned long lastSent;
    int priority;
  };
  
  ScheduledMessage schedule[16];
  int messageCount = 0;
  
public:
  void addPeriodicMessage(uint32_t id, const uint8_t* data, uint8_t length,
                         unsigned long period, int priority = 0) {
    if (messageCount < 16) {
      schedule[messageCount] = {id, {0}, length, period, 0, priority};
      memcpy(schedule[messageCount].data, data, length);
      messageCount++;
    }
  }
  
  void process() {
    unsigned long now = millis();
    
    // Sort by priority when due
    for (int i = 0; i < messageCount; i++) {
      if (now - schedule[i].lastSent >= schedule[i].period) {
        // Message is due, send it
        can.sendMessage(schedule[i].id, schedule[i].data, 
                       schedule[i].length, schedule[i].priority);
        schedule[i].lastSent = now;
      }
    }
  }
};
```

## Common Issues and Troubleshooting

### Problem: No Communication
**Check:**
- Termination resistors (120Ω at each end)
- Baud rate configuration
- CAN_H and CAN_L wiring
- Ground connections
- Node addresses/filters

### Problem: High Error Rates
**Solutions:**
- Check cable quality and length
- Verify termination resistors
- Reduce baud rate
- Check for electromagnetic interference
- Verify power supply stability

### Problem: Bus-Off Condition
**Causes:**
- Faulty node transmitting errors
- Incorrect baud rate
- Hardware malfunction
- Poor cable connections

**Recovery:**
- Reset the CAN controller
- Check error counters
- Isolate problematic nodes
- Verify network integrity

This comprehensive CAN Bus guide covers everything from basic implementation to advanced protocol stacks, error handling, and real-time communication strategies.
