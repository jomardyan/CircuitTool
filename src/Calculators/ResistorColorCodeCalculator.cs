using System;
using System.Collections.Generic;
using System.Linq;

namespace CircuitTool.Calculators
{
    /// <summary>
    /// Calculator for resistor color code encoding and decoding
    /// </summary>
    public class ResistorColorCodeCalculator
    {
        /// <summary>
        /// Resistor color bands
        /// </summary>
        public enum ResistorColor
        {
            Black = 0,
            Brown = 1,
            Red = 2,
            Orange = 3,
            Yellow = 4,
            Green = 5,
            Blue = 6,
            Violet = 7,
            Grey = 8,
            White = 9,
            Gold = -1,
            Silver = -2,
            None = -3
        }

        /// <summary>
        /// Resistor tolerance values
        /// </summary>
        public enum ToleranceColor
        {
            Brown = 1,    // ±1%
            Red = 2,      // ±2%
            Green = 5,    // ±0.5%
            Blue = 25,    // ±0.25%
            Violet = 10,  // ±0.1%
            Grey = 5,     // ±0.05%
            Gold = 5,     // ±5%
            Silver = 10,  // ±10%
            None = 20     // ±20%
        }

        /// <summary>
        /// Temperature coefficient colors
        /// </summary>
        public enum TempCoeffColor
        {
            Black = 250,    // 250 ppm/°C
            Brown = 100,    // 100 ppm/°C
            Red = 50,       // 50 ppm/°C
            Orange = 15,    // 15 ppm/°C
            Yellow = 25,    // 25 ppm/°C
            Green = 20,     // 20 ppm/°C
            Blue = 10,      // 10 ppm/°C
            Violet = 5,     // 5 ppm/°C
            Grey = 1        // 1 ppm/°C
        }

        /// <summary>
        /// Result of resistor color code calculation
        /// </summary>
        public class ResistorColorResult
        {
            public double NominalValue { get; set; }        // Ohms
            public double TolerancePercent { get; set; }    // Percentage
            public double MinValue { get; set; }            // Ohms
            public double MaxValue { get; set; }            // Ohms
            public int TempCoefficient { get; set; }        // ppm/°C
            public string FormattedValue { get; set; }      // Human readable
            public int BandCount { get; set; }              // Number of bands
            public List<ResistorColor> ColorBands { get; set; } = new List<ResistorColor>();
        }

        /// <summary>
        /// Decode 4-band resistor color code
        /// </summary>
        public ResistorColorResult Decode4Band(ResistorColor band1, ResistorColor band2, 
            ResistorColor multiplier, ResistorColor tolerance)
        {
            var result = new ResistorColorResult
            {
                BandCount = 4,
                ColorBands = new List<ResistorColor> { band1, band2, multiplier, tolerance }
            };

            // Calculate nominal value
            int digit1 = (int)band1;
            int digit2 = (int)band2;
            
            if (digit1 < 0 || digit1 > 9 || digit2 < 0 || digit2 > 9)
                throw new ArgumentException("First two bands must be standard colors (0-9)");

            double baseValue = digit1 * 10 + digit2;
            double multiplierValue = CalculateMultiplier(multiplier);
            
            result.NominalValue = baseValue * multiplierValue;
            result.TolerancePercent = GetToleranceValue(tolerance);
            
            // Calculate min/max values
            double toleranceValue = result.NominalValue * (result.TolerancePercent / 100.0);
            result.MinValue = result.NominalValue - toleranceValue;
            result.MaxValue = result.NominalValue + toleranceValue;
            
            result.FormattedValue = FormatResistorValue(result.NominalValue);
            
            return result;
        }

