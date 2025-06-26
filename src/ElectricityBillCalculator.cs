using System;


namespace CircuitTool
{
    /// <summary>
    /// Provides a simple method to calculate electricity bills.
    /// </summary>
    /// <remarks>
    /// <para>Example usage:</para>
    /// <code>
    /// double bill = ElectricityBillCalculator.CalculateBill(150, 0.12); // $18.00
    /// </code>
    /// </remarks>
    public static class ElectricityBillCalculator
    {
        /// <summary>
        /// Calculates the total electricity bill.
        /// </summary>
        /// <param name="kWh">The total energy consumed in kilowatt-hours (kWh).</param>
        /// <param name="ratePerKWh">The rate per kilowatt-hour (e.g., 0.12 for $0.12/kWh).</param>
        /// <returns>The total bill amount.</returns>
        public static double CalculateBill(double kWh, double ratePerKWh) => kWh * ratePerKWh;
    }
}
