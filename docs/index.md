# CircuitTool Documentation

> **Comprehensive C# library for electrical engineering and electronics calculations**

Welcome to the complete documentation for **CircuitTool** - a modern, high-performance library designed for engineers, students, and developers working with electronic systems.

## 🎯 What is CircuitTool?

CircuitTool provides a comprehensive suite of tools and calculators for:

- **⚡ Electrical Calculations** - Ohm's Law, Power, Voltage, Current, Resistance
- **🔧 Component Analysis** - Resistors, Capacitors, Inductors, Transformers, LEDs
- **📡 AC Circuit Analysis** - Impedance, Phase, Frequency Response, Resonance
- **🤖 Hardware Integration** - Arduino, ESP32, Raspberry Pi support and code generation
- **📡 Communication Protocols** - I2C, SPI, UART, COM Port analysis tools
- **📊 Advanced Analysis** - Signal Integrity, EMC compliance, Thermal management
- **⚡ Performance Optimization** - Vectorized calculations, Intelligent caching

## 🚀 Quick Start Guide

### Installation

Install CircuitTool via NuGet Package Manager:

```bash
# .NET CLI
dotnet add package CircuitTool

# Package Manager Console  
Install-Package CircuitTool
```

### Platform Support
- **.NET Framework**: 4.5, 4.6.2, 4.8.1+
- **.NET Core**: 3.1+  
- **.NET**: 6.0+, 8.0+
- **.NET Standard**: 2.0, 2.1

### Basic Usage Examples

```csharp
using CircuitTool;

// 🧮 Basic Ohm's Law calculation
double voltage = OhmsLawCalculator.CalculateVoltage(current: 2.0, resistance: 100.0);
Console.WriteLine($"Voltage: {voltage}V"); // Output: Voltage: 200V

// 🔧 LED current limiting resistor design
var ledResult = LEDCalculator.CalculateCurrentLimitingResistor(
    supplyVoltage: 5.0, 
    ledVoltage: 2.1, 
    ledCurrent: 0.02);
Console.WriteLine($"Required resistor: {ledResult.resistance}Ω");

// 📡 AC circuit impedance analysis
double impedance = ImpedanceCalculator.CalculateMagnitude(
    resistance: 100,
    reactance: 50);
Console.WriteLine($"Impedance magnitude: {impedance:F1}Ω");

// 🤖 Arduino GPIO code generation
string gpioCode = ArduinoTools.GenerateGPIOCode(pin: 13, mode: "OUTPUT");
Console.WriteLine($"Generated code:\n{gpioCode}");

// 📊 Advanced power analysis
double efficiency = PowerCalculator.CalculateEfficiency(
    inputPower: 100, 
    outputPower: 85);
Console.WriteLine($"Power efficiency: {efficiency:P}");
```

## 📚 Documentation Structure

| Section | Description | Target Audience |
|---------|-------------|-----------------|
| **[🚀 Getting Started](articles/getting-started.html)** | Step-by-step tutorials and basic examples | Beginners |
| **[📖 API Reference](api/)** | Complete API documentation with examples | All users |
| **[🎓 Tutorials](tutorials/)** | In-depth learning materials | Students & Engineers |
| **[💡 Examples](examples/)** | Real-world usage scenarios | Developers |
| **[🔧 Hardware Guides](technology-guides/)** | Platform-specific integration guides | Hardware developers |
| **[💻 CLI Documentation](../CircuitTool.CLI/README.html)** | Interactive command-line interface | All users |

## 🎯 Popular Use Cases

### 🔋 Electronics Design & Prototyping
```csharp
// Power supply design
var regulator = PowerCalculator.DesignLinearRegulator(
    inputVoltage: 12.0,
    outputVoltage: 5.0, 
    loadCurrent: 1.0
);

// Component tolerance analysis
var analysis = ToleranceCalculator.WorstCaseAnalysis(
    components: new[] { 100.0, 200.0, 300.0 },
    tolerances: new[] { 0.05, 0.01, 0.02 }
);
```

