using System;

namespace CircuitTool
{
    public static class VoltageDividerCalculator
    {
        // Voltage Divider: Vout = Vin × (R2 / (R1 + R2))
        public static double Calculate(double vin, double r1, double r2)
        {
            if (r1 < 0 || r2 < 0)
                throw new ArgumentException("Resistances must be non-negative.");

            double totalResistance = r1 + r2;
            if (totalResistance == 0)
                throw new ArgumentException("Total resistance cannot be zero.");

            return vin * (r2 / totalResistance);
        }
    }
}
