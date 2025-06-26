using System;
#if !NET20
using System.Linq;
#endif

namespace CircuitTool
{
    public static class ResistorCalculator
    {
        // Ohm's Law: R = V / I
        public static double Resistance(double voltage, double current)
        {
            return voltage / current;
        }

        // Series: Rtotal = R1 + R2 + ...
        public static double Series(params double[] resistors)
        {
            if (resistors == null) return 0;

#if NET20
            double total = 0;
            foreach (double resistor in resistors)
            {
                total += resistor;
            }
            return total;
#else
            return resistors.Sum();
#endif
        }

        // Parallel: 1/Rtotal = 1/R1 + 1/R2 + ...
        public static double Parallel(params double[] resistors)
        {
            if (resistors == null) return 0;

#if NET20
            double reciprocalSum = 0;
            foreach (double resistor in resistors)
            {
                reciprocalSum += 1.0 / resistor;
            }
            return 1.0 / reciprocalSum;
#else
            return 1.0 / resistors.Sum(r => 1.0 / r);
#endif
        }
    }
}
