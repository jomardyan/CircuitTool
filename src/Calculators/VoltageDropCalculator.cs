using System;

namespace CircuitTool
{
    public static class VoltageDropCalculator
    {
        // Voltage Drop: V = I × R
        public static double Calculate(double current, double resistance)
        {
            if (current < 0)
                throw new ArgumentException("Current must be non-negative.");

            if (resistance < 0)
                throw new ArgumentException("Resistance must be non-negative.");

            return current * resistance;
        }
    }
}
