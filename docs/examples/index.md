# CircuitTool Examples

This section contains practical examples and real-world use cases for CircuitTool.

## Quick Examples

### LED Strip Current Calculation

Calculate the total current required for an LED strip:

```csharp
using CircuitTool;

// LED strip specifications
int numberOfLEDs = 60;           // 60 LEDs per meter
double ledCurrent = 0.02;        // 20mA per LED
double supplyVoltage = 12.0;     // 12V power supply
double ledVoltage = 3.2;         // Forward voltage per LED

// Calculate total current
double totalCurrent = numberOfLEDs * ledCurrent;
Console.WriteLine($"Total LED current: {totalCurrent}A");

// Calculate power consumption
double totalPower = supplyVoltage * totalCurrent;
Console.WriteLine($"Total power consumption: {totalPower}W");

// Calculate current limiting resistor if needed
if (supplyVoltage > ledVoltage)
{
    var resistorResult = LEDCalculator.CalculateCurrentLimitingResistor(
        supplyVoltage, ledVoltage, ledCurrent);
    Console.WriteLine($"Current limiting resistor: {resistorResult.resistance}Ω");
}
```

### Arduino Project: Temperature Sensor

Design a temperature monitoring system with Arduino:

```csharp
using CircuitTool;

// Generate Arduino code for temperature sensor
string tempSensorCode = ArduinoTools.GenerateAnalogSensorCode(
    sensorPin: 0,
    sensorName: "LM35",
    conversionFormula: "voltage * 100", // LM35: 10mV/°C
    units: "°C"
);

// Calculate pull-up resistor for I2C temperature sensor
var i2cConfig = new CommunicationProtocolTools.I2CConfig
{
    ClockFrequency = 100000,  // 100kHz
    SupplyVoltage = 3.3,      // 3.3V system
    NumberOfDevices = 2       // Temperature sensor + RTC
};

var (minR, maxR, recommendedR) = CommunicationProtocolTools.CalculateI2CPullUpResistors(i2cConfig);
Console.WriteLine($"I2C pull-up resistor: {recommendedR}Ω");

// Generate I2C scanner code
string scannerCode = CommunicationProtocolTools.GenerateI2CScannerCode();
```

### Power Supply Design

Design a linear power supply:

```csharp
using CircuitTool;

// Input specifications
double inputVoltage = 15.0;      // 15V input
double outputVoltage = 5.0;      // 5V output
double outputCurrent = 2.0;      // 2A output current

// Calculate linear regulator requirements
var (powerDissipation, efficiency) = PowerSupplyCalculator.CalculateLinearRegulator(
    inputVoltage, outputVoltage, outputCurrent);

Console.WriteLine($"Power dissipation: {powerDissipation}W");
Console.WriteLine($"Efficiency: {efficiency * 100:F1}%");

// Calculate heat sink requirements
double thermalResistance = ThermalCalculator.CalculateHeatSinkRequirements(
    powerDissipation: powerDissipation,
    ambientTemp: 25.0,
    maxJunctionTemp: 85.0,
    junctionToCase: 2.0  // Thermal resistance junction-to-case
);

Console.WriteLine($"Required heat sink thermal resistance: {thermalResistance:F2} °C/W");

// Calculate filter capacitor
double filterCap = PowerSupplyCalculator.CalculateBuckCapacitor(
    outputCurrent: outputCurrent,
    switchingFrequency: 100000,  // 100kHz (for ripple estimation)
    voltageRipple: 0.05          // 50mV ripple
);

Console.WriteLine($"Output filter capacitor: {filterCap * 1e6:F0}µF");
```

### Motor Control System

Design a DC motor control circuit:

