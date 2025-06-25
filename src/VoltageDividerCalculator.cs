using System;

namespace CircuitTool
{
    public static class VoltageDividerCalculator
    {
        // Voltage Divider: Vout = Vin × (R2 / (R1 + R2))
        public static double Calculate(double vin, double r1, double r2) => vin * (r2 / (r1 + r2));
    }
}
