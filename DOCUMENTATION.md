# CircuitTool Documentation

## Overview

CircuitTool is a comprehensive C# library designed for electrical engineering and electronics calculations. It provides a wide range of calculators and tools for circuit analysis, power calculations, microcontroller projects, and educational purposes.

## Core Classes and Methods

### OhmsLawCalculator

Provides fundamental Ohm's Law calculations.

#### Methods:
- `Voltage(current, resistance)` - Calculate voltage (V = I × R)
- `Current(voltage, resistance)` - Calculate current (I = V / R)  
- `Resistance(voltage, current)` - Calculate resistance (R = V / I)

### LEDCalculator

Specialized calculations for LED circuits and lighting projects.

#### Methods:
- `CalculateResistorValue(supplyVoltage, ledVoltage, ledCurrent)` - Calculate required current-limiting resistor
- `CalculateLEDPower(supplyVoltage, ledCurrent)` - Calculate LED power consumption
- `CalculateBrightness(dutyCycle)` - Convert PWM duty cycle to brightness ratio
- `CalculateSeriesResistor(supplyVoltage, ledVoltage, numLEDs, ledCurrent)` - Calculate resistor for series LED configuration

### ArduinoTools

Arduino-specific utilities and calculations.

#### Methods:
- `AnalogToVoltage(analogReading, referenceVoltage)` - Convert 10-bit ADC reading to voltage
- `VoltageToAnalog(voltage, referenceVoltage)` - Convert voltage to ADC reading
- `ServoAngleToPulseWidth(angle)` - Calculate servo pulse width for angle
- `CalculateCurrentConsumption(cpuCurrent, digitalPins, analogPins, additionalCurrent)` - Estimate total current consumption
- `CalculatePWMFrequency(prescaler, clockFrequency)` - Calculate PWM frequency

### ESP32Tools

ESP32-specific utilities and power management calculations.

#### Methods:
- `AnalogToVoltage(analogReading, referenceVoltage)` - Convert 12-bit ADC reading to voltage
- `VoltageToAnalog(voltage, referenceVoltage)` - Convert voltage to ADC reading
- `CalculateWiFiPowerConsumption(mode)` - Get power consumption for WiFi mode
- `CalculateTotalCurrentConsumption(cpuFrequency, wifiMode, bluetoothActive, additionalCurrent)` - Calculate total ESP32 power consumption
- `CalculateBatteryLife(batteryCapacity, averageCurrent, efficiency)` - Estimate battery life
- `CalculateTouchThreshold(baselineReading, sensitivity)` - Calculate touch sensor threshold

#### WiFiMode Enum:
- `DeepSleep` - Ultra-low power mode
- `LightSleep` - Low power with RAM retention
- `ModemSleep` - WiFi modem off, CPU active
- `Active` - Normal WiFi operation
- `Transmitting` - WiFi transmitting data

### BeginnerCalculators

Educational and beginner-friendly calculators for common electronics problems.

#### Methods:
- `BatteryLifeCalculator(batteryCapacity, loadCurrent)` - Calculate battery runtime
- `WireGaugeCalculator(current, safetyFactor)` - Determine appropriate AWG wire gauge
- `InductorTurnsCalculator(inductance, coreDiameter, coreLength, permeability)` - Calculate inductor turns
- `RCTimeConstantCapacitor(resistance, timeConstant)` - Calculate capacitor for RC time constant
- `RCOscillatorFrequency(resistance, capacitance)` - Calculate RC oscillator frequency
- `PowerRatioToDecibels(powerRatio)` - Convert power ratio to dB
- `VoltageRatioToDecibels(voltageRatio)` - Convert voltage ratio to dB
- `TransformerTurnsRatio(primaryVoltage, secondaryVoltage)` - Calculate transformer turns ratio

### CircuitCalculations

Advanced circuit analysis methods.

#### Methods:
- `CalculateTotalResistance(resistances, isSeries)` - Calculate total resistance for series or parallel configurations
- `CalculatePower(voltage, current)` - Calculate electrical power
- `CalculateEnergy(power, time)` - Calculate energy consumption

## Error Handling

All calculators include input validation and throw `ArgumentException` for invalid inputs such as:
- Negative values where positive values are required
- Zero values where non-zero values are required
- Values outside valid ranges (e.g., duty cycle > 100%)

## Best Practices

1. **Input Validation**: Always validate inputs before calling calculator methods
2. **Unit Consistency**: Ensure all inputs use consistent units (e.g., all currents in Amperes)
3. **Safety Margins**: Consider safety factors for real-world applications
4. **Temperature Effects**: Account for temperature coefficients in precision applications

## Examples by Use Case

### LED Strip Design
```csharp
// Calculate resistor for LED strip
double resistor = LEDCalculator.CalculateResistorValue(12.0, 3.2, 0.02);
double power = LEDCalculator.CalculateLEDPower(12.0, 0.02);
```

### Arduino Project Power Budget
```csharp
// Calculate total Arduino project current
double arduinoCurrent = ArduinoTools.CalculateCurrentConsumption(20, 10, 4, 100);
double batteryLife = BeginnerCalculators.BatteryLifeCalculator(2000, arduinoCurrent);
```

### ESP32 IoT Device
```csharp
// Calculate ESP32 power consumption
double totalCurrent = ESP32Tools.CalculateTotalCurrentConsumption(240, WiFiMode.Active, false, 50);
double batteryHours = ESP32Tools.CalculateBatteryLife(3000, totalCurrent);
```

### Circuit Analysis
```csharp
// Analyze resistor network
double[] resistors = {100, 200, 300};
double seriesR = CircuitCalculations.CalculateTotalResistance(resistors, true);
double parallelR = CircuitCalculations.CalculateTotalResistance(resistors, false);
```
