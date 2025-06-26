using System;
#if NET20 || NET35 || NET40
using System.Collections.Generic;
#endif

namespace CircuitTool
{
    /// <summary>
    /// Popular calculators for electronics beginners
    /// </summary>
    public static class BeginnerCalculators
    {
        /// <summary>
        /// Calculates battery life for simple circuits
        /// </summary>
        /// <param name="batteryCapacity">Battery capacity in mAh</param>
        /// <param name="loadCurrent">Load current in mA</param>
        /// <returns>Battery life in hours</returns>
        public static double BatteryLifeCalculator(double batteryCapacity, double loadCurrent)
        {
            if (loadCurrent <= 0)
                throw new ArgumentException("Load current must be greater than zero");
            
            return batteryCapacity / loadCurrent;
        }

        /// <summary>
        /// Calculates wire gauge (AWG) needed for a given current
        /// </summary>
        /// <param name="current">Current in Amperes</param>
        /// <param name="safetyFactor">Safety factor (default 1.5)</param>
        /// <returns>Recommended AWG wire gauge</returns>
        public static int WireGaugeCalculator(double current, double safetyFactor = 1.5)
        {
            double adjustedCurrent = current * safetyFactor;
            
            // Standard AWG current ratings (approximate)
            if (adjustedCurrent <= 0.5) return 30;
            if (adjustedCurrent <= 0.75) return 28;
            if (adjustedCurrent <= 1.0) return 26;
            if (adjustedCurrent <= 1.5) return 24;
            if (adjustedCurrent <= 2.0) return 22;
            if (adjustedCurrent <= 3.0) return 20;
            if (adjustedCurrent <= 5.0) return 18;
            if (adjustedCurrent <= 7.0) return 16;
            if (adjustedCurrent <= 10.0) return 14;
            if (adjustedCurrent <= 15.0) return 12;
            if (adjustedCurrent <= 20.0) return 10;
            if (adjustedCurrent <= 30.0) return 8;
            if (adjustedCurrent <= 40.0) return 6;
            if (adjustedCurrent <= 55.0) return 4;
            if (adjustedCurrent <= 75.0) return 2;
            if (adjustedCurrent <= 95.0) return 1;
            if (adjustedCurrent <= 125.0) return 0;
            
            return -1; // Use professional calculation for higher currents
        }

        /// <summary>
        /// Calculates the number of turns for a basic inductor
        /// </summary>
        /// <param name="inductance">Desired inductance in microhenries (µH)</param>
        /// <param name="coreDiameter">Core diameter in mm</param>
        /// <param name="coreLength">Core length in mm</param>
        /// <param name="permeability">Core permeability (default 1 for air core)</param>
        /// <returns>Number of turns needed</returns>
        public static int InductorTurnsCalculator(double inductance, double coreDiameter, double coreLength, double permeability = 1)
        {
            // Wheeler's formula for single-layer air core inductors
            // L = (d^2 * n^2) / (18d + 40l) where L is in µH, d in inches, l in inches
            double diameterInches = coreDiameter / 25.4;
            double lengthInches = coreLength / 25.4;
            
            double turns = Math.Sqrt((inductance * (18 * diameterInches + 40 * lengthInches)) / (Math.Pow(diameterInches, 2) * permeability));
            
            return (int)Math.Ceiling(turns);
        }

        /// <summary>
        /// Calculates capacitor value for RC time constant
        /// </summary>
        /// <param name="resistance">Resistance in Ohms</param>
        /// <param name="timeConstant">Desired time constant in seconds</param>
        /// <returns>Capacitor value in Farads</returns>
        public static double RCTimeConstantCapacitor(double resistance, double timeConstant)
        {
            if (resistance <= 0)
                throw new ArgumentException("Resistance must be greater than zero");
            
            return timeConstant / resistance;
        }

        /// <summary>
        /// Calculates frequency for RC oscillator circuit
        /// </summary>
        /// <param name="resistance">Resistance in Ohms</param>
        /// <param name="capacitance">Capacitance in Farads</param>
        /// <returns>Oscillation frequency in Hz</returns>
        public static double RCOscillatorFrequency(double resistance, double capacitance)
        {
            if (resistance <= 0 || capacitance <= 0)
                throw new ArgumentException("Resistance and capacitance must be greater than zero");
            
            // Approximate frequency for RC relaxation oscillator
            return 1.0 / (2.2 * resistance * capacitance);
        }

        /// <summary>
        /// Calculates decibel (dB) from power ratio
        /// </summary>
        /// <param name="powerRatio">Power ratio (Pout/Pin)</param>
        /// <returns>Decibel value</returns>
        public static double PowerRatioToDecibels(double powerRatio)
        {
            if (powerRatio <= 0)
                throw new ArgumentException("Power ratio must be greater than zero");
            
            return 10 * Math.Log10(powerRatio);
        }

        /// <summary>
        /// Calculates decibel (dB) from voltage ratio
        /// </summary>
        /// <param name="voltageRatio">Voltage ratio (Vout/Vin)</param>
        /// <returns>Decibel value</returns>
        public static double VoltageRatioToDecibels(double voltageRatio)
        {
            if (voltageRatio <= 0)
                throw new ArgumentException("Voltage ratio must be greater than zero");
            
            return 20 * Math.Log10(voltageRatio);
        }

        /// <summary>
        /// Calculates transformer turns ratio
        /// </summary>
        /// <param name="primaryVoltage">Primary voltage</param>
        /// <param name="secondaryVoltage">Secondary voltage</param>
        /// <returns>Turns ratio (Ns/Np)</returns>
        public static double TransformerTurnsRatio(double primaryVoltage, double secondaryVoltage)
        {
            if (primaryVoltage <= 0)
                throw new ArgumentException("Primary voltage must be greater than zero");
            
            return secondaryVoltage / primaryVoltage;
        }
    }
}
