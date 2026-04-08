namespace CircuitTool
{
    public static class VoltageCalculator
    {
        // Voltage Drop: V = I * R
        public static double VoltageDrop(double current, double resistance)
        {
            return VoltageDropCalculator.Calculate(current, resistance);
        }

        // Voltage Divider: Vout = Vin * (R2 / (R1 + R2))
        public static double VoltageDivider(double vin, double r1, double r2)
        {
            return VoltageDividerCalculator.Calculate(vin, r1, r2);
        }
    }
}
