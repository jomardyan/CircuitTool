# CircuitTool Documentation

Welcome to the **CircuitTool** documentation! CircuitTool is a comprehensive C# library for electrical engineering and electronics calculations, designed for embedded systems development and hardware design.

## Quick Start

```csharp
using CircuitTool;

// Basic Ohm's Law calculation
double voltage = OhmsLawCalculator.CalculateVoltage(current: 2.0, resistance: 100.0);
Console.WriteLine($"Voltage: {voltage}V"); // Output: 200V

// LED current limiting resistor
var ledResult = LEDCalculator.CalculateCurrentLimitingResistor(
    supplyVoltage: 5.0, 
    ledVoltage: 2.1, 
    ledCurrent: 0.02);
Console.WriteLine($"Resistor needed: {ledResult.resistance}Ω");

// Arduino GPIO configuration
string gpioCode = ArduinoTools.GenerateGPIOCode(pin: 13, mode: "OUTPUT");
```

## Features

### 🧮 Core Calculations
- **Ohm's Law** - Voltage, current, resistance calculations
- **Power Calculations** - Power dissipation and efficiency
- **AC Circuit Analysis** - Impedance, phase, frequency response
- **Component Calculations** - Resistors, capacitors, inductors

### 🔧 Hardware Support
- **Arduino Tools** - GPIO, PWM, and peripheral calculations
- **ESP32 Tools** - WiFi, Bluetooth, and ESP32-specific features
- **Raspberry Pi Tools** - GPIO current limits and hardware interface
- **COM Port Tools** - Serial communication and device detection

### 📊 Advanced Analysis
- **Signal Integrity** - Signal quality and timing analysis
- **Thermal Calculations** - Heat dissipation and thermal management
- **EMC Analysis** - Electromagnetic compatibility calculations
- **Power Analysis** - Complex power system analysis

### ⚡ Performance Features
- **Vectorized Calculations** - SIMD-optimized operations
- **Async Support** - Asynchronous calculation processing
- **Caching System** - Result caching for improved performance
- **Bulk Operations** - Efficient batch processing

## Supported Platforms

- **.NET Framework 4.5+**
- **.NET Framework 4.6.2+**
- **.NET 6.0+**
- **.NET 8.0+**
- **.NET Standard 2.0+**
- **.NET Standard 2.1+**

## Installation

```bash
dotnet add package CircuitTool
```

Or via Package Manager Console:
```powershell
Install-Package CircuitTool
```

## Module Overview

| Module | Classes | Description |
|--------|---------|-------------|
| **Calculators** | 23 | Core electrical and electronics calculations |
| **Hardware** | 10 | Platform-specific tools and communication protocols |
| **Analysis** | 5 | Advanced analysis and simulation tools |
| **Performance** | 6 | Performance optimization and monitoring |
| **Documentation** | 4 | Examples and educational resources |
| **Math** | 2 | Advanced mathematical operations |
| **Serialization** | 1 | Data import/export functionality |
| **Units** | 3 | Unit conversion and management |

## Getting Help

- 📖 **API Reference** - Browse the complete API documentation
- 💡 **Examples** - Check out practical examples and tutorials
- 🐛 **Issues** - Report bugs or request features on [GitHub](https://github.com/jomardyan/CircuitTool/issues)
- 💬 **Discussions** - Join the community discussions

## License

CircuitTool is licensed under the [MIT License](https://github.com/jomardyan/CircuitTool/blob/main/LICENSE), making it free for both commercial and non-commercial use.