```csharp
using CircuitTool;

// Motor specifications
double motorVoltage = 12.0;      // 12V motor
double motorCurrent = 5.0;       // 5A stall current
double pwmFrequency = 20000;     // 20kHz PWM

// Calculate H-bridge requirements
var hBridgeReq = MotorControlCalculator.CalculateHBridgeRequirements(
    motorVoltage, motorCurrent, pwmFrequency, safetyFactor: 1.5);

Console.WriteLine($"MOSFET voltage rating: {hBridgeReq.mosfetVoltageRating}V");
Console.WriteLine($"MOSFET current rating: {hBridgeReq.mosfetCurrentRating}A");
Console.WriteLine($"Gate drive requirements: {hBridgeReq.gateDriverRequirements}");

// Generate Arduino motor control code
string motorCode = MotorControlCalculator.GenerateArduinoMotorCode(
    enablePin: 9,
    directionPin1: 7,
    directionPin2: 8,
    pwmFrequency: 1000
);

// Calculate thermal requirements
var thermalAnalysis = MotorControlCalculator.CalculateThermalRequirements(
    motorCurrent, pwmFrequency, ambientTemp: 25.0);

Console.WriteLine($"Heat sink required: {thermalAnalysis.heatSinkRequired}");
Console.WriteLine($"Estimated temperature rise: {thermalAnalysis.temperatureRise:F1}°C");
```

### Signal Integrity Analysis

Analyze high-speed digital signals:

```csharp
using CircuitTool;

// PCB trace specifications
double traceWidth = 0.1;         // 0.1mm trace width
double traceLength = 50.0;       // 50mm trace length
double frequency = 100e6;        // 100MHz signal

// Calculate trace impedance
double impedance = PCBDesignCalculator.CalculateTraceImpedance(
    traceWidth: traceWidth,
    traceThickness: 0.035,       // 35µm copper
    dielectricHeight: 0.2,       // 0.2mm dielectric
    dielectricConstant: 4.5      // FR4 εr
);

Console.WriteLine($"Trace impedance: {impedance:F1}Ω");

// Calculate maximum frequency for clean signals
double maxFrequency = SignalIntegrityCalculator.CalculateMaximumFrequency(
    traceLength: traceLength,
    riseTime: 1e-9              // 1ns rise time
);

Console.WriteLine($"Maximum recommended frequency: {maxFrequency / 1e6:F1}MHz");

// EMC considerations
var emcAnalysis = EMCCalculator.AnalyzeEMCCompliance(
    frequency: frequency,
    traceLength: traceLength,
    current: 0.1               // 100mA switching current
);

Console.WriteLine($"EMI risk level: {emcAnalysis.riskLevel}");
Console.WriteLine($"Recommended mitigation: {emcAnalysis.recommendations}");
```

### Communication Protocol Optimization

Optimize I2C, SPI, and UART settings:

```csharp
using CircuitTool;

// I2C Bus Analysis
var i2cConfig = new CommunicationProtocolTools.I2CConfig
{
    ClockFrequency = 400000,     // Fast mode I2C
    SupplyVoltage = 3.3,
    BusCapacitance = 200e-12,    // 200pF bus capacitance
    NumberOfDevices = 4
};

// Calculate optimal pull-up resistors
var (minRes, maxRes, optimalRes) = CommunicationProtocolTools.CalculateI2CPullUpResistors(i2cConfig);
Console.WriteLine($"Optimal I2C pull-up: {optimalRes}Ω");

// SPI Frequency Optimization
var spiConfig = new CommunicationProtocolTools.SPIConfig
{
    ClockFrequency = 10000000,   // 10MHz desired
    CableLength = 0.1,           // 10cm cable
    Mode = CommunicationProtocolTools.SPIMode.Mode0
};

double maxSpiFreq = CommunicationProtocolTools.CalculateMaxSPIFrequency(spiConfig);
Console.WriteLine($"Maximum safe SPI frequency: {maxSpiFreq / 1e6:F1}MHz");

// UART Error Rate Analysis
var uartConfig = new CommunicationProtocolTools.UARTConfig
{
    BaudRate = 115200,
    DataBits = 8,
    StopBits = 1,
    Parity = "None"
};

double bitErrorRate = CommunicationProtocolTools.CalculateUARTBitErrorRate(
    uartConfig, clockAccuracyPpm: 100);
Console.WriteLine($"Estimated bit error rate: {bitErrorRate:P4}");
```

