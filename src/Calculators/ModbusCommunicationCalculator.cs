using System;
using System.Collections.Generic;
using System.Linq;
using CircuitTool.Units;

namespace CircuitTool.Calculators
{
    /// <summary>
    /// Calculator for Modbus communication parameters and frame analysis
    /// </summary>
    public static class ModbusCommunicationCalculator
    {
        #region Frame Timing Calculations
        
        /// <summary>
        /// Calculate character time for given baud rate
        /// </summary>
        /// <param name="baudRate">Baud rate in bps</param>
        /// <param name="dataBits">Number of data bits (typically 8)</param>
        /// <param name="stopBits">Number of stop bits (1 or 2)</param>
        /// <param name="parity">Whether parity bit is used</param>
        /// <returns>Character time in microseconds</returns>
        public static double CalculateCharacterTime(int baudRate, int dataBits = 8, int stopBits = 1, bool parity = false)
        {
            if (baudRate <= 0) throw new ArgumentException("Baud rate must be positive", nameof(baudRate));
            
            int totalBits = 1 + dataBits + (parity ? 1 : 0) + stopBits; // Start + Data + Parity + Stop
            double characterTime = (double)totalBits / baudRate * 1_000_000; // Convert to microseconds
            
            return characterTime;
        }
        
        /// <summary>
        /// Calculate frame timeout (3.5 character times)
        /// </summary>
        /// <param name="baudRate">Baud rate in bps</param>
        /// <returns>Frame timeout in microseconds</returns>
        public static double CalculateFrameTimeout(int baudRate)
        {
            double charTime = CalculateCharacterTime(baudRate);
            return charTime * 3.5;
        }
        
        /// <summary>
        /// Calculate character timeout (1.5 character times)
        /// </summary>
        /// <param name="baudRate">Baud rate in bps</param>
        /// <returns>Character timeout in microseconds</returns>
        public static double CalculateCharacterTimeout(int baudRate)
        {
            double charTime = CalculateCharacterTime(baudRate);
            return charTime * 1.5;
        }
        
        #endregion
        
        #region CRC Calculations
        
