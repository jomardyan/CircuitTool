using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CircuitTool.Calculators
{
    /// <summary>
    /// Calculator for decoding and encoding capacitor codes (ceramic, electrolytic, tantalum)
    /// </summary>
    public class CapacitorCodeCalculator
    {
        /// <summary>
        /// Capacitor code types
        /// </summary>
        public enum CapacitorCodeType
        {
            Numeric,        // Direct value (e.g., "100nF", "22μF")
            ThreeDigit,     // 3-digit code (e.g., "104" = 100nF)
            TwoDigit,       // 2-digit code (e.g., "22" = 22pF)
            Letter,         // Letter code (e.g., "2A2" = 2.2pF)
            ColorCode,      // Color bands (older capacitors)
            Alphanumeric    // Mixed letters and numbers (e.g., "n47" = 0.47nF)
        }

        /// <summary>
        /// Capacitor voltage rating codes
        /// </summary>
        public enum VoltageCode
        {
            Z = 0,      // No voltage rating specified
            A = 10,     // 1.0V
            B = 15,     // 1.5V
            C = 25,     // 2.5V
            D = 35,     // 3.5V
            E = 50,     // 5.0V
            F = 63,     // 6.3V
            G = 100,    // 10V
            H = 160,    // 16V
            J = 250,    // 25V
            K = 350,    // 35V
            L = 500,    // 50V
            M = 630,    // 63V
            N = 1000,   // 100V
            P = 1600,   // 160V
            Q = 2500,   // 250V
            R = 3500,   // 350V
            S = 5000,   // 500V
            T = 6300,   // 630V
            U = 10000,  // 1000V
            V = 16000,  // 1600V
            W = 25000,  // 2500V
            X = 35000,  // 3500V
            Y = 50000,  // 5000V
            ZZ = 63000  // 6300V
        }

        /// <summary>
        /// Temperature coefficient codes
        /// </summary>
        public enum TempCoeffCode
        {
            C0G,    // ±30 ppm/°C (Class I)
            NP0,    // ±30 ppm/°C (Class I, same as C0G)
            X7R,    // ±15% (-55°C to +125°C) (Class II)
            X5R,    // ±15% (-55°C to +85°C) (Class II)
            Z5U,    // +22%, -56% (+10°C to +85°C) (Class II)
            Y5V,    // +22%, -82% (-30°C to +85°C) (Class II)
            X8R,    // ±15% (-55°C to +150°C) (Class II)
            X6S,    // ±22% (-55°C to +105°C) (Class II)
            X7S     // ±22% (-55°C to +125°C) (Class II)
        }

        /// <summary>
        /// Capacitor code decode result
        /// </summary>
        public class CapacitorCodeResult
        {
            public double CapacitancePicofarads { get; set; }
            public double CapacitanceMicrofarads { get; set; }
            public double CapacitanceNanofarads { get; set; }
            public string FormattedValue { get; set; }
            public CapacitorCodeType CodeType { get; set; }
            public string OriginalCode { get; set; }
            public double VoltageRating { get; set; }
            public string TempCoefficient { get; set; }
            public double TolerancePercent { get; set; }
            public string Notes { get; set; }
        }

        /// <summary>
        /// Decode capacitor code
        /// </summary>
        public CapacitorCodeResult DecodeCapacitorCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Code cannot be null or empty");

            code = code.Trim().ToUpper();
            var result = new CapacitorCodeResult
            {
                OriginalCode = code
            };

            // Try different decoding methods
            if (TryDecodeNumeric(code, result)) return result;
            if (TryDecodeThreeDigit(code, result)) return result;
            if (TryDecodeTwoDigit(code, result)) return result;
            if (TryDecodeAlphanumeric(code, result)) return result;
            if (TryDecodeLetter(code, result)) return result;

            throw new ArgumentException($"Unable to decode capacitor code: {code}");
        }

        /// <summary>
        /// Encode capacitance value to standard codes
        /// </summary>
        public class CapacitorEncodingResult
        {
            public string ThreeDigitCode { get; set; }
            public string NumericCode { get; set; }
            public string AlphanumericCode { get; set; }
            public string LetterCode { get; set; }
            public double CapacitancePicofarads { get; set; }
        }

        /// <summary>
        /// Encode capacitance to various code formats
        /// </summary>
        public CapacitorEncodingResult EncodeCapacitance(double picofarads)
        {
            var result = new CapacitorEncodingResult
            {
                CapacitancePicofarads = picofarads
            };

            // Three-digit code (most common)
            result.ThreeDigitCode = EncodeThreeDigit(picofarads);

            // Numeric codes
            if (picofarads >= 1000000) // ≥ 1μF
            {
                double microfarads = picofarads / 1000000;
                result.NumericCode = $"{microfarads:0.##}μF";
            }
            else if (picofarads >= 1000) // ≥ 1nF
            {
                double nanofarads = picofarads / 1000;
                result.NumericCode = $"{nanofarads:0.##}nF";
            }
            else
            {
                result.NumericCode = $"{picofarads:0.##}pF";
            }

            // Alphanumeric code
            result.AlphanumericCode = EncodeAlphanumeric(picofarads);

            // Letter code (for small values)
            if (picofarads < 100)
            {
                result.LetterCode = EncodeLetterCode(picofarads);
            }

            return result;
        }

        /// <summary>
        /// Decode voltage rating from letter code
        /// </summary>
        public double DecodeVoltageRating(char voltageCode)
        {
            string code = voltageCode.ToString().ToUpper();
            
            if (Enum.TryParse<VoltageCode>(code, out VoltageCode voltage))
            {
                return (double)voltage / 10.0; // Convert back to volts
            }
            
            return 0; // Unknown voltage code
        }

        /// <summary>
        /// Get temperature coefficient description
        /// </summary>
        public string GetTempCoefficientDescription(string tempCoeff)
        {
            switch (tempCoeff?.ToUpper())
            {
                case "C0G":
                case "NP0":
                    return "±30 ppm/°C, Class I ceramic, stable";
                case "X7R":
                    return "±15% (-55°C to +125°C), Class II ceramic";
                case "X5R":
                    return "±15% (-55°C to +85°C), Class II ceramic";
                case "Z5U":
                    return "+22%, -56% (+10°C to +85°C), Class II ceramic";
                case "Y5V":
                    return "+22%, -82% (-30°C to +85°C), Class II ceramic";
                case "X8R":
                    return "±15% (-55°C to +150°C), Class II ceramic";
                case "X6S":
                    return "±22% (-55°C to +105°C), Class II ceramic";
                case "X7S":
                    return "±22% (-55°C to +125°C), Class II ceramic";
                default:
                    return "Unknown temperature coefficient";
            }
        }

        /// <summary>
        /// Calculate capacitor impedance at frequency
        /// </summary>
        public class CapacitorImpedanceResult
        {
            public double Frequency { get; set; }           // Hz
            public double Reactance { get; set; }           // ohms
            public double ESR { get; set; }                 // ohms
            public double Impedance { get; set; }           // ohms
            public double Phase { get; set; }               // degrees
            public double QualityFactor { get; set; }       // dimensionless
        }

        /// <summary>
        /// Calculate capacitor impedance characteristics
        /// </summary>
        public CapacitorImpedanceResult CalculateImpedance(double capacitanceFarads, double frequency, double esr = 0)
        {
            var result = new CapacitorImpedanceResult
            {
                Frequency = frequency,
                ESR = esr
            };

            // Capacitive reactance: Xc = 1 / (2πfC)
            result.Reactance = 1.0 / (2 * Math.PI * frequency * capacitanceFarads);

            // Total impedance: Z = √(ESR² + Xc²)
            result.Impedance = Math.Sqrt(esr * esr + result.Reactance * result.Reactance);

            // Phase angle: φ = arctan(-Xc / ESR)
            if (esr > 0)
            {
                result.Phase = Math.Atan(-result.Reactance / esr) * 180.0 / Math.PI;
                result.QualityFactor = result.Reactance / esr;
            }
            else
            {
                result.Phase = -90.0; // Pure capacitive
                result.QualityFactor = double.PositiveInfinity;
            }

            return result;
        }

        #region Private Decoding Methods

        private bool TryDecodeNumeric(string code, CapacitorCodeResult result)
        {
            // Patterns like "100nF", "22μF", "4.7uF", "0.1µF"
            var numericPattern = @"^([0-9]*\.?[0-9]+)\s*(pF|nF|uF|μF|µF|mF|F)$";
            var match = Regex.Match(code, numericPattern, RegexOptions.IgnoreCase);
            
            if (match.Success)
            {
                double value = double.Parse(match.Groups[1].Value);
                string unit = match.Groups[2].Value.ToLower();
                
                result.CodeType = CapacitorCodeType.Numeric;
                
                switch (unit)
                {
                    case "pf":
                        result.CapacitancePicofarads = value;
                        break;
                    case "nf":
                        result.CapacitancePicofarads = value * 1000;
                        break;
                    case "uf":
                    case "μf":
                    case "µf":
                        result.CapacitancePicofarads = value * 1000000;
                        break;
                    case "mf":
                        result.CapacitancePicofarads = value * 1000000000;
                        break;
                    case "f":
                        result.CapacitancePicofarads = value * 1000000000000;
                        break;
                }
                
                SetDerivedValues(result);
                return true;
            }
            
            return false;
        }

        private bool TryDecodeThreeDigit(string code, CapacitorCodeResult result)
        {
            // Pattern like "104", "223", "470"
            if (Regex.IsMatch(code, @"^\d{3}$"))
            {
                int digit1 = int.Parse(code[0].ToString());
                int digit2 = int.Parse(code[1].ToString());
                int multiplier = int.Parse(code[2].ToString());
                
                result.CodeType = CapacitorCodeType.ThreeDigit;
                result.CapacitancePicofarads = (digit1 * 10 + digit2) * Math.Pow(10, multiplier);
                
                SetDerivedValues(result);
                result.Notes = "3-digit code: first two digits are significant, third is multiplier";
                return true;
            }
            
            return false;
        }

        private bool TryDecodeTwoDigit(string code, CapacitorCodeResult result)
        {
            // Pattern like "22", "47" (usually pF for small ceramic caps)
            if (Regex.IsMatch(code, @"^\d{1,2}$"))
            {
                result.CodeType = CapacitorCodeType.TwoDigit;
                result.CapacitancePicofarads = double.Parse(code);
                
                SetDerivedValues(result);
                result.Notes = "Direct value in picofarads (typical for small ceramic capacitors)";
                return true;
            }
            
            return false;
        }

        private bool TryDecodeAlphanumeric(string code, CapacitorCodeResult result)
        {
            // Patterns like "n47" (0.47nF), "4n7" (4.7nF), "p22" (0.22pF)
            var patterns = new[]
            {
                (@"^n(\d+)$", 1000.0, "0.{0}nF"),           // n47 = 0.47nF
                (@"^(\d+)n(\d*)$", 1000.0, "{0}.{1}nF"),    // 4n7 = 4.7nF
                (@"^p(\d+)$", 1.0, "0.{0}pF"),             // p22 = 0.22pF
                (@"^(\d+)p(\d*)$", 1.0, "{0}.{1}pF"),      // 4p7 = 4.7pF
                (@"^u(\d+)$", 1000000.0, "0.{0}μF"),       // u47 = 0.47μF
                (@"^(\d+)u(\d*)$", 1000000.0, "{0}.{1}μF") // 4u7 = 4.7μF
            };

            foreach (var (pattern, multiplier, format) in patterns)
            {
                var match = Regex.Match(code, pattern, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    result.CodeType = CapacitorCodeType.Alphanumeric;
                    
                    if (match.Groups.Count == 2) // Single digit after letter
                    {
                        double fractional = double.Parse(match.Groups[1].Value) / 100.0;
                        result.CapacitancePicofarads = fractional * multiplier;
                    }
                    else if (match.Groups.Count == 3) // Two parts
                    {
                        double whole = double.Parse(match.Groups[1].Value);
                        string fractionalStr = match.Groups[2].Value;
                        double fractional = string.IsNullOrEmpty(fractionalStr) ? 0 : 
                                          double.Parse(fractionalStr) / Math.Pow(10, fractionalStr.Length);
                        result.CapacitancePicofarads = (whole + fractional) * multiplier;
                    }
                    
                    SetDerivedValues(result);
                    result.Notes = "Alphanumeric code with decimal point replacement";
                    return true;
                }
            }
            
            return false;
        }

        private bool TryDecodeLetter(string code, CapacitorCodeResult result)
        {
            // Pattern like "2A2" where A represents decimal point
            var letterPattern = @"^(\d+)([A-Z])(\d+)$";
            var match = Regex.Match(code, letterPattern);
            
            if (match.Success)
            {
                int digit1 = int.Parse(match.Groups[1].Value);
                char letter = match.Groups[2].Value[0];
                int digit2 = int.Parse(match.Groups[3].Value);
                
                double multiplier = GetLetterMultiplier(letter);
                if (multiplier > 0)
                {
                    result.CodeType = CapacitorCodeType.Letter;
                    result.CapacitancePicofarads = (digit1 + digit2 / 10.0) * multiplier;
                    
                    SetDerivedValues(result);
                    result.Notes = $"Letter code: {letter} represents {multiplier}x multiplier";
                    return true;
                }
            }
            
            return false;
        }

        private double GetLetterMultiplier(char letter)
        {
            switch (letter)
            {
                case 'A': return 1.0;          // pF
                case 'B': return 10.0;         // x10 pF
                case 'C': return 100.0;        // x100 pF
                case 'D': return 1000.0;       // nF
                case 'E': return 10000.0;      // x10 nF
                case 'F': return 100000.0;     // x100 nF
                case 'G': return 1000000.0;    // μF
                default: return 0;
            }
        }

        private void SetDerivedValues(CapacitorCodeResult result)
        {
            result.CapacitanceMicrofarads = result.CapacitancePicofarads / 1000000.0;
            result.CapacitanceNanofarads = result.CapacitancePicofarads / 1000.0;
            result.FormattedValue = FormatCapacitanceValue(result.CapacitancePicofarads);
        }

        #endregion

        #region Private Encoding Methods

        private string EncodeThreeDigit(double picofarads)
        {
            if (picofarads < 10)
            {
                return $"{picofarads:0}"; // Direct value for very small capacitors
            }

            // Find the best representation
            string valueStr = picofarads.ToString("0");
            
            if (valueStr.Length <= 2)
            {
                return valueStr.PadLeft(2, '0') + "0"; // Add zero multiplier
            }
            else
            {
                // Take first two significant digits and calculate multiplier
                int firstDigit = int.Parse(valueStr[0].ToString());
                int secondDigit = int.Parse(valueStr[1].ToString());
                int multiplier = valueStr.Length - 2;
                
                return $"{firstDigit}{secondDigit}{multiplier}";
            }
        }

        private string EncodeAlphanumeric(double picofarads)
        {
            if (picofarads >= 1000000) // μF range
            {
                double microfarads = picofarads / 1000000.0;
                if (microfarads < 1)
                {
                    int fraction = (int)(microfarads * 100);
                    return $"u{fraction:00}";
                }
                else
                {
                    return $"{microfarads:0.#}uF";
                }
            }
            else if (picofarads >= 1000) // nF range
            {
                double nanofarads = picofarads / 1000.0;
                if (nanofarads < 1)
                {
                    int fraction = (int)(nanofarads * 100);
                    return $"n{fraction:00}";
                }
                else
                {
                    return $"{nanofarads:0.#}nF";
                }
            }
            else // pF range
            {
                if (picofarads < 1)
                {
                    int fraction = (int)(picofarads * 100);
                    return $"p{fraction:00}";
                }
                else
                {
                    return $"{picofarads:0.#}pF";
                }
            }
        }

        private string EncodeLetterCode(double picofarads)
        {
            // For very small values, use letter code format
            if (picofarads < 10)
            {
                int whole = (int)picofarads;
                int fraction = (int)((picofarads - whole) * 10);
                return $"{whole}A{fraction}"; // A = 1x multiplier
            }
            
            return null; // Not suitable for letter code
        }

        private string FormatCapacitanceValue(double picofarads)
        {
            if (picofarads >= 1000000) // μF range
            {
                double microfarads = picofarads / 1000000.0;
                return $"{microfarads:0.###} μF";
            }
            else if (picofarads >= 1000) // nF range
            {
                double nanofarads = picofarads / 1000.0;
                return $"{nanofarads:0.###} nF";
            }
            else // pF range
            {
                return $"{picofarads:0.###} pF";
            }
        }

        #endregion
    }
}
