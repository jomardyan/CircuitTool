using System;

namespace CircuitTool
{
    public static class VoltageDropCalculator
    {
        // Voltage Drop: V = I × R
        public static double Calculate(double current, double resistance) => current * resistance;
    }
}
