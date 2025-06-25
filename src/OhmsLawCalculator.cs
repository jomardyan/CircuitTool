using System;

namespace CircuitTool
{
    public static class OhmsLawCalculator
    {
        // V = I * R
        public static double Voltage(double current, double resistance) => current * resistance;
        // I = V / R
        public static double Current(double voltage, double resistance) => voltage / resistance;
        // R = V / I
        public static double Resistance(double voltage, double current) => voltage / current;
    }
}
