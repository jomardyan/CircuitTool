using System;

namespace CircuitTool
{
    public static class PowerCalculator
    {
        // P = V * I
        public static double Power(double voltage, double current) => voltage * current;
        // P = I^2 * R
        public static double PowerFromCurrentResistance(double current, double resistance) => current * current * resistance;
        // P = V^2 / R
        public static double PowerFromVoltageResistance(double voltage, double resistance) => (voltage * voltage) / resistance;
    }
}