        /// <summary>
        /// Decode 5-band resistor color code
        /// </summary>
        public ResistorColorResult Decode5Band(ResistorColor band1, ResistorColor band2, 
            ResistorColor band3, ResistorColor multiplier, ResistorColor tolerance)
        {
            var result = new ResistorColorResult
            {
                BandCount = 5,
                ColorBands = new List<ResistorColor> { band1, band2, band3, multiplier, tolerance }
            };

            // Calculate nominal value
            int digit1 = (int)band1;
            int digit2 = (int)band2;
            int digit3 = (int)band3;
            
            if (digit1 < 0 || digit1 > 9 || digit2 < 0 || digit2 > 9 || digit3 < 0 || digit3 > 9)
                throw new ArgumentException("First three bands must be standard colors (0-9)");

            double baseValue = digit1 * 100 + digit2 * 10 + digit3;
            double multiplierValue = CalculateMultiplier(multiplier);
            
            result.NominalValue = baseValue * multiplierValue;
            result.TolerancePercent = GetToleranceValue(tolerance);
            
            // Calculate min/max values
            double toleranceValue = result.NominalValue * (result.TolerancePercent / 100.0);
            result.MinValue = result.NominalValue - toleranceValue;
            result.MaxValue = result.NominalValue + toleranceValue;
            
            result.FormattedValue = FormatResistorValue(result.NominalValue);
            
            return result;
        }

        /// <summary>
        /// Decode 6-band resistor color code (includes temperature coefficient)
        /// </summary>
        public ResistorColorResult Decode6Band(ResistorColor band1, ResistorColor band2, 
            ResistorColor band3, ResistorColor multiplier, ResistorColor tolerance, 
            ResistorColor tempCoeff)
        {
            var result = Decode5Band(band1, band2, band3, multiplier, tolerance);
            result.BandCount = 6;
            result.ColorBands.Add(tempCoeff);
            result.TempCoefficient = GetTempCoefficientValue(tempCoeff);
            
            return result;
        }

        /// <summary>
        /// Encode resistance value to 4-band color code
        /// </summary>
        public List<ResistorColor> EncodeToColorCode(double resistance, double tolerancePercent = 5.0)
        {
            var colors = new List<ResistorColor>();
            
            // Find the best representation
            string resistanceStr = resistance.ToString("0");
            
            // For 4-band encoding, we need exactly 2 significant digits
            if (resistanceStr.Length >= 2)
            {
                int digit1 = int.Parse(resistanceStr[0].ToString());
                int digit2 = int.Parse(resistanceStr[1].ToString());
                
                colors.Add((ResistorColor)digit1);
                colors.Add((ResistorColor)digit2);
                
                // Calculate multiplier
                int zeros = resistanceStr.Length - 2;
                colors.Add(GetMultiplierColor(zeros));
                
                // Add tolerance
                colors.Add(GetToleranceColor(tolerancePercent));
            }
            else if (resistanceStr.Length == 1)
            {
                int digit1 = int.Parse(resistanceStr[0].ToString());
                colors.Add((ResistorColor)digit1);
                colors.Add(ResistorColor.Black);
                colors.Add(ResistorColor.Black); // x1 multiplier
                colors.Add(GetToleranceColor(tolerancePercent));
            }
            
            return colors;
        }

        /// <summary>
        /// Encode resistance value to 5-band color code for higher precision
        /// </summary>
        public List<ResistorColor> EncodeToColorCode5Band(double resistance, double tolerancePercent = 1.0)
        {
            var colors = new List<ResistorColor>();
            
            // Find the best representation with 3 significant digits
            string resistanceStr = resistance.ToString("0");
            
            if (resistanceStr.Length >= 3)
            {
                int digit1 = int.Parse(resistanceStr[0].ToString());
                int digit2 = int.Parse(resistanceStr[1].ToString());
                int digit3 = int.Parse(resistanceStr[2].ToString());
                
                colors.Add((ResistorColor)digit1);
                colors.Add((ResistorColor)digit2);
                colors.Add((ResistorColor)digit3);
                
                // Calculate multiplier
                int zeros = resistanceStr.Length - 3;
                colors.Add(GetMultiplierColor(zeros));
                
                // Add tolerance
                colors.Add(GetToleranceColor(tolerancePercent));
            }
            else
            {
                // Pad with zeros for shorter values
                string paddedStr = resistanceStr.PadRight(3, '0');
                for (int i = 0; i < 3; i++)
                {
                    colors.Add((ResistorColor)int.Parse(paddedStr[i].ToString()));
                }
                colors.Add(ResistorColor.Black); // x1 multiplier
                colors.Add(GetToleranceColor(tolerancePercent));
            }
            
            return colors;
        }

        /// <summary>
        /// Get nearest standard resistor value (E12, E24, E96 series)
        /// </summary>
        public class StandardResistorResult
        {
            public double StandardValue { get; set; }
            public double Error { get; set; }
            public double ErrorPercent { get; set; }
            public string Series { get; set; }
            public List<ResistorColor> ColorCode { get; set; }
        }

