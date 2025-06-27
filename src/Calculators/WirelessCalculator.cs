using System;

namespace CircuitTool
{
    /// <summary>
    /// Provides calculations for wireless communication systems and protocols.
    /// </summary>
    /// <remarks>
    /// <para>Example usage:</para>
    /// <code>
    /// double linkBudget = WirelessCalculator.LinkBudget(20, -90, 5); // Link budget calculation
    /// double bitrate = WirelessCalculator.ShannonCapacity(1e6, 20); // Shannon capacity
    /// double ber = WirelessCalculator.BERFromEbN0(10, ModulationType.BPSK); // Bit error rate
    /// </code>
    /// </remarks>
    public static class WirelessCalculator
    {
        /// <summary>
        /// Modulation types for BER calculations.
        /// </summary>
        public enum ModulationType
        {
            BPSK,
            QPSK,
            QAM16,
            QAM64,
            QAM256
        }

        /// <summary>
        /// Calculates link budget for wireless communication.
        /// </summary>
        /// <param name="transmitPowerDbm">Transmit power in dBm.</param>
        /// <param name="receiverSensitivityDbm">Receiver sensitivity in dBm.</param>
        /// <param name="systemMarginDb">System margin in dB.</param>
        /// <returns>Available path loss budget in dB.</returns>
        /// <example>
        /// double linkBudget = WirelessCalculator.LinkBudget(20, -90, 5); // Link budget calculation
        /// </example>
        public static double LinkBudget(double transmitPowerDbm, double receiverSensitivityDbm, double systemMarginDb)
        {
            if (systemMarginDb < 0)
                throw new ArgumentException("System margin must be non-negative.");
            
            return transmitPowerDbm - receiverSensitivityDbm - systemMarginDb;
        }

        /// <summary>
        /// Calculates Shannon channel capacity.
        /// </summary>
        /// <param name="bandwidth">Channel bandwidth in Hz.</param>
        /// <param name="snrDb">Signal-to-noise ratio in dB.</param>
        /// <returns>Maximum data rate in bits per second.</returns>
        /// <example>
        /// double bitrate = WirelessCalculator.ShannonCapacity(1e6, 20); // Shannon capacity
        /// </example>
        public static double ShannonCapacity(double bandwidth, double snrDb)
        {
            if (bandwidth <= 0)
                throw new ArgumentException("Bandwidth must be positive.");
            
            double snrLinear = Math.Pow(10, snrDb / 10);
            return bandwidth * (Math.Log(1 + snrLinear) / Math.Log(2));
        }

        /// <summary>
        /// Calculates bit error rate (BER) from Eb/N0.
        /// </summary>
        /// <param name="ebN0Db">Energy per bit to noise spectral density ratio in dB.</param>
        /// <param name="modulation">Modulation type.</param>
        /// <returns>Bit error rate.</returns>
        /// <example>
        /// double ber = WirelessCalculator.BERFromEbN0(10, ModulationType.BPSK); // Bit error rate
        /// </example>
        public static double BERFromEbN0(double ebN0Db, ModulationType modulation)
        {
            double ebN0Linear = Math.Pow(10, ebN0Db / 10);
            
            switch (modulation)
            {
                case ModulationType.BPSK:
                    return 0.5 * Erfc(Math.Sqrt(ebN0Linear));
                
                case ModulationType.QPSK:
                    return 0.5 * Erfc(Math.Sqrt(ebN0Linear));
                
                case ModulationType.QAM16:
                    return 0.75 * Erfc(Math.Sqrt(0.4 * ebN0Linear));
                
                case ModulationType.QAM64:
                    return (7.0/12.0) * Erfc(Math.Sqrt(ebN0Linear / 21));
                
                case ModulationType.QAM256:
                    return (15.0/32.0) * Erfc(Math.Sqrt(ebN0Linear / 85));
                
                default:
                    throw new ArgumentException("Unsupported modulation type.");
            }
        }

        /// <summary>
        /// Calculates required Eb/N0 for target BER.
        /// </summary>
        /// <param name="targetBer">Target bit error rate.</param>
        /// <param name="modulation">Modulation type.</param>
        /// <returns>Required Eb/N0 in dB.</returns>
        public static double EbN0FromBER(double targetBer, ModulationType modulation)
        {
            if (targetBer <= 0 || targetBer >= 1)
                throw new ArgumentException("Target BER must be between 0 and 1.");
            
            // Simplified approximations for common modulations
            switch (modulation)
            {
                case ModulationType.BPSK:
                case ModulationType.QPSK:
                    return 10 * Math.Log10(Math.Pow(InverseErfc(2 * targetBer), 2));
                
                default:
                    throw new NotImplementedException("EbN0 calculation from BER not implemented for this modulation.");
            }
        }

        /// <summary>
        /// Calculates antenna gain required for a given EIRP.
        /// </summary>
        /// <param name="eirpDbm">Effective Isotropic Radiated Power in dBm.</param>
        /// <param name="transmitPowerDbm">Transmit power in dBm.</param>
        /// <returns>Required antenna gain in dBi.</returns>
        public static double AntennaGainForEIRP(double eirpDbm, double transmitPowerDbm)
        {
            return eirpDbm - transmitPowerDbm;
        }

