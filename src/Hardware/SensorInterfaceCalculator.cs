#nullable enable
using System;
using System.Collections.Generic;

namespace CircuitTool
{
    /// <summary>
    /// Provides calculations and utilities for sensor interfacing and signal conditioning
    /// </summary>
    public static class SensorInterfaceCalculator
    {
        /// <summary>
        /// Common sensor types and their characteristics
        /// </summary>
        public enum SensorType
        {
            Thermistor,
            RTD,
            Thermocouple,
            StrainGauge,
            Photoresistor,
            HallEffect,
            Accelerometer,
            Gyroscope
        }

        /// <summary>
        /// Calculates voltage divider for resistive sensor conditioning
        /// </summary>
        /// <param name="sensorResistanceMin">Minimum sensor resistance</param>
        /// <param name="sensorResistanceMax">Maximum sensor resistance</param>
        /// <param name="supplyVoltage">Supply voltage</param>
        /// <param name="targetVoltageRange">Desired output voltage range</param>
        /// <returns>Reference resistor value and bias voltage</returns>
        public static (double referenceResistor, double biasVoltage) CalculateResistiveSensorDivider(
            double sensorResistanceMin, double sensorResistanceMax, double supplyVoltage, double targetVoltageRange = 2.5)
        {
            // Calculate geometric mean for optimal sensitivity
            double referenceResistor = Math.Sqrt(sensorResistanceMin * sensorResistanceMax);
            
            // Calculate bias voltage at mid-range
            double midResistance = Math.Sqrt(sensorResistanceMin * sensorResistanceMax);
            double biasVoltage = supplyVoltage * midResistance / (midResistance + referenceResistor);
            
            return (referenceResistor, biasVoltage);
        }

        /// <summary>
        /// Calculates thermistor temperature from resistance using Steinhart-Hart equation
        /// </summary>
        /// <param name="resistance">Thermistor resistance in ohms</param>
        /// <param name="a">Steinhart-Hart coefficient A</param>
        /// <param name="b">Steinhart-Hart coefficient B</param>
        /// <param name="c">Steinhart-Hart coefficient C</param>
        /// <returns>Temperature in Celsius</returns>
        public static double ThermistorTemperature(double resistance, double a = 1.009249522e-3, double b = 2.378405444e-4, double c = 2.019202697e-7)
        {
            double logR = Math.Log(resistance);
            double tempK = 1.0 / (a + b * logR + c * logR * logR * logR);
            return tempK - 273.15; // Convert to Celsius
        }

        /// <summary>
        /// Calculates RTD temperature from resistance
        /// </summary>
        /// <param name="resistance">RTD resistance in ohms</param>
        /// <param name="r0">RTD resistance at 0°C (typically 100Ω for PT100)</param>
        /// <param name="alpha">Temperature coefficient (0.00385 for PT100)</param>
        /// <returns>Temperature in Celsius</returns>
        public static double RTDTemperature(double resistance, double r0 = 100.0, double alpha = 0.00385)
        {
            return (resistance - r0) / (r0 * alpha);
        }

        /// <summary>
        /// Calculates amplifier gain for sensor signal conditioning
        /// </summary>
        /// <param name="sensorSignalMin">Minimum sensor signal voltage</param>
        /// <param name="sensorSignalMax">Maximum sensor signal voltage</param>
        /// <param name="adcReferenceVoltage">ADC reference voltage</param>
        /// <param name="adcResolution">ADC resolution in bits</param>
        /// <param name="utilizationFactor">Desired ADC range utilization (0-1)</param>
        /// <returns>Required amplifier gain</returns>
        public static double CalculateAmplifierGain(double sensorSignalMin, double sensorSignalMax, 
            double adcReferenceVoltage, int adcResolution, double utilizationFactor = 0.8)
        {
            double sensorRange = sensorSignalMax - sensorSignalMin;
            double targetRange = adcReferenceVoltage * utilizationFactor;
            return targetRange / sensorRange;
        }

