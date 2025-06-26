# Getting Started with CircuitTool

This guide will help you get started with CircuitTool, a comprehensive C# library for electrical engineering calculations.

## Installation

### Via NuGet Package Manager

```bash
dotnet add package CircuitTool
```

### Via Package Manager Console

```powershell
Install-Package CircuitTool
```

### Via PackageReference

Add this to your `.csproj` file:

```xml
<PackageReference Include="CircuitTool" Version="2.0.0" />
```

## Basic Usage

### 1. Simple Ohm's Law Calculations

```csharp
using CircuitTool;

// Calculate voltage (V = I × R)
double voltage = OhmsLawCalculator.CalculateVoltage(current: 0.5, resistance: 220);
Console.WriteLine($"Voltage: {voltage}V"); // Output: 110V

// Calculate current (I = V / R)
double current = OhmsLawCalculator.CalculateCurrent(voltage: 12, resistance: 1000);
Console.WriteLine($"Current: {current}A"); // Output: 0.012A

// Calculate resistance (R = V / I)
double resistance = OhmsLawCalculator.CalculateResistance(voltage: 5, current: 0.02);
Console.WriteLine($"Resistance: {resistance}Ω"); // Output: 250Ω

// Calculate power (P = V × I)
double power = OhmsLawCalculator.CalculatePower(voltage: 12, current: 2);
Console.WriteLine($"Power: {power}W"); // Output: 24W
```

### 2. LED Current Limiting Calculations

```csharp
using CircuitTool;

// Calculate current limiting resistor for LED
var ledResult = LEDCalculator.CalculateCurrentLimitingResistor(
    supplyVoltage: 5.0,
    ledVoltage: 2.1,
    ledCurrent: 0.02  // 20mA
);

Console.WriteLine($"Resistor value: {ledResult.resistance}Ω");
Console.WriteLine($"Power dissipation: {ledResult.powerDissipation}W");
Console.WriteLine($"Recommended resistor: {ledResult.recommendedStandardValue}Ω");
```

### 3. Resistor Network Calculations

```csharp
using CircuitTool;

// Series resistance
double[] seriesResistors = { 100, 220, 470 };
double totalSeries = ResistorCalculator.CalculateSeriesResistance(seriesResistors);
Console.WriteLine($"Total series resistance: {totalSeries}Ω"); // Output: 790Ω

// Parallel resistance
double[] parallelResistors = { 1000, 2200, 4700 };
double totalParallel = ResistorCalculator.CalculateParallelResistance(parallelResistors);
Console.WriteLine($"Total parallel resistance: {totalParallel:F1}Ω");
```

### 4. Voltage Divider Calculations

```csharp
using CircuitTool;

// Calculate output voltage of voltage divider
double outputVoltage = VoltageDividerCalculator.CalculateOutputVoltage(
    inputVoltage: 12.0,
    r1: 1000,  // Upper resistor
    r2: 2000   // Lower resistor
);
Console.WriteLine($"Output voltage: {outputVoltage}V"); // Output: 8V

// Calculate resistor values for desired output
var dividerResult = VoltageDividerCalculator.CalculateResistorValues(
    inputVoltage: 5.0,
    outputVoltage: 3.3,
    currentLoad: 0.001  // 1mA load
);
Console.WriteLine($"R1: {dividerResult.r1}Ω, R2: {dividerResult.r2}Ω");
```

## Hardware Platform Support

### Arduino Development

```csharp
using CircuitTool;

// Generate Arduino GPIO code
string gpioCode = ArduinoTools.GenerateGPIOCode(
    pin: 13,
    mode: "OUTPUT"
);
Console.WriteLine(gpioCode);

// Calculate PWM values
var pwmResult = ArduinoTools.CalculatePWMValues(
    dutyCycle: 0.75,  // 75%
    frequency: 1000   // 1kHz
);
Console.WriteLine($"PWM value: {pwmResult.pwmValue}");

// Generate LED control code
string ledCode = ArduinoTools.GenerateLEDCode(
    pin: 9,
    brightness: 128,  // 50% brightness
    usePWM: true
);
```

### ESP32 Development

