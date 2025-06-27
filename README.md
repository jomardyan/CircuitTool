[![Build and Publish NuGet Package](https://github.com/jomardyan/CircuitTool/actions/workflows/publish-nuget.yml/badge.svg)](https://github.com/jomardyan/CircuitTool/actions/workflows/publish-nuget.yml)
[![.NET Build and Test](https://github.com/jomardyan/CircuitTool/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/jomardyan/CircuitTool/actions/workflows/dotnet.yml)
[![NuGet Version](https://img.shields.io/nuget/v/CircuitTool?style=flat-square&logo=nuget&color=blue)](https://www.nuget.org/packages/CircuitTool)
[![NuGet Downloads](https://img.shields.io/nuget/dt/CircuitTool?style=flat-square&logo=nuget&color=brightgreen)](https://www.nuget.org/packages/CircuitTool)

[![GitHub Release](https://img.shields.io/github/v/release/jomardyan/CircuitTool?style=flat-square&logo=github&color=success)](https://github.com/jomardyan/CircuitTool/releases)
[![GitHub Stars](https://img.shields.io/github/stars/jomardyan/CircuitTool?style=flat-square&logo=github&color=yellow)](https://github.com/jomardyan/CircuitTool/stargazers)
[![GitHub Forks](https://img.shields.io/github/forks/jomardyan/CircuitTool?style=flat-square&logo=github&color=lightgrey)](https://github.com/jomardyan/CircuitTool/network/members)
[![GitHub Issues](https://img.shields.io/github/issues/jomardyan/CircuitTool?style=flat-square&logo=github&color=red)](https://github.com/jomardyan/CircuitTool/issues)

[![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.5%2B-blue?style=flat-square&logo=.net)](https://dotnet.microsoft.com/)
[![.NET](https://img.shields.io/badge/.NET-6.0%2B%20%7C%208.0%2B-purple?style=flat-square&logo=.net)](https://dotnet.microsoft.com/)
[![.NET Standard](https://img.shields.io/badge/.NET%20Standard-2.0%20%7C%202.1-orange?style=flat-square&logo=.net)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg?style=flat-square)](https://opensource.org/licenses/MIT)

# 🔌 CircuitTool

**A comprehensive C# library for electrical engineering and electronics calculations.** CircuitTool provides utilities for circuit analysis, power calculations, unit conversions, microcontroller development, and much more.

## ⚡ Key Features

- **🧮 Core Calculations**: Ohm's Law, power analysis, resistor networks, voltage dividers
- **🔧 Component Design**: LEDs, capacitors, inductors, transformers, filters
- **📡 AC Analysis**: Impedance, reactance, resonance, power factor
- **🤖 Hardware Support**: Arduino, ESP32, Raspberry Pi with code generation
- **📊 Advanced Analysis**: Signal integrity, EMC, thermal, tolerance analysis
- **⚙️ Communication**: COM port tools, I2C/SPI/UART protocol analysis
- **📈 Performance**: Vectorized calculations, caching, async operations
- **🎯 Interactive CLI**: Command-line interface for testing and learning

## 🚀 Quick Start

### Installation
```bash
dotnet add package CircuitTool
```

### Basic Usage
```csharp
using CircuitTool;

// Ohm's Law calculations
double voltage = OhmsLawCalculator.Voltage(current: 2.0, resistance: 100); // 200V
double power = PowerCalculator.Power(voltage: 12.0, current: 2.0); // 24W

// LED current limiting resistor
double resistor = LEDCalculator.CalculateResistorValue(
    supplyVoltage: 5.0,
    ledVoltage: 2.1,
    ledCurrent: 0.02  // 20mA
); // 145Ω

// Arduino development
double analogVoltage = ArduinoTools.AnalogToVoltage(512); // 2.5V (on 5V Arduino)
string gpioCode = ArduinoTools.GenerateGPIOCode(pin: 13, mode: "OUTPUT");
```

## 📚 Documentation

| Resource | Description |
|----------|-------------|
| [📖 **Full Documentation**](DOCUMENTATION.md) | Complete API reference with examples |
| [🚀 **Getting Started Guide**](articles/getting-started.md) | Step-by-step tutorials |
| [💻 **Interactive CLI**](CircuitTool.CLI/README.md) | Command-line interface documentation |
| [🗺️ **Project Code Map**](PROJECT_CODE_MAP.md) | Architecture and module overview |

## 💻 Interactive CLI

Test and explore CircuitTool capabilities with the interactive command-line interface:

```bash
# Run interactively
CircuitTool.CLI

# Command-line calculations
CircuitTool.CLI basic ohms --voltage 12 --current 2
CircuitTool.CLI component led --supply 5 --forward 2.1 --current 0.02
CircuitTool.CLI benchmark --iterations 10000
```

See [CLI Documentation](CircuitTool.CLI/README.md) for complete usage guide.

## 🏗️ Architecture & Modules

CircuitTool is organized into logical modules for easy navigation:

| Module | Classes | Description |
|--------|---------|-------------|
| **🧮 Calculators** | 23 | Core electrical calculations and component design |
| **🔧 Hardware** | 10 | Platform-specific tools (Arduino, ESP32, RPi) |
| **📊 Analysis** | 5 | Advanced power, signal, and thermal analysis |
| **⚡ Performance** | 6 | Optimization, caching, and vectorized operations |
| **📚 Documentation** | 4 | Examples, tutorials, and interactive guides |
| **🔢 Math** | 2 | Matrix operations and Fourier transforms |
| **💾 Serialization** | 1 | Circuit data import/export functionality |
| **📏 Units** | 3 | Comprehensive unit conversion system |

See the [Project Code Map](PROJECT_CODE_MAP.md) for detailed architecture overview.

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
