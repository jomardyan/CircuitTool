# CircuitTool Interactive CLI

> **Interactive command-line interface for testing and exploring CircuitTool framework capabilities**

The CircuitTool CLI provides both interactive and direct command-line access to all library features, making it perfect for learning, testing, and automation.

## ✨ Key Features

### 🧮 Core Calculations
- **Ohm's Law**: Voltage, current, resistance, and power calculations
- **Power Analysis**: Multiple power calculation methods (P=VI, P=I²R, P=V²/R)
- **Network Analysis**: Series and parallel resistance combinations
- **Component Design**: Real-world component sizing and selection

### ⚡ AC Circuit Analysis  
- **Reactance Calculations**: Inductive (XL) and capacitive (XC) reactance
- **Impedance Analysis**: Complex impedance for RLC circuits
- **Resonance**: Find resonant frequencies and Q-factors
- **Frequency Response**: Magnitude and phase analysis

### 🔧 Component Design Tools
- **LED Design**: Current limiting resistor calculator with safety margins
- **Filter Design**: RC/RL low-pass, high-pass filter calculations
- **Voltage Dividers**: Design optimal resistor ratios
- **Power Supply**: Linear and switching regulator design

### 📊 Advanced Analysis
- **Energy Calculations**: Power consumption and energy cost analysis
- **Efficiency Analysis**: Power conversion efficiency calculations
- **Performance Benchmarking**: Speed and accuracy testing
- **Statistical Analysis**: Component tolerance and worst-case analysis

### 🎯 Interactive Features
- **Guided Menu System**: Step-by-step calculation wizards
- **Built-in Examples**: Pre-loaded circuit examples and tutorials
- **Multiple Output Formats**: Results in table, JSON, CSV, or XML format
- **Batch Processing**: Execute multiple calculations from files
- **Help System**: Context-sensitive help and documentation

## 🚀 Usage Guide

### Interactive Mode (Recommended for Learning)
```bash
CircuitTool.CLI
```
Launches the interactive menu system with guided calculations and tutorials.

### Direct Command Mode (Great for Automation)
```bash
# 🧮 Basic electrical calculations
CircuitTool.CLI basic ohms --voltage 12 --current 2
CircuitTool.CLI basic power --voltage 12 --current 2 --method vi

# 🔧 Component design
CircuitTool.CLI component led --supply 5 --forward 2.1 --current 0.02
CircuitTool.CLI component divider --input 12 --output 5 --current 0.1

# ⚡ AC circuit analysis
CircuitTool.CLI ac reactance --frequency 1000 --inductance 0.01
CircuitTool.CLI ac impedance --resistance 100 --reactance 50
CircuitTool.CLI ac resonance --inductance 0.001 --capacitance 1e-6

# 📊 Advanced analysis
CircuitTool.CLI analysis power --input-power 100 --output-power 85
CircuitTool.CLI analysis thermal --power 25 --resistance 2.5
CircuitTool.CLI analysis tolerance --nominal 100 --tolerance 0.05

# 🎯 Performance and benchmarking
CircuitTool.CLI benchmark --iterations 10000 --test ohms
CircuitTool.CLI performance --test vectorized --size 1000
```

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
