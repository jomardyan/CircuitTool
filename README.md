# 🔌 CircuitTool

[![Build Status](https://github.com/jomardyan/CircuitTool/actions/workflows/dotnet.yml/badge.svg)](https://github.com/jomardyan/CircuitTool/actions/workflows/dotnet.yml)
[![NuGet Version](https://img.shields.io/nuget/v/CircuitTool)](https://www.nuget.org/packages/CircuitTool)
[![NuGet Downloads](https://img.shields.io/nuget/dt/CircuitTool)](https://www.nuget.org/packages/CircuitTool)
[![GitHub Release](https://img.shields.io/github/v/release/jomardyan/CircuitTool)](https://github.com/jomardyan/CircuitTool/releases)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)

> **A comprehensive C# library for electrical engineering and electronics calculations**

CircuitTool is a modern, high-performance library that provides utilities for circuit analysis, power calculations, unit conversions, microcontroller development, and much more. Designed for engineers, students, and developers working with electronic systems.

## ✨ Features

### 🧮 Core Electrical Calculations
- **Ohm's Law & Power**: Voltage, current, resistance, and power calculations
- **Component Analysis**: Resistors, capacitors, inductors, transformers
- **Circuit Networks**: Series/parallel combinations, voltage dividers
- **Unit Conversions**: Comprehensive electrical units system

### 🔧 Component Design & Analysis
- **LED Design**: Current limiting resistors, brightness calculations
- **Filter Design**: RC/RL low-pass, high-pass, band-pass filters
- **Power Systems**: Efficiency, regulation, transformer calculations
- **Tolerance Analysis**: Worst-case and statistical analysis

### 📡 AC Circuit Analysis
- **Impedance & Reactance**: Inductive and capacitive reactance
- **Frequency Response**: Magnitude and phase calculations
- **Resonance**: RLC circuit resonant frequency analysis
- **Power Factor**: Real, reactive, and apparent power

### 🤖 Hardware Platform Support
- **Arduino**: Pin configuration, ADC/PWM, code generation
- **ESP32**: WiFi power calculations, GPIO management
- **Raspberry Pi**: GPIO control, hardware interface tools
- **General MCU**: Universal microcontroller utilities

### 📊 Advanced Analysis
- **Signal Integrity**: Transmission line analysis, crosstalk
- **EMC Compliance**: Electric field strength, shielding effectiveness
- **Thermal Management**: Heat transfer, junction temperature
- **Performance**: Vectorized calculations, caching, async operations

### 💻 Development Tools
- **Interactive CLI**: Command-line interface for testing and learning
- **Code Generation**: Hardware-specific code templates
- **Data Export**: Circuit serialization (JSON/XML)
- **Documentation**: Comprehensive examples and tutorials

## 🚀 Quick Start

### Installation

Install CircuitTool via NuGet Package Manager:

```bash
# .NET CLI
dotnet add package CircuitTool

# Package Manager Console
Install-Package CircuitTool

# PackageReference (add to .csproj)
<PackageReference Include="CircuitTool" Version="2.1.0" />
```

### Platform Support
- **.NET Framework**: 4.5, 4.6.2, 4.8.1+
- **.NET Core**: 3.1+
- **.NET**: 6.0+, 8.0+
- **.NET Standard**: 2.0, 2.1

### Basic Usage Examples

```csharp
using CircuitTool;

// 🧮 Ohm's Law calculations
double voltage = OhmsLawCalculator.Voltage(current: 2.0, resistance: 100); // 200V
double power = PowerCalculator.Power(voltage: 12.0, current: 2.0); // 24W

// 🔧 LED current limiting resistor
double resistor = LEDCalculator.CalculateResistorValue(
    supplyVoltage: 5.0,
    ledVoltage: 2.1,
    ledCurrent: 0.02  // 20mA
); // 145Ω

// 📡 AC circuit analysis
double reactance = CapacitorCalculator.CapacitiveReactance(
    frequency: 1000,    // 1kHz
    capacitance: 1e-6   // 1µF
); // ~159Ω

// 🤖 Arduino development
double analogVoltage = ArduinoTools.AnalogToVoltage(512); // 2.5V
string gpioCode = ArduinoTools.GenerateGPIOCode(pin: 13, mode: "OUTPUT");

// 📊 Advanced analysis
var noiseAnalysis = NoiseCalculator.ThermalNoise(resistance: 1000, temperature: 25);
var powerEfficiency = PowerCalculator.Efficiency(inputPower: 100, outputPower: 85);
```

## 📚 Documentation & Resources

| Resource | Description | Link |
|----------|-------------|------|
| 📖 **API Documentation** | Complete API reference with examples | [API Docs](docs/api/) |
| 🚀 **Getting Started** | Step-by-step tutorial guide | [Getting Started](articles/getting-started.md) |
| 💻 **Interactive CLI** | Command-line interface guide | [CLI Documentation](CircuitTool.CLI/README.md) |
| 🏗️ **Architecture Guide** | Project structure and modules | [Code Map](PROJECT_CODE_MAP.md) |
| 🔧 **Hardware Guides** | Platform-specific tutorials | [Hardware Docs](docs/technology-guides/) |
| 📋 **Examples** | Real-world usage examples | [Examples](docs/examples/) |
| 🎓 **Tutorials** | Learning materials | [Tutorials](docs/tutorials/) |

### � Interactive CLI

Explore CircuitTool capabilities with the interactive command-line interface:

```bash
# 🎯 Interactive mode (guided menu)
CircuitTool.CLI

# 🧮 Direct calculations
CircuitTool.CLI basic ohms --voltage 12 --current 2
CircuitTool.CLI component led --supply 5 --forward 2.1 --current 0.02
CircuitTool.CLI ac impedance --resistance 100 --reactance 50

# 📊 Performance testing
CircuitTool.CLI benchmark --iterations 10000
CircuitTool.CLI performance --test vectorized
```

### 🎯 Quick Examples

#### Electronics Design
```csharp
// Power supply design
var powerSupply = PowerCalculator.DesignLinearRegulator(
    inputVoltage: 12.0,
    outputVoltage: 5.0,
    loadCurrent: 1.0
);

// Component tolerance analysis
var tolerance = ToleranceCalculator.WorstCaseAnalysis(
    nominalValues: new[] { 100, 200, 300 },
    tolerances: new[] { 0.05, 0.01, 0.02 }
);
```

#### Embedded Development
```csharp
// ESP32 power optimization
var batteryLife = ESP32Tools.CalculateBatteryLife(
    batteryCapacity: 2000,  // mAh
    activeCurrentMA: 160,
    sleepCurrentUA: 10,
    activeTimePercent: 1
);

// Communication protocol analysis
var uart = UARTCommunicationCalculator.CalculateTiming(
    baudRate: 115200,
    dataBits: 8,
    stopBits: 1
);
```

## 🏗️ Architecture & Module Overview

CircuitTool is organized into focused, cohesive modules for maximum usability:

| Module | Classes | Purpose | Key Features |
|--------|---------|---------|--------------|
| **🧮 Calculators** | 23 | Core electrical calculations | Ohm's Law, Power, Components, Filters |
| **🔧 Hardware** | 10 | Platform-specific tools | Arduino, ESP32, RPi, Code generation |
| **📊 Analysis** | 5 | Advanced engineering analysis | Signal integrity, EMC, Thermal |
| **⚡ Performance** | 6 | Optimization & efficiency | Vectorization, Caching, Async |
| **📚 Documentation** | 4 | Examples & learning materials | Tutorials, Interactive guides |
| **🔢 Math** | 2 | Mathematical operations | Matrix operations, FFT/DFT |
| **💾 Serialization** | 1 | Data persistence | JSON/XML circuit export |
| **📏 Units** | 3 | Measurement systems | Voltage, Current, Resistance units |

### 🔄 Calculation Flow
```
Input Parameters → Validation → Core Calculation → Unit Conversion → Result
     ↓               ↓              ↓               ↓            ↓
 User Values → Range Check → Algorithm → Format → Typed Output
```

### 🎯 Design Principles
- **Type Safety**: Strong typing with unit-aware calculations
- **Performance**: Vectorized operations and intelligent caching
- **Extensibility**: Plugin architecture for custom calculators
- **Cross-Platform**: Consistent behavior across all supported frameworks
- **Documentation**: Comprehensive XML docs and examples

For detailed architecture information, see the [Project Code Map](PROJECT_CODE_MAP.md).

## 🎯 Common Use Cases

### Electronics Design
- LED current limiting resistor calculations
- Power supply design and analysis
- Component tolerance and worst-case analysis
- EMC compliance verification

### Embedded Development
- Arduino/ESP32 pin configuration and code generation
- Power consumption optimization
- Communication protocol (I2C/SPI/UART) analysis
- Battery life estimation

### Education & Learning
- Interactive tutorials and examples
- Step-by-step calculation guidance
- Performance benchmarking and testing
- Real-world circuit analysis

### Industrial Applications
- Power factor correction calculations
- Energy consumption monitoring
- Signal integrity analysis for high-speed designs
- Thermal management calculations

## 📦 Package Distribution

### NuGet.org (Primary)
```bash
dotnet add package CircuitTool
```
- **Package URL**: https://www.nuget.org/packages/CircuitTool
- **Latest Version**: 2.0.0
- **Total Downloads**: ![NuGet Downloads](https://img.shields.io/nuget/dt/CircuitTool)

### GitHub Packages (Secondary)
```bash
# Requires GitHub authentication
dotnet add package CircuitTool --source https://nuget.pkg.github.com/jomardyan/index.json
```

## 🤝 Contributing

We welcome contributions! Here's how to get involved:

### Quick Contribution Guide
1. **🍴 Fork** the repository
2. **🌿 Create** a feature branch (`git checkout -b feature/amazing-feature`)
3. **✨ Add** your changes with tests and documentation
4. **📝 Commit** your changes (`git commit -m 'Add amazing feature'`)
5. **🚀 Push** to the branch (`git push origin feature/amazing-feature`)
6. **🔄 Open** a Pull Request

### Development Guidelines
- Follow C# coding standards and conventions
- Add XML documentation for all public methods
- Include unit tests for new functionality
- Update documentation with usage examples
- Ensure cross-platform compatibility

### Areas for Contribution
- **🧮 New Calculators**: Specialized calculation modules
- **🔧 Hardware Support**: Additional microcontroller platforms
- **📊 Analysis Tools**: Advanced simulation capabilities
- **🌐 Internationalization**: Multi-language support
- **📚 Documentation**: Examples, tutorials, and guides

## 📈 Version History

### Version 2.0.0 (Current)
- **🆕 Modern C# Features**: Records, pattern matching, init-only properties
- **🔄 API Redesign**: Consistent naming, better separation of concerns
- **⚙️ Dependency Injection**: Service-based architecture, plugin system
- **🌐 Integration**: Extension methods for UI frameworks, scientific computing
- **📱 Mobile Support**: Xamarin/MAUI optimizations, offline calculations
- **⚡ Performance**: SIMD/vectorized calculations, enhanced caching

### Previous Versions
- **1.x Series**: Core electrical calculations, basic hardware support
- **0.x Series**: Initial proof-of-concept and foundational features

See [GitHub Releases](https://github.com/jomardyan/CircuitTool/releases) for complete changelog.

## 📄 License

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.

### License Summary
- ✅ **Commercial Use**: Use in commercial projects
- ✅ **Modification**: Modify the source code
- ✅ **Distribution**: Distribute the library
- ✅ **Private Use**: Use in private projects
- ❌ **Liability**: No warranty or liability
- ❌ **Trademark**: No trademark rights included

## 🆘 Support & Community

### Get Help
- **📚 Documentation**: Check [DOCUMENTATION.md](DOCUMENTATION.md) and [Getting Started](articles/getting-started.md)
- **🐛 Issues**: Report bugs or request features on [GitHub Issues](https://github.com/jomardyan/CircuitTool/issues)
- **💬 Discussions**: Join community discussions on [GitHub Discussions](https://github.com/jomardyan/CircuitTool/discussions)

### Contact
- **📧 Email**: Create an issue for general inquiries
- **🐦 GitHub**: [@jomardyan](https://github.com/jomardyan)

---

*For more information, visit the [GitHub repository](https://github.com/jomardyan/CircuitTool) or check out the [Getting Started Guide](articles/getting-started.md).*