### 🤖 Embedded Systems Development
```csharp
// ESP32 battery life estimation
var batteryLife = ESP32Tools.CalculateBatteryLife(
    batteryCapacity: 2000,  // mAh
    activeCurrentMA: 160,
    sleepCurrentUA: 10,
    dutyCycle: 0.01
);

// UART communication analysis
var timing = UARTCommunicationCalculator.CalculateTiming(
    baudRate: 115200,
    dataBits: 8,
    stopBits: 1
);
```

### 📊 Signal Analysis & EMC
```csharp
// Signal integrity analysis
var crosstalk = SignalIntegrityCalculator.CrosstalkCoupling(
    aggressorVoltage: 3.3,
    couplingCapacitance: 1e-12,
    frequency: 100e6
);

// EMC compliance testing
var fieldStrength = EMCCalculator.ElectricFieldStrength(
    power: 1.0,
    distance: 3.0,
    antennaGain: 2.15
);
```

## 🎓 Learning Path

### 1. **Fundamentals** (Start here if you're new)
   - [Getting Started Guide](articles/getting-started.html)
   - [Basic Electrical Calculations](tutorials/)
   - [Ohm's Law and Power](api/CircuitTool.OhmsLawCalculator.html)

### 2. **Component Design**
   - [LED and Resistor Calculations](api/CircuitTool.LEDCalculator.html)
   - [Capacitor and Inductor Analysis](api/CircuitTool.CapacitorCalculator.html)
   - [Filter Design](api/CircuitTool.FilterCalculator.html)

### 3. **Advanced Topics**
   - [AC Circuit Analysis](api/CircuitTool.ACCircuitCalculator.html)
   - [Signal Integrity](api/CircuitTool.Analysis.SignalIntegrityCalculator.html)
   - [Performance Optimization](api/CircuitTool.Performance.html)

### 4. **Hardware Integration**
   - [Arduino Development](api/CircuitTool.Hardware.ArduinoTools.html)
   - [ESP32 Optimization](api/CircuitTool.Hardware.ESP32Tools.html)
   - [Communication Protocols](technology-guides/)

## 🚀 Next Steps

Ready to start using CircuitTool? Here are some recommended next steps:

1. **📥 Install the Package**: Follow the [installation guide](#installation)
2. **🎯 Try the Examples**: Start with the [Getting Started guide](articles/getting-started.html)
3. **💻 Use the CLI**: Experiment with the [Interactive CLI](../CircuitTool.CLI/README.html)
4. **📖 Explore the API**: Browse the [complete API reference](api/)
5. **🤝 Join the Community**: Check out the [GitHub repository](https://github.com/jomardyan/CircuitTool)
```

## Key Features

### 🧮 Comprehensive Calculators
- **23 specialized calculator classes** covering all aspects of electrical engineering
- **Cross-platform compatibility** (.NET Framework 4.5+ to .NET 8.0)
- **Unit conversion** with automatic handling of different measurement systems

### 🔧 Hardware Integration
- **Embedded platform support** for Arduino, ESP32, and Raspberry Pi
- **Communication protocol tools** for I2C, SPI, UART analysis
- **Code generation** for common hardware tasks and configurations

### 📊 Advanced Analysis
- **Signal integrity** calculations for high-speed designs
- **EMC analysis** for electromagnetic compatibility
- **Thermal management** tools for heat dissipation analysis
- **Power analysis** for efficiency optimization

### ⚡ Performance Optimized
- **Vectorized calculations** using SIMD instructions
- **Caching system** for repeated calculations
- **Asynchronous support** for long-running operations
- **Bulk processing** for large datasets

## Documentation Sections

- **[API Reference](api/index.md)** - Complete API documentation
- **[Tutorials](tutorials/index.md)** - Step-by-step guides
- **[Examples](examples/index.md)** - Practical code examples
- **[Hardware Guides](hardware/index.md)** - Platform-specific documentation

## Support & Contributing

- **GitHub Repository**: [https://github.com/jomardyan/CircuitTool](https://github.com/jomardyan/CircuitTool)
- **Issues & Bug Reports**: [GitHub Issues](https://github.com/jomardyan/CircuitTool/issues)
- **License**: MIT License - see [LICENSE](https://github.com/jomardyan/CircuitTool/blob/main/LICENSE)

---

*CircuitTool v2.0.0 - © 2025 Jomardyan - MIT License*