```csharp
using CircuitTool;

// Calculate WiFi power consumption
var wifiPower = ESP32Tools.CalculateWiFiPowerConsumption(
    mode: "STA",
    transmitPower: 20,  // 20dBm
    dutyCycle: 0.1      // 10% active
);
Console.WriteLine($"Average power: {wifiPower.averagePower}mW");

// Generate ESP32 GPIO code
string esp32Code = ESP32Tools.GenerateGPIOCode(
    pin: 2,
    mode: "OUTPUT",
    pullMode: "NONE"
);
```

### COM Port Communication

```csharp
using CircuitTool;

// Discover available COM ports
var availablePorts = ComPortTools.GetAvailablePorts();
foreach (var port in availablePorts)
{
    Console.WriteLine($"{port.PortName}: {port.DeviceType} - {port.Description}");
}

// Auto-detect baud rate
string portName = "COM3";
int detectedBaudRate = ComPortTools.AutoDetectBaudRate(
    portName, 
    ComPortTools.ArduinoBaudRates
);
Console.WriteLine($"Detected baud rate: {detectedBaudRate}");

// Monitor COM port
var config = new ComPortTools.SerialConfig
{
    PortName = "COM3",
    BaudRate = 115200,
    Parity = ComPortTools.SerialParity.None
};

string monitorLog = ComPortTools.MonitorPort(config, durationSeconds: 10);
Console.WriteLine(monitorLog);
```

## Advanced Features

### Power Analysis

```csharp
using CircuitTool;

// Calculate power supply efficiency
var efficiency = PowerCalculator.CalculateEfficiency(
    inputPower: 100,
    outputPower: 85
);
Console.WriteLine($"Efficiency: {efficiency:P1}"); // Output: 85.0%

// Battery life calculation
var batteryLife = EnergyCalculator.CalculateBatteryLife(
    batteryCapacity: 2500,  // mAh
    averageCurrent: 150,    // mA
    efficiencyFactor: 0.85
);
Console.WriteLine($"Battery life: {batteryLife:F1} hours");
```

### Signal Analysis

```csharp
using CircuitTool;

// Calculate filter cutoff frequency
double cutoffFreq = FilterCalculator.CalculateLowPassCutoff(
    resistance: 1000,     // 1kΩ
    capacitance: 0.000001 // 1µF
);
Console.WriteLine($"Cutoff frequency: {cutoffFreq:F1} Hz");

// Signal integrity analysis
var signalResult = SignalIntegrityCalculator.AnalyzeSignalPath(
    traceLength: 0.1,      // 10cm
    frequency: 100000000,  // 100MHz
    impedance: 50          // 50Ω
);
Console.WriteLine($"Signal delay: {signalResult.propagationDelay}ns");
```

## Unit Conversions

```csharp
using CircuitTool;

// Convert between units
double milliamps = UnitConverter.ConvertCurrent(2.5, "A", "mA");
Console.WriteLine($"2.5A = {milliamps}mA"); // Output: 2500mA

double kilovolts = UnitConverter.ConvertVoltage(3300, "mV", "kV");
Console.WriteLine($"3300mV = {kilovolts}kV"); // Output: 0.0033kV

double microfarads = UnitConverter.ConvertCapacitance(0.000001, "F", "µF");
Console.WriteLine($"0.000001F = {microfarads}µF"); // Output: 1µF
```

## Error Handling

CircuitTool includes comprehensive error handling:

```csharp
try
{
    // This will throw an exception for invalid values
    double result = OhmsLawCalculator.CalculateVoltage(
        current: -1,  // Negative current
        resistance: 0 // Zero resistance
    );
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Invalid input: {ex.Message}");
}
```

## Performance Optimization

For high-performance scenarios, use the vectorized operations:

```csharp
using CircuitTool;

// Vectorized calculations for multiple values
double[] currents = { 0.1, 0.2, 0.3, 0.4, 0.5 };
double[] resistances = { 100, 200, 300, 400, 500 };

double[] voltages = VectorizedCalculations.CalculateVoltagesVector(
    currents, 
    resistances
);

foreach (var voltage in voltages)
{
    Console.WriteLine($"Voltage: {voltage}V");
}
```

## Next Steps

- Explore the [API Reference](../api/) for complete method documentation
- Check out more [examples and tutorials](examples.md)
- Learn about [advanced features](advanced-features.md)
- Read about [hardware integration](hardware-integration.md)
