# CircuitTool - Project Architecture & Code Map

> **Comprehensive overview of the CircuitTool library structure and organization**

## 📊 Project Overview

**CircuitTool** is a modern, high-performance C# library for electrical engineering and electronics calculations, designed for embedded systems development, hardware design, and educational purposes.

| Property | Value |
|----------|-------|
| **Repository** | [github.com/jomardyan/CircuitTool](https://github.com/jomardyan/CircuitTool) |
| **License** | MIT License |
| **Current Version** | 2.1.0 |
| **Target Frameworks** | .NET 4.5, 4.6.2, 4.8.1, 6.0, 8.0, Core 3.1, Standard 2.0/2.1 |
| **Package Type** | Library + Interactive CLI |
| **Primary Language** | C# 12.0 with nullable reference types |

## 🏗️ Solution Structure

```
CircuitTool/
├── 📄 CircuitTool.sln                   # Main solution file
├── 📄 CircuitTool.CLI.sln               # CLI solution file  
├── � src/                              # 📚 Core library source code
├── � CircuitTool.CLI/                  # 💻 Interactive CLI application
├── � tests/                            # 🧪 Unit tests (344 test cases)
├── 📁 docs/                             # 📖 Documentation website
├── � articles/                         # 📝 Tutorials and guides
├── 📁 .github/                          # 🔄 CI/CD workflows
├── � README.md                         # 📋 Project overview
├── � DOCUMENTATION.md                  # 📚 Complete API docs
├── 📄 PROJECT_CODE_MAP.md               # 🗺️ This file
└── � LICENSE                           # ⚖️ MIT License
```

## 🧩 Core Library Architecture

### 📁 `/src/` - Main Source Directory

The core library is organized into focused modules for maximum usability and maintainability:

```
src/
├── 📄 ACTypes.cs                        # AC circuit type definitions
├── 📄 CircuitBuilder.cs                 # Circuit construction utilities  
├── 📄 CircuitCalculations.cs            # Core calculation methods
├── 📄 Utilities.cs                      # Cross-cutting utility functions
├── 📁 Calculators/                      # 🧮 Calculation modules (23 classes)
├── 📁 Hardware/                         # 🔧 Platform-specific tools (10 classes)
├── 📁 Analysis/                         # 📊 Advanced analysis (5 classes)
├── 📁 Performance/                      # ⚡ Optimization features (6 classes)
├── 📁 Documentation/                    # 📚 Examples & tutorials (4 classes)
├── 📁 Math/                             # 🔢 Mathematical operations (2 classes)
├── 📁 Serialization/                    # 💾 Data persistence (1 class)
├── 📁 Units/                            # 📏 Unit system (3 classes)
├── 📁 Electromagnetics/                 # 📡 RF & EMC tools
├── 📁 PowerElectronics/                 # 🔋 Power system analysis
└── 📁 Utilities/                        # 🔧 Helper functions
```
├── 📁 Math/                           # Mathematical utilities (2 files)
├── 📁 Serialization/                  # Import/export functionality (1 file)
└── 📁 Units/                          # Unit system management (3 files)
```

## 🧮 Calculators Module (23 Classes)

### Core Electrical Calculations
- **`OhmsLawCalculator.cs`** - Fundamental V=IR calculations
- **`PowerCalculator.cs`** - Power calculations (P=VI, P=I²R)
- **`VoltageCalculator.cs`** - Voltage analysis and calculations
- **`ResistorCalculator.cs`** - Resistor networks, parallel/series
- **`CapacitorCalculator.cs`** - Capacitance calculations and analysis
- **`InductorCalculator.cs`** - Inductance calculations and analysis

### Advanced Circuit Analysis
- **`ACCircuitCalculator.cs`** - AC circuit analysis and impedance
- **`FilterCalculator.cs`** - Filter design (low-pass, high-pass, band-pass)
- **`TransformerCalculator.cs`** - Transformer calculations and design
- **`PowerFactorCalculator.cs`** - Power factor correction calculations
- **`AdvancedCalculators.cs`** - Complex circuit analysis

### Specialized Applications
- **`LEDCalculator.cs`** - LED current limiting and power calculations
- **`AntennaCalculator.cs`** - Antenna design and analysis
- **`BeginnerCalculators.cs`** - Simplified calculations for learning
- **`PhysicsCircuitCalculators.cs`** - Physics-based circuit modeling

### System-Level Calculations
- **`EnergyCalculator.cs`** - Energy consumption and efficiency
- **`EnergyConsumptionCalculator.cs`** - Power usage analysis
- **`ElectricityBillCalculator.cs`** - Cost analysis and billing
- **`VoltageDropCalculator.cs`** - Voltage drop in conductors
- **`VoltageDividerCalculator.cs`** - Voltage divider networks
- **`WattsVoltsAmpsOhmsCalculator.cs`** - Multi-parameter calculations

### Utilities
- **`UnitConverter.cs`** - Unit conversion between systems
- **`ComponentCalculator.cs`** - General component calculations

## 🔧 Hardware Module (10 Classes)

### Platform-Specific Tools
- **`ArduinoTools.cs`** - Arduino-specific calculations and utilities
- **`ESP32Tools.cs`** - ESP32 development and configuration tools
- **`RaspberryPiTools.cs`** - Raspberry Pi GPIO and hardware tools

### Communication & Protocols
- **`ComPortTools.cs`** - COM port management and serial communication
- **`CommunicationProtocolTools.cs`** - I2C, SPI, UART protocol analysis
- **`HardwareDebuggingTools.cs`** - Hardware troubleshooting and diagnostics

### Hardware Design
- **`MotorControlCalculator.cs`** - Motor control system design
- **`SensorInterfaceCalculator.cs`** - Sensor interface and calibration
- **`PCBDesignCalculator.cs`** - PCB layout and design calculations
- **`PowerSupplyCalculator.cs`** - Power supply design and analysis

## 📊 Analysis Module (5 Classes)

### Power & Thermal Analysis
- **`AdvancedPowerAnalysis.cs`** - Complex power system analysis
- **`ThermalCalculator.cs`** - Thermal management and heat dissipation

### Signal & EMC Analysis
- **`SignalIntegrityCalculator.cs`** - Signal quality and integrity analysis
- **`EMCCalculator.cs`** - Electromagnetic compatibility calculations
- **`ToleranceAnalysis.cs`** - Component tolerance and variation analysis

## ⚡ Performance Module (6 Classes)

### Optimization & Caching
- **`Performance.cs`** - Performance monitoring and metrics
- **`PerformanceMonitor.cs`** - Real-time performance tracking
- **`CalculationCache.cs`** - Calculation result caching system
- **`VectorizedCalculations.cs`** - SIMD and vectorized operations

### Bulk Operations
- **`BulkOperations.cs`** - Batch processing for large datasets
- **`AsyncCalculations.cs`** - Asynchronous calculation support

## 📚 Documentation Module (4 Classes)

### Educational Resources
- **`DocumentationExamples.cs`** - Code examples and tutorials
- **`InteractiveTutorials.cs`** - Step-by-step learning modules
- **`UseCaseTemplates.cs`** - Common use case templates
- **`Examples.cs`** - Practical application examples

## 🔢 Math Module (2 Classes)

### Advanced Mathematics
- **`MatrixOperations.cs`** - Matrix calculations for circuit analysis
- **`FourierTransform.cs`** - FFT and frequency domain analysis

## 💾 Serialization Module (1 Class)

### Data Management
- **`CircuitSerialization.cs`** - Circuit data import/export functionality

## 📏 Units Module (3 Classes)

### Unit Management
- **Unit system classes** - Comprehensive unit conversion and management

## 🧪 Tests Module (30+ Test Classes)

### Comprehensive Test Coverage
```
tests/
├── Calculator Tests (15+ files)        # Unit tests for all calculators
├── Hardware Tests (5+ files)           # Hardware module tests
├── Advanced Features Tests (5+ files)  # Complex functionality tests
├── Performance Tests (3+ files)        # Performance and optimization tests
└── Documentation Tests (2+ files)      # Example and tutorial tests
```

## 📦 Dependencies & Packages

### NuGet Package References
```xml
<!-- Core Dependencies -->
<PackageReference Include="System.IO.Ports" Version="8.0.0" /> <!-- COM port support -->
<PackageReference Include="System.Text.Json" Version="8.0.5" /> <!-- JSON serialization -->
<PackageReference Include="System.Numerics.Vectors" Version="4.5.0" /> <!-- Vectorization -->
<PackageReference Include="System.ValueTuple" Version="4.5.0" /> <!-- Tuple support -->
<PackageReference Include="Microsoft.Bcl.Async" Version="1.0.168" /> <!-- Async support -->

<!-- Development Tools -->
<PackageReference Include="Microsoft.SourceLink.GitHub" Version="8.0.0" /> <!-- Source linking -->
```

### Framework Compatibility Matrix
| Component | .NET 4.5 | .NET 4.6.2 | .NET 6.0+ | .NET Standard 2.0+ |
|-----------|-----------|-------------|-----------|-------------------|
| Core Calculators | ✅ | ✅ | ✅ | ✅ |
| Hardware Tools | ⚠️ Limited | ✅ | ✅ | ✅ |
| COM Port Tools | ⚠️ Limited | ✅ | ✅ | ✅ |
| Performance Features | ✅ | ✅ | ✅ | ✅ |
| Analysis Tools | ✅ | ✅ | ✅ | ✅ |

## 🔗 Inter-Module Dependencies

### Dependency Flow
```
┌─────────────────┐    ┌─────────────────┐
│   Calculators   │────│      Units      │
│   (23 classes)  │    │   (3 classes)   │
└─────────────────┘    └─────────────────┘
         │                       │
         ▼                       ▼
┌─────────────────┐    ┌─────────────────┐
│    Hardware     │────│      Math       │
│   (10 classes)  │    │   (2 classes)   │
└─────────────────┘    └─────────────────┘
         │                       │
         ▼                       ▼
┌─────────────────┐    ┌─────────────────┐
│    Analysis     │────│   Performance   │
│   (5 classes)   │    │   (6 classes)   │
└─────────────────┘    └─────────────────┘
         │                       │
         ▼                       ▼
┌─────────────────┐    ┌─────────────────┐
│ Documentation   │────│ Serialization   │
│   (4 classes)   │    │   (1 class)     │
└─────────────────┘    └─────────────────┘
```

### Core Shared Components
- **`UnitConverter`** - Used by all calculation modules
- **`Utilities`** - Common helper functions across modules
- **`CircuitCalculations`** - Base calculation methods
- **`Performance`** - Optimization used throughout

## 🚀 Key Features by Module

### 🧮 Calculators
- **Basic**: Ohm's Law, Power, Voltage, Current calculations
- **Components**: Resistors, Capacitors, Inductors, Transformers
- **AC Analysis**: Impedance, Phase, Frequency response
- **Specialized**: LEDs, Antennas, Filters, Power Factor

### 🔧 Hardware
- **Platforms**: Arduino, ESP32, Raspberry Pi support
- **Communication**: I2C, SPI, UART, COM port tools
- **Design**: PCB layout, Power supplies, Motor control
- **Debug**: Hardware troubleshooting, Diagnostics

### 📊 Analysis
- **Power**: Advanced power analysis and optimization
- **Signal**: Signal integrity and EMC compliance
- **Thermal**: Heat management and thermal analysis
- **Tolerance**: Component variation analysis

### ⚡ Performance
- **Optimization**: Vectorized calculations, Caching
- **Monitoring**: Real-time performance tracking
- **Async**: Asynchronous calculation support
- **Bulk**: Large dataset processing

## 🎯 Usage Patterns

### Typical Usage Flow
1. **Import** CircuitTool namespace
2. **Configure** units and parameters
3. **Calculate** using appropriate module
4. **Analyze** results with analysis tools
5. **Generate** code or documentation
6. **Export** results via serialization

### Example Integration Points
```csharp
using CircuitTool;

// Basic calculation
var voltage = OhmsLawCalculator.CalculateVoltage(current: 2.0, resistance: 100);

// Hardware-specific
var gpioCode = ArduinoTools.GenerateGPIOCode(pin: 13, mode: "OUTPUT");

// Protocol analysis
var i2cSettings = CommunicationProtocolTools.CalculateI2CPullUpResistors(config);

// Performance monitoring
using (var monitor = new PerformanceMonitor())
{
    // Calculations with monitoring
}
```

## 📈 Project Evolution

### Recent Enhancements
- ✅ **Code Reorganization** - Modular structure with logical grouping
- ✅ **Hardware Tools** - Comprehensive embedded systems support
- ✅ **COM Port Tools** - Complete serial communication framework
- ✅ **Protocol Analysis** - I2C, SPI, UART optimization tools
- ✅ **Debug Tools** - Hardware troubleshooting and diagnostics

### Technical Achievements
- **54 Classes** across 8 logical modules
- **30+ Test Classes** with comprehensive coverage
- **Cross-Framework** compatibility (.NET 4.5 - 8.0)
- **Performance Optimized** with vectorization and caching
- **Industry Standards** compliance (IPC, IEEE)

This code map provides a complete overview of the CircuitTool project architecture, showing how all components work together to provide a comprehensive electrical engineering calculation library.
