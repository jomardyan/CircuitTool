# CircuitTool Documentation

Welcome to the comprehensive documentation for **CircuitTool** - a powerful C# library for electrical engineering and electronics calculations.

## Quick Start

CircuitTool provides a wide range of calculators and tools for:

- **Basic Electrical Calculations** - Ohm's Law, Power, Voltage, Current
- **Component Analysis** - Resistors, Capacitors, Inductors, Transformers
- **AC Circuit Analysis** - Impedance, Phase, Frequency Response
- **Hardware Tools** - Arduino, ESP32, Raspberry Pi support
- **Communication Protocols** - I2C, SPI, UART, COM Port tools
- **Advanced Analysis** - Signal Integrity, EMC, Thermal Analysis
- **Performance Optimization** - Vectorized calculations, Caching

## Getting Started

### Installation

Install CircuitTool via NuGet Package Manager:

```bash
dotnet add package CircuitTool
```

Or via Package Manager Console:

```powershell
Install-Package CircuitTool
```

### Basic Usage

```csharp
using CircuitTool;

// Basic Ohm's Law calculation
double voltage = OhmsLawCalculator.CalculateVoltage(current: 2.0, resistance: 100.0);
Console.WriteLine($"Voltage: {voltage}V"); // Output: Voltage: 200V

// LED current limiting resistor
var ledResult = LEDCalculator.CalculateCurrentLimitingResistor(
    supplyVoltage: 5.0, 
    ledVoltage: 2.1, 
    ledCurrent: 0.02);
Console.WriteLine($"Required resistor: {ledResult.resistance}Ω");

// Arduino GPIO code generation
string gpioCode = ArduinoTools.GenerateGPIOCode(pin: 13, mode: "OUTPUT");
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
