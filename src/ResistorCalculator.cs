using System;
using System.Linq;

namespace CircuitTool
{
    public static class ResistorCalculator
    {
        // Ohm's Law: R = V / I
        public static double Resistance(double voltage, double current)
            => voltage / current;

        // Series: Rtotal = R1 + R2 + ...
        public static double Series(params double[] resistors)
            => resistors == null ? 0 : resistors.Sum();

        // Parallel: 1/Rtotal = 1/R1 + 1/R2 + ...
        public static double Parallel(params double[] resistors)
            => resistors == null ? 0 : 1.0 / resistors.Sum(r => 1.0 / r);
    }
}