        /// <summary>
        /// Find nearest standard resistor value
        /// </summary>
        public StandardResistorResult FindNearestStandardValue(double targetValue, string series = "E12")
        {
            var standardValues = GetStandardValues(series);
            var result = new StandardResistorResult { Series = series };
            
            double bestMatch = standardValues[0];
            double minError = Math.Abs(targetValue - bestMatch);
            
            foreach (var value in standardValues)
            {
                double error = Math.Abs(targetValue - value);
                if (error < minError)
                {
                    minError = error;
                    bestMatch = value;
                }
            }
            
            result.StandardValue = bestMatch;
            result.Error = targetValue - bestMatch;
            result.ErrorPercent = (result.Error / targetValue) * 100.0;
            result.ColorCode = EncodeToColorCode(bestMatch);
            
            return result;
        }

        /// <summary>
        /// Calculate power rating required for resistor
        /// </summary>
        public class PowerRatingResult
        {
            public double CalculatedPower { get; set; }     // Watts
            public double RecommendedRating { get; set; }   // Watts
            public double SafetyFactor { get; set; }        // Multiplier
            public List<double> StandardRatings { get; set; } = new List<double>();
        }

        /// <summary>
        /// Calculate required power rating for resistor
        /// </summary>
        public PowerRatingResult CalculatePowerRating(double resistance, double voltage = 0, 
            double current = 0, double safetyFactor = 2.0)
        {
            var result = new PowerRatingResult { SafetyFactor = safetyFactor };
            
            // Calculate power using available parameters
            if (voltage > 0 && current > 0)
            {
                result.CalculatedPower = voltage * current;
            }
            else if (voltage > 0)
            {
                result.CalculatedPower = (voltage * voltage) / resistance;
            }
            else if (current > 0)
            {
                result.CalculatedPower = current * current * resistance;
            }
            else
            {
                throw new ArgumentException("Either voltage or current (or both) must be provided");
            }
            
            // Apply safety factor
            result.RecommendedRating = result.CalculatedPower * safetyFactor;
            
            // Standard power ratings
            var standardRatings = new[] { 0.125, 0.25, 0.5, 1.0, 2.0, 5.0, 10.0, 25.0, 50.0, 100.0 };
            result.StandardRatings = standardRatings.Where(r => r >= result.RecommendedRating).ToList();
            
            return result;
        }

        #region Private Helper Methods

        private double CalculateMultiplier(ResistorColor multiplier)
        {
            switch (multiplier)
            {
                case ResistorColor.Black: return 1;
                case ResistorColor.Brown: return 10;
                case ResistorColor.Red: return 100;
                case ResistorColor.Orange: return 1000;
                case ResistorColor.Yellow: return 10000;
                case ResistorColor.Green: return 100000;
                case ResistorColor.Blue: return 1000000;
                case ResistorColor.Violet: return 10000000;
                case ResistorColor.Grey: return 100000000;
                case ResistorColor.White: return 1000000000;
                case ResistorColor.Gold: return 0.1;
                case ResistorColor.Silver: return 0.01;
                default: throw new ArgumentException($"Invalid multiplier color: {multiplier}");
            }
        }

        private double GetToleranceValue(ResistorColor tolerance)
        {
            switch (tolerance)
            {
                case ResistorColor.Brown: return 1.0;
                case ResistorColor.Red: return 2.0;
                case ResistorColor.Green: return 0.5;
                case ResistorColor.Blue: return 0.25;
                case ResistorColor.Violet: return 0.1;
                case ResistorColor.Grey: return 0.05;
                case ResistorColor.Gold: return 5.0;
                case ResistorColor.Silver: return 10.0;
                case ResistorColor.None: return 20.0;
                default: throw new ArgumentException($"Invalid tolerance color: {tolerance}");
            }
        }

        private int GetTempCoefficientValue(ResistorColor tempCoeff)
        {
            switch (tempCoeff)
            {
                case ResistorColor.Black: return 250;
                case ResistorColor.Brown: return 100;
                case ResistorColor.Red: return 50;
                case ResistorColor.Orange: return 15;
                case ResistorColor.Yellow: return 25;
                case ResistorColor.Green: return 20;
                case ResistorColor.Blue: return 10;
                case ResistorColor.Violet: return 5;
                case ResistorColor.Grey: return 1;
                default: throw new ArgumentException($"Invalid temperature coefficient color: {tempCoeff}");
            }
        }

