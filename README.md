# CircuitTool

A C# class library for electronics and circuit calculations, including:

- Voltage Drop Calculator
- Resistor Calculator
- Ohm's Law Calculator
- Power Calculator
- Energy Calculator
- Unit Conversions (Amps, kW, kVA, VA, Volts, Watts, Joules, eV, kWh, etc.)
- Electricity Bill Calculator
- Energy Consumption Calculator
- Voltage Divider Calculator
- Power Factor Calculator
- Watts-Volts-Amps-Ohms Calculator

## Usage

Add a reference to this library in your C# project and use the static methods in the provided classes.

Example:
```csharp
using CircuitTool;

var voltageDrop = VoltageCalculator.VoltageDrop(10, 2); // 20V
var resistance = ResistorCalculator.Resistance(12, 2); // 6 Ohms
var power = PowerCalculator.Power(230, 5); // 1150W
```

## License

MIT