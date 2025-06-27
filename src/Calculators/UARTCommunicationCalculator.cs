using System;
using System.Collections.Generic;
using System.Linq;

namespace CircuitTool.Calculators
{
    /// <summary>
    /// Calculator for UART communication parameters and timing analysis
    /// </summary>
    public class UARTCommunicationCalculator
    {
        /// <summary>
        /// Configuration for UART communication
        /// </summary>
        public class UARTConfig
        {
            public int BaudRate { get; set; }
            public int DataBits { get; set; } = 8;
            public ParityType Parity { get; set; } = ParityType.None;
            public StopBitsType StopBits { get; set; } = StopBitsType.One;
            public FlowControlType FlowControl { get; set; } = FlowControlType.None;
        }

        /// <summary>
        /// UART timing analysis results
        /// </summary>
        public class UARTTimingResult
        {
            public double BitTime { get; set; }  // Time per bit in microseconds
            public double CharacterTime { get; set; }  // Time per character in microseconds
            public double MaxDataRate { get; set; }  // Maximum data rate in bytes/second
            public double EfficiencyPercent { get; set; }  // Data efficiency percentage
            public double TotalBitsPerFrame { get; set; }  // Total bits per character frame
            public double InterCharacterGap { get; set; }  // Minimum gap between characters
        }

        /// <summary>
        /// UART buffer analysis results
        /// </summary>
        public class UARTBufferAnalysis
        {
            public int RecommendedBufferSize { get; set; }  // Recommended buffer size in bytes
            public double MaxLatency { get; set; }  // Maximum latency in milliseconds
            public double OverrunProbability { get; set; }  // Probability of buffer overrun
            public int MessagesPerSecond { get; set; }  // Estimated messages per second
        }

        /// <summary>
        /// Cable length analysis for UART
        /// </summary>
        public class UARTCableAnalysis
        {
            public double MaxRecommendedLength { get; set; }  // Maximum cable length in meters
            public double SignalRiseTime { get; set; }  // Signal rise time in nanoseconds
            public double MaxBaudRate { get; set; }  // Maximum recommended baud rate
            public List<string> Recommendations { get; set; } = new List<string>();
        }

        public enum ParityType
        {
            None = 0,
            Even = 1,
            Odd = 2,
            Mark = 3,
            Space = 4
        }

        public enum StopBitsType
        {
            One = 1,
            OneAndHalf = 2,
            Two = 3
        }

        public enum FlowControlType
        {
            None = 0,
            Hardware = 1,  // RTS/CTS
            Software = 2   // XON/XOFF
        }

        /// <summary>
        /// Calculate UART timing parameters
        /// </summary>
        public UARTTimingResult CalculateUARTTiming(UARTConfig config)
        {
            var result = new UARTTimingResult();

            // Calculate bit time
            result.BitTime = 1_000_000.0 / config.BaudRate;  // microseconds per bit

            // Calculate total bits per frame
            result.TotalBitsPerFrame = 1 + config.DataBits;  // Start bit + data bits

            // Add parity bit if enabled
            if (config.Parity != ParityType.None)
            {
                result.TotalBitsPerFrame++;
            }

            // Add stop bits
            switch (config.StopBits)
            {
                case StopBitsType.One:
                    result.TotalBitsPerFrame += 1;
                    break;
                case StopBitsType.OneAndHalf:
                    result.TotalBitsPerFrame += 1.5;
                    break;
                case StopBitsType.Two:
                    result.TotalBitsPerFrame += 2;
                    break;
            }

            // Calculate character time
            result.CharacterTime = result.BitTime * result.TotalBitsPerFrame;

            // Calculate maximum data rate (theoretical)
            result.MaxDataRate = 1_000_000.0 / result.CharacterTime;  // characters per second

            // Calculate efficiency (data bits vs total bits)
            result.EfficiencyPercent = (config.DataBits / result.TotalBitsPerFrame) * 100;

            // Calculate inter-character gap (minimum idle time)
            result.InterCharacterGap = result.BitTime;  // At least one bit time

            return result;
        }

        /// <summary>
        /// Analyze UART buffer requirements
        /// </summary>
        public UARTBufferAnalysis AnalyzeUARTBuffer(UARTConfig config, double dataRatePercent = 80, 
            double processingTimeMs = 1.0, int averageMessageLength = 10)
        {
            var timing = CalculateUARTTiming(config);
            var result = new UARTBufferAnalysis();

            // Calculate actual data rate
            double actualDataRate = timing.MaxDataRate * (dataRatePercent / 100.0);
            
            // Calculate messages per second
            result.MessagesPerSecond = (int)(actualDataRate / averageMessageLength);

            // Calculate bytes received during processing time
            double bytesPerProcessingCycle = (actualDataRate * processingTimeMs) / 1000.0;

            // Recommend buffer size (with safety margin)
            result.RecommendedBufferSize = Math.Max(32, (int)(bytesPerProcessingCycle * 4));

            // Calculate maximum latency
            result.MaxLatency = (result.RecommendedBufferSize / actualDataRate) * 1000.0;  // milliseconds

            // Calculate overrun probability (simplified model)
            double bufferUtilization = bytesPerProcessingCycle / result.RecommendedBufferSize;
            result.OverrunProbability = Math.Max(0, Math.Min(1, Math.Pow(bufferUtilization, 3)));

            return result;
        }

