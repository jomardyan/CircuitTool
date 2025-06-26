using System;

namespace CircuitTool
{
    public static class EnergyCalculator
    {
        // Energy (Joules) = Power (Watts) * Time (Seconds)
        public static double Joules(double power, double timeSeconds) => power * timeSeconds;
        // kWh = (Watts * hours) / 1000
        public static double KWh(double watts, double hours) => (watts * hours) / 1000.0;
        // Cost = kWh * rate
        public static double EnergyCost(double kWh, double ratePerKWh) => kWh * ratePerKWh;
    }
}
