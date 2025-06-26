using System;

namespace CircuitTool
{
    /// <summary>
    /// Provides ESP32-specific calculations and utilities
    /// </summary>
    public static class ESP32Tools
    {
        /// <summary>
        /// Converts analog reading to voltage for ESP32 (12-bit ADC, 3.3V reference)
        /// </summary>
        /// <param name="analogReading">Analog reading value (0-4095)</param>
        /// <param name="referenceVoltage">Reference voltage (default 3.3V for ESP32)</param>
        /// <returns>Voltage value</returns>
        public static double AnalogToVoltage(int analogReading, double referenceVoltage = 3.3)
        {
            if (analogReading < 0 || analogReading > 4095)
                throw new ArgumentException("Analog reading must be between 0 and 4095");
            
            return (analogReading / 4095.0) * referenceVoltage;
        }

        /// <summary>
        /// Converts voltage to analog reading for ESP32
        /// </summary>
        /// <param name="voltage">Voltage value</param>
        /// <param name="referenceVoltage">Reference voltage (default 3.3V for ESP32)</param>
        /// <returns>Analog reading value (0-4095)</returns>
        public static int VoltageToAnalog(double voltage, double referenceVoltage = 3.3)
        {
            if (voltage < 0 || voltage > referenceVoltage)
                throw new ArgumentException($"Voltage must be between 0 and {referenceVoltage}V");
            
            return (int)Math.Round((voltage / referenceVoltage) * 4095.0);
        }

        /// <summary>
        /// Calculates WiFi power consumption based on operation mode
        /// </summary>
        /// <param name="mode">WiFi operation mode</param>
        /// <returns>Current consumption in mA</returns>
        public static double CalculateWiFiPowerConsumption(WiFiMode mode)
        {
            switch (mode)
            {
                case WiFiMode.DeepSleep:
                    return 0.01;
                case WiFiMode.LightSleep:
                    return 0.8;
                case WiFiMode.ModemSleep:
                    return 20;
                case WiFiMode.Active:
                    return 80;
                case WiFiMode.Transmitting:
                    return 170;
                default:
                    return 80;
            }
        }

        /// <summary>
        /// Calculates total ESP32 current consumption
        /// </summary>
        /// <param name="cpuFrequency">CPU frequency in MHz</param>
        /// <param name="wifiMode">WiFi operation mode</param>
        /// <param name="bluetoothActive">Whether Bluetooth is active</param>
        /// <param name="additionalCurrent">Additional current from external components (mA)</param>
        /// <returns>Total current consumption in mA</returns>
        public static double CalculateTotalCurrentConsumption(int cpuFrequency, WiFiMode wifiMode, bool bluetoothActive = false, double additionalCurrent = 0)
        {
            double cpuCurrent = cpuFrequency * 0.5; // Approximate: 0.5mA per MHz
            double wifiCurrent = CalculateWiFiPowerConsumption(wifiMode);
            double bluetoothCurrent = bluetoothActive ? 12 : 0; // Approximate Bluetooth consumption
            
            return cpuCurrent + wifiCurrent + bluetoothCurrent + additionalCurrent;
        }

        /// <summary>
        /// Calculates battery life for ESP32 projects
        /// </summary>
        /// <param name="batteryCapacity">Battery capacity in mAh</param>
        /// <param name="averageCurrent">Average current consumption in mA</param>
        /// <param name="efficiency">Battery efficiency factor (0.7-0.9)</param>
        /// <returns>Battery life in hours</returns>
        public static double CalculateBatteryLife(double batteryCapacity, double averageCurrent, double efficiency = 0.8)
        {
            if (averageCurrent <= 0)
                throw new ArgumentException("Average current must be greater than zero");
            
            return (batteryCapacity * efficiency) / averageCurrent;
        }

        /// <summary>
        /// Calculates ESP32 touch sensor threshold
        /// </summary>
        /// <param name="baselineReading">Baseline touch reading</param>
        /// <param name="sensitivity">Sensitivity factor (0.1-0.9, higher = more sensitive)</param>
        /// <returns>Touch threshold value</returns>
        public static int CalculateTouchThreshold(int baselineReading, double sensitivity = 0.3)
        {
            if (sensitivity < 0.1 || sensitivity > 0.9)
                throw new ArgumentException("Sensitivity must be between 0.1 and 0.9");
            
            return (int)(baselineReading * (1.0 - sensitivity));
        }
    }

    /// <summary>
    /// ESP32 WiFi operation modes
    /// </summary>
    public enum WiFiMode
    {
        DeepSleep,
        LightSleep,
        ModemSleep,
        Active,
        Transmitting
    }
}
