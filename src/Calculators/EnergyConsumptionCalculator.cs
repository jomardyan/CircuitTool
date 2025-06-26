using System;

namespace CircuitTool
{
    public static class EnergyConsumptionCalculator
    {
        // Energy Consumption (kWh) = Power (W) × Time (h) / 1000
        public static double ConsumptionKWh(double powerWatts, double timeHours) => (powerWatts * timeHours) / 1000.0;
    }
}