        /// <summary>
        /// Analyze cable length constraints for UART
        /// </summary>
        public UARTCableAnalysis AnalyzeCableLength(double cableLengthMeters, double cableCapacitancePfPerMeter = 100, 
            double outputResistance = 50, double inputCapacitance = 15)
        {
            var result = new UARTCableAnalysis();

            // Calculate total capacitance
            double totalCapacitance = (cableLengthMeters * cableCapacitancePfPerMeter) + inputCapacitance;  // pF

            // Calculate signal rise time (RC time constant)
            result.SignalRiseTime = 2.2 * outputResistance * (totalCapacitance / 1000.0);  // nanoseconds

            // Calculate maximum baud rate (rule of thumb: rise time < 20% of bit time)
            double maxBitTimeNs = result.SignalRiseTime * 5;  // Allow 5x rise time for clean signal
            result.MaxBaudRate = 1_000_000_000.0 / maxBitTimeNs;  // Convert ns to baud

            // Standard baud rate recommendations based on distance
            var standardBaudRates = new[] { 9600, 19200, 38400, 57600, 115200, 230400, 460800, 921600 };
            var recommendedBaudRate = standardBaudRates.Where(br => br <= result.MaxBaudRate).LastOrDefault();

            // Distance-based recommendations
            if (cableLengthMeters <= 15)
            {
                result.MaxRecommendedLength = 15;
                result.Recommendations.Add("Short distance: Up to 115200 baud recommended");
                result.Recommendations.Add("Consider higher speeds with good quality cable");
            }
            else if (cableLengthMeters <= 100)
            {
                result.MaxRecommendedLength = 100;
                result.Recommendations.Add("Medium distance: Up to 38400 baud recommended");
                result.Recommendations.Add("Use low-capacitance cable for higher speeds");
            }
            else if (cableLengthMeters <= 1000)
            {
                result.MaxRecommendedLength = 1000;
                result.Recommendations.Add("Long distance: Up to 9600 baud recommended");
                result.Recommendations.Add("Consider RS-485 for better noise immunity");
                result.Recommendations.Add("Use twisted pair cable with proper termination");
            }
            else
            {
                result.MaxRecommendedLength = 1000;
                result.Recommendations.Add("Very long distance: RS-485 strongly recommended");
                result.Recommendations.Add("UART may not be suitable for this distance");
                result.Recommendations.Add("Consider differential signaling protocols");
            }

            if (recommendedBaudRate > 0)
            {
                result.Recommendations.Add($"Maximum calculated baud rate: {recommendedBaudRate:N0} bps");
            }

            // Add noise considerations
            if (cableLengthMeters > 3)
            {
                result.Recommendations.Add("Use shielded cable to reduce EMI");
                result.Recommendations.Add("Ensure proper grounding");
            }

            return result;
        }

        /// <summary>
        /// Calculate UART error probability
        /// </summary>
        public class UARTErrorAnalysis
        {
            public double BitErrorRate { get; set; }
            public double FrameErrorRate { get; set; }
            public double CharacterErrorRate { get; set; }
            public double MessageErrorRate { get; set; }
            public List<string> ErrorSources { get; set; } = new List<string>();
        }

        /// <summary>
        /// Analyze UART error rates
        /// </summary>
        public UARTErrorAnalysis AnalyzeUARTErrors(UARTConfig config, double clockAccuracyPpm = 100, 
            double noiseLevel = 0.1, int messageLength = 10)
        {
            var result = new UARTErrorAnalysis();
            var timing = CalculateUARTTiming(config);

            // Clock accuracy error contribution
            double clockError = clockAccuracyPpm / 1_000_000.0;
            
            // Calculate cumulative timing error over a frame
            double timingErrorPerFrame = clockError * timing.TotalBitsPerFrame;
            
            // Bit error rate from timing (simplified model)
            double timingBER = Math.Min(0.1, timingErrorPerFrame * 10);

            // Noise-induced bit error rate (simplified model)
            double noiseBER = Math.Min(0.1, noiseLevel * noiseLevel);

            // Combined bit error rate
            result.BitErrorRate = timingBER + noiseBER - (timingBER * noiseBER);

            // Frame error rate (probability of any bit error in frame)
            result.FrameErrorRate = 1 - Math.Pow(1 - result.BitErrorRate, timing.TotalBitsPerFrame);

            // Character error rate (same as frame error for UART)
            result.CharacterErrorRate = result.FrameErrorRate;

            // Message error rate (probability of any character error in message)
            result.MessageErrorRate = 1 - Math.Pow(1 - result.CharacterErrorRate, messageLength);

            // Identify error sources
            if (timingBER > 0.001)
            {
                result.ErrorSources.Add($"Clock accuracy: {clockAccuracyPpm} ppm may cause timing errors");
            }

            if (noiseBER > 0.001)
            {
                result.ErrorSources.Add($"Noise level: {noiseLevel:P1} may cause signal corruption");
            }

            if (config.Parity == ParityType.None)
            {
                result.ErrorSources.Add("No parity checking - single bit errors undetected");
            }

            if (config.FlowControl == FlowControlType.None)
            {
                result.ErrorSources.Add("No flow control - buffer overruns possible");
            }

            return result;
        }