        /// <summary>
        /// Calculate CRC16 for Modbus RTU
        /// </summary>
        /// <param name="data">Data bytes</param>
        /// <returns>CRC16 value</returns>
        public static ushort CalculateCRC16(byte[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            
            ushort crc = 0xFFFF;
            
            foreach (byte b in data)
            {
                crc ^= b;
                for (int i = 0; i < 8; i++)
                {
                    if ((crc & 1) != 0)
                        crc = (ushort)((crc >> 1) ^ 0xA001);
                    else
                        crc >>= 1;
                }
            }
            
            return crc;
        }
        
        /// <summary>
        /// Calculate LRC (Longitudinal Redundancy Check) for Modbus ASCII
        /// </summary>
        /// <param name="data">Data bytes</param>
        /// <returns>LRC value</returns>
        public static byte CalculateLRC(byte[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            
            byte lrc = 0;
            foreach (byte b in data)
            {
                lrc += b;
            }
            return (byte)(0x100 - lrc);
        }
        
        #endregion
        
        #region Frame Analysis
        
        /// <summary>
        /// Calculate total frame transmission time
        /// </summary>
        /// <param name="frameLength">Frame length in bytes</param>
        /// <param name="baudRate">Baud rate in bps</param>
        /// <returns>Transmission time in milliseconds</returns>
        public static double CalculateTransmissionTime(int frameLength, int baudRate)
        {
            if (frameLength <= 0) throw new ArgumentException("Frame length must be positive", nameof(frameLength));
            if (baudRate <= 0) throw new ArgumentException("Baud rate must be positive", nameof(baudRate));
            
            double charTime = CalculateCharacterTime(baudRate);
            double frameTime = charTime * frameLength / 1000.0; // Convert to milliseconds
            double gapTime = CalculateFrameTimeout(baudRate) / 1000.0; // Convert to milliseconds
            
            return frameTime + gapTime;
        }
        
        /// <summary>
        /// Calculate maximum theoretical throughput
        /// </summary>
        /// <param name="frameLength">Average frame length in bytes</param>
        /// <param name="baudRate">Baud rate in bps</param>
        /// <param name="responseTime">Device response time in milliseconds</param>
        /// <returns>Maximum frames per second</returns>
        public static double CalculateMaxThroughput(int frameLength, int baudRate, double responseTime = 0)
        {
            double transmissionTime = CalculateTransmissionTime(frameLength, baudRate);
            double totalCycleTime = transmissionTime * 2 + responseTime; // Request + Response + processing time
            
            return 1000.0 / totalCycleTime; // Frames per second
        }
        
        #endregion
        
        #region Address and Register Calculations
        
        /// <summary>
        /// Convert between different Modbus address formats
        /// </summary>
        /// <param name="address">Input address</param>
        /// <param name="fromFormat">Source format</param>
        /// <param name="toFormat">Target format</param>
        /// <returns>Converted address</returns>
        public static int ConvertAddress(int address, ModbusAddressFormat fromFormat, ModbusAddressFormat toFormat)
        {
            // Convert to wire format (0-based) first
            int wireAddress = fromFormat switch
            {
                ModbusAddressFormat.Wire => address,
                ModbusAddressFormat.Protocol => address - 1,
                ModbusAddressFormat.Schneider => address - 1,
                _ => address
            };
            
            // Convert from wire format to target format
            return toFormat switch
            {
                ModbusAddressFormat.Wire => wireAddress,
                ModbusAddressFormat.Protocol => wireAddress + 1,
                ModbusAddressFormat.Schneider => wireAddress + 1,
                _ => wireAddress
            };
        }
        
        /// <summary>
        /// Calculate register count needed for data type
        /// </summary>
        /// <param name="dataType">Data type</param>
        /// <returns>Number of 16-bit registers required</returns>
        public static int GetRegisterCount(ModbusDataType dataType)
        {
            return dataType switch
            {
                ModbusDataType.Int16 => 1,
                ModbusDataType.UInt16 => 1,
                ModbusDataType.Int32 => 2,
                ModbusDataType.UInt32 => 2,
                ModbusDataType.Float32 => 2,
                ModbusDataType.Float64 => 4,
                ModbusDataType.String => throw new ArgumentException("String length must be specified"),
                _ => 1
            };
        }
        
        #endregion
        
        #region Network Analysis
        
        /// <summary>
        /// Calculate network loading analysis
        /// </summary>
        /// <param name="devices">List of Modbus devices</param>
        /// <param name="baudRate">Network baud rate</param>
        /// <returns>Network analysis results</returns>
        public static ModbusNetworkAnalysis AnalyzeNetwork(List<ModbusDevice> devices, int baudRate)
        {
            if (devices == null) throw new ArgumentNullException(nameof(devices));
            
            double totalFrameTime = 0;
            double totalDataBytes = 0;
            double maxResponseTime = 0;
            
            foreach (var device in devices)
            {
                double deviceFrameTime = CalculateTransmissionTime(device.FrameLength, baudRate);
                totalFrameTime += deviceFrameTime * device.PollRate; // per second
                totalDataBytes += device.DataBytes * device.PollRate;
                maxResponseTime = Math.Max(maxResponseTime, device.ResponseTime);
            }
            
            double networkUtilization = totalFrameTime / 1000.0; // Convert to seconds
            double effectiveThroughput = totalDataBytes; // bytes per second
            
            return new ModbusNetworkAnalysis
            {
                NetworkUtilization = networkUtilization * 100, // Percentage
                EffectiveThroughput = effectiveThroughput,
                MaxResponseTime = maxResponseTime,
                TotalDevices = devices.Count,
                RecommendedPollRate = CalculateOptimalPollRate(devices, baudRate)
            };
        }
        
        private static double CalculateOptimalPollRate(List<ModbusDevice> devices, int baudRate)
        {
            // Calculate optimal poll rate to keep network utilization under 50%
            double totalFrameTime = devices.Sum(d => CalculateTransmissionTime(d.FrameLength, baudRate));
            double maxPollRate = 500.0 / totalFrameTime; // 50% utilization target
            
            return Math.Min(maxPollRate, 10.0); // Cap at 10 Hz
        }
        
        #endregion
        
        #region Power Calculations for RS485
        
        /// <summary>
        /// Calculate power consumption for RS485 network
        /// </summary>
        /// <param name="deviceCount">Number of devices on network</param>
        /// <param name="transmitCurrent">Transmit current per device (mA)</param>
        /// <param name="receiveCurrent">Receive current per device (mA)</param>
        /// <param name="dutyCycle">Transmission duty cycle (0-1)</param>
        /// <param name="voltage">Supply voltage (V)</param>
        /// <returns>Total network power consumption (mW)</returns>
        public static double CalculateRS485Power(int deviceCount, double transmitCurrent, 
            double receiveCurrent, double dutyCycle, double voltage = 5.0)
        {
            if (deviceCount <= 0) throw new ArgumentException("Device count must be positive", nameof(deviceCount));
            if (dutyCycle < 0 || dutyCycle > 1) throw new ArgumentException("Duty cycle must be between 0 and 1", nameof(dutyCycle));
            
            // Assume only one device transmits at a time
            double avgTransmitCurrent = transmitCurrent * dutyCycle / deviceCount;
            double avgReceiveCurrent = receiveCurrent * (1 - dutyCycle / deviceCount);
            
            double totalCurrent = deviceCount * (avgTransmitCurrent + avgReceiveCurrent);
            return totalCurrent * voltage; // Power in mW
        }
        
        #endregion
    }
    
    #region Supporting Types
    
    public enum ModbusAddressFormat
    {
        Wire,        // 0-based (actual protocol)
        Protocol,    // 1-based (common documentation)
        Schneider    // 1-based (Schneider convention)
    }
    
    public enum ModbusDataType
    {
        Int16,
        UInt16,
        Int32,
        UInt32,
        Float32,
        Float64,
        String
    }
    
    public class ModbusDevice
    {
        public int SlaveId { get; set; }
        public int FrameLength { get; set; } = 8; // Typical request frame
        public int DataBytes { get; set; } = 4;   // Typical data payload
        public double PollRate { get; set; } = 1.0; // Hz
        public double ResponseTime { get; set; } = 10.0; // ms
        public string Name { get; set; } = "";
    }
    
    public class ModbusNetworkAnalysis
    {
        public double NetworkUtilization { get; set; } // Percentage
        public double EffectiveThroughput { get; set; } // Bytes per second
        public double MaxResponseTime { get; set; } // Milliseconds
        public int TotalDevices { get; set; }
        public double RecommendedPollRate { get; set; } // Hz
    }
    
    #endregion
}
