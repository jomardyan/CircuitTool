using System;

namespace CircuitTool.Units
{
    /// <summary>
    /// Represents voltage measurement units
    /// </summary>
    public enum VoltageUnit
    {
        /// <summary>Nanovolts (nV)</summary>
        Nanovolts,
        /// <summary>Microvolts (μV)</summary>
        Microvolts,
        /// <summary>Millivolts (mV)</summary>
        Millivolts,
        /// <summary>Volts (V)</summary>
        Volts,
        /// <summary>Kilovolts (kV)</summary>
        Kilovolts,
        /// <summary>Megavolts (MV)</summary>
        Megavolts
    }

    /// <summary>
    /// Represents a strongly-typed voltage measurement with automatic unit conversion
    /// </summary>
    public readonly struct Voltage : IEquatable<Voltage>, IComparable<Voltage>
    {
        private readonly double _volts;

        /// <summary>
        /// Creates a new voltage measurement
        /// </summary>
        /// <param name="value">The value in the specified unit</param>
        /// <param name="unit">The unit of measurement</param>
        public Voltage(double value, VoltageUnit unit)
        {
            _volts = ConvertToVolts(value, unit);
        }

        /// <summary>
        /// Gets the voltage value in volts
        /// </summary>
        public double Volts => _volts;

        /// <summary>
        /// Gets the voltage value in millivolts
        /// </summary>
        public double Millivolts => _volts * 1000;

        /// <summary>
        /// Gets the voltage value in kilovolts
        /// </summary>
        public double Kilovolts => _volts / 1000;

        /// <summary>
        /// Gets the voltage value in the specified unit
        /// </summary>
        /// <param name="unit">The desired unit</param>
        /// <returns>The voltage value in the specified unit</returns>
        public double GetValue(VoltageUnit unit)
        {
            return unit switch
            {
                VoltageUnit.Nanovolts => _volts * 1e9,
                VoltageUnit.Microvolts => _volts * 1e6,
                VoltageUnit.Millivolts => _volts * 1000,
                VoltageUnit.Volts => _volts,
                VoltageUnit.Kilovolts => _volts / 1000,
                VoltageUnit.Megavolts => _volts / 1e6,
                _ => throw new ArgumentException("Invalid voltage unit")
            };
        }

        private static double ConvertToVolts(double value, VoltageUnit unit)
        {
            return unit switch
            {
                VoltageUnit.Nanovolts => value / 1e9,
                VoltageUnit.Microvolts => value / 1e6,
                VoltageUnit.Millivolts => value / 1000,
                VoltageUnit.Volts => value,
                VoltageUnit.Kilovolts => value * 1000,
                VoltageUnit.Megavolts => value * 1e6,
                _ => throw new ArgumentException("Invalid voltage unit")
            };
        }

        // Operators
        public static Voltage operator +(Voltage left, Voltage right) => new(left._volts + right._volts, VoltageUnit.Volts);
        public static Voltage operator -(Voltage left, Voltage right) => new(left._volts - right._volts, VoltageUnit.Volts);
        public static Voltage operator *(Voltage voltage, double multiplier) => new(voltage._volts * multiplier, VoltageUnit.Volts);
        public static Voltage operator /(Voltage voltage, double divisor) => new(voltage._volts / divisor, VoltageUnit.Volts);

        // Comparison operators
        public static bool operator ==(Voltage left, Voltage right) => Math.Abs(left._volts - right._volts) < 1e-10;
        public static bool operator !=(Voltage left, Voltage right) => !(left == right);
        public static bool operator <(Voltage left, Voltage right) => left._volts < right._volts;
        public static bool operator >(Voltage left, Voltage right) => left._volts > right._volts;
        public static bool operator <=(Voltage left, Voltage right) => left._volts <= right._volts;
        public static bool operator >=(Voltage left, Voltage right) => left._volts >= right._volts;

        // Implicit conversion from double (assumes volts)
        public static implicit operator Voltage(double volts) => new(volts, VoltageUnit.Volts);
        public static implicit operator double(Voltage voltage) => voltage._volts;

        public bool Equals(Voltage other) => this == other;
        public override bool Equals(object obj) => obj is Voltage voltage && Equals(voltage);
        public override int GetHashCode() => _volts.GetHashCode();
        public int CompareTo(Voltage other) => _volts.CompareTo(other._volts);

        public override string ToString() => $"{_volts:F3} V";
        public string ToString(VoltageUnit unit) => $"{GetValue(unit):F3} {GetUnitSymbol(unit)}";

        private static string GetUnitSymbol(VoltageUnit unit) => unit switch
        {
            VoltageUnit.Nanovolts => "nV",
            VoltageUnit.Microvolts => "μV",
            VoltageUnit.Millivolts => "mV",
            VoltageUnit.Volts => "V",
            VoltageUnit.Kilovolts => "kV",
            VoltageUnit.Megavolts => "MV",
            _ => "V"
        };
    }
}