### Hardware Debugging

Diagnose hardware problems automatically:

```csharp
using CircuitTool;

// Hardware configuration
var hwConfig = new HardwareDebuggingTools.HardwareConfig
{
    SupplyVoltage = 3.25,        // Slightly low voltage
    OperatingFrequency = 16e6,   // 16MHz crystal
    AmbientTemperature = 35.0,   // Elevated temperature
    HasCrystal = true,
    HasBrownoutDetection = true
};

// Reported symptoms
var symptoms = new List<string>
{
    "Random resets",
    "I2C communication errors",
    "Timing issues with UART"
};

// Run comprehensive diagnostics
var diagnosticResults = HardwareDebuggingTools.PerformDiagnostics(hwConfig, symptoms);

foreach (var result in diagnosticResults)
{
    Console.WriteLine($"Test: {result.TestName}");
    Console.WriteLine($"Status: {(result.Passed ? "PASS" : "FAIL")}");
    Console.WriteLine($"Details: {result.Details}");
    Console.WriteLine($"Recommendation: {result.Recommendation}");
    Console.WriteLine();
}

// Generate troubleshooting guide
string troubleshootingGuide = HardwareDebuggingTools.GenerateTroubleshootingGuide(symptoms);
Console.WriteLine(troubleshootingGuide);

// Generate Arduino diagnostic sketch
string diagnosticSketch = HardwareDebuggingTools.GenerateArduinoDiagnosticSketch(
    new List<string> { "power", "timing", "memory", "pins" });
```

## Complete Project Examples

### Smart Home Sensor Node

A complete IoT sensor node project:

```csharp
// 1. Power budget analysis
double sensorCurrent = 0.001;    // 1mA sensor
double mcuActiveCurrent = 0.020; // 20mA MCU active
double mcuSleepCurrent = 0.0001; // 0.1mA MCU sleep
double dutyCycle = 0.01;         // 1% duty cycle

double averageCurrent = (mcuActiveCurrent * dutyCycle) + 
                       (mcuSleepCurrent * (1 - dutyCycle)) + 
                       sensorCurrent;

// 2. Battery life calculation
double batteryCapacity = 2000;   // 2000mAh battery
double batteryLife = batteryCapacity / averageCurrent;
Console.WriteLine($"Estimated battery life: {batteryLife:F0} hours");

// 3. Communication setup
string espCode = ESP32Tools.GenerateWiFiCode("MyNetwork", "password");
string sensorCode = ArduinoTools.GenerateAnalogSensorCode(0, "BME280", "readBME280()", "°C/RH%");

// 4. Power supply design
var powerResult = PowerSupplyCalculator.CalculateLinearRegulator(
    inputVoltage: 3.7,   // Li-ion battery
    outputVoltage: 3.3,  // ESP32 supply
    outputCurrent: averageCurrent
);
```

### RF Circuit Design

Design and analyze RF circuits:

```csharp
// Antenna calculations
double frequency = 2.4e9;        // 2.4GHz WiFi
var antennaResult = AntennaCalculator.CalculateDipoleAntenna(frequency);

Console.WriteLine($"Dipole length: {antennaResult.totalLength * 1000:F1}mm");
Console.WriteLine($"Each element: {antennaResult.elementLength * 1000:F1}mm");

// Transmission line analysis
double characteristicImpedance = 50.0;
double lineLength = 0.1;         // 10cm
var transmissionLine = AntennaCalculator.CalculateTransmissionLine(
    frequency, characteristicImpedance, lineLength);

Console.WriteLine($"Electrical length: {transmissionLine.electricalLength:F1}°");
Console.WriteLine($"Propagation delay: {transmissionLine.propagationDelay * 1e9:F2}ns");
```

These examples demonstrate the versatility and power of CircuitTool across different domains of electrical engineering and embedded systems development.
