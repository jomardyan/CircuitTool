using System;

namespace CircuitTool
{
    public static class OhmsLawCalculator
    {
        // V = I * R
        public static double Voltage(double current, double resistance) 
        {
            return current * resistance;
        }
        // I = V / R
        public static double Current(double voltage, double resistance) 
        {
            return voltage / resistance;
        }
        // R = V / I
        public static double Resistance(double voltage, double current) 
        {
            return voltage / current;
        }
    }
}
