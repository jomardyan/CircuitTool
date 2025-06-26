# CircuitTool

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

**Note**: You'll need a GitHub Personal Access Token with `read:packages` permission. See [GITHUB_TOKEN_GUIDE.md](GITHUB_TOKEN_GUIDE.md) for detailed instructions on creating a token.
```bash
Install-Package CircuitTool
```

### Development Installation

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

### Publishing

For maintainers, use the provided scripts to publish new versions:

**PowerShell (Windows):**
```powershell
# Publish to GitHub Packages
.\publish.ps1 -GitHub

# Publish to NuGet.org
.\publish.ps1 -NuGet

# Publish to both
.\publish.ps1 -All
```

**Bash (Linux/macOS):**
```bash
# Publish to GitHub Packages
./publish.sh --github

# Publish to NuGet.org
./publish.sh --nuget

# Publish to both
./publish.sh --all
```

See `GITHUB_PACKAGES.md` for detailed publishing instructions.

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