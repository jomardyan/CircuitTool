# CircuitTool CLI - Quick Start Guide

## Overview

The CircuitTool CLI is an interactive command-line application designed to test and explore the CircuitTool framework capabilities. It provides a user-friendly interface for electrical engineering calculations, component design, and circuit analysis.

## What's Different from the Main Library

- **Interactive Testing**: Unlike the library which is used programmatically, this CLI provides interactive menus and guided calculations
- **Standalone Application**: This is a separate console application that references the CircuitTool library but is not part of the NuGet package
- **Educational Focus**: Designed for learning, testing, and validation rather than production use
- **Rich UI**: Features colored output, tables, progress bars, and interactive prompts using Spectre.Console

## Quick Start

### 1. Build and Run

**Linux/macOS:**
```bash
./run-cli.sh
```

**Windows:**
```cmd
run-cli.bat
```

**Or manually:**
```bash
cd CircuitTool.CLI
dotnet run
```

### 2. Interactive Mode (Default)

When you run the CLI without arguments, it launches an interactive menu:

```
CircuitTool Interactive CLI
A comprehensive C# library for electrical engineering calculations
Test the framework capabilities interactively

Select a category to explore:
🧮 Basic Calculations (Ohm's Law, Power)
⚡ AC Circuit Analysis  
🔧 Component Design
📊 Power Analysis
🎯 Run Examples
⏱️  Performance Benchmarks
❓ Help & Documentation
🚪 Exit
```

### 3. Command Line Mode

You can also use specific commands directly:

```bash
# Calculate voltage using Ohm's Law
dotnet run -- basic ohms --current 2 --resistance 100

# Design LED current limiting resistor
dotnet run -- component led --supply 5 --forward 2.1 --current 0.02

# Analyze AC reactance
dotnet run -- ac reactance --frequency 1000 --inductance 0.01

# Calculate energy consumption
dotnet run -- power energy --power 100 --time 24

# Run built-in examples
dotnet run -- examples

# Performance benchmarks
dotnet run -- benchmark --iterations 10000
```

## Available Commands

### Basic Calculations (`basic`)
- `ohms` - Ohm's Law calculations (provide 2 of 3: voltage, current, resistance)
- `power` - Power calculations
- `resistance` - Series/parallel resistance combinations

### AC Analysis (`ac`)
- `reactance` - Inductive and capacitive reactance
- `impedance` - RLC circuit impedance analysis
- `resonance` - Resonant frequency calculations

### Component Design (`component`)
- `led` - LED current limiting resistor calculator
- `divider` - Voltage divider design
- `filter` - Simple RC filter calculations

### Power Analysis (`power`)
- `energy` - Energy consumption calculations
- `bill` - Electricity bill estimation
- `efficiency` - Power efficiency analysis

### Utilities
- `examples` - Run built-in CircuitTool examples
- `benchmark` - Performance testing
- `interactive` - Enter interactive mode

## Examples

### Interactive LED Calculator
```bash
dotnet run -- component led --supply 12 --forward 3.2 --current 0.02
```

Output:
```
┌─ LED Current Limiting Resistor ─┐
│ Parameter              │ Value  │ Unit │
├────────────────────────┼────────┼──────┤
│ Supply Voltage         │ 12.00  │ V    │
│ LED Forward Voltage    │ 3.20   │ V    │
│ LED Current            │ 20.0   │ mA   │
│ Resistor Value         │ 440.0  │ Ω    │
│ LED Power              │ 64.0   │ mW   │
│ Resistor Power         │ 176.0  │ mW   │
└────────────────────────┴────────┴──────┘
```

### AC Circuit Analysis
```bash
dotnet run -- ac impedance --resistance 100 --inductance 0.01 --capacitance 0.000001 --frequency 1000
```

### Energy Bill Calculation
```bash
dotnet run -- power bill --energy 150 --rate 0.12 --fixed 15.00
```

## Features

### Rich Console UI
- Colored output with syntax highlighting
- Interactive tables and charts
- Progress bars for long operations
- Error handling with helpful messages

### Educational Focus
- Built-in examples and tutorials
- Step-by-step calculations
- Design recommendations and tips
- Performance analysis

### Extensible Design
- Easy to add new commands
- Modular command structure
- Consistent UI patterns

## Development

### Project Structure
```
CircuitTool.CLI/
├── Commands/           # Command implementations
├── UI/                # Console UI utilities
├── Resources/         # Static resources
├── Program.cs         # Application entry point
└── README.md          # Documentation
```

### Adding New Commands
1. Create a new command class in `Commands/`
2. Implement the command logic
3. Add it to the root command in `Program.cs`
4. Follow the established UI patterns

### Dependencies
- .NET 8.0
- System.CommandLine (command-line parsing)
- Spectre.Console (rich console UI)
- ConsoleTables (table formatting)
- CircuitTool library (project reference)

## Testing Framework Capabilities

This CLI serves as a comprehensive test harness for the CircuitTool framework:

- **Functional Testing**: Verify calculations work correctly
- **Performance Testing**: Benchmark calculation speed
- **Usability Testing**: Test the API ergonomics
- **Documentation**: Living examples of how to use the library

## Next Steps

1. **Run the Interactive Mode**: Start with `dotnet run` to explore all features
2. **Try Command Examples**: Use the command-line interface for specific calculations
3. **Run Benchmarks**: Test performance with `dotnet run -- benchmark`
4. **Explore Examples**: Run `dotnet run -- examples` to see the framework in action
5. **Customize**: Modify the CLI to test specific scenarios relevant to your use case

The CLI provides a complete testing environment for the CircuitTool framework while being completely separate from the production library code.
