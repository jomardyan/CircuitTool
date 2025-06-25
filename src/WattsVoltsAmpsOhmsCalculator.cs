using System;

namespace CircuitTool
{
    public static class WattsVoltsAmpsOhmsCalculator
    {
        // Watts = Volts * Amps
        public static double Watts(double volts, double amps) => volts * amps;
        // Volts = Watts / Amps
        public static double Volts(double watts, double amps) => watts / amps;
        // Amps = Watts / Volts
        public static double Amps(double watts, double volts) => watts / volts;
        // Ohms = Volts / Amps
        public static double Ohms(double volts, double amps) => volts / amps;
    }
}