        /// <summary>
        /// Calculates received signal strength (RSS) using Friis equation.
        /// </summary>
        /// <param name="transmitPowerDbm">Transmit power in dBm.</param>
        /// <param name="transmitGainDbi">Transmit antenna gain in dBi.</param>
        /// <param name="receiveGainDbi">Receive antenna gain in dBi.</param>
        /// <param name="frequency">Frequency in Hz.</param>
        /// <param name="distance">Distance in meters.</param>
        /// <returns>Received signal strength in dBm.</returns>
        public static double ReceivedSignalStrength(double transmitPowerDbm, double transmitGainDbi, 
                                                   double receiveGainDbi, double frequency, double distance)
        {
            if (frequency <= 0 || distance <= 0)
                throw new ArgumentException("Frequency and distance must be positive.");
            
            double wavelength = 299792458.0 / frequency; // c/f
            double pathLossDb = 20 * Math.Log10(4 * Math.PI * distance / wavelength);
            
            return transmitPowerDbm + transmitGainDbi + receiveGainDbi - pathLossDb;
        }

        /// <summary>
        /// Calculates data rate from symbol rate and modulation order.
        /// </summary>
        /// <param name="symbolRate">Symbol rate in symbols per second.</param>
        /// <param name="bitsPerSymbol">Bits per symbol (log2 of modulation order).</param>
        /// <returns>Data rate in bits per second.</returns>
        public static double DataRate(double symbolRate, int bitsPerSymbol)
        {
            if (symbolRate <= 0 || bitsPerSymbol <= 0)
                throw new ArgumentException("Symbol rate and bits per symbol must be positive.");
            
            return symbolRate * bitsPerSymbol;
        }

        /// <summary>
        /// Calculates spectral efficiency.
        /// </summary>
        /// <param name="dataRate">Data rate in bits per second.</param>
        /// <param name="bandwidth">Bandwidth in Hz.</param>
        /// <returns>Spectral efficiency in bits per second per Hz.</returns>
        public static double SpectralEfficiency(double dataRate, double bandwidth)
        {
            if (dataRate < 0 || bandwidth <= 0)
                throw new ArgumentException("Data rate must be non-negative and bandwidth must be positive.");
            
            return dataRate / bandwidth;
        }

        /// <summary>
        /// Calculates processing gain for spread spectrum systems.
        /// </summary>
        /// <param name="chipRate">Chip rate in chips per second.</param>
        /// <param name="dataRate">Data rate in bits per second.</param>
        /// <returns>Processing gain in dB.</returns>
        public static double ProcessingGain(double chipRate, double dataRate)
        {
            if (chipRate <= 0 || dataRate <= 0)
                throw new ArgumentException("Chip rate and data rate must be positive.");
            if (chipRate < dataRate)
                throw new ArgumentException("Chip rate must be greater than or equal to data rate.");
            
            return 10 * Math.Log10(chipRate / dataRate);
        }

        /// <summary>
        /// Calculates fade margin for wireless link.
        /// </summary>
        /// <param name="linkMargin">Available link margin in dB.</param>
        /// <param name="requiredAvailability">Required availability percentage (e.g., 99.9).</param>
        /// <param name="frequency">Frequency in GHz.</param>
        /// <param name="distance">Distance in km.</param>
        /// <returns>Fade margin in dB.</returns>
        public static double FadeMargin(double linkMargin, double requiredAvailability, double frequency, double distance)
        {
            if (requiredAvailability <= 0 || requiredAvailability >= 100)
                throw new ArgumentException("Required availability must be between 0 and 100 percent.");
            if (frequency <= 0 || distance <= 0)
                throw new ArgumentException("Frequency and distance must be positive.");
            
            // Simplified Rayleigh fading model
            double unavailability = 100 - requiredAvailability;
            double fadeDepth = -10 * Math.Log10(unavailability / 100);
            
            return Math.Min(linkMargin, fadeDepth);
        }

        /// <summary>
        /// Calculates VSWR from return loss.
        /// </summary>
        /// <param name="returnLossDb">Return loss in dB.</param>
        /// <returns>VSWR value.</returns>
        public static double VSWRFromReturnLoss(double returnLossDb)
        {
            if (returnLossDb < 0)
                throw new ArgumentException("Return loss must be non-negative (positive dB value).");
            
            double reflectionCoeff = Math.Pow(10, -returnLossDb / 20);
            return (1 + reflectionCoeff) / (1 - reflectionCoeff);
        }

        /// <summary>
        /// Complementary error function approximation.
        /// </summary>
        /// <param name="x">Input value.</param>
        /// <returns>Complementary error function value.</returns>
        private static double Erfc(double x)
        {
            // Approximation for complementary error function
            if (x < 0) return 2 - Erfc(-x);
            
            double a1 = 0.254829592;
            double a2 = -0.284496736;
            double a3 = 1.421413741;
            double a4 = -1.453152027;
            double a5 = 1.061405429;
            double p = 0.3275911;
            
            double t = 1.0 / (1.0 + p * x);
            double y = 1.0 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t * Math.Exp(-x * x);
            
            return y;
        }

        /// <summary>
        /// Inverse complementary error function approximation.
        /// </summary>
        /// <param name="y">Input value (0 &lt; y &lt; 2).</param>
        /// <returns>Inverse complementary error function value.</returns>
        private static double InverseErfc(double y)
        {
            if (y <= 0 || y >= 2)
                throw new ArgumentException("Input must be between 0 and 2.");
            
            // Simplified approximation for inverse erfc
            if (y > 1) return -InverseErfc(2 - y);
            
            double w = -Math.Log(y * (2 - y));
            double x = Math.Sqrt(w);
            
            return x; // Simplified approximation
        }
    }
}
