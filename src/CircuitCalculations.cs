using System;

namespace CircuitTool
{
    public static class CircuitCalculations
    {
        // Add any general circuit calculation methods here in the future

        public static double CalculateTotalResistance(double[] resistances, bool isSeries)
        {
            if (isSeries)
            {
                // Series: Sum of all resistances
                double totalResistance = 0;
                foreach (var resistance in resistances)
                {
                    totalResistance += resistance;
                }
                return totalResistance;
            }
            else
            {
                // Parallel: Reciprocal of the sum of reciprocals
                double reciprocalSum = 0;
                foreach (var resistance in resistances)
                {
                    reciprocalSum += 1 / resistance;
                }
                return 1 / reciprocalSum;
            }
        }

        public static double CalculatePower(double voltage, double current)
        {
            // Power = Voltage × Current
            return voltage * current;
        }

        public static double CalculateEnergy(double power, double time)
        {
            // Energy = Power × Time
            return power * time;
        }
    }
}
