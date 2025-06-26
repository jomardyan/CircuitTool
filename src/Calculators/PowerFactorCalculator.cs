using System;

namespace CircuitTool
{
    public static class PowerFactorCalculator
    {
        // Power Factor = Real Power (W) / Apparent Power (VA)
        public static double PowerFactor(double realPowerWatts, double apparentPowerVA) => realPowerWatts / apparentPowerVA;
    }
}