        /// <summary>
        /// Calculates low-pass filter for sensor noise reduction
        /// </summary>
        /// <param name="signalFrequency">Maximum signal frequency of interest</param>
        /// <param name="noiseFrequency">Noise frequency to attenuate</param>
        /// <param name="attenuationDb">Desired attenuation in dB</param>
        /// <returns>Filter resistance and capacitance values</returns>
        public static (double resistance, double capacitance) CalculateSensorFilter(
            double signalFrequency, double noiseFrequency, double attenuationDb = 40)
        {
            // First-order RC filter
            double cutoffFrequency = signalFrequency * 2; // Set cutoff above signal frequency
            
            // Check if single pole provides enough attenuation
            double singlePoleAttenuation = 20 * Math.Log10(noiseFrequency / cutoffFrequency);
            if (singlePoleAttenuation < attenuationDb)
            {
                // Adjust cutoff for multi-pole filter or warn user
                cutoffFrequency = noiseFrequency / Math.Pow(10, attenuationDb / 20);
            }
            
            // Choose standard capacitor value
            double capacitance = 1e-6; // 1µF
            double resistance = 1.0 / (2 * Math.PI * cutoffFrequency * capacitance);
            
            return (resistance, capacitance);
        }

        /// <summary>
        /// Calculates Wheatstone bridge output for strain gauge
        /// </summary>
        /// <param name="strain">Applied strain (µε)</param>
        /// <param name="gaugeFactor">Strain gauge factor (typically 2.0)</param>
        /// <param name="bridgeVoltage">Bridge excitation voltage</param>
        /// <param name="nominalResistance">Gauge nominal resistance</param>
        /// <returns>Bridge output voltage</returns>
        public static double StrainGaugeBridgeOutput(double strain, double gaugeFactor = 2.0, 
            double bridgeVoltage = 5.0, double nominalResistance = 350)
        {
            double resistanceChange = nominalResistance * gaugeFactor * strain / 1e6;
            double sensitivity = gaugeFactor / 4.0; // Quarter bridge sensitivity
            return bridgeVoltage * sensitivity * strain / 1e6;
        }

        /// <summary>
        /// Calculates ADC resolution requirements for sensor measurement
        /// </summary>
        /// <param name="sensorRange">Full-scale sensor output range</param>
        /// <param name="requiredAccuracy">Required measurement accuracy</param>
        /// <param name="noiseBits">Effective noise in LSBs (typically 1-3)</param>
        /// <returns>Required ADC resolution in bits</returns>
        public static int CalculateADCResolution(double sensorRange, double requiredAccuracy, double noiseBits = 2)
        {
            double requiredSteps = sensorRange / requiredAccuracy;
            double effectiveSteps = requiredSteps * noiseBits; // Account for noise
            return (int)Math.Ceiling(Math.Log(effectiveSteps) / Math.Log(2));
        }

        /// <summary>
        /// Calculates sensor calibration coefficients (linear)
        /// </summary>
        /// <param name="rawValues">Array of raw sensor readings</param>
        /// <param name="referenceValues">Array of reference values</param>
        /// <returns>Calibration slope and offset</returns>
        public static (double slope, double offset) CalculateLinearCalibration(double[] rawValues, double[] referenceValues)
        {
            if (rawValues.Length != referenceValues.Length || rawValues.Length < 2)
                throw new ArgumentException("Need at least 2 matched data points");
            
            int n = rawValues.Length;
            double sumX = 0, sumY = 0, sumXY = 0, sumX2 = 0;
            
            for (int i = 0; i < n; i++)
            {
                sumX += rawValues[i];
                sumY += referenceValues[i];
                sumXY += rawValues[i] * referenceValues[i];
                sumX2 += rawValues[i] * rawValues[i];
            }
            
            double slope = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);
            double offset = (sumY - slope * sumX) / n;
            
            return (slope, offset);
        }

        /// <summary>
        /// Generates sensor interface code template
        /// </summary>
        /// <param name="sensorType">Type of sensor</param>
        /// <param name="interface_">Interface type (Analog, I2C, SPI)</param>
        /// <returns>Code template</returns>
        public static string GenerateSensorCode(SensorType sensorType, string interface_ = "Analog")
        {
            return sensorType switch
            {
                SensorType.Thermistor => GenerateThermistorCode(interface_),
                SensorType.RTD => GenerateRTDCode(interface_),
                SensorType.StrainGauge => GenerateStrainGaugeCode(interface_),
                SensorType.HallEffect => GenerateHallEffectCode(interface_),
                _ => GenerateGenericSensorCode(sensorType, interface_)
            };
        }

