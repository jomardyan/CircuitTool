double power = LEDCalculator.CalculateLEDPower(12.0, 0.02);
double batteryLife = BeginnerCalculators.BatteryLifeCalculator(2000, arduinoCurrent);
double batteryHours = ESP32Tools.CalculateBatteryLife(3000, totalCurrent);
double seriesR = CircuitCalculations.CalculateTotalResistance(resistors, true);
double parallelR = CircuitCalculations.CalculateTotalResistance(resistors, false);
# CircuitTool Documentation

CircuitTool is a comprehensive C# library for electrical engineering and electronics calculations. It provides a wide range of calculators and utilities for circuit analysis, power calculations, unit conversions, and more. This documentation covers the main features, usage examples, and API references for the library.

## Table of Contents
- [Getting Started](#getting-started)
- [Core Calculators](#core-calculators)
- [Advanced Calculators](#advanced-calculators)
- [Microcontroller Tools](#microcontroller-tools)
- [Beginner Calculators](#beginner-calculators)
- [Unit Conversion](#unit-conversion)
- [API Reference](#api-reference)
- [Contributing](#contributing)
- [License](#license)

---

## Getting Started

### Installation

Install via NuGet:
```bash
dotnet add package CircuitTool
```

Or via Package Manager Console:
```bash
Install-Package CircuitTool
```

### Supported Frameworks
- .NET Framework 2.0+
- .NET 6.0+
- .NET Standard 2.0+

---

## Core Calculators

- **OhmsLawCalculator**: Voltage, current, resistance calculations
- **ResistorCalculator**: Series/parallel resistor networks
- **VoltageCalculator**: Voltage drop, divider, and analysis
- **PowerCalculator**: Power, energy, and cost calculations
- **CapacitorCalculator**: Capacitance, reactance, energy, time constants
- **InductorCalculator**: Inductance, reactance, energy, resonance
- **TransformerCalculator**: Voltage/current/turns ratios, efficiency
- **LEDCalculator**: LED resistor, power, brightness calculations

---

## Advanced Calculators

- **ACCircuitCalculator**: Impedance, phase, RMS/peak/average, Q factor
- **FilterCalculator**: RC/RL filter design, cutoff, gain, phase
- **ToleranceAnalysis**: Component tolerance, Monte Carlo, worst-case
- **PerformanceOptimizations**: SIMD, caching, vectorized calculations

---

## Microcontroller Tools

- **ArduinoTools**: ADC conversions, servo, current calculations
- **ESP32Tools**: ADC, WiFi power, battery life, touch sensor

---

## Beginner Calculators

- **BeginnerCalculators**: Battery life, wire gauge, RC time, dB conversions

---

## Unit Conversion

- **UnitConverter**: Amps, volts, ohms, watts, prefixes, etc.

---

## API Reference

See XML documentation in source code for detailed API usage and parameters. Example usage:

```csharp
using CircuitTool;
double voltage = OhmsLawCalculator.Voltage(2.0, 5.0); // 10V
```

---

## Contributing

- Follow C# coding standards
- Add XML docs for public APIs
- Include unit tests for new features

---

## License

MIT License. See LICENSE file for details.
