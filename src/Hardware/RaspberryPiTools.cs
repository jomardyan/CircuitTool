#nullable enable
using System;
using System.Collections.Generic;

namespace CircuitTool
{
    /// <summary>
    /// Provides calculations and utilities for Raspberry Pi projects
    /// </summary>
    public static class RaspberryPiTools
    {
        /// <summary>
        /// GPIO pin mapping for different Raspberry Pi models
        /// </summary>
        public enum GPIOPin
        {
            GPIO2 = 2, GPIO3 = 3, GPIO4 = 4, GPIO5 = 5, GPIO6 = 6, GPIO7 = 7, GPIO8 = 8, GPIO9 = 9,
            GPIO10 = 10, GPIO11 = 11, GPIO12 = 12, GPIO13 = 13, GPIO14 = 14, GPIO15 = 15, GPIO16 = 16,
            GPIO17 = 17, GPIO18 = 18, GPIO19 = 19, GPIO20 = 20, GPIO21 = 21, GPIO22 = 22, GPIO23 = 23,
            GPIO24 = 24, GPIO25 = 25, GPIO26 = 26, GPIO27 = 27
        }

        /// <summary>
        /// PWM configuration settings
        /// </summary>
        public struct PWMConfig
        {
            public double Frequency { get; set; }
            public double DutyCycle { get; set; }
            public double Voltage { get; set; }
        }

        /// <summary>
        /// Calculates current limiting resistor for LED connected to GPIO pin
        /// </summary>
        /// <param name="ledVoltage">LED forward voltage in volts</param>
        /// <param name="ledCurrent">LED forward current in amperes</param>
        /// <param name="gpioVoltage">GPIO output voltage (typically 3.3V)</param>
        /// <returns>Required resistor value in ohms</returns>
        public static double CalculateLEDResistor(double ledVoltage, double ledCurrent, double gpioVoltage = 3.3)
        {
            if (ledVoltage >= gpioVoltage)
                throw new ArgumentException("LED voltage must be less than GPIO voltage");
            
            return (gpioVoltage - ledVoltage) / ledCurrent;
        }

        /// <summary>
        /// Calculates PWM settings for motor speed control
        /// </summary>
        /// <param name="motorVoltage">Motor rated voltage</param>
        /// <param name="supplyVoltage">Power supply voltage</param>
        /// <param name="speedPercentage">Desired speed as percentage (0-100)</param>
        /// <returns>PWM duty cycle (0-1)</returns>
        public static double CalculateMotorPWM(double motorVoltage, double supplyVoltage, double speedPercentage)
        {
            if (speedPercentage < 0 || speedPercentage > 100)
                throw new ArgumentException("Speed percentage must be between 0 and 100");
            
            double targetVoltage = motorVoltage * (speedPercentage / 100.0);
            return Math.Min(targetVoltage / supplyVoltage, 1.0);
        }

        /// <summary>
        /// Calculates pull-up/pull-down resistor value for GPIO inputs
        /// </summary>
        /// <param name="inputVoltage">Input signal voltage</param>
        /// <param name="leakageCurrent">Maximum leakage current (typically 1µA)</param>
        /// <param name="noiseMargin">Desired noise margin voltage (typically 0.5V)</param>
        /// <returns>Recommended resistor value in ohms</returns>
        public static double CalculatePullResistor(double inputVoltage = 3.3, double leakageCurrent = 1e-6, double noiseMargin = 0.5)
        {
            return (inputVoltage - noiseMargin) / leakageCurrent;
        }

        /// <summary>
        /// Calculates power consumption for Raspberry Pi project
        /// </summary>
        /// <param name="piModel">Pi model power consumption</param>
        /// <param name="peripheralPower">Additional peripheral power in watts</param>
        /// <returns>Total power consumption in watts</returns>
        public static double CalculatePowerConsumption(RaspberryPiModel piModel, double peripheralPower = 0)
        {
            double basePower = piModel switch
            {
                RaspberryPiModel.Pi4B => 6.4,  // Watts
                RaspberryPiModel.Pi3B => 5.1,
                RaspberryPiModel.PiZero2W => 1.3,
                RaspberryPiModel.PiZero => 0.5,
                RaspberryPiModel.Pi400 => 8.0,
                _ => 5.0
            };
            
            return basePower + peripheralPower;
        }

        /// <summary>
        /// Generates I2C address scanning code
        /// </summary>
        /// <returns>Python code for I2C address scanning</returns>
        public static string GenerateI2CScanCode()
        {
            return @"
# I2C Address Scanner for Raspberry Pi
import smbus
import time

def scan_i2c_devices():
    bus = smbus.SMBus(1)  # I2C bus 1
    devices = []
    
    print('Scanning I2C devices...')
    for addr in range(0x03, 0x78):
        try:
            bus.read_byte(addr)
            devices.append(hex(addr))
            print(f'Device found at address: {hex(addr)}')
        except:
            pass
    
    if not devices:
        print('No I2C devices found')
    else:
        print(f'Found {len(devices)} device(s): {devices}')
    
    return devices

# Run the scan
scan_i2c_devices()
";
        }

        /// <summary>
        /// Calculates SPI clock frequency limits
        /// </summary>
        /// <param name="cableLength">SPI cable length in meters</param>
        /// <param name="loadCapacitance">Load capacitance in farads</param>
        /// <returns>Maximum safe SPI frequency in Hz</returns>
        public static double CalculateMaxSPIFrequency(double cableLength, double loadCapacitance = 50e-12)
        {
            // Simplified calculation based on signal integrity
            double maxFreq = 1.0 / (2 * Math.PI * 50 * loadCapacitance); // 50Ω characteristic impedance
            
            // Reduce frequency for longer cables
            if (cableLength > 0.1) // 10cm
            {
                maxFreq *= 0.1 / cableLength;
            }
            
            return Math.Min(maxFreq, 32e6); // Max 32MHz for most Pi models
        }
    }

    /// <summary>
    /// Raspberry Pi model enumeration
    /// </summary>
    public enum RaspberryPiModel
    {
        Pi4B,
        Pi3B,
        PiZero2W,
        PiZero,
        Pi400
    }
}
