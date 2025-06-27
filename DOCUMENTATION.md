# CircuitTool - Complete API Documentation

> **Comprehensive C# library for electrical engineering and electronics calculations**

CircuitTool is a modern, high-performance library designed for engineers, students, and developers working with electronic systems. This documentation provides complete API references, usage examples, and implementation guides.

## 📋 Table of Contents

- [🚀 Quick Start](#-quick-start)
- [🧮 Core Calculators](#-core-calculators)
- [🔧 Hardware Platform Support](#-hardware-platform-support)  
- [📊 Advanced Analysis](#-advanced-analysis)
- [⚡ Performance & Optimization](#-performance--optimization)
- [📏 Unit System](#-unit-system)
- [💻 Interactive CLI](#-interactive-cli)
- [🔍 API Reference](#api-reference)
- [🤝 Contributing](#contributing)

---

## 🚀 Quick Start

### Installation Methods

#### NuGet Package Manager
```bash
dotnet add package CircuitTool
```

#### Package Manager Console
```powershell
Install-Package CircuitTool
```

#### PackageReference
Add to your `.csproj` file:
```xml
<PackageReference Include="CircuitTool" Version="2.1.0" />
```

### Platform Compatibility
| Platform | Versions | Status |
|----------|----------|---------|
| **.NET Framework** | 4.5, 4.6.2, 4.8.1+ | ✅ Fully Supported |
| **.NET Core** | 3.1+ | ✅ Fully Supported |
| **.NET** | 6.0+, 8.0+ | ✅ Fully Supported |
| **.NET Standard** | 2.0, 2.1 | ✅ Fully Supported |

### First Steps

```csharp
using CircuitTool;

// Simple voltage calculation
double voltage = OhmsLawCalculator.Voltage(current: 2.0, resistance: 100.0);
Console.WriteLine($"Voltage: {voltage}V"); // Output: 200V

// LED resistor calculation
var result = LEDCalculator.CalculateResistorValue(
    supplyVoltage: 5.0,
    ledVoltage: 2.1, 
    ledCurrent: 0.02
);
Console.WriteLine($"Required resistor: {result}Ω"); // Output: 145Ω
```

---

## Core Calculators

### Ohm's Law & Basic Calculations

```csharp
using CircuitTool;

// Voltage calculations (V = I × R)
double voltage = OhmsLawCalculator.Voltage(current: 2.0, resistance: 100); // 200V

// Current calculations (I = V / R)
double current = OhmsLawCalculator.Current(voltage: 12.0, resistance: 1000); // 0.012A

// Resistance calculations (R = V / I)
double resistance = OhmsLawCalculator.Resistance(voltage: 5.0, current: 0.02); // 250Ω

// Power calculations
double power = PowerCalculator.Power(voltage: 12.0, current: 2.0); // 24W
double powerFromVR = PowerCalculator.PowerFromVoltageResistance(voltage: 12.0, resistance: 6.0); // 24W
double powerFromIR = PowerCalculator.PowerFromCurrentResistance(current: 2.0, resistance: 6.0); // 24W
```

### LED Circuit Calculations

```csharp
using CircuitTool;

// Calculate resistor value for LED
double resistorValue = LEDCalculator.CalculateResistorValue(9.0, 2.0, 0.02);  // 9V supply, 2V LED, 20mA = 350Ω

// Calculate LED power consumption
double ledPower = LEDCalculator.CalculateLEDPower(5.0, 0.02);  // 5V, 20mA = 0.1W

// Calculate brightness from PWM duty cycle
double brightness = LEDCalculator.CalculateBrightness(75);  // 75% duty cycle = 0.75 brightness

// Calculate resistor for series LEDs
double seriesResistor = LEDCalculator.CalculateSeriesResistor(12.0, 3.3, 3, 0.02);  // 12V, 3x 3.3V LEDs, 20mA
```

### Voltage Analysis

```csharp
using CircuitTool;

// Calculate voltage drop in a conductor
double voltageDrop = VoltageDropCalculator.CalculateVoltageDrop(5.0, 0.1, 100); // 5A, 0.1Ω/km, 100m = 0.05V

// Calculate conductor resistance
double resistance = VoltageDropCalculator.CalculateConductorResistance(0.05, 5.0, 100); // 0.05V drop, 5A, 100m = 0.1Ω/km

// Calculate maximum current for allowed voltage drop
double maxCurrent = VoltageDropCalculator.CalculateMaxCurrent(0.1, 0.1, 100); // 0.1V max drop, 0.1Ω/km, 100m = 10A

// Calculate voltage divider output
double outputVoltage = VoltageDividerCalculator.CalculateOutputVoltage(12, 1000, 2000); // 12V, 1kΩ, 2kΩ = 8V

// Calculate required resistor for voltage divider
double requiredR2 = VoltageDividerCalculator.CalculateR2(12, 8, 1000); // 12V in, 8V out, 1kΩ R1 = 2kΩ

// Calculate input voltage from voltage divider
double inputVoltage = VoltageDividerCalculator.CalculateInputVoltage(8, 1000, 2000); // 8V out, 1kΩ, 2kΩ = 12V
```

### Resistor Networks

```csharp
using CircuitTool;

// Calculate total resistance in series
double seriesResistance = CircuitCalculations.CalculateTotalResistance(new double[] { 10, 20, 30 }, true); // 60Ω

// Calculate total resistance in parallel
double parallelResistance = CircuitCalculations.CalculateTotalResistance(new double[] { 10, 20, 30 }, false); // ~5.45Ω
```

---

## Advanced Calculators

### Capacitor Calculations

```csharp
using CircuitTool;

// Calculate capacitive reactance
double reactance = CapacitorCalculator.CapacitiveReactance(frequency: 1000, capacitance: 0.000001); // 1μF at 1kHz = 159.15Ω

// Calculate energy stored in capacitor
double energy = CapacitorCalculator.EnergyStored(capacitance: 0.000001, voltage: 12); // 1μF at 12V = 72μJ

// Calculate RC time constant
double timeConstant = CapacitorCalculator.TimeConstant(resistance: 1000, capacitance: 0.000001); // 1kΩ, 1μF = 1ms

// Calculate total capacitance in series
double seriesCapacitance = CapacitorCalculator.SeriesCapacitance(new double[] { 0.000001, 0.000002 }); // 1μF, 2μF in series = 0.67μF

// Calculate total capacitance in parallel
double parallelCapacitance = CapacitorCalculator.ParallelCapacitance(new double[] { 0.000001, 0.000002 }); // 1μF, 2μF in parallel = 3μF

// Calculate charging voltage
double voltage = CapacitorCalculator.ChargingVoltage(sourceVoltage: 12, time: 0.001, timeConstant: 0.001); // 12V source, t=1ms, τ=1ms = 7.59V

// Calculate discharging voltage
double dischargingVoltage = CapacitorCalculator.DischargingVoltage(initialVoltage: 12, time: 0.001, timeConstant: 0.001); // 12V initial, t=1ms, τ=1ms = 4.41V
```

### Inductor Calculations

```csharp
using CircuitTool;

// Calculate inductive reactance
double reactance = InductorCalculator.InductiveReactance(frequency: 1000, inductance: 0.001); // 1mH at 1kHz = 6.28Ω

// Calculate energy stored in inductor
double energy = InductorCalculator.EnergyStored(inductance: 0.001, current: 2); // 1mH with 2A = 2mJ

// Calculate RL time constant
double timeConstant = InductorCalculator.TimeConstant(inductance: 0.001, resistance: 100); // 1mH, 100Ω = 10μs

// Calculate total inductance in series
double seriesInductance = InductorCalculator.SeriesInductance(new double[] { 0.001, 0.002 }); // 1mH, 2mH in series = 3mH

// Calculate total inductance in parallel
double parallelInductance = InductorCalculator.ParallelInductance(new double[] { 0.001, 0.002 }); // 1mH, 2mH in parallel = 0.67mH

// Calculate current buildup
double current = InductorCalculator.CurrentBuildup(finalCurrent: 2, time: 0.00001, timeConstant: 0.00001); // 2A final, t=10μs, τ=10μs = 1.26A

// Calculate resonant frequency
double frequency = InductorCalculator.ResonantFrequency(inductance: 0.001, capacitance: 0.000001); // 1mH, 1μF = 5.03kHz
```

### Transformer Calculations

```csharp
using CircuitTool;

// Calculate secondary voltage
double secondaryVoltage = TransformerCalculator.SecondaryVoltage(primaryVoltage: 120, turnsRatio: 10); // 120V primary, 10:1 ratio = 12V

// Calculate secondary current
double secondaryCurrent = TransformerCalculator.SecondaryCurrent(primaryCurrent: 2, turnsRatio: 10); // 2A primary, 10:1 ratio = 20A

// Calculate turns ratio
double turnsRatio = TransformerCalculator.TurnsRatio(primaryVoltage: 120, secondaryVoltage: 12); // 120V to 12V = 10:1

// Calculate voltage ratio
double voltageRatio = TransformerCalculator.VoltageRatio(primaryVoltage: 240, secondaryVoltage: 120); // 240V to 120V = 2:1

// Calculate efficiency
double efficiency = TransformerCalculator.Efficiency(outputPower: 1000, inputPower: 1100); // 1000W out, 1100W in = 90.9%

// Calculate power loss
double powerLoss = TransformerCalculator.PowerLoss(inputPower: 1100, outputPower: 1000); // 1100W in, 1000W out = 100W loss

// Calculate voltage regulation
double regulation = TransformerCalculator.VoltageRegulation(noLoadVoltage: 120, fullLoadVoltage: 115); // 120V no-load, 115V load = 4.35%

// Calculate apparent power
double apparentPower = TransformerCalculator.ApparentPower(voltage: 120, current: 10); // 120V, 10A = 1200VA
```

### AC Circuit Analysis

```csharp
using CircuitTool;

// Calculate impedance
double impedance = ACCircuitCalculator.CalculateImpedance(voltage: 120, current: 5); // 120V, 5A = 24Ω

// Calculate phase angle
double phaseAngle = ACCircuitCalculator.CalculatePhase(voltage: ACVoltage.FromPolar(120, 0), current: ACCurrent.FromPolar(5, -30)); // 30° lag

// Calculate resonant frequency
double frequency = ACCircuitCalculator.CalculateFrequency(inductance: 0.001, capacitance: 0.000001); // 1mH, 1μF = 5.03kHz

// Calculate power factor
double powerFactor = ACCircuitCalculator.PowerFactor(phaseAngle: 30); // 30° = 0.866

// Calculate RMS from peak
double rms = ACCircuitCalculator.RMS(peakValue: 170); // 170V peak = 120.21V RMS

// Calculate peak from RMS
double peak = ACCircuitCalculator.Peak(rmsValue: 120); // 120V RMS = 169.74V peak

// Calculate average from peak
double average = ACCircuitCalculator.Average(peakValue: 170); // 170V peak = 108.13V average
```

### Filter Calculations

```csharp
using CircuitTool;

// Calculate RC low-pass filter cutoff frequency
double cutoffFreq = FilterCalculator.RCLowPassCutoff(resistance: 1000, capacitance: 0.000001); // 1kΩ, 1μF = 159.15Hz

// Calculate RC high-pass filter cutoff frequency
double highPassCutoff = FilterCalculator.RCHighPassCutoff(resistance: 1000, capacitance: 0.000001); // 1kΩ, 1μF = 159.15Hz

// Calculate RL low-pass filter cutoff frequency
double rlCutoff = FilterCalculator.RLLowPassCutoff(resistance: 100, inductance: 0.001); // 100Ω, 1mH = 15.92kHz

// Calculate RL high-pass filter cutoff frequency
double rlHighPass = FilterCalculator.RLHighPassCutoff(resistance: 100, inductance: 0.001); // 100Ω, 1mH = 15.92kHz

// Calculate filter gain in dB
double gain = FilterCalculator.FilterGain(frequency: 1000, cutoffFrequency: 159.15); // 1kHz signal, 159.15Hz cutoff = -15.97dB

// Calculate filter phase shift
double phaseShift = FilterCalculator.FilterPhaseShift(frequency: 1000, cutoffFrequency: 159.15); // 1kHz signal, 159.15Hz cutoff = -80.96°

// Calculate magnitude response
double magnitude = FilterCalculator.MagnitudeResponse(frequency: 1000, cutoffFrequency: 159.15); // 1kHz signal, 159.15Hz cutoff = 0.158

// Calculate required resistor for RC filter
double resistor = FilterCalculator.RequiredResistor(cutoffFrequency: 159.15, capacitance: 0.000001); // 159.15Hz cutoff, 1μF = 1kΩ

// Calculate required capacitor for RC filter
double capacitor = FilterCalculator.RequiredCapacitor(cutoffFrequency: 159.15, resistance: 1000); // 159.15Hz cutoff, 1kΩ = 1μF
```

---

## Hardware Support

### Arduino Tools

```csharp
using CircuitTool;

// Convert Arduino analog reading to voltage
double voltage = ArduinoTools.AnalogToVoltage(512);  // 512 reading = 2.5V (on 5V Arduino)

// Convert voltage to analog reading
int analogReading = ArduinoTools.VoltageToAnalog(3.3);  // 3.3V = 675 reading

// Calculate servo pulse width for specific angle
double pulseWidth = ArduinoTools.ServoAngleToPulseWidth(90);  // 90° = 1500μs pulse

// Calculate current consumption
double current = ArduinoTools.CalculateCurrentConsumption(20, 5, 2, 50);  // CPU + pins + external = total mA

// Generate GPIO configuration code
string gpioCode = ArduinoTools.GenerateGPIOCode(pin: 13, mode: "OUTPUT");
```

### ESP32 Tools

```csharp
using CircuitTool;

// Convert ESP32 analog reading to voltage
double voltage = ESP32Tools.AnalogToVoltage(2048);  // 2048 reading = 1.65V (on 3.3V ESP32)

// Calculate WiFi power consumption
double wifiPower = ESP32Tools.CalculateWiFiPowerConsumption(WiFiMode.Active);  // 80mA

// Calculate total ESP32 current consumption
double totalCurrent = ESP32Tools.CalculateTotalCurrentConsumption(240, WiFiMode.Active, true, 20);

// Calculate battery life
double batteryLife = ESP32Tools.CalculateBatteryLife(2000, 50);  // 2000mAh battery, 50mA load = 32 hours

// Calculate touch sensor threshold
int touchThreshold = ESP32Tools.CalculateTouchThreshold(1000, 0.3);  // 1000 baseline, 30% sensitivity
```

### Raspberry Pi Tools

```csharp
using CircuitTool;

// GPIO voltage level calculations
double gpioVoltage = RaspberryPiTools.CalculateGPIOVoltage(3.3, 1);  // 3.3V logic high

// I2C pull-up resistor calculation
double pullUpResistor = RaspberryPiTools.CalculateI2CPullUp(3.3, 0.003, 400000);  // 3.3V, 3mA, 400kHz

// SPI timing calculations
var spiTiming = RaspberryPiTools.CalculateSPITiming(1000000);  // 1MHz SPI clock
```

### COM Port Tools

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
int detectedBaudRate = ComPortTools.AutoDetectBaudRate(portName, ComPortTools.ArduinoBaudRates);

// Monitor COM port
var config = new ComPortTools.SerialConfig
{
    PortName = "COM3",
    BaudRate = 115200,
    Parity = ComPortTools.SerialParity.None
};

string monitorLog = ComPortTools.MonitorPort(config, durationSeconds: 10);
```

---

## Performance & Optimization

### Beginner Calculator Examples

```csharp
using CircuitTool;

// Calculate battery life
double hours = BeginnerCalculators.BatteryLifeCalculator(1000, 50);  // 1000mAh, 50mA = 20 hours

// Determine wire gauge
int awgGauge = BeginnerCalculators.WireGaugeCalculator(3.0);  // 3A = AWG 20

// Calculate RC time constant capacitor value
double capacitor = BeginnerCalculators.RCTimeConstantCapacitor(1000, 0.001);  // 1kΩ, 1ms = 1μF

// Calculate RC oscillator frequency
double frequency = BeginnerCalculators.RCOscillatorFrequency(1000, 0.000001);  // 1kΩ, 1μF = ~455Hz

// Convert power ratio to decibels
double db = BeginnerCalculators.PowerRatioToDecibels(10);  // 10x power = 10dB

// Convert voltage ratio to decibels
double dbVoltage = BeginnerCalculators.VoltageRatioToDecibels(2);  // 2x voltage = 6.02dB

// Calculate transformer turns ratio
double turnsRatio = BeginnerCalculators.TransformerTurnsRatio(120, 12);  // 120V to 12V = 0.1 ratio
```

### Power and Energy Calculations

```csharp
using CircuitTool;

// Power factor calculations
double apparentPower = PowerFactorCalculator.ApparentPower(1000, 0.8);  // 1000W real, 0.8 PF = 1250VA
double reactivePower = PowerFactorCalculator.ReactivePower(1000, 0.8);  // 1000W real, 0.8 PF = 750VAR

// Energy consumption calculations
double monthlyCost = EnergyConsumptionCalculator.MonthlyCost(5000, 0.12);  // 5kWh, $0.12/kWh = $600/month
double carbonFootprint = EnergyConsumptionCalculator.CarbonFootprint(1000, 0.5);  // 1kWh, 0.5kg/kWh = 0.5kg CO2

// Electricity bill calculation
double billAmount = ElectricityBillCalculator.CalculateBill(150, 0.12, 15.00);  // 150kWh, $0.12/kWh, $15 fixed = $33
```

### Vectorized Operations

For high-performance scenarios with large datasets:

```csharp
using CircuitTool;

// Vectorized calculations for multiple values
double[] currents = { 0.1, 0.2, 0.3, 0.4, 0.5 };
double[] resistances = { 100, 200, 300, 400, 500 };

double[] voltages = VectorizedCalculations.CalculateVoltagesVector(currents, resistances);

// Bulk power calculations
double[] powers = VectorizedCalculations.CalculatePowersVector(voltages, currents);

// Parallel processing for large datasets
var results = BulkOperations.ProcessCircuitAnalysisParallel(circuitData);
```

---

## Unit Conversion

```csharp
using CircuitTool;

// Convert between electrical units
double milliamps = UnitConverter.AmperesToMilliamps(0.5);  // 0.5A = 500mA
double kilovolts = UnitConverter.VoltsToKilovolts(5000);  // 5000V = 5kV
double megaohms = UnitConverter.OhmsToMegaohms(2000000);  // 2MΩ = 2000000Ω

// Comprehensive unit conversions
double microfarads = UnitConverter.ConvertCapacitance(0.000001, "F", "µF");  // 1µF
double millihenries = UnitConverter.ConvertInductance(0.001, "H", "mH");  // 1mH
double megahertz = UnitConverter.ConvertFrequency(1000000, "Hz", "MHz");  // 1MHz
```

---

## Interactive CLI

The CircuitTool CLI provides an interactive way to test calculations and explore the library's capabilities.

### Installation & Basic Usage

The CLI is included as a separate project. To use it:

```bash
# Run interactively
CircuitTool.CLI

# Command-line mode examples
CircuitTool.CLI basic ohms --voltage 12 --current 2
CircuitTool.CLI component led --supply 5 --forward 2.1 --current 0.02
CircuitTool.CLI ac reactance --frequency 1000 --inductance 0.01
CircuitTool.CLI power energy --power 100 --time 24
CircuitTool.CLI benchmark --iterations 10000
```

### CLI Features

- **🎯 Interactive Mode**: Guided menu system for step-by-step calculations
- **⚡ Command Line**: Direct calculation commands with parameters
- **📊 Multiple Formats**: Output in table, JSON, or CSV format
- **🔧 Examples**: Built-in examples and tutorials
- **📈 Benchmarking**: Performance testing with customizable iterations
- **📚 Help System**: Comprehensive help and usage information

See the [CLI Documentation](CircuitTool.CLI/README.md) for complete usage guide.

---

## API Reference

### Error Handling

CircuitTool includes comprehensive error handling for invalid inputs:

```csharp
try
{
    // This will throw an exception for invalid values
    double result = OhmsLawCalculator.Voltage(
        current: -1,  // Negative current
        resistance: 0 // Zero resistance
    );
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Invalid input: {ex.Message}");
}
catch (CircuitCalculationException ex)
{
    Console.WriteLine($"Calculation error: {ex.Message}");
}
```

### Configuration Options

```csharp
using CircuitTool;

// Configure calculation precision
CalculationSettings.DefaultPrecision = 6;

// Enable performance monitoring
PerformanceMonitor.EnableMonitoring = true;

// Configure unit preferences
UnitPreferences.DefaultCurrentUnit = CurrentUnit.Milliamps;
UnitPreferences.DefaultVoltageUnit = VoltageUnit.Volts;
```

### Extension Methods

```csharp
using CircuitTool.Extensions;

// Extension methods for common calculations
double power = 12.0.Volts() * 2.0.Amps();  // 24W
double resistance = 5.0.Volts() / 0.02.Amps();  // 250Ω

// Fluent calculation chains
var result = 12.0.Volts()
    .WithCurrent(2.0.Amps())
    .CalculatePower()
    .ToWatts();
```

---

## Contributing

We welcome contributions to CircuitTool! Here's how you can help:

### Development Setup

1. **Clone the repository**:
   ```bash
   git clone https://github.com/jomardyan/CircuitTool.git
   cd CircuitTool
   ```

2. **Build the project**:
   ```bash
   dotnet build
   ```

3. **Run tests**:
   ```bash
   dotnet test
   ```

### Contribution Guidelines

1. **Code Standards**: Follow C# coding conventions and use XML documentation
2. **Testing**: Include unit tests for all new functionality
3. **Documentation**: Update relevant documentation and examples
4. **Compatibility**: Ensure cross-framework compatibility

### Areas for Contribution

- **🧮 New Calculators**: Specialized calculation modules
- **🔧 Hardware Support**: Additional microcontroller platforms  
- **📊 Analysis Tools**: Advanced simulation capabilities
- **🌐 Localization**: Multi-language support
- **📚 Documentation**: Examples, tutorials, and guides
- **⚡ Performance**: Optimization and vectorization improvements

### Project Structure

CircuitTool is organized into logical modules:

- **`src/Calculators/`** (23 classes) - Core electrical calculations
- **`src/Hardware/`** (10 classes) - Platform-specific tools
- **`src/Analysis/`** (5 classes) - Advanced analysis capabilities
- **`src/Performance/`** (6 classes) - Optimization and caching
- **`tests/`** (30+ classes) - Comprehensive test coverage

See the [Project Code Map](PROJECT_CODE_MAP.md) for detailed architecture information.

---

*For more information, visit the [GitHub repository](https://github.com/jomardyan/CircuitTool) or check out the [Getting Started Guide](articles/getting-started.md).*
