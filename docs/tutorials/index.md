# Getting Started with CircuitTool

This tutorial will guide you through the basics of using CircuitTool for electrical engineering calculations.

## Prerequisites

- .NET Framework 4.5+ or .NET 6.0+
- Basic understanding of electrical engineering concepts
- Visual Studio, VS Code, or any .NET-compatible IDE

## Installation

### Via NuGet Package Manager (Recommended)

1. **Using Package Manager Console:**
   ```powershell
   Install-Package CircuitTool
   ```

2. **Using .NET CLI:**
   ```bash
   dotnet add package CircuitTool
   ```

3. **Using PackageReference in .csproj:**
   ```xml
   <PackageReference Include="CircuitTool" Version="2.0.0" />
   ```

## Basic Usage Examples

### 1. Ohm's Law Calculations

```csharp
using CircuitTool;

// Calculate voltage when current and resistance are known
double voltage = OhmsLawCalculator.CalculateVoltage(current: 2.5, resistance: 100.0);
Console.WriteLine($"Voltage: {voltage}V"); // Output: 250V

// Calculate current when voltage and resistance are known
double current = OhmsLawCalculator.CalculateCurrent(voltage: 12.0, resistance: 1000.0);
Console.WriteLine($"Current: {current}A"); // Output: 0.012A

// Calculate resistance when voltage and current are known
double resistance = OhmsLawCalculator.CalculateResistance(voltage: 5.0, current: 0.1);
Console.WriteLine($"Resistance: {resistance}Ω"); // Output: 50Ω
```

### 2. Power Calculations

```csharp
// Power using voltage and current
double power1 = PowerCalculator.CalculatePower(voltage: 12.0, current: 2.0);
Console.WriteLine($"Power: {power1}W"); // Output: 24W

// Power using voltage and resistance
double power2 = PowerCalculator.CalculatePowerFromVoltageResistance(voltage: 12.0, resistance: 6.0);
Console.WriteLine($"Power: {power2}W"); // Output: 24W

// Power using current and resistance
double power3 = PowerCalculator.CalculatePowerFromCurrentResistance(current: 2.0, resistance: 6.0);
Console.WriteLine($"Power: {power3}W"); // Output: 24W
```

### 3. LED Calculations

```csharp
// Calculate current limiting resistor for LED
var ledResult = LEDCalculator.CalculateCurrentLimitingResistor(
    supplyVoltage: 5.0,      // 5V supply
    ledVoltage: 2.1,         // Red LED forward voltage
    ledCurrent: 0.02         // 20mA desired current
);

Console.WriteLine($"Required resistor: {ledResult.resistance}Ω");
Console.WriteLine($"Power dissipation: {ledResult.powerDissipation}W");
Console.WriteLine($"Nearest standard value: {ledResult.nearestStandardResistor}Ω");
```

### 4. Resistor Networks

```csharp
// Series resistance calculation
double[] seriesResistors = { 100, 220, 330, 470 };
double totalSeries = ResistorCalculator.CalculateSeriesResistance(seriesResistors);
Console.WriteLine($"Total series resistance: {totalSeries}Ω"); // Output: 1120Ω

// Parallel resistance calculation
double[] parallelResistors = { 1000, 1000, 1000 };
double totalParallel = ResistorCalculator.CalculateParallelResistance(parallelResistors);
Console.WriteLine($"Total parallel resistance: {totalParallel:F2}Ω"); // Output: 333.33Ω
```

### 5. Unit Conversions

```csharp
// Convert between different units
double milliAmps = UnitConverter.ConvertCurrent(2.5, "A", "mA");
Console.WriteLine($"2.5A = {milliAmps}mA"); // Output: 2500mA

double kiloOhms = UnitConverter.ConvertResistance(4700, "Ω", "kΩ");
Console.WriteLine($"4700Ω = {kiloOhms}kΩ"); // Output: 4.7kΩ

double milliVolts = UnitConverter.ConvertVoltage(3.3, "V", "mV");
Console.WriteLine($"3.3V = {milliVolts}mV"); // Output: 3300mV
```

## Next Steps

- **[Hardware Integration Tutorial](hardware-tutorial.md)** - Learn to use Arduino and ESP32 tools
- **[Advanced Calculations](advanced-tutorial.md)** - AC circuits, filters, and transformers
- **[Communication Protocols](communication-tutorial.md)** - I2C, SPI, UART tools
- **[API Reference](../api/index.md)** - Complete documentation of all classes and methods

## Common Patterns

### Error Handling

```csharp
try
{
    double result = OhmsLawCalculator.CalculateVoltage(current: 0, resistance: 100);
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Invalid input: {ex.Message}");
}
```

### Batch Calculations

```csharp
// Calculate multiple LED resistors at once
var ledConfigs = new[]
{
    new { Supply = 5.0, LedV = 2.1, Current = 0.02 },
    new { Supply = 12.0, LedV = 3.3, Current = 0.03 },
    new { Supply = 3.3, LedV = 1.8, Current = 0.01 }
};

foreach (var config in ledConfigs)
{
    var result = LEDCalculator.CalculateCurrentLimitingResistor(
        config.Supply, config.LedV, config.Current);
    Console.WriteLine($"Supply: {config.Supply}V, LED: {config.LedV}V, " +
                     $"Current: {config.Current}A → Resistor: {result.resistance}Ω");
}
```

## Performance Tips

1. **Use caching for repeated calculations:**
   ```csharp
   // The library automatically caches common calculations
   var calc1 = OhmsLawCalculator.CalculateVoltage(2.0, 100.0);
   var calc2 = OhmsLawCalculator.CalculateVoltage(2.0, 100.0); // Retrieved from cache
   ```

2. **Use bulk operations for large datasets:**
   ```csharp
   // For processing many values efficiently
   var resistanceValues = new double[] { 100, 220, 330, 470, 680, 1000 };
   var results = BulkOperations.CalculateMultipleOhmsLaw(current: 0.01, resistanceValues);
   ```

3. **Use async methods for long-running calculations:**
   ```csharp
   var result = await AsyncCalculations.CalculateComplexCircuitAsync(circuitData);
   ```

Ready to dive deeper? Check out our [Hardware Integration Tutorial](hardware-tutorial.md) next!
