using System;

namespace CircuitTool
{
    public static class ElectricityBillCalculator
    {
        // Calculate electricity bill: bill = kWh * rate
        public static double CalculateBill(double kWh, double ratePerKWh) => kWh * ratePerKWh;
    }
}