        private static string GenerateThermistorCode(string interface_)
        {
            return $@"
// Thermistor Temperature Sensor ({interface_})
class ThermistorSensor {{
    private int analogPin;
    private double refResistor;
    private double[] shCoeffs; // Steinhart-Hart coefficients
    
    public ThermistorSensor(int pin, double refR = 10000) {{
        this.analogPin = pin;
        this.refResistor = refR;
        this.shCoeffs = new double[] {{ 1.009249522e-3, 2.378405444e-4, 2.019202697e-7 }};
    }}
    
    public double ReadTemperature() {{
        int adcValue = analogRead(analogPin);
        double voltage = (adcValue / 1023.0) * 5.0; // Assuming 5V reference
        double resistance = refResistor * (5.0 / voltage - 1.0);
        
        double logR = Math.Log(resistance);
        double tempK = 1.0 / (shCoeffs[0] + shCoeffs[1] * logR + shCoeffs[2] * logR * logR * logR);
        return tempK - 273.15; // Convert to Celsius
    }}
}}";
        }

        private static string GenerateRTDCode(string interface_)
        {
            return $@"
// RTD Temperature Sensor ({interface_})
class RTDSensor {{
    private int analogPin;
    private double r0, alpha;
    private double excitationVoltage;
    
    public RTDSensor(int pin, double r0Val = 100.0, double alphaVal = 0.00385) {{
        this.analogPin = pin;
        this.r0 = r0Val;
        this.alpha = alphaVal;
        this.excitationVoltage = 3.3;
    }}
    
    public double ReadTemperature() {{
        int adcValue = analogRead(analogPin);
        double voltage = (adcValue / 4095.0) * 3.3; // 12-bit ADC
        double current = 0.001; // 1mA excitation current
        double resistance = voltage / current;
        
        return (resistance - r0) / (r0 * alpha);
    }}
}}";
        }

        private static string GenerateStrainGaugeCode(string interface_)
        {
            return $@"
// Strain Gauge Sensor ({interface_})
class StrainGaugeSensor {{
    private int analogPin;
    private double gaugeFactor, bridgeVoltage;
    private double zeroOffset;
    
    public StrainGaugeSensor(int pin, double gf = 2.0, double vBridge = 5.0) {{
        this.analogPin = pin;
        this.gaugeFactor = gf;
        this.bridgeVoltage = vBridge;
        this.zeroOffset = 0;
    }}
    
    public void Calibrate() {{
        // Read zero strain value
        zeroOffset = ReadRawVoltage();
    }}
    
    public double ReadStrain() {{
        double voltage = ReadRawVoltage() - zeroOffset;
        double sensitivity = gaugeFactor / 4.0; // Quarter bridge
        return (voltage / bridgeVoltage) / sensitivity * 1e6; // Return in µε
    }}
    
    private double ReadRawVoltage() {{
        int adcValue = analogRead(analogPin);
        return (adcValue / 1023.0) * 5.0;
    }}
}}";
        }

        private static string GenerateHallEffectCode(string interface_)
        {
            return $@"
// Hall Effect Sensor ({interface_})
class HallEffectSensor {{
    private int analogPin;
    private double sensitivity; // mV/Gauss
    private double zeroVoltage;
    
    public HallEffectSensor(int pin, double sens = 5.0) {{
        this.analogPin = pin;
        this.sensitivity = sens / 1000.0; // Convert mV to V
        this.zeroVoltage = 2.5; // Typical for 5V supply
    }}
    
    public double ReadMagneticField() {{
        int adcValue = analogRead(analogPin);
        double voltage = (adcValue / 1023.0) * 5.0;
        return (voltage - zeroVoltage) / sensitivity; // Return in Gauss
    }}
    
    public bool IsFieldPresent(double threshold = 10) {{
        return Math.Abs(ReadMagneticField()) > threshold;
    }}
}}";
        }

        private static string GenerateGenericSensorCode(SensorType sensorType, string interface_)
        {
            return $@"
// {sensorType} Sensor ({interface_})
class {sensorType}Sensor {{
    private int pin;
    
    public {sensorType}Sensor(int sensorPin) {{
        this.pin = sensorPin;
    }}
    
    public double ReadValue() {{
        // Implement specific reading logic for {sensorType}
        int adcValue = analogRead(pin);
        return (adcValue / 1023.0) * 5.0; // Convert to voltage
    }}
}}";
        }
    }
}
