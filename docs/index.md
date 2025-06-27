# 🔌 CircuitTool Documentation

[![Build Status](https://github.com/jomardyan/CircuitTool/actions/workflows/dotnet.yml/badge.svg)](https://github.com/jomardyan/CircuitTool/actions/workflows/dotnet.yml)
[![NuGet Version](https://img.shields.io/nuget/v/CircuitTool)](https://www.nuget.org/packages/CircuitTool)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)

> **Comprehensive C# library for electrical engineering and electronics calculations**

Welcome to the complete documentation for CircuitTool - a modern, high-performance library designed for engineers, students, and developers working with electronic systems.

## 🚀 Quick Start

### Installation
```bash
dotnet add package CircuitTool
```

### Basic Example
```csharp
using CircuitTool;

// Calculate LED resistor value
double resistor = LEDCalculator.CalculateResistorValue(
    supplyVoltage: 5.0,
    ledVoltage: 2.1,
    ledCurrent: 0.02
); // Returns: 145Ω

// Ohm's Law calculation
double power = PowerCalculator.Power(voltage: 12, current: 2); // 24W
```

## 📚 Documentation Sections

| Section | Description | Best For |
|---------|-------------|----------|
| 🚀 **[Getting Started](articles/getting-started.md)** | Installation, basic usage, first steps | New users |
| 📖 **[API Reference](docs/api/)** | Complete API documentation | All users |
| 🎓 **[Tutorials](docs/tutorials/)** | Step-by-step learning materials | Learning |
| 💡 **[Examples](docs/examples/)** | Real-world usage scenarios | Practical applications |
| 🔧 **[Hardware Guides](docs/technology-guides/)** | Platform-specific integration | Hardware developers |
| 💻 **[CLI Documentation](CircuitTool.CLI/README.md)** | Interactive command-line interface | Testing & automation |

## ✨ Key Features

### 🧮 Core Electrical Calculations
- **Ohm's Law & Power**: All basic electrical relationships
- **Component Analysis**: Resistors, capacitors, inductors, transformers  
- **Circuit Networks**: Series/parallel combinations, voltage dividers
- **Unit Conversions**: Comprehensive electrical units system

### 🔧 Component Design & Analysis
- **LED Design**: Current limiting resistors, brightness calculations
- **Filter Design**: RC/RL filters, frequency response analysis
- **Power Systems**: Efficiency, regulation, thermal analysis
- **Tolerance Analysis**: Worst-case and statistical methods

### 📡 AC Circuit Analysis
- **Impedance & Reactance**: Complex impedance calculations
- **Frequency Response**: Magnitude and phase analysis
- **Resonance**: RLC circuit analysis and optimization
- **Power Factor**: Real, reactive, and apparent power

### 🤖 Hardware Platform Support
- **Arduino**: Pin configuration, ADC/PWM, code generation
- **ESP32**: Power optimization, WiFi calculations
- **Raspberry Pi**: GPIO control, hardware interfaces
- **Universal MCU**: Cross-platform microcontroller tools

### 📊 Advanced Engineering Analysis
- **Signal Integrity**: Transmission lines, crosstalk analysis
- **EMC Compliance**: Electric field calculations, shielding
- **Thermal Management**: Heat transfer, junction temperature
- **Performance**: Vectorized calculations, intelligent caching

## 🎯 Popular Use Cases

### 🔋 Electronics Design
```csharp
// Power supply design with thermal analysis
var regulator = PowerCalculator.DesignLinearRegulator(
    inputVoltage: 12.0,
    outputVoltage: 5.0,
    loadCurrent: 1.0
);

// Component tolerance analysis  
var analysis = ToleranceCalculator.WorstCaseAnalysis(
    nominalValues: new[] { 100, 200, 300 },
    tolerances: new[] { 0.05, 0.01, 0.02 }
);
```

### 🤖 Embedded Development
```csharp
// ESP32 battery life optimization
var batteryLife = ESP32Tools.CalculateBatteryLife(
    batteryCapacity: 2500,  // mAh
    activeCurrentMA: 160,
    sleepCurrentUA: 10,
    dutyCycle: 0.01
);

// Arduino sensor interface design
var interface = ArduinoTools.DesignSensorInterface(
    sensorType: "NTC",
    targetResolution: 0.1  // 0.1°C
);
```

### 📡 Signal & Communication Analysis
```csharp
// Signal integrity analysis
var crosstalk = SignalIntegrityCalculator.CrosstalkCoupling(
    aggressorVoltage: 3.3,
    couplingCapacitance: 1e-12,
    frequency: 100e6
);

// UART communication timing
var timing = UARTCommunicationCalculator.CalculateTiming(
    baudRate: 115200,
    dataBits: 8,
    stopBits: 1
);
```

## 🛠️ Platform Support

| Platform | Versions | Status |
|----------|----------|---------|
| **.NET Framework** | 4.5, 4.6.2, 4.8.1+ | ✅ Fully Supported |
| **.NET Core** | 3.1+ | ✅ Fully Supported |
| **.NET** | 6.0+, 8.0+ | ✅ Fully Supported |
| **.NET Standard** | 2.0, 2.1 | ✅ Fully Supported |

## 💻 Interactive CLI

Test and explore CircuitTool with the interactive command-line interface:

```bash
# Interactive mode with guided menus
CircuitTool.CLI

# Direct calculations
CircuitTool.CLI basic ohms --voltage 12 --current 2
CircuitTool.CLI component led --supply 5 --forward 2.1 --current 0.02
CircuitTool.CLI analysis power --input 100 --output 85
```

## 🎓 Learning Path

### 1. **Start Here** (New to CircuitTool)
   - [Installation Guide](articles/getting-started.md#installation--setup)
   - [Basic Calculations](articles/getting-started.md#fundamental-calculations)
   - [Your First Project](articles/getting-started.md#practical-projects)

### 2. **Core Concepts** (Building Skills)
   - [Ohm's Law & Power](docs/tutorials/fundamentals.md)
   - [Component Design](docs/tutorials/components.md)
   - [Circuit Analysis](docs/tutorials/circuits.md)

### 3. **Advanced Topics** (Specialized Applications)
   - [AC Circuit Analysis](docs/tutorials/ac-analysis.md)
   - [Signal Integrity](docs/tutorials/signal-integrity.md)
   - [Performance Optimization](docs/tutorials/performance.md)

### 4. **Hardware Integration** (Real-World Projects)
   - [Arduino Development](docs/tutorials/arduino-guide.md)
   - [ESP32 Optimization](docs/tutorials/esp32-guide.md)
   - [Communication Protocols](docs/tutorials/communications.md)

## 🔗 Quick Links

- **📦 NuGet Package**: [CircuitTool on NuGet.org](https://www.nuget.org/packages/CircuitTool)
- **🐙 Source Code**: [GitHub Repository](https://github.com/jomardyan/CircuitTool)
- **🐛 Issues & Support**: [GitHub Issues](https://github.com/jomardyan/CircuitTool/issues)
- **💬 Community**: [GitHub Discussions](https://github.com/jomardyan/CircuitTool/discussions)
- **📄 License**: [MIT License](LICENSE)

---

**Ready to get started?** Begin with the [Getting Started Guide](articles/getting-started.md) or explore the [Interactive CLI](CircuitTool.CLI/README.md)!