        private ResistorColor GetMultiplierColor(int zeros)
        {
            switch (zeros)
            {
                case 0: return ResistorColor.Black;
                case 1: return ResistorColor.Brown;
                case 2: return ResistorColor.Red;
                case 3: return ResistorColor.Orange;
                case 4: return ResistorColor.Yellow;
                case 5: return ResistorColor.Green;
                case 6: return ResistorColor.Blue;
                case 7: return ResistorColor.Violet;
                case 8: return ResistorColor.Grey;
                case 9: return ResistorColor.White;
                default: throw new ArgumentException($"Cannot represent {zeros} zeros as multiplier");
            }
        }

        private ResistorColor GetToleranceColor(double tolerancePercent)
        {
            if (tolerancePercent <= 0.05) return ResistorColor.Grey;
            if (tolerancePercent <= 0.1) return ResistorColor.Violet;
            if (tolerancePercent <= 0.25) return ResistorColor.Blue;
            if (tolerancePercent <= 0.5) return ResistorColor.Green;
            if (tolerancePercent <= 1.0) return ResistorColor.Brown;
            if (tolerancePercent <= 2.0) return ResistorColor.Red;
            if (tolerancePercent <= 5.0) return ResistorColor.Gold;
            if (tolerancePercent <= 10.0) return ResistorColor.Silver;
            return ResistorColor.None; // 20%
        }

        private string FormatResistorValue(double value)
        {
            if (value >= 1000000)
            {
                return $"{value / 1000000:0.##}MΩ";
            }
            else if (value >= 1000)
            {
                return $"{value / 1000:0.##}kΩ";
            }
            else
            {
                return $"{value:0.##}Ω";
            }
        }

        private double[] GetStandardValues(string series)
        {
            var baseValues = new Dictionary<string, double[]>
            {
                ["E12"] = new[] { 1.0, 1.2, 1.5, 1.8, 2.2, 2.7, 3.3, 3.9, 4.7, 5.6, 6.8, 8.2 },
                ["E24"] = new[] { 1.0, 1.1, 1.2, 1.3, 1.5, 1.6, 1.8, 2.0, 2.2, 2.4, 2.7, 3.0, 
                                 3.3, 3.6, 3.9, 4.3, 4.7, 5.1, 5.6, 6.2, 6.8, 7.5, 8.2, 9.1 },
                ["E96"] = new[] { 1.00, 1.02, 1.05, 1.07, 1.10, 1.13, 1.15, 1.18, 1.21, 1.24, 1.27, 1.30,
                                 1.33, 1.37, 1.40, 1.43, 1.47, 1.50, 1.54, 1.58, 1.62, 1.65, 1.69, 1.74,
                                 1.78, 1.82, 1.87, 1.91, 1.96, 2.00, 2.05, 2.10, 2.15, 2.21, 2.26, 2.32,
                                 2.37, 2.43, 2.49, 2.55, 2.61, 2.67, 2.74, 2.80, 2.87, 2.94, 3.01, 3.09,
                                 3.16, 3.24, 3.32, 3.40, 3.48, 3.57, 3.65, 3.74, 3.83, 3.92, 4.02, 4.12,
                                 4.22, 4.32, 4.42, 4.53, 4.64, 4.75, 4.87, 4.99, 5.11, 5.23, 5.36, 5.49,
                                 5.62, 5.76, 5.90, 6.04, 6.19, 6.34, 6.49, 6.65, 6.81, 6.98, 7.15, 7.32,
                                 7.50, 7.68, 7.87, 8.06, 8.25, 8.45, 8.66, 8.87, 9.09, 9.31, 9.53, 9.76 }
            };

            if (!baseValues.ContainsKey(series))
                throw new ArgumentException($"Unknown series: {series}");

            var values = new List<double>();
            var baseArray = baseValues[series];
            
            // Generate values for different decades
            for (int decade = -2; decade <= 6; decade++) // 0.01Ω to 10MΩ
            {
                double multiplier = Math.Pow(10, decade);
                foreach (var baseValue in baseArray)
                {
                    values.Add(baseValue * multiplier);
                }
            }
            
            return values.OrderBy(v => v).ToArray();
        }

        #endregion
    }
}
