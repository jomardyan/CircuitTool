# CircuitTool Interactive CLI

An interactive command-line interface for testing the CircuitTool framework capabilities.

## Features

### 🧮 Basic Calculations
- Ohm's Law calculations (V, I, R)
- Power calculations (P = VI, P = I²R, P = V²/R)
- Series and parallel resistance combinations

### ⚡ AC Circuit Analysis  
- Reactance calculations (inductive and capacitive)
- RLC impedance analysis
- Resonant frequency calculations

### 🔧 Component Design
- LED current limiting resistor calculator
- Voltage divider design
- Simple RC filter calculations

### 📊 Power Analysis
- Energy consumption calculations
- Electricity bill estimation
- Power efficiency analysis

### 🎯 Interactive Features
- Guided menu system
- Built-in examples and tutorials
- Performance benchmarking
- Multiple output formats (table, JSON, CSV)

## Usage

### Interactive Mode
```bash
CircuitTool.CLI
```
Launches the interactive menu system for guided calculations.

### Command Line Mode
```bash
# Basic Ohm's Law calculation
CircuitTool.CLI basic ohms --voltage 12 --current 2

# LED resistor calculation
CircuitTool.CLI component led --supply 5 --forward 2 --current 0.02

# AC reactance calculation
CircuitTool.CLI ac reactance --frequency 1000 --inductance 0.01

# Energy consumption analysis
CircuitTool.CLI power energy --power 100 --time 24

# Run examples
CircuitTool.CLI examples --example basic

# Performance benchmarks
CircuitTool.CLI benchmark --iterations 10000
```

### Global Options
- `--verbose, -v` - Enable verbose output
- `--format, -f` - Output format (table, json, csv)

## Examples

### Calculate LED Current Limiting Resistor
```bash
CircuitTool.CLI component led --supply 5 --forward 2.1 --current 0.02
```

### Analyze RLC Circuit
```bash
CircuitTool.CLI ac impedance --resistance 100 --inductance 0.01 --capacitance 0.000001 --frequency 1000
```

### Energy Bill Calculation
```bash
CircuitTool.CLI power bill --energy 150 --rate 0.12 --fixed 15.00
```

## Requirements

- .NET 8.0 or later
- CircuitTool library (automatically referenced)

## Development

This CLI application is built on top of the CircuitTool framework and provides an interactive way to explore its capabilities. It's designed for:

- Learning electrical engineering concepts
- Quick circuit calculations
- Testing and validation
- Educational purposes

The CLI is completely separate from the main CircuitTool NuGet package and serves as a testing and demonstration tool.