        /// <summary>
        /// Generate UART configuration recommendations
        /// </summary>
        public List<string> GenerateUARTRecommendations(UARTConfig config, double cableLength = 1.0, 
            bool criticalApplication = false, double dataRate = 1000)
        {
            var recommendations = new List<string>();
            var timing = CalculateUARTTiming(config);
            var cableAnalysis = AnalyzeCableLength(cableLength);
            var errorAnalysis = AnalyzeUARTErrors(config);

            // Baud rate recommendations
            if (config.BaudRate > cableAnalysis.MaxBaudRate)
            {
                recommendations.Add($"Reduce baud rate to {cableAnalysis.MaxBaudRate:N0} or less for cable length {cableLength}m");
            }

            // Data efficiency
            if (timing.EfficiencyPercent < 70)
            {
                recommendations.Add($"Data efficiency is {timing.EfficiencyPercent:F1}% - consider reducing overhead");
            }

            // Error detection for critical applications
            if (criticalApplication && config.Parity == ParityType.None)
            {
                recommendations.Add("Enable parity checking for critical applications");
            }

            // Flow control recommendations
            if (dataRate > timing.MaxDataRate * 0.8 && config.FlowControl == FlowControlType.None)
            {
                recommendations.Add("Enable flow control for high data rates to prevent buffer overruns");
            }

            // Long distance recommendations
            if (cableLength > 15)
            {
                recommendations.Add("Consider RS-485 for long distance communication");
                recommendations.Add("Use twisted pair cable with proper shielding");
            }

            // High error rate warnings
            if (errorAnalysis.MessageErrorRate > 0.01)  // > 1% message error rate
            {
                recommendations.Add($"High message error rate ({errorAnalysis.MessageErrorRate:P1}) - implement error recovery");
            }

            // Performance optimization
            if (config.BaudRate <= 19200 && cableLength < 10)
            {
                recommendations.Add("Consider higher baud rate for improved performance");
            }

            // Power considerations
            if (config.BaudRate > 115200)
            {
                recommendations.Add("High baud rates increase power consumption - verify power budget");
            }

            return recommendations;
        }

        /// <summary>
        /// Calculate optimal UART configuration for given requirements
        /// </summary>
        public UARTConfig OptimizeUARTConfiguration(double cableLength, double requiredDataRate, 
            bool lowPower = false, bool criticalApplication = false)
        {
            var config = new UARTConfig();

            // Start with standard parameters
            config.DataBits = 8;
            config.Parity = criticalApplication ? ParityType.Even : ParityType.None;
            config.StopBits = StopBitsType.One;
            config.FlowControl = FlowControlType.None;

            // Calculate required baud rate
            var timing = CalculateUARTTiming(config);
            double requiredBaudRate = requiredDataRate / (timing.EfficiencyPercent / 100.0);

            // Get cable constraints
            var cableAnalysis = AnalyzeCableLength(cableLength);
            
            // Select appropriate baud rate
            var standardRates = new[] { 9600, 19200, 38400, 57600, 115200, 230400, 460800, 921600 };
            
            config.BaudRate = standardRates
                .Where(rate => rate >= requiredBaudRate && rate <= cableAnalysis.MaxBaudRate)
                .FirstOrDefault();

            if (config.BaudRate == 0)
            {
                // If no standard rate works, use the maximum possible
                config.BaudRate = standardRates
                    .Where(rate => rate <= cableAnalysis.MaxBaudRate)
                    .LastOrDefault();
                
                if (config.BaudRate == 0)
                {
                    config.BaudRate = 9600;  // Fallback to lowest standard rate
                }
            }

            // Optimize for low power
            if (lowPower)
            {
                // Use lower baud rate to reduce power consumption
                var lowPowerRates = standardRates.Where(rate => rate <= 38400).ToArray();
                if (lowPowerRates.Any())
                {
                    config.BaudRate = lowPowerRates
                        .Where(rate => rate >= requiredBaudRate)
                        .FirstOrDefault();
                    
                    if (config.BaudRate == 0)
                    {
                        config.BaudRate = lowPowerRates.Last();
                    }
                }
            }

            // Enable flow control for high data rates
            if (requiredDataRate > 5000)  // More than 5KB/s
            {
                config.FlowControl = FlowControlType.Hardware;
            }

            return config;
        }
    }
}
