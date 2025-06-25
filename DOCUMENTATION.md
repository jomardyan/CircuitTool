# CircuitTool Documentation

This document provides detailed documentation for the CircuitTool library.

## Table of Contents

1. [Ohm's Law Calculator](#ohms-law-calculator)
2. [Resistor Calculator](#resistor-calculator)
3. [Voltage Calculator](#voltage-calculator)
4. [Power Calculator](#power-calculator)
5. [Energy Calculator](#energy-calculator)
6. [Unit Converter](#unit-converter)

---

## Ohm's Law Calculator

The `OhmsLawCalculator` provides methods for calculating voltage, current, and resistance based on Ohm's Law.

### Methods

#### `Voltage(double current, double resistance)`

Calculates voltage using Ohm's Law: V = I × R.

- **Parameters**:
  - `current`: The electric current in amperes (A)
  - `resistance`: The electrical resistance in ohms (Ω)
- **Returns**: The voltage in volts (V)
- **Example**:
  ```csharp
  double voltage = OhmsLawCalculator.Voltage(2.0, 5.0); // 10V
  ```

#### `Current(double voltage, double resistance)`

Calculates current using Ohm's Law: I = V / R.

- **Parameters**:
  - `voltage`: The voltage in volts (V)
  - `resistance`: The electrical resistance in ohms (Ω)
- **Returns**: The current in amperes (A)
- **Example**:
  ```csharp
  double current = OhmsLawCalculator.Current(10.0, 5.0); // 2A
  ```

#### `Resistance(double voltage, double current)`

Calculates resistance using Ohm's Law: R = V / I.

- **Parameters**:
  - `voltage`: The voltage in volts (V)
  - `current`: The electric current in amperes (A)
- **Returns**: The resistance in ohms (Ω)
- **Example**:
  ```csharp
  double resistance = OhmsLawCalculator.Resistance(10.0, 2.0); // 5Ω
  ```

---

## Resistor Calculator

The `ResistorCalculator` provides methods for calculating resistance in different circuit configurations.

### Methods

#### `Resistance(double voltage, double current)`

Calculates resistance using Ohm's Law: R = V / I.

- **Parameters**:
  - `voltage`: The voltage in volts (V)
  - `current`: The electric current in amperes (A)
- **Returns**: The resistance in ohms (Ω)
- **Example**:
  ```csharp
  double resistance = ResistorCalculator.Resistance(10.0, 2.0); // 5Ω
  ```

#### `Series(params double[] resistors)`

Calculates the total resistance of resistors connected in series.

- **Parameters**:
  - `resistors`: Array of resistor values in ohms (Ω)
- **Returns**: The total resistance in ohms (Ω)
- **Example**:
  ```csharp
  double totalResistance = ResistorCalculator.Series(10.0, 20.0, 30.0); // 60Ω
  ```

#### `Parallel(params double[] resistors)`

Calculates the total resistance of resistors connected in parallel.

- **Parameters**:
  - `resistors`: Array of resistor values in ohms (Ω)
- **Returns**: The total resistance in ohms (Ω)
- **Example**:
  ```csharp
  double totalResistance = ResistorCalculator.Parallel(10.0, 10.0); // 5Ω
  ```

---

## Voltage Calculator

The `VoltageCalculator` provides methods for various voltage-related calculations.

### Methods

#### `VoltageDrop(double current, double resistance)`

Calculates the voltage drop across a resistor: V = I × R.

- **Parameters**:
  - `current`: The electric current in amperes (A)
  - `resistance`: The electrical resistance in ohms (Ω)
- **Returns**: The voltage drop in volts (V)
- **Example**:
  ```csharp
  double voltageDrop = VoltageCalculator.VoltageDrop(10.0, 2.0); // 20V
  ```

#### `VoltageDivider(double vin, double r1, double r2)`

Calculates the output voltage of a voltage divider circuit: Vout = Vin × (R2 / (R1 + R2)).

- **Parameters**:
  - `vin`: The input voltage in volts (V)
  - `r1`: The first resistor value in ohms (Ω)
  - `r2`: The second resistor value in ohms (Ω)
- **Returns**: The output voltage in volts (V)
- **Example**:
  ```csharp
  double outputVoltage = VoltageCalculator.VoltageDivider(9.0, 2.0, 1.0); // 3V
  ```

---

## Power Calculator

The `PowerCalculator` provides methods for calculating electrical power.

### Methods

#### `Power(double voltage, double current)`

Calculates power using voltage and current: P = V × I.

- **Parameters**:
  - `voltage`: The voltage in volts (V)
  - `current`: The electric current in amperes (A)
- **Returns**: The power in watts (W)
- **Example**:
  ```csharp
  double power = PowerCalculator.Power(12.0, 2.0); // 24W
  ```

#### `PowerFromCurrentResistance(double current, double resistance)`

Calculates power using current and resistance: P = I² × R.

- **Parameters**:
  - `current`: The electric current in amperes (A)
  - `resistance`: The electrical resistance in ohms (Ω)
- **Returns**: The power in watts (W)
- **Example**:
  ```csharp
  double power = PowerCalculator.PowerFromCurrentResistance(2.0, 6.0); // 24W
  ```

#### `PowerFromVoltageResistance(double voltage, double resistance)`

Calculates power using voltage and resistance: P = V² / R.

- **Parameters**:
  - `voltage`: The voltage in volts (V)
  - `resistance`: The electrical resistance in ohms (Ω)
- **Returns**: The power in watts (W)
- **Example**:
  ```csharp
  double power = PowerCalculator.PowerFromVoltageResistance(12.0, 6.0); // 24W
  ```

---

## Energy Calculator

The `EnergyCalculator` provides methods for energy calculations and conversions.

### Methods

#### `Joules(double power, double timeSeconds)`

Calculates energy in joules: E = P × t.

- **Parameters**:
  - `power`: The power in watts (W)
  - `timeSeconds`: The time in seconds (s)
- **Returns**: The energy in joules (J)
- **Example**:
  ```csharp
  double energy = EnergyCalculator.Joules(100.0, 10.0); // 1000J
  ```

#### `KWh(double watts, double hours)`

Converts power and time to kilowatt-hours: kWh = (W × h) / 1000.

- **Parameters**:
  - `watts`: The power in watts (W)
  - `hours`: The time in hours (h)
- **Returns**: The energy in kilowatt-hours (kWh)
- **Example**:
  ```csharp
  double energy = EnergyCalculator.KWh(1000.0, 2.0); // 2kWh
  ```

#### `EnergyCost(double kWh, double ratePerKWh)`

Calculates the cost of electrical energy: Cost = kWh × rate.

- **Parameters**:
  - `kWh`: The energy in kilowatt-hours (kWh)
  - `ratePerKWh`: The cost per kilowatt-hour
- **Returns**: The cost of energy
- **Example**:
  ```csharp
  double cost = EnergyCalculator.EnergyCost(5.0, 3.0); // 15.0
  ```

---

## Unit Converter

The `UnitConverter` provides methods for converting between different electrical units.

### Selected Methods

#### `AmpsToWatts(double amps, double volts)`

Converts amperes to watts: W = A × V.

- **Parameters**:
  - `amps`: The current in amperes (A)
  - `volts`: The voltage in volts (V)
- **Returns**: The power in watts (W)
- **Example**:
  ```csharp
  double watts = UnitConverter.AmpsToWatts(2.0, 120.0); // 240W
  ```

#### `KWToKWh(double kW, double hours)`

Converts kilowatts to kilowatt-hours: kWh = kW × h.

- **Parameters**:
  - `kW`: The power in kilowatts (kW)
  - `hours`: The time in hours (h)
- **Returns**: The energy in kilowatt-hours (kWh)
- **Example**:
  ```csharp
  double kWh = UnitConverter.KWToKWh(1.5, 2.0); // 3kWh
  ```

#### `KVAToKW(double kVA, double powerFactor)`

Converts kilovolt-amperes to kilowatts: kW = kVA × PF.

- **Parameters**:
  - `kVA`: The apparent power in kilovolt-amperes (kVA)
  - `powerFactor`: The power factor (default: 1.0)
- **Returns**: The real power in kilowatts (kW)
- **Example**:
  ```csharp
  double kW = UnitConverter.KVAToKW(10.0, 0.8); // 8kW
  ```

> Note: The UnitConverter contains many more conversion methods not listed here. See the source code for the complete list.
