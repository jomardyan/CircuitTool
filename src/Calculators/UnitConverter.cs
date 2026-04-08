using System;

namespace CircuitTool
{
    public static class UnitConverter
    {
        // Amps to kW: kW = (A × V × PF) / 1000
        public static double AmpsToKW(double amps, double volts, double powerFactor = 1.0) => (amps * volts * powerFactor) / 1000.0;
        // Amps to kVA: kVA = (A × V) / 1000
        public static double AmpsToKVA(double amps, double volts) => (amps * volts) / 1000.0;
        // Amps to VA: VA = A × V
        public static double AmpsToVA(double amps, double volts) => amps * volts;
        // Amps to Volts: V = W / A
        public static double AmpsToVolts(double watts, double amps) => watts / amps;
        // Amps to Watts: W = A × V
        public static double AmpsToWatts(double amps, double volts) => amps * volts;
        // kVA to Amps: A = (kVA × 1000) / V
        public static double KVAToAmps(double kVA, double volts) => (kVA * 1000.0) / volts;
        // kVA to Watts: W = kVA × 1000 × PF
        public static double KVAToWatts(double kVA, double powerFactor = 1.0) => kVA * 1000.0 * powerFactor;
        // kVA to kW: kW = kVA × PF
        public static double KVAToKW(double kVA, double powerFactor = 1.0) => kVA * powerFactor;
        // kVA to VA: VA = kVA × 1000
        public static double KVAToVA(double kVA) => kVA * 1000.0;
        // kW to Amps: A = (kW × 1000) / (V × PF)
        public static double KWToAmps(double kW, double volts, double powerFactor = 1.0) => (kW * 1000.0) / (volts * powerFactor);
        // kW to Volts: V = (kW × 1000) / (A × PF)
        public static double KWToVolts(double kW, double amps, double powerFactor = 1.0) => (kW * 1000.0) / (amps * powerFactor);
        // kW to kWh: kWh = kW × hours
        public static double KWToKWh(double kW, double hours) => kW * hours;
        // kW to VA: VA = kW × 1000 / PF
        public static double KWToVA(double kW, double powerFactor = 1.0) => (kW * 1000.0) / powerFactor;
        // kW to kVA: kVA = kW / PF
        public static double KWToKVA(double kW, double powerFactor = 1.0) => kW / powerFactor;
        // kWh to kW: kW = kWh / hours
        public static double KWhToKW(double kWh, double hours) => kWh / hours;
        // kWh to Watts: W = kWh × 1000 / hours
        public static double KWhToWatts(double kWh, double hours) => (kWh * 1000.0) / hours;
        // VA to Amps: A = VA / V
        public static double VAToAmps(double va, double volts) => va / volts;
        // VA to Watts: W = VA × PF
        public static double VAToWatts(double va, double powerFactor = 1.0) => va * powerFactor;
        // VA to kW: kW = (VA × PF) / 1000
        public static double VAToKW(double va, double powerFactor = 1.0) => (va * powerFactor) / 1000.0;
        // VA to kVA: kVA = VA / 1000
        public static double VAToKVA(double va) => va / 1000.0;
        // Volts to Amps: A = W / V
        public static double VoltsToAmps(double watts, double volts) => watts / volts;
        // Volts to Watts: W = V × A
        public static double VoltsToWatts(double volts, double amps) => volts * amps;
        // Volts to kW: kW = (V × A × PF) / 1000
        public static double VoltsToKW(double volts, double amps, double powerFactor = 1.0) => (volts * amps * powerFactor) / 1000.0;
        // Volts to Joules: J = V × C
        public static double VoltsToJoules(double volts, double coulombs) => volts * coulombs;
        // Volts to eV: eV = V × 1.602176634e-19
        public static double VoltsToEV(double volts) => volts * 1.602176634e-19;
        // Watts to Amps: A = W / V
        public static double WattsToAmps(double watts, double volts) => watts / volts;
        // Watts to Joules: J = W × s
        public static double WattsToJoules(double watts, double seconds) => watts * seconds;
        // Watts to kWh: kWh = W × hours / 1000
        public static double WattsToKWh(double watts, double hours) => (watts * hours) / 1000.0;
        // Watts to Volts: V = W / A
        public static double WattsToVolts(double watts, double amps) => watts / amps;
        // Watts to VA: VA = W / PF
        public static double WattsToVA(double watts, double powerFactor = 1.0) => watts / powerFactor;
        // Watts to kVA: kVA = W / (1000 × PF)
        public static double WattsToKVA(double watts, double powerFactor = 1.0) => watts / (1000.0 * powerFactor);
        // eV to Volts: V = eV / 1.602176634e-19
        public static double EVToVolts(double eV) => eV / 1.602176634e-19;
        // Joules to Watts: W = J / s
        public static double JoulesToWatts(double joules, double seconds) => joules / seconds;
        // Joules to Volts: V = J / C
        public static double JoulesToVolts(double joules, double coulombs) => joules / coulombs;
    }
}
