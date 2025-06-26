# CircuitTool

A comprehensive C# library for electrical engineering and electronics calculations. CircuitTool simplifies circuit analysis, power calculations, and unit conversions for electrical engineers and electronics enthusiasts.

## Features

- **Ohm's Law Calculations**: Calculate voltage, current, and resistance
- **Resistor Network Analysis**: Series and parallel resistor calculations
- **Voltage Analysis**: Voltage drop and voltage divider calculations
- **Power Calculations**: Various methods to calculate electrical power
- **Energy Calcululations**: Convert between energy units, calculate energy costs
- **Unit Conversions**: Comprehensive electrical unit conversion utilities
- **Specialized Calculators**: Power factor, electricity bill, and energy consumption calculators
- **Advanced Circuit Analysis**: Total resistance, power, and energy calculations for complex circuits

## Getting Started

### Prerequisites

- .NET 8.0 SDK or later

### Installation

Clone the repository:
```bash
git clone https://github.com/jomardyan/CircuitTool.git
cd CircuitTool
```

Build the library:
```bash
dotnet build
```

Run the tests:
```bash
dotnet test
```

### Installation via NuGet

To install CircuitTool via NuGet, use the following command:
```bash
nuget install CircuitTool
```

Or add it to your project using the .NET CLI:
```bash
dotnet add package CircuitTool
```

## Usage Examples

### Ohm's Law Calculations

```csharp
using CircuitTool;

// Calculate voltage using Ohm's Law (V = I × R)
double voltage = OhmsLawCalculator.Voltage(2.0, 5.0);  // 2A through 5Ω = 10V

// Calculate current using Ohm's Law (I = V / R)
double current = OhmsLawCalculator.Current(10.0, 5.0);  // 10V across 5Ω = 2A

// Calculate resistance using Ohm's Law (R = V / I)
double resistance = OhmsLawCalculator.Resistance(10.0, 2.0);  // 10V with 2A = 5Ω
```

### Resistor Network Analysis

```csharp
using CircuitTool;

// Calculate total resistance of resistors in series
double seriesResistance = ResistorCalculator.Series(10.0, 20.0, 30.0);  // = 60Ω

// Calculate total resistance of resistors in parallel
double parallelResistance = ResistorCalculator.Parallel(10.0, 10.0);  // = 5Ω
```

### Voltage and Power Calculations

```csharp
using CircuitTool;

// Calculate voltage drop across a resistor
double voltageDrop = VoltageCalculator.VoltageDrop(10, 2); // 20V

// Use a voltage divider
double outputVoltage = VoltageCalculator.VoltageDivider(9, 2, 1); // 3V

// Calculate power from voltage and current
double power = PowerCalculator.Power(230, 5); // 1150W
```

### Advanced Circuit Analysis

```csharp
using CircuitTool;

// Calculate total resistance in series
var seriesResistance = CircuitCalculations.CalculateTotalResistance(new double[] { 10, 20, 30 }, true);

// Calculate total resistance in parallel
var parallelResistance = CircuitCalculations.CalculateTotalResistance(new double[] { 10, 20, 30 }, false);

// Calculate power
var power = CircuitCalculations.CalculatePower(230, 5); // 230V × 5A = 1150W

// Calculate energy
var energy = CircuitCalculations.CalculateEnergy(1150, 2); // 1150W × 2h = 2300Wh
```

## Documentation

See [DOCUMENTATION.md](DOCUMENTATION.md) for detailed API documentation.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

MIT