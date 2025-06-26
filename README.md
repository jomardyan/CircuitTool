<div align="center">

[![Build and Publish NuGet Package](https://github.com/jomardyan/CircuitTool/actions/workflows/publish-nuget.yml/badge.svg)](https://github.com/jomardyan/CircuitTool/actions/workflows/publish-nuget.yml)
[![.NET Build and Test](https://github.com/jomardyan/CircuitTool/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/jomardyan/CircuitTool/actions/workflows/dotnet.yml)
[![NuGet Version](https://img.shields.io/nuget/v/CircuitTool?style=flat-square&logo=nuget&color=blue)](https://www.nuget.org/packages/CircuitTool)
[![NuGet Downloads](https://img.shields.io/nuget/dt/CircuitTool?style=flat-square&logo=nuget&color=brightgreen)](https://www.nuget.org/packages/CircuitTool)

[![GitHub Release](https://img.shields.io/github/v/release/jomardyan/CircuitTool?style=flat-square&logo=github&color=success)](https://github.com/jomardyan/CircuitTool/releases)
[![GitHub Stars](https://img.shields.io/github/stars/jomardyan/CircuitTool?style=flat-square&logo=github&color=yellow)](https://github.com/jomardyan/CircuitTool/stargazers)
[![GitHub Forks](https://img.shields.io/github/forks/jomardyan/CircuitTool?style=flat-square&logo=github&color=lightgrey)](https://github.com/jomardyan/CircuitTool/network/members)
[![GitHub Issues](https://img.shields.io/github/issues/jomardyan/CircuitTool?style=flat-square&logo=github&color=red)](https://github.com/jomardyan/CircuitTool/issues)

[![.NET Framework](https://img.shields.io/badge/.NET%20Framework-2.0%2B-blue?style=flat-square&logo=.net)](https://dotnet.microsoft.com/)
[![.NET](https://img.shields.io/badge/.NET-6.0%2B%20%7C%208.0%2B-purple?style=flat-square&logo=.net)](https://dotnet.microsoft.com/)
[![.NET Standard](https://img.shields.io/badge/.NET%20Standard-2.0%20%7C%202.1-orange?style=flat-square&logo=.net)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg?style=flat-square)](https://opensource.org/licenses/MIT)

[![Language](https://img.shields.io/github/languages/top/jomardyan/CircuitTool?style=flat-square&logo=csharp&color=purple)](https://github.com/jomardyan/CircuitTool)
[![Code Size](https://img.shields.io/github/languages/code-size/jomardyan/CircuitTool?style=flat-square&logo=github)](https://github.com/jomardyan/CircuitTool)
[![Repo Size](https://img.shields.io/github/repo-size/jomardyan/CircuitTool?style=flat-square&logo=github)](https://github.com/jomardyan/CircuitTool)
[![Last Commit](https://img.shields.io/github/last-commit/jomardyan/CircuitTool?style=flat-square&logo=github)](https://github.com/jomardyan/CircuitTool/commits/main)

</div>

# 🔌 CircuitTool

A comprehensive C# library for electrical engineering and electronics calculations. CircuitTool simplifies circuit analysis, power calculations, and unit conversions for electrical engineers, electronics enthusiasts, and makers working with Arduino, ESP32, and other microcontrollers.

## Features

### Core Electrical Calculations
- **Ohm's Law Calculations**: Calculate voltage, current, and resistance
- **Resistor Network Analysis**: Series and parallel resistor calculations
- **Voltage Analysis**: Voltage drop and voltage divider calculations
- **Power Calculations**: Various methods to calculate electrical power
- **Energy Calculations**: Convert between energy units, calculate energy costs
- **Unit Conversions**: Comprehensive electrical unit conversion utilities
- **Specialized Calculators**: Power factor, electricity bill, and energy consumption calculators

### Advanced Circuit Analysis
- **Capacitor Calculations**: Capacitance, reactance, energy storage, time constants, series/parallel combinations
- **Inductor Calculations**: Inductive reactance, energy storage, time constants, current buildup/decay, resonance
- **Transformer Analysis**: Voltage/current/turns ratios, efficiency, power loss, regulation calculations
- **AC Circuit Analysis**: Impedance, phase angles, power factor, RMS/peak/average values, form/crest factors
- **Filter Calculations**: RC/RL low-pass/high-pass filters, cutoff frequencies, gain, phase response

### LED and Lighting Calculations
- **LED Resistor Calculator**: Calculate required resistor values for LED circuits
- **LED Power Consumption**: Calculate power usage for LED projects
- **LED Brightness Control**: PWM duty cycle to brightness ratio calculations
- **Series LED Calculations**: Multi-LED circuit calculations

### Microcontroller Tools
- **Arduino Tools**: ADC conversions, servo control, current consumption calculations
- **ESP32 Tools**: ADC conversions, WiFi power management, battery life estimation
- **Touch Sensor Calculations**: ESP32 touch threshold calculations

### Beginner-Friendly Calculators
- **Battery Life Calculator**: Estimate battery runtime for projects
- **Wire Gauge Calculator**: Determine appropriate wire gauge for current loads
- **RC Circuit Tools**: Time constants, oscillator frequencies
- **Inductor Design**: Basic inductor turn calculations
- **Decibel Conversions**: Power and voltage ratio to dB conversions
- **Transformer Calculations**: Turns ratio calculations

## Getting Started

### Prerequisites

- .NET Framework 2.0 or later
- .NET 6.0 or later
- .NET Standard 2.0 or later

### Framework Compatibility

CircuitTool supports a wide range of .NET frameworks for maximum compatibility:

- **.NET Framework**: 2.0, 3.5, 4.0, 4.5, 4.6.2
- **.NET Core/.NET**: 6.0, 8.0
- **.NET Standard**: 2.0, 2.1

This ensures the library works with both legacy and modern .NET applications.

### Installation via NuGet.org

To install CircuitTool from NuGet.org, use the following command:
```bash
dotnet add package CircuitTool
```

Or using the Package Manager Console:
```bash
Install-Package CircuitTool
```

### Installation via GitHub Packages

To install CircuitTool from GitHub Packages:

1. **Add GitHub Packages as a package source** (one-time setup):
```bash
dotnet nuget add source --username YOUR_GITHUB_USERNAME --password YOUR_GITHUB_TOKEN --store-password-in-clear-text --name github "https://nuget.pkg.github.com/jomardyan/index.json"
```

2. **Install the package**:
```bash
dotnet add package CircuitTool --source github
```

**Note**: You'll need a GitHub Personal Access Token with `read:packages` permission.

## Usage Examples

### Ohm's Law Calculations

```csharp
using CircuitTool;

// Calculate voltage using Ohm's Law (V = I × R)
double voltage = OhmsLawCalculator.Voltage(2.0, 5.0);  // 2A through 5Ω = 10V

// Calculate current using Ohm's Law (I = V / R)
double current = OhmsLawCalculator.Current(10.0, 5.0);  // 10V across 5Ω = 2A

// Calculate resistance using Ohm's Law (R = V / I)
double resistance = OhmsLawCalculator.Resistance(10.0, 2.0);  // 10V, 2A = 5Ω
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

### Power Analysis

```csharp
using CircuitTool;

// Calculate watts from voltage and current
double watts = WattsVoltsAmpsOhmsCalculator.CalculateWatts(12, 2); // 12V, 2A = 24W

// Calculate volts from watts and current
double volts = WattsVoltsAmpsOhmsCalculator.CalculateVolts(24, 2); // 24W, 2A = 12V

// Calculate amps from watts and voltage
double amps = WattsVoltsAmpsOhmsCalculator.CalculateAmps(24, 12); // 24W, 12V = 2A

// Calculate ohms from voltage and current
double ohms = WattsVoltsAmpsOhmsCalculator.CalculateOhms(12, 2); // 12V, 2A = 6Ω

// Calculate watts from voltage and resistance
double wattsFromVR = WattsVoltsAmpsOhmsCalculator.CalculateWattsFromVoltageAndResistance(12, 6); // 12V, 6Ω = 24W

// Calculate watts from current and resistance
double wattsFromIR = WattsVoltsAmpsOhmsCalculator.CalculateWattsFromCurrentAndResistance(2, 6); // 2A, 6Ω = 24W
```

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

### Advanced Circuit Analysis

```csharp
using CircuitTool;

// Calculate total resistance in series
double seriesResistance = CircuitCalculations.CalculateTotalResistance(new double[] { 10, 20, 30 }, true);

// Calculate total resistance in parallel
double parallelResistance = CircuitCalculations.CalculateTotalResistance(new double[] { 10, 20, 30 }, false);

// Calculate power
double power = CircuitCalculations.CalculatePower(230, 5); // 230V × 5A = 1150W

// Calculate energy
double energy = CircuitCalculations.CalculateEnergy(1150, 2); // 1150W × 2h = 2300Wh
```

### Capacitor Calculations

```csharp
using CircuitTool;

// Calculate capacitive reactance
double reactance = CapacitorCalculator.CalculateCapacitiveReactance(0.000001, 1000); // 1μF at 1kHz = 159.15Ω

// Calculate energy stored in capacitor
double energy = CapacitorCalculator.CalculateEnergyStored(0.000001, 12); // 1μF at 12V = 72μJ

// Calculate RC time constant
double timeConstant = CapacitorCalculator.CalculateTimeConstant(1000, 0.000001); // 1kΩ, 1μF = 1ms

// Calculate total capacitance in series
double seriesCapacitance = CapacitorCalculator.CalculateSeriesCapacitance(new double[] { 0.000001, 0.000002 }); // 1μF, 2μF in series = 0.67μF

// Calculate total capacitance in parallel
double parallelCapacitance = CapacitorCalculator.CalculateParallelCapacitance(new double[] { 0.000001, 0.000002 }); // 1μF, 2μF in parallel = 3μF

// Calculate charging voltage
double voltage = CapacitorCalculator.CalculateChargingVoltage(12, 0.001, 0.001); // 12V source, t=1ms, τ=1ms = 7.59V

// Calculate discharging voltage
double dischargingVoltage = CapacitorCalculator.CalculateDischargingVoltage(12, 0.001, 0.001); // 12V initial, t=1ms, τ=1ms = 4.41V
```

### Inductor Calculations

```csharp
using CircuitTool;

// Calculate inductive reactance
double reactance = InductorCalculator.CalculateInductiveReactance(0.001, 1000); // 1mH at 1kHz = 6.28Ω

// Calculate energy stored in inductor
double energy = InductorCalculator.CalculateEnergyStored(0.001, 2); // 1mH with 2A = 2mJ

// Calculate RL time constant
double timeConstant = InductorCalculator.CalculateTimeConstant(0.001, 100); // 1mH, 100Ω = 10μs

// Calculate total inductance in series
double seriesInductance = InductorCalculator.CalculateSeriesInductance(new double[] { 0.001, 0.002 }); // 1mH, 2mH in series = 3mH

// Calculate total inductance in parallel
double parallelInductance = InductorCalculator.CalculateParallelInductance(new double[] { 0.001, 0.002 }); // 1mH, 2mH in parallel = 0.67mH

// Calculate current buildup
double current = InductorCalculator.CalculateCurrentBuildup(2, 0.001, 0.00001); // 2A final, t=10μs, τ=10μs = 1.26A

// Calculate resonant frequency
double frequency = InductorCalculator.CalculateResonantFrequency(0.001, 0.000001); // 1mH, 1μF = 5.03kHz
```

### Transformer Calculations

```csharp
using CircuitTool;

// Calculate secondary voltage
double secondaryVoltage = TransformerCalculator.CalculateSecondaryVoltage(120, 10, 1); // 120V primary, 10:1 ratio = 12V

// Calculate secondary current
double secondaryCurrent = TransformerCalculator.CalculateSecondaryCurrent(2, 10, 1); // 2A primary, 10:1 ratio = 20A

// Calculate turns ratio
double turnsRatio = TransformerCalculator.CalculateTurnsRatio(120, 12); // 120V to 12V = 10:1

// Calculate voltage ratio
double voltageRatio = TransformerCalculator.CalculateVoltageRatio(240, 120); // 240V to 120V = 2:1

// Calculate efficiency
double efficiency = TransformerCalculator.CalculateEfficiency(1000, 1100); // 1000W out, 1100W in = 90.9%

// Calculate power loss
double powerLoss = TransformerCalculator.CalculatePowerLoss(1100, 1000); // 1100W in, 1000W out = 100W loss

// Calculate voltage regulation
double regulation = TransformerCalculator.CalculateVoltageRegulation(120, 115); // 120V no-load, 115V load = 4.35%

// Calculate apparent power
double apparentPower = TransformerCalculator.CalculateApparentPower(120, 10); // 120V, 10A = 1200VA
```

### AC Circuit Analysis

```csharp
using CircuitTool;

// Calculate impedance
double impedance = ACCircuitCalculator.CalculateImpedance(50, 30); // 50Ω resistance, 30Ω reactance = 58.31Ω

// Calculate phase angle
double phaseAngle = ACCircuitCalculator.CalculatePhaseAngle(50, 30); // 50Ω R, 30Ω X = 30.96°

// Calculate power factor
double powerFactor = ACCircuitCalculator.CalculatePowerFactor(30.96); // 30.96° phase angle = 0.857

// Calculate RMS from peak
double rms = ACCircuitCalculator.CalculateRMS(170); // 170V peak = 120.21V RMS

// Calculate peak from RMS
double peak = ACCircuitCalculator.CalculatePeak(120); // 120V RMS = 169.74V peak

// Calculate average from peak
double average = ACCircuitCalculator.CalculateAverage(170); // 170V peak = 108.13V average

// Calculate form factor
double formFactor = ACCircuitCalculator.CalculateFormFactor(120, 108); // 120V RMS, 108V avg = 1.11

// Calculate crest factor
double crestFactor = ACCircuitCalculator.CalculateCrestFactor(170, 120); // 170V peak, 120V RMS = 1.42

// Calculate Q factor
double qFactor = ACCircuitCalculator.CalculateQFactor(30, 5); // 30Ω reactance, 5Ω resistance = 6

// Calculate bandwidth
double bandwidth = ACCircuitCalculator.CalculateBandwidth(1000, 6); // 1kHz resonant, Q=6 = 166.67Hz
```

### Filter Calculations

```csharp
using CircuitTool;

// Calculate RC low-pass filter cutoff frequency
double cutoffFreq = FilterCalculator.CalculateRCLowPassCutoff(1000, 0.000001); // 1kΩ, 1μF = 159.15Hz

// Calculate RC high-pass filter cutoff frequency
double highPassCutoff = FilterCalculator.CalculateRCHighPassCutoff(1000, 0.000001); // 1kΩ, 1μF = 159.15Hz

// Calculate RL low-pass filter cutoff frequency
double rlCutoff = FilterCalculator.CalculateRLLowPassCutoff(100, 0.001); // 100Ω, 1mH = 15.92kHz

// Calculate RL high-pass filter cutoff frequency
double rlHighPass = FilterCalculator.CalculateRLHighPassCutoff(100, 0.001); // 100Ω, 1mH = 15.92kHz

// Calculate filter gain in dB
double gain = FilterCalculator.CalculateFilterGain(1000, 159.15); // 1kHz signal, 159.15Hz cutoff = -15.97dB

// Calculate filter phase shift
double phaseShift = FilterCalculator.CalculateFilterPhaseShift(1000, 159.15); // 1kHz signal, 159.15Hz cutoff = -80.96°

// Calculate magnitude response
double magnitude = FilterCalculator.CalculateMagnitudeResponse(1000, 159.15); // 1kHz signal, 159.15Hz cutoff = 0.158

// Calculate required resistor for RC filter
double resistor = FilterCalculator.CalculateRequiredResistor(159.15, 0.000001); // 159.15Hz cutoff, 1μF = 1kΩ

// Calculate required capacitor for RC filter
double capacitor = FilterCalculator.CalculateRequiredCapacitor(159.15, 1000); // 159.15Hz cutoff, 1kΩ = 1μF
```

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

## Power and Energy Calculations

```csharp
using CircuitTool;

// Power factor calculations
double apparentPower = PowerFactorCalculator.ApparentPower(1000, 0.8);  // 1000W real, 0.8 PF = 1250VA
double reactivePower = PowerFactorCalculator.ReactivePower(1000, 0.8);  // 1000W real, 0.8 PF = 750VAR

// Energy consumption calculations
double monthlyCost = EnergyConsumptionCalculator.MonthlyCost(5000, 0.12);  // 5kWh, $0.12/kWh = $600/month
double carbonFootprint = EnergyConsumptionCalculator.CarbonFootprint(1000, 0.5);  // 1kWh, 0.5kg/kWh = 0.5kg CO2
```

## Unit Conversions

```csharp
using CircuitTool;

// Convert between electrical units
double milliamps = UnitConverter.AmperesToMilliamps(0.5);  // 0.5A = 500mA
double kilovolts = UnitConverter.VoltsToKilovolts(5000);  // 5000V = 5kV
double megaohms = UnitConverter.OhmsToMegaohms(2000000);  // 2MΩ = 2000000Ω
```

## Package Distribution

CircuitTool is available on multiple package registries:

### NuGet.org
- **Package URL**: https://www.nuget.org/packages/CircuitTool
- **Installation**: `dotnet add package CircuitTool`

### GitHub Packages
- **Package URL**: https://github.com/jomardyan/CircuitTool/packages
- **Registry URL**: https://nuget.pkg.github.com/jomardyan/index.json
- **Installation**: Requires GitHub authentication (see installation section above)

## Contributing

We welcome contributions! Please feel free to submit pull requests or open issues for bugs and feature requests.

### Development Guidelines

1. Follow C# coding standards and conventions
2. Add XML documentation for all public methods
3. Include unit tests for new functionality
4. Update the README.md with usage examples

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Changelog

### Version 1.0.13 (Current)
- **Enhanced Build Configuration**: Improved deterministic build settings for better CI/CD integration
- **Continuous Integration Support**: Added CI-specific build optimizations with environment detection
- **Source Embedding Enhancements**: Better source code embedding for debugging and troubleshooting
- **PathMap Optimization**: Updated source path mapping for consistent debugging experience
- **Build Reproducibility**: Enhanced deterministic builds across all supported target frameworks
- **Version Management**: Synchronized assembly and package versions for consistency

### Version 1.0.12
- **New Advanced Calculators**: Added comprehensive physics calculators for electrical engineering
  - **CapacitorCalculator**: Capacitive reactance, energy storage, time constants, series/parallel combinations, charging/discharging curves
  - **InductorCalculator**: Inductive reactance, energy storage, time constants, current buildup/decay, resonance frequency
  - **TransformerCalculator**: Voltage/current/turns ratios, efficiency, power loss, regulation, apparent power calculations
  - **ACCircuitCalculator**: Impedance, phase angles, power factor, RMS/peak/average conversions, form/crest factors, Q factor, bandwidth
  - **FilterCalculator**: RC/RL low-pass/high-pass filters, cutoff frequencies, gain, phase response, magnitude response
- **Test Framework Standardization**: Migrated all unit tests from MSTest to NUnit.Framework for consistency
- **Complete Test Coverage**: Added comprehensive unit tests for all previously untested calculators and new calculators
- **Bug Fixes**: Fixed logic issues in test expectations and duplicate variable declarations
- **Enhanced Documentation**: Updated README.md with detailed usage examples for all new calculators

### Version 1.0.11
- **Documentation Improvements**: Enhanced README.md structure for better end-user experience
- **Workflow Organization**: Moved development and publishing documentation to WORKFLOWS.md
- **Package Quality**: Continued improvements to NuGet package structure and metadata
- **User Experience**: Simplified installation instructions and focused documentation

### Version 1.0.10
- **Documentation Restructure**: Separated end-user documentation from developer/maintainer documentation
- **Changelog Enhancement**: Added comprehensive version history with detailed feature descriptions
- **Package Distribution**: Improved package distribution documentation and user guidance

### Version 1.0.9
- **Enhanced Documentation**: Added comprehensive XML documentation for all public methods and classes
- **Debug Symbols**: Included portable debug symbols (.pdb files) for better debugging experience
- **Package Icon**: Added custom circuit-themed package icon for better package identification
- **Automated Publishing**: Implemented GitHub Actions for automatic publishing to NuGet.org and GitHub Packages
- **Multi-Framework Compatibility**: Maintained support for all target frameworks with proper debug symbol generation
- **Package Quality**: Improved NuGet package quality with proper metadata, symbols, and documentation

### Version 1.0.8
- **GitHub Actions Integration**: Added automated build, test, and publishing workflows
- **Package Validation**: Implemented automated package validation and quality checks
- **Cross-Platform Testing**: Added testing on Ubuntu, Windows, and macOS environments
- **Environment Protection**: Added production environment protection for secure publishing

### Version 1.0.7
- **Source Linking**: Enhanced source linking for better debugging experience
- **Symbol Packages**: Added symbol packages (.snupkg) for NuGet.org publishing
- **Repository Integration**: Improved repository URL configuration and source embedding

### Version 1.0.6
- **Package Metadata**: Enhanced NuGet package metadata with proper tags and descriptions
- **License Integration**: Added MIT license file to NuGet package
- **README Integration**: Included README.md in NuGet package for better documentation

### Version 1.0.5
- **Multi-Framework Support**: Added compatibility with .NET Framework 2.0, 3.5, 4.0, 4.5, 4.6.2, .NET 6.0, 8.0, and .NET Standard 2.0, 2.1
- **Legacy Compatibility**: Refactored code to remove C# 8.0+ features for broader compatibility
- **Conditional Compilation**: Added framework-specific compilation directives

### Version 1.0.4
- Added LED circuit calculators
- Added Arduino and ESP32 tools
- Added beginner-friendly calculators
- Improved NuGet package metadata
- Added comprehensive unit tests
- Enhanced documentation

### Version 1.0.3
- Improved source linking and symbols
- Better NuGet package structure

### Version 1.0.2
- Added advanced circuit analysis methods
- Improved README documentation

### Version 1.0.1
- Initial NuGet package release
- Core electrical calculations

## Support

For questions, issues, or feature requests, please visit the [GitHub repository](https://github.com/jomardyan/CircuitTool) or open an issue.
